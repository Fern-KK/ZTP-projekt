using Avalonia;
using System;

namespace ZTP;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}



// using System.Collections.Generic;
// using System.ComponentModel;
// using System.ComponentModel.DataAnnotations;
// using System.Linq;


// public enum Priorities
// {
//     None,
//     Low,
//     Normal,
//     Important
// }


























// public interface IComponent
// {
//     public string Name { get; }
//     DateTime StartDate { get; }
//     public void Display(int depth);
//     public void Display();
// }




// public class Note : IComponent
// {
//     public string Name { get; }
//     public string Content { get; }
//     public DateTime StartDate { get; }

//     // Konstruktor klasy Task, ustawiający nazwę oraz daty początku i końca zadania
//     public Note(string name, string content)
//     {
//         Name = name;
//         Content = content;
//         StartDate = DateTime.Now;
//     }
//     public Note(Note other)
//     {
//         Name = other.Name;
//         Content = other.Content;
//         StartDate = other.StartDate;
//     }


//     // Używana do wyświetlenia szczegółów zadania wraz ze statusem
//     public void Display()
//     {
//         this.Display(1);
//     }
//     public void Display(int depth)
//     {
//         Console.WriteLine(new String('-', depth) + $"{Name} ({StartDate:dd.MM.yyyy}) \n{new String(' ', depth)}Treść: {Content}");
//     }
// }











// public interface ITaskComponent : IComponent
// {
//     DateTime EndDate { get; }
//     bool IsCompleted { get; }
//     bool IsLate { get; }
//     Priorities Priority { get; }
//     public void MarkAsCompleted(DateTime completionDate);
//     public string GetStatus();
//     void SetPriority(Priorities priority);
// }




// public class Task : ITaskComponent
// {
//     public string Name { get; }
//     public DateTime StartDate { get; }
//     public DateTime EndDate { get; }
//     public Priorities Priority { get; private set; } = 0;
//     public bool IsCompleted { get; private set; } = false;
//     public bool IsLate { get; private set; } = false;

//     // Konstruktor klasy Task, ustawiający nazwę oraz daty początku i końca zadania
//     public Task(string name, DateTime endDate)
//     {
//         Name = name;
//         StartDate = DateTime.Now;
//         EndDate = endDate;
//     }
//     public Task(Task other)
//     {
//         Name = other.Name;
//         StartDate = other.StartDate;
//         EndDate = other.EndDate;
//         Priority = other.Priority;
//         IsCompleted = other.IsCompleted;
//         IsLate = other.IsLate;

//     }

//     // Metoda oznaczająca zadanie jako wykonane; przyjmuje datę wykonania i sprawdza, czy zadanie wykonano na czas
//     public void MarkAsCompleted(DateTime completionDate)
//     {
//         IsCompleted = true;
//         IsLate = completionDate > EndDate;
//     }

//     // Zwraca status zadania: "Completed", "Completed Late" lub "Pending"
//     public string GetStatus()
//     {
//         if (IsCompleted)
//             return IsLate ? "[Completed Late]" : "[Completed]";
//         return "[Pending]";
//     }

//     public void SetPriority(Priorities priority)
//     {
//         Priority = priority;
//     }

//     // Używana do wyświetlenia szczegółów zadania wraz ze statusem
//     public void Display()
//     {
//         this.Display(1);
//     }
//     public void Display(int depth)
//     {
//         Console.WriteLine(new String('-', depth) + $"{Name} ({StartDate:dd.MM.yyyy} to {EndDate:dd.MM.yyyy}) - Status: {GetStatus()}");
//     }
// }





















// public class TaskList : ITaskComponent
// {
//     public string Name { get; }
//     private List<ITaskComponent> components = new List<ITaskComponent>();

//     public DateTime StartDate
//     {
//         get
//         {
//             if (components.Count == 0)
//                 return DateTime.MinValue;
//             return components.Min(component => component.StartDate);
//         }
//     }

//     public DateTime EndDate
//     {
//         get
//         {
//             if (components.Count == 0)
//                 return DateTime.MaxValue;
//             return components.Max(component => component.EndDate);
//         }
//     }

//     public bool IsCompleted
//     {
//         get
//         {
//             return components.Count > 0 && components.All(component => component.IsCompleted);
//         }
//     }

//     public bool IsLate
//     {
//         get
//         {
//             return components.Count > 0 && components.Any(component => component.IsLate);
//         }
//     }
//     public Priorities Priority { get; private set; } = 0;

//     public TaskList(string name)
//     {
//         Name = name;
//     }

