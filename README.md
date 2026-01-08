#### Aby wysztko z Avalonią działało trzeba dodać do VSCode takie rozszerzenia:
- Avalonia Templates
- Avalonia for VSCode



#### Podzieliłem wcześniejszt kod na 3 pliki: 
- Composite.cs
- Builder.cs
- GlobalGroups.cs

#### Istotne pliki Avalonii:
- MainWindow.axaml.cs - to jak plik js w stronach internetowych. Tak funkcjonalnie jest Program.cs naszego programu, gdzie MainWindow() jest odpowiednikiem normalnego Main()
- MainWindow.axaml - To jak plik html w stronach internetowej
- App.axaml - to jak plik css, gdzie definuje style 






### Aplikacja do zarządzania notatkami i zadaniami. 
Aplikacja umożliwia użytkownikowi tworzenie, edytowanie i organizowanie notatek oraz list zadań. Notatki i zadania można opatrywać tagami (np. „praca”, „pomysł”) lub przypisywać do kategorii (np. „dom”, „projekt”). Zadania można oznaczać jako wykonane i nadawać im priorytety (np. „wysoki”, „niski”) oraz termin realizacji. Aplikacja obsługuje wyszukiwanie po słowach kluczowych, grupowanie według tagów lub kategorii, sortowanie według terminów lub priorytetów oraz generowanie raportów o zbliżających się terminach (np. „na najbliższy tydzień”) i podsumowań o wykonanych i zaległych zadaniach.




















using Avalonia; using Avalonia.Controls; using Avalonia.Input.TextInput; using Avalonia.Interactivity; using Avalonia.Markup.Xaml; using System; using System.Collections.Generic; using System.ComponentModel; using System.ComponentModel.DataAnnotations; using System.Linq;

namespace ZTP { public partial class MainWindow : Window { public MainWindow() { InitializeComponent(); GlobalGroups.Initialize();

 Task zadanie1 = new Task("Nauczyć się Avalonii", new DateTime(2025, 12, 15)); zadanie1.SetPriority(Priorities.Important); GlobalGroups.AllTasksGroup.Add(zadanie1);

 Task zadanie2 = new Task("Zrobić zakupy", new DateTime(2025, 11, 25)); GlobalGroups.AllTasksGroup.Add(zadanie2);

 Task zadanie3 = new Task("Napisać raport", new DateTime(2025, 12, 10)); zadanie3.SetPriority(Priorities.Normal); GlobalGroups.AllTasksGroup.Add(zadanie3);

 // Zadanie już wykonane Task zadanie4 = new Task("Umyć naczynia", new DateTime(2025, 11, 20)); zadanie4.MarkAsCompleted(new DateTime(2025, 11, 19)); // Wykonane przed terminem GlobalGroups.AllTasksGroup.Add(zadanie4);

 // Zadanie z opóźnieniem Task zadanie5 = new Task("Oddać książki do biblioteki", new DateTime(2025, 11, 15)); zadanie5.MarkAsCompleted(new DateTime(2025, 11, 18)); // Wykonane po terminie GlobalGroups.AllTasksGroup.Add(zadanie5);

 // Dodajemy też do AllGroup foreach (var task in GlobalGroups.AllTasksGroup.GetComponents()) { if (!GlobalGroups.AllGroup.Contains(task)) { GlobalGroups.AllGroup.Add(task); } }

 // Tworzymy listę zadań (TaskList) - Projekt Avalonia TaskList projektAvalonia = new TaskList("Projekt Avalonia"); projektAvalonia.Add(new Task("Stworzyć UI", new DateTime(2025, 12, 5))); projektAvalonia.Add(new Task("Zaimplementować logikę biznesową", new DateTime(2025, 12, 12))); projektAvalonia.Add(new Task("Przetestować aplikację", new DateTime(2025, 12, 18))); projektAvalonia.Add(new Task("Dokumentacja", new DateTime(2025, 12, 20))); projektAvalonia.SetPriority(Priorities.Important);

 GlobalGroups.AllTasksGroup.Add(projektAvalonia); GlobalGroups.AllGroup.Add(projektAvalonia);

 // Druga lista zadań - Codzienne obowiązki TaskList codzienneObowiazki = new TaskList("Codzienne obowiązki"); codzienneObowiazki.Add(new Task("Poranna kawa", new DateTime(2025, 11, 22))); codzienneObowiazki.Add(new Task("Spacer z psem", new DateTime(2025, 11, 22))); codzienneObowiazki.Add(new Task("Planowanie dnia", new DateTime(2025, 11, 22)));

