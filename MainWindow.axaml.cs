using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System.Collections.Generic;

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
        }
        
        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                string buttonName = button.Name;
                
                switch (buttonName)
                {
                    case "BtnAll":
                        WyswietlListe(wszystkieElementy, "Wszystkie elementy:");
                        break;
                    case "BtnTasks":
                        WyswietlListe(zadania, "Lista zadań:");
                        break;
                    case "BtnNotes":
                        WyswietlListe(notatki, "Notatki:");
                        break;
                    default:
                        ContentText.Text = "Nieznany przycisk";
                        break;
                }
            }
        }

        private void WyswietlListe(List<string> lista, string naglowek)
        {
            string tekst = naglowek + "\n\n";
            
            for (int i = 0; i < lista.Count; i++)
            {
                tekst += $"{i + 1}. {lista[i]}\n";
            }
            
            ContentText.Text = tekst;
        }
    }
}