//     public TaskList(string name, List<ITaskComponent> list)
//     {
//         Name = name;
//         components = list;
//     }

//     public TaskList(TaskList other)
//     {
//         Name = other.Name;
//         components = other.components;
//     }

//     public void Add(ITaskComponent component)
//     {
//         components.Add(component);
//     }

//     public void Remove(ITaskComponent component)
//     {
//         components.Remove(component);
//     }
//     public void SetPriority(Priorities priority)
//     {
//         Priority = priority;
//     }

//     public void MarkAsCompleted(DateTime completionDate)
//     {
//         foreach (var component in components)
//         {
//             component.MarkAsCompleted(completionDate);
//         }
//     }

//     public string GetStatus()
//     {
//         if (IsCompleted)
//             return IsLate ? "[Completed Late]" : "[Completed]";
//         return "[Pending]";
//     }
//     public void Display()
//     {
//         this.Display(1);
//     }
//     public void Display(int depth)
//     {
//         Console.WriteLine(new String('-', depth) + $"{Name} ({StartDate:dd.MM.yyyy} to {EndDate:dd.MM.yyyy}) - Status: {GetStatus()}");
//         foreach (var component in components)
//         {
//             component.Display(depth + 2);
//         }
//     }
//     private int[] getStatistics()
//     {
//         int[] statistics = new int[4] {
//         components.OfType<Task>().Count(t => t.IsCompleted && !t.IsLate),
//         components.OfType<Task>().Count(t => t.IsCompleted && t.IsLate),
//         components.OfType<Task>().Count(t => !t.IsCompleted),
//         components.OfType<Task>().Count(t => !t.IsCompleted && DateTime.Now > t.EndDate)
//         };

//         foreach (TaskList group in components.OfType<TaskList>())
//         {
//             int[] groupReport = group.getStatistics();
//             for (int i = 0; i < statistics.Length; i++)
//             {
//                 statistics[i] += groupReport[i];
//             }
//         }

//         return statistics;
//     }
//     public void Report()
//     {
//         int[] stat = this.getStatistics();
//         Console.WriteLine("\nPodsumowanie zadań:");
//         Console.WriteLine($"Zadania wykonane na czas: {stat[0]}");
//         Console.WriteLine($"Zadania wykonane z opóźnieniem: {stat[1]}");
//         Console.WriteLine($"Zadania oczekujące: {stat[2]}");
//         Console.WriteLine($"Zadania oczekujące z przekroczonym terminem: {stat[3]}");
//     }

// }


































// public class Group
// {
//     public string Name { get; }
//     private List<IComponent> components = new List<IComponent>();
//     public Group(string name)
//     {
//         Name = name;
//     }

//     public void Add(IComponent component)
//     {
//         components.Add(component);
//     }

//     public void Remove(IComponent component)
//     {
//         components.Remove(component);
//     }
//     public bool Contains(IComponent component)
//     {
//         return components.Contains(component);
//     }
//     public int Count()
//     {
//         return components.Count();
//     }
//     public void Display()
//     {
//         this.Display(1);
//     }
//     public void Display(int depth)
//     {
//         foreach (var component in components)
//         {
//             component.Display(depth + 2);
//         }
//     }
// }






// /*
// public interface ICommand
// {
//     void Execute();
//     void Undo();
// };

// public class NameCommand : ICommand
// {
//     private Component component;

// };
// */




// public class Program
// {


//     public interface IState
//     {
//         void WithName(string name);
//         void AddComponent(IComponent component);
//         IComponent Build();
//         void Clear();
//         string GetName();
//     }

//     public class TaskBuilderState : IState
//     {
//         private List<IComponent> components = new List<IComponent>();
//         private string _name = "";

//         public string GetName()
//         {
//             if (string.IsNullOrEmpty(_name) && components.Count > 0)
//                 return components.First().Name;
//             return _name;
//         }

//         public void WithName(string name)
//         {
//             _name = name;
//         }

//         public void AddComponent(IComponent component)
//         {
//             if (component is ITaskComponent)
//             {
//                 components.Add(component);
//             }
//             else
//             {
//                 throw new InvalidOperationException("TaskBuilder can only add task components");
//             }
//         }

//         public IComponent Build()
//         {
//             if (components.Count == 0)
//                 throw new InvalidOperationException("Cannot build - no components added");

//             string buildName = GetName();
//             List<IComponent> copy = new List<IComponent>(components);
//             Clear();

