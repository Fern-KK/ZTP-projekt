using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using ZTP.Composite;

namespace ZTP;

// Interfejs obiektu odwiedzanego 
public interface IVisitedComponent
{
    void Accept(IVisitor visitor);
}

// Interfejs Visitor
public interface IVisitor
{
    void Visit(Note note);
    void Visit(Task task);
    void Visit(TaskList taskList);
    void Visit(Group group);
}

// Visitor dla wyszukiwania
public class SearchVisitor : IVisitor
{
    private string SearchQuery;
    private List<Composite.IComponent> SearchResults = new List<Composite.IComponent>();

    public SearchVisitor(string query)
    {
        SearchQuery = query.ToLower();
    }

    public List<Composite.IComponent> GetResults() => SearchResults;

    public void Visit(Note note)
    {
        if (MatchesSearch(note))
        {
            SearchResults.Add(note);
        }
    }

    public void Visit(Task task)
    {
        if (MatchesSearch(task))
        {
            SearchResults.Add(task);
        }
    }

    public void Visit(TaskList taskList)
    {
        if (MatchesSearch(taskList))
            SearchResults.Add(taskList);

        // Przeszukaj również podzadania
        var components = taskList.GetType().GetProperty("components")?.GetValue(taskList) as List<ITaskComponent>;
        if (components != null)
        {
            foreach (var component in components)
            {
                switch(component)
                {
                    case Task t:
                        if (MatchesSearch(t))
                            SearchResults.Add(taskList);
                        break;

                    case TaskList tl:
                        if (MatchesSearch(tl))
                            SearchResults.Add(taskList);
                        break;
                }
            }
        }
    }

    public void Visit(Group group)
    {
        if (MatchesSearch(group))
        {
            SearchResults.Add(group);
        }
            

        // Przeszukaj elementy grupy
        var components = group.GetComponents();
        foreach (var component in components)
        {
            if (component is Note n)
                Visit(n);
            else if (component is Task t)
                Visit(t);
            else if (component is TaskList tl)
                Visit(tl);
            else if (component is Group g)
                Visit(g);
        }
    }

    private bool MatchesSearch(Composite.IComponent component)
    {
        if (string.IsNullOrWhiteSpace(SearchQuery))
            return false;

        // Sprawdź nazwę
        if (component.Name.ToLower().Contains(SearchQuery))
            return true;

        // Sprawdź kategorię
        if (component.Category?.ToLower().Contains(SearchQuery) == true)
            return true;

        // Sprawdź pola specjalne dla poszczególnych kategorii
        switch (component)
        {
            case Note note:
                // Sprawdź opis
                if (note.Content?.ToLower().Contains(SearchQuery) == true)
                    return true;
                
                // Sprawdź tagi
                if (note.Tags.Any(tag => tag.ToLower().Contains(SearchQuery)))
                    return true;
                break;
            
            case ITaskComponent task:
                // Sprawdź tagi
                if (task.Tags.Any(tag => tag.ToLower().Contains(SearchQuery)))
                    return true;
                break;
        }

        return false;
    }
}


// Visitor dla statystyk
public class StatisticsVisitor : IVisitor
{
    public int TotalNotes { get; private set; }
    public int TotalTasks { get; private set; }
    public int TotalTaskLists { get; private set; }
    public int CompletedTasks { get; private set; }
    public int LateTasks { get; private set; }
    public int PendingTasks { get; private set; }
    public Dictionary<string, int> CategoryStats { get; } = new Dictionary<string, int>();
    public Dictionary<string, int> TagStats { get; } = new Dictionary<string, int>();

    public void Visit(Note note)
    {
        TotalNotes++;
        if (!string.IsNullOrEmpty(note.Category))
        {
            CategoryStats[note.Category] = CategoryStats.GetValueOrDefault(note.Category) + 1;
        }

        foreach (var tag in note.Tags)
        {
            TagStats[tag] = TagStats.GetValueOrDefault(tag) + 1;
        }
    }

