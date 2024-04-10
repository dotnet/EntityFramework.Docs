using NewInEfCore9;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("Starting application...");

        using var context = new BlogsContext();

        Console.WriteLine($"Model loaded with {context.Model.GetEntityTypes().Count()} entity types.");
    }
}
