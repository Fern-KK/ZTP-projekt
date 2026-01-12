using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using HarfBuzzSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ZTP;

public static class GlobalGroups
{
    public static Group AllGroup = new Group("Wszystko");
    public static Group AllTasksGroup = new Group("Zadania");
    public static Group AllNotesGroup = new Group("Notatki");
    private static List<string> AllCategories { get; } = new List<string>();
    private static List<string> AllTags = new List<string>();
    public static void Initialize()
    {
        ServerConnection client = ServerConnection.CreateServerConnection();
        client.FetchContent();
        // Dodaj domyślne tagi
        AddTags("pilne");
        AddTags("ważne");
        AddTags("codzienne");
        AddTags("studia");
        AddTags("praca");
        AddTags("zdrowie");
        AddTags("finanse");

        // Dodaj domyślne kategorie
        AddCategory("szkola");
        AddCategory("dom");
        AddCategory("dzieci");
        AddCategory("praca");
        AddCategory("zakupy");
        AddCategory("zdrowie");
        AddCategory("hobby");

        // PRZYKŁADOWE ZADANIA

        // Zadanie 1 - Codzienne obowiązki domowe
        var zadanie1 = new Task("Posprzątać w pokoju", new DateTime(2026, 1, 25));
        zadanie1.SetPriority(Priorities.Normal);
        zadanie1.SetTags(new List<string> { "codzienne", "dom" });
        zadanie1.SetCategory("dom");
        AllTasksGroup.Add(zadanie1);
        AllGroup.Add(zadanie1);

        // Zadanie 2 - Praca
        var zadanie2 = new Task("Przygotować prezentację dla klienta", new DateTime(2026, 1, 20));
        zadanie2.SetPriority(Priorities.Important);
        zadanie2.SetTags(new List<string> { "praca", "pilne" });
        zadanie2.SetCategory("praca");
        AllTasksGroup.Add(zadanie2);
        AllGroup.Add(zadanie2);

        // Zadanie 3 - Zdrowie
        var zadanie3 = new Task("Wizyta u dentysty", new DateTime(2026, 1, 30));
        zadanie3.SetPriority(Priorities.Normal);
        zadanie3.SetTags(new List<string> { "zdrowie" });
        zadanie3.SetCategory("zdrowie");
        AllTasksGroup.Add(zadanie3);
        AllGroup.Add(zadanie3);

        // Zadanie 4 - Zakupy
        var zadanie4 = new Task("Zrobić duże zakupy spożywcze", new DateTime(2026, 1, 18));
        zadanie4.SetPriority(Priorities.Normal);
        zadanie4.SetTags(new List<string> { "zakupy", "codzienne" });
        zadanie4.SetCategory("zakupy");
        AllTasksGroup.Add(zadanie4);
        AllGroup.Add(zadanie4);

        // Zadanie 5 - Pilne
        var zadanie5 = new Task("Zapłacić rachunki", new DateTime(2026, 1, 15));
        zadanie5.SetPriority(Priorities.Important);
        zadanie5.SetTags(new List<string> { "pilne", "finanse" });
        zadanie5.SetCategory("dom");
        AllTasksGroup.Add(zadanie5);
        AllGroup.Add(zadanie5);

        // Zadanie 6 - Hobby
        var zadanie6 = new Task("Przeczytać nową książkę", new DateTime(2026, 2, 10));
        zadanie6.SetPriority(Priorities.Low);
        zadanie6.SetTags(new List<string> { "hobby" });
        zadanie6.SetCategory("hobby");
        AllTasksGroup.Add(zadanie6);
        AllGroup.Add(zadanie6);

        // PRZYKŁADOWE NOTATKI

        // Notatka 1 - Szkoła (matematyka)
        var notatka1 = new Note("Pitagoras",
            "Twierdzenie Pitagorasa, które mówi, że w trójkącie prostokątnym suma kwadratów długości przyprostokątnych (a^2 + b^2) jest równa kwadratowi długości przeciwprostokątnej (c^2), co pozwala obliczać długości boków w takich trójkątach.\n\n" +
            "Wzór:\n" +
            "a^2 + b^2 = c^2\n\n" +
            "Przykład:\n" +
            "Jeśli a=3, b=4, to c=√(3²+4²)=√(9+16)=√25=5");
        notatka1.SetCategory("szkola");
        notatka1.SetTags(new List<string> { "studia" });
        AllNotesGroup.Add(notatka1);
        AllGroup.Add(notatka1);

        // Notatka 2 - Lista zakupów
        var notatka2 = new Note("Lista zakupów na tydzień",
            "PIEKARNIA:\n" +
            "- Chleb pełnoziarnisty\n" +
            "- Bułki\n" +
            "- Bagietka\n\n" +
            "NABIAŁ:\n" +
            "- Mleko 3,2%\n" +
            "- Jajka (10 szt)\n" +
            "- Ser żółty\n" +
            "- Jogurty naturalne\n\n" +
            "WARZYWA/OWOCE:\n" +
            "- Pomidory\n" +
            "- Ogórki\n" +
            "- Marchewki\n" +
            "- Jabłka\n" +
            "- Banany");
        notatka2.SetCategory("zakupy");
        notatka2.SetTags(new List<string> { "codzienne" });
        AllNotesGroup.Add(notatka2);
        AllGroup.Add(notatka2);

        // Notatka 3 - Przepis kulinarny
        var notatka3 = new Note("Przepis na spaghetti bolognese",
            "SKŁADNIKI:\n" +
            "- 400g mięsa mielonego\n" +
            "- 1 cebula\n" +
            "- 2 ząbki czosnku\n" +
            "- 400g pomidorów krojonych\n" +
            "- 2 łyżki koncentratu pomidorowego\n" +
            "- 250g makaronu spaghetti\n" +
            "- sól, pieprz, oregano, bazylia\n\n" +
            "PRZYGOTOWANIE:\n" +
            "1. Cebulę i czosnek posiekać, zeszklić na oliwie\n" +
            "2. Dodać mięso, smażyć do zrumienienia\n" +
            "3. Dodać pomidory i koncentrat, doprawić\n" +
            "4. Dusić 20-30 minut na małym ogniu\n" +
            "5. Ugotować makaron al dente\n" +
            "6. Połączyć makaron z sosem");
        notatka3.SetCategory("dom");
        AllNotesGroup.Add(notatka3);
        AllGroup.Add(notatka3);

        // Notatka 4 - Notatka techniczna
        var notatka4 = new Note("Komendy GIT - przydatne",
            "PODSTAWOWE KOMENDY:\n" +
            "git init - inicjalizacja repozytorium\n" +
            "git add . - dodanie wszystkich plików\n" +
            "git commit -m \"opis\" - commit ze wiadomością\n" +
            "git push - wysłanie zmian na serwer\n" +
            "git pull - pobranie zmian z serwera\n\n" +
            "BRANCHING:\n" +
            "git branch - lista branchy\n" +
            "git checkout -b nazwa - nowy branch\n" +
            "git merge nazwa - scalenie brancha");
        notatka4.SetCategory("szkola");
        notatka4.SetTags(new List<string> { "studia", "praca" });
        AllNotesGroup.Add(notatka4);
        AllGroup.Add(notatka4);

        // Notatka 5 - Spotkania
        var notatka5 = new Note("Harmonogram spotkań styczeń",
            "TYDZIEŃ 1 (06.01-12.01):\n" +
            "- Pon: Spotkanie zespołu 10:00\n" +
            "- Wt: Prezentacja projektu 14:00\n\n" +
            "TYDZIEŃ 2 (13.01-19.01):\n" +
            "- Śr: Szkolenie z Avalonii 9:00-13:00\n" +
            "- Pt: Podsumowanie tygodnia 16:00\n\n" +
            "TYDZIEŃ 3 (20.01-26.01):\n" +
            "- Pon: Spotkanie z klientem 11:30\n" +
            "- Czw: Demo aplikacji 15:00");
        notatka5.SetCategory("praca");
        notatka5.SetTags(new List<string> { "praca", "ważne" });
        AllNotesGroup.Add(notatka5);
        AllGroup.Add(notatka5);

        // Notatka 6 - Pomysły
        var notatka6 = new Note("Pomysły na projekty programistyczne",
            "1. Aplikacja do zarządzania budżetem domowym\n" +
            "2. System rezerwacji wizyty u lekarza\n" +
            "3. Platforma do nauki języków obcych\n" +
            "4. Gra edukacyjna dla dzieci\n" +
            "5. Analizator wydatków z paragonów\n\n" +
            "TECHNOLOGIE DO NAUKI:\n" +
            "- Avalonia dla desktop\n" +
            "- Blazor dla web\n" +
            ".NET MAUI dla mobile");
        notatka6.SetCategory("hobby");
        AllNotesGroup.Add(notatka6);
        AllGroup.Add(notatka6);

        // PRZYKŁADOWE LISTY ZADAŃ

        // Lista zadań 1 - Projekt szkolny
        var listaZadan1 = new TaskList("Projekt ze stron www");
        listaZadan1.SetCategory("szkola");
        listaZadan1.SetTags(new List<string> { "studia", "projekt" });

        var listaZadan1_zadanie1 = new Task("Opracować szkic wyglądu strony", new DateTime(2026, 1, 22));
        listaZadan1_zadanie1.SetPriority(Priorities.Normal);

        var listaZadan1_zadanie2 = new Task("Zakodować style w CSS", new DateTime(2026, 2, 5));
        listaZadan1_zadanie2.SetPriority(Priorities.Normal);

        var listaZadan1_zadanie3 = new Task("Zaimplementować responsywność", new DateTime(2026, 2, 12));
        listaZadan1_zadanie3.SetPriority(Priorities.Normal);

        var listaZadan1_zadanie4 = new Task("Przetestować na różnych przeglądarkach", new DateTime(2026, 2, 19));
        listaZadan1_zadanie4.SetPriority(Priorities.Normal);

        var listaZadan1_zadanie5 = new Task("Oddać projekt", new DateTime(2026, 3, 1));
        listaZadan1_zadanie5.SetPriority(Priorities.Important);

        listaZadan1.Add(listaZadan1_zadanie1);
        listaZadan1.Add(listaZadan1_zadanie2);
        listaZadan1.Add(listaZadan1_zadanie3);
        listaZadan1.Add(listaZadan1_zadanie4);
        listaZadan1.Add(listaZadan1_zadanie5);
        AllTasksGroup.Add(listaZadan1);
        AllGroup.Add(listaZadan1);

        // Lista zadań 2 - Porządki wiosenne
        var listaZadan2 = new TaskList("Porządki wiosenne w domu");
        listaZadan2.SetCategory("dom");
        listaZadan2.SetTags(new List<string> { "dom", "ważne" });

        listaZadan2.Add(new Task("Posprzątać piwnicę", new DateTime(2026, 3, 15)));
        listaZadan2.Add(new Task("Umyć okna", new DateTime(2026, 3, 20)));
        listaZadan2.Add(new Task("Posortować ubrania", new DateTime(2026, 3, 25)));
        listaZadan2.Add(new Task("Oddać niepotrzebne rzeczy", new DateTime(2026, 4, 1)));

        AllTasksGroup.Add(listaZadan2);
        AllGroup.Add(listaZadan2);

        // Lista zadań 3 - Nauka programowania
        var listaZadan3 = new TaskList("Nauka C# i .NET");
        listaZadan3.SetCategory("szkola");
        listaZadan3.SetTags(new List<string> { "studia", "hobby" });

        listaZadan3.Add(new Task("Przerobić tutorial OOP", new DateTime(2026, 1, 31)));
        listaZadan3.Add(new Task("Stworzyć prostą aplikację", new DateTime(2026, 2, 14)));
        listaZadan3.Add(new Task("Nauczyć się Entity Framework", new DateTime(2026, 2, 28)));
        listaZadan3.Add(new Task("Zrobić projekt z bazą danych", new DateTime(2026, 3, 15)));

        AllTasksGroup.Add(listaZadan3);
        AllGroup.Add(listaZadan3);

        // Lista zadań 4 - Dzieci (zagnieżdżona struktura)
        var listaZadan4_dzieci = new TaskList("Zadania związane z dziećmi");
        listaZadan4_dzieci.SetCategory("dzieci");

        var szkoła = new TaskList("Szkoła dzieci");
        szkoła.Add(new Task("Sprawdzić prace domowe", new DateTime(2026, 1, 19)));
        szkoła.Add(new Task("Zapłacić za wycieczkę", new DateTime(2026, 1, 25)));
        szkoła.Add(new Task("Rozmowa z wychowawcą", new DateTime(2026, 1, 28)));

        var zajęcia = new TaskList("Zajęcia dodatkowe");
        zajęcia.Add(new Task("Zawieźć na basen", new DateTime(2026, 1, 20)));
        zajęcia.Add(new Task("Kupić strój na zajęcia taneczne", new DateTime(2026, 1, 22)));
        zajęcia.Add(new Task("Umówić na angielski", new DateTime(2026, 1, 30)));

        listaZadan4_dzieci.Add(szkoła);
        listaZadan4_dzieci.Add(zajęcia);

        AllTasksGroup.Add(listaZadan4_dzieci);
        AllGroup.Add(listaZadan4_dzieci);
    }