 GlobalGroups.AllTasksGroup.Add(codzienneObowiazki); GlobalGroups.AllGroup.Add(codzienneObowiazki);

 // Trzecia lista zadań - Studia (zagnieżdżona struktura) TaskList przedmiot1 = new TaskList("Analiza Matematyczna"); przedmiot1.Add(new Task("Rozdział 1 - Granice", new DateTime(2025, 11, 28))); przedmiot1.Add(new Task("Rozdział 2 - Pochodne", new DateTime(2025, 12, 5)));

 TaskList przedmiot2 = new TaskList("Programowanie Obiektowe"); przedmiot2.Add(new Task("Wzorzec Singleton", new DateTime(2025, 11, 25))); przedmiot2.Add(new Task("Wzorzec Fabryka", new DateTime(2025, 12, 2))); przedmiot2.Add(new Task("Wzorzec Obserwator", new DateTime(2025, 12, 9)));

 TaskList studia = new TaskList("Zadania ze studiów"); studia.Add(przedmiot1); studia.Add(przedmiot2); studia.SetPriority(Priorities.Normal);

 GlobalGroups.AllTasksGroup.Add(studia); GlobalGroups.AllGroup.Add(studia);

 // Dodajemy notatki Note notatka1 = new Note("Pomysł na projekt", "Stworzyć aplikację do zarządzania zadaniami z użyciem wzorców projektowych."); Note notatka2 = new Note("Lista zakupów", "Mleko, Jajka, Chleb, Owoce, Warzywa, Kawa"); Note notatka3 = new Note("Spotkania w tym tygodniu", "Poniedziałek: Spotkanie zespołu 10:00\n" + "Wtorek: Prezentacja projektu 14:00\n" + "Czwartek: Konsultacje z klientem 11:30");

 GlobalGroups.AllNotesGroup.Add(notatka1); GlobalGroups.AllNotesGroup.Add(notatka2); GlobalGroups.AllNotesGroup.Add(notatka3);

 // Dodajemy również do AllGroup foreach (var note in GlobalGroups.AllNotesGroup.GetComponents()) { if (!GlobalGroups.AllGroup.Contains(note)) { GlobalGroups.AllGroup.Add(note); } }

 // Notatka techniczna Note notatka4 = new Note("Ważne linki", "Avalonia docs: https://docs.avaloniaui.net/\n" + "GitHub projektu: https://github.com/AvaloniaUI\n" + ".NET dokumentacja: https://learn.microsoft.com/dotnet/"); GlobalGroups.AllNotesGroup.Add(notatka4); GlobalGroups.AllGroup.Add(notatka4);

 // Notatka z cytatem Note notatka5 = new Note("Inspiracja", "„Perfekcjonizm to nie dążenie do doskonałości, " + "a strach przed popełnieniem błędu.” – Brene Brown"); GlobalGroups.AllNotesGroup.Add(notatka5); GlobalGroups.AllGroup.Add(notatka5);

 // Dodajemy też przykłady do kategorii Categories.Add("szkoła"); Categories.Add("dom"); Categories.Add("praca"); Categories.Add("hobby");

 // Przypisujemy elementy do kategorii Categories.AddToCategory(zadanie1, "szkoła"); Categories.AddToCategory(zadanie2, "dom"); Categories.AddToCategory(zadanie3, "praca"); Categories.AddToCategory(projektAvalonia, "szkoła"); Categories.AddToCategory(codzienneObowiazki, "dom"); Categories.AddToCategory(studia, "szkoła"); Categories.AddToCategory(notatka1, "praca"); Categories.AddToCategory(notatka2, "dom"); Categories.AddToCategory(notatka4, "szkoła"); Categories.AddToCategory(notatka5, "hobby");

 // Dodajemy też tagi Tags.Add("pilne"); Tags.Add("ważne"); Tags.Add("codzienne"); Tags.Add("studia"); Tags.Add("projekt");

 // Przypisujemy tagi do elementów Tags.AddToCategory(zadanie1, "pilne"); Tags.AddToCategory(zadanie1, "studia"); Tags.AddToCategory(projektAvalonia, "projekt"); Tags.AddToCategory(projektAvalonia, "ważne"); Tags.AddToCategory(codzienneObowiazki, "codzienne"); Tags.AddToCategory(zadanie4, "codzienne"); Tags.AddToCategory(notatka4, "ważne");

