﻿using System.Diagnostics;

namespace AoC_2024;

internal class Day_18 : IDay
{
    private readonly Direction[] _directions =
    {
        new() { x = 0, y = -1 }, new() { x = 1, y = 0 }, new() { x = 0, y = 1 }, new() { x = -1, y = 0 }
    };

    private List<(int x, int y)> _bytes = new();
    public Result Solve()
    {
        var sw = Stopwatch.StartNew();
        sw.Start();

        //Read input
        ReadFileData();
        //carry out tasks
        var part01 = SolvePart1();
        var part02 = SolvePart2();
        var part2String = $"{part02.Item1},{part02.Item2}";
        sw.Stop();

        var time = sw.ElapsedMilliseconds / 1000.0;
        return new Result("18.RAM Run", part01.ToString(), part2String, time);
    }

    private readonly List<char[]> _map = new();
    private int _mapSize = 71;
    private void ReadFileData()
    {
        //construct the empty_map
        
        for( var y =  0; y < _mapSize; y++)
        {
            _map.Add(Enumerable.Repeat('.', _mapSize).ToArray());
        }
        //Each line of file is "p=0,4 v=3,-3" p is position, v is velocity
        using (var read = new StreamReader("./input/day_18.txt"))
        {
            for (var line = read.ReadLine(); line != null; line = read.ReadLine())
            {
                //Process reading in the falling bytes
                var coordinates = line.Split(',');
                _bytes.Add((int.Parse(coordinates[0]), int.Parse(coordinates[1]) ));
            }
        }

        //populate the map with x number of memory blocks
        for (int i = 0; i < 1024; ++i)
        {
            var (x, y) = _bytes[i];
            _map[y][x] = '#';
        }
    }

    

    private List<(int, int)> GetNeighbours((int, int) point)
    {
        var neighbours = new List<(int, int)>();
        foreach (var d in _directions)
        {
            var nx = point.Item1 + d.x;
            var ny = point.Item2 + d.y;

            if (nx >= 0 && nx < _map[0].Length &&
                ny >= 0 && ny < _map.Count)
                //next point inside map boundary and not wall
                if (_map[ny][nx] != '#')
                    //next point is valid
                    neighbours.Add((nx, ny));
        }

        return neighbours;
    }

    private List<(int, int,int,int)> _ParentList = new List<(int, int, int, int)>();
    private long SolvePart1(int byteCount = 1024)
    {
        _ParentList.Clear();
        
        long totalCost = 0;
        //tuple is x, y, cost
        var openList = new List<(int, int, int, int, int)>() { (0, 0, 0, -1, -1) };
        
        var closedList = new List<(int, int)>(); //to improve speed of detecting an item has been visited use this close list 
        var endPoint = (_mapSize - 1, _mapSize - 1);
        while (openList.Count > 0)
        {
            //walk the maze
            var o = openList[0];
            openList.RemoveAt(0);
            _ParentList.Add((o.Item1, o.Item2, o.Item4, o.Item5));
            closedList.Add((o.Item1, o.Item2));
            var currentPos = (o.Item1, o.Item2);
            if (currentPos == endPoint) //we have made it to the destination
            {
                totalCost = o.Item3;
                break;
            }

            var neighbours = GetNeighbours((o.Item1, o.Item2));
            foreach (var n in neighbours)
                if (!closedList.Contains(n)) //if we haven't been to this neighbour already
                {
                    var cost = 1 + o.Item3;
                    //find in open list
                    var located = false;
                    for (var i = 1; i < openList.Count; ++i)
                    {
                        var opos = (openList[i].Item1, openList[i].Item2);
                        if (opos == n)
                        {
                            located = true;
                            if (cost < openList[i].Item3)
                            {
                                openList.RemoveAt(i);
                                //add to the open list
                                openList.Add((n.Item1, n.Item2, cost, o.Item1, o.Item2));
                                break;
                            }
                        }
                    }

                    if (located == false)
                        //add to the open list
                        openList.Add((n.Item1, n.Item2, cost, o.Item1, o.Item2));
                    
                }
            openList.Sort((x, y) => x.Item3.CompareTo(y.Item3));
        }
        return totalCost;
    }

    private List<(int,int)> GetPath()
    {
        _ParentList.Reverse();
        var parent = _ParentList[0];
        var path = new List<(int, int)>();
        while( parent.Item3 != -1 )
        {
            path.Add( (parent.Item1, parent.Item2) );
            var l = _ParentList.IndexOf(parent);
            //get next parent
            for (var p = l; p < _ParentList.Count; ++p)
            {
                if ((_ParentList[p].Item1, _ParentList[p].Item2) == (parent.Item3, parent.Item4))
                {
                    parent = _ParentList[p];
                    break;
                }
            }
        }
        path.Add((parent.Item1, parent.Item2));
        return path;
        
    }
    private (int,int) SolvePart2()
    {
        var path = GetPath();
        for( int i = 1024; i < _bytes.Count; ++i)
        {
            var(x,y) = _bytes[i];
            _map[y][x] = '#';
            if (path.Contains((x, y))) ;
            {
                //dropped tile on the path re-acquire path if possible
                if( SolvePart1(i) == 0 )
                {
                    //no path possible
                    return _bytes[i];
                }
                path = GetPath();
            }
        }
        return (-1,-1);
    }



    private struct Direction
    {
        public int x;
        public int y;
    }
}