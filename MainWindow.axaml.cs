using Avalonia;
using Avalonia.Controls;
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
        // Listy danych
        private List<string> wszystkieElementy = new List<string>
        {
            "Wszystko 1", "Wszystko 2", "Wszystko 3", "Wszystko 4"
        };

        private List<string> zadania = new List<string>
        {
            "Zadanie 1: Zrobić zakupy",
            "Zadanie 2: Nauczyć się Avalonii",
            "Zadanie 3: Spotkanie z klientem",
            "Zadanie 4: Napisać raport"
        };

        private List<string> notatki = new List<string>
        {
            "Notatka 1: Pomysł na projekt",
            "Notatka 2: Lista zakupów",
            "Notatka 3: Spotkania w tym tygodniu",
            "Notatka 4: Ważne numery telefonów"
        };

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



        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                string buttonName = button.Name;

                switch (buttonName)
                {
                    case "BtnAll":
                        WyswietlListe(GlobalGroups.AllGroup, GlobalGroups.AllGroup.Name);
                        break;
                    case "BtnTasks":
                        WyswietlListe(GlobalGroups.AllTasksGroup, GlobalGroups.AllTasksGroup.Name);
                        break;
                    case "BtnNotes":
                        WyswietlListe(GlobalGroups.AllNotesGroup, GlobalGroups.AllNotesGroup.Name);
                        break;
                    default:
                        ContentText.Text = "Nieznany przycisk";
                        break;
                }
            }
        }

        private void WyswietlListe(Group group, string naglowek)
        {
            ContentText.Text = naglowek + "\n\n" + group.GetDetailedList();
        }
    }
}