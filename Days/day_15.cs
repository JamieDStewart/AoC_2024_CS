using System;
using System.Data.Common;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic.CompilerServices;

namespace AoC_2024;

internal class Day_15 : IDay
{
    public Result Solve()
    {
        var sw = Stopwatch.StartNew();
        sw.Start();

        //Read input
        var map = ReadFileData();
        //carry out tasks
        var part01 = SolvePart1(map);
        //re-read in file data as map is modified by part 1
        map = ReadFileData();
        var part02 = SolvePart2(map);

        sw.Stop();

        var time = sw.ElapsedMilliseconds / 1000.0;
        return new Result("15.Warehouse Woes", part01, part02, time);
    }



    private Map ReadFileData()
    {
        //Each line of file is "p=0,4 v=3,-3" p is position, v is velocity
        var m = new Map();
        var readDirections = false;
        using (var read = new StreamReader("./input/day_15.txt"))
        {
            var yPos = 0;
            for (var line = read.ReadLine(); line != null; line = read.ReadLine())
            {
                if (line.Length == 0)
                {                    
                    readDirections = true;
                }
                //Process reading in the map data
                if( !readDirections )
                {
                    char[] arr = line.ToCharArray(0, line.Length);
                    if( line.Contains("@") )
                    {
                        var i = line.IndexOf("@");
                        m.currentPos = (i, yPos);
                        m.startingPos = (i, yPos);
                        arr[i] = '.';
                    }
                    m.grid.Add(arr);
                    ++yPos;
                }
                else
                {
                    m.commands += line;
                }
                
            }
            m.gridBounds = (m.grid[0].Length, yPos);
        }
        return m;
    }

    private long SolvePart1(Map map)
    {
        //Using the directions move the robot through the warehouse
        //robots can push boxes of any mass if there is a space beyond to facilitate the move
        foreach( var c in map.commands)
        {
            var d = (c == '^') ? 0 : -1;
            d = (c == '>') ? 1 : d;
            d = (c == 'v') ? 2 : d;
            d = (c == '<') ? 3 : d;
            var dir = _directions[d];

            var mp = map.currentPos;
            char v = map.grid[mp.Item2][mp.Item1];
           
            mp = (mp.Item1 + dir.x, mp.Item2 + dir.y);
            v = map.grid[mp.Item2][mp.Item1];

            if( v == '.' ) //Just move the damn robot Shinji
            {
                map.currentPos = (mp.Item1, mp.Item2);
            }
            //next item is a barrell can we push it?
            else if(v == 'O')
            {
                var relocatePos = mp;
                while (v != '#')
                {
                    mp = (mp.Item1 + dir.x, mp.Item2 + dir.y); // move to next square
                    v = map.grid[mp.Item2][mp.Item1];
                    if( v == '.')
                    {
                        map.grid[mp.Item2][mp.Item1] = 'O';
                        map.grid[relocatePos.Item2][relocatePos.Item1] = '.';
                        map.currentPos = relocatePos;
                        break;
                    }
                }
            }
        }
        
        long part1Total = 0;
        //walk the map and get teh values for all the locations of 
        for (var y = 0; y < map.gridBounds.Item2; ++y)
        {
            for (var x = 0; x < map.gridBounds.Item1; ++x)
            {
                if (map.grid[y][x] == 'O' )
                {
                    part1Total += y * 100 + x;
                }
            }
        }
        return part1Total;
    }

