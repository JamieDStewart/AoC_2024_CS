using System.Diagnostics;
using System.Text.RegularExpressions;

namespace AoC_2024;

internal class Day_03 : IDay
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
        return new Result("3.Mull It Over", part_01.ToString(), part_02.ToString(), time);
    }

    private Input ReadFileData()
    {
        Input input = new();
        using (var read = new StreamReader("./input/day_03.txt"))
        {
            for (var line = read.ReadLine(); line != null; line = read.ReadLine()) input.instructions.Add(line);
        }

        return input;
    }

    //Input is just a vector of strings read directly from the day's input text file
    private long SolvePart1(Input input)
    {
        var summed_mul_operations = 0L;
        //use a regular expression to get each mul(x,y) instruction out of the data
        //Use groups to separate out the values to make converting them simple
        const string pattern = @"mul\(([0-9]{1,3})\,([0-9]{1,3})\)";
        Regex regex = new(pattern);

        foreach (var instruction in input.instructions)
        {
            var matches = regex.Matches(instruction);
            foreach (Match match in matches)
            {
                var x = long.Parse(match.Groups[1].Value);
                var y = long.Parse(match.Groups[2].Value);
                summed_mul_operations += x * y;
            }
        }

        return summed_mul_operations;
    }

    private long SolvePart2(Input input)
    {
        var summedMulOperations = 0L;
        //Instructions all have different lengths do = 4, dont = 7 (minimum length mul = 8)
        const long doInstructionLength = 4;
        const long dontInstructionLength = 7;

        // introduction of do() and don't() operators
        var skip = false; //skip instruction carries over each line
        //use a regular expression to get each mul(x,y) instruction out of the data
        const string pattern = @"mul\(([0-9]{1,3})\,([0-9]{1,3})\)|don't\(\)|do\(\)";
        Regex regex = new(pattern);

        foreach (var instruction in input.instructions)
        {
            var matches = regex.Matches(instruction);

            foreach (Match match in matches)
            {
                if (match.Value.Length == doInstructionLength)
                {
                    skip = false;
                    continue;
                }

                if (match.Value.Length == dontInstructionLength)
                {
                    skip = true;
                    continue;
                }

                if (skip) continue;

                var x = long.Parse(match.Groups[1].Value);
                var y = long.Parse(match.Groups[2].Value);
                summedMulOperations += x * y;
            }
        }

        return summedMulOperations;
    }

    private class Input
    {
        public readonly List<string> instructions = new();
    }
}