---
title: "Entity Framework Designer Keyboard Shortcuts - EF6"
author: divega
ms.date: "2016-10-23"
ms.prod: "entity-framework"
ms.author: divega
ms.manager: avickers
ms.technology: entity-framework-6
ms.topic: "article"
ms.assetid: 3c76cdd5-17c5-4c54-a6a5-cf21b974636b
caps.latest.revision: 3
---
# Entity Framework Designer Keyboard Shortcuts
This page provides a list of keyboard shorcuts that are available in the various screens of the Entity Framework Tools for Visual Studio.

## ADO.NET Entity Data Model Wizard

### Step One: Choose Model Contents

![WizardOne](~/ef6/media/wizardone.png)

| Shortcut  | Action                                                     | Notes                                               |
|:----------|:-----------------------------------------------------------|:----------------------------------------------------|
| **Alt+n** | Move to next screen                                        | Not available for all selections of model contents. |
| **Alt+f** | Finish wizard                                              | Not available for all selections of model contents. |
| **Alt+w** | Switch focus to the “What should the model contain?” pane. |                                                     |

### Step Two: Choose Your Connection

![WizardTwo](~/ef6/media/wizardtwo.png)

| Shortcut  | Action                                                     | Notes                                                   |
|:----------|:-----------------------------------------------------------|:--------------------------------------------------------|
| **Alt+n** | Move to next screen                                        |                                                         |
| **Alt+p** | Move to previous screen                                    |                                                         |
| **Alt+w** | Switch focus to the “What should the model contain?” pane. |                                                         |
| **Alt+c** | Open the “Connection Properties” window                    | Allows for the definition of a new database connection. |
| **Alt+e** | Exclude sensitive data from the connection string          |                                                         |
| **Alt+i** | Include sensitive data in the connection string            |                                                         |
| **Alt+s** | Toggle the “Save connection settings in App.Config” option |                                                         |

### Step Three: Choose Your Version

![WizardThree](~/ef6/media/wizardthree.png)

| Shortcut  | Action                                             | Notes                                                                                 |
|:----------|:---------------------------------------------------|:--------------------------------------------------------------------------------------|
| **Alt+n** | Move to next screen                                |                                                                                       |
| **Alt+p** | Move to previous screen                            |                                                                                       |
| **Alt+w** | Switch focus to Entity Framework version selection | Allows for specifying a different version of Entity Framework for use in the project. |

### Step Four: Choose Your Database Objects and Settings

![WizardFour](~/ef6/media/wizardfour.png)

| Shortcut  | Action                                                                                    | Notes                                                               |
|:----------|:------------------------------------------------------------------------------------------|:--------------------------------------------------------------------|
| **Alt+f** | Finish wizard                                                                             |                                                                     |
| **Alt+p** | Move to previous screen                                                                   |                                                                     |
| **Alt+w** | Switch focus to database object selection pane                                            | Allows for specifying database objects to be reverse engineered.    |
| **Alt+s** | Toggle the “Pluralize or singularize generated object names” option                       |                                                                     |
| **Alt+k** | Toggle the “Include foreign key columns in the model” option                              | Not available for all selections of model contents.                 |
| **Alt+i** | Toggle the “Import selected stored procedures and functions into the entity model” option | Not available for all selections of model contents.                 |
| **Alt+m** | Switches focus to the “Model Namespace” text field                                        | Not available for all selections of model contents.                 |
| **Space** | Toggle selection on element                                                               | If element has children, all child elements will be toggled as well |
| **Left**  | Collapse child tree                                                                       |                                                                     |
| **Right** | Expand child tree                                                                         |                                                                     |
| **Up**    | Navigate to previous element in tree                                                      |                                                                     |
| **Down**  | Navigate to next element in tree                                                          |                                                                     |

## EF Designer Surface

![DesignerSurface](~/ef6/media/designersurface.png)

