using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;


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
    public Note(string name, string content, DateTime startDate)
    {
        Name = name;
        Content = content;
        StartDate = startDate;
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
        Console.WriteLine(new String('-', depth) + $"{Name} ({StartDate:dd.MM.yyyy}, Treść: {Content})");
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
    public Task(string name, DateTime startDate, DateTime endDate)
    {
        Name = name;
        StartDate = startDate;
        EndDate = endDate;
    }
    public Task(Task other)
    {
        Name=other.Name;
        StartDate=other.StartDate;
        EndDate=other.EndDate;
        Priority=other.Priority;
        IsCompleted=other.IsCompleted;
        IsLate=other.IsLate;

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






/*
public interface ICommand
{
    void Execute();
    void Undo();
};

public class NameCommand : ICommand
{
    private Component component;

};
*/




public class Program
{
    public static class Builder
    {
        private static List<IComponent> components = new List<IComponent>();
        private static string _name = "No name"; // Domyślna nazwa

        private static Priorities prioritie;

        // Property tylko do odczytu - pobiera nazwę na podstawie zawartości
        public static string Name
        {
            get
            {
                if (components.Count == 0)
                    return "Empty";

                if (components.Count == 1)
                    return components[0].Name;

                // Jeśli mamy wiele zadań, używamy ustawionej nazwy lub domyślnej
                return _name;
            }
        }

        // Metoda do ustawienia nazwy komponentu
        public static void WithName(string name)
        {
            _name = name;
        }

        public static void AddComponent(IComponent component)
        {
            if (component is Note && component.Count == 0)
            {
                components.Add(component);
                Build();
            }
            else if (component is not Note)
            {
                component.Add(component);
            }
        }

        public static void EditComponent(IComponent component)
        {
            if (component is Note note && component.Count == 0)
            {
                Note other = new Note(note);
                components.Add(other);
            }
            else if (component is Task task)
            {
                Task other = new Task(task);
                components.Add(other);
            }
            else if (component is TaskList tasklist)
            {
                TaskList other = new TaskList(tasklist);
                components.Add(other);
            }
        }

        public static IComponent Build()
        {
            List<IComponent> copy = new List<IComponent>(components);
            Clear();
            if (copy.Count == 0)
            {
                Clear();
                throw new InvalidOperationException("Cannot build - no components added");
            }

            if (copy.Count == 1)
            {
                return copy[0];
            }
            else
            {
                TaskList list = new TaskList(Name);
                foreach (var i in copy)
                {
                    if (i is Task t)
                        list.Add(new Task(t));
                    else if (i is TaskList tl)
                        list.Add(new TaskList(tl));
                }

                return list;
                
            }
        }

        public static void Clear()
        {
            components.Clear();
            _name = "No name"; // Reset do domyślnej
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

        public static List<Group> GetCategories()
        {
            return new List<Group>(categories);
        }
    }











    public static void Main()
    {
        // Przykładowe zadania
        var task1 = new Task("1A - Implementacja algorytmu sortowania", new DateTime(2024, 10, 21), new DateTime(2024, 10, 27));
        var task2 = new Task("1B - Analiza złożoności czasowej", new DateTime(2024, 10, 24), new DateTime(2024, 10, 31));
        var task3 = new Task("2A - Projektowanie schematu bazy danych", new DateTime(2024, 10, 28), new DateTime(2024, 11, 3));
        var task4 = new Task("2B - Tworzenie zapytań SQL", new DateTime(2024, 11, 1), new DateTime(2024, 11, 30));
        var task5 = new Task("2.1A - Implementacja rozwiązań", new DateTime(2024, 9, 1), new DateTime(2024, 11, 30));
        var task6 = new Task("2.1B - Testy", new DateTime(2025, 11, 5), new DateTime(2025, 11, 30));
        var task7 = new Task("3A - Przejrzenie wytycznych kodów", new DateTime(2024, 9, 1), new DateTime(2024, 11, 30));
        var task8 = new Task("3B - Wykonanie kodów", new DateTime(2025, 11, 5), new DateTime(2025, 11, 30));
        var task9 = new Task("3.1A - Testy kodów", new DateTime(2024, 9, 1), new DateTime(2024, 11, 30));
        var task10 = new Task("3.1B - Wgranie gotowych kodów", new DateTime(2025, 11, 5), new DateTime(2025, 11, 30));


        // Oznaczanie przykładowych zadań jako wykonane (z różnymi datami ukończenia)
        task1.MarkAsCompleted(new DateTime(2024, 10, 25)); // Wykonane na czas
        task2.MarkAsCompleted(new DateTime(2024, 11, 1)); // Wykonane z opóźnieniem
        // task3 i task4 są jeszcze niewykonane

        // Lista zadań (przykładowa organizacja wyłącznie według nazw)
        var tasks = new List<Task> { task1, task2, task3, task4 };

        // Wyświetlanie listy zadań i ich statusów
        TaskList gr0 = new TaskList("Lista zadań");

        TaskList gr1 = new TaskList("Zadania z algorytmiki");
        gr1.Add(task1);
        gr1.Add(task2);
        TaskList gr2 = new TaskList("Zadania z baz danych");
        gr2.Add(task3);
        gr2.Add(task4);
        TaskList gr3 = new TaskList("Zadania z baz danych - wdrażanie");
        gr3.Add(task5);
        gr3.Add(task6);
        TaskList gr4 = new TaskList("Zadania z programowania");
        gr4.Add(task7);
        gr4.Add(task8);
        TaskList gr5 = new TaskList("Zadania z programowania cd.");
        gr5.Add(task9);
        gr5.Add(task10);

        gr0.Add(gr1);
        gr0.Add(gr2);
        gr0.Add(gr3);
        gr0.Add(gr4);
        gr0.Add(gr5);
        
        gr4.MarkAsCompleted(new DateTime(2024, 11, 1));


        gr0.Display();
        gr0.Report();


    }
}
