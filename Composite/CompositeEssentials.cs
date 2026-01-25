using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ZTP.Composite;
using ZTP.Visitor;
using ZTP.Manager;
using ZTP.Strategy;
using ZTP.Services;
using ZTP.Builder;


namespace ZTP.Composite;

// Poziomy ważności dla zadań
public enum Priorities
{
    None,
    Low,
    Normal,
    Important
}

// Podstawowy interfejs dla wszystkich elementów systemu (notatek, zadań, grup)
public interface IComponent : IVisitedComponent
{
    string Name { get; }
    DateTime StartDate { get; }
    List<string> Tags { get; }
    string Category { get; }
    StackPanel SimpleDisplay(int depth);
    StackPanel SimpleDisplay();
}

// Interfejs rozszerzający komponent o dodatkowe funkcjonalności zadań
public interface ITaskComponent : IComponent
{
    DateTime? EndDate { get; }
    bool IsCompleted { get; }
    bool IsLate { get; }
    Priorities Priority { get; }
    void MarkAsCompleted(DateTime completionDate);
    void MarkAsIncomplete();
    string GetStatus();
    void SetPriority(Priorities priority);
    void SetTags(string tag);
    void SetTags(List<string> tags);
    void SetCategory(string category);
}