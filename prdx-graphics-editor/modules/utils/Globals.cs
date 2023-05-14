using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using prdx_graphics_editor.modules.color_picker.FormColorPicker;
using prdx_graphics_editor.modules.canvas.PageCanvas;
using prdx_graphics_editor.modules.main;
using System.Windows.Shapes;
using System.Windows;

namespace prdx_graphics_editor.modules.utils
{
    public static class Globals
    {
        public static ApplicationSettings applicationSettings = ApplicationSettings.CreateApplicationSettings();
        public static PageColorPicker pageColorPickerRef;
        public static PageCanvas pageCanvasRef;
        public static PageHistory pageHistoryRef;
        public static string currentFile;
        public static Stack<(Shape, string, Point)> changeHistoryBefore = new Stack<(Shape, string, Point)>();
        public static Stack<(Shape, string, Point)> changeHistoryAfter = new Stack<(Shape, string, Point)>();


        //public static ApplicationSettings applicationSettings = new ApplicationSettings();
        //public static CanvasPage canvasPageRef;
        //public static SettingsPage settingsPageRef;

    }
}
