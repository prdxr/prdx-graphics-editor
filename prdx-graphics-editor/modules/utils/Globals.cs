using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using prdx_graphics_editor.modules.color_picker.FormColorPicker;
using prdx_graphics_editor.modules.canvas.PageCanvas;
using System.Windows.Shapes;

namespace prdx_graphics_editor.modules.utils
{
    public static class Globals
    {
        public static ApplicationSettings applicationSettings = ApplicationSettings.CreateApplicationSettings();
        public static PageColorPicker pageColorPickerRef;
        public static PageCanvas pageCanvasRef;
        public static string currentFile;
        public static Queue<Shape> changeHistoryBefore = new Queue<Shape>();
        public static Queue<Shape> changeHistoryAfter = new Queue<Shape>();


        //public static ApplicationSettings applicationSettings = new ApplicationSettings();
        //public static CanvasPage canvasPageRef;
        //public static SettingsPage settingsPageRef;

    }
}
