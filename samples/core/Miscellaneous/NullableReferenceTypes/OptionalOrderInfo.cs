namespace NullableReferenceTypes
{
    public class OptionalOrderInfo
    {
        public int Id { get; set; }
        public string AdditionalInfo { get; set; }
        public ExtraOptionalOrderInfo? ExtraAdditionalInfo { get; set; }

        public OptionalOrderInfo(string additionalInfo)
        {
            AdditionalInfo = additionalInfo;
        }
    }
}
