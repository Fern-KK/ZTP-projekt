using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using HarfBuzzSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ZTP;

public static class TaskReportService
{
    public static StackPanel GetUpcomingTasksReport(int daysAhead = 7)
    {
        var startDate = DateTime.Today;
        var endDate = DateTime.Today.AddDays(daysAhead);

        var visitor = new UpcomingDeadlinesVisitor(startDate, endDate);
        DataManager.AllGroup.Accept(visitor);

        return visitor.GetUpcomingTasksPanel();
    }

    public static StackPanel GetOverdueTasksReport()
    {
        var startDate = DateTime.MinValue;
        var endDate = DateTime.Today;

        var visitor = new UpcomingDeadlinesVisitor(startDate, endDate);
        DataManager.AllGroup.Accept(visitor);

        var overdueTasks = visitor.GetUpcomingTasks();
        return CreateOverdueTasksPanel(overdueTasks);
    }

    private static StackPanel CreateOverdueTasksPanel(List<ITaskComponent> overdueTasks)
    {
        var panel = new StackPanel { Spacing = 10, Margin = new Thickness(20) };

        panel.Children.Add(new TextBlock
        {
            Text = $"⚠️ Zadania po terminie",
            FontSize = 18,
            FontWeight = FontWeight.Bold,
            Margin = new Thickness(0, 0, 0, 10)
        });

        if (overdueTasks.Count == 0)
        {
            panel.Children.Add(new TextBlock
            {
                Text = "Brak zadań po terminie.",
                FontStyle = FontStyle.Italic,
                Foreground = Brushes.Gray
            });
            return panel;
        }

        foreach (var task in overdueTasks.OrderBy(t => t.EndDate))
        {
            panel.Children.Add(CreateTaskRow(task));
        }

        return panel;
    }

    private static StackPanel CreateTaskRow(ITaskComponent task)
    {
        var row = new StackPanel
        {
            Orientation = Avalonia.Layout.Orientation.Horizontal,
            Spacing = 10,
            Margin = new Thickness(0, 5, 0, 5)
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
            row.Children.Add(new TextBlock
            {
                Text = $"{task.EndDate:dd.MM.yyyy}",
                FontSize = 11,
                Foreground = Brushes.Red,
                Width = 150
            });
        }

        return row;
    }
}