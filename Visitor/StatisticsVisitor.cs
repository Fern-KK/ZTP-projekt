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
