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
    private static List<IComponent> Components = new List<IComponent>();
    private static string CurrentName = "";
    private static int Counter = 1;
    private static string Content = "";
    private static Priorities Priority = 0;
    private static DateTime EndDate;
    private static string Category;
    private static string Tags;

    public static string GetName()
    {
        if (string.IsNullOrEmpty(CurrentName) && Components.Count > 0)
            return Components.First().Name;
        return CurrentName;
    }
    public static string DefaultName()
    {
        return $"New note {Counter}";
    }

    public static void SetName(string s)
    {
        CurrentName = s;
    }

    public static void SetContent(string s)
    {
        Content = s;
    }

    public static void StartNew(string name = "")
    {
        Clear();
        CurrentName = name ?? "";
    }
    public static void SetPriority(Priorities priority)
    {
        Priority=priority;
    }

    public static void AddTaskComponent(ITaskComponent component)
    {
        Components.Add(component);
    }

    public static void SetCategory(string selectedCategory)
    {
        Category=selectedCategory;    
    }
    public static void SetTags(string selectedTags)
    {
        Tags=selectedTags;
    }
    public static void Clear()
    {
        Components.Clear();
        CurrentName = "";
        Content = "";
        Priority = 0;
        CurrentName = "";
        Category = "";
        Tags = "";
    }

    public static void BuildNote()
    {
        Note note = new Note(CurrentName, Content);
        GlobalGroups.AllGroup.Add(note);
        GlobalGroups.AllNotesGroup.Add(note);
        if(CurrentName == $"New note {Counter}")
        {
            Counter++;
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
        if (Components.Count == 0)
            return null;

        if (Components.Count == 1)
        {
            var result = Components.First();

            Clear();
            GlobalGroups.AllNotesGroup.Add(result);
            GlobalGroups.AllGroup.Add(result);
            return result;
        }

        string name = string.IsNullOrEmpty(CurrentName) ? Components.First().Name : CurrentName;
        var taskList = new TaskList(name);
        taskList.SetCategory(Category);
        taskList.SetTags(Tags);
        taskList.SetPriority(Priority);

        foreach (var component in Components)
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
}
