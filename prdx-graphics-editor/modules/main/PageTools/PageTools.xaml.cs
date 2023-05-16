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
using prdx_graphics_editor.modules.actions;
using prdx_graphics_editor.modules.canvas.PageCanvas;
using prdx_graphics_editor.modules.utils;

namespace prdx_graphics_editor
{
    /// <summary>
    /// Логика взаимодействия для tools.xaml
    /// </summary>
    public partial class PageTools : Page
    {
        public PageTools()
        {
            InitializeComponent();
            ChangeColorListener();

            toolGrid.Background = Globals.colorAccent1;
            colorGrid.Background = Globals.colorAccent1;
            mainLabel.Foreground = Globals.colorTextBright;
        }

        private void ChangeTool(object sender, RoutedEventArgs e)
        {
            //List<RadioButton> buttons = new List<RadioButton>();
            RadioButton[] buttons = { 
                ButtonPencil,
                ButtonBrush,
                ButtonEraser,
                ButtonSelect,
                ButtonFill,
                ButtonSquare,
                ButtonCircle,
                ButtonTriangle,
                ButtonLine,
                ButtonArrow
            };
            int index = Array.IndexOf(buttons, sender as RadioButton);
            CanvasToolType toolType = (CanvasToolType) index;

            Actions.SetActiveTool(toolType);
        }

        public void ChangeColorListener()
        {
            buttonForegroundColor.Background = new SolidColorBrush(Globals.applicationSettings.primaryColor);
            buttonBackgroundColor.Background = new SolidColorBrush(Globals.applicationSettings.secondaryColor);
        }

        private void ChangeForegroundColor(object sender, RoutedEventArgs e)
        {
            Color? color = Actions.PickColor();
            if (color != null)
            {
                Color result = color.GetValueOrDefault();
                (sender as Button).Background = new SolidColorBrush(result);
                Globals.applicationSettings.primaryColor = result;
            }
        }
        private void ChangeBackgroundColor(object sender, RoutedEventArgs e)
        {
            Color? color = Actions.PickColor();
            if (color != null)
            {
                Color result = color.GetValueOrDefault();
                (sender as Button).Background = new SolidColorBrush(result);
                Globals.applicationSettings.secondaryColor = result;
            }
        }
        private void SwitchColors(object sender, RoutedEventArgs e)
        {
            Color buff = Globals.applicationSettings.secondaryColor;
            Globals.applicationSettings.secondaryColor = Globals.applicationSettings.primaryColor;
            Globals.applicationSettings.primaryColor = buff;
            ChangeColorListener();
        }
    }
}
