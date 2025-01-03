using System;
using System.Threading.Tasks;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Samples for _Change Tracker Debugging_");
        Console.WriteLine();

        await Samples.Change_tracker_debug_view_1();
        await Samples.Change_tracker_logging_1();
    }
}
