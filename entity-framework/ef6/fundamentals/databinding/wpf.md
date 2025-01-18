---
title: Databinding with WPF - EF6
description: Databinding with WPF in Entity Framework 6
author: SamMonoRT
ms.date: 05/19/2020
uid: ef6/fundamentals/databinding/wpf
---
# Databinding with WPF

> [!IMPORTANT]
> **This document is valid for WPF on the .NET Framework only**
>
> This document describes databinding for WPF on the .NET Framework. For new .NET Core projects, we recommend you use [EF Core](xref:core/index) instead of Entity Framework 6. The documentation for databinding in EF Core is here: [Getting Started with WPF](xref:core/get-started/wpf).

This step-by-step walkthrough shows how to bind POCO types to WPF controls in a “master-detail" form. The application uses the Entity Framework APIs to populate objects with data from the database, track changes, and persist data to the database.

The model defines two types that participate in one-to-many relationship: **Category** (principal\\master) and **Product** (dependent\\detail). Then, the Visual Studio tools are used to bind the types defined in the model to the WPF controls. The WPF data-binding framework enables navigation between related objects: selecting rows in the master view causes the detail view to update with the corresponding child data.

The screen shots and code listings in this walkthrough are taken from Visual Studio 2013 but you can complete this walkthrough with Visual Studio 2012 or Visual Studio 2010.

## Use the 'Object' Option for Creating WPF Data Sources

With previous version of Entity Framework we used to recommend using the **Database** option when creating a new Data Source based on a model created with the EF Designer. This was because the designer would generate a context that derived from ObjectContext and entity classes that derived from EntityObject. Using the Database option would help you write the best code for interacting with this API surface.

The EF Designers for Visual Studio 2012 and Visual Studio 2013 generate a context that derives from DbContext together with simple POCO entity classes. With Visual Studio 2010 we recommend swapping to a code generation template that uses DbContext as described later in this walkthrough.

When using the DbContext API surface you should use the **Object** option when creating a new Data Source, as shown in this walkthrough.

If needed, you can [revert to ObjectContext based code generation](xref:ef6/modeling/designer/codegen/legacy-objectcontext) for models created with the EF Designer.

## Pre-Requisites

You need to have Visual Studio 2013, Visual Studio 2012 or Visual Studio 2010 installed to complete this walkthrough.

If you are using Visual Studio 2010, you also have to install NuGet. For more information, see [Installing NuGet](/nuget/install-nuget-client-tools).  

## Create the Application

- Open Visual Studio
- **File -&gt; New -&gt; Project….**
- Select **Windows** in the left pane and **WPFApplication** in the right pane
- Enter **WPFwithEFSample** as the name
- Select **OK**

## Install the Entity Framework NuGet package

- In Solution Explorer, right-click on the **WinFormswithEFSample** project
- Select **Manage NuGet Packages…**
- In the Manage NuGet Packages dialog, Select the **Online** tab and choose the **EntityFramework** package
- Click **Install**  
    >[!NOTE]
    > In addition to the EntityFramework assembly a reference to System.ComponentModel.DataAnnotations is also added. If the project has a reference to System.Data.Entity, then it will be removed when the EntityFramework package is installed. The System.Data.Entity assembly is no longer used for Entity Framework 6 applications.

## Define a Model

In this walkthrough you can chose to implement a model using Code First or the EF Designer. Complete one of the two following sections.

### Option 1: Define a Model using Code First

This section shows how to create a model and its associated database using Code First. Skip to the next section (**Option 2: Define a model using Database First)** if you would rather use Database First to reverse engineer your model from a database using the EF designer

When using Code First development you usually begin by writing .NET Framework classes that define your conceptual (domain) model.

- Add a new class to the **WPFwithEFSample:**
  - Right-click on the project name
  - Select **Add**, then **New Item**
  - Select **Class** and enter **Product** for the class name
- Replace the **Product** class definition with the following code:

``` csharp
    namespace WPFwithEFSample
    {
        public class Product
        {
            public int ProductId { get; set; }
            public string Name { get; set; }

            public int CategoryId { get; set; }
            public virtual Category Category { get; set; }
        }
    }
```

- Add a **Category** class with the following definition:

``` csharp
    using System.Collections.ObjectModel;

    namespace WPFwithEFSample
    {
        public class Category
        {
            public Category()
            {
                this.Products = new ObservableCollection<Product>();
            }

            public int CategoryId { get; set; }
            public string Name { get; set; }

            public virtual ObservableCollection<Product> Products { get; private set; }
        }
    }
```

