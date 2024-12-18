using System.Diagnostics;

namespace AoC_2024;

internal class Day_02 : IDay
{
    public Result Solve()
    {
        var sw = Stopwatch.StartNew();
        sw.Start();
        //Read input
        var input = ReadFileData();
        //carry out tasks
        var part_01 = SolvePart1(input);
        var part_02 = SolvePart2(input);

        sw.Stop();

        var time = sw.ElapsedMilliseconds / 1000.0;
        return new Result("2.Red-Nosed Reports", part_01.ToString(), part_02.ToString(), time);
    }

    private List<Record> ReadFileData()
    {
        List<Record> input = new();
        using (var read = new StreamReader("./input/day_02.txt"))
        {
            for (var line = read.ReadLine(); line != null; line = read.ReadLine())
            {
                List<long> values = new();
                var lineContent = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                foreach (var value in lineContent)
                    if (long.TryParse(value, out var numValue))
                        values.Add(numValue);
                input.Add(new Record { values = values });
            }
        }

        return input;
    }

    private bool testForIncrementalPass(List<long> record)
    {
        return record.Skip(1).Select((v, i) => v > record[i] && Math.Abs(v - record[i]) <= 3).All(v => v);
    }

    private bool testForDecrementalPass(List<long> record)
    {
        return record.Skip(1).Select((v, i) => v < record[i] && Math.Abs(v - record[i]) <= 3).All(v => v);
    }

    private long SolvePart1(List<Record> input)
    {
        //find safe rows
        // Row is safe if all values are incrementing or decrementing with difference between adjacent values no greater than 3
        long count = 0;
        foreach (var record in input)
        {
            var decremental = testForDecrementalPass(record.values);
            var incremental = testForIncrementalPass(record.values);
            if (incremental || decremental) count++;
        }

        return count;
    }

    private bool DampenerTestPass(List<long> record, TestPass testFunc)
    {
        if (testFunc(record))
            return true;
        //This could be sped up if the false value from the test was removed instead of testing by removing a single value across the record
        for (var i = 0; i < record.Count; i++)
        {
            List<long> values = new(record);
            values.RemoveAt(i);
            if (testFunc(values))
            {
                return true;
                ;
            }
        }

        return false;
    }

    private long SolvePart2(List<Record> input)
    {
        //find safe rows - dampener allows 1 value to be removed from failing test to allow passing
        // Row is safe if all values are incrementing or decrementing with difference between adjacent values no greater than 3
        long count = 0;
        TestPass increment = testForIncrementalPass;
        TestPass decrement = testForDecrementalPass;

        foreach (var record in input)
            if (DampenerTestPass(record.values, increment))
                ++count;
            else if (DampenerTestPass(record.values, decrement)) ++count;
        return count;
    }

    private class Record
    {
        public List<long> values = new();
    }

    private delegate bool TestPass(List<long> input);
}