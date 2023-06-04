using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using prdx_graphics_editor.modules.canvas.PageCanvas;
using prdx_graphics_editor.modules.main;
using prdx_graphics_editor.modules.main.PageInfoLine;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;
using prdx_graphics_editor.modules.colorPicker.FormColorPicker;
using prdx_graphics_editor.modules.colorPicker.WindowColorPicker;

namespace prdx_graphics_editor.modules.utils
{
    public static class Globals
    {
        public static ApplicationSettings applicationSettings = ApplicationSettings.CreateApplicationSettings();
        public static PageColorPicker pageColorPickerRef;
        public static WindowColorPicker windowColorPickerRef;
        public static PageTools pageToolsRef;
        public static PageCanvas pageCanvasRef;
        public static PageHistory pageHistoryRef;
        public static PageInfoLine pageInfoLineRef;
        public static string _currentFile;
        public static string currentFile
        {
            get
            {
                return _currentFile;
            }
            set
            {
                _currentFile = value;
                if (!(pageInfoLineRef is null))
                {
                    pageInfoLineRef.SetCurrentProject();
                }
            }
        }
        public static bool _isProjectSaved = true;
        public static bool isProjectSaved
        {
            get
            { 
                return _isProjectSaved;
            }
            set
            {
                _isProjectSaved = value;
                if (!(pageInfoLineRef is null))
                {
                    pageInfoLineRef.SetCurrentProject();
                }
            }
        }
        public static Color? changingColor;
        public static Stack<(Shape, string, Point)> changeHistoryBefore = new Stack<(Shape, string, Point)>();
        public static Stack<(Shape, string, Point)> changeHistoryAfter = new Stack<(Shape, string, Point)>();
        public static SolidColorBrush colorAccent1 = new SolidColorBrush(applicationSettings.applicationTheme[0]);
        public static SolidColorBrush colorAccent2 = new SolidColorBrush(applicationSettings.applicationTheme[1]);
        public static SolidColorBrush colorTextBright = new SolidColorBrush(applicationSettings.applicationTheme[2]);
        public static SolidColorBrush colorTextDim = new SolidColorBrush(applicationSettings.applicationTheme[3]);
    }
}
