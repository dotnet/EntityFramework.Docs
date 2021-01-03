// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#if !DEBUG
using BenchmarkDotNet.Running;      // remember to add <Optimize>true</Optimize> to .csproj
#endif

namespace Benchmarks
{
    public static class Program
    {
        public static void Main(string[] args)
        {

#if DEBUG   // in Benchmarks project Build tab: set "Define DEBUG constant", and unset "Optimize code" checkbox
            var sut = new AvgBlogRankByPK();
            sut.DIYMark();
#else       // flip the above settings, to actually invoke the Benchmark smarts
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
#endif
            System.Console.WriteLine("all done, type any key to exit [after keeping above log!]");
            System.Console.ReadKey();
        }
    }
}
