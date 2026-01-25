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
using ZTP.Composite;
using ZTP.Visitor;
using ZTP.Manager;
using ZTP.Strategy;
using ZTP.Services;
using ZTP.Builder;


namespace ZTP.Strategy;


// Interfejst strategii
public interface ISortingStrategy
{
    IEnumerable<Composite.IComponent> Sort(IEnumerable<Composite.IComponent> components);
    string DisplayName { get; } 
}



// Sortowanie alfabetyczne po nazwie
public class SortByNameStrategy : ISortingStrategy
{
    public string DisplayName => "Nazwa (A-Z)";
    public IEnumerable<Composite.IComponent> Sort(IEnumerable<Composite.IComponent> components) =>
        components.OrderBy(c => c.Name);
}




// Sortowanie po EndDate (tylko dla obiektów implementujących ITaskComponent)
public class SortByEndDateStrategy : ISortingStrategy
{
    public string DisplayName => "Termin";
    public IEnumerable<Composite.IComponent> Sort(IEnumerable<Composite.IComponent> components)
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
    public IEnumerable<Composite.IComponent> Sort(IEnumerable<Composite.IComponent> components) =>
        components.OrderByDescending(c => (c as ITaskComponent)?.Priority ?? Priorities.None);
}
