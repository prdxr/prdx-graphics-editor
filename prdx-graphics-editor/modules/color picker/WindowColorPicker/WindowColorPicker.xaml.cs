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

namespace prdx_graphics_editor.modules.color_picker.WindowColorPicker
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
    }
}
