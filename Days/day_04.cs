﻿
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AoC_2024
{
    class Day_04 : IDay
    {
        class WordSearch
        {
            public List<string> rows = new();
        }
        private WordSearch ReadFileData()
        {
            WordSearch input = new();
            using (var read = new StreamReader("./input/day_04.txt"))
            {

                for (var line = read.ReadLine(); line != null; line = read.ReadLine())
                {
                    input.rows.Add(line);
                }
            }
            return input;
        }

        struct Direction
        {
            public int x;
            public int y;
        }

        private Direction[] directions = {  new Direction{x = 0, y = -1}, new Direction { x = 1, y = -1 },new Direction { x = 1, y = 0 }, new Direction { x = 1, y = 1 },
                                            new Direction { x = 0, y = 1 }, new Direction { x = -1, y = 1 }, new Direction { x = -1, y = 0 }, new Direction { x = -1, y = -1 }};

        //Input is just a vector of strings read directly from the day's input text file
        private long SolvePart1(WordSearch ws)
        {
            //Need to count occurrences of substring XMAS in input
            long count = 0;
            const string xmas = "XMAS";
            //move along each row looking for occurrences of X then look for rest of xmas string in all directions
            for( var r = 0; r < ws.rows.Count; ++r )
            {
                for (var c = 0; c < ws.rows[r].Length; ++c)
                {
                    var x = 0;
                    if (ws.rows[r][c] == xmas[x] ) //we have an 'x' look in all directions for rest of sequence
                    {
                        foreach (var direction in directions)
                        {
                            x = 1;
                            var nc = c + direction.x;
                            var nr = r + direction.y;
                            while(nc >= 0 && nc < ws.rows[r].Length && nr >= 0 && nr < ws.rows.Count) //valid grid location
                            {
                                if (ws.rows[nr][nc] == xmas[x])
                                {
                                    x++;
                                    if (x == xmas.Length) //found whole xmas
                                    {
                                        count++;
                                        break;
                                    }
                                    //move to next token
                                    nc += direction.x;
                                    nr += direction.y;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            

            return count;
        }

        struct PairedDirection
        {
            public List<Direction> dirs;
            
            public PairedDirection(Direction a, Direction b)
            {
                dirs = new List<Direction> { a, b };
            }
        }

        private PairedDirection[] pairedDirections =
        {
            new(new Direction { x =  1, y = -1 }, new Direction { x =  1, y =  1 }), // NE & SE
        };

        private long SolvePart2(WordSearch ws)
        {
            //Need to count occurrences of substring MAS crossed with MAS where the A is shared between them
            long count = 0;
            const char A = 'A'; 
            //move along each row looking for occurrences of A then look for rest of MAS string in all directions
            for (var r = 0; r < ws.rows.Count; ++r)
            {
                for (var c = 0; c < ws.rows[r].Length; ++c)
                {
                    if (ws.rows[r][c] == A) //we have an 'A' look in all directions for rest of sequence
                    {
                        foreach (var pd in pairedDirections)
                        {
                            var match = 0;
                            foreach (var d in pd.dirs)
                            {
                                var mc = c - d.x; //'M' row location
                                var mr = r - d.y;
                                var sc = c + d.x; //'S' row location
                                var sr = r + d.y;

                                if (mc >= 0 && mc < ws.rows[r].Length && mr >= 0 && mr < ws.rows.Count &&
                                    sc >= 0 && sc < ws.rows[r].Length && sr >= 0 && sr < ws.rows.Count) //valid grid location
                                {
                                    if (ws.rows[mr][mc] == 'M' && ws.rows[sr][sc] == 'S' ||
                                        ws.rows[mr][mc] == 'S' && ws.rows[sr][sc] == 'M' )
                                    {
                                        match++;
                                    }
                                }
                            }

                            if (match == 2)
                            {
                                ++count;
                            }
                        }
                    }
                }
            }


            return count;
        }
        public Result Solve()
        {
            Stopwatch sw = Stopwatch.StartNew();
            sw.Start();
            //Read input
            WordSearch ws = ReadFileData();
            //carry out tasks
            var part_01 = SolvePart1(ws);
            var part_02 = SolvePart2(ws);

            sw.Stop();

            var time = sw.ElapsedMilliseconds / 1000.0;
            return new Result(name: "4.Ceres Search", part_01: part_01, part_02: part_02, execution_time: time);
        }
    }
}

