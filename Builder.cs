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

public static class Builder
{
    private static List<IComponent> components = new List<IComponent>();
    private static string currentName = "";
    private static int counter = 1;
    private static string content = "";
    private static Priorities prioritie = 0;
    private static DateTime endDate;
    private static string Category;
    private static string Tags;
    public static void SetName(string s)
    {
        currentName = s;
    }

    public static void SetContent(string s)
    {
        content = s;
    }
    public static void SetCategory(string selectedCategory)
    {
        Category=selectedCategory;    
    }
    public static void SetTags(string selectedTags)
    {
        Tags=selectedTags;
    }

    public static string GetName()
    {
        if (string.IsNullOrEmpty(currentName) && components.Count > 0)
            return components.First().Name;
        return currentName;
    }
    public static string DefaultName()
    {
        return $"New note {counter}";
    }

    public static void AddTaskComponent(ITaskComponent component)
    {
        components.Add(component);
    }

    public static void BuildNote()
    {
        Note note = new Note(currentName, content);
        GlobalGroups.AllGroup.Add(note);
        GlobalGroups.AllNotesGroup.Add(note);
        if(currentName == $"New note {counter}")
        {
            counter++;
        }
        if (Category != null)
        {
            note.SetCategory(Category);
        }
        if(Tags != null)
        {
            GlobalGroups.AddTags(Tags);
            var tags = Tags.Split(',');
            foreach (var t in tags)
            {
                string tag = t?.Trim().ToLower() ?? "";
                if (!string.IsNullOrWhiteSpace(tag))
                {
                    note.SetTags(tag);
                }
            }
        }

        Clear();
    }
    
    public static IComponent BuildTask()
    {
        if (components.Count == 0)
            return null;

        if (components.Count == 1 && components.First() is Task result)
        {
            result.SetCategory(Category);
            result.SetTags(Tags);
            result.SetPriority(prioritie);
            Clear();
            GlobalGroups.AllNotesGroup.Add(result);
            GlobalGroups.AllGroup.Add(result);
            return result;
        }

        string name = string.IsNullOrEmpty(currentName) ? components.First().Name : currentName;
        var taskList = new TaskList(name);
        taskList.SetCategory(Category);
        taskList.SetTags(Tags);
        taskList.SetPriority(prioritie);

        foreach (var component in components)
        {
            switch (component)
            {
                case Task task:
                    taskList.Add(new Task(task));
                    break;
                
                case TaskList tl:
                    taskList.Add(new TaskList(tl));
                    break;
            }
        }
        
        Clear();
        GlobalGroups.AllTasksGroup.Add(taskList);
        GlobalGroups.AllGroup.Add(taskList);
        return taskList;
    }
    
    public static void Clear()
    {
        components.Clear();
        currentName = "";
        content = "";
        prioritie = 0;
        currentName = "";
        Category = "";
        Tags = "";
    }
}