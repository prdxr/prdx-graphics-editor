using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using prdx_graphics_editor.modules.color_picker.WindowColorPicker;
using System.Windows.Media;

namespace prdx_graphics_editor.modules.actions
{
    static class Actions
    {
        public static Color? PickColor()
        {
            WindowColorPicker window = new WindowColorPicker();
            window.ShowDialog();
            Color? color = window.color;
            return color;
        }
    }
}
