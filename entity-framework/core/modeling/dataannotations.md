---
title: Data Annotations (primary) | Microsoft Docs
author: MarcAir
ms.author: ?

ms.date: 12/07/2016

ms.assetid: 
ms.technology: entity-framework-core
 
uid: core/modeling/dataannotations
---
# Keys (primary)

> [!NOTE]
> This documentation is for EF Core. For EF6.x, see [Entity Framework 6](../../ef6/index.md).

Provides a general-purpose attribute that lets you specify localizable strings for types and members of entity partial classes. Or can specify that a data field value is required.

````csharp
class User
{
    public string Id { get; set; }
    
    [Required(ErrorMessage = "The Email field is required.")]
    [Display(Name = "Email", Prompt = "Email")]
    [DataType(DataType.EmailAddress)] 
    public string Email { get; set; }
    
    public string Name { get; set; }
}
````
> see   [Enum for datatype](https://msdn.microsoft.com/en-us/library/system.componentmodel.dataannotations.datatype(v=vs.110).aspx), 
>       [DisplayAttribute](https://msdn.microsoft.com/en-us/library/system.componentmodel.dataannotations.displayattribute(v=vs.110).aspx), 
>       [RequiredAttribute](../required-optional.md) / [MSDN RequiredAttribute ](https://msdn.microsoft.com/en-us/library/system.componentmodel.dataannotations.requiredattribute(v=vs.110).aspx)
