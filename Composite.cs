using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.VisualTree;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ZTP;

// Poziomy ważności dla zadań
public enum Priorities
{
    None,
    Low,
    Normal,
    Important
}

// Podstawowy interfejs dla wszystkich elementów systemu (notatek, zadań, grup)
public interface IComponent : IVisitedComponent
{
    string Name { get; }
    DateTime StartDate { get; }
    List<string> Tags { get; }
    string Category { get; }
    StackPanel SimpleDisplay(int depth);
    StackPanel SimpleDisplay();
}

// Pojedyncza notatka
public class Note : IComponent
{
    public string Name { get; set; }
    public int NoteId { get; set; }
    public string Content { get; set; }
    public DateTime StartDate { get; }
    public List<string> Tags { get; set; } = new List<string>();
    public string Category { get; set; } = "";

    // Konstruktory
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

    // Metody modyfikujące stan notatki
    public void SetCategory(string category) => Category = category;
    public void SetId(int id) => NoteId = id;
    public void SetTags(List<string> tags) => Tags = tags;
    public void SetTags(string tag) => Tags.Add(tag);

    // Uproszczony podgląd notatki do wyświetlenia na liście
    public StackPanel SimpleDisplay(int depth)
    {
        var mainSection = new StackPanel { Margin = new Thickness(10 * depth, 5, 5, 10) };

        // Przycisk z tytułem
        var titleButton = new Button
        {
            Content = Name,
            FontSize = 14,
            FontWeight = FontWeight.Bold,
        };
        titleButton.Classes.Add("leftMenuButton");
        titleButton.Click += (s, e) => MainWindow.Instance.EditDisplay(this);
        mainSection.Children.Add(titleButton);

        // Wyświetlanie kategorii i tagów (jeśli istnieją)
        if (Category != "" && Category != null)
            mainSection.Children.Add(new TextBlock { Text = $"Kategoria: {Category}", FontSize = 11, Foreground = Brushes.Gray, Margin = new Thickness(10, 0, 0, 0) });
        if (Tags.Count > 0)
            mainSection.Children.Add(new TextBlock { Text = $"Tagi: #{string.Join(", #", Tags)}", FontSize = 11, Foreground = Brushes.Gray, Margin = new Thickness(10, 0, 0, 0) });

        // Krótki podgląd treści
        if (!string.IsNullOrEmpty(Content))
            mainSection.Children.Add(new TextBlock { Text = Content, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(10, 0, 0, 0) });

        return mainSection;
    }
    public StackPanel SimpleDisplay() => SimpleDisplay(1);
    
    // Pełny formularz edycji notatki z polami wejściowymi i przyciskiem zapisu
    public StackPanel DisplayDetails()
    {
        var mainSection = new StackPanel { Spacing = 10 };

        var inputTitle = new TextBox { Text = Name, AcceptsReturn = true };
        var inputContent = new TextBox { MinHeight = 200, Text = Content, TextWrapping = TextWrapping.Wrap, AcceptsReturn = true };
        var dateBox = new TextBlock { Text = $"Utworzono: {StartDate:dd.MM.yyyy HH:mm}", FontSize = 11, Foreground = Brushes.Gray };

        // Sekcja dolna - kategoria, tagi i przycisk zapisu
        var downSection = new Grid { ColumnDefinitions = ColumnDefinitions.Parse("Auto, *") };
        var inputCategory = GlobalGroups.SelectableCategoryList();
        inputCategory.SelectedItem = Category;
        var inputTags = new TextBox { Text = string.Join(",", Tags), MaxWidth = 200 };

        var leftSide = new StackPanel { Orientation = Avalonia.Layout.Orientation.Horizontal, Spacing = 5 };
        leftSide.Children.Add(inputCategory);
        leftSide.Children.Add(inputTags);

       // Obsługa zapisu i aktualizacji serwera
        var saveButton = new Button { Content = "Zapisz", HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right };
        saveButton.Click += (s, e) =>
        {
            Name = inputTitle.Text;
            Content = inputContent.Text;
            Category = inputCategory.SelectedItem?.ToString().ToLower().Trim();
            
            // Logika aktualizacji tagów
            if (inputTags.Text != string.Join(",", Tags))
            {
                Tags.Clear();
                string[] tags = inputTags.Text.Split(',');
                foreach (var t in tags)
                {
                    if(!string.IsNullOrWhiteSpace(t)) SetTags(t.Trim().ToLower());
                }
            }
            
            ServerConnection.CreateServerConnection().UpdateNote(this, this.NoteId);
            UIManager.DisplayGroup(GlobalGroups.AllGroup);      // Powrót do widoku głównego
        };

        var rightSide = new StackPanel { HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right };
        rightSide.Children.Add(saveButton);

        Grid.SetColumn(leftSide, 0);
        Grid.SetColumn(rightSide, 1);
        downSection.Children.Add(leftSide);
        downSection.Children.Add(rightSide);

        mainSection.Children.Add(inputTitle);
        mainSection.Children.Add(inputContent);
        mainSection.Children.Add(dateBox);
        mainSection.Children.Add(downSection);

        return mainSection;
    }

    public void Accept(IVisitor visitor) => visitor.Visit(this);
}

// Interfejs rozszerzający komponent o dodatkowe funkcjonalności zadań
public interface ITaskComponent : IComponent
{
    DateTime? EndDate { get; }
    bool IsCompleted { get; }
    bool IsLate { get; }
    Priorities Priority { get; }
    void MarkAsCompleted(DateTime completionDate);
    void MarkAsIncomplete();
    string GetStatus();
    void SetPriority(Priorities priority);
    void SetTags(string tag);
    void SetTags(List<string> tags);
    void SetCategory(string category);
}

// Pojedyncze zadanie do wykonania
public class Task : ITaskComponent
{
    public string Name { get; }
    public int TaskId { get; set; }
    public DateTime StartDate { get; }
    public DateTime? EndDate { get; }
    public Priorities Priority { get; private set; } = 0;
    public bool IsCompleted { get; private set; } = false;
    public bool IsLate { get; private set; } = false;
    public string Category { get; set; } = "";
    public List<string> Tags { get; set; } = new List<string>();

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
    public void SetPriority(Priorities priority) => Priority = priority;
    public void SetId(int id) => TaskId = id;
    public void SetTags(string tag) => Tags.Add(tag);
    public void SetTags(List<string> tags) => Tags = tags;
    public void SetCategory(string category) => Category = category;

    public void MarkAsCompleted(DateTime completionDate)
    {
        IsCompleted = true;
        IsLate = completionDate > EndDate;
    }
    public void MarkAsIncomplete() => IsCompleted = false;

    public string GetStatus() => IsCompleted ? (IsLate ? "[Spóźnione, zakończone]" : "[Zakończone]") : "[W toku]";

    // Wyświetla zadanie z Checkboxem i ikoną priorytetu
    public StackPanel SimpleDisplay(int depth)
    {
        var mainSection = new StackPanel { Margin = new Thickness(10 * depth, 5, 5, 10) };

        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto)); // Checkbox
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star)); // Nazwa
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto)); // Data
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto)); // Priorytet

        var nameText = new TextBlock { Text = Name, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center, Classes = { IsCompleted ? "checkTrue" : "checkFalse" } };
        Grid.SetColumn(nameText, 1);

        // Checkbox dla statusu
        var checkBox = new CheckBox { IsChecked = IsCompleted };
        checkBox.Classes.Add("taskCheckbox");
        checkBox.IsCheckedChanged += (s, e) =>
        {
            bool isDone = checkBox.IsChecked == true;
            if (isDone) MarkAsCompleted(DateTime.Now);
            else MarkAsIncomplete();

            // Aktualizacja wyglądu
            nameText.Classes.Set("checkTrue", isDone);
            nameText.Classes.Set("checkFalse", !isDone);
        };

        Grid.SetColumn(checkBox, 0);
        grid.Children.Add(checkBox);
        grid.Children.Add(nameText);

        // Data
        if (EndDate != null)
        {
            var dateText = new TextBlock { Text = $"({EndDate:dd.MM.yyyy})", VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center, FontSize = 12, Foreground = Brushes.Gray };
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
    public StackPanel SimpleDisplay() => SimpleDisplay(1);

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
    public void Accept(IVisitor visitor) => visitor.Visit(this);
}

