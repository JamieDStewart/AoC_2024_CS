using System;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic.CompilerServices;

namespace AoC_2024;

internal class Day_17 : IDay
{
    public Result Solve()
    {
        var sw = Stopwatch.StartNew();
        sw.Start();

        //Read input
        var input = ReadFileData();
        //carry out tasks
        var part01 = SolvePart1(input);
        //re-read in file data as map is modified by part 1
        var part02 = SolvePart2(input);

        sw.Stop();

        var time = sw.ElapsedMilliseconds / 1000.0;
        return new Result("17.Chronospatial Computer", part01, part02.ToString(), time);
    }



    private Input ReadFileData()
    {
        var i = new Input();
        var readProgram = false;
        using (var read = new StreamReader("./input/day_17.txt"))
        {
            var reg = 0;
            for (var line = read.ReadLine(); line != null; line = read.ReadLine())
            {
                if (!readProgram)
                {
                    if (line.Length == 0)
                    {
                        readProgram = true;
                        continue;
                    }

                    i._Registers[reg] = long.Parse(line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Last());
                    ++reg;
                }
                else
                {
                    int parsed = 0;
                    i._Program = line.Split(' ').Last().Split(',').Where(x => int.TryParse(x, out parsed))
                        .Select(x => parsed).ToList();
                }

            }
        }
        return i;
    }

    long GetComboOperand(long[] registers, int a)
    {
        switch (a)
        {
            case 0:
            case 1:
            case 2:
            case 3:
                return a;
            case 4:
                return registers[0];
            case 5:
                return registers[1];
            case 6: 
                return registers[2];
            default:
                return -1;
        }
    }

    private string SolvePart1(Input input)
    {
        var ip = 0;
        var output = new List<int>();
        while (ip < input._Program.Count)
        {
            var comboOperand = GetComboOperand(input._Registers, input._Program[ip + 1]);

            switch (input._Program[ip])
            {
                case 0: // adv instruction performs division
                    input._Registers[0] = input._Registers[0] >> (int)comboOperand;
                    ip+=2;
                    break;
                case 1: // bxl instruction performs and xor on register B
                    input._Registers[1] ^= input._Program[++ip];
                    ++ip;
                    break;
                case 2: //bst operand modulo 8
                    input._Registers[1] = comboOperand % 8;
                    ip+=2;
                    break;
                case 3: //jnz jump non zero - if a != 0 jump increase pc by 1.
                    if (input._Registers[0] != 0)
                    {
                        ip = input._Program[++ip];
                    }
                    else
                    {
                        ip += 2;
                    }
                    break;
                case 4: //bxc bitwise or of register b & c
                    input._Registers[1] ^= input._Registers[2];
                    ip += 2;
                    break;
                case 5: //out outputs the value of the operand mod 8
                    output.Add((int)(comboOperand % 8));
                    ip+=2;
                    break;
                case 6: //bdv 
                    input._Registers[1] = input._Registers[0] >> (int)comboOperand;
                    ip+=2;
                    break;
                case 7:
                    input._Registers[2] = input._Registers[0] >> (int)comboOperand;
                    ip += 2;
                    break;

            }
        }

        var part1Total = String.Join( ',', output.Select(n => n.ToString()).ToArray());
        return part1Total;
    }

    private long SolvePart2(Input input)
    {
        //The program is meant to recreate itself find the right value for register a that does this.
        //If we run the program with register a set to a value of 0 -> 7 it will produce the last output for the program for some of those values
        //if we take that correct output and shift it left 3 places and then run again for 0 -> 7 we will get the last two outputs 
        //we can loop this to produce the correct value for the program.
        var registerAValues = new Queue<(long, int)>();
        registerAValues.Enqueue((0, input._Program.Count - 1));
        long reg = 0;
        while (registerAValues.Count > 0)
        {
            var (currentA, outputIndex) = registerAValues.Dequeue();
            //only uses 3 bit values so loop from 0->7
            for (var i = 0; i < 8; ++i)
            {
                reg = (currentA << 3) + i;
                input._Registers[0] = reg;
                int outVal = 0;
                var result = SolvePart1(input).Split(',').Where(x => int.TryParse(x, out outVal))
                    .Select(x => outVal).ToList();
                //assess if the output is equal from the offset point
                if (result.Count != input._Program.Count - outputIndex) break;
                bool equal = true;
                for( var o = 0; o < result.Count; ++o )
                {
                    if (result[o] != input._Program[outputIndex + o])
                    {
                        equal = false;
                        break;
                    }
                }

                if (equal) //this value works so far so add it to the queue to test
                {
                    if (outputIndex == 0)
                    {
                        registerAValues.Clear();
                        break;
                    }
                    registerAValues.Enqueue((reg, outputIndex-1));
                }


            }
        }
        //we have found the value for register A that duplicates the program
        return reg;
    }

    class Input
    {
        public long[] _Registers = new long[3];
        public List<int> _Program = new();
    }

}