    public void Visit(Task task)
    {
        TotalTasks++;

        if (task.IsCompleted)
        {
            CompletedTasks++;
            if (task.IsLate) LateTasks++;
        }
        else
        {
            PendingTasks++;
        }

        if (!string.IsNullOrEmpty(task.Category))
        {
            CategoryStats[task.Category] = CategoryStats.GetValueOrDefault(task.Category) + 1;
        }

        foreach (var tag in task.Tags)
        {
            TagStats[tag] = TagStats.GetValueOrDefault(tag) + 1;
        }
    }

    public void Visit(TaskList taskList)
    {
        TotalTaskLists++;

        if (!string.IsNullOrEmpty(taskList.Category))
        {
            CategoryStats[taskList.Category] = CategoryStats.GetValueOrDefault(taskList.Category) + 1;
        }

        foreach (var tag in taskList.Tags)
        {
            TagStats[tag] = TagStats.GetValueOrDefault(tag) + 1;
        }

        // Sprawdza podzadania
        var components = taskList.GetType().GetProperty("components")?.GetValue(taskList) as List<ITaskComponent>;
        if (components != null)
        {
            foreach (var component in components)
            {
                if (component is Task t)
                    Visit(t);
                else if (component is TaskList tl)
                    Visit(tl);
            }
        }
    }

    public void Visit(Group group)
    {
        var components = group.GetComponents();
        foreach (var component in components)
        {
            if (component is Note n)
                Visit(n);
            else if (component is Task t)
                Visit(t);
            else if (component is TaskList tl)
                Visit(tl);
            else if (component is Group g)
                Visit(g);
        }
    }

    public StackPanel GetStatisticsPanel()
    {
        var mainSection = new StackPanel { Spacing = 10, Margin = new Thickness(0,0,0,20) };

        mainSection.Children.Add(new TextBlock{Text = "Statystyki ogólne", FontSize = 18, FontWeight = FontWeight.Bold});

        mainSection.Children.Add(CreateStatRow("Notatki:", TotalNotes.ToString()));
        mainSection.Children.Add(CreateStatRow("Zadania pojedyńcze:", TotalTasks.ToString()));
        mainSection.Children.Add(CreateStatRow("Listy zadań:", TotalTaskLists.ToString()));
        mainSection.Children.Add(CreateStatRow("Zadania wykonane:", $"{CompletedTasks} (w tym {LateTasks} spóźnionych)"));
        mainSection.Children.Add(CreateStatRow("Zadania oczekujące:", PendingTasks.ToString()));

        if (CategoryStats.Count > 0)
        {
            mainSection.Children.Add(new TextBlock{ Text = "Kategorie:", 
                                                    FontSize = 14, 
                                                    FontWeight = FontWeight.SemiBold, 
                                                    Margin = new Thickness(0, 10, 0, 10) });

            foreach (var category in CategoryStats.OrderByDescending(x => x.Value))
            {
                mainSection.Children.Add(CreateStatRow($"  {category.Key}:", category.Value.ToString()));
            }
        }

        if (TagStats.Count > 0)
        {
            mainSection.Children.Add(new TextBlock { Text = "Tagi:", 
                                                     FontSize = 14, 
                                                     FontWeight = FontWeight.SemiBold, 
                                                     Margin = new Thickness(0, 10, 0, 10) });

            foreach (var tag in TagStats.OrderByDescending(x => x.Value))
            {
                mainSection.Children.Add(CreateStatRow($"  #{tag.Key}:", tag.Value.ToString()));
            }
        }

        return mainSection;
    }

    private StackPanel CreateStatRow(string label, string value)
    {
        var row = new StackPanel { Orientation = Avalonia.Layout.Orientation.Horizontal };

        row.Children.Add(new TextBlock {Text = label, Width = 250});
        row.Children.Add(new TextBlock{ Text = value, FontWeight = FontWeight.Bold });
        return row;
    }
}


// Visitor dla raportu zbliżających się terminów
public class UpcomingDeadlinesVisitor : IVisitor
{
    private DateTime StartDate;
    private DateTime EndDate;
    private List<ITaskComponent> UpcomingTasks = new List<ITaskComponent>();

