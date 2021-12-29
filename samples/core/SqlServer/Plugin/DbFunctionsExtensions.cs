using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace SqlServer.Plugin;
public static class DbFunctionsExtensions
{
    public static T Augment<T>(this DbFunctions _, T number)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Augment)));
}