// Lista zadań do wykonania
public class TaskList : ITaskComponent
{
    public string Name { get; }
    public List<ITaskComponent> components { get; set; } = new List<ITaskComponent>();
    public List<string> Tags { get; set; } = new List<string>();
    public string Category { get; set; } = "";
    public DateTime StartDate => components.Count == 0 ? DateTime.MinValue : components.Min(c => c.StartDate);
    public DateTime? EndDate => components.Count == 0 ? DateTime.MaxValue : components.Min(c => c.EndDate);
    public bool IsCompleted => components.Count > 0 && components.All(c => c.IsCompleted);
    public bool IsLate => components.Any(c => c.IsLate);
    public Priorities Priority { get; private set; } = Priorities.None;

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

    public void Add(ITaskComponent component) => components.Add(component);
    public void MarkAsCompleted(DateTime completionDate) => components.ForEach(c => c.MarkAsCompleted(completionDate));
    public void MarkAsIncomplete() => components.ForEach(c => c.MarkAsIncomplete());
    public string GetStatus() => IsCompleted ? (IsLate ? "[Spóźnione, zakończone]" : "[Zakończone]") : "[W toku]";
    public void SetPriority(Priorities priority) => Priority = priority;

    public void SetTags(string tag) => Tags.Add(tag);
    public void SetTags(List<string> tags) => Tags = tags;
    public void SetCategory(string category) => Category = category;

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

