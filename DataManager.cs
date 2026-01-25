using Avalonia.Controls;
using System.Collections.Generic;
using System.Linq;

namespace ZTP;

public static class DataManager
{
    public static Group AllGroup = new Group("Wszystko");
    public static Group AllTasksGroup = new Group("Zadania");
    public static Group AllNotesGroup = new Group("Notatki");

    private static List<string> AllCategories { get; set; } = new List<string>();
    private static List<string> AllTags { get; set; } = new List<string>();

    public static void Initialize()
    {

    }

    // Tagi
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
        foreach (var tag in AllTags)
        {
            var tagButton = new Button { Content = $"#{tag}", Name = tag };
            tagButton.Classes.Add("leftMenuButton");
            buttons.Add(tagButton);
        }
        return buttons;
    }

    public static List<string> GetTagList() => AllTags;

    // Kategorie
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
        if (category == null) { return; }

        List<string> categories = new List<string>(category.Split(','));
        AddCategory(categories);
    }

    public static List<Button> GetCategories()
    {
        var buttons = new List<Button>();
        foreach (var category in AllCategories)
        {
            var categoryButton = new Button { Content = $"{category}", Name = category };
            categoryButton.Classes.Add("leftMenuButton");
            buttons.Add(categoryButton);
        }
        return buttons;
    }

    public static List<string> GetCategoryList() => AllCategories;

    public static ComboBox SelectableCategoryList()
    {
        var comboBox = new ComboBox();
        comboBox.ItemsSource = AllCategories;
        return comboBox;
    }

    // Pobierz wszystkie komponenty
    public static List<IComponent> GetAllComponents()
    {
        var components = new List<IComponent>();

        foreach (var component in AllGroup.GetComponents())
        {
            components.Add(component);

            if (component is Group group)
            {
                components.AddRange(group.GetComponents());
            }
        }

        return components;
    }
}