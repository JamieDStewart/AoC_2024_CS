using System.Text;

namespace AoC_2024;

internal class Program
{
    private const string AoC = "Advent of Code 2024";
    private const string day_title = "Day";
    private const string part_1 = "Part 1";
    private const string part_2 = "Part 2";
    private const string time = "Time";

    private static readonly string separator = new('=', 87);

    private static void Main(string[] args)
    {
        //Get all days from this assembly - means I can just add a new DayXX : IDay class and not worry about editing this main function
        var iDay = typeof(IDay);
        var dayTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()
            .Where(type => iDay.IsAssignableFrom(type) && type.IsClass));
        var days = new List<IDay>();
        //Get an instance of each day and shove it in the list
        foreach (var day in dayTypes)
        {
            var instance = Activator.CreateInstance(day) as IDay;
            if (instance != null) days.Add(instance);
        }

        //Go over all days in the list and Solve them!
        var results = new List<Result>();
        foreach (var day in days) results.Add(day.Solve());
        //Display the answers to the console
        PrintTitleBar();
        foreach (var result in results) Console.WriteLine(result);
    }

    private static void PrintTitleBar()
    {
        StringBuilder sb = new();
        sb.Append(separator);
        sb.AppendLine();
        sb.Append("||");
        sb.Append(AoC.PadLeft(53).PadRight(83));
        sb.Append("||");
        sb.AppendLine();
        sb.Append(separator);
        sb.AppendLine();
        sb.Append("||");
        sb.Append(day_title.PadRight(27));
        sb.Append("||");
        sb.Append(part_1.PadRight(20));
        sb.Append("||");
        sb.Append(part_2.PadRight(20));
        sb.Append("||");
        sb.Append(time.PadRight(10));
        sb.Append("||");
        sb.AppendLine();
        sb.Append(separator);
        Console.WriteLine(sb.ToString());
    }
}