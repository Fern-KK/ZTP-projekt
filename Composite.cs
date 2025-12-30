using Avalonia;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ZTP;

public enum Priorities
{
    None,
    Low,
    Normal,
    Important
}

public interface IComponent
{
    public string Name { get; }
    DateTime StartDate { get; }
    public string Display(int depth);
    public string Display();
}

public class Note : IComponent
{
    public string Name { get; }
    public string Content { get; }
    public DateTime StartDate { get; }

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

    public string Display()
    {
        return this.Display(1);
    }
    
    public string Display(int depth)
    {
        string indent = new string(' ', depth);
        string dashPrefix = new string('-', depth);
        
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"{dashPrefix}{Name} ({StartDate:dd.MM.yyyy})");
        sb.AppendLine($"{indent}Treść: {Content}");
        
        return sb.ToString();
    }
}

public interface ITaskComponent : IComponent
{
    DateTime EndDate { get; }
    bool IsCompleted { get; }
    bool IsLate { get; }
    Priorities Priority { get; }
    public void MarkAsCompleted(DateTime completionDate);
    public string GetStatus();
    void SetPriority(Priorities priority);
}

public class Task : ITaskComponent
{
    public string Name { get; }
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }
    public Priorities Priority { get; private set; } = 0;
    public bool IsCompleted { get; private set; } = false;
    public bool IsLate { get; private set; } = false;

    public Task(string name, DateTime endDate)
    {
        Name = name;
        StartDate = DateTime.Now;
        EndDate = endDate;
    }
    
    public Task(Task other)
    {
        Name = other.Name;
        StartDate = other.StartDate;
        EndDate = other.EndDate;
        Priority = other.Priority;
        IsCompleted = other.IsCompleted;
        IsLate = other.IsLate;
    }

    public void MarkAsCompleted(DateTime completionDate)
    {
        IsCompleted = true;
        IsLate = completionDate > EndDate;
    }

    public string GetStatus()
    {
        if (IsCompleted)
            return IsLate ? "[Completed Late]" : "[Completed]";
        return "[Pending]";
    }

    public void SetPriority(Priorities priority)
    {
        Priority = priority;
    }

    public string Display()
    {
        return this.Display(1);
    }
    
    public string Display(int depth)
    {
        string dashPrefix = new string('-', depth);
        return $"{dashPrefix}{Name} ({StartDate:dd.MM.yyyy} to {EndDate:dd.MM.yyyy}) - Status: {GetStatus()}\n";
    }
}

public class TaskList : ITaskComponent
{
    public string Name { get; }
    private List<ITaskComponent> components = new List<ITaskComponent>();

    public DateTime StartDate
    {
        get
        {
            if (components.Count == 0)
                return DateTime.MinValue;
            return components.Min(component => component.StartDate);
        }
    }

    public DateTime EndDate
    {
        get
        {
            if (components.Count == 0)
                return DateTime.MaxValue;
            return components.Max(component => component.EndDate);
        }
    }

    public bool IsCompleted
    {
        get
        {
            return components.Count > 0 && components.All(component => component.IsCompleted);
        }
    }

    public bool IsLate
    {
        get
        {
            return components.Count > 0 && components.Any(component => component.IsLate);
        }
    }
    
    public Priorities Priority { get; private set; } = 0;

    public TaskList(string name)
    {
        Name = name;
    }

    public TaskList(string name, List<ITaskComponent> list)
    {
        Name = name;
        components = list;
    }

    public TaskList(TaskList other)
    {
        Name = other.Name;
        components = other.components;
    }

    public void Add(ITaskComponent component)
    {
        components.Add(component);
    }

    public void Remove(ITaskComponent component)
    {
        components.Remove(component);
    }
    
    public void SetPriority(Priorities priority)
    {
        Priority = priority;
    }

    public void MarkAsCompleted(DateTime completionDate)
    {
        foreach (var component in components)
        {
            component.MarkAsCompleted(completionDate);
        }
    }

    public string GetStatus()
    {
        if (IsCompleted)
            return IsLate ? "[Completed Late]" : "[Completed]";
        return "[Pending]";
    }
    
    public string Display()
    {
        return this.Display(1);
    }
    
    public string Display(int depth)
    {
        StringBuilder sb = new StringBuilder();
        string dashPrefix = new string('-', depth);
        
        sb.AppendLine($"{dashPrefix}{Name} ({StartDate:dd.MM.yyyy} to {EndDate:dd.MM.yyyy}) - Status: {GetStatus()}");
        
        foreach (var component in components)
        {
            sb.Append(component.Display(depth + 2));
        }
        
        return sb.ToString();
    }
    
    private int[] getStatistics()
    {
        int[] statistics = new int[4] {
            components.OfType<Task>().Count(t => t.IsCompleted && !t.IsLate),
            components.OfType<Task>().Count(t => t.IsCompleted && t.IsLate),
            components.OfType<Task>().Count(t => !t.IsCompleted),
            components.OfType<Task>().Count(t => !t.IsCompleted && DateTime.Now > t.EndDate)
        };

        foreach (TaskList group in components.OfType<TaskList>())
        {
            int[] groupReport = group.getStatistics();
            for (int i = 0; i < statistics.Length; i++)
            {
                statistics[i] += groupReport[i];
            }
        }

        return statistics;
    }
    
    public string Report()
    {
        int[] stat = this.getStatistics();
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("\nPodsumowanie zadań:");
        sb.AppendLine($"Zadania wykonane na czas: {stat[0]}");
        sb.AppendLine($"Zadania wykonane z opóźnieniem: {stat[1]}");
        sb.AppendLine($"Zadania oczekujące: {stat[2]}");
        sb.AppendLine($"Zadania oczekujące z przekroczonym terminem: {stat[3]}");
        
        return sb.ToString();
    }
}

public class Group : IComponent
{
    public string Name { get; }
    private List<IComponent> components = new List<IComponent>();
        public DateTime StartDate
    {
        get
        {
            if (components.Count == 0)
                return DateTime.MinValue;
            return components.Min(component => component.StartDate);
        }
    }
    public Group(string name)
    {
        Name = name;
    }

    public void Add(IComponent component)
    {
        components.Add(component);
    }

    public void Remove(IComponent component)
    {
        components.Remove(component);
    }
    
    public bool Contains(IComponent component)
    {
        return components.Contains(component);
    }
    
    public int Count()
    {
        return components.Count();
    }
    
    public IReadOnlyList<IComponent> GetComponents()
    {
        return components.AsReadOnly();
    }
    
    public string Display()
    {
        return this.Display(1);
    }
    
    public string Display(int depth)
    {
        StringBuilder sb = new StringBuilder();
        
        foreach (var component in components)
        {
            sb.Append(component.Display(depth + 2));
        }
        
        return sb.ToString();
    }
    
    public string GetFormattedList()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"{Name}:");
        sb.AppendLine();
        
        int i = 1;
        foreach (var component in components)
        {
            sb.AppendLine($"{i}. {component.Name}");
            i++;
        }
        
        return sb.ToString();
    }
    
    public string GetDetailedList()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"{Name}:");
        sb.AppendLine();
        
        int i = 1;
        foreach (var component in components)
        {
            sb.AppendLine($"{i}. {component.Display()}");
            i++;
        }
        
        return sb.ToString();
    }
}