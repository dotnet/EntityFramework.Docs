namespace Common
{
    public class TenantChangedEventArgs : EventArgs
    {
        public TenantChangedEventArgs(string? oldTenant, string newTenant)
        {
            OldTenant = oldTenant;
            NewTenant = newTenant;
        }

        public string? OldTenant { get; private set; }

        public string NewTenant { get; private set; }
    }
}