The **Products** property on the **Category** class and **Category** property on the **Product** class are navigation properties. In Entity Framework, navigation properties provide a way to navigate a relationship between two entity types.

In addition to defining entities, you need to define a class that derives from DbContext and exposes DbSet&lt;TEntity&gt; properties. The DbSet&lt;TEntity&gt; properties let the context know which types you want to include in the model.

An instance of the DbContext derived type manages the entity objects during run time, which includes populating objects with data from a database, change tracking, and persisting data to the database.

- Add a new **ProductContext** class to the project with the following definition:

``` csharp
    using System.Data.Entity;

    namespace WPFwithEFSample
    {
        public class ProductContext : DbContext
        {
            public DbSet<Category> Categories { get; set; }
            public DbSet<Product> Products { get; set; }
        }
    }
```

Compile the project.

### Option 2: Define a model using Database First

This section shows how to use Database First to reverse engineer your model from a database using the EF designer. If you completed the previous section (**Option 1: Define a model using Code First)**, then skip this section and go straight to the **Lazy Loading** section.

#### Create an Existing Database

Typically when you are targeting an existing database it will already be created, but for this walkthrough we need to create a database to access.

The database server that is installed with Visual Studio is different depending on the version of Visual Studio you have installed:

- If you are using Visual Studio 2010 you'll be creating a SQL Express database.
- If you are using Visual Studio 2012 then you'll be creating a [LocalDB](https://msdn.microsoft.com/library/hh510202.aspx) database.

Let's go ahead and generate the database.

- **View -&gt; Server Explorer**
- Right click on **Data Connections -&gt; Add Connection…**
- If you haven’t connected to a database from Server Explorer before you’ll need to select Microsoft SQL Server as the data source

    ![Change Data Source](~/ef6/media/changedatasource.png)

- Connect to either LocalDB or SQL Express, depending on which one you have installed, and enter **Products** as the database name

    ![Add Connection LocalDB](~/ef6/media/addconnectionlocaldb.png)

    ![Add Connection Express](~/ef6/media/addconnectionexpress.png)

- Select **OK** and you will be asked if you want to create a new database, select **Yes**

    ![Create Database](~/ef6/media/createdatabase.png)

- The new database will now appear in Server Explorer, right-click on it and select **New Query**
- Copy the following SQL into the new query, then right-click on the query and select **Execute**

``` SQL
    CREATE TABLE [dbo].[Categories] (
        [CategoryId] [int] NOT NULL IDENTITY,
        [Name] [nvarchar](max),
        CONSTRAINT [PK_dbo.Categories] PRIMARY KEY ([CategoryId])
    )

    CREATE TABLE [dbo].[Products] (
        [ProductId] [int] NOT NULL IDENTITY,
        [Name] [nvarchar](max),
        [CategoryId] [int] NOT NULL,
        CONSTRAINT [PK_dbo.Products] PRIMARY KEY ([ProductId])
    )

    CREATE INDEX [IX_CategoryId] ON [dbo].[Products]([CategoryId])

    ALTER TABLE [dbo].[Products] ADD CONSTRAINT [FK_dbo.Products_dbo.Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Categories] ([CategoryId]) ON DELETE CASCADE
```

#### Reverse Engineer Model

We’re going to make use of Entity Framework Designer, which is included as part of Visual Studio, to create our model.

- **Project -&gt; Add New Item…**
- Select **Data** from the left menu and then **ADO.NET Entity Data Model**
- Enter **ProductModel** as the name and click **OK**
- This launches the **Entity Data Model Wizard**
- Select **Generate from Database** and click **Next**

    ![Choose Model Contents](~/ef6/media/choosemodelcontents.png)

- Select the connection to the database you created in the first section, enter **ProductContext** as the name of the connection string and click **Next**

    ![Choose Your Connection](~/ef6/media/chooseyourconnection.png)

- Click the checkbox next to ‘Tables’ to import all tables and click ‘Finish’

    ![Choose Your Objects](~/ef6/media/chooseyourobjects.png)

Once the reverse engineer process completes the new model is added to your project and opened up for you to view in the Entity Framework Designer. An App.config file has also been added to your project with the connection details for the database.

#### Additional Steps in Visual Studio 2010

If you are working in Visual Studio 2010 then you will need to update the EF designer to use EF6 code generation.

- Right-click on an empty spot of your model in the EF Designer and select **Add Code Generation Item…**
- Select **Online Templates** from the left menu and search for **DbContext**
- Select the **EF 6.x DbContext Generator for C\#,** enter **ProductsModel** as the name and click Add

#### Updating code generation for data binding