//             if (copy.Count == 1)
//             {
//                 return copy[0];
//             }
//             else
//             {
//                 TaskList list = new TaskList(buildName);
//                 foreach (var component in copy)
//                 {
//                     if (component is Task task)
//                         list.Add(new Task(task));
//                     else if (component is TaskList taskList)
//                         list.Add(new TaskList(taskList));
//                 }
//                 return list;
//             }
//         }

//         public void Clear()
//         {
//             components.Clear();
//             _name = "";
//         }
//     }

//     public class NoteBuilderState : IState
//     {
//         private List<IComponent> components = new List<IComponent>();
//         private string _name = "";

//         public string GetName()
//         {
//             if (string.IsNullOrEmpty(_name) && components.Count > 0)
//                 return components.First().Name;
//             return _name;
//         }

//         public void WithName(string name)
//         {
//             _name = name;
//         }

//         public void AddComponent(IComponent component)
//         {
//             if (component is Note note)
//             {
//                 components.Add(component);
//             }
//             else
//             {
//                 throw new InvalidOperationException("NoteBuilder can only add notes");
//             }
//         }

//         public IComponent Build()
//         {
//             if (components.Count == 0)
//                 throw new InvalidOperationException("Cannot build - no components added");

//             if (components.Count > 1)
//             {
//                 Console.WriteLine("Warning: Multiple notes added, returning only the first one");
//             }

//             IComponent result = components.First();
//             Clear();
//             return result;
//         }

//         public void Clear()
//         {
//             components.Clear();
//             _name = "";
//         }
//     }



//     public static class Builder
//     {
//         private static IState currentState = new NoteBuilderState();


//         public static string Name
//         {
//             get
//             {
//                 return currentState.GetName();
//             }
//         }

//         public static void SetState(IState state)
//         {
//             currentState = state;
//         }

//         public static void WithName(string name)
//         {
//             currentState.WithName(name);
//         }

//         public static void AddComponent(IComponent component)
//         {
//             // Automatycznie zmień stan na podstawie typu komponentu
//             if (component is Note && !(currentState is NoteBuilderState))
//             {
//                 Console.WriteLine("Automatically switching to NoteBuilder state");
//                 SetState(new NoteBuilderState());
//             }
//             else if (component is ITaskComponent && !(currentState is TaskBuilderState))
//             {
//                 Console.WriteLine("Automatically switching to TaskBuilder state");
//                 SetState(new TaskBuilderState());
//             }
            
//             currentState.AddComponent(component);
//         }

//         public static IComponent Build()
//         {
//             return currentState.Build();
//         }

//         public static void Clear()
//         {
//             currentState.Clear();
//         }
//     }







//     public static class Categories
//     {
//         private static List<Group> categories = new List<Group>();

//         public static void Add(string name)
//         {
//             // Check if category already exists
//             if (categories.Any(c => c.Name == name.ToLower()))
//             {
//                 Console.WriteLine("Category already exists");
//             }
//             else
//             {
//                 categories.Add(new Group(name.ToLower()));
//             }
//         }

//         public static void Remove(string name)
//         {
//             var category = categories.FirstOrDefault(c => c.Name == name.ToLower());
//             if (category != null)
//             {
//                 categories.Remove(category);
//                 category = null;
//             }
//             else
//             {
//                 Console.WriteLine("Category not found");
//             }
//         }

//         public static void AddToCategory(IComponent component, string categoryName)
//         {
//             // Najpierw usuwamy komponent ze wszystkich istniejących kategorii
//             foreach (var category in categories)
//             {
//                 if (category.Contains(component))
//                 {
//                     category.Remove(component);
//                     break; // Zakładamy że komponent jest tylko w jednej kategorii
//                 }
//             }

//             // Teraz dodajemy do nowej kategorii
//             var targetCategory = categories.FirstOrDefault(c => c.Name == categoryName.ToLower());
//             if (targetCategory != null)
//             {
//                 targetCategory.Add(component);
//             }
//             else
//             {
//                 Console.WriteLine($"Category '{categoryName}' not found");
//             }
//         }

//         public static void RemoveFromCategory(IComponent component, string categoryName)
//         {
//             var category = categories.FirstOrDefault(c => c.Name == categoryName.ToLower());
//             if (category != null)
//             {
//                 category.Remove(component);
//             }
//             else
//             {
//                 Console.WriteLine($"Category '{categoryName}' not found");
//             }
//         }

