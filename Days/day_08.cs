using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic.CompilerServices;

namespace AoC_2024;

internal class Day_08 : IDay
{
    public Result Solve()
    {
        var sw = Stopwatch.StartNew();
        sw.Start();

        //Read input
        var al = ReadFileData();
        //carry out tasks
        var part01 = SolvePart1(al);
        var part02 = SolvePart2(al);

        sw.Stop();

        var time = sw.ElapsedMilliseconds / 1000.0;
        return new Result("8.Resonant Collinearity", part01.ToString(), part02.ToString(), time);
    }

    class MapData
    {
        public List<Tuple<int, int>>[] AntennaLocations = new List<Tuple<int, int>>[255];
        public int MapWidth = 0;
        public int MapHeight = 0;
    }

    private MapData ReadFileData()
    {
        //as all antenna are represented by A-Z, a-z or 0-9 use ascii values to index them in an array
        var mapData = new MapData();

        using (var read = new StreamReader("./input/day_08.txt"))
        {
            int y = 0;
            for (var line = read.ReadLine(); line != null; line = read.ReadLine())
            {
                mapData.MapWidth = line.Length;
                int x = 0;
                foreach (var c in line)
                {
                    if (c != '.')
                    {
                        if (mapData.AntennaLocations[c] == null)
                        {
                            mapData.AntennaLocations[c] = new List<Tuple<int, int>>();
                        }

                        mapData.AntennaLocations[c].Add(new Tuple<int, int>(x, y));
                    }
                    ++x;
                }
                ++y;
            }
            mapData.MapHeight = y;
           
        }
        return mapData;
    }

    //function to recurse over list and accumulate values
    
    private long SolvePart1(MapData md)
    {
        var antinodeLocations = new HashSet<Tuple<int,int>>();
        //iterate ove the map data antenna locations
        foreach (var ag in md.AntennaLocations)
        {
            if (ag == null || ag.Count == 0)
            {
                continue;
            }
            //go through each antenna loctation and get the manhattan distance to it's neighbours
            for (int i = 0; i < ag.Count - 1; ++i)
            {
                for (int j = i + 1; j < ag.Count; ++j)
                {
                    var mx = ag[i].Item1 - ag[j].Item1;
                    var my = ag[i].Item2 - ag[j].Item2;

                    var ax = ag[i].Item1 + mx; var ay = ag[i].Item2 + my;
                    if (ax >= 0 && ax < md.MapWidth && ay >= 0 && ay < md.MapHeight)
                    {
                        antinodeLocations.Add(new Tuple<int, int>(ax, ay));
                    }

                    ax = ag[j].Item1 - mx; ay = ag[j].Item2 - my;
                    if (ax >= 0 && ax < md.MapWidth && ay >= 0 && ay < md.MapHeight)
                    {
                        antinodeLocations.Add(new Tuple<int, int>(ax, ay));
                    }
                }
            }
        }
        return antinodeLocations.Count;
    }

    private long SolvePart2(MapData md)
    {
        var antinodeLocations = new HashSet<Tuple<int, int>>();
        //iterate ove the map data antenna locations
        foreach (var ag in md.AntennaLocations)
        {
            if (ag == null || ag.Count == 0)
            {
                continue;
            }
            //go through each antenna loctation and get the manhattan distance to it's neighbours
            for (int i = 0; i < ag.Count - 1; ++i)
            {
                for (int j = i + 1; j < ag.Count; ++j)
                {
                    antinodeLocations.Add(new Tuple<int, int>(ag[i].Item1, ag[i].Item2));
                    antinodeLocations.Add(new Tuple<int, int>(ag[j].Item1, ag[j].Item2));
                    var mx = ag[i].Item1 - ag[j].Item1;
                    var my = ag[i].Item2 - ag[j].Item2;

                    var ax = ag[i].Item1 + mx; var ay = ag[i].Item2 + my;
                    while (ax >= 0 && ax < md.MapWidth && ay >= 0 && ay < md.MapHeight)
                    {
                        antinodeLocations.Add(new Tuple<int, int>(ax, ay));
                        ax += mx; 
                        ay += my;
                    }

                    ax = ag[j].Item1 - mx; ay = ag[j].Item2 - my;
                    while(ax >= 0 && ax < md.MapWidth && ay >= 0 && ay < md.MapHeight)
                    {
                        antinodeLocations.Add(new Tuple<int, int>(ax, ay));
                        ax -= mx;
                        ay -= my;
                    }
                }
            }
        }
        return antinodeLocations.Count;
    }

}