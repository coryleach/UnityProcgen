using System;
using System.Collections.Generic;

namespace Gameframe.Procgen.Towngen
{
    public class Rectangle
    {
        public int X, Y, Width, Height;

        public Rectangle(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public int MinX => X;
        public int MaxX => X + Width;
        public int MinY => Y;
        public int MaxY => Y + Height;

        public bool Overlaps(Rectangle other)
        {
            return !(X + Width <= other.X || other.X + other.Width <= X || Y + Height <= other.Y || other.Y + other.Height <= Y);
        }
    }

    public class Building
    {
        public Rectangle Rect;
        public (int X, int Y) DoorPoint;

        public Building(Rectangle rect, (int X, int Y) doorPoint)
        {
            Rect = rect;
            DoorPoint = doorPoint;
        }
    }

    public class Road
    {
        public Rectangle Rect;

        public Road(Rectangle rect)
        {
            Rect = rect;
        }
    }

    public class TownGenerator
    {
        public List<Building> buildings = new List<Building>();
        public List<Road> roads = new List<Road>();
        public int townWidth;
        public int townHeight;

        public void Generate(int minBuildings, int maxBuildings, int seed)
        {
            Random random = new Random(seed);

            int numBuildings = random.Next(minBuildings, maxBuildings + 1);

            for (int i = 0; i < numBuildings; i++)
            {
                int width = random.Next(3, 10);
                int height = random.Next(3, 10);
                int x = random.Next(1, townWidth - width - 1);
                int y = random.Next(1, townHeight - height - 1);

                Rectangle buildingRect = new Rectangle(x, y, width, height);
                if (!CheckOverlap(buildingRect))
                {
                    (int doorX, int doorY) = GenerateDoorPoint(buildingRect, random);
                    buildings.Add(new Building(buildingRect, (doorX, doorY)));
                }
            }

            GenerateRoads();
        }

        private bool CheckOverlap(Rectangle newRect)
        {
            foreach (Building building in buildings)
            {
                if (building.Rect.Overlaps(newRect))
                    return true;
            }

            return false;
        }

        private (int, int) GenerateDoorPoint(Rectangle rect, Random random)
        {
            int doorX, doorY;

            switch (random.Next(4))
            {
                case 0: // Top edge
                    doorX = random.Next(rect.X + 1, rect.X + rect.Width - 1);
                    doorY = rect.Y;
                    break;
                case 1: // Right edge
                    doorX = rect.X + rect.Width;
                    doorY = random.Next(rect.Y + 1, rect.Y + rect.Height - 1);
                    break;
                case 2: // Bottom edge
                    doorX = random.Next(rect.X + 1, rect.X + rect.Width - 1);
                    doorY = rect.Y + rect.Height;
                    break;
                default: // Left edge
                    doorX = rect.X;
                    doorY = random.Next(rect.Y + 1, rect.Y + rect.Height - 1);
                    break;
            }

            return (doorX, doorY);
        }

        private void GenerateRoads()
{
    // Connect all buildings sequentially with L-shaped roads
    for (int i = 0; i < buildings.Count - 1; i++)
    {
        var door1 = buildings[i].DoorPoint;
        var door2 = buildings[i + 1].DoorPoint;

        // Horizontal road segment
        int roadX = Math.Min(door1.X, door2.X);
        int roadY = door1.Y;
        int roadWidth = Math.Abs(door1.X - door2.X);
        Rectangle horizontalRoadRect = new Rectangle(roadX, roadY, roadWidth, 1);

        // Vertical road segment
        roadX = door2.X;
        roadY = Math.Min(door1.Y, door2.Y);
        int roadHeight = Math.Abs(door1.Y - door2.Y);
        Rectangle verticalRoadRect = new Rectangle(roadX, roadY, 1, roadHeight);

        // Check if the horizontal road segment overlaps with any buildings
        bool horizontalOverlap = false;
        foreach (Building building in buildings)
        {
            if (building.Rect.Overlaps(horizontalRoadRect))
            {
                horizontalOverlap = true;
                break;
            }
        }

        // Check if the vertical road segment overlaps with any buildings
        bool verticalOverlap = false;
        foreach (Building building in buildings)
        {
            if (building.Rect.Overlaps(verticalRoadRect))
            {
                verticalOverlap = true;
                break;
            }
        }

        // If both road segments do not overlap with any buildings, add them to the roads list
        if (!horizontalOverlap && !verticalOverlap)
        {
            roads.Add(new Road(horizontalRoadRect));
            roads.Add(new Road(verticalRoadRect));
        }
        else
        {
            // If the L-shaped road overlaps any buildings, try another sequence
            if (i > 0)
            {
                i -= 2;
            }
            else
            {
                // If there's no previous sequence to try, generate a new layout
                //buildings.Clear();
                //roads.Clear();
                //Generate(minBuildings, maxBuildings, seed);
                return;
            }
        }
    }
}

    }
}
