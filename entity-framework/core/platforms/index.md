---
title: Platforms | Microsoft Docs
author: rowanmiller
ms.author: divega
ms.date: 03/13/2017
ms.assetid: bfce70e5-7e14-47d3-87b2-e0b93352e955
ms.technology: entity-framework-core
uid: core/platforms/index
---

# Platforms

> [!NOTE]
> This documentation is for EF Core. For EF6.x, see [Entity Framework 6](../../ef6/index.md).

We want EF Core to be available anywhere you write .NET code, and we're still working towards that goal. The following table provides guidance for each platform where we want to enable EF Core.

<table>
  <tr>
    <th></th>
    <th>Status</th>
    <th>Details</th>
    <th>Known Issues</th>
  </tr>

  <tr>
    <td>
      <strong>.NET Framework</strong>
      <br/><br />
      (WinForms, WPF, ASP.NET, Console, etc.)
    </td>
    <td>
      Fully supported – we recommend using EF Core <sup>(1)</sup>.
    </td>
    <td>
      .NET Framework is well covered by our automated test suite. Many applications are successfully using EF Core.
      <br /><br />
      <sup>(1)</sup> It is worth noting that EF6.x is also available to .NET Framework applications. We recommend reading the [Compare EF Core & EF6.x](../../efcore-and-ef6/index.md) section of our documentation to help you chose the right technology.
    </td>
    <td>
      No significant platform-specific issues.
    </td>
  </tr>

  <tr>
    <td>
      <strong>.NET Core</strong>
      <br /><br />
      (ASP.NET Core, and .NET Core Console)
    </td>
    <td>
      Fully supported – we recommend using EF Core.
    </td>
    <td>
      .NET Core is well covered by our automated test suite. Many applications are successfully using EF Core.
    </td>
    <td>
      No significant platform-specific issues.
    </td>
  </tr>

  <tr>
    <td>
      <strong>Xamarin/Mono</strong>
    </td>
    <td>
      In progress – EF Core can be used, but issues may be encountered.
    </td>
    <td>
      Xamarin recently added support for .NET Standard, which allows EF Core to be used in Xamarin applications.
      <br /><br />
      Ad-hoc testing has been performed by the EF Core team and customers. Early adopters have reported success in using EF Core.
      <br /><br />
      There are some known issues and others will likely be uncovered as testing continues.
    </td>
    <td>
      There are several known issues when running on Mono, including the following.
      <ul>
        <li>[iOS NotImplementedException during query](https://github.com/aspnet/EntityFramework/issues/7158) (will be fixed in Mono 5.0)</li>
        <li>[Tools: Support Xamarin Projects (currently requires separate .NET Standard class library)](https://github.com/aspnet/EntityFramework/issues/7152)</li>
      </ul>
    </td>
  </tr>

  <tr>
    <td>
      <strong>UWP</strong>
    </td>
    <td>
      In progress – not recommended due to .NET Native issues.
    </td>
    <td>
      Ad-hoc testing has been performed by the EF Core team and customers.
      <br /><br />
      There are significant issues reported when compiled with .NET Native. This is typically done during a release build, and is a requirement for deploying to the Windows Store.
      <br /><br />
      If you are not using .NET Native, or just want to experiment UWP, EF Core can be used.
    </td>
    <td>
      There are several known issues for UWP, including the following.
      <ul>
        <li>[.NET Native: SqliteException 'FOREIGN KEY constraint failed' during SaveChanges](https://github.com/aspnet/EntityFramework/issues/7614)</li>
        <li>[.NET Native: Slow performance in change tracker](https://github.com/aspnet/EntityFramework/issues/7582)</li>
        <li>[.NET Native: LINQ query of nullable Enum type throws ArgumentException](https://github.com/aspnet/EntityFramework/issues/6537)</li>
        <li>[.NET Native: NullReferenceException in LINQ query](https://github.com/aspnet/EntityFramework/issues/6423)</li>
        <li>[.NET Native: Wrong result from LINQ query with condition based on navigation property](https://github.com/aspnet/EntityFramework/issues/5841)</li>
        <li>[.NET Native: LINQ query with anonymous types throw NullReferenceException](https://github.com/aspnet/EntityFramework/issues/5119)</li>
      </ul>
    </td>
  </tr>
</table>
