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
using prdx_graphics_editor.modules.utils;

namespace prdx_graphics_editor.modules.color_picker.FormColorPicker
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
            TextBoxColorRgb.Text = Globals.applicationSettings.colorPickerDefaultColor;
            this.color = null;
        }

        public Color? GetColor()
        {
            this.color = (Color)ColorConverter.ConvertFromString(TextBoxColorRgb.Text);
            return this.color;
        }
    }
}
