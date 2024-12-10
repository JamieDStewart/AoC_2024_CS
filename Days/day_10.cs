using System;
using System.Data.Common;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic.CompilerServices;

namespace AoC_2024;

internal class Day_10 : IDay
{
    public Result Solve()
    {
        var sw = Stopwatch.StartNew();
        sw.Start();

        //Read input
        var map = ReadFileData();
        //carry out tasks
        var part01 = SolveBothParts(map);
        

        sw.Stop();

        var time = sw.ElapsedMilliseconds / 1000.0;
        return new Result("10.Hoof It", part01.Item1, part01.Item2, time);
    }

    private Map ReadFileData()
    {
        var map = new Map();
        using (var read = new StreamReader("./input/day_10.txt"))
        {
            map.data = new List<List<int>>();
            map.StartLocations = new List<Tuple<int, int>>();
            for (var line = read.ReadLine(); line != null; line = read.ReadLine())
            {
                var row = line.ToList().Select(p => p - '0').ToList();
                // any starting points in this row?
                var rsps = row.Select((x, i) => new { x, i })
                    .Where(x => x.x == 0)
                    .Select(x => x.i);
                foreach (var sp in rsps)
                {
                    map.StartLocations.Add( new Tuple<int, int>(sp, map.data.Count));
                }
                map.data.Add(row);
            }
        }
        return map;
    }

    List<Tuple<int, int>> GetNeighbours( List<List<int>> mapData, Tuple<int, int> point)
    {
        var neighbours = new List<Tuple<int, int>>();
        foreach (var d in _directions)
        {
            var nx = point.Item1 + d.x;
            var ny = point.Item2 + d.y;

            if (nx >= 0 && nx < mapData[0].Count &&
                ny >= 0 && ny < mapData.Count)
            {
                //next point inside map boundary
                if (mapData[ny][nx] - mapData[point.Item2][point.Item1] == 1)
                {
                    //next point is valid
                    neighbours.Add(new Tuple<int,int>(nx,ny));
                }
            }
        }
        return neighbours;
    }

    private (long, long) SolveBothParts(Map map)
    {
        long part1Total = 0;
        long part2Total = 0;
        //From all starting location fan out north, east, south & west to find a path
        foreach (var sp in map.StartLocations)
        {
            var validPaths = new HashSet<Tuple<int, int>>();
            var openList = new List<Tuple<int, int>>() { sp };
            while (openList.Count > 0)
            {
                if (map.data[openList[0].Item2][openList[0].Item1] == 9)
                {
                    //reached goal add this to a Hash Dict
                    validPaths.Add(new Tuple<int, int>(openList[0].Item2, openList[0].Item1));
                    ++part2Total;
                }
                else
                {
                    var neighbours = GetNeighbours(map.data, openList[0]);
                    openList.AddRange(neighbours);
                }
                openList.RemoveAt(0);
                //sort the list to the smallest value in map first
                openList.Sort((a, b) => map.data[a.Item2][a.Item1].CompareTo(map.data[b.Item2][b.Item1]));
            }

            part1Total += validPaths.Count;
        }
        return (part1Total, part2Total);
    }
    

    struct Map
    {
        public List<List<int>> data;
        public List<Tuple<int, int>> StartLocations;
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