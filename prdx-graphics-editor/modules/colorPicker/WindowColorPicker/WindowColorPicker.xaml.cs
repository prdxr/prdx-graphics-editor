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
            this.Background = Globals.colorAccent2;

            this.ButtonApply.Background = Globals.colorAccent1;
            this.ButtonApply.Foreground = Globals.colorTextBright;
            this.ButtonCancel.Background = Globals.colorAccent1;
            this.ButtonCancel.Foreground = Globals.colorTextBright;
        }

        private void ButtonApply_Click(object sender, RoutedEventArgs e)
        {
            this.color = Globals.pageColorPickerRef.GetColor();
            Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void onMouseEnterStyle(object sender, MouseEventArgs e)
        {
            (sender as Button).Foreground = Globals.colorTextDim;
        }

        private void onMouseLeaveStyle(object sender, MouseEventArgs e)
        {
            (sender as Button).Foreground = Globals.colorTextBright;
        }
    }
}
