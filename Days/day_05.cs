
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AoC_2024
{
    class Day_05 : IDay
    {
        struct Updates
        {
            public Dictionary<int, List<int>> pageOrderingRules;
            public List<List<int>> pageUpdates;
            public List<List<int>> invalidUpdates;

        }

        private Updates ReadFileData()
        {
            Updates updates = new();
            using (var read = new StreamReader("./input/day_05.txt"))
            {
                bool section2 = false;
                updates.pageOrderingRules = new Dictionary<int, List<int>>();
                updates.pageUpdates = new List<List<int>>();
                updates.invalidUpdates = new List<List<int>>();

                for (var line = read.ReadLine(); line != null; line = read.ReadLine())
                {
                    if (line.Length == 0)
                    {
                        section2 = true;
                        continue;
                    }

                    if (!section2)
                    {
                        var values = line.Split('|', StringSplitOptions.RemoveEmptyEntries);
                        var key = int.Parse(values[0]);
                        var value = int.Parse(values[1]);
                        //find the key in the dictionary if it exists
                        if (updates.pageOrderingRules.ContainsKey(key))
                            updates.pageOrderingRules[key].Add(value);
                        else
                            updates.pageOrderingRules.Add(key, new(){value} );

                    }
                    else
                    {
                        var values = line.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => int.Parse(s)).ToList();
                        updates.pageUpdates.Add(values);
                    }
                }
            }
            return updates;
        }

        private long SolvePart1(Updates up)
        {
            long count = 0;
            // find the good print update jobs and add middle values to count
            foreach (var printJob in up.pageUpdates)
            {
                //all pages numbers should be lower than 100 according to puzzle
                var pages = new bool[100];
                var validJob = true;
                foreach( var page in printJob )
                {
                    //get the page rules from the update
                    if (up.pageOrderingRules.ContainsKey(page))
                    {
                        var rules = up.pageOrderingRules[page];
                        //iterate over all rules and ensure they're not already in the printJob pages
                        foreach (var r in rules)
                        {
                            if (pages[r])
                            {
                                validJob = false;
                                break;
                            }
                        }

                        if (!validJob)
                        {
                            //might as well make a small modification to part 1 to collect the invalid jobs for part 2.
                            up.invalidUpdates.Add(printJob);
                            break;
                        }
                    }
                    //set page to be printed in the job
                    pages[page] = true;
                }

                if (validJob)
                {
                    count += printJob[printJob.Count / 2];
                }
            }
            return count;
        }

        private long SolvePart2(Updates up)
        {
            //Need to fix up the print jobs that were invalid 
            long count = 0;
            foreach (var invalidJob in up.invalidUpdates)
            {
                var printedPagesIndex = new int[100];
                var sortedList = new List<int>();
                //for each page in the job if a page has already been printed that breaks the rule find that page in the job and swap them
                foreach( var page in invalidJob)
                {
                    if (up.pageOrderingRules.ContainsKey(page))
                    {
                        var rules = up.pageOrderingRules[page];
                        var lowestIndex = sortedList.Count;
                        foreach (var r in rules)
                        {
                            if (sortedList.Contains(r))
                            {
                                var index = sortedList.IndexOf(r);
                                lowestIndex =  index < lowestIndex ? index : lowestIndex;
                            }
                        }
                        sortedList.Insert(lowestIndex, page);
                    }
                    else
                    {
                        sortedList.Add(page);
                    }
                }

                count += sortedList[sortedList.Count / 2];
            }
            return count;
        }
        public Result Solve()
        {
            Stopwatch sw = Stopwatch.StartNew();
            sw.Start();

            //Read input
            var up = ReadFileData();
            //carry out tasks
            var part_01 = SolvePart1(up);
            var part_02 = SolvePart2(up);

            sw.Stop();

            var time = sw.ElapsedMilliseconds / 1000.0;
            return new Result(name: "5.Print Queue", part_01: part_01, part_02: part_02, execution_time: time);
        }
    }
}

