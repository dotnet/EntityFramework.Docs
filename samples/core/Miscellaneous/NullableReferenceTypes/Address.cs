namespace NullableReferenceTypes
{
    #region OrderDetails
    public class Address
    {
        public int Id { get; set; }
        public string City { get; set; }
        public string Street { get; set; }

        public Address(string city, string street)
        {
            City = city;
            Street = street;
        }
    }
    #endregion
}
