

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AoC_2024
{
    class Day_01 : IDay
    {
        class Input
        {
            public List<long> column_1 = new();
            public List<long> column_2 = new();
        }
        private Input ReadFileData()
        {
            Input input = new Input();
            using (var read = new StreamReader("./input/day_01.txt"))
            {

                for (var line = read.ReadLine(); line != null; line = read.ReadLine())
                {
                    string[] lineContent = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (lineContent.Length == 2)
                    {
                        if (long.TryParse(lineContent[0], out long col1))
                        {
                            input.column_1.Add(col1);
                        }
                        if (long.TryParse(lineContent[1], out long col2))
                        {
                            input.column_2.Add(col2);
                        }
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
            for( var i = 0; i < input.column_1.Count; i++)
            {
                sumVariance += Math.Abs(input.column_1[i] - input.column_2[i]);
            }
            return sumVariance;
        }

        private long SolvePart2(Input input)
        {
            Dictionary<long, long> frequencyTable = new Dictionary<long, long>();

            for (var i = 0; i < input.column_2.Count; i++)
            {
                if (frequencyTable.ContainsKey(input.column_2[i]))
                    frequencyTable[input.column_2[i]]++;
                else
                    frequencyTable.Add(input.column_2[i], 1);
            }

            long accumulatedDiff = 0;
            foreach (var i in input.column_1)
            {
                if (frequencyTable.ContainsKey(i))
                {
                    accumulatedDiff += i * frequencyTable[i];
                }
            }
            return accumulatedDiff;
        }


        public Result Solve()
        {
            Stopwatch sw = Stopwatch.StartNew();
            sw.Start();
            //Get Input
            Input input = ReadFileData();

            //carry out tasks
            long part_01 = SolvePart1(input);
            long part_02 = SolvePart2(input);

            sw.Stop();

            double time = sw.ElapsedMilliseconds / 1000.0;
            return new Result(name: "1.Historian Hysteria", part_01: part_01, part_02: part_02, execution_time: time);
        }
    }
}

