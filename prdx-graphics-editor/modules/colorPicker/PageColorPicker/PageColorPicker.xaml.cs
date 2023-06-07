using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Text.RegularExpressions;
using prdx_graphics_editor.modules.utils;

namespace prdx_graphics_editor.modules.colorPicker.PageColorPicker
{
    public partial class PageColorPicker : Page
    {
        private Color? color;

        public PageColorPicker()
        {
            InitializeComponent();
            Globals.pageColorPickerRef = this;
            color = Globals.changingColor;
            TextBoxHexInput.Text = Globals.applicationSettings.colorPickerDefaultColor;
            PrepareGrid();
            UpdateColors();
        }
        // Получение итогового цвета из HEX поля, вызывается в конце работы окна выбора цвета
        public Color? GetColor()
        {
            return (Color)ColorConverter.ConvertFromString("#" + TextBoxHexInput.Text.ToLower());
        }
        // Матрица стандартных цветов
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
        // Маски допустимых символов для HEX и RGB полей
        private readonly Regex RegexHexColorString = new Regex("^[a-fA-F0-9]{6,6}$");
        private readonly Regex RegexRgbColorNumber = new Regex ("^[0-9]{1,3}$");

        // Обновление значений всех элементов, отображающих текущий цвет, в соответствии с сохранённым значением цвета
        private void UpdateColors()
        {
            RectangleCurrentColor.Fill = new SolidColorBrush((Color)color);
            TextBoxHexInput.Text = color.ToString().Substring(3);
            Color newColor = (Color)color;
            TextBoxInputR.Text = newColor.R.ToString();
            TextBoxInputG.Text = newColor.G.ToString();
            TextBoxInputB.Text = newColor.B.ToString();
        }

        // Инициализация палитры стандартных цветов
        private void PrepareGrid()
        {
            int rowsCount = colorsTable.GetLength(0);
            int colsCount = colorsTable.GetLength(1);
            for (int i = 0; i < colsCount; i++)
            {
                ColumnDefinition colDefinition = new ColumnDefinition();
                GridColors.ColumnDefinitions.Add(colDefinition);
            }
            for (int i = 0; i < rowsCount; i++)
            {
                RowDefinition rowDefinition = new RowDefinition();
                GridColors.RowDefinitions.Add(rowDefinition);
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
                    GridColors.Children.Add(panel);
                }
            }

        }

        // Установка цвета из палитры стандартных цветов
        private void SetColorFromTable(object sender, MouseButtonEventArgs e)
        {
            int row = Grid.GetRow((DockPanel)sender);
            int col = Grid.GetColumn((DockPanel)sender);
            color = (Color)ColorConverter.ConvertFromString(colorsTable[row, col]);
            RectangleCurrentColor.Fill = new SolidColorBrush((Color)color);
            TextBoxHexInput.Text = color.ToString().Substring(3);
            Color newColor = (Color)color;
            TextBoxInputR.Text = newColor.R.ToString();
            TextBoxInputG.Text = newColor.G.ToString();
            TextBoxInputB.Text = newColor.B.ToString();
        }
        // Проверка ввода, установка красного фона у поля с некорректным значением, запрет или разрешение изменений
        private void SetInputValid(TextBox textBox, bool isValid)
        {
            textBox.Background = !isValid ? Brushes.DarkRed : Globals.appcolorAccent1;
            Globals.windowColorPickerRef.ButtonApply.IsEnabled = isValid;
        }
        // Проверка значений в поле HEX при любом изменении
        private void OnKeyPressHex(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            bool isValid = UtilityFunctions.CheckInputValidity(textBox, RegexHexColorString, Globals.appcolorAccent1);
            SetInputValid(textBox, isValid);
            if (isValid) 
            {
                color = (Color)ColorConverter.ConvertFromString("#" + TextBoxHexInput.Text.ToLower());
                UpdateColors();
            }
        }
        // Проверка значений в полях RGB при любом изменении
        private void OnKeyPressRgb(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (TextBoxInputR is null || TextBoxInputG is null || TextBoxInputB is null)
            {
                return;
            }
            bool isValid = true;

            isValid = !(textBox.Text.Length == 0
                || !UtilityFunctions.CheckInputValidity(textBox, RegexRgbColorNumber, Globals.appcolorAccent1)
                || Convert.ToInt32(textBox.Text) > 255 || Convert.ToInt32(textBox.Text) < 0);


            SetInputValid(textBox, isValid);
            if (isValid)
            {
                color = Color.FromRgb(Convert.ToByte(TextBoxInputR.Text), Convert.ToByte(TextBoxInputG.Text), Convert.ToByte(TextBoxInputB.Text));
                UpdateColors();
            }
        }
    }

}
