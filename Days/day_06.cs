using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AoC_2024;

internal class Day_06 : IDay
{
    public Result Solve()
    {
        var sw = Stopwatch.StartNew();
        sw.Start();

        //Read input
        var md = ReadFileData();
        //carry out tasks
        var part01 = SolvePart1(md);
        var part02 = SolvePart2(md);

        sw.Stop();

        var time = sw.ElapsedMilliseconds / 1000.0;
        return new Result("6.Guard Gallivant", part01, part02, time);
    }


    //Input is a map of  '.' and '#' with '^' as the starting location
    //Only cardinal travel directions are allowed, keep two dictionaries of (x, set<y>) and (y, <set<x>)
    //keep two vars for the location of the grid size and the player location
    private MapData ReadFileData()
    {
        MapData md = new();
        using (var read = new StreamReader("./input/day_06.txt"))
        {
            int y = 0;
            for (var line = read.ReadLine(); line != null; line = read.ReadLine())
            {
                md.mapWidth = line.Length;
                md.yObstacles.Add(y, new List<int>());
                for (int x = 0; x < line.Length; ++x)
                {
                    
                    if ( !md.xObstacles.ContainsKey(x) )
                    {
                        md.xObstacles.Add(x, new List<int>());
                    }
                    if (line[x] == '#')
                    {
                        
                        md.xObstacles[x].Add(y);
                        md.yObstacles[y].Add(x);
                        
                    }
                    else if (line[x] == '^')
                    {
                        md.startX = x;
                        md.startY = y;
                    }
                }
                ++y;
            }

            md.mapHeight = y;
        }

        return md;
    }

    int GetDistanceToNearestObstacle(MapData md, int x, int y, int dir)
    {
        //choose the right list
        var obs = md.xObstacles;
        var key = x;
        var val = y;
        var mapSize = md.mapWidth;
        if (dir == 1 || dir == 3)
        {
            obs = md.yObstacles;
            key = y;
            val = x;
            mapSize = md.mapHeight;
        }

        var obstacleList = obs[key];
        if (dir == 0 || dir == 3)
        {
            return val - obstacleList.LastOrDefault(v => v < val, -1) - 1;
        }
        return obstacleList.FirstOrDefault(v => v > val, mapSize) - val - 1;
        
    }

    private int SolvePart1(MapData md)
    {
        var px = md.startX;
        var py = md.startY;
        var di = 0;
        
        md.visitedTiles.Add(new(px, py));
        while (px > 0 && px < md.mapWidth - 1 && py > 0 && py < md.mapHeight - 1)
        {
            int distanceToObstacle = GetDistanceToNearestObstacle(md, px, py, di);
            //move the player in direction until they hit an obstacle, then turn right
            for (var i = 0; i < distanceToObstacle; ++i)
            {
                md.visitedTiles.Add(new(px += _directions[di].x, py += _directions[di].y));
            }
            di = (di + 1) % 4;
        }
        //add last overlap point
        return md.visitedTiles.Count;
    }

    private int SolvePart2(MapData md )
    {
        //add in a blocker at indexes in the has set and see if a loop is present in the new map
        var count = 0;
        Tuple<int, int> previousTile = new(0,0);
        foreach (var location in md.visitedTiles)
        {
            //skip first tuple
            if (location.Item1 == md.startX && location.Item2 == md.startY)
            {
                previousTile = location;
                continue;
            }
            //add an item to the map.
            md.xObstacles[location.Item1].Add(location.Item2);
            md.xObstacles[location.Item1] = md.xObstacles[location.Item1].OrderBy(num => num).ToList();

            md.yObstacles[location.Item2].Add(location.Item1);
            md.yObstacles[location.Item2] = md.yObstacles[location.Item2].OrderBy(num => num).ToList();

            //move the elf around the map again, but this time the tuple stores direction
            //get the starting position from the previous location on the map and work out the direction from that tile to the current location
            var px = previousTile.Item1;
            var py = previousTile.Item2;
            var di = 0;
            //work out previous direction
            var previousXDir = location.Item1 - previousTile.Item1;
            if (previousXDir != 0)
            {
                di = previousXDir < 0 ? 3 : 1;
            }
            else
            {
                var previousYDir = location.Item2 - previousTile.Item2;
                if (previousYDir != 0)
                {
                    di = previousYDir < 0 ? 0 : 2;
                }
            }
            previousTile = location;
            
            HashSet<Tuple<int, int, int>> reVisited = new() { };

            while (px > 0 && px < md.mapWidth - 1 && py > 0 && py < md.mapHeight - 1)
            {
                int distanceToObstacle = GetDistanceToNearestObstacle(md, px, py, di);
                //move the player in direction until they hit an obstacle, then turn right
                if (!reVisited.Add(new(px += _directions[di].x * distanceToObstacle, py += _directions[di].y * distanceToObstacle, di)))
                {
                    ++count;
                    break;
                }
                di = (di + 1) % 4;
            }

            //remove the obstacle from the map
            md.xObstacles[location.Item1].Remove(location.Item2);
            md.yObstacles[location.Item2].Remove(location.Item1);
            

        }
        return count;
    }

    class MapData
    {
        public Dictionary<int, List<int>> xObstacles = new Dictionary<int, List<int>>();
        public Dictionary<int, List<int>> yObstacles = new Dictionary<int, List<int>>();

        public int mapWidth = 0;
        public int mapHeight = 0;

        public int startX = 0;
        public int startY = 0;

        public HashSet<Tuple<int, int>> visitedTiles= new HashSet<Tuple<int, int>>();

    }

    

struct Direction
    {
        public int x;
        public int y;
    }

    private readonly Direction[] _directions =
    {
        new() { x = 0, y = -1 }, new() { x = 1, y = 0 }, new() { x = 0, y = 1 }, new() { x = -1, y = 0 }
    };

}