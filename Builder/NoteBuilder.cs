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

namespace ZTP.Builder;



public class BuilderNote 
{
    public string CurrentName {get; set; } = "";
    public string Category { get; set; } = "";
    public string Tags { get; set; } = "";
    private string Content = "";
    
    public BuilderNote SetName(string name)
    {
        CurrentName = name;
        return this;
    }

    public BuilderNote SetContent(string content)
    {
        Content = content;
        return this;
    }
    public BuilderNote SetCategory(string category)
    {
        Category=category;
        return this;
    }
    public BuilderNote SetTags(string tags)
    {
        Tags=tags;
        return this;
    }

    public string GetName()
    {
        if (string.IsNullOrEmpty(CurrentName) || CurrentName == DefaultName())
        {
            string NewName = DefaultName();
            return NewName;
        }
        return CurrentName;
    }
    public string DefaultName()
    {
        return $"Nowa notatka";
    }

    public BuilderNote Build()
    {
        Note note = new Note(GetName(), Content);
        DataManager.AllGroup.Add(note);
        DataManager.AllNotesGroup.Add(note);
        ServerConnection client = ServerConnection.CreateServerConnection();
        
        if (Category != null)
        {
            note.SetCategory(Category);
        }
        if(Tags != null)
        {
            var tags = Tags.Split(',');
            foreach (var t in tags)
            {
                string tag = t?.Trim().ToLower() ?? "";
                if (!string.IsNullOrWhiteSpace(tag))
                {
                    note.SetTags(tag);
                }
            }
        }
        client.NewNote(note);
        Clear();

        return this;
    }

    
    public BuilderNote Clear()
    {
        CurrentName = "";
        Content = "";
        Category = "";
        Tags = "";

        return this;
    }
}
