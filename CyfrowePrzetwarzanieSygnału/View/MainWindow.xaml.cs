using System.Windows;
using System.Windows.Input;
using ViewModel;

namespace View
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = new MainViewModel();
            InitializeComponent();
        }

        private void OpenAntennaWindow(object sender, RoutedEventArgs e)
        {
            AntennaWindow aw = new AntennaWindow();
            aw.Show();
        }
    }
}