| Shortcut                                                                                | Action                      | Notes                                                                                                                                                                                                                           |
|:----------------------------------------------------------------------------------------|:----------------------------|:--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **Space/Enter**                                                                         | Toggle Selection            | Toggles selection on the object with focus.                                                                                                                                                                                     |
| **Esc**                                                                                 | Cancel Selection            | Cancels the current selection.                                                                                                                                                                                                  |
| **Ctrl + A**                                                                            | Select All                  | Selects all the shapes on the design surface.                                                                                                                                                                                   |
| **Up arrow**                                                                            | Move up                     | Moves selected entity up one grid increment. <br/> If in a list, moves to the previous sibling subfield.                                                                                                                        |
| **Down arrow**                                                                          | Move down                   | Moves selected entity down one grid increment. <br/> If in a list, moves to the next sibling subfield.                                                                                                                          |
| **Left arrow**                                                                          | Move left                   | Moves selected entity left one grid increment. <br/> If in a list, moves to the previous sibling subfield.                                                                                                                      |
| **Right arrow**                                                                         | Move right                  | Moves selected entity right one grid increment. <br/> If in a list, moves to the next sibling subfield.                                                                                                                         |
| **Shift + left arrow**                                                                  | Size shape left             | Reduces the width of the selected entity by one grid increment.                                                                                                                                                                 |
| **Shift + right arrow**                                                                 | Size shape right            | Increases the width of the selected entity by one grid increment.                                                                                                                                                               |
| **Home**                                                                                | First Peer                  | Moves focus and selection to the first object on the design surface at the same peer level.                                                                                                                                     |
| **End**                                                                                 | Last Peer                   | Moves focus and selection to the last object on the design surface at the same peer level.                                                                                                                                      |
| **Ctrl + Home**                                                                         | First Peer (focus)          | Same as first peer, but moves focus instead of moving focus and selection.                                                                                                                                                      |
| **Ctrl + End**                                                                          | Last Peer (focus)           | Same as last peer, but moves focus instead of moving focus and selection.                                                                                                                                                       |
| **Tab**                                                                                 | Next Peer                   | Moves focus and selection to the next object on the design surface at the same peer level.                                                                                                                                      |
| **Shift+Tab**                                                                           | Previous Peer               | Moves focus and selection to the previous object on the design surface at the same peer level.                                                                                                                                  |
| **Alt+Ctrl+Tab**                                                                        | Next Peer (focus)           | Same as next peer, but moves focus instead of moving focus and selection.                                                                                                                                                       |
| **Alt+Ctrl+Shift+Tab**                                                                  | Previous Peer (focus)       | Same as previous peer, but moves focus instead of moving focus and selection.                                                                                                                                                   |
| **&lt;**                                                                                | Ascend                      | Moves to the next object on the design surface one level higher in the hierarchy. If there are no shapes above this shape in the hierarchy (i.e. the object is placed directly on the design surface), the diagram is selected. |
| **&gt;**                                                                                | Descend                     | Moves to the next contained object on the design surface one level below this one in the hierarchy. If there are no contained object, this is a no-op.                                                                          |
| **Ctrl + &lt;**                                                                         | Ascend (focus)              | Same as ascend command, but moves focus without selection.                                                                                                                                                                      |
| **Ctrl + &gt;**                                                                         | Descend (focus)             | Same as descend command, but moves focus without selection.                                                                                                                                                                     |
| **Shift + End**                                                                         | Follow to connected         | From an entity, moves to an entity which this entity is connected to.                                                                                                                                                           |
| **Del**                                                                                 | Delete                      | Delete an object or connector from the diagram.                                                                                                                                                                                 |
| **Ins**                                                                                 | Insert                      | Adds a new property to an entity when either the “Scalar Properties” compartment header or a property itself is selected.                                                                                                       |
| **Pg Up**                                                                               | Scroll diagram up           | Scrolls the design surface up, in increments equal to 75% of the height of the currently visible design surface.                                                                                                                |
| **Pg Down**                                                                             | Scroll diagram down         | Scrolls the design surface down.                                                                                                                                                                                                |
| **Shift + Pg Down**                                                                     | Scroll diagram right        | Scrolls the design surface to the right.                                                                                                                                                                                        |
| **Shift + Pg Up**                                                                       | Scroll diagram left         | Scrolls the design surface to the left.                                                                                                                                                                                         |
| **F2**                                                                                  | Enter edit mode             | Standard keyboard shortcut for entering edit mode for a text control.                                                                                                                                                           |
| **Shift + F10**                                                                         | Display shortcut menu       | Standard keyboard shortcut for displaying a selected item’s shortcut menu.                                                                                                                                                      |
| **Control + Shift + Mouse Left Click**  <br/> **Control + Shift + MouseWheel forward**  | Semantic Zoom In            | Zooms in on the area of the Diagram View beneath the mouse pointer.                                                                                                                                                             |
| **Control + Shift + Mouse Right Click** <br/> **Control + Shift + MouseWheel backward** | Semantic Zoom Out           | Zooms out from the area of the Diagram View beneath the mouse pointer. It re-centers the diagram when you zoom out too far for the current diagram center.                                                                      |
| **Control + Shift + '+'** <br/> **Control + MouseWheel forward**                        | Zoom In                     | Zooms in on the center of the Diagram View.                                                                                                                                                                                     |
| **Control + Shift + '-'** <br/> **Control + MouseWheel backward**                       | Zoom Out                    | Zooms out from the clicked area of the Diagram View. It re-centers the diagram when you zoom out too far for the current diagram center.                                                                                        |
| **Control + Shift + Draw a rectangle with the left mouse button down**                  | Zoom Area                   | Zooms in centered on the area that you've selected. When you hold down the Control + Shift keys, you'll see that the cursor changes to a magnifying glass, which allows you to define the area to zoom into.                    |
| **Context Menu Key + ‘M’**                                                              | Open Mapping Details Window | Opens the Mapping Details window to edit mappings for selected entity                                                                                                                                                           |

