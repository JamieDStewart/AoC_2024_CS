

using System.Text;
using System.Xml.Linq;

namespace AoC_2024
{

    class Program
    {       
        static void Main(string[] args)
        {
            //Get all days from this assembly - means I can just add a new DayXX : IDay class and not worry about editing this main function
            var iDay = typeof(IDay);
            var dayTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany( assembly => assembly.GetTypes()
                .Where( type => iDay.IsAssignableFrom(type) && type.IsClass) );
            List<IDay> days = new List<IDay>();
            //Get an instance of each day and shove it in the list
            foreach ( var day in dayTypes )
            {
                var instance = Activator.CreateInstance(day) as IDay;
                if( instance != null ) {days.Add(instance);}
            }

            //Go over all days in the list and Solve them!
            List<Result> results = new List<Result>();
            foreach ( var day in days ) 
            {
                results.Add(day.Solve());
            }
            //Display the answers to the console
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