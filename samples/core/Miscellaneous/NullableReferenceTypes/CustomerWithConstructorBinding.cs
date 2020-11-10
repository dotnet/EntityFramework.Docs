namespace NullableReferenceTypes
{
    #region CustomerWithConstructorBinding
    public class CustomerWithConstructorBinding
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public CustomerWithConstructorBinding(string name)
        {
            Name = name;
        }
    }
    #endregion
}