## Mapping Details Window

![MappingDetailsShortcuts](~/ef6/media/mappingdetailsshortcuts.png)

| Shortcut                  | Action         | Notes                                                                                                                                 |
|:--------------------------|:---------------|:--------------------------------------------------------------------------------------------------------------------------------------|
| **Tab**                   | Switch Context | Switches between the main window area and the toolbar on the left                                                                     |
| **Arrow keys**            | Navigation     | Move up and down rows, or right and left across columns in the main window area. Move between the buttons in the toolbar on the left. |
| **Enter** <br/> **Space** | Select         | Selects a button in the toolbar on the left.                                                                                          |
| **Alt + Down Arrow**      | Open List      | Drop down a list if a cell is selected that has a drop down list.                                                                     |
| **Enter**                 | List Select    | Selects an element in a drop down list.                                                                                               |
| **Esc**                   | List Close     | Closes a drop down list.                                                                                                              |

## Visual Studio Navigation

Entity Framework also supplies a number of actions that can have custom keyboard shortcuts mapped (no shortcuts are mapped by default). To create these custom shortcuts, click on the Tools menu, then Options.  Under Environment, choose Keyboard.  Scroll down the list in the middle until you can select the desired command, enter the shortcut in the “Press shortcut keys” text box, and click Assign. The possible shortcuts are as follows:

| Shortcut                                                                                       |
|:-----------------------------------------------------------------------------------------------|
| **OtherContextMenus.MicrosoftDataEntityDesignContext.Add.ComplexProperty.ComplexTypes**        |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.AddCodeGenerationItem**                   |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.AddFunctionImport**                       |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.AddNew.AddEnumType**                      |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.AddNew.Association**                      |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.AddNew.ComplexProperty**                  |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.AddNew.ComplexType**                      |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.AddNew.Entity**                           |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.AddNew.FunctionImport**                   |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.AddNew.Inheritance**                      |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.AddNew.NavigationProperty**               |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.AddNew.ScalarProperty**                   |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.AddNewDiagram**                           |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.AddtoDiagram**                            |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.Close**                                   |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.Collapse**                                |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.ConverttoEnum**                           |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.Diagram.CollapseAll**                     |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.Diagram.ExpandAll**                       |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.Diagram.ExportasImage**                   |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.Diagram.LayoutDiagram**                   |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.Edit**                                    |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.EntityKey**                               |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.Expand**                                  |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.FunctionImportMapping**                   |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.GenerateDatabasefromModel**               |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.GoToDefinition**                          |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.Grid.ShowGrid**                           |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.Grid.SnaptoGrid**                         |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.IncludeRelated**                          |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.MappingDetails**                          |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.ModelBrowser**                            |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.MoveDiagramstoSeparateFile**              |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.MoveProperties.Down**                     |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.MoveProperties.Down5**                    |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.MoveProperties.ToBottom**                 |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.MoveProperties.ToTop**                    |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.MoveProperties.Up**                       |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.MoveProperties.Up5**                      |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.MovetonewDiagram**                        |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.Open**                                    |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.Refactor.MovetoNewComplexType**           |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.Refactor.Rename**                         |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.RemovefromDiagram**                       |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.Rename**                                  |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.ScalarPropertyFormat.DisplayName**        |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.ScalarPropertyFormat.DisplayNameandType** |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.Select.BaseType**                         |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.Select.Entity**                           |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.Select.Property**                         |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.Select.Subtype**                          |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.SelectAll**                               |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.SelectAssociation**                       |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.ShowinDiagram**                           |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.ShowinModelBrowser**                      |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.StoredProcedureMapping**                  |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.TableMapping**                            |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.UpdateModelfromDatabase**                 |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.Validate**                                |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.Zoom.10**                                 |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.Zoom.100**                                |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.Zoom.125**                                |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.Zoom.150**                                |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.Zoom.200**                                |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.Zoom.25**                                 |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.Zoom.300**                                |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.Zoom.33**                                 |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.Zoom.400**                                |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.Zoom.50**                                 |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.Zoom.66**                                 |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.Zoom.75**                                 |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.Zoom.Custom**                             |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.Zoom.ZoomIn**                             |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.Zoom.ZoomOut**                            |
| **OtherContextMenus.MicrosoftDataEntityDesignContext.Zoom.ZoomtoFit**                          |
| **View.EntityDataModelBrowser**                                                                |
| **View.EntityDataModelMappingDetails**                                                         |