    public static void AddTags(List<string> tags)
    {
        foreach (var t in tags)
        {
            string tag = t?.Trim().ToLower() ?? "";
            if (!string.IsNullOrWhiteSpace(tag) && !AllTags.Contains(tag))
            {
                AllTags.Add(tag);
            }

        }
    }
    public static void AddTags(string tag)
    {
        if (tag.Contains(','))
        {
            List<string> tags = new List<string>(tag.Split(','));
            AddTags(tags);

        }
        else
        {
            string t = tag?.Trim().ToLower() ?? "";
            if (!string.IsNullOrWhiteSpace(t) && !AllTags.Contains(t))
            {
                AllTags.Add(t);
            }
        }
        
    }
    public static List<Button> GetTags()
    {
        var buttons = new List<Button>();
        foreach(var tag in AllTags)
        {
            var tagButton = new Button{Content = $"#{tag}", Name=tag};
            tagButton.Classes.Add("leftMenuButton");
            buttons.Add(tagButton);
        }
        return buttons;
    }


    public static void AddCategory(List<string> categories)
    {
        foreach (var c in categories)
        {
            string category = c?.Trim().ToLower() ?? "";
            if (!string.IsNullOrWhiteSpace(category) && !AllCategories.Contains(category))
            {
                AllCategories.Add(category);
            }

        }
    }
    public static void AddCategory(string category)
    {
            if(category==null){return;};
            List<string> categories = new List<string>(category.Split(','));
            AddCategory(categories);
        
    }
    public static List<Button> GetCategories()
    {
        var buttons = new List<Button>();
        foreach(var category in AllCategories)
        {
            var categoryButton = new Button{Content = $"{category}", Name=category};
            categoryButton.Classes.Add("leftMenuButton");
            buttons.Add(categoryButton);
        }
        return buttons;
    }
    public static ComboBox SelectableCategoryList()
    {
        return new ComboBox{ItemsSource = AllCategories };
    }















