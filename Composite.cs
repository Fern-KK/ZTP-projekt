using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using HarfBuzzSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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

public interface IComponent
{
    public string Name { get; }
    DateTime StartDate { get; }
    // public List<string> Tags { get; }
    // public string Category { get; }
    
    public string Display(int depth);
    public string Display();
    public StackPanel SimpleDisplay(int depth);
    public StackPanel SimpleDisplay();
}

public class Note : IComponent
{
    public string Name { get; }
    public string Content { get; }
    public DateTime StartDate { get; }
    public List<string> Tags { get; set;} = new List<string>();
    public string Category { get; set;}

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
        Category=category;
    }
    public void SetTags(List<string> tags)
    {
        Tags=tags;
    }
    public void SetTags(string tag)
    {
        Tags.Add(tag);
    }

    public string Display()
    {
        return this.Display(1);
    }

    public string Display(int depth)
    {
        string indent = new string(' ', depth);
        string dashPrefix = new string('-', depth);

        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"{dashPrefix}{Name} ({StartDate:dd.MM.yyyy})");
        sb.AppendLine($"{indent}Treść: {Content}");

        return sb.ToString();
    }

    public StackPanel SimpleDisplay(int depth)
    {
        var mainSection = new StackPanel{ Margin = new Thickness(10*depth, 5)};

        // Tytuł notatki jako TextBox
        var titleBox = new TextBox{Text = $"📝 {Name}, {Category}, {string.Join( ",", Tags.ToArray() )}",
                                   FontSize = 14,
                                   FontWeight = FontWeight.SemiBold,
                                   Margin = new Thickness(0, 0, 0, 5),
                                   IsReadOnly = true,
                                   BorderThickness = new Thickness(0),
                                   Background = Brushes.Transparent};
        mainSection.Children.Add(titleBox);

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

        // Data utworzenia
        var dateBox = new TextBlock
        {
            Text = $"Utworzono: {StartDate:dd.MM.yyyy HH:mm}",
            FontSize = 11,
            Foreground = Brushes.Gray,
            Background = Brushes.Transparent,
            Margin = new Thickness(10, 0, 0, 0)
        };
        mainSection.Children.Add(dateBox);

        return mainSection;
    }
    public StackPanel SimpleDisplay()
    {
        return SimpleDisplay(1);
    }
    public StackPanel DisplayDetails()
    {
        var mainSection = new StackPanel{Orientation = Avalonia.Layout.Orientation.Vertical,
                                   Spacing = 10,
                                   Margin = new Thickness(20)};

        var inputTitle = new TextBox{HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                                     Text = Name,
                                     AcceptsReturn = true};

        var inputContent = new TextBox{HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                                       MinHeight = 300,
                                       Text = Content,
                                       AcceptsReturn = true};
        
        var dateBox = new TextBlock{Text = $"Utworzono: {StartDate:dd.MM.yyyy HH:mm}",
                                    FontSize = 11,
                                    Foreground = Brushes.Gray,
                                    Background = Brushes.Transparent};

        var saveButton = new Button{Content = "Zapisz notatkę",
                                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                                    Width = 120,
                                    Margin = new Thickness(0, 10, 0, 0)};
        // saveButton.Click += (s, e) => NoteBuilder();
        
        mainSection.Children.Add(inputTitle);
        mainSection.Children.Add(inputContent);
        mainSection.Children.Add(dateBox);
        mainSection.Children.Add(saveButton);
        return mainSection;
    }
    
}

public interface ITaskComponent : IComponent
{
    DateTime EndDate { get; }
    bool IsCompleted { get; }
    bool IsLate { get; }
    Priorities Priority { get; }
    public void MarkAsCompleted(DateTime completionDate);
    public string GetStatus();
    void SetPriority(Priorities priority);
}

public class Task : ITaskComponent
{
    public string Name { get; }
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }
    public Priorities Priority { get; private set; } = 0;
    public bool IsCompleted { get; private set; } = false;
    public bool IsLate { get; private set; } = false;

    public Task(string name, DateTime endDate)
    {
        Name = name;
        StartDate = DateTime.Now;
        EndDate = endDate;
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

    public void SetPriority(Priorities priority)
    {
        Priority = priority;
    }

    public string Display()
    {
        return this.Display(1);
    }

    public string Display(int depth)
    {
        string dashPrefix = new string('-', depth);
        return $"{dashPrefix}{Name} ({StartDate:dd.MM.yyyy} to {EndDate:dd.MM.yyyy}) - Status: {GetStatus()}\n";
    }

    public StackPanel SimpleDisplay(int depth)
    {
        var mainSection = new StackPanel
        {
            Spacing = 10,
            Margin = new Thickness(10*depth, 5)
        };

        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto)); // Checkbox
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));  // Nazwa
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto)); // Data
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto)); // Priorytet

        // Checkbox dla statusu
        var checkBox = new CheckBox
        {
            IsChecked = IsCompleted,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
            Margin = new Thickness(0, 0, 10, 0)
        };
        checkBox.IsCheckedChanged += (s, e) =>
        {
            if (checkBox.IsChecked == true)
                MarkAsCompleted(DateTime.Now);
        };
        Grid.SetColumn(checkBox, 0);

        // Nazwa zadania
        var nameText = new TextBlock
        {
            Text = Name,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
            FontWeight = IsCompleted ? FontWeight.Normal : FontWeight.Bold,
            TextDecorations = IsCompleted ? TextDecorations.Strikethrough : null
        };
        Grid.SetColumn(nameText, 1);

        // Data
        var dateText = new TextBlock
        {
            Text = $"({EndDate:dd.MM.yyyy})",
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
            FontSize = 12,
            Foreground = Brushes.Gray
        };
        Grid.SetColumn(dateText, 2);

        // Priorytet
        var priorityIcon = new TextBlock
        {
            Text = GetPriorityIcon(),
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
            FontSize = 14,
            Margin = new Thickness(10, 0, 0, 0)
        };
        Grid.SetColumn(priorityIcon, 3);

        grid.Children.Add(checkBox);
        grid.Children.Add(nameText);
        grid.Children.Add(dateText);
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
            Priorities.Important => "⚠️",
            Priorities.Normal => "🔵",
            Priorities.Low => "⚪",
            _ => ""
        };
    }
}

