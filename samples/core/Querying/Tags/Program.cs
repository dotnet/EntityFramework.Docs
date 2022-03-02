﻿using System.Linq;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace EFQuerying.Tags;

internal class Program
{
    private static void Main(string[] args)
    {
        using (var context = new SpatialContext())
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        using (var context = new SpatialContext())
        {
            #region BasicQueryTag
            var myLocation = new Point(1, 2);
            var nearestPeople = (from f in context.People.TagWith("This is my spatial query!")
                                 orderby f.Location.Distance(myLocation) descending
                                 select f).Take(5).ToList();
            #endregion
        }

        using (var context = new SpatialContext())
        {
            #region ChainedQueryTags
            var results = Limit(GetNearestPeople(context, new Point(1, 2)), 25).ToList();
            #endregion
        }

        using (var context = new SpatialContext())
        {
            #region MultilineQueryTag
            var results = Limit(GetNearestPeople(context, new Point(1, 2)), 25).TagWith(
                @"This is a multi-line
string").ToList();
            #endregion
        }
    }

    #region QueryableMethods
    private static IQueryable<Person> GetNearestPeople(SpatialContext context, Point myLocation)
        => from f in context.People.TagWith("GetNearestPeople")
           orderby f.Location.Distance(myLocation) descending
           select f;

    private static IQueryable<T> Limit<T>(IQueryable<T> source, int limit) => source.TagWith("Limit").Take(limit);
    #endregion
}

public class Person
{
    public int Id { get; set; }
    public Point Location { get; set; }
}