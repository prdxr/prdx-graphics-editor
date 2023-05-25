using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
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
using prdx_graphics_editor.modules.utils;

namespace prdx_graphics_editor.modules.colorPicker.FormColorPicker
{
    /// <summary>
    /// Логика взаимодействия для PageColorPicker.xaml
    /// </summary>
    public partial class PageColorPicker : Page
    {
        private Color? color;

        public PageColorPicker()
        {
            InitializeComponent();
            Globals.pageColorPickerRef = this;
            this.color = Globals.changingColor;
            textboxHexInput.Text = Globals.applicationSettings.colorPickerDefaultColor;
            PrepareGrid();
            UpdateColors();

            this.Background = Globals.colorAccent1;

            this.labelCurrentColor.Foreground = Globals.colorTextBright;
            this.labelHexInput.Foreground = Globals.colorTextBright;
            this.labelRgbField.Foreground = Globals.colorTextBright;
            this.labelR.Foreground = Globals.colorTextBright;
            this.labelG.Foreground = Globals.colorTextBright;
            this.labelB.Foreground = Globals.colorTextBright;

            this.textboxHexInput.Background = Globals.colorAccent1;
            this.textboxHexInput.Foreground = Globals.colorTextBright;
            this.textboxHexSign.Background = Globals.colorAccent2;
            this.textboxHexSign.Foreground = Globals.colorTextBright;
            this.textboxInputR.Background = Globals.colorAccent1;
            this.textboxInputR.Foreground = Globals.colorTextBright;
            this.textboxInputG.Background = Globals.colorAccent1;
            this.textboxInputG.Foreground = Globals.colorTextBright;
            this.textboxInputB.Background = Globals.colorAccent1;
            this.textboxInputB.Foreground = Globals.colorTextBright;
        }

        public Color? GetColor()
        {
            this.color = (Color)ColorConverter.ConvertFromString("#" + textboxHexInput.Text.ToLower());
            return this.color;
        }

        string[,] colorsTable = new string[,] {
            {"#330000", "#331900", "#333300", "#193300", "#003300", "#003319", "#003333", "#001933", "#000033", "#190033", "#330033", "#330019", "#000000"},
            {"#660000", "#663300", "#666600", "#336600", "#006600", "#006633", "#006666", "#003366", "#000066", "#330066", "#660066", "#660033", "#202020"},
            {"#990000", "#994C00", "#999900", "#4C9900", "#009900", "#00994C", "#009999", "#004C99", "#000099", "#4C0099", "#990099", "#99004C", "#404040"},
            {"#CC0000", "#CC6600", "#CCCC00", "#66CC00", "#00CC00", "#00CC66", "#00CCCC", "#0066CC", "#0000CC", "#6600CC", "#CC00CC", "#CC0066", "#606060"},
            {"#FF0000", "#FF8000", "#FFFF00", "#80FF00", "#00FF00", "#00FF80", "#00FFFF", "#0080FF", "#0000FF", "#7F00FF", "#FF00FF", "#FF007F", "#808080"},
            {"#FF3333", "#FF9933", "#FFFF33", "#99FF33", "#33FF33", "#33FF99", "#33FFFF", "#3399FF", "#3333FF", "#9933FF", "#FF33FF", "#FF3399", "#A0A0A0"},
            {"#FF6666", "#FFB266", "#FFFF66", "#B2FF66", "#66FF66", "#66FFB2", "#66FFFF", "#66B2FF", "#6666FF", "#B266FF", "#FF66FF", "#FF66B2", "#C0C0C0"},
            {"#FF9999", "#FFCC99", "#FFFF99", "#CCFF99", "#99FF99", "#99FFCC", "#99FFFF", "#99CCFF", "#9999FF", "#CC99FF", "#FF99FF", "#FF99CC", "#E0E0E0"},
            {"#FFCCCC", "#FFE5CC", "#FFFFCC", "#E5FFCC", "#CCFFCC", "#CCFFE5", "#CCFFFF", "#CCE5FF", "#CCCCFF", "#E5CCFF", "#FFCCFF", "#FFCCE5", "#FFFFFF"}
        };

        readonly string allowedHex = "^[a-fA-F0-9]+$";
        readonly string allowedRgb = "^[0-9]+$";
        //readonly char[] allowedHex = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };

