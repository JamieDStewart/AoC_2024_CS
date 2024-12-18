using System;
using System.Data.Common;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic.CompilerServices;

namespace AoC_2024;

internal class Day_13 : IDay
{
    public Result Solve()
    {
        var sw = Stopwatch.StartNew();
        sw.Start();

        //Read input
        var rules = ReadFileData();
        //carry out tasks
        var part01 = SolvePart1(rules);
        var part02 = SolvePart2(rules);

        sw.Stop();

        var time = sw.ElapsedMilliseconds / 1000.0;
        return new Result("13.Claw Contraption", part01.ToString(), part02.ToString(), time);
    }



    private List<Rules> ReadFileData()
    {
        //Input Example
        //Button A: X+94, Y+34
        //Button B: X+22, Y+67
        //Prize: X =8400, Y=5400
        var rules = new List<Rules>();
        using (var read = new StreamReader("./input/day_13.txt"))
        {
            for (var line = read.ReadLine(); line != null; line = read.ReadLine())
            {
                if (line.Length > 0)
                {
                    //get the Button1 values from the line
                    const string pattern = @"(\d+)";
                    Regex regex = new(pattern);

                    var matches = regex.Matches(line);
                    var buttonA = (int.Parse(matches[0].Value), int.Parse(matches[1].Value));
                    line = read.ReadLine();
                    //get button 2 values
                    matches = regex.Matches(line);
                    var buttonB = (int.Parse(matches[0].Value), int.Parse(matches[1].Value)); ;
                    line = read.ReadLine();
                    //get prize location
                    matches = regex.Matches(line);
                    var prize = (int.Parse(matches[0].Value), int.Parse(matches[1].Value)); ;

                    rules.Add(new(){ButtonA = buttonA, ButtonB = buttonB, Prize = prize});
                }
            }
        }

        return rules;
    }

    struct Matrix
    {
        public double a; public double b; public double c; public double d;
    }

    private long SolvePart1(List<Rules> rules)
    {
        long part1Total = 0;
        foreach (var r in rules)
        {
            // m*x1 + n*x2 = p1
            // m*y1 + n*y2 = p2

            //This is a matrix problem we need to get the inverse of the matrix 
            // [ x1, x2 ]
            // [ y1, y2 ]
            Matrix m = new() { a = r.ButtonA.Item1, b = r.ButtonB.Item1, c = r.ButtonA.Item2, d = r.ButtonB.Item2 };
            var (px, py) = (r.Prize.Item1, r.Prize.Item2);
            var denominator = m.a * m.d - m.b * m.c;
            if (denominator != 0)
            {
                //This is solvable 
                var determinant = 1.0 / denominator;
                Matrix inverse_m = new() { a = m.d * determinant, b = -m.b * determinant, c = -m.c * determinant, d = m.a * determinant };
                var (a, b) = (px * inverse_m.a + py * inverse_m.b, px * inverse_m.c + py * inverse_m.d);
                var (ta, tb) = (a - Math.Truncate(a), b - Math.Truncate(b));
                if ((ta < 0.0001 || ta > 0.999) && (tb < 0.0001 || tb > 0.999))
                {
                    part1Total += 3 * (int)Math.Round(a) + (int)Math.Round(b);
                }
            }


        }
        return part1Total;
    }

    private ulong SolvePart2(List<Rules> rules)
    {
        ulong part2Total = 0;
        foreach (var r in rules)
        {
            // m*x1 + n*x2 = p1
            // m*y1 + n*y2 = p2

            //This is a matrix problem we need to get the inverse of the matrix 
            // [ x1, x2 ]
            // [ y1, y2 ]
            Matrix m = new() { a = r.ButtonA.Item1, b = r.ButtonB.Item1, c = r.ButtonA.Item2, d = r.ButtonB.Item2 };
            var (px, py) = (r.Prize.Item1 + 10000000000000L, r.Prize.Item2 + 10000000000000L);
            var denominator = m.a * m.d - m.b * m.c;
            if (denominator != 0)
            {
                //This is solvable 
                var determinant = 1.0 / denominator;
                Matrix inverse_m = new() { a = m.d * determinant, b = -m.b * determinant, c = -m.c * determinant, d = m.a * determinant };
                var (a, b) = (px * inverse_m.a + py * inverse_m.b, px * inverse_m.c + py * inverse_m.d);
                var (ta, tb) = (a - Math.Truncate(a), b - Math.Truncate(b));
                if ((ta < 0.0001 || ta > 0.999) && (tb < 0.0001 || tb > 0.999))
                {
                    part2Total += 3UL * (ulong)Math.Round(a) + (ulong)Math.Round(b);
                }
            }


        }
        return part2Total;
    }

    struct Rules
    {
        public (long, long) ButtonA;
        public (long, long) ButtonB;
        public (long, long) Prize;
    }
    
}