    // Pomocnicza metoda do aktualizacji zadań w liście
    private void UpdateChildrenStyles(StackPanel container, bool isDone)
    {
        // Szukanie wszystkich TextBlocków do dynamicznego aktualizowania UI
        var descendants = container.GetVisualDescendants();
        foreach (var textBlock in descendants.OfType<TextBlock>())
        {
            if (textBlock.Classes.Contains("taskNameLabel"))
            {
                textBlock.Classes.Set("checkTrue", isDone);
                textBlock.Classes.Set("checkFalse", !isDone);
            }
        }
        // To samo dla checkboxów
        foreach (var checkBox in descendants.OfType<CheckBox>())
        {
            if (checkBox.Classes.Contains("taskCheckbox"))
            {
                if (checkBox.IsChecked != isDone)
                    checkBox.IsChecked = isDone;
            }
        }
    }

    // Wyświetla listę zadań oraz wszystkie zadania wewnątrz niej
    public StackPanel SimpleDisplay(int depth)
    {
        var mainSection = new StackPanel { Margin = new Thickness(10 * depth, 5, 5, 10) };

        // Tytuł listy zadań
        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto)); // Checkbox
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star)); // Nazwa

        // Nazwa listy zadań
        var nameText = new TextBlock
        {
            Text = Name,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
            Classes = { IsCompleted ? "checkTrue" : "checkFalse" }
        };
        Grid.SetColumn(nameText, 1);

        // Status i informacje
        var infoText = new TextBlock
        {
            FontSize = 12,
            Foreground = Brushes.Gray,
            Margin = new Thickness(10 * depth, 0, 0, 0)
        };

        // Dynamiczna aktualizacja statusu
        void RefreshStatusLabel()
        {
            infoText.Text = EndDate.HasValue && EndDate != DateTime.MaxValue
                ? $"Status: {GetStatus()} | Termin: {StartDate:dd.MM.yyyy} - {EndDate:dd.MM.yyyy}"
                : $"Status: {GetStatus()} | Termin: {StartDate:dd.MM.yyyy}";
        }

        RefreshStatusLabel();

        // Checkbox dla statusu
        var checkBox = new CheckBox { IsChecked = IsCompleted };
        checkBox.Classes.Add("taskCheckbox");
        checkBox.IsCheckedChanged += (s, e) =>
        {
            bool isDone = checkBox.IsChecked == true;

            if (checkBox.IsPressed || checkBox.IsFocused)
            {
                if (isDone) MarkAsCompleted(DateTime.Now);
                else MarkAsIncomplete();

                UpdateChildrenStyles(mainSection, isDone);
            }

            // Zmiana wizualna checkboxa rodzica
            nameText.Classes.Set("checkTrue", isDone);
            nameText.Classes.Set("checkFalse", !isDone);
            RefreshStatusLabel();
        };

        Grid.SetColumn(checkBox, 0);
        grid.Children.Add(checkBox);
        grid.Children.Add(nameText);

        mainSection.Children.Add(grid);
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
            var childView = c.SimpleDisplay(depth + 1);

            var childCheckBox = childView.GetVisualDescendants()
                                        .OfType<CheckBox>()
                                        .FirstOrDefault(x => x.Classes.Contains("taskCheckbox"));

           if (childCheckBox != null)
            {
                childCheckBox.IsCheckedChanged += (sender, args) =>
                {
                    bool areAllCompleted = components.All(comp => comp.IsCompleted);

                    if (checkBox.IsChecked != areAllCompleted)
                        checkBox.IsChecked = areAllCompleted;

                    RefreshStatusLabel();
                };
            }

            mainSection.Children.Add(childView);
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

// Reprezentuje ogólną grupę przechowującą dowolne elementy IComponent
public class Group : IComponent
{
    public string Name { get; }
    private List<IComponent> components = new List<IComponent>();
    public DateTime StartDate => components.Count == 0 ? DateTime.MinValue : components.Min(c => c.StartDate);
    public List<string> Tags { get; }
    public string Category { get; }

    public Group(string name) => Name = name;

    public void Add(IComponent component) => components.Add(component);
    public int Count() => components.Count;
    public void Remove(IComponent component) => components.Remove(component);
    public bool Contains(IComponent component) => components.Contains(component);
    public IReadOnlyList<IComponent> GetComponents() => components.AsReadOnly();

    // Wyświetla nagłówek grupy i renderuje całą zawartość
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
    public StackPanel SimpleDisplay() => SimpleDisplay(1);
    public void Accept(IVisitor visitor) => visitor.Visit(this);

}