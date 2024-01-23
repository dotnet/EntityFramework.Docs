namespace Common;

public interface ITenantService
{
    string Tenant { get; }

    void SetTenant(string tenant);

    string[] GetTenants();

    event EventHandler<TenantChangedEventArgs> OnTenantChanged;
}
