using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ZTP;

public static class UIManager
{
    private static MainWindow MainWindow => MainWindow.Instance;
    
    public static void InitializeMainWindow(MainWindow window)
    {
        // Inicjalizacja jeśli potrzebna
    }
    
    public static void DisplayContent(Control content)
    {
        MainWindow.Desktop.Content = content;
    }
    
    public static void DisplayText(string text)
    {
        DisplayContent(new TextBlock
        {
            Text = text,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            TextWrapping = TextWrapping.Wrap
        });
    }
    
    public static void DisplayGroup(Group group)
    {
        DisplayContent(group.SimpleDisplay());
    }
    
    public static void DisplayNewObjectSelection()
    {
        var noteButton = CreateButton("Nowa notatka", () => MainWindow.Instance.CreateNoteView());
        var taskButton = CreateButton("Nowe zadanie", () => MainWindow.Instance.CreateTaskView());
        
        var panel = new StackPanel
        {
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Orientation = Orientation.Horizontal,
            Spacing = 10
        };
        
        panel.Children.Add(noteButton);
        panel.Children.Add(taskButton);
        
        DisplayContent(panel);
    }
    
    public static void DisplayStatistics()
    {
        DisplayContent(GlobalGroups.GetStatistics());
    }
    
    public static void DisplayUpcomingTasks()
    {
        DisplayContent(GlobalGroups.GetUpcomingTasksReport(7));
    }
    
    public static void DisplaySearchResults(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return;
            
        DisplayContent(GlobalGroups.Search(query));
    }
    
    public static void DisplayByTagOrCategory(string tagOrCategory)
    {
        DisplayContent(GlobalGroups.Search(tagOrCategory));
    }
    
    public static StackPanel CreateNoteEditor(
        string defaultTitle,
        out TextBox titleBox,
        out TextBox contentBox,
        ComboBox categoryComboBox,
        TextBox tagsTextBox,
        Action onSave)
    {
        var panel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Spacing = 10,
            Margin = new Thickness(20)
        };
        
        titleBox = new TextBox
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Text = defaultTitle,
            Margin =  new Thickness(0,0,10,0),
        };
        
        contentBox = new TextBox
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            MinHeight = 200,
            AcceptsReturn = true,
            Margin =  new Thickness(0,0,10,0),
        };
        
        var bottomPanel = CreateBottomPanel(categoryComboBox, tagsTextBox, onSave, null);
        
        panel.Children.Add(titleBox);
        panel.Children.Add(contentBox);
        panel.Children.Add(bottomPanel);
        
        return panel;
    }
    
    public static StackPanel CreateTaskEditor(
        string defaultTitle,
        out TextBox titleBox,
        StackPanel tasksSection,
        Action onAddTask,
        ComboBox categoryComboBox,
        TextBox tagsTextBox,
        ComboBox priorityComboBox,
        Action onSave)
    {
        var panel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Spacing = 10,
            Margin = new Thickness(20)
        };
        
        titleBox = new TextBox
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Text = defaultTitle
        };
        
        var addButtonSection = new StackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Center
        };
        
        var addButton = CreateButton("Dodaj zadanie", onAddTask);
        addButtonSection.Children.Add(addButton);
        
        var bottomPanel = CreateBottomPanel(categoryComboBox, tagsTextBox, onSave, priorityComboBox);
        
        panel.Children.Add(titleBox);
        panel.Children.Add(tasksSection);
        panel.Children.Add(addButtonSection);
        panel.Children.Add(bottomPanel);
        
        return panel;
    }
    
    public static StackPanel CreateTaskInputRow(
        out TextBox taskTextBox,
        out DatePicker datePicker)
    {
        var panel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 5
        };
        
        taskTextBox = new TextBox
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Watermark = "nowe zadanie",
            Width = 400
        };
        
        datePicker = new DatePicker();
        
        panel.Children.Add(taskTextBox);
        panel.Children.Add(datePicker);
        
        return panel;
    }
    
    private static Button CreateButton(string content, Action onClick)
    {
        var button = new Button { Content = content };
        button.Click += (s, e) => onClick();
        return button;
    }
    
    private static StackPanel CreateBottomPanel(
        ComboBox categoryComboBox,
        TextBox tagsTextBox,
        Action onSave,
        ComboBox priorityComboBox)
    {
        var grid = new Grid
        {
            ColumnDefinitions = ColumnDefinitions.Parse("Auto, *")
        };
        
        var leftPanel = new StackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            Orientation = Orientation.Horizontal,
            Spacing = 5
        };
        Grid.SetColumn(leftPanel, 0);
        
        leftPanel.Children.Add(categoryComboBox);
        leftPanel.Children.Add(tagsTextBox);
        
        if (priorityComboBox != null)
        {
            leftPanel.Children.Add(priorityComboBox);
        }
        
        var rightPanel = new StackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        Grid.SetColumn(rightPanel, 1);
        
        var saveButton = CreateButton(
            priorityComboBox != null ? "Zapisz zadania" : "Zapisz notatkę",
            onSave);
        rightPanel.Children.Add(saveButton);
        
        grid.Children.Add(leftPanel);
        grid.Children.Add(rightPanel);
        
        var wrapper = new StackPanel();
        wrapper.Children.Add(grid);
        
        return wrapper;
    }
    
    public static void ShowValidationError(Control control, bool hasError)
    {
        if (control == null) return;
        
        if (hasError)
            control.Classes.Add(control is DatePicker ? "wrongDate" : "mustFill");
        else
            control.Classes.Remove(control is DatePicker ? "wrongDate" : "mustFill");
    }
    
    public static StackPanel CreateMenuSection(List<Button> buttons, Action<string> onButtonClick)
    {
        var panel = new StackPanel();
        
        foreach (var button in buttons)
        {
            button.Click += (s, e) => onButtonClick(button.Name);
            panel.Children.Add(button);
        }
        
        return panel;
    }
}