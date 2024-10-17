using System.Collections.Generic;

namespace EFModeling.DataSeeding;

public class Country
{
    public int CountryId { get; set; }
    public string Name { get; set; }
    public virtual ICollection<Language> OfficialLanguages { get; set; }
}
