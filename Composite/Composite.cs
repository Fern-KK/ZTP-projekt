using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ZTP.Composite;
using ZTP.Visitor;
using ZTP.Manager;
using ZTP.Strategy;
using ZTP.Services;
using ZTP.Builder;


namespace ZTP.Composite{

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
    public void SetId(int id) => NoteId = id;
    public void SetCategory(string category)
    {
        Category = category; 
        DataManager.AddCategory(category);
    }
    public void SetTags(List<string> tags)
    {
        Tags = tags;
        DataManager.AddTags(tags);
    }
    public void SetTags(string tag)
    {
        Tags.Add(tag);
        DataManager.AddTags(tag);
    }

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
        var inputCategory = DataManager.SelectableCategoryList();
        inputCategory.SelectedItem = Category;
        var inputTags = new TextBox { Text = string.Join(",", Tags), MaxWidth = 200 };

        var leftSide = new StackPanel { Orientation = Avalonia.Layout.Orientation.Horizontal, Spacing = 5 };
        leftSide.Children.Add(inputCategory);
        leftSide.Children.Add(inputTags);

        // Obsługa zapisu i aktualizacji serwera
        var saveButton = new Button { Content = "Zapisz", HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right };
        saveButton.Click += (s, e) =>
        {
            if (string.IsNullOrWhiteSpace(inputTitle.Text)) { inputTitle.Classes.Add("mustFill"); return; }
            Name = inputTitle.Text;
            Content = inputContent.Text;
            Category = inputCategory.SelectedItem?.ToString().ToLower().Trim();

            // Logika aktualizacji tagów
            if (inputTags.Text != string.Join(",", Tags))
            {
                DataManager.AddTags(Tags);
                Tags.Clear();
                string[] tags = inputTags.Text.Split(',');
                foreach (var t in tags)
                {
                    if (!string.IsNullOrWhiteSpace(t)) SetTags(t.Trim().ToLower());
                }
            }

            ServerConnection.CreateServerConnection().UpdateNote(this, this.NoteId);
            
            UIManager.DisplayGroup(DataManager.AllGroup);      // Powrót do widoku głównego
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
    public string Name { get; private set; }
    public int TaskId { get; set; }
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
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

    public Task(string name, DateTime? endDate = null)
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
        Category = other.Category;
        Tags = new List<string>(other.Tags);
    }

    public void SetPriority(Priorities priority) => Priority = priority;
    public void SetId(int id) => TaskId = id;
    public void SetTags(List<string> tags)
    {
        Tags = tags;
        DataManager.AddTags(tags);
    }
    public void SetTags(string tag)
    {
        Tags.Add(tag);
        DataManager.AddTags(tag);
    }
    public void SetCategory(string category)
    {
        Category = category; 
        DataManager.AddCategory(category);
    }

    public void MarkAsCompleted(DateTime completionDate)
    {
        IsCompleted = true;
        IsLate = completionDate > EndDate;
    }

    public void MarkAsIncomplete() => IsCompleted = false;

    public string GetStatus() => IsCompleted ? (IsLate ? "[Spóźnione, zakończone]" : "[Zakończone]") : "[W toku]";

    public void SetEndDate(DateTime? endDate)
    {
        EndDate = endDate;
    }

    public void SetName(string name)
    {
        Name = name;
    }

