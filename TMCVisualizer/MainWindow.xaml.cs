using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TMCVisualizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += Handle_MainWindow_Loaded;
        }

        private void Handle_MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.KeyUp += new KeyEventHandler(Handle_MainWindow_KeyUp);
        }

        private void Handle_MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                // Do something
                var result = MessageBox.Show("Quit?", "Quit TMCVisualizer?", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    Close();
                }
            }
            else if (e.Key == Key.O)
            {
                Microsoft.Win32.OpenFileDialog openDialog = new Microsoft.Win32.OpenFileDialog();
                openDialog.Filter = "(*.mp3)|*.mp3";
                if (openDialog.ShowDialog() == true)
                {
                    NAudioEngine.Instance.OpenFile(openDialog.FileName);
                }
            }
        }
    }
}
