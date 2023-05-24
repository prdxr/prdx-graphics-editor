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
        readonly string allowedMask = "^[0-9]+$";

        public PageToolParams()
        {
            InitializeComponent();
            mainLabel.Foreground = Globals.colorTextBright;
            paramGrid.Background = Globals.colorAccent2;

            labelBrushThickness.Foreground = Globals.colorTextBright;
            labelBrushThickness.Background = Brushes.Transparent;
            textboxBrushThickness.Foreground = Globals.colorTextBright;
            textboxBrushThickness.Background = Globals.colorAccent1;

            labelBorderThickness.Foreground = Globals.colorTextBright;
            labelBorderThickness.Background = Brushes.Transparent;
            textboxBorderThickness.Foreground = Globals.colorTextBright;
            textboxBorderThickness.Background = Globals.colorAccent1;

            labelEnableBorder.Foreground = Globals.colorTextBright;
            labelEnableBorder.Background = Brushes.Transparent;
            
            labelEnableFill.Foreground = Globals.colorTextBright;
            labelEnableFill.Background = Brushes.Transparent;
            
            labelEnableBoth.Foreground = Globals.colorTextBright;
            labelEnableBoth.Background = Brushes.Transparent;

            getParamsData();
        }

        void getParamsData()
        {
            textboxBorderThickness.Text = Globals.applicationSettings.borderSize.ToString();
            textboxBrushThickness.Text = Globals.applicationSettings.brushSize.ToString();

            if (Globals.applicationSettings.enableFigureBorder)
            {
                if(Globals.applicationSettings.enableFigureFill)
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
            
            if (trueSender.Text.Length == 0)
            {
                trueSender.Background = Brushes.DarkRed;
                return;
            }

            for (int i = 0; i < trueSender.Text.Length; i++)
            {
                if (!Regex.IsMatch(trueSender.Text, allowedMask))
                {
                    trueSender.Background = Brushes.DarkRed;
                    return;
                }
            }

            int newSize = Convert.ToInt32((sender as TextBox).Text);
            
            Globals.applicationSettings.brushSize = newSize;
            trueSender.Background = Globals.colorAccent1;
        }
        private void ChangeBorderThickness(object sender, TextChangedEventArgs e)
        {
            TextBox trueSender = sender as TextBox;

            if (trueSender.Text.Length == 0 || trueSender.Text[0] == '0')
            {
                trueSender.Background = Brushes.DarkRed;
                return;
            }

            for (int i = 0; i < trueSender.Text.Length; i++)
            {
                if (!Regex.IsMatch(trueSender.Text, allowedMask))
                {
                    trueSender.Background = Brushes.DarkRed;
                    return;
                }
            }

            int newSize = Convert.ToInt32((sender as TextBox).Text); 
            
            Globals.applicationSettings.borderSize = newSize;
            trueSender.Background = Globals.colorAccent1;
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
