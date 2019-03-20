using System.Windows;
using View.ViewModels;

namespace View
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = new GraphViewModel();
            InitializeComponent();
        }
    }
}
