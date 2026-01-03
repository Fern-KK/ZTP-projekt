using Avalonia;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ZTP;



public static class TaskBuilder
{
    private static List<IComponent> components = new List<IComponent>();
    private static string currentName = "";
    private static string content = "";
    private static Priorities prioritie = 0;
    // private static DateTime EndTime = null;
    public static string GetName()
    {
        if (string.IsNullOrEmpty(currentName) && components.Count > 0)
            return components.First().Name;
        return currentName;
    }
    public static void StartNew(string name = "")
    {
        Clear();
        currentName = name ?? "";
    }

    public static void AddTaskComponent(ITaskComponent component)
    {
        components.Add(component);
    }

    public static void AddNote(Note note)
    {
        components.Add(note);
    }

    public static IComponent BuildTask()
    {
        if (components.Count == 0)
            throw new InvalidOperationException("Cannot build task - no components added");

        if (components.Count == 1)
        {
            var result = components.First();
            Clear();
            return result;
        }

        string name = string.IsNullOrEmpty(currentName) ? "Task Group" : currentName;
        var taskList = new TaskList(name);

        foreach (var component in components)
        {
            if (component is Task task)
                taskList.Add(new Task(task));
            else if (component is TaskList tl)
                taskList.Add(new TaskList(tl));
        }

        Clear();
        return taskList;
    }

    public static Note BuildNote()
    {
        if (components.Count == 0)
            throw new InvalidOperationException("Cannot build note - no components added");

        if (components.Count > 1)
        {
            Console.WriteLine("Warning: Multiple notes in builder, using first one");
        }

        var result = components.First() as Note;
        Clear();
        return result ?? throw new InvalidOperationException("Failed to build note");
    }

    public static void Clear()
    {
        components.Clear();
        currentName = "";
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

    public static void AddToCategory(IComponent component, string categoryName)
    {
        // Najpierw usuwamy komponent ze wszystkich istniejących kategorii
        foreach (var category in categories)
        {
            if (category.Contains(component))
            {
                category.Remove(component);
                break; // Zakładamy że komponent jest tylko w jednej kategorii
            }
        }

        // Teraz dodajemy do nowej kategorii
        var targetCategory = categories.FirstOrDefault(c => c.Name == categoryName.ToLower());
        if (targetCategory != null)
        {
            targetCategory.Add(component);
        }
        else
        {
            Console.WriteLine($"Category '{categoryName}' not found");
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

    public static void Display()
    {
        Console.WriteLine("Categories: ");
        foreach (var category in categories)
        {
            Console.WriteLine("-" + category.Name);
        }
    }
    public static void DisplayCategory(int index)
    {
        if (index < 0 || index >= categories.Count)
        {
            Console.WriteLine("Nieprawidłowy numer kategorii!");
            return;
        }
        var category = categories[index];
        category.Display();
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
}

