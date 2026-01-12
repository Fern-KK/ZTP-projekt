using Avalonia;
using Avalonia.Controls;
using Avalonia.Input.TextInput;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;


namespace ZTP
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            GlobalGroups.Initialize();
            InitializeMenu();

        }


        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                string buttonName = button.Name;

                switch (buttonName)
                {
                    case "BtnAll":
                        DisplayGroup(GlobalGroups.AllGroup);
                        break;
                    case "BtnTasks":
                        DisplayGroup(GlobalGroups.AllTasksGroup);
                        break;
                    case "BtnNotes":
                        DisplayGroup(GlobalGroups.AllNotesGroup);
                        break;
                    default:
                        break;
                }
            }
        }

        private void DisplayGroup(Group group)
        {
            Desktop.Content = group.SimpleDisplay();
        }
        private void NewObject_Click(object sender, RoutedEventArgs e)
        {
            var button1 = new Button { Content = "Nowa notatka", Name = "BtnSelectNote" };
            button1.Click += (s, e) => CreateNoteView();

            var button2 = new Button { Content = "Nowe zadanie", Name = "BtnSelectTask" };
            button2.Click += (s, e) => CreateTaskView();

            var mainSection = new StackPanel{VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                                       HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                                       Orientation = Avalonia.Layout.Orientation.Horizontal,
                                       Spacing = 10};
            mainSection.Children.Add(button1);
            mainSection.Children.Add(button2);
            Desktop.Content = mainSection;
        }
        
        // Dodaj pola dla kontrolek
        private TextBox? inputTitle;
        private TextBox? inputContent;
        private TextBox? inputTags;
        private ComboBox? inputCategory;
        private ComboBox? inputPriority;
        private Button? saveEditingButton;
        private StackPanel? inputTasksSection;
        private List<TextBox>? taskTextBoxesList;
        private List<DatePicker>? taskEndDateList = new List<DatePicker>();
        

        private void CreateNoteView()
        {
            var mainSection = new StackPanel{Orientation = Avalonia.Layout.Orientation.Vertical,
                                       Spacing = 10,
                                       Margin = new Thickness(20)};

            inputTitle = new TextBox{HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                                     Text = Builder.DefaultName()};

            inputContent = new TextBox{HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                                        MinHeight = 200,
                                        AcceptsReturn = true};

            var downSection = new Grid{ ColumnDefinitions = ColumnDefinitions.Parse("Auto, *") }; 

            inputCategory = GlobalGroups.SelectableCategoryList();
            inputTags = new TextBox{Watermark="Wpisz tagi...", MaxWidth=200, Text=null};
            
            
            saveEditingButton = new Button{Content = "Zapisz notatkę"};
            saveEditingButton.Click += (s, e) =>{   string title = inputTitle.Text?.Trim() ?? "";
                                                    if (string.IsNullOrWhiteSpace(title))
                                                    { 
                                                        inputTitle.Classes.Add("mustFill");
                                                        return; 
                                                    }
                                                    NoteBuilder();
                                                };

            var leftSide = new StackPanel{ HorizontalAlignment=Avalonia.Layout.HorizontalAlignment.Left, 
                                           Orientation=Avalonia.Layout.Orientation.Horizontal,
                                           Spacing=5};
            Grid.SetColumn(leftSide, 0);
            leftSide.Children.Add(inputCategory);
            leftSide.Children.Add(inputTags);

            var rightSide = new StackPanel{ HorizontalAlignment=Avalonia.Layout.HorizontalAlignment.Right};
            Grid.SetColumn(rightSide, 1);
            rightSide.Children.Add(saveEditingButton);

            downSection.Children.Add(leftSide);
            downSection.Children.Add(rightSide);



            
            

            mainSection.Children.Add(inputTitle);
            mainSection.Children.Add(inputContent);
        
            mainSection.Children.Add(downSection);


            Desktop.Content = mainSection;
        }

        private void NoteBuilder()
        {
            inputTitle.Classes.Remove("mustFill");
            if (inputCategory.SelectedItem is string category) {Builder.SetCategory(category);}
            if (inputTags.Text is string tag) {Builder.SetTags(tag);}

            Builder.SetName(inputTitle.Text);
            Builder.SetContent(inputContent.Text?.Trim() ?? "");
            Builder.BuildNote();


            // Wyczyść pola
            inputTitle.Text = "";
            inputContent.Text = "";
            inputTags.Text = "";
            inputCategory.SelectedItem = "";
            DisplayGroup(GlobalGroups.AllGroup);
        }





        private void CreateTaskView()
        {
            var mainSection = new StackPanel{Orientation = Avalonia.Layout.Orientation.Vertical,
                                       Spacing = 10,
                                       Margin = new Thickness(20)};
            

            inputTitle = new TextBox{HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch, Watermark="Tytuł listy"};

            taskTextBoxesList = new List<TextBox>();
            inputTasksSection = new StackPanel{Spacing=5};

            

            
            var addTaskButtonSection = new StackPanel{HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center};

            var addTaskButton = new Button{Content = "Dodaj zadanie"};
            addTaskButton.Click += (s, e) => AddTaskButtons();
            addTaskButtonSection.Children.Add(addTaskButton);

            AddTaskButtons();

            var downSection = new Grid{ ColumnDefinitions = ColumnDefinitions.Parse("Auto, *") }; 

            inputCategory = GlobalGroups.SelectableCategoryList();
            inputTags = new TextBox{Watermark="Wpisz tagi...", MaxWidth=200};
            inputPriority = new ComboBox{ItemsSource = Enum.GetValues<Priorities>()};
            
            var leftSide = new StackPanel{ HorizontalAlignment=Avalonia.Layout.HorizontalAlignment.Left, 
                                           Orientation=Avalonia.Layout.Orientation.Horizontal,
                                           Spacing=5};
            Grid.SetColumn(leftSide, 0);
            leftSide.Children.Add(inputCategory);
            leftSide.Children.Add(inputTags);
            leftSide.Children.Add(inputPriority);



            saveEditingButton = new Button{Content = "Zapisz notatkę"};
            saveEditingButton.Click += (s, e) => TaskBuilder();

            var rightSide = new StackPanel{ HorizontalAlignment=Avalonia.Layout.HorizontalAlignment.Right};
            Grid.SetColumn(rightSide, 1);
            rightSide.Children.Add(saveEditingButton);

            downSection.Children.Add(leftSide);
            downSection.Children.Add(rightSide);
            mainSection.Children.Add(inputTitle);
            mainSection.Children.Add(inputTasksSection);
            mainSection.Children.Add(addTaskButtonSection);
            mainSection.Children.Add(downSection);
            Desktop.Content = mainSection;
        }
        private void AddTaskButtons()
        {
            var mainSection = new StackPanel{Orientation=Avalonia.Layout.Orientation.Horizontal, Spacing=5};
            var inputTask = new TextBox{HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch, Watermark="nowe zadanie", Width=400};
            var inputEndDate = new DatePicker{};

            taskTextBoxesList.Add(inputTask);
            taskEndDateList.Add(inputEndDate);
            mainSection.Children.Add(inputTask);
            mainSection.Children.Add(inputEndDate);
            inputTasksSection.Children.Add(mainSection);
        }
        private void TaskBuilder()
        {
            foreach(var date in taskEndDateList)
            {
                if (date.SelectedDate.HasValue && date.SelectedDate.Value.Date < DateTime.Today)
                {
                    //date = BorderBrush =3 itp że zła data
                    return;
                }
            }
            // if (inputEndDate.SelectedDate.Value < DateTime.Today)
            // {
            //     MessageBox.Show("Data nie może być z przeszłości!");
            //     return;
            // }
            int i = 0;
            foreach (var textBox in taskTextBoxesList)
            {
                string text = textBox.Text?.Trim() ?? "";
                if (!string.IsNullOrEmpty(text))
                {
                    if(taskEndDateList[i].SelectedDate.HasValue)
                    {
                      Builder.AddTaskComponent(new Task(text, taskEndDateList[i].SelectedDate.Value.Date));  
                    }
                    else{Builder.AddTaskComponent(new Task(text));}
                    
                }
                i++;
            }
            Builder.BuildTask();

            // Wyczyść pola
            inputTitle.Text = "";
            DisplayGroup(GlobalGroups.AllGroup);
        }






        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Save();
            // Builder.AddComponent
        }
        private void Save()
        {
            Desktop.Content = new TextBlock{VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                                            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                                            Text = "Zapisywanie..."};
        }
        private void Sych_Click(object sender, RoutedEventArgs e)
        {
            Save();

            var button = new Button { Content = "Zaloguj się", Name = "BtnLogIn" };
            // button2.Classes.Add("menuButton"); NIE POTRZEBNY TU STYL TEN, MOZE BYĆ DOMYŚLNY
            button.Click += (s, e) => Save_Click(s, e);

            var mainSection = new StackPanel{VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                                             HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                                             Orientation = Avalonia.Layout.Orientation.Horizontal,
                                             Spacing = 10};
            mainSection.Children.Add(button);
            Desktop.Content = mainSection;
        }




        // private void SaveCategoriesToCloud()
        // {
        //     var categories = Categories.GetCategories();
        //     // Tutaj kod zapisu do chmury (np. do pliku JSON, bazy danych itp.)
        //     // Przykład: Serializacja do JSON
        //     var json = System.Text.Json.JsonSerializer.Serialize(categories);
        //     // Zapisz gdzieś (plik, API, etc.)
        // }

        // private void LoadCategoriesFromCloud()
        // {
        //     // Tutaj kod wczytywania z chmury
        //     // Przykład: Deserializacja z JSON
        //     // var json = ... // wczytaj z chmury
        //     // var categories = System.Text.Json.JsonSerializer.Deserialize<List<Group>>(json);
        // }
        private void InitializeMenu()
        {
            var mainSectionTag = new StackPanel { };
            foreach (var button in GlobalGroups.GetTags())
            {
                button.Click += (s, e) => DisplayContaing(button.Name);
                mainSectionTag.Children.Add(button);
            }
            TagsExtender.Content = mainSectionTag;

            var mainSectionCategory = new StackPanel { };
            foreach (var button in GlobalGroups.GetCategories())
            {
                button.Click += (s, e) => DisplayContaing(button.Name);
                mainSectionCategory.Children.Add(button);
            }
            CategoriesExtender.Content = mainSectionCategory;



            var statsButton = new Button { Content = "Statystyki", Classes = { "leftMenuButton" } };
            statsButton.Click += (s, e) => DisplayStatistics();
            ButtonSection.Children.Add(statsButton);

            // Dodaj przycisk raportów
            var reportButton = new Button {Content = "Nadchodzące terminy", Classes = { "leftMenuButton" }};
            reportButton.Click += (s, e) => DisplayUpcomingTasks();
            ButtonSection.Children.Add(reportButton);


            searchButton.Click += (s, e) => PerformSearch(searchBox.Text);
        }
        private void DisplayStatistics()
        {
            Desktop.Content = GlobalGroups.GetStatistics();
        }

        private void DisplayUpcomingTasks()
        {
            Desktop.Content = GlobalGroups.GetUpcomingTasksReport(7); // Na najbliższy tydzień
        }

        private void PerformSearch(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return;

            Desktop.Content = GlobalGroups.Search(query, true); // true = szukaj również w treści
        }

        private void DisplayContaing(string tagOrCategory)
        {
            // Wyszukiwanie po tagach/kategoriach
            Desktop.Content = GlobalGroups.Search(tagOrCategory);
        }
    }
}