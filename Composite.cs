using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ZTP;

public enum Priorities
{
    None,
    Low,
    Normal,
    Important
}

public interface IComponent : IVisitedComponent
{
    string Name { get; }
    DateTime StartDate { get; }
    List<string> Tags { get; }
    string Category { get; }
    StackPanel SimpleDisplay(int depth);
    StackPanel SimpleDisplay();
}

public class Note : IComponent
{
    public string Name { get; set; }
    public int NoteId { get; set; }
    public string Content { get; set; }
    public DateTime StartDate { get; }
    public List<string> Tags { get; set; } = new List<string>();
    public string Category { get; set; } = "";

    public Note(string name, string content)
    {
        Name = name;
        Content = content;
        StartDate = DateTime.Now;
    }

    public Note(Note other)
    {
        Name = other.Name;
        Content = other.Content;
        StartDate = other.StartDate;
    }

    public void SetCategory(string category)
    {
        Category = category;
    }
    public void SetId(int id)
    {
        NoteId = id;
    }
    public void SetTags(List<string> tags)
    {
        Tags = tags;
    }
    public void SetTags(string tag)
    {
        Tags.Add(tag);
    }
    public StackPanel SimpleDisplay(int depth)
    {
        var mainSection = new StackPanel { Margin = new Thickness(10 * depth, 5, 5, 10) };

        // Tytuł notatki jako TextBox
        var titleButton = new Button
        {
            Content = Name,
            FontSize = 14,
            FontWeight = FontWeight.Bold,
        };
        titleButton.Classes.Add("leftMenuButton");
        titleButton.Click += (s, e) => MainWindow.Instance.EditDisplay(this);
        mainSection.Children.Add(titleButton);

        // Kategoria
        if (Category != "")
        {
            var catBox = new TextBlock
            {
                Text = $"Kategoria: {Category}",
                FontSize = 11,
                Foreground = Brushes.Gray,
                Background = Brushes.Transparent,
                Margin = new Thickness(10, 0, 0, 0)
            };
            mainSection.Children.Add(catBox);
        }

        // Tagi
        if (Tags.Count > 0)
        {
            var tagBox = new TextBlock
            {
                Text = $"Tagi: #{string.Join(", #", Tags.ToArray())}",
                FontSize = 11,
                Foreground = Brushes.Gray,
                Background = Brushes.Transparent,
                Margin = new Thickness(10, 0, 0, 0)
            };
            mainSection.Children.Add(tagBox);
        }

        // Treść notatki
        if (!string.IsNullOrEmpty(Content))
        {
            var contentBox = new TextBox
            {
                Text = Content,
                IsReadOnly = true,
                TextWrapping = TextWrapping.Wrap,
                BorderThickness = new Thickness(0),
                Background = Brushes.Transparent,
                Margin = new Thickness(10, 0, 0, 0)
            };
            mainSection.Children.Add(contentBox);
        }

        return mainSection;
    }
    public StackPanel SimpleDisplay()
    {
        return SimpleDisplay(1);
    }
    public StackPanel DisplayDetails()
    {
        var mainSection = new StackPanel
        {
            Orientation = Avalonia.Layout.Orientation.Vertical,
            Spacing = 10
        };

        var inputTitle = new TextBox
        {
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
            Text = Name,
            AcceptsReturn = true
        };

        var inputContent = new TextBox
        {
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
            MinHeight = 300,
            Text = Content,
            AcceptsReturn = true
        };

        var dateBox = new TextBlock
        {
            Text = $"Utworzono: {StartDate:dd.MM.yyyy HH:mm}",
            FontSize = 11,
            Foreground = Brushes.Gray,
            Background = Brushes.Transparent
        };

        




        var downSection = new Grid { ColumnDefinitions = ColumnDefinitions.Parse("Auto, *") };

        var inputCategory = GlobalGroups.SelectableCategoryList();
        inputCategory.SelectedItem = Category;
        var inputTags = new TextBox { Text = string.Join(",", Tags) , MaxWidth = 200 };
        //var inputPriority = new ComboBox { ItemsSource = Enum.GetValues<Priorities>() };

        var leftSide = new StackPanel
        {
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left,
            Orientation = Avalonia.Layout.Orientation.Horizontal,
            Spacing = 5
        };
        Grid.SetColumn(leftSide, 0);
        leftSide.Children.Add(inputCategory);
        leftSide.Children.Add(inputTags);
        //leftSide.Children.Add(inputPriority);

        var saveButton = new Button { Content = "Zapisz", HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right };
        saveButton.Click += (s, e) =>
        {
            Name = inputTitle.Text;
            Content = inputContent.Text;
            Category = inputCategory.SelectedItem?.ToString().ToLower().Trim();
            if(inputTags.Text != string.Join(",", Tags))
            {
                string[] tags = inputTags.Text.Split(',');
                foreach(var t in tags)
                {
                    t.Trim().ToLower();
                    if(t != null) SetTags(t);
                }
            }
            ServerConnection client = ServerConnection.CreateServerConnection();
            client.UpdateNote(this, this.NoteId);
        };


        var rightSide = new StackPanel { HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right };
        Grid.SetColumn(rightSide, 1);
        rightSide.Children.Add(saveButton);

        downSection.Children.Add(leftSide);
        downSection.Children.Add(rightSide);

        mainSection.Children.Add(inputTitle);
        mainSection.Children.Add(inputContent);
        mainSection.Children.Add(dateBox);
        mainSection.Children.Add(downSection);
        return mainSection;
    }

