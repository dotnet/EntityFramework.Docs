namespace Common;
public class Contact
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public bool IsUnicorn {get; set; }

    public static Contact[] GeneratedContacts =>
    new []
    {
        new Contact
        {
            Name = "Magic Unicorns",
            IsUnicorn = true
        },
        new Contact
        {
            Name =  "Unicorns Running",
            IsUnicorn = true
        },
        new Contact
        {
            Name = "SQL Server DBA",
            IsUnicorn = false
        },
        new Contact
        {
            Name = "Azure Cosmos DB Admin",
            IsUnicorn = false
        }
    };
}
