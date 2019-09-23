using System;

namespace NullableReferenceTypes
{
    #region Order
    public class Order
    {
        public int Id { get; set; }

        private Address? _shippingAddress;

        public Address ShippingAddress
        {
            set => _shippingAddress = value;
            get => _shippingAddress
                   ?? throw new InvalidOperationException("Uninitialized property: " + nameof(ShippingAddress));
        }

        public Product Product { get; set; } = null!;

        public OptionalOrderInfo? OptionalInfo { get; set; }
    }
    #endregion
}
