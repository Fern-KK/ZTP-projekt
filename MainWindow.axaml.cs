using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ZTP
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void Button_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                ContentText.Text = $"Wybrano: {button.Content}";
            }
        }
    }
}