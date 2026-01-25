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

