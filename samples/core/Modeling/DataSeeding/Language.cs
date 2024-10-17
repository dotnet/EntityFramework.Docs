using System.Collections.Generic;

namespace EFModeling.DataSeeding;

public class Language
{
    public int Id { get; set; }
    public string Name { get; set; }

    public LanguageDetails Details { get; set; }
    public List<Country> UsedIn { get; set; }
}
