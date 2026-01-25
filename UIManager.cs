using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ZTP;

// Statyczna klasa pomocnicza zarządzająca dynamicznym tworzeniem i wyświetlaniem UI w głównym oknie aplikacj
public static class UIManager
{
    // Skrót do głównego okna aplikacji
    private static MainWindow MainWindow => MainWindow.Instance;
    
    public static void InitializeMainWindow(MainWindow window)
    {
        // Inicjalizacja jeśli potrzebna
    }
    
    // Główna metoda podmieniająca zawartość centralnego obszaru roboczego
    public static void DisplayContent(Control content)
    {
        MainWindow.Desktop.Content = content;
    }
    
    // Wyświetla prosty tekst na środku ekranu
    public static void DisplayText(string text)
    {
        DisplayContent(new TextBlock
        {
            Text = text,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            TextWrapping = TextWrapping.Wrap
        });
    }
    
    // Wyświetla strukturę grupy (zadań/notatek) pobierając jej graficzną reprezentację
    public static void DisplayGroup(Group group)
    {
        DisplayContent(group.SimpleDisplay());
    }
    
    // Wyświetla widok wyboru: czy użytkownik chce stworzyć nową notatkę, czy zadanie
    public static void DisplayNewObjectSelection()
    {
        var noteButton = CreateButton("Nowa notatka", () => MainWindow.Instance.CreateNoteView());
        var taskButton = CreateButton("Nowe zadanie", () => MainWindow.Instance.CreateTaskView());
        
        var panel = new StackPanel
        {
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Orientation = Orientation.Horizontal,
            Spacing = 10
        };
        
        panel.Children.Add(noteButton);
        panel.Children.Add(taskButton);
        
        DisplayContent(panel);
    }
    
    // Pobiera i wyświetla panel statystyk (wygenerowany przez wzorzec Visitor w GlobalGroups)
    public static void DisplayStatistics()
    {
        DisplayContent(GlobalGroups.GetStatistics());
    }
    
    // Pobiera i wyświetla raport nadchodzących terminów (domyślnie z 7 dni)
    public static void DisplayUpcomingTasks()
    {
        DisplayContent(GlobalGroups.GetUpcomingTasksReport(7));
    }
    
    // Wyświetla wyniki wyszukiwania na podstawie wpisanej frazy
    public static void DisplaySearchResults(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return;
            
        DisplayContent(GlobalGroups.Search(query));
    }
    
    // Wyświetla elementy przefiltrowane po konkretnym tagu lub kategorii
    public static void DisplayByTagOrCategory(string tagOrCategory)
    {
        DisplayContent(GlobalGroups.Search(tagOrCategory));
    }
    
    // Buduje dynamicznie formularz edycji/tworzenia notatki
    public static StackPanel CreateNoteEditor(
        string defaultTitle,
        out TextBox titleBox,
        out TextBox contentBox,
        ComboBox categoryComboBox,
        TextBox tagsTextBox,
        Action onSave)
    {
        var panel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Spacing = 10,
            Margin = new Thickness(20)
        };
        
