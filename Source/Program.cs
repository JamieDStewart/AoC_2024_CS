

using System.Text;
using System.Xml.Linq;

namespace AoC_2024
{

    class Program
    {       
        static void Main(string[] args)
        {
            //Get all days from this assembly
            var iDay = typeof(IDay);
            var dayTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany( assembly=> assembly.GetTypes().Where( type => iDay.IsAssignableFrom(type) && type.IsClass) );
            List<IDay> days = new List<IDay>();
            foreach ( var day in dayTypes )
            {
                days.Add(Activator.CreateInstance(day) as IDay);
            }

            List<Result> results = new List<Result>();
            
            foreach ( var day in days ) 
            {
                results.Add(day.Solve());
            }

            PrintTitleBar();
            foreach ( var result in results )
            {
                Console.WriteLine(result);
            }
        }

        static string separator = new('=', 70);
        const string AoC = "Advent of Code 2024";
        const string day_title = "Day";
        const string part_1 = "Part 1";
        const string part_2 = "Part 2";
        const string time = "Time";
        static void PrintTitleBar()
        {
            StringBuilder sb = new();
            sb.Append(separator);
            sb.AppendLine();
            sb.Append("||");
            sb.Append(AoC.PadLeft(42).PadRight(66));
            sb.Append("||");
            sb.AppendLine();
            sb.Append(separator);
            sb.AppendLine();
            sb.Append("||");
            sb.Append(day_title.PadRight(23));
            sb.Append("||");
            sb.Append(part_1.PadRight(12));
            sb.Append("||");
            sb.Append(part_2.PadRight(15));
            sb.Append("||");
            sb.Append(time.PadRight(10));
            sb.Append("||");
            sb.AppendLine();
            sb.Append(separator);
            Console.WriteLine(sb.ToString());

        }
    }
}