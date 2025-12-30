using Avalonia;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;


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
    public void Display(int depth);
    public void Display();
}


public class Note : IComponent
{
    public string Name { get; }
    public string Content { get; }
    public DateTime StartDate { get; }

    // Konstruktor klasy Task, ustawiający nazwę oraz daty początku i końca zadania
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


    // Używana do wyświetlenia szczegółów zadania wraz ze statusem
    public void Display()
    {
        this.Display(1);
    }
    public void Display(int depth)
    {
        Console.WriteLine(new String('-', depth) + $"{Name} ({StartDate:dd.MM.yyyy}) \n{new String(' ', depth)}Treść: {Content}");
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

    // Konstruktor klasy Task, ustawiający nazwę oraz daty początku i końca zadania
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

    // Metoda oznaczająca zadanie jako wykonane; przyjmuje datę wykonania i sprawdza, czy zadanie wykonano na czas
    public void MarkAsCompleted(DateTime completionDate)
    {
        IsCompleted = true;
        IsLate = completionDate > EndDate;
    }

    // Zwraca status zadania: "Completed", "Completed Late" lub "Pending"
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

    // Używana do wyświetlenia szczegółów zadania wraz ze statusem
    public void Display()
    {
        this.Display(1);
    }
    public void Display(int depth)
    {
        Console.WriteLine(new String('-', depth) + $"{Name} ({StartDate:dd.MM.yyyy} to {EndDate:dd.MM.yyyy}) - Status: {GetStatus()}");
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
    public void Display()
    {
        this.Display(1);
    }
    public void Display(int depth)
    {
        Console.WriteLine(new String('-', depth) + $"{Name} ({StartDate:dd.MM.yyyy} to {EndDate:dd.MM.yyyy}) - Status: {GetStatus()}");
        foreach (var component in components)
        {
            component.Display(depth + 2);
        }
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
    public void Report()
    {
        int[] stat = this.getStatistics();
        Console.WriteLine("\nPodsumowanie zadań:");
        Console.WriteLine($"Zadania wykonane na czas: {stat[0]}");
        Console.WriteLine($"Zadania wykonane z opóźnieniem: {stat[1]}");
        Console.WriteLine($"Zadania oczekujące: {stat[2]}");
        Console.WriteLine($"Zadania oczekujące z przekroczonym terminem: {stat[3]}");
    }

}





public class Group
{
    public string Name { get; }
    private List<IComponent> components = new List<IComponent>();
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
    public void Display()
    {
        this.Display(1);
    }
    public void Display(int depth)
    {
        foreach (var component in components)
        {
            component.Display(depth + 2);
        }
    }
}