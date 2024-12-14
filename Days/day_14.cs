using System;
using System.Data.Common;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic.CompilerServices;

namespace AoC_2024;

internal class Day_14 : IDay
{
    public Result Solve()
    {
        var sw = Stopwatch.StartNew();
        sw.Start();

        //Read input
        var robots = ReadFileData();
        //carry out tasks
        var part01 = SolvePart1(robots);
        var part02 = SolvePart2(robots);

        sw.Stop();

        var time = sw.ElapsedMilliseconds / 1000.0;
        return new Result("14.Restroom Redoubt", part01, part02, time);
    }



    private List<Robot> ReadFileData()
    {
        //Each line of file is "p=0,4 v=3,-3" p is position, v is velocity
        var robots = new List<Robot>();
        using (var read = new StreamReader("./input/day_14.txt"))
        {
            for (var line = read.ReadLine(); line != null; line = read.ReadLine())
            {
                if (line.Length > 0)
                {
                    //get the positive or negative integer values from the line
                    const string pattern = @"(-?\d+)";
                    Regex regex = new(pattern);
                    var matches = regex.Matches(line);
                    robots.Add(new() { Position = (int.Parse(matches[0].Value), int.Parse(matches[1].Value)), 
                                            Velocity = (int.Parse(matches[2].Value), int.Parse(matches[3].Value)) });
                    
                }
            }
        }

        return robots;
    }
    
    private long SolvePart1(List<Robot> robots)
    {
        var quadrants = (0L, 0L, 0L, 0L);
        var halfWidth = mapWidth / 2 ;
        var halfHeight = mapHeight / 2;
        foreach (var robot in robots)
        {
            //100 seconds of iteration
            var cycles = 100;
            var (x, y) = ( (robot.Position.Item1 + (robot.Velocity.Item1 * cycles)) % mapWidth,
                                    (robot.Position.Item2 + (robot.Velocity.Item2 * cycles)) % mapHeight);
            
            //adjust if heading in negative direction
            if( x < 0 ) { x = mapWidth + x; } if (y < 0) { y = mapHeight + y; }

            if ( x < halfWidth && y < halfHeight){ quadrants.Item1++; }
            if (x > halfWidth && y <  halfHeight) { quadrants.Item2++; }
            if (x < halfWidth && y > halfHeight) { quadrants.Item3++; }
            if (x > halfWidth && y > halfHeight) { quadrants.Item4++; }
        }

        long part1Total = quadrants.Item1 * quadrants.Item2 * quadrants.Item3 * quadrants.Item4;
        return part1Total;
    }

    private long SolvePart2(List<Robot> robots)
    {
        long part2Total = 0;
        var mostSeen = 0;
        //When the christmas tree is visible this requires the most number of robots to be visible
        var positions = new HashSet<(int, int)>();
        //take the highest number of different frames as mapWidth * mapHeight
        //After frames width*height all robots are back in their starting positions
        for (long c = 1; c <= mapHeight*mapWidth; ++c)
        {
            //create a grid for the robots
            foreach(var robot in robots)
            {
                var (x, y) = ( (robot.Position.Item1 + robot.Velocity.Item1 ) % mapWidth,
                                      (robot.Position.Item2 + robot.Velocity.Item2) % mapHeight);
                //adjust if heading in negative direction
                if (x < 0){ x = mapWidth + x; }
                if (y < 0){ y = mapHeight + y; }

                robot.Position.Item1 = x;
                robot.Position.Item2 = y;
                positions.Add((x, y));
            }

            var totalShown = positions.Count;
            if (totalShown > mostSeen)
            {
                mostSeen = totalShown;
                part2Total = c;
            }
            positions.Clear();
            if (totalShown == robots.Count) break;
        }

        return part2Total;
    }

    class Robot
    {
        public (int, int) Position = (0,0);
        public (int, int) Velocity = (0, 0);
    }

    private int mapWidth = 101;
    private int mapHeight = 103;

}