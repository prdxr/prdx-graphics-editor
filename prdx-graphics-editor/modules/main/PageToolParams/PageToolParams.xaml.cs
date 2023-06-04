using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace prdx_graphics_editor.modules.main.PageToolParams
{
    /// <summary>
    /// Логика взаимодействия для ToolParams.xaml
    /// </summary>
    public partial class PageToolParams : Page
    {
        private readonly Regex allowedMask = new Regex(("^[0-9]+$"));

        public PageToolParams()
        {
            InitializeComponent();
            GetParamsData();
        }

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

        private void ChangeBrushThickness(object sender, TextChangedEventArgs e)
        {
            TextBox trueSender = sender as TextBox;

            if (UtilityFunctions.CheckInputValidity(trueSender, allowedMask, Globals.appcolorAccent1))
            {
                int newSize = Convert.ToInt32((sender as TextBox).Text);

                Globals.applicationSettings.brushSize = newSize;
            }
        }
        private void ChangeBorderThickness(object sender, TextChangedEventArgs e)
        {
            TextBox trueSender = sender as TextBox;

            if (trueSender.Text[0] == '0')
            {
                trueSender.Background = Brushes.DarkRed;
                return;
            }

            if (UtilityFunctions.CheckInputValidity(trueSender, allowedMask, Globals.appcolorAccent1))
            {
                int newSize = Convert.ToInt32((sender as TextBox).Text);
                Globals.applicationSettings.borderSize = newSize;
            }
        }

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
