using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ZTP
{
    public partial class MainWindow : Window
    {
        BuilderNote noteBuilder;
        BuilderTask taskBuilder;
        public static MainWindow Instance { get; private set; }
        
        // Kontrolki dla tworzenia notatek/zadań
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
            GlobalGroups.Initialize();
            noteBuilder = new BuilderNote();
            taskBuilder = new BuilderTask();
            InitializeMenu();
            UIManager.InitializeMainWindow(this);
        }

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

        public void EditDisplay(object o)
        {
            if (o is Note note)
            {
                Desktop.Content = note.DisplayDetails();
            }
        }

        private void NewObject_Click(object sender, RoutedEventArgs e)
        {
            UIManager.DisplayNewObjectSelection();
        }
        
        public void CreateNoteView()
        {
            inputCategory = GlobalGroups.SelectableCategoryList();
            inputTags = new TextBox { Watermark = "Wpisz tagi...", MaxWidth = 200 };
            
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

        private void NoteBuilder()
        {
            var title = inputTitle?.Text?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(title))
            {
                UIManager.ShowValidationError(inputTitle, true);
                return;
            }
            
            UIManager.ShowValidationError(inputTitle, false);
            
            if (inputCategory?.SelectedItem is string category)
                noteBuilder.SetCategory(category);
            
            if (!string.IsNullOrWhiteSpace(inputTags?.Text))
                noteBuilder.SetTags(inputTags.Text);
            
            noteBuilder.SetName(title)
                      .SetContent(inputContent?.Text?.Trim() ?? "")
                      .Build();
            
            // Wyczyść i pokaż wszystkie
            inputTitle = null;
            inputContent = null;
            inputTags = null;
            inputCategory = null;
            
            UIManager.DisplayGroup(GlobalGroups.AllGroup);
        }

        public void CreateTaskView()
        {
            taskTextBoxesList = new List<TextBox>();
            inputTasksSection = new StackPanel { Spacing = 5 };
            inputCategory = GlobalGroups.SelectableCategoryList();
            inputTags = new TextBox { Watermark = "Wpisz tagi...", MaxWidth = 200 };
            inputPriority = new ComboBox { ItemsSource = Enum.GetValues<Priorities>() };
            
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

        private void TaskBuilder()
        {
            // Sprawdź poprawność dat
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
            
            if (inputPriority?.SelectedItem is Priorities priority)
                taskBuilder.SetPriority(priority);
            
            if (taskTextBoxesList != null && taskEndDateList != null)
            {
                taskBuilder.AddTaskComponent(taskTextBoxesList, taskEndDateList)
                          .SetCategory(inputCategory?.SelectedItem?.ToString())
                          .SetTags(inputTags?.Text?.Trim())
                          .SetName(inputTitle?.Text?.Trim() ?? "")
                          .Build();
            }
            
            // Wyczyść
            taskTextBoxesList?.Clear();
            taskEndDateList?.Clear();
            inputTasksSection?.Children.Clear();
            inputTitle = null;
            inputTags = null;
            inputCategory = null;
            inputPriority = null;
            
            UIManager.DisplayGroup(GlobalGroups.AllGroup);
        }
        
        private void InitializeMenu()
        {
            // Tagi
            var tagsSection = UIManager.CreateMenuSection(
                GlobalGroups.GetTags(),
                UIManager.DisplayByTagOrCategory
            );
            TagsExtender.Content = tagsSection;
            
            // Kategorie
            var categoriesSection = UIManager.CreateMenuSection(
                GlobalGroups.GetCategories(),
                UIManager.DisplayByTagOrCategory
            );
            CategoriesExtender.Content = categoriesSection;
            
            // Statystyki
            var statsButton = new Button { 
                Content = "Podsumowanie", 
                Classes = { "leftMenuButton" } 
            };
            statsButton.Click += (s, e) => UIManager.DisplayStatistics();
            ButtonSection.Children.Add(statsButton);
            
            // Raporty
            var reportButton = new Button {
                Content = "Nadchodzące terminy", 
                Classes = { "leftMenuButton" } 
            };
            reportButton.Click += (s, e) => UIManager.DisplayUpcomingTasks();
            ButtonSection.Children.Add(reportButton);
            
            // Wyszukiwanie
            searchButton.Click += (s, e) => 
                UIManager.DisplaySearchResults(searchBox.Text);
        }    
    }
}