    public StackPanel SimpleDisplay(int depth)
    {
        var mainSection = new StackPanel { Margin = new Thickness(10 * depth, 5, 5, 10) };

        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto)); // Checkbox
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star)); // Nazwa
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto)); // Data
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto)); // Priorytet

        var titleButton = new Button
        {
            VerticalAlignment = VerticalAlignment.Center,
            Classes = { IsCompleted ? "checkTrue" : "checkFalse" },
            Content = new TextBlock { Text = Name, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center, Classes = { IsCompleted ? "checkTrue" : "checkFalse" } }
        };
        Grid.SetColumn(titleButton, 1);
        titleButton.Classes.Add("leftMenuButton");
        titleButton.Click += (s, e) => MainWindow.Instance.EditDisplay(this);
        // Checkbox dla statusu
        var checkBox = new CheckBox { IsChecked = IsCompleted };
        checkBox.Classes.Add("taskCheckbox");
        checkBox.IsCheckedChanged += (s, e) =>
        {
            bool isDone = checkBox.IsChecked == true;
            if (isDone) MarkAsCompleted(DateTime.Now);
            else MarkAsIncomplete();

            // Aktualizacja wyglądu
            if (titleButton.Content is TextBlock textBlock)
            {
                textBlock.Classes.Set("checkTrue", isDone);
                textBlock.Classes.Set("checkFalse", !isDone);
            }
        };

        Grid.SetColumn(checkBox, 0);
        grid.Children.Add(checkBox);
        grid.Children.Add(titleButton);

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

    public StackPanel DisplayDetails()
    {
        var mainSection = new StackPanel { Spacing = 10 };


        var panel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 5
        };
        var inputTitle = new TextBox { Text = Name, AcceptsReturn = true, MinWidth = 400, HorizontalAlignment = HorizontalAlignment.Stretch, };

        var datePicker = new DatePicker();
        if (EndDate != null) { datePicker.SelectedDate = EndDate; }


        panel.Children.Add(inputTitle);
        panel.Children.Add(datePicker);

        var infoBox = new TextBlock { Text = $"Utworzono: {StartDate:dd.MM.yyyy HH:mm} {GetStatus()}", FontSize = 11, Foreground = Brushes.Gray };

        // Sekcja dolna - kategoria, tagi i przycisk zapisu
        var downSection = new Grid { ColumnDefinitions = ColumnDefinitions.Parse("Auto, *") };
        var inputCategory = DataManager.SelectableCategoryList();
        inputCategory.SelectedItem = Category;
        var inputTags = new TextBox { Text = string.Join(",", Tags), MaxWidth = 200 };
        var inputPriority = new ComboBox { ItemsSource = Enum.GetValues<Priorities>(), SelectedItem = Priority };


        var leftSide = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 5 };
        leftSide.Children.Add(inputCategory);
        leftSide.Children.Add(inputTags);
        leftSide.Children.Add(inputPriority);

        // Obsługa zapisu i aktualizacji serwera
        var saveButton = new Button { Content = "Zapisz", HorizontalAlignment = HorizontalAlignment.Right };
        saveButton.Click += (s, e) =>
        {
            Name = inputTitle.Text;
            Category = inputCategory.SelectedItem?.ToString();
            if (inputPriority.SelectedItem is Priorities priority) { Priority = priority; }

            // Logika aktualizacji tagów
            if (inputTags.Text != string.Join(",", Tags))
            {
                DataManager.AddTags(Tags);
                Tags.Clear();
                string[] tags = inputTags.Text.Split(',');
                foreach (var t in tags)
                {
                    if (!string.IsNullOrWhiteSpace(t)) SetTags(t.Trim().ToLower());
                }
            }
            if (datePicker.SelectedDate.HasValue) { EndDate = datePicker.SelectedDate.Value.Date; }

            ServerConnection.CreateServerConnection().UpdateTask(this, TaskId);
            UIManager.DisplayGroup(DataManager.AllGroup);      // Powrót do widoku głównego
        };

        var rightSide = new StackPanel { HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right };
        rightSide.Children.Add(saveButton);

        Grid.SetColumn(leftSide, 0);
        Grid.SetColumn(rightSide, 1);
        downSection.Children.Add(leftSide);
        downSection.Children.Add(rightSide);

        mainSection.Children.Add(panel);
        mainSection.Children.Add(infoBox);
        mainSection.Children.Add(downSection);

        return mainSection;
    }
    public void Accept(IVisitor visitor) => visitor.Visit(this);
}

