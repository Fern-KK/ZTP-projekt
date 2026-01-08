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

public static class GlobalGroups
{
    public static Group AllGroup = new Group("Wszystko");
    public static Group AllTasksGroup = new Group("Zadania");
    public static Group AllNotesGroup = new Group("Notatki");
    private static List<string> AllCategories { get; } = new List<string>();
    private static List<string> AllTags = new List<string>();

    public static void Initialize()
    {
        // Dodaj domyślne kategorie
        Categories.Add("szkoła");
        Categories.Add("dom");

        // Opcjonalnie: dodaj też tagi
        AddTags("pilne");
        AddTags("ważne");
        AddTags("codzienne");


        AddCategory("szkola");
        AddCategory("dom");
        AddCategory("dzieci");
        
    }
    // public static StackPanel adsa(string contains)
    // {
    //     var mainSection = new StackPanel{};
    //     foreach(var)
    //     return mainSection;
    // }
    public static void AddTags(List<string> tags)
    {
        foreach (var t in tags)
        {
            string tag = t?.Trim().ToLower() ?? "";
            if (!string.IsNullOrWhiteSpace(tag) && !AllTags.Contains(tag))
            {
                AllTags.Add(tag);
            }

        }
    }
    public static void AddTags(string tag)
    {
        if (tag.Contains(','))
        {
            List<string> tags = new List<string>(tag.Split(','));
            AddTags(tags);

        }
        else
        {
            string t = tag?.Trim().ToLower() ?? "";
            if (!string.IsNullOrWhiteSpace(t) && !AllTags.Contains(t))
            {
                AllTags.Add(t);
            }
        }
        
    }
    public static List<Button> GetTags()
    {
        var buttons = new List<Button>();
        foreach(var tag in AllTags)
        {
            var tagButton = new Button{Content = $"#{tag}", Name=tag};
            tagButton.Classes.Add("leftMenuButton");
            buttons.Add(tagButton);
        }
        return buttons;
    }


    public static void AddCategory(List<string> categories)
    {
        foreach (var c in categories)
        {
            string category = c?.Trim().ToLower() ?? "";
            if (!string.IsNullOrWhiteSpace(category) && !AllCategories.Contains(category))
            {
                AllCategories.Add(category);
            }

        }
    }
    public static void AddCategory(string category)
    {
            if(category==null){return;};
            List<string> categories = new List<string>(category.Split(','));
            AddCategory(categories);
        
    }
    public static List<Button> GetCategories()
    {
        var buttons = new List<Button>();
        foreach(var category in AllCategories)
        {
            var categoryButton = new Button{Content = $"{category}", Name=category};
            categoryButton.Classes.Add("leftMenuButton");
            buttons.Add(categoryButton);
        }
        return buttons;
    }
    public static ComboBox SelectableCategoryList()
    {
        return new ComboBox{ItemsSource = AllCategories };
    }
}









































public static class Categories
{
    private static List<Group> categories = new List<Group>();

    public static void Add(string name)
    {
        // Check if category already exists
        if (categories.Any(c => c.Name == name.ToLower()))
        {
            Console.WriteLine("Category already exists");
        }
        else
        {
            categories.Add(new Group(name.ToLower()));
        }
    }

    public static void Remove(string name)
    {
        var category = categories.FirstOrDefault(c => c.Name == name.ToLower());
        if (category != null)
        {
            categories.Remove(category);
            category = null;
        }
        else
        {
            Console.WriteLine("Category not found");
        }
    }

    public static void AddToCategory(IComponent component, Group selectedCategory)
    {
        // Najpierw usuwamy komponent ze wszystkich istniejących kategorii
        foreach (var c in categories)
        {
            if (c.Contains(component))
            {
                c.Remove(component);
                break; // Zakładamy że komponent jest tylko w jednej kategorii
            }
        }

        // Teraz dodajemy do nowej kategorii
        var targetCategory = categories.FirstOrDefault(c => c == selectedCategory);
        if (targetCategory != null)
        {
            targetCategory.Add(component);
        }
        else
        {
            return;
        }

    }
    public static void AddToCategory(IComponent component, string selectedCategory)
    {
        // Najpierw usuwamy komponent ze wszystkich istniejących kategorii
        foreach (var c in categories)
        {
            if (c.Contains(component))
            {
                c.Remove(component);
                break; // Zakładamy że komponent jest tylko w jednej kategorii
            }
        }

        // Teraz dodajemy do nowej kategorii
        var targetCategory = categories.FirstOrDefault(c => c.Name == selectedCategory.ToLower());
        if (targetCategory != null)
        {
            targetCategory.Add(component);
        }
        else
        {
            return;
        }

    }

