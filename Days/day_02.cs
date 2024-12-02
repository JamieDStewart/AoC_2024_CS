
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
        public Result Solve()
        {
            Stopwatch sw = Stopwatch.StartNew();
            sw.Start();

            //carry out tasks

            sw.Stop();

            long time = sw.ElapsedMilliseconds / 1000L;
            return new Result(name: "Day 02", part_01: 0L, part_02: 0L, execution_time: time);
        }
    }
}

