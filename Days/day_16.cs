using System.Diagnostics;

namespace AoC_2024;

internal class Day_16 : IDay
{
    private readonly Direction[] _directions =
    {
        new() { x = 0, y = -1 }, new() { x = 1, y = 0 }, new() { x = 0, y = 1 }, new() { x = -1, y = 0 }
    };

    private (int, int) _endPoint = (0, 0);

    private readonly List<char[]> _map = new();
    private (int, int) _startPoint = (0, 0);

    public Result Solve()
    {
        var sw = Stopwatch.StartNew();
        sw.Start();

        //Read input
        ReadFileData();
        //carry out tasks
        var part01 = SolvePart1();
        //re-read in file data as map is modified by part 1
        var part02 = SolvePart2(part01);

        sw.Stop();

        var time = sw.ElapsedMilliseconds / 1000.0;
        return new Result("16.Reindeer Maze", part01[0].Item3, part02, time);
    }


    private void ReadFileData()
    {
        //Each line of file is "p=0,4 v=3,-3" p is position, v is velocity
        var readDirections = false;
        using (var read = new StreamReader("./input/day_16.txt"))
        {
            var yPos = 0;
            for (var line = read.ReadLine(); line != null; line = read.ReadLine())
                //Process reading in the map data
                if (!readDirections)
                {
                    var arr = line.ToCharArray(0, line.Length);
                    if (line.Contains("S"))
                    {
                        var i = line.IndexOf("S");
                        _startPoint = (i, yPos);
                    }

                    if (line.Contains("E"))
                    {
                        var i = line.IndexOf("E");
                        _endPoint = (i, yPos);
                    }

                    _map.Add(arr);
                    ++yPos;
                }
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

    private List<(int, int, int)> SolvePart1()
    {
        //This is pretty much dijkstra's algo cost per tile are 1000 per directional change and 1 per step
        var currentDir = _directions[1];
        //tuple is x, y, cost, parentX, parentY
        var openList = new List<(int, int, int, int, int)> { (_startPoint.Item1, _startPoint.Item2, 0, -1, -1) };
        var closedList = new List<(int, int)>(); //to improve speed of detecting an item has been visited use this close list 

        var visitedList = new List<(int, int, int)>();

        while (openList.Count > 0)
        {
            //walk the maze
            var o = openList[0];
            var currentPos = (o.Item1, o.Item2);
            if (currentPos == _endPoint) //we have made it to the destination
            {
                visitedList.Add((o.Item1, o.Item2, o.Item3));
                closedList.Add((o.Item1, o.Item2));
                break;
            }

            if (o.Item4 != -1) currentDir = new Direction { x = o.Item1 - o.Item4, y = o.Item2 - o.Item5 };
            var neighbours = GetNeighbours((openList[0].Item1, openList[0].Item2));
            foreach (var n in neighbours)
                if (!closedList.Contains(n)) //if we haven't been to this neighbour already
                {
                    //get the cost of each neighbour based off direction
                    //cost of moving 1 step is 1 + distance to end
                    var cost = 1 + o.Item3;
                    //Add the direction to the cost
                    Direction nDir = new() { x = n.Item1 - o.Item1, y = n.Item2 - o.Item2 };
                    var dirCompound = nDir.x * currentDir.x + nDir.y * currentDir.y;
                    if (dirCompound == 0) cost += 1000; //1 turn

                    if (dirCompound == -1) cost += 2000; //2 turns

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

            openList.RemoveAt(0); //remove first item in open list and add to closed list
            visitedList.Add((o.Item1, o.Item2, o.Item3));
            closedList.Add((o.Item1, o.Item2));

            openList.Sort((x, y) => x.Item3.CompareTo(y.Item3));
        }

        //now walk the visited list navigating back through each parent to the starting node accumulating the cost
        visitedList.Reverse();

        return visitedList;
    }

    private long SolvePart2(List<(int, int, int )> pathList)
    {
        //walk through the path list looking for neighbours with the right cost
        var tilesOnPath = new HashSet<(int, int)> { (pathList[0].Item1, pathList[0].Item2) };
        var openList = new List<(int, int, int, int)> //pos x, pos y, cost, parent cost
        {
            (pathList[0].Item1, pathList[0].Item2, pathList[0].Item3, -1)
        }; 

        while (openList.Count > 0)
        {
            var neighbours = GetNeighbours((openList[0].Item1, openList[0].Item2));
            foreach (var n in neighbours)
            {
                var cost = openList[0].Item3 - 1;
                var turnCost = cost - 1000;
                for (var i = tilesOnPath.Count; i < pathList.Count; ++i)
                {
                    var opos = (pathList[i].Item1, pathList[i].Item2);
                    if (opos == n)
                    {
                        var parentCost = openList[0].Item4; // to account for corners test against parent and current for a value of 2?
                        if (cost == pathList[i].Item3 || pathList[i].Item3 == turnCost || parentCost - pathList[i].Item3 == 2 )
                        {
                            openList.Add((pathList[i].Item1, pathList[i].Item2, pathList[i].Item3, openList[0].Item3));
                            break;
                        }
                    }
                }
            }

            tilesOnPath.Add((openList[0].Item1, openList[0].Item2));
            openList.RemoveAt(0);
        }

        return tilesOnPath.Count;
    }

    private struct Direction
    {
        public int x;
        public int y;
    }
}