﻿using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;
using prdx_graphics_editor.modules.main;
using prdx_graphics_editor.modules.main.PageInfoLine;
using prdx_graphics_editor.modules.canvas.PageCanvas;
using prdx_graphics_editor.modules.colorPicker.PageColorPicker;
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
        public static Color? changingColor;
        public static Stack<(object, string, Point)> changeHistoryBefore = new Stack<(object, string, Point)>();
        public static Stack<(object, string, Point)> changeHistoryAfter = new Stack<(object, string, Point)>();
        public static SolidColorBrush appcolorAccent1 = new SolidColorBrush(applicationSettings.applicationTheme[0]);
        public static SolidColorBrush appcolorAccent2 = new SolidColorBrush(applicationSettings.applicationTheme[1]);
        public static SolidColorBrush appcolorText = new SolidColorBrush(applicationSettings.applicationTheme[2]);

        private static string _currentFile;
        // При изменении значения currentFile полученное значение сохраняется в _currentFile и вызывается метод установки текущего проекта в информационной строке
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
        private static bool _isProjectSaved = true;
        // При изменении значения isProjectSaved полученное значение сохраняется в _isProjectSaved и вызывается метод установки текущего проекта в информационной строке
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
    }
}
