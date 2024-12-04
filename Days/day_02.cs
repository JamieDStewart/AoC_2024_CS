
using AoC_2024;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2024
{
    class Day_02 : IDay
    {
        class Record
        {
            public List<long> values = new();
        }
        private List<Record> ReadFileData()
        {
            List<Record> input = new();
            using (var read = new StreamReader("./input/day_02.txt"))
            {

                for (var line = read.ReadLine(); line != null; line = read.ReadLine())
                {
                    List<long> values = new();
                    string[] lineContent = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    foreach( var value in lineContent )
                    {
                        if (long.TryParse(value, out long numValue))
                        {
                            values.Add(numValue);
                        }                        
                    }
                    input.Add(new Record { values  = values } );
                }
            }
            return input;
        }

        private bool testForIncrementalPass( List<long> record)
        {
            return record.Skip(1).Select((v, i) => v > (record[i]) && Math.Abs(v - (record[i])) <= 3).All(v => v);
        }

        private bool testForDecrementalPass(List<long> record)
        {
            return record.Skip(1).Select((v, i) => v < (record[i]) && Math.Abs(v - (record[i])) <= 3).All(v => v);
        }
        private long SolvePart1(List<Record> input)
        {
            //find safe rows
            // Row is safe if all values are incrementing or decrementing with difference between adjacent values no greater than 3
            long count = 0;
           foreach(  var record in input )
            {
                bool decremental = testForDecrementalPass(record.values);
                bool incremental = testForIncrementalPass(record.values);
                if (incremental || decremental) { count++; }
            }
           return count;
        }

        private delegate bool TestPass( List<long> input );

        private bool DampenerTestPass( List<long> record, TestPass testFunc )
        {
            if (testFunc(record))
            {
                return true;
            }
            else
            {
                //This could be sped up if the false value from the test was removed instead of testing by removing a single value across the record
                for (int i = 0; i < record.Count; i++)
                {
                    List<long> values = new(record);
                    values.RemoveAt(i);
                    if (testFunc(values))
                    {
                        return true; ;
                    }
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
            {
                if(DampenerTestPass( record.values, increment))
                {
                    ++count;
                }
                else if( DampenerTestPass( record.values, decrement))
                {
                    ++count;
                }
                
            }
            return count;
        }
        public Result Solve()
        {
            Stopwatch sw = Stopwatch.StartNew();
            sw.Start();
            //Read input
            List<Record> input = ReadFileData();
            //carry out tasks
            long part_01 = SolvePart1(input);
            long part_02 = SolvePart2(input);

            sw.Stop();

            double time = sw.ElapsedMilliseconds / 1000.0;
            return new Result(name: "2.Red-Nosed Reports", part_01: part_01, part_02: part_02, execution_time: time);
        }
    }
}

