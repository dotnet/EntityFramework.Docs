Configuring a DbContext
=======================

This article shows patterns for configuring a ``DbContext`` with
``DbContextOptions``. Options are primarily used to select and configure the
data store.

.. contents:: `In this article`
  :local:

Configuring DbContextOptions
----------------------------

``DbContext`` must have an instance of ``DbContextOptions`` in order to execute. This can be
supplied to ``DbContext`` in one of two ways.

1. `Constructor argument`_
2. `OnConfiguring`_

If both are used, "OnConfiguring" takes higher priority, which means it can overwrite or change options supplied by the constructor argument.

Constructor argument
~~~~~~~~~~~~~~~~~~~~

.. code-block:: csharp
  :caption: Context code with constructor

  public class BloggingContext : DbContext
  {
      public BloggingContext(DbContextOptions<BloggingContext> options)
          : base(options)
      { }

      public DbSet<Blog> Blogs { get; set; }
  }

.. tip::

  The base constructor of DbContext also accepts the non-generic version of ``DbContextOptions``. Using the non-generic version is not recommended for applications with multiple context types.

.. code-block:: csharp
  :caption: Application code to initialize from constructor argument

  var optionsBuilder = new DbContextOptionsBuilder<BloggingContext>();
  optionsBuilder.UseSqlite("Filename=./blog.db");

  using (var context = new BloggingContext(optionsBuilder.Options))
  {
      // do stuff
  }

OnConfiguring
~~~~~~~~~~~~~

.. caution::
  ``OnConfiguring`` occurs last and can overwrite options obtained from DI or
  the constructor. This approach does not lend itself to testing (unless you
  target the full database).

.. code-block:: csharp
  :caption: Context code with OnConfiguring

  public class BloggingContext : DbContext
  {
      public DbSet<Blog> Blogs { get; set; }

      protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
      {
          optionsBuilder.UseSqlite("Filename=./blog.db");
      }
  }

.. code-block:: csharp
  :caption: Application code to initialize with "OnConfiguring"

  using (var context = new BloggingContext())
  {
      // do stuff
  }

Using DbContext with dependency injection
-----------------------------------------

EF supports using ``DbContext`` with a dependency injection container. Your DbContext type can
be added to the service container by using ``AddDbContext<TContext>``.

``AddDbContext`` will add make both your DbContext type, ``TContext``, and ``DbContextOptions<TContext>`` to the available for injection from the service container.

See `more reading`_ below for information on dependency injection.

.. code-block:: csharp
  :caption: Adding dbcontext to dependency injection

  public void ConfigureServices(IServiceCollection services)
  {
      services.AddDbContext<BloggingContext>(options => options.UseSqlite("Filename=./blog.db"));
  }

This requires adding a `constructor argument`_ to you DbContext type that accepts ``DbContextOptions``.

.. code-block:: csharp
  :caption: Context code

  public class BloggingContext : DbContext
  {
      public BloggingContext(DbContextOptions<BloggingContext> options)
        :base(options)
      { }

      public DbSet<Blog> Blogs { get; set; }
  }


.. code-block:: csharp
  :caption: Application code (in ASP.NET Core)

  public MyController(BloggingContext context)

.. code-block:: csharp
  :caption: Application code (using ServiceProvider directly, less common)

  using (var context = serviceProvider.GetService<BloggingContext>())
  {
    // do stuff
  }

  var options = serviceProvider.GetService<DbContextOptions<BloggingContext>>();

.. _use_idbcontextfactory:

Using ``IDbContextFactory<TContext>``
-------------------------------------

As an alternative to the options above, you may also provide an implementation of ``IDbContextFactory<TContext>``.
EF command line tools and dependency injection can use this factory to create an instance of your DbContext. This may be required in order to enable specific design-time experiences such as migrations.

Implement this interface to enable design-time services for context types that do not have a public default constructor. Design-time services will automatically discover implementations of
this interface that are in the same assembly as the derived context.

Example:

.. code-block:: csharp

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;

    namespace MyProject
    {
        public class BloggingContextFactory : IDbContextFactory<BloggingContext>
        {
            public BloggingContext Create()
            {
                var optionsBuilder = new DbContextOptionsBuilder<BloggingContext>();
                optionsBuilder.UseSqlite("Filename=./blog.db");

                return new BloggingContext(optionsBuilder.Options);
            }
        }
    }

More reading
------------

- Read :doc:`/platforms/aspnetcore/index` for more information on
  using EF with ASP.NET Core.
- Read `Dependency Injection <https://docs.asp.net/en/latest/fundamentals/dependency-injection.html>`_ to
  learn more about using DI.
- Read :doc:`testing` for more information.
- Read :doc:`/internals/services` for more details on how EF uses dependency injection internally.