        titleBox = new TextBox
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Text = defaultTitle,
            Margin =  new Thickness(0,0,10,0),
        };
        
        contentBox = new TextBox
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            MinHeight = 200,
            AcceptsReturn = true,
            Margin =  new Thickness(0,0,10,0),
        };
        
        // Tworzenie dolnego paska (kategorie, tagi, zapis)
        var bottomPanel = CreateBottomPanel(categoryComboBox, tagsTextBox, onSave, null);
        
        panel.Children.Add(titleBox);
        panel.Children.Add(contentBox);
        panel.Children.Add(bottomPanel);
        
        return panel;
    }
    
    // Buduje dynamicznie formularz edycji/tworzenia listy zadań
    public static StackPanel CreateTaskEditor(
        string defaultTitle,
        out TextBox titleBox,
        StackPanel tasksSection,
        Action onAddTask,
        ComboBox categoryComboBox,
        TextBox tagsTextBox,
        ComboBox priorityComboBox,
        Action onSave)
    {
        var panel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Spacing = 10,
            Margin = new Thickness(20)
        };
        
        titleBox = new TextBox
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Text = defaultTitle
        };
        
        var addButtonSection = new StackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Center
        };
        
        // Przycisk pozwalający dynamicznie dodawać kolejne wiersze zadań do tasksSection
        var addButton = CreateButton("Dodaj zadanie", onAddTask);
        addButtonSection.Children.Add(addButton);
        
        var bottomPanel = CreateBottomPanel(categoryComboBox, tagsTextBox, onSave, priorityComboBox);
        
        panel.Children.Add(titleBox);
        panel.Children.Add(tasksSection);
        panel.Children.Add(addButtonSection);
        panel.Children.Add(bottomPanel);
        
        return panel;
    }
    
    // Tworzy pojedynczy wiersz dla zadania (tekst + wybór daty) wewnątrz listy zadań
    public static StackPanel CreateTaskInputRow(out TextBox taskTextBox, out DatePicker datePicker)
    {
        var panel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 5
        };
        
        taskTextBox = new TextBox
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Watermark = "nowe zadanie",
            Width = 400
        };
        
        datePicker = new DatePicker();
        
        panel.Children.Add(taskTextBox);
        panel.Children.Add(datePicker);
        
        return panel;
    }
    
    // Funkcja pomocnicza do szybkiego tworzenia przycisku z akcją
    private static Button CreateButton(string content, Action onClick)
    {
        var button = new Button { Content = content };
        button.Click += (s, e) => onClick();
        return button;
    }
    
    // Wspólna metoda tworząca dolny pasek formularza (Grid z dwiema kolumnami)
    private static StackPanel CreateBottomPanel(
        ComboBox categoryComboBox,
        TextBox tagsTextBox,
        Action onSave,
        ComboBox priorityComboBox)
    {
        var grid = new Grid
        {
            ColumnDefinitions = ColumnDefinitions.Parse("Auto, *")
        };
        
        // Lewa strona: Metadane (Kategoria, Tagi, Priorytet)
        var leftPanel = new StackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            Orientation = Orientation.Horizontal,
            Spacing = 5
        };
        Grid.SetColumn(leftPanel, 0);
        
        leftPanel.Children.Add(categoryComboBox);
        leftPanel.Children.Add(tagsTextBox);
        
        if (priorityComboBox != null)
        {
            leftPanel.Children.Add(priorityComboBox);
        }
        
        // Prawa strona: Przycisk zapisu
        var rightPanel = new StackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        Grid.SetColumn(rightPanel, 1);
        
        var saveButton = CreateButton(
            priorityComboBox != null ? "Zapisz zadania" : "Zapisz notatkę",
            onSave);
        rightPanel.Children.Add(saveButton);
        
        grid.Children.Add(leftPanel);
        grid.Children.Add(rightPanel);
        
        var wrapper = new StackPanel();
        wrapper.Children.Add(grid);
        
        return wrapper;
    }
    
    // Wizualizuje błąd walidacji
    public static void ShowValidationError(Control control, bool hasError)
    {
        if (control == null) return;
        
        // Wybór klasy stylu w zależności od typu błędu (data vs brak tekstu)
        if (hasError)
            control.Classes.Add(control is DatePicker ? "wrongDate" : "mustFill");
        else
            control.Classes.Remove(control is DatePicker ? "wrongDate" : "mustFill");
    }
    
    // Tworzy sekcję menu bocznego na podstawie listy przycisków (np. tagów lub kategorii)
    public static StackPanel CreateMenuSection(List<Button> buttons, Action<string> onButtonClick)
    {
        var panel = new StackPanel();
        
        foreach (var button in buttons)
        {
            // Podpięcie wspólnego zdarzenia kliknięcia, które przekazuje nazwę przycisku (np. nazwę tagu)
            button.Click += (s, e) => onButtonClick(button.Name);
            panel.Children.Add(button);
        }
        
        return panel;
    }
}
