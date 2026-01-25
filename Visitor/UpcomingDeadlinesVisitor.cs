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
using ZTP.Visitor;
using ZTP.Manager;
using ZTP.Strategy;
using ZTP.Services;
using ZTP.Builder;

namespace ZTP.Visitor;


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