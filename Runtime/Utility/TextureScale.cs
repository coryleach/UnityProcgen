using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace Gameframe.Procgen
{
  // Only works on ARGB32, RGB24 and Alpha8 textures that are marked readable
  public static class TextureScale
  {
    public class TaskData
    {
      public int start;
      public int end;

      public TaskData(int s, int e)
      {
        start = s;
        end = e;
      }
    }

    public static async Task PointAsync(Texture2D tex, int newWidth, int newHeight)
    {
      await ThreadedScaleAsync(tex, newWidth, newHeight, false);
    }

    public static async Task BilinearAsync(Texture2D tex, int newWidth, int newHeight)
    {
       await ThreadedScaleAsync(tex, newWidth, newHeight, true);
    }

    private static async Task ThreadedScaleAsync(Texture2D tex, int newWidth, int newHeight, bool useBilinear)
    {
      var texColors = tex.GetPixels();
      var newColors = new Color[newWidth * newHeight];
      
      float ratioX;
      float ratioY;
      if (useBilinear)
      {
        ratioX = 1.0f / ((float) newWidth / (tex.width - 1));
        ratioY = 1.0f / ((float) newHeight / (tex.height - 1));
      }
      else
      {
        ratioX = ((float) tex.width) / newWidth;
        ratioY = ((float) tex.height) / newHeight;
      }

      var width = tex.width;
      
      var cores = Mathf.Min(SystemInfo.processorCount, newHeight);
      var slice = newHeight / cores;
      
      var tasks = new List<Task>();
      
      if (cores > 1)
      {
        for (var i = 0; i < cores; i++)
        {
          var taskData = new TaskData(slice * i, slice * (i + 1));
          var task = Task.Run(() =>
          {
            if (useBilinear)
            {
              BilinearScale(taskData,texColors,newColors,width,newWidth,ratioX,ratioY);
            }
            else
            {
              PointScale(taskData,texColors,newColors,width,newWidth,ratioX,ratioY);
            }
          });
          tasks.Add(task);
        }
        await Task.WhenAll(tasks);
      }
      else
      {
        var taskData = new TaskData(0, newHeight);
        await Task.Run(() =>
        {
          if (useBilinear)
          {
            BilinearScale(taskData,texColors,newColors,width,newWidth,ratioX,ratioY);
          }
          else
          {
            PointScale(taskData,texColors,newColors,width,newWidth,ratioX,ratioY);
          }
        });
      }

      tex.Reinitialize(newWidth, newHeight);
      tex.SetPixels(newColors);
      tex.Apply();
    }

    private static void BilinearScale(TaskData taskData, Color[] texColors, Color[] newColors, int width, int newWidth, float ratioX, float ratioY)
    {
      for (var y = taskData.start; y < taskData.end; y++)
      {
        int yFloor = (int) Mathf.Floor(y * ratioY);
        var y1 = yFloor * width;
        var y2 = (yFloor + 1) * width;
        var yw = y * newWidth;

        for (var x = 0; x < newWidth; x++)
        {
          int xFloor = (int) Mathf.Floor(x * ratioX);
          var xLerp = x * ratioX - xFloor;
          newColors[yw + x] = ColorLerpUnclamped(
            ColorLerpUnclamped(texColors[y1 + xFloor], texColors[y1 + xFloor + 1], xLerp),
            ColorLerpUnclamped(texColors[y2 + xFloor], texColors[y2 + xFloor + 1], xLerp),
            y * ratioY - yFloor);
        }
      }
    }

    private static void PointScale(TaskData taskData, Color[] texColors, Color[] newColors, int width, int newWidth, float ratioX, float ratioY)
    {
      for (var y = taskData.start; y < taskData.end; y++)
      {
        var thisY = (int) (ratioY * y) * width;
        var yw = y * newWidth;
        for (var x = 0; x < newWidth; x++)
        {
          newColors[yw + x] = texColors[(int) (thisY + ratioX * x)];
        }
      }
    }

    private static Color ColorLerpUnclamped(Color c1, Color c2, float value)
    {
      return new Color(c1.r + (c2.r - c1.r) * value,
        c1.g + (c2.g - c1.g) * value,
        c1.b + (c2.b - c1.b) * value,
        c1.a + (c2.a - c1.a) * value);
    }
    
  }
}