 Group group1 = new Group("Katalog"); group1.Add(zadanie1); group1.Add(studia); GlobalGroups.AllGroup.Add(group1);

 InitializeMenu();

 // foreach (var category in Categories.GetCategories()) // { // var categoryButton = category.DisplayGUI(); // categoryButton.Click += (s, e) => DisplayGroup(category); // categoriesContainer.Children.Add(categoryButton); // } }

 private void MenuButton_Click(object sender, RoutedEventArgs e) { if (sender is Button button) { string buttonName = button.Name;

 switch (buttonName) { case "BtnAll": DisplayGroup(GlobalGroups.AllGroup); break; case "BtnTasks": DisplayGroup(GlobalGroups.AllTasksGroup); break; case "BtnNotes": DisplayGroup(GlobalGroups.AllNotesGroup); break; default: ContentText.Text = "Nieznany przycisk"; break; } } }

 private void DisplayGroup(Group group) { Desktop.Content = group.SimpleDisplay(); } private void NewObject_Click(object sender, RoutedEventArgs e) { var button1 = new Button { Content = "Nowa notatka", Name = "BtnSelectNote" }; button1.Click += (s, e) => CreateNoteView();

 var button2 = new Button { Content = "Nowe zadanie", Name = "BtnSelectTask" }; button2.Click += (s, e) => CreateTaskView();

 var mainSection = new StackPanel{VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center, Orientation = Avalonia.Layout.Orientation.Horizontal, Spacing = 10}; mainSection.Children.Add(button1); mainSection.Children.Add(button2); Desktop.Content = mainSection; } // Dodaj pola dla kontrolek private TextBox? inputTitle; private TextBox? inputContent; private TextBox? inputTags; private ComboBox? inputCategory; private Button? saveEditingButton; private StackPanel? inputTasksSection; private List<TextBox>? taskTextBoxes;

 private void CreateNoteView() { var mainSection = new StackPanel{Orientation = Avalonia.Layout.Orientation.Vertical, Spacing = 10, Margin = new Thickness(20)};

 inputTitle = new TextBox{HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch, Text = Builder.DefaultName()};

 inputContent = new TextBox{HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch, MinHeight = 200, AcceptsReturn = true};

 var downSection = new Grid{ ColumnDefinitions = ColumnDefinitions.Parse("Auto, *") };

 inputCategory = GlobalGroups.SelectableCategoryList(); inputTags = new TextBox{Watermark="Wpisz tagi...", MaxWidth=200, Text=null}; saveEditingButton = new Button{Content = "Zapisz notatkę"}; saveEditingButton.Click += (s, e) => NoteBuilder();

 var leftSide = new StackPanel{ HorizontalAlignment=Avalonia.Layout.HorizontalAlignment.Left, Orientation=Avalonia.Layout.Orientation.Horizontal}; Grid.SetColumn(leftSide, 0); leftSide.Children.Add(inputCategory); leftSide.Children.Add(inputTags);

 var rightSide = new StackPanel{ HorizontalAlignment=Avalonia.Layout.HorizontalAlignment.Right}; Grid.SetColumn(rightSide, 1); rightSide.Children.Add(saveEditingButton);

 downSection.Children.Add(leftSide); downSection.Children.Add(rightSide);

 

 

 // inputTags = new ListBox{SelectionMode = SelectionMode.Multiple, // Ważne: wiele wyborów // ItemsSource = Tags.GetTags(), // DisplayMemberBinding = new Avalonia.Data.Binding("Name") };

 

 

 mainSection.Children.Add(inputTitle); mainSection.Children.Add(inputContent); mainSection.Children.Add(downSection);

 Desktop.Content = mainSection; }

