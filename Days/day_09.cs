using System;
using System.Data.Common;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic.CompilerServices;

namespace AoC_2024;

internal class Day_09 : IDay
{
    public Result Solve()
    {
        var sw = Stopwatch.StartNew();
        sw.Start();

        //Read input
        var dm = ReadFileData();
        //carry out tasks
        var part01 = SolvePart1(dm);
        var part02 = SolvePart2(dm);

        sw.Stop();

        var time = sw.ElapsedMilliseconds / 1000.0;
        return new Result("9.Disk Fragmenter", part01, part02, time);
    }

    private List<int> ReadFileData()
    {
        using (var read = new StreamReader("./input/day_09.txt"))
        {
            for (var line = read.ReadLine(); line != null; )
            {
               return line.ToList().Select(p => p - '0').ToList();
            }
        }
        return new List<int>();
    }
    
    private long SolvePart1(List<int> dm)
    {
        //modify the disk map to individual blocks and empty spaces
        var blockMap = new List<int>();
        for (var i = 0; i < dm.Count; ++i)
        {
            var fileId = (i % 2 == 0) ? i / 2 : -1;
            for (var j = 0; j < dm[i]; ++j)
            {
                blockMap.Add(fileId);
            }
            
        }
        //restructure block layout to remove empty spaces with data from end of block
        var lastIndex = blockMap.Count;
        long total = 0;
        for (var i = 0; i < lastIndex; ++i)
        {
            if (blockMap[i] == -1)
            {
                //find the last index in the file that is not -1 and swap these two values
                for (var j = lastIndex - 1; j > i; --j)
                {
                    if (blockMap[j] == -1) continue;
                    (blockMap[i], blockMap[j]) = (blockMap[j], blockMap[i]);
                    lastIndex = j;
                    total += i * blockMap[i];
                    break;
                }
            }
            else
            {
                total += i * blockMap[i];
            }
        }
        return total;
    }

    private long SolvePart2(List<int> dm)
    {
        //modify the disk map to individual blocks and empty spaces
        var freespaces = new List<FreeSpace>();
        var usedBlocks = new List<UsedSpace>();
        var index = 0;
        for (var i = 0; i < dm.Count; ++i)
        {
            var fileId = (i % 2 == 0) ? i / 2 : -1;
            if (fileId == -1)
            {
                freespaces.Add(new(index, dm[i]));
            }
            else
            {
                usedBlocks.Add(new(index, fileId, dm[i]));
            }

            index += dm[i];
        }
        //reverse the memory blocks 
        usedBlocks.Reverse();
        long total = 0;
        foreach (var b in usedBlocks) // iterate over all blocks
        {
            foreach (var f in freespaces) //find free space large enough to move block to
            {
                if (f.Location > b.Location) break; //early out on the iteration
                if (f.Size >= b.Size )
                {
                    b.Location = f.Location;
                    f.Size -= b.Size;
                    f.Location += b.Size;
                }
            }

            for (var i = b.Location; i < b.Location + b.Size; ++i)
            {
                total += i * b.FileId;
            }
        }

        return total;
    }

    private class FreeSpace
    {
        public int Location = 0;
        public int Size = 0;

        public FreeSpace(int location, int size)
        {
            Location = location;
            Size = size;
        }
    }

    private class UsedSpace
    {
        public int Location = 0;
        public int FileId = 0;
        public int Size = 0;

        public UsedSpace(int location, int fileId, int size)
        {
            Location = location;
            FileId = fileId;
            Size = size;
        }
    }

}