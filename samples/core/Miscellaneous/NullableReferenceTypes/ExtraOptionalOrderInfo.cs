namespace NullableReferenceTypes
{
    public class ExtraOptionalOrderInfo
    {
        public int Id { get; set; }
        public string SomeExtraAdditionalInfo { get; set; }

        public ExtraOptionalOrderInfo(string someExtraAdditionalInfo)
        {
            SomeExtraAdditionalInfo = someExtraAdditionalInfo;
        }
    }
}
