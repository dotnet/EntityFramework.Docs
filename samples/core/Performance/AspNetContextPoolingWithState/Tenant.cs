namespace Performance.AspNetContextPoolingWithState;

public class Tenant : ITenant
{
    public Tenant(int tenantId)
        => TenantId = tenantId;

    public int TenantId { get; set; }
}