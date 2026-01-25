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
using ZTP.Composite;

namespace ZTP;

public static class StatisticsService
{
    public static StackPanel GetStatistics()
    {
        var visitor = new StatisticsVisitor();
        DataManager.AllGroup.Accept(visitor);

        return visitor.GetStatisticsPanel();
    }

    public static StackPanel GetDetailedStatistics()
    {
        var visitor = new StatisticsVisitor();
        DataManager.AllGroup.Accept(visitor);

        return GetStatisticsPanelWithDetails(visitor);
    }

    private static StackPanel GetStatisticsPanelWithDetails(StatisticsVisitor visitor)
    {
        var mainSection = new StackPanel { Spacing = 10, Margin = new Thickness(0,0,0,20) };

        mainSection.Children.Add(new TextBlock{Text = "Statystyki ogólne", FontSize = 18, FontWeight = FontWeight.Bold});

        mainSection.Children.Add(CreateStatRow("Notatki:", visitor.TotalNotes.ToString()));
        mainSection.Children.Add(CreateStatRow("Zadania pojedyńcze:", visitor.TotalTasks.ToString()));
        mainSection.Children.Add(CreateStatRow("Listy zadań:", visitor.TotalTaskLists.ToString()));
        mainSection.Children.Add(CreateStatRow("Zadania wykonane:", $"{visitor.CompletedTasks} (w tym {visitor.LateTasks} spóźnionych)"));
        mainSection.Children.Add(CreateStatRow("Zadania oczekujące:", visitor.PendingTasks.ToString()));

        if (visitor.CategoryStats.Count > 0)
        {
            mainSection.Children.Add(new TextBlock{ 
                Text = "Kategorie:", 
                FontSize = 14, 
                FontWeight = FontWeight.SemiBold, 
                Margin = new Thickness(0, 10, 0, 10) 
            });

            foreach (var category in visitor.CategoryStats.OrderByDescending(x => x.Value))
            {
                mainSection.Children.Add(CreateStatRow($"  {category.Key}:", category.Value.ToString()));
            }
        }

        if (visitor.TagStats.Count > 0)
        {
            mainSection.Children.Add(new TextBlock { 
                Text = "Tagi:", 
                FontSize = 14, 
                FontWeight = FontWeight.SemiBold, 
                Margin = new Thickness(0, 10, 0, 10) 
            });

            foreach (var tag in visitor.TagStats.OrderByDescending(x => x.Value))
            {
                mainSection.Children.Add(CreateStatRow($"  #{tag.Key}:", tag.Value.ToString()));
            }
        }

        return mainSection;
    }

    private static StackPanel CreateStatRow(string label, string value)
    {
        var row = new StackPanel { Orientation = Avalonia.Layout.Orientation.Horizontal };
        row.Children.Add(new TextBlock { Text = label, Width = 250 });
        row.Children.Add(new TextBlock{ Text = value, FontWeight = FontWeight.Bold });
        return row;
    }
}