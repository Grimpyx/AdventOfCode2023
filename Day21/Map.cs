using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day21
{
    class Map
    {
        MapTile[][] mapTiles;
        (int x, int y) startCoord;
        PriorityQueue<(int x, int y)[], int> queue;
        private int step = 0;
        public int Distinct { get; private set; }

        public Map(MapTile[][] mapTiles, (int x, int y) startCoord)
        {
            this.mapTiles = mapTiles;
            this.startCoord = startCoord;

            queue = new PriorityQueue<(int x, int y)[], int>();
            queue.Enqueue([startCoord], 0);
        }

        public Map(MapTile[][] mapTiles)
        {
            this.mapTiles = mapTiles;
            this.startCoord = GetMidpoint();

            queue = new PriorityQueue<(int x, int y)[], int>();
            queue.Enqueue([startCoord], 0);
        }

        public void Step()
        {
            // step
            step++;

            // grab coordinates from queue
            var nextCoords = queue.Dequeue();

            // Find all neighbors of the current
            List<(int x, int y)> toAdd = new List<(int x, int y)>();
            foreach (var coord in nextCoords)
            {
                toAdd.AddRange(GetNeighbours(coord, mapTiles));
            }

            // Update map
            (int x, int y)[] distinct = toAdd.Distinct().ToArray();
            Distinct = distinct.Count();
            queue.EnqueueRange([distinct], step);

            foreach (var item in nextCoords)
            {
                mapTiles[item.y][item.x] = MapTile.Garden;
            }

            foreach (var item in distinct)
            {
                mapTiles[item.y][item.x] = MapTile.Step;
            }

            if (mapTiles[startCoord.y][startCoord.x] != MapTile.Step) mapTiles[startCoord.y][startCoord.x] = MapTile.Start;
        }

        private (int x, int y)[] GetNeighbours((int x, int y) coord, MapTile[][] map)
        {
            List<(int x, int y)> allNeighbours = new List<(int x, int y)>();

            (int x, int y) nextCoord;
            nextCoord = (coord.x + 1, coord.y);
            if (WithinBounds(nextCoord)) allNeighbours.Add(nextCoord);
            nextCoord = (coord.x, coord.y + 1);
            if (WithinBounds(nextCoord)) allNeighbours.Add(nextCoord);
            nextCoord = (coord.x - 1, coord.y);
            if (WithinBounds(nextCoord)) allNeighbours.Add(nextCoord);
            nextCoord = (coord.x, coord.y - 1);
            if (WithinBounds(nextCoord)) allNeighbours.Add(nextCoord);

            return allNeighbours.Where(c => map[c.y][c.x] != MapTile.Rock).ToArray();

            bool WithinBounds((int x, int y) cc)
            {
                if (cc.x < 0 || cc.y < 0 || cc.x >= map[0].Length || cc.y >= map.Length) return false;
                else return true;
            }
        }

        public (int x, int y) GetMidpoint()
        {
            if (mapTiles[0].Length % 2 != 1 || mapTiles.Length % 2 != 1) // if even number of tiles there is no midpoint
                return (-1, -1);
            return ((mapTiles[0].Length - 1) / 2, (mapTiles.Length - 1) / 2);
        }
    }
}
