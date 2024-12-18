using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic.CompilerServices;

namespace AoC_2024;

internal class Day_07 : IDay
{
    public Result Solve()
    {
        var sw = Stopwatch.StartNew();
        sw.Start();

        //Read input
        var calibrations = ReadFileData();
        //carry out tasks
        var part01 = SolvePart1(calibrations, out List<Calibration> part2Calibrations);
        var part02 = part01 + SolvePart2(part2Calibrations);

        sw.Stop();

        var time = sw.ElapsedMilliseconds / 1000.0;
        return new Result("7.Bridge Repair", part01.ToString(), part02.ToString(), time);
    }

    class Calibration
    {
        public long output;
        public List<long> factors = new List<long>();
    }

    private List<Calibration> ReadFileData()
    {
        List<Calibration> calibrations = new();
        using (var read = new StreamReader("./input/day_07.txt"))
        {
            for (var line = read.ReadLine(); line != null; line = read.ReadLine())
            {
                var c = new Calibration();
                var lineContent = line.Split(':', StringSplitOptions.RemoveEmptyEntries);
                //split into output : <factors>
                c.output = long.Parse(lineContent[0]);
                long parsed = 0;
                c.factors = lineContent[1].Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Where( x => long.TryParse(x, out parsed))
                    .Select(x => parsed).ToList();
                calibrations.Add(c);
            }
        }
        return calibrations;
    }

    //function to recurse over list and accumulate values
    private bool PerformOps(long target, long accumulatedVal, List<long> factors, bool part1 = true)
    {
        if (accumulatedVal > target)
        {
            return false;
        }
        if (factors.Count == 0)
        {
            return target == accumulatedVal;
        }
        var remaining = factors.Skip(1).ToList();

        if (part1)
        {
            return (PerformOps(target, accumulatedVal + factors[0], remaining) ||
                    PerformOps(target, accumulatedVal * factors[0], remaining));
        }

        return (PerformOps(target, accumulatedVal + factors[0], remaining, false) ||
                PerformOps(target, accumulatedVal * factors[0], remaining, false) ||
                PerformOps(target, long.Parse($"{accumulatedVal}{factors[0]}"), remaining, false));

    }
    private long SolvePart1(List<Calibration> calibrations, out List<Calibration> part2Calibrations )
    {
        long accumulated = 0; 
        part2Calibrations = new List<Calibration>();
        foreach (var c in calibrations)
        {
            if (PerformOps(c.output, c.factors[0], c.factors.Skip(1).ToList()))
            {
                accumulated += c.output;
            }
            else
            {
                part2Calibrations.Add(c);
            }
        }
        return accumulated;
    }

    private long SolvePart2(List<Calibration> calibrations)
    {
        long accumulated = 0;
        foreach (var c in calibrations)
        {
            if (PerformOps(c.output, c.factors[0], c.factors.Skip(1).ToList(), false))
                accumulated += c.output;
        }
        return accumulated;
    }

}