    public UpcomingDeadlinesVisitor(DateTime startDate, DateTime endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
    }

    public List<ITaskComponent> GetUpcomingTasks() => UpcomingTasks;

    public void Visit(Note note) { }

    public void Visit(Task task)
    {
        if (task.EndDate.HasValue &&
            task.EndDate.Value.Date >= StartDate.Date &&
            task.EndDate.Value.Date <= EndDate.Date &&
            !task.IsCompleted)
        {
            UpcomingTasks.Add(task);
        }
    }

    public void Visit(TaskList taskList)
    {
        if (taskList.EndDate.HasValue &&
            taskList.EndDate.Value.Date >= StartDate.Date &&
            taskList.EndDate.Value.Date <= EndDate.Date &&
            !taskList.IsCompleted)
        {
            UpcomingTasks.Add(taskList);
        }

        // Sprawdź podzadania
        var components = taskList.GetType().GetProperty("components")?.GetValue(taskList) as List<ITaskComponent>;
        if (components != null)
        {
            foreach (var component in components)
            {
                if (component is Task t)
                    Visit(t);
                else if (component is TaskList tl)
                    Visit(tl);
            }
        }
    }

    public void Visit(Group group)
    {
        var components = group.GetComponents();
        foreach (var component in components)
        {
            if (component is Task t)
                Visit(t);
            else if (component is TaskList tl)
                Visit(tl);
            else if (component is Group g)
                Visit(g);
        }
    }

    public StackPanel GetUpcomingTasksPanel()
    {
        var panel = new StackPanel { Spacing = 10, Margin = new Thickness(20) };

        panel.Children.Add(new TextBlock
        {
            Text = $"📅 Zadania na okres {StartDate:dd.MM.yyyy} - {EndDate:dd.MM.yyyy}",
            FontSize = 18,
            FontWeight = FontWeight.Bold,
            Margin = new Thickness(0, 0, 0, 10)
        });

        if (UpcomingTasks.Count == 0)
        {
            panel.Children.Add(new TextBlock
            {
                Text = "Brak zadań w wybranym okresie.",
                FontStyle = FontStyle.Italic,
                Foreground = Brushes.Gray
            });
            return panel;
        }

        var sortedTasks = UpcomingTasks
            .OrderBy(t => t.EndDate)
            .ThenBy(t => t is TaskList ? 0 : 1)
            .ThenBy(t => t.Name);

        foreach (var task in sortedTasks)
        {
            panel.Children.Add(CreateTaskRow(task));
        }

        return panel;
    }

    private StackPanel CreateTaskRow(ITaskComponent task)
    {
        var row = new StackPanel
        {
            Orientation = Avalonia.Layout.Orientation.Horizontal,
            Spacing = 10,
            Margin = new Thickness(0, 5, 0, 5),
            Background = task is TaskList ? Brushes.AliceBlue : Brushes.Transparent,
        };

        string type = task is TaskList ? "Lista" : "Zadanie";

        row.Children.Add(new TextBlock
        {
            Text = task.Name,
            FontWeight = FontWeight.SemiBold,
            Width = 200,
            TextWrapping = TextWrapping.Wrap
        });

        row.Children.Add(new TextBlock
        {
            Text = type,
            FontSize = 11,
            Foreground = Brushes.Gray,
            Width = 50
        });

        if (task.EndDate.HasValue)
        {
            var daysLeft = (task.EndDate.Value.Date - DateTime.Today).Days;
            string daysText = daysLeft switch
            {
                0 => "Dziś",
                1 => "Jutro",
                < 0 => $"{Math.Abs(daysLeft)} dni temu",
                _ => $"za {daysLeft} dni"
            };

            row.Children.Add(new TextBlock
            {
                Text = $"{task.EndDate:dd.MM.yyyy} ({daysText})",
                FontSize = 11,
                Foreground = daysLeft <= 0 ? Brushes.Red : Brushes.Green,
                Width = 150
            });
        }

        return row;
    }
}