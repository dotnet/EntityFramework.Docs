namespace NullableReferenceTypes
{
    #region Customer
    public class Customer
    {
        public int Id { get; set; }
        public string FirstName { get; set; } // Required by convention
        public string LastName { get; set; } // Required by convention
        public string? MiddleName { get; set; } // Optional by convention

        // Note the following use of constructor binding, which avoids compiled warnings
        // for uninitialized non-nullable properties.
        public Customer(string firstName, string lastName, string? middleName = null)
        {
            FirstName = firstName;
            LastName = lastName;
            MiddleName = middleName;
        }
    }
    #endregion
}
