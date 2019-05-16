using System.Windows;
using ViewModel;

namespace View
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class AntennaWindow : Window
    {
        public AntennaWindow()
        {
            DataContext = new AntennaViewModel();
            InitializeComponent();
        }
    }
}
