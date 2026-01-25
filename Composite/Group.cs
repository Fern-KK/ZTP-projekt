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
