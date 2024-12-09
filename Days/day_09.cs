using System;
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
            for (var line = read.ReadLine(); line != null; line = read.ReadLine())
            {
               return line.ToList().Select(p => p - '0').ToList();
            }
        }
        return new List<int>();
    }
    
    private long SolvePart1(List<int> dm)
    {
        //modify the disk map to individual blocks and empty spaces
        var fileId = 0;
        var blockMap = new List<int>();
        for (var i = 0; i < dm.Count; ++i)
        {
            if (i % 2 == 1) // odd locations are empty spaces
            {
                for (var j = 0; j < dm[i]; ++j)
                {
                    blockMap.Add(-1);
                }
            }
            else
            {
                for (int j = 0; j < dm[i]; ++j)
                {
                    blockMap.Add(fileId);
                }

                fileId++;
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
                for (var j = lastIndex - 1; j > 0; --j)
                {
                    if (blockMap[j] != -1 )
                    {
                        lastIndex = j;
                        break;
                    }
                }

                if (lastIndex > i)
                {
                    (blockMap[i], blockMap[lastIndex]) = (blockMap[lastIndex], blockMap[i]);
                }
            }

            if (blockMap[i] != -1)
            {
                total += i * blockMap[i];
            }
        }
        return total;
    }

    private long SolvePart2(List<int> dm)
    {
        //modify the disk map to individual blocks and empty spaces
        var fileId = 0;
        var blockMap = new List<int>();
        for (var i = 0; i < dm.Count; ++i)
        {
            if (i % 2 == 1) // odd locations are empty spaces
            {
                for (var j = 0; j < dm[i]; ++j)
                {
                    blockMap.Add(-1);
                }
            }
            else
            {
                for (var j = 0; j < dm[i]; ++j)
                {
                    blockMap.Add(fileId);
                }

                fileId++;
            }
        }
        //Rules state that we start with last block and attempt to move to first available slot that is large enough to hold it
        //If no blocks large enough then this block does not move.
        var lastBlock = blockMap.Count;
        for (var i = lastBlock-1; i >= 0; )
        {
            if (blockMap[i] != -1) // Found first block that isn't empty now get size of block
            {
                var blockSize = 1;
                while (i - blockSize >= 0 && blockMap[i - blockSize] == blockMap[i])
                {
                    blockSize++;
                }

                var blockStart = i - (blockSize-1);
                //Size of block acquired now find free block that will fit
                for (var j = 0; j < blockStart; ++j)
                {
                    if (blockMap[j] == -1) //found free space get size of space
                    {
                        var freeSpace = 1;
                        while (j + freeSpace < blockStart && blockMap[j + freeSpace] == blockMap[j])
                        {
                            freeSpace++;
                        }

                        if (freeSpace >= blockSize) //block can be moved here
                        {
                            for (int p = 0; p < blockSize; ++p)
                            {
                                (blockMap[blockStart + p], blockMap[j+p]) = (blockMap[j+p], blockMap[blockStart + p]);
                            }

                            break;
                        }
                        j += freeSpace;
                        
                    }
                }

                i -= blockSize;
            }
            else
            {
                --i;
            }
        }

        long total = 0;
        for (var i = 0; i < blockMap.Count; i++)
        {
            if (blockMap[i] != -1)
            {
                total += i * blockMap[i];
            }
        }
        return total;
    }

}