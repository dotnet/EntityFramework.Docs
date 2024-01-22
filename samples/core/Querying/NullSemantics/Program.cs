using System.Linq;

namespace NullSemantics;

internal class Program
{
    private static void Main()
    {
        using var context = new NullSemanticsContext();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        //#region FunctionSqlRaw
        //context.Database.ExecuteSqlRaw(
        //    @"create function [dbo].[ConcatStrings] (@prm1 nvarchar(max), @prm2 nvarchar(max))
        //        returns nvarchar(max)
        //        as
        //        begin
        //            return @prm1 + @prm2;
        //        end");
        //#endregion

        BasicExamples();
        Functions();
        ManualOptimization();
    }

    private static void BasicExamples()
    {
        using var context = new NullSemanticsContext();
        #region BasicExamples
        IQueryable<NullSemanticsEntity> query1 = context.Entities.Where(e => e.Id == e.Int);
        IQueryable<NullSemanticsEntity> query2 = context.Entities.Where(e => e.Id == e.NullableInt);
        IQueryable<NullSemanticsEntity> query3 = context.Entities.Where(e => e.Id != e.NullableInt);
        IQueryable<NullSemanticsEntity> query4 = context.Entities.Where(e => e.String1 == e.String2);
        IQueryable<NullSemanticsEntity> query5 = context.Entities.Where(e => e.String1 != e.String2);
        #endregion

        var result1 = query1.ToList();
        var result2 = query2.ToList();
        var result3 = query3.ToList();
        var result4 = query4.ToList();
        var result5 = query5.ToList();
    }

    private static void Functions()
    {
        using var context = new NullSemanticsContext();

        #region Functions
        IQueryable<NullSemanticsEntity> query = context.Entities.Where(e => e.String1.Substring(0, e.String2.Length) == null);
        #endregion

        var result = query.ToList();
    }

    private static void ManualOptimization()
    {
        using var context = new NullSemanticsContext();

        #region ManualOptimization
        IQueryable<NullSemanticsEntity> query1 = context.Entities.Where(e => e.String1 != e.String2 || e.String1.Length == e.String2.Length);
        IQueryable<NullSemanticsEntity> query2 = context.Entities.Where(
            e => e.String1 != null && e.String2 != null && (e.String1 != e.String2 || e.String1.Length == e.String2.Length));
        #endregion

        var result1 = query1.ToList();
        var result2 = query2.ToList();
    }
}
