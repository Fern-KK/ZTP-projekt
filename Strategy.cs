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


// Interfejst strategii
public interface ISortingStrategy
{
    IEnumerable<IComponent> Sort(IEnumerable<IComponent> components);
    string DisplayName { get; } // Dodana właściwość do wyświetlania
}

// Sortowanie alfabetyczne po nazwie
public class SortByNameStrategy : ISortingStrategy
{
    public string DisplayName => "Nazwa (A-Z)";
    public IEnumerable<IComponent> Sort(IEnumerable<IComponent> components) =>
        components.OrderBy(c => c.Name);
}

// Sortowanie po EndDate (tylko dla obiektów implementujących ITaskComponent)
public class SortByEndDateStrategy : ISortingStrategy
{
    public string DisplayName => "Termin";
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
    public string DisplayName => "Priorytet";
    public IEnumerable<IComponent> Sort(IEnumerable<IComponent> components) =>
        components.OrderByDescending(c => (c as ITaskComponent)?.Priority ?? Priorities.None);
}