EF generates code from your model using T4 templates. The templates shipped with Visual Studio or downloaded from the Visual Studio gallery are intended for general purpose use. This means that the entities generated from these templates have simple ICollection&lt;T&gt; properties. However, when doing data binding using WPF it is desirable to use **ObservableCollection** for collection properties so that WPF can keep track of changes made to the collections. To this end we will to modify the templates to use ObservableCollection.

- Open the **Solution Explorer** and find **ProductModel.edmx** file
- Find the **ProductModel.tt** file which will be nested under the ProductModel.edmx file

    ![WPF Product Model Template](~/ef6/media/wpfproductmodeltemplate.png)

- Double-click on the ProductModel.tt file to open it in the Visual Studio editor
- Find and replace the two occurrences of “**ICollection**” with “**ObservableCollection**”. These are located approximately at lines 296 and 484.
- Find and replace the first occurrence of “**HashSet**” with “**ObservableCollection**”. This occurrence is located approximately at line 50. **Do not** replace the second occurrence of HashSet found later in the code.
- Find and replace the only occurrence of “**System.Collections.Generic**” with “**System.Collections.ObjectModel**”. This is located approximately at line 424.
- Save the ProductModel.tt file. This should cause the code for entities to be regenerated. If the code does not regenerate automatically, then right click on ProductModel.tt and choose “Run Custom Tool”.

If you now open the Category.cs file (which is nested under ProductModel.tt) then you should see that the Products collection has the type **ObservableCollection&lt;Product&gt;**.

Compile the project.

## Lazy Loading

The **Products** property on the **Category** class and **Category** property on the **Product** class are navigation properties. In Entity Framework, navigation properties provide a way to navigate a relationship between two entity types.

EF gives you an option of loading related entities from the database automatically the first time you access the navigation property. With this type of loading (called lazy loading), be aware that the first time you access each navigation property a separate query will be executed against the database if the contents are not already in the context.

When using POCO entity types, EF achieves lazy loading by creating instances of derived proxy types during runtime and then overriding virtual properties in your classes to add the loading hook. To get lazy loading of related objects, you must declare navigation property getters as **public** and **virtual** (**Overridable** in Visual Basic), and your class must not be **sealed** (**NotOverridable** in Visual Basic). When using Database First navigation properties are automatically made virtual to enable lazy loading. In the Code First section we chose to make the navigation properties virtual for the same reason.

## Bind Object to Controls

Add the classes that are defined in the model as data sources for this WPF application.

- Double-click **MainWindow.xaml** in Solution Explorer to open the main form
- From the main menu, select **Project -&gt; Add New Data Source …**
    (in Visual Studio 2010, you need to select **Data -&gt; Add New Data Source…**)
- In the Choose a Data Source Typewindow, select **Object** and click **Next**
- In the Select the Data Objects dialog, unfold the **WPFwithEFSample** two times and select **Category**  
    *There is no need to select the **Product** data source, because we will get to it through the **Product**’s property on the **Category** data source*  

    ![Select Data Objects](~/ef6/media/selectdataobjects.png)

- Click **Finish.**
- The Data Sources window is opened next to the MainWindow.xaml window
    *If the Data Sources window is not showing up, select **View -&gt; Other Windows-&gt; Data Sources***
- Press the pin icon, so the Data Sources window does not auto hide. You may need to hit the refresh button if the window was already visible.

    ![Data Sources](~/ef6/media/datasources.png)

- Select the **Category** data source and drag it on the form.

The following happened when we dragged this source:

- The **categoryViewSource** resource and the **categoryDataGrid** control were added to XAML
- The DataContext property on the parent Grid element was set to "{StaticResource **categoryViewSource** }". The **categoryViewSource** resource serves as a binding source for the outer\\parent Grid element. The inner Grid elements then inherit the DataContext value from the parent Grid (the categoryDataGrid’s ItemsSource property is set to "{Binding}")

``` xml
    <Window.Resources>
        <CollectionViewSource x:Key="categoryViewSource"
                                d:DesignSource="{d:DesignInstance {x:Type local:Category}, CreateList=True}"/>
    </Window.Resources>
    <Grid DataContext="{StaticResource categoryViewSource}">
        <DataGrid x:Name="categoryDataGrid" AutoGenerateColumns="False" EnableRowVirtualization="True"
                    ItemsSource="{Binding}" Margin="13,13,43,191"
                    RowDetailsVisibilityMode="VisibleWhenSelected">
            <DataGrid.Columns>
                <DataGridTextColumn x:Name="categoryIdColumn" Binding="{Binding CategoryId}"
                                    Header="Category Id" Width="SizeToHeader"/>
                <DataGridTextColumn x:Name="nameColumn" Binding="{Binding Name}"
                                    Header="Name" Width="SizeToHeader"/>
            </DataGrid.Columns>
        </DataGrid>
    </Grid>
```

