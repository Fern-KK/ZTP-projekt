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


namespace ZTP.Composite;

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