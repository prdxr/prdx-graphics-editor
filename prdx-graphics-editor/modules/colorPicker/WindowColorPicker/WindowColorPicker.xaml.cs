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
using System.Windows.Shapes;
using prdx_graphics_editor.modules.utils;

namespace prdx_graphics_editor.modules.colorPicker.WindowColorPicker
{
    /// <summary>
    /// Логика взаимодействия для WindowColorPicker.xaml
    /// </summary>
    public partial class WindowColorPicker : Window
    {
        public Color? color;
        public WindowColorPicker()
        {
            InitializeComponent();
            Background = Globals.colorAccent2;

            ButtonApply.Background = Globals.colorAccent1;
            ButtonApply.Foreground = Globals.colorTextBright;
            ButtonCancel.Background = Globals.colorAccent1;
            ButtonCancel.Foreground = Globals.colorTextBright;

            Globals.windowColorPickerRef = this;
        }

        private void OnButtonApplyClick(object sender, RoutedEventArgs e)
        {
            color = Globals.pageColorPickerRef.GetColor();
            Close();
        }

        private void OnButtonCancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnMouseEnterStyle(object sender, MouseEventArgs e)
        {
            (sender as Button).Foreground = Globals.colorTextDim;
        }

        private void OnMouseLeaveStyle(object sender, MouseEventArgs e)
        {
            (sender as Button).Foreground = Globals.colorTextBright;
        }
    }
}