        private void UpdateColors()
        {
            this.CurrentColorDisplay.Fill = new SolidColorBrush((Color)this.color);
            this.textboxHexInput.Text = this.color.ToString().Substring(3);
            Color newColor = (Color)this.color;
            this.textboxInputR.Text = newColor.R.ToString();
            this.textboxInputG.Text = newColor.G.ToString();
            this.textboxInputB.Text = newColor.B.ToString();
        }

        private void PrepareGrid()
        {
            int rowsCount = colorsTable.GetLength(0);
            int colsCount = colorsTable.GetLength(1);
            for (int i = 0; i < colsCount; i++)
            {
                ColumnDefinition colDefinition = new ColumnDefinition();
                ColorsGrid.ColumnDefinitions.Add(colDefinition);
            }
            for (int i = 0; i < rowsCount; i++)
            {
                RowDefinition rowDefinition = new RowDefinition();
                ColorsGrid.RowDefinitions.Add(rowDefinition);
            }
            BrushConverter brushConverter = new BrushConverter();
            for (int row = 0; row < rowsCount; row++)
            {
                for (int col = 0; col < colsCount; col++)
                {
                    string color = colorsTable[row, col];
                    DockPanel panel = new DockPanel();
                    panel.Background = (Brush)brushConverter.ConvertFrom(color);
                    panel.MouseDown += SetColorFromTable;
                    panel.Width = 50;
                    panel.Height = 50;
                    Grid.SetRow(panel, row);
                    Grid.SetColumn(panel, col);
                    ColorsGrid.Children.Add(panel);
                }
            }

        }

        private event Action ColorChangeEvent;

        private void SetColorFromTable(object sender, MouseButtonEventArgs e)
        {
            int row = Grid.GetRow((DockPanel)sender);
            int col = Grid.GetColumn((DockPanel)sender);
            this.color = (Color)ColorConverter.ConvertFromString(colorsTable[row, col]);
            this.CurrentColorDisplay.Fill = new SolidColorBrush((Color)this.color);
            this.textboxHexInput.Text = this.color.ToString().Substring(3);
            Color newColor = (Color) this.color;
            this.textboxInputR.Text = newColor.R.ToString();
            this.textboxInputG.Text = newColor.G.ToString();
            this.textboxInputB.Text = newColor.B.ToString();
        }

        private void OnHexKeyPress(object sender, TextChangedEventArgs e)
        {
            bool correctInput = true;
            if ((sender as TextBox).Text.Length == 6)
            {
                for (int i = 0; i < 6; i++)
                {
                    if (!Regex.IsMatch((sender as TextBox).Text, allowedHex))
                    {
                        (sender as TextBox).Background = new SolidColorBrush(Colors.DarkRed);
                        correctInput = false;
                    }
                }
                if (correctInput)
                {
                    (sender as TextBox).Background = Globals.colorAccent1;
                    this.color = (Color)ColorConverter.ConvertFromString("#" + textboxHexInput.Text.ToLower());
                    UpdateColors();
                }
            }
            else
            {
                (sender as TextBox).Background = Globals.colorAccent1;
            }
        }

        private void OnRgbKeyPress(object sender, TextChangedEventArgs e)
        {
            bool correctInput = true;
            string contents = (sender as TextBox).Text;

            if (!Regex.IsMatch((sender as TextBox).Text, allowedRgb))
            {
                (sender as TextBox).Background = new SolidColorBrush(Colors.DarkRed);
                return;
            }

            if (textboxInputR is null || textboxInputG is null || textboxInputB is null)
            {
                return;
            }
            else if (Convert.ToInt32(contents) > 255 || Convert.ToInt32(contents) < 0)
            {
                (sender as TextBox).Background = new SolidColorBrush(Colors.DarkRed);
                correctInput = false;
            }

            if (correctInput)
            {
                (sender as TextBox).Background = Globals.colorAccent1;
                this.color = Color.FromRgb(Convert.ToByte(textboxInputR.Text), Convert.ToByte(textboxInputG.Text), Convert.ToByte(textboxInputB.Text));
                UpdateColors();
            }
        }


        //private void SelectColorFromDefault(object sender, MouseButtonEventHandler e)
        //{
        //    string hex = ((sender as DockPanel).Grid.Row)
        //    //call actions change color
        //}
    }
}
