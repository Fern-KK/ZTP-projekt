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