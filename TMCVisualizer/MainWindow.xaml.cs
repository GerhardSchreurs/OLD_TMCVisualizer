using SharpMik;
using SharpMik.Drivers;
using SharpMik.Player;
using SharpMik.Player.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using WPFSoundVisualizationLib;

namespace TMCVisualizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Module Mod;
        public MikMod Player;

        public void Play(string fileName)
        {
            Mod = Player.LoadModule(fileName.Replace("mp3", "s3m"));
            //Mod.volume = 0;

            NAudioEngine.Instance.OpenFile(fileName);


            if (NAudioEngine.Instance.CanPlay)
            {
                NAudioEngine.Instance.Play();
                //Player.Play(Mod);
            }

        }


        public MainWindow()
        {
            InitializeComponent();

            Player = new MikMod();


            //waveformTimeline.LeftLevelBrush = new SolidColorBrush(darkColor);
            //waveformTimeline.RightLevelBrush = new SolidColorBrush(darkColor);



            //this.MouseMove += Handle_MainWindow_MouseMove;

            this.Loaded += Handle_MainWindow_Loaded;
            this.Closed += Handle_MainWindow_Closed;
        }

        private void Handle_MainWindow_Closed(object sender, EventArgs e)
        {
            ModPlayer.Player_Stop();
            ModDriver.MikMod_Exit();
        }

        public static Color ChangeColorBrightness(Color color, float correctionFactor)
        {
            float red = (float)color.R;
            float green = (float)color.G;
            float blue = (float)color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }

            var x = Color.FromArgb(0, 1, 2, 3);

            return Color.FromArgb(color.A, Convert.ToByte(red), Convert.ToByte(green), Convert.ToByte(blue));
        }

        private void Handle_MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            e.Handled = true;
        }

        private void Handle_MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Player.PlayerStateChangeEvent += Handle_Player_PlayerStateChangeEvent;

            EventInvoker.OnNewRow += Handle_EventInvoker_OnNewRow;

            ModDriver.Mode = (ushort)(ModDriver.Mode | SharpMikCommon.DMODE_NOISEREDUCTION);

            try
            {
                Player.Init<NaudioDriver>("");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            var color = System.Windows.Media.Colors.Green;
            var darkColor = ChangeColorBrightness(color, -0.1f);
            var darkerColor = ChangeColorBrightness(color, -0.2f);

            boxBackground.Fill = new SolidColorBrush(color);

            NAudioEngine soundEngine = NAudioEngine.Instance;
            waveformTimeline.RegisterSoundPlayer(soundEngine);
            spectrumAnalyzer.RegisterSoundPlayer(soundEngine);
            waveformTimeline.ProgressBarThickness = 5;
            waveformTimeline.ProgressBarBrush = new SolidColorBrush(Colors.Transparent);

            var style = new Style(typeof(Rectangle));
            style.Setters.Add(new Setter(Rectangle.FillProperty, new SolidColorBrush(darkerColor)));
            spectrumAnalyzer.BarStyle = style;
            spectrumAnalyzer.PeakStyle = style;

            this.KeyUp += new KeyEventHandler(Handle_MainWindow_KeyUp);

            Play("C:\\Users\\Wroah\\Documents\\MEKX-RMB.mp3");
        }

        private void Handle_EventInvoker_OnNewRow(object sender, SharpMik.Player.Events.EventArgs.RowEventArgs rowEventArgs)
        {
            foreach (var col in rowEventArgs.Row.Cols)
            {
                if (col.note == null || col.note == "")
                {
                    Console.Write(".. ");
                }
                else
                {
                    Console.Write($"{col.note} {col.sample} |");
                }

                //Console.Write($"col.note = {col.note}, col.name = {col.name}, ");
            }

            Console.WriteLine();

        }

        private void Handle_Player_PlayerStateChangeEvent(ModPlayer.PlayerState state)
        {
            //Debug.WriteLine(state);
        }

        private void Handle_MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (this.WindowState == WindowState.Maximized)
                {
                    this.WindowState = WindowState.Normal;
                    this.WindowStyle = WindowStyle.ThreeDBorderWindow;
                }
                else
                {
                    this.WindowState = WindowState.Maximized;
                    this.WindowStyle = WindowStyle.None;
                }
            }
            else if (e.Key == Key.O)
            {
                Microsoft.Win32.OpenFileDialog openDialog = new Microsoft.Win32.OpenFileDialog();
                openDialog.Filter = "(*.mp3)|*.mp3";
                if (openDialog.ShowDialog() == true)
                {
                    Play(openDialog.FileName);
                }
            }
            else if (e.Key == Key.Q)
            {
                // Do something
                var result = MessageBox.Show("Quit?", "Quit TMCVisualizer?", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    Close();
                }
            }
        }
    }
}
