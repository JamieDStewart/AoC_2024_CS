using System.Diagnostics;

namespace AoC_2024;

internal class Day_01 : IDay
{
    public Result Solve()
    {
        var sw = Stopwatch.StartNew();
        sw.Start();
        //Get Input
        var input = ReadFileData();

        //carry out tasks
        var part_01 = SolvePart1(input);
        var part_02 = SolvePart2(input);

        sw.Stop();

        var time = sw.ElapsedMilliseconds / 1000.0;
        return new Result("1.Historian Hysteria", part_01.ToString(), part_02.ToString(), time);
    }

    private Input ReadFileData()
    {
        var input = new Input();
        using (var read = new StreamReader("./input/day_01.txt"))
        {
            for (var line = read.ReadLine(); line != null; line = read.ReadLine())
            {
                var lineContent = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (lineContent.Length == 2)
                {
                    if (long.TryParse(lineContent[0], out var col1)) input.column_1.Add(col1);
                    if (long.TryParse(lineContent[1], out var col2)) input.column_2.Add(col2);
                }
            }
        }

        return input;
    }

    private long SolvePart1(Input input)
    {
        input.column_1.Sort();
        input.column_2.Sort();

        long sumVariance = 0;
        for (var i = 0; i < input.column_1.Count; i++) sumVariance += Math.Abs(input.column_1[i] - input.column_2[i]);
        return sumVariance;
    }

    private long SolvePart2(Input input)
    {
        var frequencyTable = new Dictionary<long, long>();

        for (var i = 0; i < input.column_2.Count; i++)
            if (frequencyTable.ContainsKey(input.column_2[i]))
                frequencyTable[input.column_2[i]]++;
            else
                frequencyTable.Add(input.column_2[i], 1);

        long accumulatedDiff = 0;
        foreach (var i in input.column_1)
            if (frequencyTable.ContainsKey(i))
                accumulatedDiff += i * frequencyTable[i];
        return accumulatedDiff;
    }

    private class Input
    {
        public readonly List<long> column_1 = new();
        public readonly List<long> column_2 = new();
    }
}