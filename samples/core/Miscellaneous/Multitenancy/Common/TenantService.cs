namespace Common;

public delegate void TenantChangedEventHandler(object source, TenantChangedEventArgs args);
public class TenantService : ITenantService
{
    public TenantService() => Tenant = GetTenants()[0];

    public TenantService(string tenant) => Tenant = tenant;

    public event EventHandler<TenantChangedEventArgs> OnTenantChanged = null!;

    public string Tenant { get; private set; }

    public void SetTenant(string tenant)
    {
        if (tenant != Tenant)
        {
            var old = Tenant;
            Tenant = tenant;
            OnTenantChanged?.Invoke(this, new TenantChangedEventArgs(old, Tenant));
        }
    }

    public string[] GetTenants() =>
    [
        "TenantA",
        "TenantB",
    ];
}
