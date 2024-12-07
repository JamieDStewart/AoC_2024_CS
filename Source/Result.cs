using System.Text;

namespace AoC_2024;

internal struct Result
{
    private readonly string name;
    private readonly double execution_time;
    private readonly long part_01;
    private readonly long part_02;

    public Result(string name, long part_01, long part_02, double execution_time) : this()
    {
        this.name = name;
        this.part_01 = part_01;
        this.part_02 = part_02;
        this.execution_time = execution_time;
    }

    public readonly override string ToString()
    {
        StringBuilder sb = new();
        sb.Append("||");
        sb.Append(name.PadRight(23));
        sb.Append("||");
        sb.Append(part_01.ToString().PadRight(15));
        sb.Append("||");
        sb.Append(part_02.ToString().PadRight(15));
        sb.Append("||");
        sb.Append(execution_time.ToString().PadRight(10));
        sb.Append("||");
        return sb.ToString();
    }
}