 private void NoteBuilder() { string title = inputTitle.Text?.Trim() ?? "";

 if (string.IsNullOrWhiteSpace(title)) { inputTitle.Classes.Add("mustFill"); return; } inputTitle.Classes.Remove("mustFill");

 

 if (inputCategory.SelectedItem is string category) {Builder.SetCategory(category);} if (inputTags.Text is string tag) {Builder.SetTags(tag);}

 Builder.SetName(title); Builder.SetContent(inputContent.Text?.Trim() ?? ""); Builder.BuildNote();

 // Wyczyść pola inputTitle.Text = ""; inputContent.Text = ""; inputTags.Text = ""; inputCategory.SelectedItem = ""; DisplayGroup(GlobalGroups.AllGroup); }

 private void CreateTaskView() { var mainSection = new StackPanel{Orientation = Avalonia.Layout.Orientation.Vertical, Spacing = 10, Margin = new Thickness(20)}; 

 inputTitle = new TextBox{HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch, Watermark="Tytuł listy"};

 taskTextBoxes = new List<TextBox>(); inputTasksSection = new StackPanel{Spacing=5};

 

 var addTaskButtonSection = new StackPanel{HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center};

 var addTaskButton = new Button{Content = "Dodaj zadanie"}; addTaskButton.Click += (s, e) => AddTaskButtons(); addTaskButtonSection.Children.Add(addTaskButton); AddTaskButtons();

 var downSection = new Grid{ ColumnDefinitions = ColumnDefinitions.Parse("Auto, *") };

 inputCategory = GlobalGroups.SelectableCategoryList(); inputTags = new TextBox{Watermark="Wpisz tagi...", MaxWidth=200}; saveEditingButton = new Button{Content = "Zapisz notatkę"}; saveEditingButton.Click += (s, e) => TaskBuilder();

 var leftSide = new StackPanel{ HorizontalAlignment=Avalonia.Layout.HorizontalAlignment.Left, Orientation=Avalonia.Layout.Orientation.Horizontal}; Grid.SetColumn(leftSide, 0); leftSide.Children.Add(inputCategory); leftSide.Children.Add(inputTags);

 var rightSide = new StackPanel{ HorizontalAlignment=Avalonia.Layout.HorizontalAlignment.Right}; Grid.SetColumn(rightSide, 1); rightSide.Children.Add(saveEditingButton);

 downSection.Children.Add(leftSide); downSection.Children.Add(rightSide); mainSection.Children.Add(inputTitle); mainSection.Children.Add(inputTasksSection); mainSection.Children.Add(addTaskButtonSection); mainSection.Children.Add(downSection); Desktop.Content = mainSection; } private void AddTaskButtons() { var inputTask = new TextBox{HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch, Watermark="zadanie"}; taskTextBoxes.Add(inputTask); inputTasksSection.Children.Add(inputTask); } private void TaskBuilder() {

 foreach (var textBox in taskTextBoxes) { string text = textBox.Text?.Trim() ?? ""; if (!string.IsNullOrEmpty(text)) { Builder.AddTaskComponent(new Task(text)); } } Builder.BuildTask();

 // Wyczyść pola inputTitle.Text = ""; DisplayGroup(GlobalGroups.AllGroup); }

 private void Save_Click(object sender, RoutedEventArgs e) { Save(); // Builder.AddComponent } private void Save() { Desktop.Content = new TextBlock{VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center, Text = "Zapisywanie..."}; } private void Sych_Click(object sender, RoutedEventArgs e) { Save();

 var button = new Button { Content = "Zaloguj się", Name = "BtnLogIn" }; // button2.Classes.Add("menuButton"); NIE POTRZEBNY TU STYL TEN, MOZE BYĆ DOMYŚLNY button.Click += (s, e) => Save_Click(s, e);

 var mainSection = new StackPanel{VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center, Orientation = Avalonia.Layout.Orientation.Horizontal, Spacing = 10}; mainSection.Children.Add(button); Desktop.Content = mainSection; }

 // private void SaveCategoriesToCloud() // { // var categories = Categories.GetCategories(); // // Tutaj kod zapisu do chmury (np. do pliku JSON, bazy danych itp.) // // Przykład: Serializacja do JSON // var json = System.Text.Json.JsonSerializer.Serialize(categories); // // Zapisz gdzieś (plik, API, etc.) // }

 // private void LoadCategoriesFromCloud() // { // // Tutaj kod wczytywania z chmury // // Przykład: Deserializacja z JSON // // var json = ... // wczytaj z chmury // // var categories = System.Text.Json.JsonSerializer.Deserialize<List<Group>>(json); // } private void InitializeMenu() { var mainSectionTag = new StackPanel{}; foreach (var button in GlobalGroups.GetTags()) { button.Click += (s, e) => DisplayContaing(button.Name); mainSectionTag.Children.Add(button); } TagsExtender.Content = mainSectionTag;

 var mainSectionCategory = new StackPanel{}; foreach (var button in GlobalGroups.GetCategories()) { button.Click += (s, e) => DisplayContaing(button.Name); mainSectionCategory.Children.Add(button); } CategoriesExtender.Content = mainSectionCategory;

 } private void DisplayContaing(string sssss) { }

 private void SearchButton_Click(object sender, RoutedEventArgs e) { GlobalGroups.AddCategory(InputTextBox.Text); InitializeMenu(); } } }



namespace ZTP;

public static class Builder { private static List<IComponent> components = new List<IComponent>(); private static string currentName = ""; private static int counter = 1; private static string content = ""; private static Priorities prioritie = 0; private static DateTime endDate;

