using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace NullSemantics;

internal class Program
{
    private static async Task Main(string[] args)
    {
        using var context = new NullSemanticsContext();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        //#region FunctionSqlRaw
        //await context.Database.ExecuteSqlRawAsync(
        //    @"create function [dbo].[ConcatStrings] (@prm1 nvarchar(max), @prm2 nvarchar(max))
        //        returns nvarchar(max)
        //        as
        //        begin
        //            return @prm1 + @prm2;
        //        end");
        //#endregion

        //await BasicExamples();
        await Functions();
        //await ManualOptimization();
    }

    private static async Task BasicExamples()
    {
        using var context = new NullSemanticsContext();
        #region BasicExamples
        var query1 = context.Entities.Where(e => e.Id == e.Int);
        var query2 = context.Entities.Where(e => e.Id == e.NullableInt);
        var query3 = context.Entities.Where(e => e.Id != e.NullableInt);
        var query4 = context.Entities.Where(e => e.String1 == e.String2);
        var query5 = context.Entities.Where(e => e.String1 != e.String2);
        #endregion

        var result1 = await query1.ToListAsync();
        var result2 = await query2.ToListAsync();
        var result3 = await query3.ToListAsync();
        var result4 = await query4.ToListAsync();
        var result5 = await query5.ToListAsync();
    }

    private static async Task Functions()
    {
        using var context = new NullSemanticsContext();

        #region Functions
        var query = context.Entities.Where(e => e.String1.Substring(0, e.String2.Length) == null);
        #endregion

        var result = await query.ToListAsync();
    }

    private static async Task ManualOptimization()
    {
        using var context = new NullSemanticsContext();

        #region ManualOptimization
        var query1 = context.Entities.Where(e => e.String1 != e.String2 || e.String1.Length == e.String2.Length);
        var query2 = context.Entities.Where(
            e => e.String1 != null && e.String2 != null && (e.String1 != e.String2 || e.String1.Length == e.String2.Length));
        #endregion

        var result1 = await query1.ToListAsync();
        var result2 = await query2.ToListAsync();
    }
}