    public void Accept(IVisitor visitor) //for Visitor use
    {
        visitor.Visit(this);
    }

}

public interface ITaskComponent : IComponent
{
    DateTime? EndDate { get; }
    bool IsCompleted { get; }
    bool IsLate { get; }
    Priorities Priority { get; }
    public void MarkAsCompleted(DateTime completionDate);
    public string GetStatus();
    void SetPriority(Priorities priority);
    public void SetTags(string tag);
    public void SetCategory(string category);
}

public class Task : ITaskComponent
{
    public string Name { get; }
    public int TaskId { get; set; }
    public DateTime StartDate { get; }
    public DateTime? EndDate { get; }
    public Priorities Priority { get; private set; } = 0;
    public bool IsCompleted { get; private set; } = false;
    public bool IsLate { get; private set; } = false;
    public List<string> Tags { get; set; } = new List<string>();
    public string Category { get; set; } = "";

    public Task(string name, DateTime endDate)
    {
        Name = name;
        StartDate = DateTime.Now;
        EndDate = endDate;
    }

    public Task(string name)
    {
        Name = name;
        StartDate = DateTime.Now;
        EndDate = null;
    }

    public Task(Task other)
    {
        Name = other.Name;
        StartDate = other.StartDate;
        EndDate = other.EndDate;
        Priority = other.Priority;
        IsCompleted = other.IsCompleted;
        IsLate = other.IsLate;
    }
    public void SetPriority(Priorities priority)
    {
        Priority = priority;
    }
    public void SetId(int id)
    {
        TaskId = id;
    }
    public void SetTags(List<string> tags)
    {
        Tags = tags;
    }
    public void SetTags(string tag)
    {
        Tags.Add(tag);
    }

    public void SetCategory(string category)
    {
        Category = category;
    }

    public void MarkAsCompleted(DateTime completionDate)
    {
        IsCompleted = true;
        IsLate = completionDate > EndDate;
    }

    public string GetStatus()
    {
        if (IsCompleted)
            return IsLate ? "[Completed Late]" : "[Completed]";
        return "[Pending]";
    }

