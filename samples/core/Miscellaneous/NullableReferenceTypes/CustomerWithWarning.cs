#pragma warning disable CS8618

namespace NullableReferenceTypes
{
    #region CustomerWithWarning
    public class CustomerWithWarning
    {
        public int Id { get; set; }

        // Generates CS8618, uninitialized non-nullable property:
        public string Name { get; set; }
    }
    #endregion
}
