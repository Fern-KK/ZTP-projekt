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

public interface IBuilder
{
    string CurrentName { get; set; }
    string Category { get; set; }
    string Tags { get; set; }
    int Counter { get; set; }

    public void SetName(string s);
    public void SetCategory(string c);
    public void SetTags(string t);
    public void Build();
}

public class BuilderNote : IBuilder
{
    public int Counter { get; set; } = 1;
    public string CurrentName {get; set;} = "";
    public string Category { get; set; } = "";
    public string Tags { get; set; } = "";
    private string Content = "";
    public void SetName(string s)
    {
        CurrentName = s;
    }

    public void SetContent(string s)
    {
        Content = s;
    }
    public void SetCategory(string c)
    {
        Category=c;    
    }
    public void SetTags(string t)
    {
        Tags=t;
    }

    public string GetName()
    {
        return CurrentName;
    }
    public string DefaultName()
    {
        return $"Nowa notatka {Counter}";
    }

    public void Build()
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

    
    public void Clear()
    {
        CurrentName = "";
        Content = "";
        Category = "";
        Tags = "";
    }
}

public class BuilderTask : IBuilder
{
    private List<IComponent> Components = new List<IComponent>();
    public string CurrentName {get; set;} = "";
    public int Counter { get; set; } = 1;
    public string Category { get; set; } = "";
    public string Tags { get; set; } = "";
    private Priorities priority = 0;
    private DateTime endDate;
    public void SetName(string s)
    {
        CurrentName = s;
    }
    public void SetCategory(string c)
    {
        Category=c;    
    }
    public void SetTags(string t)
    {
        Tags=t;
    }

    public void SetPriority(Priorities p)
    {
        priority=p;
    }

    public string GetName()
    {
        if (string.IsNullOrEmpty(CurrentName) && Components.Count > 0)
            return Components.First().Name;
        return CurrentName;
    }
    public string DefaultName()
    {
        return $"Nowe zadanie {Counter}";
    }

    public void AddTaskComponent(ITaskComponent t)
    {
        Components.Add(t);
    }
    
    public void Build()
    {
        if (Components.Count == 0) return;

        // Pojedyńcze zadanie
        if (Components.Count == 1 && Components.First() is Task result)
        {
            result.SetCategory(Category);

            if (!string.IsNullOrWhiteSpace(Tags))
            {
                foreach (var t in Tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                    .Select(x => x.Trim().ToLower()))
                {
                    result.SetTags(t);
                }
            }

            result.SetPriority(priority);
            Clear();
            GlobalGroups.AllTasksGroup.Add(result);
            GlobalGroups.AllGroup.Add(result);
            return;
        }

        // Lista zadań
        string name = CurrentName;
        if (!string.IsNullOrEmpty(CurrentName))
        {
            name = DefaultName();
            Counter++;
        }
        var taskList = new TaskList(name);
        taskList.SetCategory(Category);

        // Ustawianie tagów
        if (!string.IsNullOrWhiteSpace(Tags))
        {
            foreach (var t in Tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(x => x.Trim().ToLower()))
            {
                taskList.SetTags(t);
            }
        }

        taskList.SetPriority(priority);

        // Dodawanie zadań do listy
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
        return;
    }

    
    public void Clear()
    {
        Components.Clear();
        CurrentName = "";
        priority = 0;
        Category = "";
        Tags = "";
    }
}