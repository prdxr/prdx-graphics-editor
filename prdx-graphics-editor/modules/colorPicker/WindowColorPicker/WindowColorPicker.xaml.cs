using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using prdx_graphics_editor.modules.utils;

namespace prdx_graphics_editor.modules.colorPicker.WindowColorPicker
{

    public partial class WindowColorPicker : Window
    {

        public Color? color;

        public WindowColorPicker()
        {
            InitializeComponent();
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
            (sender as Button).Foreground = Globals.appcolorText;
        }
    }
}
