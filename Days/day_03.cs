
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AoC_2024
{
    class Day_03 : IDay
    {
        class Input
        {
            public List<string> instructions = new();
        }
        private Input ReadFileData()
        {
            Input input = new();
            using (var read = new StreamReader("./input/day_03.txt"))
            {
                
                for (var line = read.ReadLine(); line != null; line = read.ReadLine())
                {
                    input.instructions.Add(line);
                }
            }
            return input;
        }

        private long SolvePart1(Input input)
        {
            var summed_mul_operations = 0L;
            foreach (var instruction in input.instructions)
            {
                //use a regular expression to get each mul(x,y) instruction out of the data
                string pattern = @"(mul)\([0-9]{1,3}\,[0-9]{1,3}\)";
                Regex regex = new(pattern);

                MatchCollection matches = regex.Matches(instruction);
                foreach (Match match in matches)
                {
                    //Just use Regex again to extract the numbers
                    var values = Regex.Split(match.Value, @"\D+").Where(s => s != String.Empty).ToArray();
                    summed_mul_operations += long.Parse(values[0]) * long.Parse(values[1]);
                }
            }
            return summed_mul_operations;
        }

        private long SolvePart2(Input input)
        {
            var summed_mul_operations = 0L;
            //Instructions all have different lengths do = 4, dont = 7 (minimum length mul = 8)
            var do_instruction_length = 4;
            var dont_instruction_length = 7;

            // introduction of do() and don't() operators
            bool skip = false;
            foreach (var instruction in input.instructions)
            {
                //use a regular expression to get each mul(x,y) instruction out of the data
                string pattern = @"(mul)\([0-9]{1,3}\,[0-9]{1,3}\)|don't\(\)|do\(\)";
                Regex regex = new(pattern);

                MatchCollection matches = regex.Matches(instruction);
                
                foreach (Match match in matches)
                {
                    if (match.Value.Length == do_instruction_length)
                    {
                        skip = false;
                        continue;
                    }
                    if (match.Value.Length == dont_instruction_length)
                    {
                        skip = true;
                        continue;
                    }

                    if (skip)
                    {
                        continue;
                    }
                    //Just use Regex again to extract the numbers
                    var values = Regex.Split(match.Value, @"\D+").Where(s => s != String.Empty).ToArray();
                    summed_mul_operations += long.Parse(values[0]) * long.Parse(values[1]);
                    
                }
            }

            return summed_mul_operations;
        }
        public Result Solve()
        {
            Stopwatch sw = Stopwatch.StartNew();
            sw.Start();
            //Read input
            Input input = ReadFileData();
            //carry out tasks
            var part_01 = SolvePart1(input);
            var part_02 = SolvePart2(input);

            sw.Stop();

            var time = sw.ElapsedMilliseconds / 1000.0;
            return new Result(name: "3.Mull It Over", part_01: part_01, part_02: part_02, execution_time: time);
        }
    }
}

