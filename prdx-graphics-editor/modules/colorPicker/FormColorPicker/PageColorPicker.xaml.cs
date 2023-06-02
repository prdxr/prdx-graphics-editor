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
            color = Globals.changingColor;
            textboxHexInput.Text = Globals.applicationSettings.colorPickerDefaultColor;
            PrepareGrid();
            UpdateColors();

            Background = Globals.colorAccent1;

            labelCurrentColor.Foreground = Globals.colorTextBright;
            labelHexInput.Foreground = Globals.colorTextBright;
            labelRgbField.Foreground = Globals.colorTextBright;
            labelR.Foreground = Globals.colorTextBright;
            labelG.Foreground = Globals.colorTextBright;
            labelB.Foreground = Globals.colorTextBright;

            textboxHexInput.Background = Globals.colorAccent1;
            textboxHexInput.Foreground = Globals.colorTextBright;
            textboxHexSign.Background = Globals.colorAccent2;
            textboxHexSign.Foreground = Globals.colorTextBright;
            textboxInputR.Background = Globals.colorAccent1;
            textboxInputR.Foreground = Globals.colorTextBright;
            textboxInputG.Background = Globals.colorAccent1;
            textboxInputG.Foreground = Globals.colorTextBright;
            textboxInputB.Background = Globals.colorAccent1;
            textboxInputB.Foreground = Globals.colorTextBright;
        }

        public Color? GetColor()
        {
            color = (Color)ColorConverter.ConvertFromString("#" + textboxHexInput.Text.ToLower());
            return color;
        }

        private readonly string[,] colorsTable = new string[,] {
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
        private readonly Regex allowedHex = new Regex("^[a-fA-F0-9]+$");
        private readonly Regex allowedRgb = new Regex ("^[0-9]+$");

        private void UpdateColors()
        {
            CurrentColorDisplay.Fill = new SolidColorBrush((Color)color);
            textboxHexInput.Text = color.ToString().Substring(3);
            Color newColor = (Color)color;
            textboxInputR.Text = newColor.R.ToString();
            textboxInputG.Text = newColor.G.ToString();
            textboxInputB.Text = newColor.B.ToString();
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
                    DockPanel panel = new DockPanel
                    {
                        Background = (Brush)brushConverter.ConvertFrom(color),
                        Width = 50,
                        Height = 50,
                    };
                    panel.MouseDown += SetColorFromTable;
                    Grid.SetRow(panel, row);
                    Grid.SetColumn(panel, col);
                    ColorsGrid.Children.Add(panel);
                }
            }

        }

        private void SetColorFromTable(object sender, MouseButtonEventArgs e)
        {
            int row = Grid.GetRow((DockPanel)sender);
            int col = Grid.GetColumn((DockPanel)sender);
            color = (Color)ColorConverter.ConvertFromString(colorsTable[row, col]);
            CurrentColorDisplay.Fill = new SolidColorBrush((Color)color);
            textboxHexInput.Text = color.ToString().Substring(3);
            Color newColor = (Color)color;
            textboxInputR.Text = newColor.R.ToString();
            textboxInputG.Text = newColor.G.ToString();
            textboxInputB.Text = newColor.B.ToString();
        }

        private void OnHexKeyPress(object sender, TextChangedEventArgs e)
        {
            TextBox trueSender = sender as TextBox;
            if (trueSender.Text.Length == 6)
            {
                if (UtilityFunctions.CheckInputValidity(trueSender, allowedHex, Globals.colorAccent1))
                {
                    color = (Color)ColorConverter.ConvertFromString("#" + textboxHexInput.Text.ToLower());
                    UpdateColors();
                }
            }
            else
            {
                trueSender.Background = Globals.colorAccent1;
            }
        }

        private void OnRgbKeyPress(object sender, TextChangedEventArgs e)
        {
            TextBox trueSender = sender as TextBox;
            bool correctInput = true;
            if (textboxInputR is null || textboxInputG is null || textboxInputB is null)
            {
                return;
            }
            else if (trueSender.Text.Length == 0 || !UtilityFunctions.CheckInputValidity(trueSender, allowedRgb, Globals.colorAccent1) || 
                Convert.ToInt32(trueSender.Text) > 255 || Convert.ToInt32(trueSender.Text) < 0)
            {
                trueSender.Background = Brushes.DarkRed;
                correctInput = false;
                Globals.windowColorPickerRef.ButtonApply.IsEnabled = false;
                return;
            }
            else
            {
                color = Color.FromRgb(Convert.ToByte(textboxInputR.Text), Convert.ToByte(textboxInputG.Text), Convert.ToByte(textboxInputB.Text));
                UpdateColors();
                Globals.windowColorPickerRef.ButtonApply.IsEnabled = true;
                trueSender.Background = Globals.colorAccent1;
            }
        }
    }
}
