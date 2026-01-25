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

// Interfejs obiektu odwiedzanego 
public interface IVisitedComponent
{
    void Accept(IVisitor visitor);
}

// Interfejs Visitor
public interface IVisitor
{
    void Visit(Note note);
    void Visit(Task task);
    void Visit(TaskList taskList);
    void Visit(Group group);
}