    public static StackPanel Search(string query)
    {
        var visitor = new SearchVisitor(query);
        AllGroup.Accept(visitor);
        
        var results = visitor.GetResults();
        var group = new Group($"Wyniki wyszukiwania: '{query}'");
        
        foreach (var result in results)
        {
            group.Add(result);
        }
        
        return group.SimpleDisplay();
    }
    
    public static StackPanel GetStatistics()
    {
        var visitor = new StatisticsVisitor();
        AllGroup.Accept(visitor);
        
        return visitor.GetStatisticsPanel();
    }
    
    public static StackPanel GetUpcomingTasksReport(int daysAhead = 7)
    {
        var startDate = DateTime.Today;
        var endDate = DateTime.Today.AddDays(daysAhead);
        
        var visitor = new UpcomingDeadlinesVisitor(startDate, endDate);
        AllGroup.Accept(visitor);
        
        return visitor.GetUpcomingTasksPanel();
    }
    
    public static StackPanel GetTasksByPriority(Priorities priority)
    {
        var group = new Group($"Zadania z priorytetem: {priority}");
        var components = AllGroup.GetComponents();
        
        foreach (var component in components)
        {
            if (component is ITaskComponent taskComponent && taskComponent.Priority == priority)
            {
                group.Add(component);
            }
            else if (component is Group g)
            {
                // Możesz dodać rekurencyjne przeszukiwanie
            }
        }
        
        return group.SimpleDisplay();
    }
}




