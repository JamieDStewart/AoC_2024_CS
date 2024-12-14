using System;
using System.Data.Common;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic.CompilerServices;

namespace AoC_2024;

internal class Day_12 : IDay
{
    public Result Solve()
    {
        var sw = Stopwatch.StartNew();
        sw.Start();

        //Read input
        ReadFileData();
        //carry out tasks
        var part01 = SolvePart1(_map);


        sw.Stop();

        var time = sw.ElapsedMilliseconds / 1000.0;
        return new Result("12.Garden Groups", part01, part01, time);
    }

    

    private void ReadFileData()
    {
        _map.data = new List<string>();
        using (var read = new StreamReader("./input/day_12.txt"))
        {
            _map.data = new List<string>();
            for (var line = read.ReadLine(); line != null; line = read.ReadLine())
            {
                _map.data.Add(line);
            }
        }
    }

    List<(int, int)> GetNeighbours((int, int) point, char val)
    {
        var neighbours = new List<(int, int)>();
        foreach (var d in _directions)
        {
            var nx = point.Item1 + d.x;
            var ny = point.Item2 + d.y;

            if (nx >= 0 && nx < _map.data[0].Length &&
                ny >= 0 && ny < _map.data.Count)
            {
                //next point inside map boundary
                if (_map.data[ny][nx] == val)
                {
                    //next point is valid
                    neighbours.Add((nx, ny));
                }
            }
        }
        return neighbours;
    }

    private long SolvePart1(Map map)
    {
        long part1Total = 0;
        var visitedPlots = new HashSet<(int, int)>();
        //From all starting location fan out north, east, south & west to find a path
        for (var r = 0; r < map.data.Count; ++r)
        {
            string row = map.data[r];
            for ( var c = 0; c < row.Length; ++c)
            {
                if ( !visitedPlots.Contains((c,r)))
                {
                    var plot = new List<(int, int)>();
                    var perimiter = 0;
                    var openList = new List<(int, int)> { (c, r) };
                    while (openList.Count > 0)
                    {
                        visitedPlots.Add(openList[0]); //we've visited this location now.
                        //get all valid neighbors
                        var neighbours = GetNeighbours(openList[0], row[c]);
                        perimiter += 4 - neighbours.Count;
                        plot.Add((openList[0].Item1, openList[0].Item2)); //add this to the list of locations in the current plot
                        foreach (var n in neighbours)
                        {
                            if (!visitedPlots.Contains(n) && !openList.Contains(n))
                            {
                                openList.Add(n);
                            }
                        }
                        openList.RemoveAt(0);
                    }

                    _plots[row[c]-'A'].Add((plot.Count, perimiter));
                    
                }
            }
        }

        foreach (var p in _plots)
        {
            foreach (var ap in p)
            {
                var( area,  perimeter) = ap;
                part1Total += area * perimeter;

            }
        }
        return part1Total;
    }

    private Map _map = new Map();
    private List<(int, int)>[] _plots = Enumerable.Range(0, 26).Select(_ => new List<(int, int)>()).ToArray();

    struct Map
    {
        public List<string> data;
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