 private static string Category; private static string Tags;

 // private static DateTime EndTime = null; public static string GetName() { if (string.IsNullOrEmpty(currentName) && components.Count > 0) return components.First().Name; return currentName; } public static string DefaultName() { return $"New note {counter}"; }

 public static void SetName(string s) { currentName = s; }

 public static void SetContent(string s) { content = s; }

 public static void StartNew(string name = "") { Clear(); currentName = name ?? ""; }

 public static void AddTaskComponent(ITaskComponent component) { components.Add(component); }

 public static void BuildNote() { Note note = new Note(currentName, content); GlobalGroups.AllGroup.Add(note); GlobalGroups.AllNotesGroup.Add(note); if(currentName == $"New note {counter}") { counter++; } if (Category != null) { note.SetCategory(Category); } if(Tags != null) { GlobalGroups.AddTags(Tags); var tags = Tags.Split(','); foreach (var t in tags) { string tag = t?.Trim().ToLower() ?? ""; if (!string.IsNullOrWhiteSpace(tag)) { note.SetTags(tag); } } } } 

 public static IComponent BuildTask() { if (components.Count == 0) return null;

 if (components.Count == 1) { var result = components.First();

 Clear(); GlobalGroups.AllNotesGroup.Add(result); GlobalGroups.AllGroup.Add(result); return result; }

 string name = string.IsNullOrEmpty(currentName) ? components.First().Name : currentName; var taskList = new TaskList(name);

 foreach (var component in components) { if (component is Task task) taskList.Add(new Task(task)); else if (component is TaskList tl) taskList.Add(new TaskList(tl)); } Clear(); GlobalGroups.AllNotesGroup.Add(taskList); GlobalGroups.AllGroup.Add(taskList); return taskList; } public static void SetCategory(string selectedCategory) { Category=selectedCategory; } public static void SetTags(string selectedTags) { Tags=selectedTags; } public static void Clear() { components.Clear(); currentName = ""; } }
namespace ZTP;

public enum Priorities { None, Low, Normal, Important }

public interface IComponent { public string Name { get; } DateTime StartDate { get; } // public List<string> Tags { get; } // public string Category { get; } public string Display(int depth); public string Display(); public StackPanel SimpleDisplay(int depth); public StackPanel SimpleDisplay(); }

public class Note : IComponent { public string Name { get; } public string Content { get; } public DateTime StartDate { get; } public List<string> Tags { get; set;} = new List<string>(); public string Category { get; set;}

 public Note(string name, string content) { Name = name; Content = content; StartDate = DateTime.Now; }

 public Note(Note other) { Name = other.Name; Content = other.Content; StartDate = other.StartDate; }

 public void SetCategory(string category) { Category=category; } public void SetTags(List<string> tags) { Tags=tags; } public void SetTags(string tag) { Tags.Add(tag); }

 public string Display() { return this.Display(1); }

 public string Display(int depth) { string indent = new string(' ', depth); string dashPrefix = new string('-', depth);

 StringBuilder sb = new StringBuilder(); sb.AppendLine($"{dashPrefix}{Name} ({StartDate:dd.MM.yyyy})"); sb.AppendLine($"{indent}Treść: {Content}");

 return sb.ToString(); }

