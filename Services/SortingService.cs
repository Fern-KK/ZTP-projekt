using System.Collections.Generic;
using System.Linq;
using ZTP.Composite;

namespace ZTP;
using ZTP.Visitor;
using ZTP.Manager;
using ZTP.Strategy;
using ZTP.Services;
using ZTP.Builder;

public static class SortingService
{
    public static ISortingStrategy SortingStrategy { get; private set; } = new SortByNameStrategy();
    
    public static List<ISortingStrategy> AvailableStrategies { get; } = new List<ISortingStrategy>
    {
        new SortByNameStrategy(),
        new SortByEndDateStrategy(),
        new SortByPriorityStrategy(),
    };

    public static void SetSortingStrategy(ISortingStrategy strategy) => SortingStrategy = strategy;

    public static IEnumerable<IComponent> SortComponents(IEnumerable<IComponent> components)
    {
        return SortingStrategy.Sort(components);
    }

    public static IEnumerable<IComponent> SortByName(IEnumerable<IComponent> components)
    {
        return new SortByNameStrategy().Sort(components);
    }

    public static IEnumerable<IComponent> SortByEndDate(IEnumerable<IComponent> components)
    {
        return new SortByEndDateStrategy().Sort(components);
    }

    public static IEnumerable<IComponent> SortByPriority(IEnumerable<IComponent> components)
    {
        return new SortByPriorityStrategy().Sort(components);
    }
}