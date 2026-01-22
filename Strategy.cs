// Interfejst strategii
public interface ISortingStrategy
{
    IEnumerable<IComponent> Sort(IEnumerable<IComponent> components);
}

// Sortowanie alfabetyczne po nazwie
public class SortByNameStrategy : ISortingStrategy
{
    public IEnumerable<IComponent> Sort(IEnumerable<IComponent> components) =>
        components.OrderBy(c => c.Name);
}

// Sortowanie po EndDate (tylko dla obiektów implementujących ITaskComponent)
public class SortByEndDateStrategy : ISortingStrategy
{
    public IEnumerable<IComponent> Sort(IEnumerable<IComponent> components)
    {
        return components.OrderBy(c => 
        {
            // Próbujemy rzutować na ITaskComponent
            if (c is ITaskComponent task && task.EndDate.HasValue)
            {
                return task.EndDate.Value;
            }
            
            // Jeśli to nie task lub nie ma daty, zwracamy MaxValue, 
            // aby te elementy trafiły na koniec listy
            return DateTime.MaxValue;
        });
    }
}

// Sortowanie po priorytecie (tylko dla zadań, notatki idą na koniec)
public class SortByPriorityStrategy : ISortingStrategy
{
    public IEnumerable<IComponent> Sort(IEnumerable<IComponent> components) =>
        components.OrderByDescending(c => (c as ITaskComponent)?.Priority ?? Priorities.None);
}