 public StackPanel SimpleDisplay(int depth) { var mainSection = new StackPanel{ Margin = new Thickness(10*depth, 5)};

 // Tytuł notatki jako TextBox var titleBox = new TextBox{Text = $"📝 {Name}, {Category}, {string.Join( ",", Tags.ToArray() )}", FontSize = 14, FontWeight = FontWeight.SemiBold, Margin = new Thickness(0, 0, 0, 5), IsReadOnly = true, BorderThickness = new Thickness(0), Background = Brushes.Transparent}; mainSection.Children.Add(titleBox);

 // Treść notatki if (!string.IsNullOrEmpty(Content)) { var contentBox = new TextBox { Text = Content, IsReadOnly = true, TextWrapping = TextWrapping.Wrap, BorderThickness = new Thickness(0), Background = Brushes.Transparent, Margin = new Thickness(10, 0, 0, 0) }; mainSection.Children.Add(contentBox); }

 // Data utworzenia var dateBox = new TextBlock { Text = $"Utworzono: {StartDate:dd.MM.yyyy HH:mm}", FontSize = 11, Foreground = Brushes.Gray, Background = Brushes.Transparent, Margin = new Thickness(10, 0, 0, 0) }; mainSection.Children.Add(dateBox);

 return mainSection; } public StackPanel SimpleDisplay() { return SimpleDisplay(1); } public StackPanel DisplayDetails() { var mainSection = new StackPanel{Orientation = Avalonia.Layout.Orientation.Vertical, Spacing = 10, Margin = new Thickness(20)};

 var inputTitle = new TextBox{HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch, Text = Name, AcceptsReturn = true};

 var inputContent = new TextBox{HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch, MinHeight = 300, Text = Content, AcceptsReturn = true}; var dateBox = new TextBlock{Text = $"Utworzono: {StartDate:dd.MM.yyyy HH:mm}", FontSize = 11, Foreground = Brushes.Gray, Background = Brushes.Transparent};

 var saveButton = new Button{Content = "Zapisz notatkę", HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right, Width = 120, Margin = new Thickness(0, 10, 0, 0)}; // saveButton.Click += (s, e) => NoteBuilder(); mainSection.Children.Add(inputTitle); mainSection.Children.Add(inputContent); mainSection.Children.Add(dateBox); mainSection.Children.Add(saveButton); return mainSection; } }

public interface ITaskComponent : IComponent { DateTime? EndDate { get; } bool IsCompleted { get; } bool IsLate { get; } Priorities Priority { get; } public void MarkAsCompleted(DateTime completionDate); public string GetStatus(); void SetPriority(Priorities priority); }

public class Task : ITaskComponent { public string Name { get; } public DateTime StartDate { get; } public DateTime? EndDate { get; } public Priorities Priority { get; private set; } = 0; public bool IsCompleted { get; private set; } = false; public bool IsLate { get; private set; } = false;

 public Task(string name, DateTime endDate) { Name = name; StartDate = DateTime.Now; EndDate = endDate; } public Task(string name) { Name = name; StartDate = DateTime.Now; EndDate = null; }

 public Task(Task other) { Name = other.Name; StartDate = other.StartDate; EndDate = other.EndDate; Priority = other.Priority; IsCompleted = other.IsCompleted; IsLate = other.IsLate; }

 public void MarkAsCompleted(DateTime completionDate) { IsCompleted = true; IsLate = completionDate > EndDate; }

 public string GetStatus() { if (IsCompleted) return IsLate ? "[Completed Late]" : "[Completed]"; return "[Pending]"; }

 public void SetPriority(Priorities priority) { Priority = priority; }

 public string Display() { return this.Display(1); }

 public string Display(int depth) { string dashPrefix = new string('-', depth); return $"{dashPrefix}{Name} ({StartDate:dd.MM.yyyy} to {EndDate:dd.MM.yyyy}) - Status: {GetStatus()}\n"; }

 public StackPanel SimpleDisplay(int depth) { var mainSection = new StackPanel { Spacing = 10, Margin = new Thickness(10*depth, 5) };

 var grid = new Grid(); grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto)); // Checkbox grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star)); // Nazwa grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto)); // Data grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto)); // Priorytet

 // Checkbox dla statusu var checkBox = new CheckBox { IsChecked = IsCompleted, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center, Margin = new Thickness(0, 0, 10, 0) }; checkBox.IsCheckedChanged += (s, e) => { if (checkBox.IsChecked == true) MarkAsCompleted(DateTime.Now); }; Grid.SetColumn(checkBox, 0);

 // Nazwa zadania var nameText = new TextBlock { Text = Name, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center, FontWeight = IsCompleted ? FontWeight.Normal : FontWeight.Bold, TextDecorations = IsCompleted ? TextDecorations.Strikethrough : null }; Grid.SetColumn(nameText, 1);

 // Data var dateText = new TextBlock { Text = $"({EndDate:dd.MM.yyyy})", VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center, FontSize = 12, Foreground = Brushes.Gray }; Grid.SetColumn(dateText, 2);

 // Priorytet var priorityIcon = new TextBlock { Text = GetPriorityIcon(), VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center, FontSize = 14, Margin = new Thickness(10, 0, 0, 0) }; Grid.SetColumn(priorityIcon, 3);

 grid.Children.Add(checkBox); grid.Children.Add(nameText); grid.Children.Add(dateText); grid.Children.Add(priorityIcon);

 mainSection.Children.Add(grid); return mainSection; } public StackPanel SimpleDisplay() { return SimpleDisplay(1); }

