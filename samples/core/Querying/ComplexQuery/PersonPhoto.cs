namespace EFQuerying.ComplexQuery;

public class PersonPhoto
{
    public int PersonPhotoId { get; set; }
    public string Caption { get; set; }
    public byte[] Photo { get; set; }

    public Person Person { get; set; }
}