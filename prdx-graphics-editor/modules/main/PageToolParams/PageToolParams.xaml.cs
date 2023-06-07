using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Text.RegularExpressions;
using prdx_graphics_editor.modules.utils;

namespace prdx_graphics_editor.modules.main.PageToolParams
{
    public partial class PageToolParams : Page
    {
        // Маска разрешённых значений для размеров кисти и границы фигур, в данном случае - цифры от 0 до 9
        private readonly Regex allowedMask = new Regex("^[0-9]+$");

        public PageToolParams()
        {
            InitializeComponent();
            GetParamsData();
        }

        // Получение текущих значений всех параметров для отображения актуальных данных
        private void GetParamsData()
        {
            textboxBorderThickness.Text = Globals.applicationSettings.borderSize.ToString();
            textboxBrushThickness.Text = Globals.applicationSettings.brushSize.ToString();

            if (Globals.applicationSettings.enableFigureBorder)
            {
                if (Globals.applicationSettings.enableFigureFill)
                {
                    radiobuttonEnableBoth.IsChecked = true;
                }
                else
                {
                    radiobuttonEnableBorder.IsChecked = true;
                }
            }
            else if (Globals.applicationSettings.enableFigureFill)
            {
                radiobuttonEnableFill.IsChecked = true;
            }
            else
            {
                radiobuttonEnableBorder.IsChecked = false;
                radiobuttonEnableFill.IsChecked = false;
                radiobuttonEnableBoth.IsChecked = false;
            }
        }

        // Методы изменения толщины кисти и толщины границы
        private void ChangeBrushThickness(object sender, TextChangedEventArgs e)
        {
            TextBox trueSender = sender as TextBox;

            if (!UtilityFunctions.CheckInputValidity(trueSender, allowedMask, Globals.appcolorAccent1) || trueSender.Text[0] == '0')
            {
                trueSender.Background = Brushes.DarkRed;
                return;

            }
            int newSize = Convert.ToInt32((sender as TextBox).Text);
            Globals.applicationSettings.brushSize = newSize;
        }
        private void ChangeBorderThickness(object sender, TextChangedEventArgs e)
        {
            TextBox trueSender = sender as TextBox;

            if (!UtilityFunctions.CheckInputValidity(trueSender, allowedMask, Globals.appcolorAccent1) || trueSender.Text[0] == '0')
            {
                trueSender.Background = Brushes.DarkRed;
                return;

            }
            int newSize = Convert.ToInt32((sender as TextBox).Text);
            Globals.applicationSettings.borderSize = newSize;
        }

        // Методы переключения режима рисования фигур. Используются радио-кнопками
        private void UseOnlyBorder(object sender, RoutedEventArgs e)
        {
            Globals.applicationSettings.enableFigureBorder = true;
            Globals.applicationSettings.enableFigureFill = false;
        }
        private void UseOnlyFill(object sender, RoutedEventArgs e)
        {
            Globals.applicationSettings.enableFigureBorder = false;
            Globals.applicationSettings.enableFigureFill = true;
        }
        private void UseFillAndBorder(object sender, RoutedEventArgs e)
        {
            Globals.applicationSettings.enableFigureBorder = true;
            Globals.applicationSettings.enableFigureFill = true;
        }
    }
}