 private string GetPriorityIcon() { return Priority switch { Priorities.Important => "⚠️", Priorities.Normal => "🔵", Priorities.Low => "⚪", _ => "" }; } }

public class TaskList : ITaskComponent { public string Name { get; } private List<ITaskComponent> components = new List<ITaskComponent>();

 public DateTime StartDate { get { if (components.Count == 0) return DateTime.MinValue; return components.Min(component => component.StartDate); } }

 public DateTime? EndDate { get { if (components.Count == 0) return DateTime.MaxValue; return components.Max(component => component.EndDate); } }

 public bool IsCompleted { get { return components.Count > 0 && components.All(component => component.IsCompleted); } }

 public bool IsLate { get { return components.Count > 0 && components.Any(component => component.IsLate); } }

 public Priorities Priority { get; private set; } = 0;

 public TaskList(string name) { Name = name; }

 public TaskList(string name, List<ITaskComponent> list) { Name = name; components = list; }

 public TaskList(TaskList other) { Name = other.Name; components = other.components; }

 public void Add(ITaskComponent component) { components.Add(component); }

 public void Remove(ITaskComponent component) { components.Remove(component); }

 public void SetPriority(Priorities priority) { Priority = priority; }

 public void MarkAsCompleted(DateTime completionDate) { foreach (var component in components) { component.MarkAsCompleted(completionDate); } }

 public string GetStatus() { if (IsCompleted) return IsLate ? "[Completed Late]" : "[Completed]"; return "[Pending]"; }

 public string Display() { return this.Display(1); }

 public string Display(int depth) { StringBuilder sb = new StringBuilder(); string dashPrefix = new string('-', depth);

 sb.AppendLine($"{dashPrefix}{Name} ({StartDate:dd.MM.yyyy} to {EndDate:dd.MM.yyyy}) - Status: {GetStatus()}");

 foreach (var component in components) { sb.Append(component.Display(depth + 2)); }

 return sb.ToString(); }

 private int[] getStatistics() { int[] statistics = new int[4] { components.OfType<Task>().Count(t => t.IsCompleted && !t.IsLate), components.OfType<Task>().Count(t => t.IsCompleted && t.IsLate), components.OfType<Task>().Count(t => !t.IsCompleted), components.OfType<Task>().Count(t => !t.IsCompleted && DateTime.Now > t.EndDate) };

 foreach (TaskList group in components.OfType<TaskList>()) { int[] groupReport = group.getStatistics(); for (int i = 0; i < statistics.Length; i++) { statistics[i] += groupReport[i]; } }

 return statistics; }

 public string Report() { int[] stat = this.getStatistics(); StringBuilder sb = new StringBuilder(); sb.AppendLine("\nPodsumowanie zadań:"); sb.AppendLine($"Zadania wykonane na czas: {stat[0]}"); sb.AppendLine($"Zadania wykonane z opóźnieniem: {stat[1]}"); sb.AppendLine($"Zadania oczekujące: {stat[2]}"); sb.AppendLine($"Zadania oczekujące z przekroczonym terminem: {stat[3]}");

 return sb.ToString(); }

