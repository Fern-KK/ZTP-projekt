using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ZTP
{
    // Główne okno aplikacji
    public partial class MainWindow : Window
    {
        // Instancje builderów
        BuilderNote noteBuilder;
        BuilderTask taskBuilder;

        // Statyczna instancja okna dla łatwego dostępu z innych części programu
        public static MainWindow Instance { get; private set; }
        
        // Pola przechowujące referencje do kontrolek formularzy tworzenia obiektów
        //https://en.wikipedia.org/wiki/VeggieTales
        private TextBox? inputTitle;
        private TextBox? inputContent;
        private TextBox? inputTags;
        private ComboBox? inputCategory;
        private ComboBox? inputPriority;
        private StackPanel? inputTasksSection;
        private List<TextBox>? taskTextBoxesList;
        private List<DatePicker>? taskEndDateList = new List<DatePicker>();

        public MainWindow()
        {
            InitializeComponent();
            Instance = this;

            // Inicjalizacja danych globalnych i budowniczych
            GlobalGroups.Initialize();
            noteBuilder = new BuilderNote();
            taskBuilder = new BuilderTask();

            // Konfiguracja interfejsu użytkownika
            InitializeMenu();
            UIManager.InitializeMainWindow(this);
        }

        // Obsługa kliknięć przycisków w menu bocznym (Główne filtry)
        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                switch (button.Name)
                {
                    case "BtnAll":
                        UIManager.DisplayGroup(GlobalGroups.AllGroup);
                        break;
                    case "BtnTasks":
                        UIManager.DisplayGroup(GlobalGroups.AllTasksGroup);
                        break;
                    case "BtnNotes":
                        UIManager.DisplayGroup(GlobalGroups.AllNotesGroup);
                        break;
                }
            }
        }

        // Przełącza widok główny na tryb edycji konkretnego obiektu (np. notatki)
        public void EditDisplay(object o)
        {
            if (o is Note note)
            {
                Desktop.Content = note.DisplayDetails();
            }
        }

        // Wyświetla menu wyboru typu nowego obiektu (Notatka/Zadanie)
        private void NewObject_Click(object sender, RoutedEventArgs e)
        {
            UIManager.DisplayNewObjectSelection();
        }
        
        // Przygotowuje i wyświetla formularz tworzenia nowej notatki
        public void CreateNoteView()
        {
            inputCategory = GlobalGroups.SelectableCategoryList();
            inputCategory.PlaceholderText = "Kategoria";
            inputTags = new TextBox { Watermark = "Wpisz tagi...", MaxWidth = 200 };
            
            // Wywołanie managera UI do wygenerowania layoutu edytora
            var editor = UIManager.CreateNoteEditor(
                noteBuilder.DefaultName(),
                out inputTitle,
                out inputContent,
                inputCategory,
                inputTags,
                NoteBuilder
            );
            
            Desktop.Content = editor;
        }

        // Metoda kończąca proces budowania notatki - pobiera dane z UI i tworzy obiekt
        private void NoteBuilder()
        {
            var title = inputTitle?.Text?.Trim() ?? "";

            // Walidacja tytułu
            if (string.IsNullOrWhiteSpace(title))
            {
                UIManager.ShowValidationError(inputTitle, true);
                return;
            }
            
            UIManager.ShowValidationError(inputTitle, false);
            
            // Ustawianie parametrów w budowniczym
            if (inputCategory?.SelectedItem is string category)
                noteBuilder.SetCategory(category);
            
            if (!string.IsNullOrWhiteSpace(inputTags?.Text))
                noteBuilder.SetTags(inputTags.Text);
            
            noteBuilder.SetName(title)
                      .SetContent(inputContent?.Text?.Trim() ?? "")
                      .Build();
            
            // Czyszczenie referencji i powrót do widoku głównego
            inputTitle = null;
            inputContent = null;
            inputTags = null;
            inputCategory = null;
            
            UIManager.DisplayGroup(GlobalGroups.AllGroup);
        }

        // Przygotowuje i wyświetla formularz tworzenia nowej listy zadań
        public void CreateTaskView()
        {
            taskTextBoxesList = new List<TextBox>();
            inputTasksSection = new StackPanel { Spacing = 5 };
            inputCategory = GlobalGroups.SelectableCategoryList();
            inputCategory.PlaceholderText = "Kategoria";
            inputTags = new TextBox { Watermark = "Wpisz tagi...", MaxWidth = 200 };
            inputPriority = new ComboBox { ItemsSource = Enum.GetValues<Priorities>(), PlaceholderText = "None"};
            
            // Dodanie pierwszego wiersza zadania na start
            AddTaskButtons();
            
            var editor = UIManager.CreateTaskEditor(
                taskBuilder.DefaultName(),
                out inputTitle,
                inputTasksSection,
                AddTaskButtons,
                inputCategory,
                inputTags,
                inputPriority,
                TaskBuilder
            );
            
            Desktop.Content = editor;
        }
        
        // Dodaje nowy wiersz (TextBox i DatePicker) do sekcji tworzenia zadań
        private void AddTaskButtons()
        {
            var row = UIManager.CreateTaskInputRow(
                out var taskTextBox,
                out var datePicker
            );
            
            taskTextBoxesList?.Add(taskTextBox);
            taskEndDateList?.Add(datePicker);
            inputTasksSection?.Children.Add(row);
        }

        // Metoda kończąca proces budowania zadania - waliduje daty i tworzy obiekty.
        private void TaskBuilder()
        {
            // Sprawdź poprawność dat (czy nie są z przeszłości)
            bool hasInvalidDate = false;
            if (taskEndDateList != null)
            {
                for (int i = 0; i < taskEndDateList.Count; i++)
                {
                    var date = taskEndDateList[i].SelectedDate;
                    bool isInvalid = date.HasValue && date.Value.Date < DateTime.Today;
                    UIManager.ShowValidationError(taskEndDateList[i], isInvalid);
                    hasInvalidDate |= isInvalid;
                }
            }
            
            if (hasInvalidDate)
                return;
            
            // Ustawienie priorytetu
            if (inputPriority?.SelectedItem is Priorities priority)
                taskBuilder.SetPriority(priority);
            
            // Przekazanie list kontrolek do budowniczego w celu ekstrakcji danych
            if (taskTextBoxesList != null && taskEndDateList != null)
            {
                taskBuilder.AddTaskComponent(taskTextBoxesList, taskEndDateList)
                          .SetCategory(inputCategory?.SelectedItem?.ToString())
                          .SetTags(inputTags?.Text?.Trim())
                          .SetName(inputTitle?.Text?.Trim() ?? "")
                          .Build();
            }
            
            // Czyszczenie formularza
            taskTextBoxesList?.Clear();
            taskEndDateList?.Clear();
            inputTasksSection?.Children.Clear();
            inputTitle = null;
            inputTags = null;
            inputCategory = null;
            inputPriority = null;
            
            UIManager.DisplayGroup(GlobalGroups.AllGroup);
        }
        
        // Konfiguruje menu boczne: tagi, kategorie, statystyki i wyszukiwarkę.
        private void InitializeMenu()
        {
            // Sekcja dynamicznych tagów
            var tagsSection = UIManager.CreateMenuSection(
                GlobalGroups.GetTags(),
                UIManager.DisplayByTagOrCategory
            );
            TagsExtender.Content = tagsSection;
            
            // Sekcja dynamicznych kategorii
            var categoriesSection = UIManager.CreateMenuSection(
                GlobalGroups.GetCategories(),
                UIManager.DisplayByTagOrCategory
            );
            CategoriesExtender.Content = categoriesSection;
            
            // Przycisk statystyk (wykorzystuje Visitora wewnątrz UIManager)
            var statsButton = new Button { 
                Content = "Podsumowanie", 
                Classes = { "leftMenuButton" } 
            };
            statsButton.Click += (s, e) => UIManager.DisplayStatistics();
            ButtonSection.Children.Add(statsButton);
            
            // Przycisk raportu nadchodzących terminów
            var reportButton = new Button {
                Content = "Nadchodzące terminy", 
                Classes = { "leftMenuButton" } 
            };
            reportButton.Click += (s, e) => UIManager.DisplayUpcomingTasks();
            ButtonSection.Children.Add(reportButton);
            
            // Obsługa pola wyszukiwania
            searchButton.Click += (s, e) => 
                UIManager.DisplaySearchResults(searchBox.Text);
        }    
    }
}