public class TaskList : ITaskComponent
{
    public string Name { get; }
    private List<ITaskComponent> components = new List<ITaskComponent>();

    public DateTime StartDate
    {
        get
        {
            if (components.Count == 0)
                return DateTime.MinValue;
            return components.Min(component => component.StartDate);
        }
    }

    public DateTime EndDate
    {
        get
        {
            if (components.Count == 0)
                return DateTime.MaxValue;
            return components.Max(component => component.EndDate);
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

    public string GetStatus()
    {
        if (IsCompleted)
            return IsLate ? "[Completed Late]" : "[Completed]";
        return "[Pending]";
    }

    public string Display()
    {
        return this.Display(1);
    }

    public string Display(int depth)
    {
        StringBuilder sb = new StringBuilder();
        string dashPrefix = new string('-', depth);

        sb.AppendLine($"{dashPrefix}{Name} ({StartDate:dd.MM.yyyy} to {EndDate:dd.MM.yyyy}) - Status: {GetStatus()}");

        foreach (var component in components)
        {
            sb.Append(component.Display(depth + 2));
        }

        return sb.ToString();
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

    public string Report()
    {
        int[] stat = this.getStatistics();
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("\nPodsumowanie zadań:");
        sb.AppendLine($"Zadania wykonane na czas: {stat[0]}");
        sb.AppendLine($"Zadania wykonane z opóźnieniem: {stat[1]}");
        sb.AppendLine($"Zadania oczekujące: {stat[2]}");
        sb.AppendLine($"Zadania oczekujące z przekroczonym terminem: {stat[3]}");

        return sb.ToString();
    }

    public StackPanel SimpleDisplay(int depth)
    {
        var mainSection = new StackPanel
        {
            Spacing = 10,
            Margin = new Thickness(10*depth, 5)
        };

        // Tytuł listy zadań
        var titleText = new TextBlock
        {
            Text = $"📋 {Name}",
            FontSize = 14,
            FontWeight = FontWeight.SemiBold,
            Margin = new Thickness(0, 0, 0, 5)
        };

        mainSection.Children.Add(titleText);

        // Status i informacje
        var infoText = new TextBlock
        {
            Text = $"Status: {GetStatus()} | Termin: {StartDate:dd.MM.yyyy} - {EndDate:dd.MM.yyyy}",
            FontSize = 12,
            Foreground = Brushes.Gray,
            Margin = new Thickness(10, 0, 0, 10)
        };
        mainSection.Children.Add(infoText);

        // Zadania w liście
        foreach(var c in components)
        {
            mainSection.Children.Add(c.SimpleDisplay(depth+1));
        }
        return mainSection;
    }
    public StackPanel SimpleDisplay()
    {
        return SimpleDisplay(1);
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

    public string Display()
    {
        return this.Display(1);
    }

    public string Display(int depth)
    {
        StringBuilder sb = new StringBuilder();

        foreach (var component in components)
        {
            sb.Append(component.Display(depth + 2));
        }

        return sb.ToString();
    }

    public string GetFormattedList()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"{Name}:");
        sb.AppendLine();

        int i = 1;
        foreach (var component in components)
        {
            sb.AppendLine($"{i}. {component.Name}");
            i++;
        }

        return sb.ToString();
    }

    public string GetDetailedList()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"{Name}:");
        sb.AppendLine();

        int i = 1;
        foreach (var component in components)
        {
            sb.AppendLine($"{i}. {component.Display()}");
            i++;
        }

        return sb.ToString();
    }

    public StackPanel SimpleDisplay(int depth)
    {

        var mainSection = new StackPanel{Margin = new Thickness(10*depth, 5)};

        // Tytuł grupy
        var titleText = new TextBlock{Text = $"📂 {Name}",
                                      FontSize = 14,
                                      FontWeight = FontWeight.SemiBold};
        mainSection.Children.Add(titleText);

        // Licznik elementów
        var counterText = new TextBlock{Text = $"({Count()} elementów)",
                                        FontSize = 12,
                                        Foreground = Brushes.Gray,
                                        Margin = new Thickness(10, 0, 0, 10)};
        mainSection.Children.Add(counterText);

        // Elementy grupy
        foreach(var c in components)
        {
            mainSection.Children.Add(c.SimpleDisplay(depth+1));
        }
        return mainSection;
    }
    public StackPanel SimpleDisplay()
    {
        return SimpleDisplay(1);
    }

    public Button DisplayGUI()
    {
        // Tytuł notatki jako TextBox
        var b = new Button{Content=Name,
                           HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                           Background = Brushes.Transparent};
        return b;
    }

}