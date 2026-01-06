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



            Task zadanie1 = new Task("Nauczyć się Avalonii", new DateTime(2025, 12, 15));
            zadanie1.SetPriority(Priorities.Important);
            GlobalGroups.AllTasksGroup.Add(zadanie1);

            Task zadanie2 = new Task("Zrobić zakupy", new DateTime(2025, 11, 25));
            GlobalGroups.AllTasksGroup.Add(zadanie2);

            Task zadanie3 = new Task("Napisać raport", new DateTime(2025, 12, 10));
            zadanie3.SetPriority(Priorities.Normal);
            GlobalGroups.AllTasksGroup.Add(zadanie3);

            // Zadanie już wykonane
            Task zadanie4 = new Task("Umyć naczynia", new DateTime(2025, 11, 20));
            zadanie4.MarkAsCompleted(new DateTime(2025, 11, 19)); // Wykonane przed terminem
            GlobalGroups.AllTasksGroup.Add(zadanie4);

            // Zadanie z opóźnieniem
            Task zadanie5 = new Task("Oddać książki do biblioteki", new DateTime(2025, 11, 15));
            zadanie5.MarkAsCompleted(new DateTime(2025, 11, 18)); // Wykonane po terminie
            GlobalGroups.AllTasksGroup.Add(zadanie5);

            // Dodajemy też do AllGroup
            foreach (var task in GlobalGroups.AllTasksGroup.GetComponents())
            {
                if (!GlobalGroups.AllGroup.Contains(task))
                {
                    GlobalGroups.AllGroup.Add(task);
                }
            }

            // Tworzymy listę zadań (TaskList) - Projekt Avalonia
            TaskList projektAvalonia = new TaskList("Projekt Avalonia");
            projektAvalonia.Add(new Task("Stworzyć UI", new DateTime(2025, 12, 5)));
            projektAvalonia.Add(new Task("Zaimplementować logikę biznesową", new DateTime(2025, 12, 12)));
            projektAvalonia.Add(new Task("Przetestować aplikację", new DateTime(2025, 12, 18)));
            projektAvalonia.Add(new Task("Dokumentacja", new DateTime(2025, 12, 20)));
            projektAvalonia.SetPriority(Priorities.Important);

            GlobalGroups.AllTasksGroup.Add(projektAvalonia);
            GlobalGroups.AllGroup.Add(projektAvalonia);

            // Druga lista zadań - Codzienne obowiązki
            TaskList codzienneObowiazki = new TaskList("Codzienne obowiązki");
            codzienneObowiazki.Add(new Task("Poranna kawa", new DateTime(2025, 11, 22)));
            codzienneObowiazki.Add(new Task("Spacer z psem", new DateTime(2025, 11, 22)));
            codzienneObowiazki.Add(new Task("Planowanie dnia", new DateTime(2025, 11, 22)));


            GlobalGroups.AllTasksGroup.Add(codzienneObowiazki);
            GlobalGroups.AllGroup.Add(codzienneObowiazki);

            // Trzecia lista zadań - Studia (zagnieżdżona struktura)
            TaskList przedmiot1 = new TaskList("Analiza Matematyczna");
            przedmiot1.Add(new Task("Rozdział 1 - Granice", new DateTime(2025, 11, 28)));
            przedmiot1.Add(new Task("Rozdział 2 - Pochodne", new DateTime(2025, 12, 5)));

            TaskList przedmiot2 = new TaskList("Programowanie Obiektowe");
            przedmiot2.Add(new Task("Wzorzec Singleton", new DateTime(2025, 11, 25)));
            przedmiot2.Add(new Task("Wzorzec Fabryka", new DateTime(2025, 12, 2)));
            przedmiot2.Add(new Task("Wzorzec Obserwator", new DateTime(2025, 12, 9)));

            TaskList studia = new TaskList("Zadania ze studiów");
            studia.Add(przedmiot1);
            studia.Add(przedmiot2);
            studia.SetPriority(Priorities.Normal);

            GlobalGroups.AllTasksGroup.Add(studia);
            GlobalGroups.AllGroup.Add(studia);

            // Dodajemy notatki
            Note notatka1 = new Note("Pomysł na projekt", "Stworzyć aplikację do zarządzania zadaniami z użyciem wzorców projektowych.");
            Note notatka2 = new Note("Lista zakupów", "Mleko, Jajka, Chleb, Owoce, Warzywa, Kawa");
            Note notatka3 = new Note("Spotkania w tym tygodniu",
                "Poniedziałek: Spotkanie zespołu 10:00\n" +
                "Wtorek: Prezentacja projektu 14:00\n" +
                "Czwartek: Konsultacje z klientem 11:30");

            GlobalGroups.AllNotesGroup.Add(notatka1);
            GlobalGroups.AllNotesGroup.Add(notatka2);
            GlobalGroups.AllNotesGroup.Add(notatka3);

            // Dodajemy również do AllGroup
            foreach (var note in GlobalGroups.AllNotesGroup.GetComponents())
            {
                if (!GlobalGroups.AllGroup.Contains(note))
                {
                    GlobalGroups.AllGroup.Add(note);
                }
            }

            // Notatka techniczna
            Note notatka4 = new Note("Ważne linki",
                "Avalonia docs: https://docs.avaloniaui.net/\n" +
                "GitHub projektu: https://github.com/AvaloniaUI\n" +
                ".NET dokumentacja: https://learn.microsoft.com/dotnet/");
            GlobalGroups.AllNotesGroup.Add(notatka4);
            GlobalGroups.AllGroup.Add(notatka4);

            // Notatka z cytatem
            Note notatka5 = new Note("Inspiracja",
                "„Perfekcjonizm to nie dążenie do doskonałości, " +
                "a strach przed popełnieniem błędu.” – Brene Brown");
            GlobalGroups.AllNotesGroup.Add(notatka5);
            GlobalGroups.AllGroup.Add(notatka5);

            // Dodajemy też przykłady do kategorii
            Categories.Add("szkoła");
            Categories.Add("dom");
            Categories.Add("praca");
            Categories.Add("hobby");

            // Przypisujemy elementy do kategorii
            Categories.AddToCategory(zadanie1, "szkoła");
            Categories.AddToCategory(zadanie2, "dom");
            Categories.AddToCategory(zadanie3, "praca");
            Categories.AddToCategory(projektAvalonia, "szkoła");
            Categories.AddToCategory(codzienneObowiazki, "dom");
            Categories.AddToCategory(studia, "szkoła");
            Categories.AddToCategory(notatka1, "praca");
            Categories.AddToCategory(notatka2, "dom");
            Categories.AddToCategory(notatka4, "szkoła");
            Categories.AddToCategory(notatka5, "hobby");

            // Dodajemy też tagi
            Tags.Add("pilne");
            Tags.Add("ważne");
            Tags.Add("codzienne");
            Tags.Add("studia");
            Tags.Add("projekt");

            // Przypisujemy tagi do elementów
            Tags.AddToCategory(zadanie1, "pilne");
            Tags.AddToCategory(zadanie1, "studia");
            Tags.AddToCategory(projektAvalonia, "projekt");
            Tags.AddToCategory(projektAvalonia, "ważne");
            Tags.AddToCategory(codzienneObowiazki, "codzienne");
            Tags.AddToCategory(zadanie4, "codzienne");
            Tags.AddToCategory(notatka4, "ważne");


            Group group1 = new Group("Katalog");
            group1.Add(zadanie1);
            group1.Add(studia);
            GlobalGroups.AllGroup.Add(group1);












    var panel = new StackPanel{};
    
    // Utwórz przyciski ręcznie
    foreach (var category in Categories.GetCategories())
    {
        var button = new Button{Content = category.Name,
                                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch};
        
        // Przypisz event który wywoła SimpleDisplay z depth = 0
        button.Click += (s, e) => Desktop.Content = category.SimpleDisplay();
        button.Classes.Add("leftMenuButton");
      
        panel.Children.Add(button);
    }
    CategoriesExtender.Content = panel;



























           

            // foreach (var category in Categories.GetCategories())
            // {
            //     var categoryButton = category.DisplayGUI();
            //     categoryButton.Click += (s, e) => DisplayGroup(category);
            //     categoriesContainer.Children.Add(categoryButton);
            // }
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
                        ContentText.Text = "Nieznany przycisk";
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
            string cat = @"       ,
       \`-._           __
        \\  `-..____,.'  `.
         :`.         /    \`.
         :  )       :      : \
          ;'        '   ;  |  :
          )..      .. .:.`.;  :
         /::...  .:::...   ` ;
         ; _ '    __        /:\
         `:o>   /\o_>      ;:. `.
        `-`.__ ;   __..--- /:.   \
        === \_/   ;=====_.':.     ;
         ,/'`--'...`--....        ;
              ;                    ;
            .'                      ;
          .'                        ;
        .'     ..     ,      .       ;
       :       ::..  /      ;::.     |
      /      `.;::.  |       ;:..    ;
     :         |:.   :       ;:.    ;
     :         ::     ;:..   |.    ;
      :       :;      :::....|     |
      /\     ,/ \      ;:::::;     ;
    .:. \:..|    :     ; '.--|     ;
   ::.  :''  `-.,,;     ;'   ;     ;
