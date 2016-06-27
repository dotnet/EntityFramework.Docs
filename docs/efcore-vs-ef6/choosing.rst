Which One Is Right for You
==========================

The following information will help you chose between Entity Framework Core and Entity Framework 6.x.

.. contents:: `In this article:`
    :depth: 2
    :local:

What is EF6.x
-------------

Entity Framework 6.x (EF6.x) is a tried and tested data access technology with many years of features and stabilization. It first released in 2008, as part of .NET Framework 3.5 SP1 and Visual Studio 2008 SP1. Starting with the EF4.1 release it has shipped as the `EntityFramework NuGet package <https://www.nuget.org/packages/EntityFramework/>`_ - currently the most popular package on NuGet.org.

EF6.x continues to be a supported product, and will continue to see bug fixes and minor improvements for some time to come.

What is EF Core
---------------

Entity Framework Core (EF Core) is a lightweight, extensible, and cross-platform version of Entity Framework. EF Core introduces many improvements and new features when compared with EF6.x. At the same time, EF Core is a new code base and very much a v1 product.

EF Core keeps the developer experience from EF6.x, and most of the top-level APIs remain the same too, so EF Core will feel very familiar to folks who have used EF6.x. At the same time, EF Core is built over a completely new set of core components. This means EF Core doesn't automatically inherit all the features from EF6.x. Some of these features will show up in future releases (such as lazy loading and connection resiliency), other less commonly used features will not be implemented in EF Core.

The new, extensible, and lightweight core has also allowed us to add some features to EF Core that will not be implemented in EF6.x (such as alternate keys and mixed client/database evaluation in LINQ queries).

See :doc:`features` for a detailed comparison of how the feature set in EF Core compares to EF6.x.

Guidance for new applications
-----------------------------

Because EF Core is a new product, and still lacks some critical O/RM features, EF6.x will still be the most suitable choice for many applications.

These are the types of applications we would recommend using EF Core for.
 * New applications that do not require features that are not yet implemented in EF Core. Review :doc:`features` to see if EF Core may be a suitable choice for your application.
 * Applications that target .NET Core, such as Universal Windows Platform (UWP) and ASP.NET Core applications. These applications can not use EF6.x as it requires the Full .NET Framework (i.e. .NET Framework 4.5).

Guidance for existing EF6.x applications
----------------------------------------

Because of the fundamental changes in EF Core we do not recommend attempting to move an EF6.x application to EF Core unless you have a compelling reason to make the change. If you want to move to EF Core to make use of new features, then make sure you are aware of its limitations before you start. Review :doc:`features` to see if EF Core may be a suitable choice for your application.

**You should view the move from EF6.x to EF Core as a port rather than an upgrade.** See :doc:`porting/index` for more information.