//         public static void Display()
//         {
//             Console.WriteLine("Categories: ");
//             foreach (var category in categories)
//             {
//                 Console.WriteLine("-" + category.Name);
//             }
//         }
//         public static void DisplayCategory(int index)
//         {
//             if (index < 0 || index >= categories.Count)
//             {
//                 Console.WriteLine("Nieprawidłowy numer kategorii!");
//                 return;
//             }
//             var category = categories[index];
//             category.Display();
//         }
//         public static int Count()
//         {
//             return categories.Count();
//         }

//         public static List<Group> GetCategories()
//         {
//             return new List<Group>(categories);
//         }
//     }


//     public static class Tags
//     {
//         private static List<Group> tags = new List<Group>();

//         public static void Add(string name)
//         {
//             if (tags.Any(c => c.Name == name.ToLower()))
//             {
//                 Console.WriteLine("Category already exists");
//             }
//             else
//             {
//                 tags.Add(new Group(name.ToLower()));
//             }
//         }

//         public static void Remove(string name)
//         {
//             var tag = tags.FirstOrDefault(c => c.Name == name.ToLower());
//             if (tag != null)
//             {
//                 tags.Remove(tag);
//                 tag = null;
//             }
//             else
//             {
//                 Console.WriteLine("Tag not found");
//             }
//         }

//         public static void AddToCategory(IComponent component, string tagName)
//         {


//             // Teraz dodajemy do nowej kategorii
//             var targetTag = tags.FirstOrDefault(c => c.Name == tagName.ToLower());
//             if (targetTag != null)
//             {
//                 targetTag.Add(component);
//             }
//             else
//             {
//                 Console.WriteLine($"Tag '{tagName}' not found");
//             }
//         }

//         public static void RemoveFromCategory(IComponent component, string tagName)
//         {
//             var category = tags.FirstOrDefault(c => c.Name == tagName.ToLower());
//             if (category != null)
//             {
//                 category.Remove(component);
//             }
//             else
//             {
//                 Console.WriteLine($"Tag '{tagName}' not found");
//             }
//         }

//         public static void Display()
//         {
//             Console.WriteLine("Tags: ");
//             foreach (var t in tags)
//             {
//                 Console.WriteLine("#" + t.Name);
//             }
//         }
//         public static void DisplayCategory(int index)
//         {
//             if (index < 0 || index >= tags.Count)
//             {
//                 Console.WriteLine("Nieprawidłowy numer tagu!");
//                 return;
//             }
//             var tag = tags[index];
//             tag.Display();
//         }
//         public static int Count()
//         {
//             return tags.Count();
//         }

//         public static List<Group> GetTags()
//         {
//             return new List<Group>(tags);
//         }
//     }






//     private static Group AllGroup = new Group("all");
//     private static Group AllTasksGroup = new Group("Tasks");
//     private static Group AllNotesGroup = new Group("Notes");


//     public static void Main()
//     {
//         Categories.Add("szkoła");
//         Categories.Add("dom");

//         bool exitProgram = false;
//         while (!exitProgram)
//         {
//             Console.Clear();
//             DisplayMainMenu();

//             if (int.TryParse(Console.ReadLine(), out int choice))
//             {
//                 switch (choice)
//                 {
//                     case 0:
//                         exitProgram = true;
//                         Console.WriteLine("Zamykanie programu...");
//                         break;
//                     case 1:
//                         DisplayMenu();
//                         break;
//                     case 2:
//                         AddMenu();
//                         break;
//                     case 3:
//                         //EditMenu();
//                         break;
//                     default:
//                         Console.WriteLine("Nieprawidłowy wybór!");
//                         Console.ReadKey();
//                         break;
//                 }
//             }
//             else
//             {
//                 Console.WriteLine("Nieprawidłowe dane wejściowe!");
//                 Console.ReadKey();
//             }
//         }
//     }

//     private static void DisplayMainMenu()
//     {
//         Console.WriteLine("=======================================");
//         Console.WriteLine("   SYSTEM ZARZĄDZANIA ZADANIAMI");
//         Console.WriteLine("=======================================");

//         Console.WriteLine($"\nSTATYSTYKI:");
//         Console.WriteLine($"Wszystkie elementy: {AllGroup.Count()}");
//         Console.WriteLine($"Zadania: {AllTasksGroup.Count()}");
//         Console.WriteLine($"Notatki: {AllNotesGroup.Count()}");
//         Console.WriteLine($"Kategorie: {Categories.Count()}");
//         Console.WriteLine($"Tagi: {Tags.Count()}");

