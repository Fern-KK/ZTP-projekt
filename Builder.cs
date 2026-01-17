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

    public string GetName();
    public string DefaultName();
}

public class BuilderNote : IBuilder
{
    public int Counter { get; set; } = 1;
    public string CurrentName {get; set; } = "";
    public string Category { get; set; } = "";
    public string Tags { get; set; } = "";
    private string Content = "";
    
    public BuilderNote SetName(string s)
    {
        CurrentName = s;
        return this;
    }

    public BuilderNote SetContent(string s)
    {
        Content = s;
        return this;
    }
    public BuilderNote SetCategory(string c)
    {
        Category=c;
        return this;
    }
    public BuilderNote SetTags(string t)
    {
        Tags=t;
        return this;
    }

    public string GetName()
    {
        if (string.IsNullOrEmpty(CurrentName) || CurrentName == DefaultName())
        {
            string NewName = DefaultName();
            Counter++;
            return NewName;
        }
        return CurrentName;
    }
    public string DefaultName()
    {
        return $"Nowa notatka {Counter}";
    }

    public BuilderNote Build()
    {
        Note note = new Note(GetName(), Content);
        GlobalGroups.AllGroup.Add(note);
        GlobalGroups.AllNotesGroup.Add(note);
        ServerConnection client = ServerConnection.CreateServerConnection();
        
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
        client.NewNote(note);
        Clear();

        return this;
    }

    
    public BuilderNote Clear()
    {
        CurrentName = "";
        Content = "";
        Category = "";
        Tags = "";

        return this;
    }
}

public class BuilderTask : IBuilder
{
    private List<IComponent> Components = new List<IComponent>();
    public string CurrentName {get; set; } = "";
    public int Counter { get; set; } = 1;
    public string Category { get; set; } = "";
    public string Tags { get; set; } = "";
    private Priorities priority = 0;
    private DateTime endDate;
    public BuilderTask SetName(string s)
    {
        CurrentName = s;
        return this;
    }
    public BuilderTask SetCategory(string c)
    {
        Category=c;
        return this;
    }
    public BuilderTask SetTags(string t)
    {
        Tags=t;
        return this;
    }

    public BuilderTask SetPriority(Priorities p)
    {
        priority=p;
        return this;
    }

    public string GetName()
    {
        if (string.IsNullOrEmpty(CurrentName) || CurrentName == DefaultName())
        {
            string NewName = DefaultName();
            Counter++;
            return NewName;
        }
        return CurrentName;
    }

    public string DefaultName()
    {
        return $"Nowa lista zadań {Counter}";
    }

    public BuilderTask AddTaskComponent(ITaskComponent task)
    {
        Components.Add(task);
        return this;
    }
    
    public BuilderTask AddTaskComponent(List<TextBox> tasklist, List<DatePicker> datelist)
    {
        // Dodaj Taski do Buildera
        for (int i = 0; i < tasklist.Count; i++)
        {
            string text = tasklist[i].Text?.Trim() ?? "";
            if (!string.IsNullOrEmpty(text))
            {
                if (datelist[i].SelectedDate.HasValue)
                    Components.Add(new Task(text, datelist[i].SelectedDate.Value.Date));
                else
                    Components.Add(new Task(text));
            }
        }
        return this;
    }
    
    public BuilderTask Build()
    {
        if (Components.Count == 0) return this;

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
            ServerConnection client = ServerConnection.CreateServerConnection();
            client.NewTask(result);
            return this;
        }

        // Lista zadań
        // Ustaw nazwę
        var taskList = new TaskList(GetName());
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
        return this;
    }

    
    public BuilderTask Clear()
    {
        Components.Clear();
        CurrentName = "";
        priority = 0;
        Category = "";
        Tags = "";

        return this;
    }
}