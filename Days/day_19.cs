using System.Diagnostics;
using System.Linq;
using Microsoft.VisualBasic.CompilerServices;

namespace AoC_2024;

internal class Day_19 : IDay
{
    
    public Result Solve()
    {
        var sw = Stopwatch.StartNew();
        sw.Start();

        //Read input
        ReadFileData();
        //carry out tasks
        var (part01, part02) = SolveIt();
        
        sw.Stop();

        var time = sw.ElapsedMilliseconds / 1000.0;
        return new Result("19.Linen Layout", part01.ToString(), part02.ToString(), time);
    }

    private void ReadFileData()
    {
        
        //Each line of file is "p=0,4 v=3,-3" p is position, v is velocity
        using (var read = new StreamReader("./input/day_19.txt"))
        {
            var readingPatterns = false;
            for (var line = read.ReadLine(); line != null; line = read.ReadLine())
            {
                if (line.Length == 0)
                {
                    readingPatterns = true;
                    continue;
                }

                if (!readingPatterns)
                {
                    //Process reading in the available towels
                    _input.Towels = new HashSet<string>(line.Split(", ").ToList().Distinct() );
                }
                else
                {
                    _input.Patterns.Add(line);
                }
            }
        }

    }

    private readonly Dictionary<string, long> _cachedPatterns = new Dictionary<string, long>();

    //Recursive function that tests smaller and smaller portions of the sought after pattern
    long ValidatePattern(string pattern)
    {
        if (_cachedPatterns.TryGetValue(pattern, out var cachedValue))
        {
            return cachedValue;
        }

        var result = pattern.Length == 0 ? 1L : 0L;
        foreach (var towel in _input.Towels)
        {
            if (pattern.StartsWith(towel))
            {
                result += ValidatePattern(pattern[towel.Length..]);
            }
        }
        
        _cachedPatterns[pattern] = result;
        return result;
    }
    
    private (long, long) SolveIt()
    {
        var achievablePattern = 0L;
        var totalCombinations = 0L;
        foreach (var pattern in _input.Patterns)
        {
            var result = ValidatePattern(pattern);
            achievablePattern += result > 0 ? 1 : 0;
            totalCombinations += result;
        }
        return (achievablePattern, totalCombinations);
    }

    private readonly Input _input = new();
    private class Input
    {
        public HashSet<string> Towels = new HashSet<string>();
        public readonly List<string> Patterns = new List<string>();
    }
}