## Adding a Details Grid

Now that we have a grid to display Categories let's add a details grid to display the associated Products.

- Select the **Products** property from under the **Category** data source and drag it on the form.
  - The **categoryProductsViewSource** resource and **productDataGrid** grid are added to XAML
  - The binding path for this resource is set to Products
  - WPF data-binding framework ensures that only Products related to the selected Category show up in **productDataGrid**
- From the Toolbox, drag **Button** on to the form. Set the **Name** property to **buttonSave** and the **Content** property to **Save**.

The form should look similar to this:

![Designer Form](~/ef6/media/designer.png)

## Add Code that Handles Data Interaction

It's time to add some event handlers to the main window.

- In the XAML window, click on the **&lt;Window** element, this selects the main window
- In the **Properties** window choose **Events** at the top right, then double-click the text box to right of the **Loaded** label

    ![Main Window Properties](~/ef6/media/mainwindowproperties.png)

- Also add the **Click** event for the **Save** button by double-clicking the Save button in the designer.

This brings you to the code behind for the form, we'll now edit the code to use the ProductContext to perform data access. Update the code for the MainWindow as shown below.

The code declares a long-running instance of **ProductContext**. The **ProductContext** object is used to query and save data to the database. The **Dispose()** on the **ProductContext** instance is then called from the overridden **OnClosing** method. The code comments provide details about what the code does.

``` csharp
    using System.Data.Entity;
    using System.Linq;
    using System.Windows;

    namespace WPFwithEFSample
    {
        public partial class MainWindow : Window
        {
            private ProductContext _context = new ProductContext();
            public MainWindow()
            {
                InitializeComponent();
            }

            private void Window_Loaded(object sender, RoutedEventArgs e)
            {
                System.Windows.Data.CollectionViewSource categoryViewSource =
                    ((System.Windows.Data.CollectionViewSource)(this.FindResource("categoryViewSource")));

                // Load is an extension method on IQueryable,
                // defined in the System.Data.Entity namespace.
                // This method enumerates the results of the query,
                // similar to ToList but without creating a list.
                // When used with Linq to Entities this method
                // creates entity objects and adds them to the context.
                _context.Categories.Load();

                // After the data is loaded call the DbSet<T>.Local property
                // to use the DbSet<T> as a binding source.
                categoryViewSource.Source = _context.Categories.Local;
            }

            private void buttonSave_Click(object sender, RoutedEventArgs e)
            {
                // When you delete an object from the related entities collection
                // (in this case Products), the Entity Framework doesn’t mark
                // these child entities as deleted.
                // Instead, it removes the relationship between the parent and the child
                // by setting the parent reference to null.
                // So we manually have to delete the products
                // that have a Category reference set to null.

                // The following code uses LINQ to Objects
                // against the Local collection of Products.
                // The ToList call is required because otherwise the collection will be modified
                // by the Remove call while it is being enumerated.
                // In most other situations you can use LINQ to Objects directly
                // against the Local property without using ToList first.
                foreach (var product in _context.Products.Local.ToList())
                {
                    if (product.Category == null)
                    {
                        _context.Products.Remove(product);
                    }
                }

                _context.SaveChanges();
                // Refresh the grids so the database generated values show up.
                this.categoryDataGrid.Items.Refresh();
                this.productsDataGrid.Items.Refresh();
            }

            protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
            {
                base.OnClosing(e);
                this._context.Dispose();
            }
        }

    }
```

## Test the WPF Application

- Compile and run the application. If you used Code First, then you will see that a **WPFwithEFSample.ProductContext** database is created for you.
- Enter a category name in the top grid and product names in the bottom grid
    *Do not enter anything in ID columns, because the primary key is generated by the database*

    ![Main Window with new categories and products](~/ef6/media/screen1.png)

- Press the **Save** button to save the data to the database

After the call to DbContext’s **SaveChanges()**, the IDs are populated with the database generated values. Because we called **Refresh()** after **SaveChanges()** the **DataGrid** controls are updated with the new values as well.

![Main Window with IDs populated](~/ef6/media/screen2.png)

## Additional Resources

To learn more about data binding to collections using WPF, see [this topic](/dotnet/framework/wpf/data/data-binding-overview#binding-to-collections) in the WPF documentation.  