 public StackPanel SimpleDisplay(int depth) { var mainSection = new StackPanel { Spacing = 10, Margin = new Thickness(10*depth, 5) };

 // Tytuł listy zadań var titleText = new TextBlock { Text = $"📋 {Name}", FontSize = 14, FontWeight = FontWeight.SemiBold, Margin = new Thickness(0, 0, 0, 5) };

 mainSection.Children.Add(titleText);

 // Status i informacje var infoText = new TextBlock { Text = $"Status: {GetStatus()} | Termin: {StartDate:dd.MM.yyyy} - {EndDate:dd.MM.yyyy}", FontSize = 12, Foreground = Brushes.Gray, Margin = new Thickness(10, 0, 0, 10) }; mainSection.Children.Add(infoText);

 // Zadania w liście foreach(var c in components) { mainSection.Children.Add(c.SimpleDisplay(depth+1)); } return mainSection; } public StackPanel SimpleDisplay() { return SimpleDisplay(1); } }

public class Group : IComponent { public string Name { get; } private List<IComponent> components = new List<IComponent>(); public DateTime StartDate { get { if (components.Count == 0) return DateTime.MinValue; return components.Min(component => component.StartDate); } } public Group(string name) { Name = name; }

 public void Add(IComponent component) { components.Add(component); }

 public void Remove(IComponent component) { components.Remove(component); }

 public bool Contains(IComponent component) { return components.Contains(component); }

 public int Count() { return components.Count(); }

 public IReadOnlyList<IComponent> GetComponents() { return components.AsReadOnly(); }

 public string Display() { return this.Display(1); }

 public string Display(int depth) { StringBuilder sb = new StringBuilder();

 foreach (var component in components) { sb.Append(component.Display(depth + 2)); }

 return sb.ToString(); }

 public string GetFormattedList() { StringBuilder sb = new StringBuilder(); sb.AppendLine($"{Name}:"); sb.AppendLine();

 int i = 1; foreach (var component in components) { sb.AppendLine($"{i}. {component.Name}"); i++; }

 return sb.ToString(); }

 public string GetDetailedList() { StringBuilder sb = new StringBuilder(); sb.AppendLine($"{Name}:"); sb.AppendLine();

 int i = 1; foreach (var component in components) { sb.AppendLine($"{i}. {component.Display()}"); i++; }

 return sb.ToString(); }

 public StackPanel SimpleDisplay(int depth) {

 var mainSection = new StackPanel{Margin = new Thickness(10*depth, 5)};

 // Tytuł grupy var titleText = new TextBlock{Text = $"📂 {Name}", FontSize = 14, FontWeight = FontWeight.SemiBold}; mainSection.Children.Add(titleText);

 // Licznik elementów var counterText = new TextBlock{Text = $"({Count()} elementów)", FontSize = 12, Foreground = Brushes.Gray, Margin = new Thickness(10, 0, 0, 10)}; mainSection.Children.Add(counterText);

 // Elementy grupy foreach(var c in components) { mainSection.Children.Add(c.SimpleDisplay(depth+1)); } return mainSection; } public StackPanel SimpleDisplay() { return SimpleDisplay(1); }

 public Button DisplayGUI() { // Tytuł notatki jako TextBox var b = new Button{Content=Name, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch, Background = Brushes.Transparent}; return b; }

}
namespace ZTP;

public static class GlobalGroups { public static Group AllGroup = new Group("Wszystko"); public static Group AllTasksGroup = new Group("Zadania"); public static Group AllNotesGroup = new Group("Notatki"); private static List<string> AllCategories { get; } = new List<string>(); private static List<string> AllTags = new List<string>();

 public static void Initialize() { // Dodaj domyślne kategorie Categories.Add("szkoła"); Categories.Add("dom");

 // Opcjonalnie: dodaj też tagi AddTags("pilne"); AddTags("ważne"); AddTags("codzienne");

 AddCategory("szkola"); AddCategory("dom"); AddCategory("dzieci"); } // public static StackPanel adsa(string contains) // { // var mainSection = new StackPanel{}; // foreach(var) // return mainSection; // } public static void AddTags(List<string> tags) { foreach (var t in tags) { string tag = t?.Trim().ToLower() ?? ""; if (!string.IsNullOrWhiteSpace(tag) && !AllTags.Contains(tag)) { AllTags.Add(tag); }

 } } public static void AddTags(string tag) { if (tag.Contains(',')) { List<string> tags = new List<string>(tag.Split(',')); AddTags(tags);

 } else { string t = tag?.Trim().ToLower() ?? ""; if (!string.IsNullOrWhiteSpace(t) && !AllTags.Contains(t)) { AllTags.Add(t); } } } public static List<Button> GetTags() { var buttons = new List<Button>(); foreach(var tag in AllTags) { var tagButton = new Button{Content = $"#{tag}", Name=tag}; tagButton.Classes.Add("leftMenuButton"); buttons.Add(tagButton); } return buttons; }

 public static void AddCategory(List<string> categories) { foreach (var c in categories) { string category = c?.Trim().ToLower() ?? ""; if (!string.IsNullOrWhiteSpace(category) && !AllCategories.Contains(category)) { AllCategories.Add(category); }

 } } public static void AddCategory(string category) { if(category==null){return;}; List<string> categories = new List<string>(category.Split(',')); AddCategory(categories); } public static List<Button> GetCategories() { var buttons = new List<Button>(); foreach(var category in AllCategories) { var categoryButton = new Button{Content = $"{category}", Name=category}; categoryButton.Classes.Add("leftMenuButton"); buttons.Add(categoryButton); } return buttons; } public static ComboBox SelectableCategoryList() { return new ComboBox{ItemsSource = AllCategories }; } }
po namespace każyd z kodów zanjduje się w osobny pliku
jakie są tutaj wzorce i jakie wzorce można by dodać