// Lista zadań do wykonania
public class TaskList : ITaskComponent
{
    public string Name { get; }
    public List<ITaskComponent> components { get; set; } = new List<ITaskComponent>();
    public int TaskListId { get; set; }
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
        components = new List<ITaskComponent>(other.components);
        Tags = new List<string>(other.Tags);
        Category = other.Category;
        Priority = other.Priority;
    }

    public void SetId(int id) => TaskListId = id;
    public void Add(ITaskComponent component) => components.Add(component);
    public void MarkAsCompleted(DateTime completionDate) => components.ForEach(c => c.MarkAsCompleted(completionDate));
    public void MarkAsIncomplete() => components.ForEach(c => c.MarkAsIncomplete());
    public string GetStatus() => IsCompleted ? (IsLate ? "[Spóźnione, zakończone]" : "[Zakończone]") : "[W toku]";
    public void SetPriority(Priorities priority) => Priority = priority;
    public void SetCategory(string category)
    {
        Category = category; 
        DataManager.AddCategory(category);
    }
    public void SetTags(List<string> tags)
    {
        Tags = tags;
        DataManager.AddTags(tags);
    }
    public void SetTags(string tag)
    {
        Tags.Add(tag);
        DataManager.AddTags(tag);
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
    // Wyświetla listę zadań oraz wszystkie zadania wewnątrz niej
    public StackPanel SimpleDisplay(int depth)
    {
        var mainSection = new StackPanel { Margin = new Thickness(10 * depth, 5, 5, 10) };

        // Tytuł listy zadań
        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto)); // Checkbox
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star)); // Nazwa 
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto)); // Priorytet


        // Nazwa listy zadań 
        var titleButton = new Button
        {
            VerticalAlignment = VerticalAlignment.Center,
            Classes = { IsCompleted ? "checkTrue" : "checkFalse" },
            Content = new TextBlock { Text = Name, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center, Classes = { IsCompleted ? "checkTrue" : "checkFalse" } }
        };
        Grid.SetColumn(titleButton, 1);
        titleButton.Classes.Add("leftMenuButton");
        titleButton.Click += (s, e) => MainWindow.Instance.EditDisplay(this);



        // Priorytet
        var priorityIcon = new TextBlock
        {
            Text = GetPriorityIcon(),
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
            FontSize = 14,
        };
        Grid.SetColumn(priorityIcon, 2);


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
            if (titleButton.Content is TextBlock textBlock)
            {
                textBlock.Classes.Set("checkTrue", isDone);
                textBlock.Classes.Set("checkFalse", !isDone);
            }
            RefreshStatusLabel();
        };

        Grid.SetColumn(checkBox, 0);
        grid.Children.Add(checkBox);
        grid.Children.Add(titleButton);
        grid.Children.Add(priorityIcon);

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

    public StackPanel DisplayDetails()
    {
        var mainSection = new StackPanel { Spacing = 10 };


        var dateBox = new TextBlock { };
        void RefreshStatusLabel()
        {
            dateBox.Text = EndDate.HasValue && EndDate != DateTime.MaxValue
                ? $"Status: {GetStatus()} | Termin: {StartDate:dd.MM.yyyy} - {EndDate:dd.MM.yyyy}"
                : $"Status: {GetStatus()} | Termin: {StartDate:dd.MM.yyyy}";
        }

        RefreshStatusLabel();
        // Status
        var inputCompleted = new CheckBox
        {
            IsChecked = IsCompleted
        };

        inputCompleted.IsCheckedChanged += (s, e) =>
        {
            if (inputCompleted.IsChecked == true)
            {
                MarkAsCompleted(DateTime.Now);
            }
            else
            {
                MarkAsIncomplete();
            }
            RefreshStatusLabel();
        };

        // Sekcja nazwy z checkboxem - Grid zamiast StackPanel dla lepszego układu
        var nameSection = new Grid
        {
            ColumnDefinitions = ColumnDefinitions.Parse("Auto, *"),
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
        };

        Grid.SetColumn(inputCompleted, 0);

        var inputTitle = new TextBox
        {
            Text = Name,
            AcceptsReturn = true,
            FontSize = 14,
            FontWeight = FontWeight.Bold,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch
        };

        Grid.SetColumn(inputTitle, 1);

        nameSection.Children.Add(inputCompleted);
        nameSection.Children.Add(inputTitle);

        var tasksPanel = new StackPanel { Spacing = 5 };

        foreach (var task in components)
        {
            var taskPanel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 10 };

            var taskName = new TextBlock
            {
                Text = task.Name,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                Width = 300
            };

            var editButton = new Button
            {
                Content = "Edytuj",
                Tag = task,
                Width = 80
            };

            editButton.Click += (s, e) =>
            {
                if (editButton.Tag is ITaskComponent taskToEdit)
                {
                    if (taskToEdit is Task singleTask)
                    {
                        MainWindow.Instance.EditDisplay(singleTask);
                    }
                    else if (taskToEdit is TaskList taskList)
                    {
                        MainWindow.Instance.EditDisplay(taskList);
                    }
                }
            };

            var removeButton = new Button
            {
                Content = "Usuń",
                Tag = task,
                Width = 80,
                Classes = { "deleteButton" }
            };

            removeButton.Click += (s, e) =>
            {
                if (removeButton.Tag is ITaskComponent taskToRemove)
                {
                    components.Remove(taskToRemove);
                    // Odśwież panel
                    var refreshedPanel = DisplayDetails();
                    if (MainWindow.Instance.Desktop.Content is StackPanel currentPanel)
                    {
                        MainWindow.Instance.Desktop.Content = refreshedPanel;
                    }
                }
            };

            taskPanel.Children.Add(taskName);
            taskPanel.Children.Add(editButton);
            taskPanel.Children.Add(removeButton);
            tasksPanel.Children.Add(taskPanel);
        }

        // Sekcja dodawania nowego zadania
        var addTaskSection = new StackPanel { Spacing = 5, Margin = new Thickness(0, 10, 0, 0) };
        var newTaskName = new TextBox
        {
            Watermark = "Nazwa nowego zadania",
            Width = 200
        };

        var newTaskDate = new DatePicker { };

        var addButton = new Button
        {
            Content = "Dodaj zadanie",
            Width = 120
        };

        addButton.Click += (s, e) =>
        {
            if (!string.IsNullOrWhiteSpace(newTaskName.Text))
            {
                var newTask = newTaskDate.SelectedDate.HasValue
                    ? new Task(newTaskName.Text.Trim(), newTaskDate.SelectedDate.Value.Date)
                    : new Task(newTaskName.Text.Trim());

                Add(newTask);

                // Odśwież panel
                var refreshedPanel = DisplayDetails();
                if (MainWindow.Instance.Desktop.Content is StackPanel currentPanel)
                {
                    MainWindow.Instance.Desktop.Content = refreshedPanel;
                }

                // Wyczyść pola
                newTaskName.Text = "";
                newTaskDate.SelectedDate = null;
            }
        };

        var addTaskRow = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 10
        };
        addTaskRow.Children.Add(newTaskName);
        addTaskRow.Children.Add(newTaskDate);
        addTaskRow.Children.Add(addButton);
        addTaskSection.Children.Add(addTaskRow);

        // Sekcja dolna - kategoria, tagi i przycisk zapisu
        var downSection = new Grid { ColumnDefinitions = ColumnDefinitions.Parse("Auto, *") };
        var inputCategory = DataManager.SelectableCategoryList();
        if (!string.IsNullOrEmpty(Category))
        {
            inputCategory.SelectedItem = Category;
        }

        var inputTags = new TextBox
        {
            Text = string.Join(", ", Tags),
            MaxWidth = 200,
        };
        // Priorytet
        var inputPriority = new ComboBox { ItemsSource = Enum.GetValues<Priorities>() };
        inputPriority.SelectedItem = Priority;

        var leftSide = new StackPanel
        {
            Orientation = Avalonia.Layout.Orientation.Horizontal,
            Spacing = 5,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
        };

        leftSide.Children.Add(inputCategory);
        leftSide.Children.Add(inputTags);
        leftSide.Children.Add(inputPriority);

        // Obsługa zapisu
        var saveButton = new Button
        {
            Content = "Zapisz zmiany",
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
            Classes = { "saveButton" }
        };

        saveButton.Click += (s, e) =>
        {
            // Aktualizacja priorytetu
            if (inputPriority.SelectedItem is Priorities priority)
            {
                SetPriority(priority);
            }

            // Aktualizacja kategorii
            Category = inputCategory.SelectedItem?.ToString().ToLower().Trim();

            // Aktualizacja tagów
            if (inputTags.Text != string.Join(", ", Tags))
            {
                Tags.Clear();
                string[] tags = inputTags.Text.Split(',');
                foreach (var t in tags)
                {
                    var trimmedTag = t.Trim();
                    if (!string.IsNullOrWhiteSpace(trimmedTag))
                    {
                        SetTags(trimmedTag.ToLower());
                        DataManager.AddTags(trimmedTag);
                    }
                }
            }

            // Powrót do widoku głównego
            UIManager.DisplayGroup(DataManager.AllGroup);
        };

        var rightSide = new StackPanel
        {
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
        };
        rightSide.Children.Add(saveButton);

        Grid.SetColumn(leftSide, 0);
        Grid.SetColumn(rightSide, 1);
        downSection.Children.Add(leftSide);
        downSection.Children.Add(rightSide);


        mainSection.Children.Add(nameSection);
        mainSection.Children.Add(dateBox);

        mainSection.Children.Add(tasksPanel);

        mainSection.Children.Add(addTaskSection);

        mainSection.Children.Add(downSection);

        return mainSection;
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

        // Górna sekcja z tytułem i ComboBox
        var topSection = new Grid
        {
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0, 0, 0, 10)
        };

        // Dwie kolumny: lewa na tytuł, prawa na ComboBox
        topSection.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star)); // Tytuł (rozciąga się)
        topSection.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto)); // ComboBox (auto szerokość)

        // Tytuł po lewej stronie
        var titleText = new TextBlock
        {
            Text = Name,
            FontSize = 14,
            FontWeight = FontWeight.Bold,
            VerticalAlignment = VerticalAlignment.Center
        };
        Grid.SetColumn(titleText, 0);

        // ComboBox do sortowania po prawej stronie
        var sortComboBox = new ComboBox
        {
            ItemsSource = SortingService.AvailableStrategies,
            HorizontalAlignment = HorizontalAlignment.Right,
            SelectedItem = SortingService.SortingStrategy,
            DisplayMemberBinding = new Avalonia.Data.Binding("DisplayName"),
            VerticalAlignment = VerticalAlignment.Center,
            Width = 150,
            Margin = new Thickness(10, 0, 0, 0),
            PlaceholderText = SortingService.SortingStrategy.DisplayName
        };

        sortComboBox.SelectionChanged += (s, e) =>
        {
            if (sortComboBox.SelectedItem is ISortingStrategy selectedStrategy)
            {
                SortingService.SetSortingStrategy(selectedStrategy);
                UIManager.DisplayGroup(this);
            }
        };
        Grid.SetColumn(sortComboBox, 1);

        topSection.Children.Add(titleText);
        topSection.Children.Add(sortComboBox);
        mainSection.Children.Add(topSection);

        // Licznik elementów
        var counterText = new TextBlock
        {
            Text = $"({Count()} elementów)",
            FontSize = 12,
            Foreground = Brushes.Gray,
            Margin = new Thickness(0, 0, 0, 10)
        };
        mainSection.Children.Add(counterText);

        // Użycie strategii przed renderowaniem
        var sortedComponents = SortingService.SortingStrategy.Sort(components);






        // Elementy grupy
        foreach (var c in sortedComponents)
        {
            mainSection.Children.Add(c.SimpleDisplay(depth + 1));
        }

        return mainSection;
    }
    public StackPanel SimpleDisplay() => SimpleDisplay(1);
    public void Accept(IVisitor visitor) => visitor.Visit(this);

}
}