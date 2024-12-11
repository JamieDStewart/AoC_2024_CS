using System;
using System.Data.Common;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic.CompilerServices;

namespace AoC_2024;

internal class Day_11 : IDay
{
    public Result Solve()
    {
        var sw = Stopwatch.StartNew();
        sw.Start();

        //Read input
        var input = ReadFileData();
        //carry out tasks
        var part01 = SolvePart1(input);
        var part02 = SolvePart2(input);

        sw.Stop();

        var time = sw.ElapsedMilliseconds / 1000.0;
        return new Result("11.Plutonian Pebbles", part01, part02, time);
    }

    private List<long> ReadFileData()
    {
        var input = new List<long>();
        using (var read = new StreamReader("./input/day_11.txt"))
        {
           for (var line = read.ReadLine(); line != null; line = read.ReadLine())
           {
                long parsed = 0;
                input = line.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Where(x => long.TryParse(x, out parsed))
                    .Select(x => parsed).ToList();
           }
        }
        return input;
    }

    
   private long SolvePart1(List<long> input)
   {
        
        var total = input.Sum(s => Blink(s, 25));
        return total;
    }

   private Dictionary<(long,int), long> cachedValues = new Dictionary<(long, int), long>();

   private long Blink(long num, int blinks)
   {
       //Rules 
       // if 0 number becomes 1
       // if even digits number splits in 2
       // otherwise multiply by 2024

        var key = (num, blinks);
       //have we seen this number with the same remaining number of blinks
        if (cachedValues.TryGetValue(key, out long value)) 
           return value;
        //No blinks left
        if (key.blinks == 0)
        {
            cachedValues.Add(key, 1);
            return 1L;
        }

        var result = 0L;
        if (key.num == 0) //Apply Rule 1
        {
            result = Blink(1, key.blinks - 1); 
        }
        else
        {
            var numDigits = Math.Floor(Math.Log10(num) + 1);
            if (numDigits % 2 == 0) //Apply Rule 2
            {
                var tenFactor = (long)Math.Pow(10, numDigits / 2);
                var topHalf = num / tenFactor;
                var bottomHalf = num % tenFactor;
                result = Blink(topHalf, key.blinks-1) + Blink(bottomHalf, key.blinks-1);
            }
            else //Apply rule 3
            {
                result = Blink(key.num * 2024, key.blinks - 1);
            }
        }
        cachedValues.Add(key, result);
        return result;
   }

   private long SolvePart2(List<long> input)
   {
       var total = input.Sum( s => Blink(s, 75));
       return total;
   }

}