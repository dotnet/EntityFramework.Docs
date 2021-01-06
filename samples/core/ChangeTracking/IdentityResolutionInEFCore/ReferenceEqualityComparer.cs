using System.Collections.Generic;
using System.Runtime.CompilerServices;

#region ReferenceEqualityComparer
public sealed class ReferenceEqualityComparer : IEqualityComparer<object>
{
    private ReferenceEqualityComparer()
    {
    }

    public static ReferenceEqualityComparer Instance { get; } = new ReferenceEqualityComparer();

    bool IEqualityComparer<object>.Equals(object x, object y) => x == y;

    int IEqualityComparer<object>.GetHashCode(object obj) => RuntimeHelpers.GetHashCode(obj);
}
#endregion
