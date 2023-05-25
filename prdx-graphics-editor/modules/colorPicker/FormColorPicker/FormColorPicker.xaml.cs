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
    /// Логика взаимодействия для FormColorPicker.xaml
    /// </summary>
    public partial class FormColorPicker : Page
    {
        private Color? color;
        public FormColorPicker()
        {
            InitializeComponent();
            Globals.formColorPickerRef = this;
            TextBoxColorRgb.Text = Globals.applicationSettings.colorPickerDefaultColor;
            this.color = null;
        }

        public Color? GetColor()
        {
            this.color = (Color)ColorConverter.ConvertFromString(TextBoxColorRgb.Text);
            return this.color;
        }

        void OnNewHex(object sender, TextChangedEventArgs e)
        {
            if (this.TextBoxColorRgb.Text.Length == 7 && this.TextBoxColorRgb.Text[0] == '#')
            {
                //this.PreviewColor.Background = GetColor();
            }
        }
    }
}