    public static void RemoveFromCategory(IComponent component, string categoryName)
    {
        var category = categories.FirstOrDefault(c => c.Name == categoryName.ToLower());
        if (category != null)
        {
            category.Remove(component);
        }
        else
        {
            Console.WriteLine($"Category '{categoryName}' not found");
        }
    }

    public static StackPanel SimpleDisplay()
    {

        var mainSection = new StackPanel { };
        // Licznik elementów
        // Elementy grupy
        foreach (var c in categories)
        {
            mainSection.Children.Add(c.DisplayGUI());
        }
        return mainSection;
    }
    public static int Count()
    {
        return categories.Count();
    }

    public static List<Group> GetCategories()
    {
        return new List<Group>(categories);
    }
}


public static class Tags
{
    private static List<Group> tags = new List<Group>();

    public static void Add(string name)
    {
        if (tags.Any(c => c.Name == name.ToLower()))
        {
            Console.WriteLine("Category already exists");
        }
        else
        {
            tags.Add(new Group(name.ToLower()));
        }
    }

    public static void Remove(string name)
    {
        var tag = tags.FirstOrDefault(c => c.Name == name.ToLower());
        if (tag != null)
        {
            tags.Remove(tag);
            tag = null;
        }
        else
        {
            Console.WriteLine("Tag not found");
        }
    }

    public static void AddToCategory(IComponent component, string tagName)
    {


        // Teraz dodajemy do nowej kategorii
        var targetTag = tags.FirstOrDefault(c => c.Name == tagName.ToLower());
        if (targetTag != null)
        {
            targetTag.Add(component);
        }
        else
        {
            Console.WriteLine($"Tag '{tagName}' not found");
        }
    }

    public static void RemoveFromCategory(IComponent component, string tagName)
    {
        var category = tags.FirstOrDefault(c => c.Name == tagName.ToLower());
        if (category != null)
        {
            category.Remove(component);
        }
        else
        {
            Console.WriteLine($"Tag '{tagName}' not found");
        }
    }

    public static void Display()
    {
        Console.WriteLine("Tags: ");
        foreach (var t in tags)
        {
            Console.WriteLine("#" + t.Name);
        }
    }
    public static void DisplayCategory(int index)
    {
        if (index < 0 || index >= tags.Count)
        {
            Console.WriteLine("Nieprawidłowy numer tagu!");
            return;
        }
        var tag = tags[index];
        tag.Display();
    }
    public static int Count()
    {
        return tags.Count();
    }

    public static List<Group> GetTags()
    {
        return new List<Group>(tags);
    }



    public static ComboBox CreateComboBox(string watermark = null)
    {
        var comboBox = new ComboBox
        {
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
            Height = 40,
            FontSize = 14,
            ItemsSource = GetTagsForComboBox(),
            DisplayMemberBinding = new Avalonia.Data.Binding("Name"),
            SelectedIndex = 0
        };

        if (!string.IsNullOrEmpty(watermark))
        {

        }

        return comboBox;
    }

    // Metoda zwracająca CheckBox listę (do wielokrotnego wyboru)
    public static StackPanel CreateCheckBoxList(string header = null)
    {
        var panel = new StackPanel
        {
            Spacing = 5
        };

        if (!string.IsNullOrEmpty(header))
        {
            panel.Children.Add(new TextBlock
            {
                Text = header,
                FontWeight = FontWeight.SemiBold,
                Margin = new Thickness(0, 0, 0, 5)
            });
        }

        foreach (var tag in tags)
        {
            var checkBox = new CheckBox
            {
                Content = $"#{tag.Name}",
                Tag = tag,
                Margin = new Thickness(5, 2)
            };
            panel.Children.Add(checkBox);
        }

        return panel;
    }

    // Metoda do pobierania zaznaczonych tagów z CheckBox listy
    public static List<Group> GetSelectedTags(StackPanel checkBoxPanel)
    {
        var selectedTags = new List<Group>();

        foreach (var child in checkBoxPanel.Children)
        {
            if (child is CheckBox checkBox && checkBox.IsChecked == true && checkBox.Tag is Group tag)
            {
                selectedTags.Add(tag);
            }
        }

        return selectedTags;
    }

    // Metoda pomocnicza do ComboBox
    private static List<ComboBoxItem> GetTagsForComboBox()
    {
        var items = new List<ComboBoxItem>();

        foreach (var tag in tags)
        {
            items.Add(new ComboBoxItem
            {
                Content = $"#{tag.Name}",
                Tag = tag
            });
        }

        return items;
    }

    // Metoda do aktualizacji ComboBox
    public static void UpdateComboBox(ComboBox comboBox)
    {
        comboBox.ItemsSource = GetTagsForComboBox();
    }
}

