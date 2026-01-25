using Avalonia.Controls;
using System.Collections.Generic;
using System.Linq;

namespace ZTP;

public static class SearchService
{
    public static StackPanel Search(string query)
    {
        string[] queryList = query.Split("+");
        var group = new Group($"Wyniki wyszukiwania: '{query}'");
        
        foreach (var q in queryList)
        {
            var visitor = new SearchVisitor(q);
            DataManager.AllGroup.Accept(visitor);

            var results = visitor.GetResults();
            foreach (var result in results)
            {
                group.Add(result);
            }
        }
        
        return group.SimpleDisplay();
    }

    public static StackPanel SearchByTagOrCategory(string tagOrCategory)
    {
        var group = new Group($"Elementy: {tagOrCategory}");
        var visitor = new SearchVisitor(tagOrCategory);
        DataManager.AllGroup.Accept(visitor);

        var results = visitor.GetResults();
        foreach (var result in results)
        {
            group.Add(result);
        }
        
        return group.SimpleDisplay();
    }
}