    public StackPanel SimpleDisplay(int depth)
    {
        var mainSection = new StackPanel { Margin = new Thickness(10 * depth, 5, 5, 10) };

        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto)); // Checkbox
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));  // Nazwa
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto)); // Data
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto)); // Priorytet

        var nameText = new TextBlock
        {
            Text = Name,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
            Classes = { IsCompleted ? "checkTrue" : "checkFalse" }
        };
        Grid.SetColumn(nameText, 1);

        // Checkbox dla statusu
        var checkBox = new CheckBox { IsChecked = IsCompleted };
        checkBox.IsCheckedChanged += (s, e) =>
        {
            if (checkBox.IsChecked == true)
            {
                MarkAsCompleted(DateTime.Now);
                // Zmień klasę na checkTrue i usuń checkFalse
                nameText.Classes.Remove("checkFalse");
                nameText.Classes.Add("checkTrue");
            }
        };

        Grid.SetColumn(checkBox, 0);
        grid.Children.Add(checkBox);
        grid.Children.Add(nameText);


        // Data
        if (EndDate != null)
        {
            var dateText = new TextBlock
            {
                Text = $"({EndDate:dd.MM.yyyy})",
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                FontSize = 12,
                Foreground = Brushes.Gray
            };
            Grid.SetColumn(dateText, 2);

            grid.Children.Add(dateText);
        }

        // Kategoria
        if (Category != "")
        {
            var catText = new TextBlock
            {
                Text = $"Kategoria: {Category}",
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                FontSize = 12,
                Foreground = Brushes.Gray
            };
            Grid.SetColumn(catText, 2);
        }

        // Tagi
        if (Tags.Count() > 0)
        {
            var catText = new TextBlock
            {
                Text = $"Tagi: {string.Join(",", Tags.ToArray())}",
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                FontSize = 12,
                Foreground = Brushes.Gray
            };
            Grid.SetColumn(catText, 2);
        }

        // Priorytet
        var priorityIcon = new TextBlock
        {
            Text = GetPriorityIcon(),
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
            FontSize = 14,
            Margin = new Thickness(10, 0, 0, 0)
        };
        Grid.SetColumn(priorityIcon, 3);
        grid.Children.Add(priorityIcon);

        mainSection.Children.Add(grid);
        return mainSection;
    }
    public StackPanel SimpleDisplay()
    {
        return SimpleDisplay(1);
    }

    private string GetPriorityIcon()
    {
        return Priority switch
        {
            Priorities.Important => "🔴",
            Priorities.Normal => "🔵",
            Priorities.Low => "⚪",
            _ => ""
        };
    }
    public void Accept(IVisitor visitor) //for Visitor use
    {
        visitor.Visit(this);
    }
}













public class TaskList : ITaskComponent
{
    public string Name { get; }
    public List<ITaskComponent> components { get; set; } = new List<ITaskComponent>();
    public List<string> Tags { get; set; } = new List<string>();
    public string Category { get; set; } = "";

    public DateTime StartDate
    {
        get
        {
            if (components.Count == 0)
                return DateTime.MinValue;
            return components.Min(component => component.StartDate);
        }
    }

    public DateTime? EndDate
    {
        get
        {
            if (components.Count == 0)
                return DateTime.MaxValue;
            return components.Min(component => component.EndDate);
        }
    }

    public bool IsCompleted
    {
        get
        {
            return components.Count > 0 && components.All(component => component.IsCompleted);
        }
    }

    public bool IsLate
    {
        get
        {
            return components.Count > 0 && components.Any(component => component.IsLate);
        }
    }

    public Priorities Priority { get; private set; } = 0;

    public TaskList(string name)
    {
        Name = name;
    }

    public TaskList(string name, List<ITaskComponent> list)
    {
        Name = name;
        components = list;
    }

    public TaskList(TaskList other)
    {
        Name = other.Name;
        components = other.components;
    }

    public void Add(ITaskComponent component)
    {
        components.Add(component);
    }

    public void Remove(ITaskComponent component)
    {
        components.Remove(component);
    }

    public void SetPriority(Priorities priority)
    {
        Priority = priority;
    }

    public void MarkAsCompleted(DateTime completionDate)
    {
        foreach (var component in components)
        {
            component.MarkAsCompleted(completionDate);
        }
    }

    public void SetTags(List<string> tags)
    {
        Tags = tags;
    }
    public void SetTags(string tag)
    {
        Tags.Add(tag);
    }

    public void SetCategory(string category)
    {
        Category = category;
    }

    public string GetStatus()
    {
        if (IsCompleted)
            return IsLate ? "[Completed Late]" : "[Completed]";
        return "[Pending]";
    }
    private int[] getStatistics()
    {
        int[] statistics = new int[4] {
            components.OfType<Task>().Count(t => t.IsCompleted && !t.IsLate),
            components.OfType<Task>().Count(t => t.IsCompleted && t.IsLate),
            components.OfType<Task>().Count(t => !t.IsCompleted),
            components.OfType<Task>().Count(t => !t.IsCompleted && DateTime.Now > t.EndDate)
        };

        foreach (TaskList group in components.OfType<TaskList>())
        {
            int[] groupReport = group.getStatistics();
            for (int i = 0; i < statistics.Length; i++)
            {
                statistics[i] += groupReport[i];
            }
        }

        return statistics;
    }



