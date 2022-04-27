using System.Linq;

namespace EFQuerying.ComplexQuery;

internal class Program
{
    private static void Main(string[] args)
    {
        using (var context = new BloggingContext())
        {
            #region Join
            var query = from photo in context.Set<PersonPhoto>()
                        join person in context.Set<Person>()
                            on photo.PersonPhotoId equals person.PhotoId
                        select new { person, photo };
            #endregion
        }

        using (var context = new BloggingContext())
        {
            #region JoinComposite
            var query = from photo in context.Set<PersonPhoto>()
                        join person in context.Set<Person>()
                            on new { Id = (int?)photo.PersonPhotoId, photo.Caption }
                            equals new { Id = person.PhotoId, Caption = "SN" }
                        select new { person, photo };
            #endregion
        }

        using (var context = new BloggingContext())
        {
            #region GroupJoin
            var query = from b in context.Set<Blog>()
                        join p in context.Set<Post>()
                            on b.BlogId equals p.BlogId into grouping
                        select new { b, grouping };
            #endregion
        }

        using (var context = new BloggingContext())
        {
            #region GroupJoinComposed
            var query = from b in context.Set<Blog>()
                        join p in context.Set<Post>()
                            on b.BlogId equals p.BlogId into grouping
                        select new { b, Posts = grouping.Where(p => p.Content.Contains("EF")).ToList() };
            #endregion
        }

        using (var context = new BloggingContext())
        {
            #region SelectManyConvertedToCrossJoin
            var query = from b in context.Set<Blog>()
                        from p in context.Set<Post>()
                        select new { b, p };
            #endregion
        }

        using (var context = new BloggingContext())
        {
            #region SelectManyConvertedToJoin
            var query = from b in context.Set<Blog>()
                        from p in context.Set<Post>().Where(p => b.BlogId == p.BlogId)
                        select new { b, p };

            var query2 = from b in context.Set<Blog>()
                         from p in context.Set<Post>().Where(p => b.BlogId == p.BlogId).DefaultIfEmpty()
                         select new { b, p };
            #endregion
        }

        using (var context = new BloggingContext())
        {
            #region SelectManyConvertedToApply
            var query = from b in context.Set<Blog>()
                        from p in context.Set<Post>().Select(p => b.Url + "=>" + p.Title)
                        select new { b, p };

            var query2 = from b in context.Set<Blog>()
                         from p in context.Set<Post>().Select(p => b.Url + "=>" + p.Title).DefaultIfEmpty()
                         select new { b, p };
            #endregion
        }

        using (var context = new BloggingContext())
        {
            #region GroupBy
            var query = from p in context.Set<Post>()
                        group p by p.AuthorId
                        into g
                        select new { g.Key, Count = g.Count() };
            #endregion
        }

        using (var context = new BloggingContext())
        {
            #region GroupByFilter
            var query = from p in context.Set<Post>()
                        group p by p.AuthorId
                        into g
                        where g.Count() > 0
                        orderby g.Key
                        select new { g.Key, Count = g.Count() };
            #endregion
        }

        using (var context = new BloggingContext())
        {
            #region LeftJoin
            var query = from b in context.Set<Blog>()
                        join p in context.Set<Post>()
                            on b.BlogId equals p.BlogId into grouping
                        from p in grouping.DefaultIfEmpty()
                        select new { b, p };
            #endregion
        }
    }
}