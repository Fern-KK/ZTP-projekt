using Avalonia;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ZTP;

    public interface IState
    {
        void WithName(string name);
        void AddComponent(IComponent component);
        IComponent Build();
        void Clear();
        string GetName();
    }

    public class TaskBuilderState : IState
    {
        private List<IComponent> components = new List<IComponent>();
        private string _name = "";

        public string GetName()
        {
            if (string.IsNullOrEmpty(_name) && components.Count > 0)
                return components.First().Name;
            return _name;
        }

        public void WithName(string name)
        {
            _name = name;
        }

        public void AddComponent(IComponent component)
        {
            if (component is ITaskComponent)
            {
                components.Add(component);
            }
            else
            {
                throw new InvalidOperationException("TaskBuilder can only add task components");
            }
        }

        public IComponent Build()
        {
            if (components.Count == 0)
                throw new InvalidOperationException("Cannot build - no components added");

            string buildName = GetName();
            List<IComponent> copy = new List<IComponent>(components);
            Clear();

            if (copy.Count == 1)
            {
                return copy[0];
            }
            else
            {
                TaskList list = new TaskList(buildName);
                foreach (var component in copy)
                {
                    if (component is Task task)
                        list.Add(new Task(task));
                    else if (component is TaskList taskList)
                        list.Add(new TaskList(taskList));
                }
                return list;
            }
        }

        public void Clear()
        {
            components.Clear();
            _name = "";
        }
    }

    public class NoteBuilderState : IState
    {
        private List<IComponent> components = new List<IComponent>();
        private string _name = "";

        public string GetName()
        {
            if (string.IsNullOrEmpty(_name) && components.Count > 0)
                return components.First().Name;
            return _name;
        }

        public void WithName(string name)
        {
            _name = name;
        }

        public void AddComponent(IComponent component)
        {
            if (component is Note note)
            {
                components.Add(component);
            }
            else
            {
                throw new InvalidOperationException("NoteBuilder can only add notes");
            }
        }

        public IComponent Build()
        {
            if (components.Count == 0)
                throw new InvalidOperationException("Cannot build - no components added");

            if (components.Count > 1)
            {
                Console.WriteLine("Warning: Multiple notes added, returning only the first one");
            }

            IComponent result = components.First();
            Clear();
            return result;
        }

        public void Clear()
        {
            components.Clear();
            _name = "";
        }
    }



    public static class Builder
    {
        private static IState currentState = new NoteBuilderState();


        public static string Name
        {
            get
            {
                return currentState.GetName();
            }
        }

        public static void SetState(IState state)
        {
            currentState = state;
        }

        public static void WithName(string name)
        {
            currentState.WithName(name);
        }

        public static void AddComponent(IComponent component)
        {
            // Automatycznie zmień stan na podstawie typu komponentu
            if (component is Note && !(currentState is NoteBuilderState))
            {
                Console.WriteLine("Automatically switching to NoteBuilder state");
                SetState(new NoteBuilderState());
            }
            else if (component is ITaskComponent && !(currentState is TaskBuilderState))
            {
                Console.WriteLine("Automatically switching to TaskBuilder state");
                SetState(new TaskBuilderState());
            }
            
            currentState.AddComponent(component);
        }

        public static IComponent Build()
        {
            return currentState.Build();
        }

        public static void Clear()
        {
            currentState.Clear();
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