.-'. _.'\      / `;      \,__:      \
`---'    `----'   ;      /    \,.,,,/
                   `----`              ";
            var button1 = new Button { Content = "Nowa notatka", Name = "BtnSelectNote" };
            button1.Click += (s, e) => {
                                           Desktop.Content = new TextBlock{VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                                                                           Text = cat};
                                           NewNoteView();
                                       };
            var button2 = new Button { Content = "Nowe zadanie", Name = "BtnSelectTask" };
            button2.Click += (s, e) => Save_Click(s, e);

            var mainBox = new StackPanel{VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                                       HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                                       Orientation = Avalonia.Layout.Orientation.Horizontal,
                                       Spacing = 10};
            mainBox.Children.Add(button1);
            mainBox.Children.Add(button2);
            Desktop.Content = mainBox;
        }
        
        // Dodaj pola dla kontrolek
        private TextBox? inputTitle;
        private TextBox? inputContent;
        private ComboBox? SelectCategorie;
        private ListBox? selectTags;
        private Button? saveEditing;

        private void NewNoteView()
        {
            var mainBox = new StackPanel{Orientation = Avalonia.Layout.Orientation.Vertical,
                                       Spacing = 10,
                                       Margin = new Thickness(20)};

            inputTitle = new TextBox{HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                                     Text = Builder.DefaultName()};

            inputContent = new TextBox{HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                                        MinHeight = 300,
                                        AcceptsReturn = true};

            
            
            selectTags = new ListBox{SelectionMode = SelectionMode.Multiple, // Ważne: wiele wyborów
                                   ItemsSource = Tags.GetTags(),
                                   DisplayMemberBinding = new Avalonia.Data.Binding("Name") };

            SelectCategorie = new ComboBox{ItemsSource = Categories.GetCategories(),
                                     DisplayMemberBinding = new Avalonia.Data.Binding("Name") };


            saveEditing = new Button{Content = "Zapisz notatkę",
                                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                                    Width = 120,
                                    Margin = new Thickness(0, 10, 0, 0)};
            saveEditing.Click += (s, e) => NoteBuilder();
            

            mainBox.Children.Add(inputTitle);
            mainBox.Children.Add(inputContent);
        
            mainBox.Children.Add(selectTags);
            mainBox.Children.Add(SelectCategorie);
            mainBox.Children.Add(saveEditing);

            Desktop.Content = mainBox;
        }

        private void NoteBuilder()
        {
            string title = inputTitle.Text?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(title))
            { 
                inputTitle.Classes.Add("mustFill");
                return; 
            }
            inputTitle.Classes.Remove("mustFill");

            selectTags.SelectedItems.Cast<Group>().ToList();

            if (SelectCategorie.SelectedItem is Group category)
            {
                //Categories.AddToCategory(note, category.Name);
            }

            Builder.SetName(title);
            Builder.SetContent(inputContent.Text?.Trim() ?? "");
            Builder.BuildNote();


            // Wyczyść pola
            inputTitle.Text = "";
            inputContent.Text = "";
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

            var button1 = new Button { Content = "Zapisz do Chmury", Name = "BtnSynchToCloud" };
            // button1.Classes.Add("menuButton"); NIE POTRZEBNY TU STYL TEN, MOZE BYĆ DOMYŚLNY
            button1.Click += (s, e) => {
                                           Desktop.Content = new TextBlock{VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                                                                           HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                                                                           Text = "Zsynchroniczowanie "};
                                       };
            var button2 = new Button { Content = "Pobierz z Chmury", Name = "BtnSynchFromCloud" };
            // button2.Classes.Add("menuButton"); NIE POTRZEBNY TU STYL TEN, MOZE BYĆ DOMYŚLNY
            button2.Click += (s, e) => Save_Click(s, e);

            var mainBox = new StackPanel{VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                                       HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                                       Orientation = Avalonia.Layout.Orientation.Horizontal,
                                       Spacing = 10};
            mainBox.Children.Add(button1);
            mainBox.Children.Add(button2);
            Desktop.Content = mainBox;
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



        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string userInput = InputTextBox.Text;
            //później będzie tego logika
            ContentText.Text = $"hgdfgdgf";
            ContentText.Text += $"Wpisałeś: {userInput}";
        }
    }
}