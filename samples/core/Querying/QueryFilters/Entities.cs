using System.Collections.Generic;

namespace EFQuerying.QueryFilters;

#region Entities
public class Blog
{
#pragma warning disable IDE0051, CS0169 // Remove unused private members
    private string _tenantId;
#pragma warning restore IDE0051, CS0169 // Remove unused private members

    public int BlogId { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }

    public List<Post> Posts { get; set; }
}

public class Post
{
    public int PostId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public bool IsDeleted { get; set; }

    public Blog Blog { get; set; }
}
#endregion

public class Person
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Animal> Pets { get; set; }
}

public abstract class Animal
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Person Owner { get; set; }
}

public class Cat : Animal
{
    public bool PrefersCardboardBoxes { get; set; }

    public int? ToleratesId { get; set; }

    public Dog Tolerates { get; set; }
}

public class Dog : Animal
{
    public Toy FavoriteToy { get; set; }
    public Cat FriendsWith { get; set; }
}

public class Toy
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int? BelongsToId { get; set; }
    public Dog BelongsTo { get; set; }
}