//         Console.WriteLine("\n=== GŁÓWNE MENU ===");
//         Console.WriteLine("0. Wyjście");
//         Console.WriteLine("1. Wyświetl");
//         Console.WriteLine("2. Dodaj");
//         Console.WriteLine("3. Edytuj");
//         Console.Write("\nWybierz opcję: ");
//     }



//     private static void DisplayMenu()
//     {
//         bool back = false;
//         while (!back)
//         {
//             Console.Clear();
//             Console.WriteLine("\n=== WYŚWIETL ===");
//             Console.WriteLine("0. Powrót");
//             Console.WriteLine("1. Wyświetl wszystko");
//             Console.WriteLine("2. Wyświetl zadania");
//             Console.WriteLine("3. Wyświetl notatki");
//             Categories.Display();
//             Tags.Display();
//             Console.Write("\nWybierz opcję: ");

//             if (int.TryParse(Console.ReadLine(), out int choice))
//             {
//                 switch (choice)
//                 {
//                     case 0:
//                         back = true;
//                         break;
//                     case 1:
//                         Console.WriteLine("\n=== WSZYSTKIE ELEMENTY ===");
//                         AllGroup.Display();
//                         Console.ReadKey();
//                         break;
//                     case 2:
//                         Console.WriteLine("\n=== ZADANIA ===");
//                         AllTasksGroup.Display();
//                         Console.ReadKey();
//                         break;
//                     case 3:
//                         Console.WriteLine("\n=== NOTATKI ===");
//                         AllNotesGroup.Display();
//                         Console.ReadKey();
//                         break;
//                     case 4:
//                         // DisplayByCategory();
//                         break;
//                     default:
//                         Console.WriteLine("Nieprawidłowy wybór!");
//                         Console.ReadKey();
//                         break;
//                 }
//             }
//         }
//     }
//     private static void AddMenu()
//     {
//         bool back = false;
//         while (!back)
//         {
//             Console.Clear();
//             Console.WriteLine("\n=== DODAJ NOWY ELEMENT ===");
//             Console.WriteLine("0. Powrót");
//             Console.WriteLine("1. Dodaj notatkę");
//             Console.WriteLine("2. Dodaj zadanie");
//             Console.Write("\nWybierz opcję: ");

//             if (int.TryParse(Console.ReadLine(), out int choice))
//             {
//                 switch (choice)
//                 {
//                     case 0:
//                         back = true;
//                         // Zbuduj i dodaj komponent do odpowiednich grup
//                         try
//                         {
//                             IComponent builtComponent = Builder.Build();
//                             if (builtComponent != null)
//                             {
//                                 AllGroup.Add(builtComponent);

//                                 // Dodaj do odpowiedniej grupy
//                                 if (builtComponent is Note)
//                                 {
//                                     AllNotesGroup.Add(builtComponent);
//                                 }
//                                 else if (builtComponent is ITaskComponent)
//                                 {
//                                     AllTasksGroup.Add(builtComponent);
//                                 }

//                                 Console.WriteLine($"Dodano: {builtComponent.Name}");
//                             }
//                         }
//                         catch (InvalidOperationException ex)
//                         {
//                             Console.WriteLine($"Błąd: {ex.Message}");
//                         }
//                         Console.ReadKey();
//                         break;

//                     case 1:
//                         Console.Write("\nPodaj tytuł notatki: ");
//                         string noteTitle = Console.ReadLine();
//                         Console.Write("Podaj treść notatki: ");
//                         string noteContent = Console.ReadLine();

//                         // Ustaw nazwę dla Buildera
//                         Builder.WithName(noteTitle);
//                         // Dodaj komponent
//                         Builder.AddComponent(new Note(noteTitle, noteContent));
//                         Console.WriteLine($"Dodano notatkę: {noteTitle}");
//                         break;

//                     case 2:
//                         Console.Write("\nPodaj nazwę zadania: ");
//                         string taskName = Console.ReadLine();
//                         Console.Write("Podaj datę zakończenia (dd.MM.yyyy): ");

//                         if (DateTime.TryParse(Console.ReadLine(), out DateTime endDate))
//                         {
//                             // Dodaj komponent
//                             Builder.AddComponent(new Task(taskName, endDate));
//                             Console.WriteLine($"Dodano zadanie: {taskName}");
//                         }
//                         else
//                         {
//                             Console.WriteLine("Nieprawidłowa data!");
//                         }
//                         break;

//                     default:
//                         Console.WriteLine("Nieprawidłowy wybór!");
//                         Console.ReadKey();
//                         break;
//                 }
//             }
//         }
//     }
// }