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

namespace ZTP.Visitor;


// Visitor dla wyszukiwania
public class SearchVisitor : IVisitor
{
    private string SearchQuery;
    private List<Composite.IComponent> SearchResults = new List<Composite.IComponent>();

    public SearchVisitor(string query)
    {
        SearchQuery = query.ToLower();
    }

    public List<Composite.IComponent> GetResults() => SearchResults;

    public void Visit(Note note)
    {
        if (MatchesSearch(note))
        {
            SearchResults.Add(note);
        }
    }

    public void Visit(Task task)
    {
        if (MatchesSearch(task))
        {
            SearchResults.Add(task);
        }
    }

    public void Visit(TaskList taskList)
    {
        if (MatchesSearch(taskList))
            SearchResults.Add(taskList);

        // Przeszukaj również podzadania
        var components = taskList.GetType().GetProperty("components")?.GetValue(taskList) as List<ITaskComponent>;
        if (components != null)
        {
            foreach (var component in components)
            {
                switch(component)
                {
                    case Task t:
                        if (MatchesSearch(t))
                            SearchResults.Add(taskList);
                        break;

                    case TaskList tl:
                        if (MatchesSearch(tl))
                            SearchResults.Add(taskList);
                        break;
                }
            }
        }
    }

    public void Visit(Group group)
    {
        if (MatchesSearch(group))
        {
            SearchResults.Add(group);
        }
            

        // Przeszukaj elementy grupy
        var components = group.GetComponents();
        foreach (var component in components)
        {
            if (component is Note n)
                Visit(n);
            else if (component is Task t)
                Visit(t);
            else if (component is TaskList tl)
                Visit(tl);
            else if (component is Group g)
                Visit(g);
        }
    }

    private bool MatchesSearch(Composite.IComponent component)
    {
        if (string.IsNullOrWhiteSpace(SearchQuery))
            return false;

        // Sprawdź nazwę
        if (component.Name.ToLower().Contains(SearchQuery))
            return true;

        // Sprawdź kategorię
        if (component.Category?.ToLower().Contains(SearchQuery) == true)
            return true;

        // Sprawdź pola specjalne dla poszczególnych kategorii
        switch (component)
        {
            case Note note:
                // Sprawdź opis
                if (note.Content?.ToLower().Contains(SearchQuery) == true)
                    return true;
                
                // Sprawdź tagi
                if (note.Tags.Any(tag => tag.ToLower().Contains(SearchQuery)))
                    return true;
                break;
            
            case ITaskComponent task:
                // Sprawdź tagi
                if (task.Tags.Any(tag => tag.ToLower().Contains(SearchQuery)))
                    return true;
                break;
        }

        return false;
    }
}