    public StackPanel SimpleDisplay(int depth)
    {
        var mainSection = new StackPanel { Margin = new Thickness(10 * depth, 5, 5, 10) };

        // Tytuł listy zadań
        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto)); // Checkbox
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));  // Nazwa

        // Nazwa listy zadań
        var nameText = new TextBlock
        {
            Text = Name,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
            Classes = { IsCompleted ? "checkTrue" : "checkFalse" }
        };
        Grid.SetColumn(nameText, 1);

        // Checkbox dla statusu
        var checkBox = new CheckBox { IsChecked = IsCompleted };
        checkBox.IsCheckedChanged += (s, e) =>
        {
            if (checkBox.IsChecked == true)
            {
                MarkAsCompleted(DateTime.Now);
                nameText.Classes.Remove("checkFalse");
                nameText.Classes.Add("checkTrue");
            }
        };

        Grid.SetColumn(checkBox, 0);
        grid.Children.Add(checkBox);
        grid.Children.Add(nameText);

        mainSection.Children.Add(grid);

        // Status i informacje
        string infoTextValue;
        if (EndDate.HasValue && EndDate != DateTime.MaxValue)
            infoTextValue = $"Status: {GetStatus()} | Termin: {StartDate:dd.MM.yyyy} - {EndDate:dd.MM.yyyy}";
        else
            infoTextValue = $"Status: {GetStatus()} | Termin: {StartDate:dd.MM.yyyy}";
        var infoText = new TextBlock
        {
            Text = infoTextValue,
            FontSize = 12,
            Foreground = Brushes.Gray,
            Margin = new Thickness(10 * depth, 0, 0, 0)
        };
        mainSection.Children.Add(infoText);

        // Kategoria
        if (Category != "" && Category != null)
        {
            var catText = new TextBlock
            {
                Text = $"Kategoria: {Category}",
                FontSize = 12,
                Foreground = Brushes.Gray,
                Margin = new Thickness(10 * depth, 0, 0, 0)
            };
            mainSection.Children.Add(catText);
        }

        // Tagi
        if (Tags.Count > 0)
        {
            var tagText = new TextBlock
            {
                Text = $"Tagi: #{string.Join(", #", Tags.ToArray())}",
                FontSize = 12,
                Foreground = Brushes.Gray,
                Margin = new Thickness(10 * depth, 0, 0, 0)
            };
            mainSection.Children.Add(tagText);
        }

        // Zadania w liście
        foreach (var c in components)
        {
            mainSection.Children.Add(c.SimpleDisplay(depth + 1));
        }
        return mainSection;
    }
    public StackPanel SimpleDisplay()
    {
        return SimpleDisplay(1);
    }
    public void Accept(IVisitor visitor) //for Visitor use
    {
        visitor.Visit(this);
    }
}


















public class Group : IComponent
{
    public string Name { get; }
    private List<IComponent> components = new List<IComponent>();
    public DateTime StartDate
    {
        get
        {
            if (components.Count == 0)
                return DateTime.MinValue;
            return components.Min(component => component.StartDate);
        }
    }
    public List<string> Tags { get; }
    public string Category { get; }

    public Group(string name)
    {
        Name = name;
    }

    public void Add(IComponent component)
    {
        components.Add(component);
    }

    public void Remove(IComponent component)
    {
        components.Remove(component);
    }

    public bool Contains(IComponent component)
    {
        return components.Contains(component);
    }

    public int Count()
    {
        return components.Count();
    }

    public IReadOnlyList<IComponent> GetComponents()
    {
        return components.AsReadOnly();
    }

    public StackPanel SimpleDisplay(int depth)
    {

        var mainSection = new StackPanel { Margin = new Thickness(10 * depth, 5, 5, 10) };

        // Tytuł grupy
        var titleText = new TextBlock
        {
            Text = Name,
            FontSize = 14,
            FontWeight = FontWeight.Bold
        };
        mainSection.Children.Add(titleText);

        // Licznik elementów
        var counterText = new TextBlock
        {
            Text = $"({Count()} elementów)",
            FontSize = 12,
            Foreground = Brushes.Gray,
            Margin = new Thickness(10, 0, 0, 10)
        };
        mainSection.Children.Add(counterText);

        // Elementy grupy
        foreach (var c in components)
        {
            mainSection.Children.Add(c.SimpleDisplay(depth + 1));
        }
        return mainSection;
    }
    public StackPanel SimpleDisplay()
    {
        return SimpleDisplay(1);
    }
    public void Accept(IVisitor visitor) //for Visitor use
    {
        visitor.Visit(this);
    }

}