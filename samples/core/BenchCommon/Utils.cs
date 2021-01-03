// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace BenchCommon
{
    public class Utils
    {
        private const int seed = 12345;
        private readonly int[] arry;
        private readonly int _count;

        public Utils(int count)
        {
            var r = new Random(seed);           // make sequence repeatable for consistent results
            arry = new int[count];
            var allbut1 = count - 1;
            for (var i = Blog.baseID; i < allbut1; i++)
            {
                arry[i] = i + Blog.baseID;      // must match the 1 .. n in SeedData
            }
            var swap = allbut1;                 // start with "final" value [subscript not actually assigned yet]
            for (var i = 0; i < count; i++)     // some cells will not be switched, others multiple times [= cheap shuffle]
            {
                var idx = r.Next(allbut1);      // 0 .. count-2
                var oldval = arry[idx];
                arry[idx] = swap;
                swap = oldval;
            }
            arry[allbut1] = swap;               // assign final subscript
            _count = count;
        }
        public IEnumerable<int> Next()
        {
            for (var i = 0; i < _count; i++)
            {
                yield return arry[i];
            }
        }
    }
}
