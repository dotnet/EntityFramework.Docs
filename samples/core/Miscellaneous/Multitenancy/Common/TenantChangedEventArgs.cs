namespace Common
{
    public class TenantChangedEventArgs : EventArgs
    {
        public TenantChangedEventArgs(string? oldTenant, string newTenant)
        {
            OldTenant = oldTenant;
            NewTenant = newTenant;
        }

        public string? OldTenant { get; }

        public string NewTenant { get; }
    }
}
