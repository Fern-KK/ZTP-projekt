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

namespace ZTP;

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
    private string _searchQuery;
    private List<IComponent> _searchResults = new List<IComponent>();

    public SearchVisitor(string query)
    {
        _searchQuery = query.ToLower();
    }

    public List<IComponent> GetResults() => _searchResults;

    public void Visit(Note note)
    {
        if (MatchesSearch(note))
        {
            _searchResults.Add(note);
        }

    }

    public void Visit(Task task)
    {
        if (MatchesSearch(task))
        {
            _searchResults.Add(task);
        }
    }

    public void Visit(TaskList taskList)
    {
        if (MatchesSearch(taskList))
            _searchResults.Add(taskList);

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
                            _searchResults.Add(taskList);
                        break;

                    case TaskList tl:
                        if (MatchesSearch(tl))
                            _searchResults.Add(taskList);
                        break;
                }
            }
        }
    }

    public void Visit(Group group)
    {
        if (MatchesSearch(group))
            _searchResults.Add(group);

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

    private bool MatchesSearch(IComponent component)
    {
        if (string.IsNullOrWhiteSpace(_searchQuery))
            return false;

        // Sprawdź nazwę
        if (component.Name.ToLower().Contains(_searchQuery))
            return true;

        // Sprawdź kategorię
        if (component.Category?.ToLower().Contains(_searchQuery) == true)
            return true;

        // Sprawdź pola specjalne dla poszczególnych kategorii
        switch (component)
        {
            case Note note:
                // Sprawdź opis
                if (note.Content?.ToLower().Contains(_searchQuery) == true)
                    return true;

                // Sprawdź tagi
                if (note.Tags.Any(tag => tag.ToLower().Contains(_searchQuery)))
                    return true;
                break;
            
            case ITaskComponent task:
                // Sprawdź tagi
                if (task.Tags.Any(tag => tag.ToLower().Contains(_searchQuery)))
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

        // Przejdź przez podzadania
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
        var panel = new StackPanel { Spacing = 10, Margin = new Thickness(20) };

        panel.Children.Add(new TextBlock
        {
            Text = "📊 Statystyki ogólne",
            FontSize = 18,
            FontWeight = FontWeight.Bold,
            Margin = new Thickness(0, 0, 0, 10)
        });

        panel.Children.Add(CreateStatRow("📝 Notatki:", TotalNotes.ToString()));
        panel.Children.Add(CreateStatRow("✅ Zadania proste:", TotalTasks.ToString()));
        panel.Children.Add(CreateStatRow("📋 Listy zadań:", TotalTaskLists.ToString()));
        panel.Children.Add(CreateStatRow("✔️ Zadania wykonane:", $"{CompletedTasks} (w tym {LateTasks} spóźnionych)"));
        panel.Children.Add(CreateStatRow("⏳ Zadania oczekujące:", PendingTasks.ToString()));

        if (CategoryStats.Count > 0)
        {
            panel.Children.Add(new TextBlock
            {
                Text = "🏷️ Kategorie:",
                FontSize = 14,
                FontWeight = FontWeight.SemiBold,
                Margin = new Thickness(0, 20, 0, 5)
            });

            foreach (var kvp in CategoryStats.OrderByDescending(x => x.Value))
            {
                panel.Children.Add(CreateStatRow($"  {kvp.Key}:", kvp.Value.ToString()));
            }
        }

        if (TagStats.Count > 0)
        {
            panel.Children.Add(new TextBlock
            {
                Text = "🔖 Tagi:",
                FontSize = 14,
                FontWeight = FontWeight.SemiBold,
                Margin = new Thickness(0, 20, 0, 5)
            });

            foreach (var kvp in TagStats.OrderByDescending(x => x.Value))
            {
                panel.Children.Add(CreateStatRow($"  #{kvp.Key}:", kvp.Value.ToString()));
            }
        }

        return panel;
    }

    private StackPanel CreateStatRow(string label, string value)
    {
        var row = new StackPanel
        {
            Orientation = Avalonia.Layout.Orientation.Horizontal,
            Margin = new Thickness(0, 2, 0, 2)
        };

        row.Children.Add(new TextBlock
        {
            Text = label,
            Width = 250,
            FontSize = 12
        });

        row.Children.Add(new TextBlock
        {
            Text = value,
            FontSize = 12,
            FontWeight = FontWeight.Bold
        });

        return row;
    }
}





















// Visitor dla raportu zbliżających się terminów
public class UpcomingDeadlinesVisitor : IVisitor
{
    private DateTime _startDate;
    private DateTime _endDate;
    private List<ITaskComponent> _upcomingTasks = new List<ITaskComponent>();

    public UpcomingDeadlinesVisitor(DateTime startDate, DateTime endDate)
    {
        _startDate = startDate;
        _endDate = endDate;
    }

    public List<ITaskComponent> GetUpcomingTasks() => _upcomingTasks;

    public void Visit(Note note) { }

    public void Visit(Task task)
    {
        if (task.EndDate.HasValue &&
            task.EndDate.Value.Date >= _startDate.Date &&
            task.EndDate.Value.Date <= _endDate.Date &&
            !task.IsCompleted)
        {
            _upcomingTasks.Add(task);
        }
    }

    public void Visit(TaskList taskList)
    {
        if (taskList.EndDate.HasValue &&
            taskList.EndDate.Value.Date >= _startDate.Date &&
            taskList.EndDate.Value.Date <= _endDate.Date &&
            !taskList.IsCompleted)
        {
            _upcomingTasks.Add(taskList);
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
            Text = $"📅 Zadania na okres {_startDate:dd.MM.yyyy} - {_endDate:dd.MM.yyyy}",
            FontSize = 18,
            FontWeight = FontWeight.Bold,
            Margin = new Thickness(0, 0, 0, 10)
        });

        if (_upcomingTasks.Count == 0)
        {
            panel.Children.Add(new TextBlock
            {
                Text = "Brak zadań w wybranym okresie.",
                FontStyle = FontStyle.Italic,
                Foreground = Brushes.Gray
            });
            return panel;
        }

        var sortedTasks = _upcomingTasks
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

        string icon = task is TaskList ? "📋" : "✅";
        string type = task is TaskList ? "Lista" : "Zadanie";

        row.Children.Add(new TextBlock
        {
            Text = icon,
            FontSize = 14,
            Width = 30
        });

        row.Children.Add(new TextBlock
        {
            Text = task.Name,
            FontSize = 12,
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