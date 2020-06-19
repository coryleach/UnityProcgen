using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.Ast;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameframe.Procgen
{
    public class WorldMapBorderView : MonoBehaviour, IWorldMapView
    {
        [SerializeField]
        private LineRenderer prefab = null;
        
        [SerializeField]
        private Vector3 offset;
        
        [SerializeField] 
        private Color[] regionColors = new Color[0];

        [FormerlySerializedAs("lines")] [SerializeField]
        private List<LineRenderer> lineRenderers = new List<LineRenderer>();

        [SerializeField] private int minLineLength = 2;
        
        private void Start()
        {
            if (prefab != null)
            {
                prefab.gameObject.SetActive(false);
            }
        }

        [ContextMenu("Clear Lines")]
        public void ClearLines()
        {
            foreach (var line in lineRenderers)
            {
                if (Application.isPlaying)
                {
                    Destroy(line.gameObject);
                }
                else
                {
                    DestroyImmediate(line.gameObject);

                }
            }
            lineRenderers.Clear();
        }
        
        public void DisplayMap(WorldMapData mapData)
        {
            if (!enabled || prefab == null)
            {
                return;
            }
            
            prefab.gameObject.SetActive(false);

            ClearLines();
            
            var positionOffset = new Vector3(mapData.width*-0.5f,0, mapData.height*0.5f) + offset;
            
            var regionLayer = mapData.GetLayer<RegionMapLayerData>();
            foreach (var region in regionLayer.regions)
            {
                //Each region could have more than one border line to draw
                //Border point list is unordered so we need to also figure out what the lines are
                
                //Convert border points to 3D space
                var points = new List<Vector3>(region.borderPoints.Select((pt) => new Vector3(pt.x,0,-pt.y) + positionOffset));

                //Extract lines from list of points by getting closest points
                var lines = new List<List<Vector3>>();
                var currentLine = new List<Vector3>();
                lines.Add(currentLine);
                var currentPt = points[0];
                points.RemoveAt(0);
                while (points.Count > 0)
                {
                    float minDistance = float.MaxValue;
                    int minIndex = -1;
                    
                    for (int i = 0; i < points.Count; i++)
                    {
                        var distance = (currentPt - points[i]).sqrMagnitude;
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            minIndex = i;
                        }
                    }

                    
                    if (minDistance > 2)
                    {
                        //Start a new line if the closest points was more than a certain distance away
                        lines.Add(currentLine);
                        currentLine = new List<Vector3>();
                    }

                    currentPt = points[minIndex];
                    currentLine.Add(points[minIndex]);
                    points.RemoveAt(minIndex);
                }
                
                var regionColor = regionColors.Length > 0 ? regionColors[(region.id - 1) % regionColors.Length] : Color.white;

                foreach (var line in lines)
                {
                    //Don't draw anything with less than 2 points
                    if (line.Count < minLineLength)
                    {
                        continue;
                    }
                    
                    var lineRenderer = Instantiate(prefab, transform);
                    lineRenderer.transform.localEulerAngles = new Vector3(90,0,0);
                    lineRenderer.startColor = regionColor;
                    lineRenderer.endColor = regionColor;
                
                    //lineRenderer.positionCount = region.borderPoints.Count;
                    lineRenderer.positionCount = line.Count;
                    lineRenderer.SetPositions(line.ToArray());

                    var startPt = line[0];
                    var endPt = line[line.Count - 1];
                    //If the end is close enough to the start then complete the loop
                    if ((startPt - endPt).sqrMagnitude <= 2)
                    {
                        lineRenderer.loop = true;
                    }
                    
                    lineRenderer.gameObject.SetActive(true);
                    lineRenderers.Add(lineRenderer);
                }
                
            }
        }
    }
}

