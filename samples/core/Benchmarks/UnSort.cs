// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace Benchmarks
{
    /// <summary>
    ///     scattered PKs as per real-world OLTP
    /// </summary>
    /// <remarks>
    /// 1.  same number of keys as sequential case but jumbled up, e.g. CustID 123,6,45.. rather than neat CustNames "A","B".."Z"
    /// 2.  possibly optimisable into batches if good DDD/3NF design or commpetent DBA/DEVs
    /// </remarks>
    public static class UnSort
    {
        private const int seed = 12345;

        /// <summary>
        ///     shuffle the cards into less-behaved sequence
        /// </summary>
        /// <param name="baseID"></param>
        /// <returns>int[n] of Primary Key values for semi random-access</returns>
        /// <remarks>
        /// 1   not intended to be 100% randomised (some cells will remain at i+1), but randdom enough!
        /// 2   the sequence is in range 1..1000 (not 0-999) to match MSSQL IDENTITY(1,1)
        /// 3   explicit seed to ensure apples-apples comparisons for rival runs
        /// </remarks>
        /// <param name="count">number of records to generate (e.g. 1000 as BlogId 1 .. 1000)</param>
        public static int[] Jumble(int baseID, int count)   // beware ChaosMonkey at work (taking a break from typing Shakespeare)
        {
            var arry = new int[count];
            var r = new Random(seed);               // make sequence repeatable for consistent results
            var allbut1 = count - 1;
            for (var i = 0; i < allbut1; i++)
            {
                arry[i] = i + baseID;               // must match the 1 .. n in SeedData [cf. IDENTITY(1,1)]
            }
            var swap = allbut1;                     // start with "final" value [subscript not actually assigned yet]
            for (var j = 0; j < count; j++)         // some cells will not be switched, others multiple times [= cheap shuffle]
            {
                var idx = r.Next(allbut1);          // 0 .. count-2
                var oldval = arry[idx];
                arry[idx] = swap;
                swap = oldval;
            }
            arry[allbut1] = swap;                   // assign final subscript
            return arry;
        }
    }
}