    private long SolvePart2(Map map)
    {
        //Map needs to be resized according to the rules of the question
        for( var row = 0; row < map.grid.Count; ++row )
        {
            var rowString = "";
            foreach( var c in map.grid[row])
            {
                rowString += (c switch
                {
                    '#' => "##",
                    'O' => "[]",
                    '.' => "..",
                    _ => ""
                });
            }
            map.grid[row] = rowString.ToCharArray();
        }
        map.startingPos = (map.startingPos.Item1 * 2, map.startingPos.Item2);
        map.currentPos = map.startingPos;
        //Using the directions move the robot through the warehouse
        //robots can push boxes of any mass if there is a space beyond to facilitate the move
        foreach (var c in map.commands)
        {
            var d = (c == '^') ? 0 : -1;
            d = (c == '>') ? 1 : d;
            d = (c == 'v') ? 2 : d;
            d = (c == '<') ? 3 : d;
            var dir = _directions[d];

            var cp = map.currentPos;
            char v = map.grid[cp.Item2][cp.Item1];

            cp = (cp.Item1 + dir.x, cp.Item2 + dir.y);
            v = map.grid[cp.Item2][cp.Item1];

            if (v == '.') //Just move the damn robot Shinji
            {
                map.currentPos = (cp.Item1, cp.Item2);
            }
            //next item is a barrell can we push it?
            else if (v == '[' || v == ']')
            {
                //build a chain of move positions navigating along each effected object until enough spaces are found
                var moveList = new List<(int, int)>() { cp };
                if (d == 0 || d == 2) //if moving vertical add the other half of the box
                {
                    if (v == '[') //add the other half of the box
                    {
                        moveList.Add((cp.Item1 + 1, cp.Item2));
                    }
                    else
                    {
                        moveList.Add((cp.Item1 - 1, cp.Item2));
                    }
                }
                //iterate over the list of objects getting pushed, if we find another object to move then add it to the list
                //if we reach the end of the list then the move is valid
                for (int i = 0; i < moveList.Count; i++)
                {
                    
                    var mp = (moveList[i].Item1 + dir.x, moveList[i].Item2 + dir.y); // move to next square in x move twice!
                    var nv = map.grid[mp.Item2][mp.Item1];
                    if( nv == '#') //Move not possible break out of this and empty the move list
                    {
                        moveList.Clear();
                        break;
                    }
                    if( nv == '[' || nv == ']') //found a new object to add to the move list
                    {
                        if (!moveList.Contains(mp))
                        {
                            moveList.Add(mp);
                        }
                        if (d == 0 || d == 2) //if moving vertical add the other half of the box
                        {
                            var cv = map.grid[moveList[i].Item2][moveList[i].Item1];
                            if (nv == '[' && cv == ']') //add the other half of the box
                            {
                                if (!moveList.Contains((mp.Item1 + 1, mp.Item2)))
                                    moveList.Add((mp.Item1 + 1, mp.Item2));
                            }
                            else if(nv == ']' && cv == '[')
                            {
                                if (!moveList.Contains((mp.Item1 - 1, mp.Item2)))
                                    moveList.Add((mp.Item1 - 1, mp.Item2));
                            }
                        }
                    }
                    // this could be a valid move don't add anything just keep iterating over the list
                }
                if (moveList.Count > 0)
                {
                    moveList.Reverse(); //reverse the list and go through it in reverse order
                    foreach (var move in moveList)
                    {
                        var cc = map.grid[move.Item2][move.Item1];
                        var np = (move.Item1 + dir.x, move.Item2 + dir.y);
                        map.grid[move.Item2][move.Item1] = '.';
                        map.grid[np.Item2][np.Item1] = cc;
                    }
                    cp = map.currentPos;
                    cp = (cp.Item1 + dir.x, cp.Item2 + dir.y);
                    map.currentPos = cp;
                }
            }
                       
        }

        long part2Total = 0;
        //walk the map and get the values for all the locations of 
        for (var y = 0; y < map.grid.Count; ++y)
        {
            for (var x = 0; x < map.grid[y].Length; ++x)
            {
                if (map.grid[y][x] == '[')
                {
                    part2Total += y * 100 + x;
                }
            }
        }
        return part2Total;
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

    class Map
    {
        public List<char[]> grid = new List<char[]>();
        public (int, int) gridBounds = new(0,0);
        public (int, int) currentPos = new(0, 0);
        public (int, int) startingPos = new(0, 0);
        public string commands = new("");
    }   

}