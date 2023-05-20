﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using prdx_graphics_editor.modules.color_picker.WindowColorPicker;
using System.Windows.Media;
using prdx_graphics_editor.modules.canvas.PageCanvas;
using prdx_graphics_editor.modules.utils;
using System.IO;
using System.Windows.Shapes;


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

        public static void SetActiveTool(CanvasToolType toolType)
        {
            Globals.pageCanvasRef.SetActiveTool(toolType);
        }

        private static int CheckBeforeErasing()
        {
            string boxCaption = "Новый проект";
            string boxText = "Внимание! Вы собираетесь открыть новый проект, но текущий не был сохранён. Сохранить проект?";
            MessageBoxButton boxButtons = MessageBoxButton.YesNoCancel;
            MessageBoxImage boxIcon = MessageBoxImage.Warning;
            MessageBoxResult boxResult = MessageBox.Show(boxText, boxCaption, boxButtons, boxIcon);

            if (boxResult == MessageBoxResult.Yes)
            {
                //сохранить
                //создать проект
                Globals.pageCanvasRef.ResetCanvas();
            }
            else if (boxResult == MessageBoxResult.No)
            {
                //создать проект
                Globals.pageCanvasRef.ResetCanvas();
            }
            else if (boxResult == MessageBoxResult.Cancel)
            {
                return -1;
            }
            return 0;
        }

        public static int CreateProject()
        {
            if (CheckBeforeErasing() == 0)
            {
                var dialog = new Microsoft.Win32.SaveFileDialog();
                
                if (Directory.Exists(ApplicationSettings.projectspath))
                {
                    dialog.InitialDirectory = ApplicationSettings.projectspath;
                }
                else
                {
                    dialog.InitialDirectory = @"C:\";
                }

                dialog.Title = "Выбор расположения проекта";
                dialog.Filter = "Файл проекта (*.xml) |*.xml";
                Nullable<bool> result = dialog.ShowDialog();
                if (result == true)
                {
                    Globals.currentFile = dialog.FileName;

                }
                else
                {
                    return 1;
                }
            }
            return 0;
        }

        public static int OpenProject()
        {
            if (CheckBeforeErasing() == 0)
            {
                var dialog = new Microsoft.Win32.OpenFileDialog();

                if (Directory.Exists(ApplicationSettings.projectspath))
                {
                    dialog.InitialDirectory = ApplicationSettings.projectspath;
                }
                else
                {
                    dialog.InitialDirectory = @"C:\";
                }

                dialog.Title = "Выбор расположения проекта";
                dialog.Filter = "Файл проекта (*.xml) |*.xml";
                Nullable<bool> result = dialog.ShowDialog();
                if (result == true)
                {
                    Globals.currentFile = dialog.FileName;
                    Globals.pageCanvasRef.DeserializeFromXML(dialog.FileName);

                }
                else
                {
                    return 1;
                }
            }
            return 0;
        }

        public static void SaveProject()
        {
            if (Globals.currentFile == null)
            {
                SaveProjectAs();
            }

            Globals.pageCanvasRef.SerializeToXML(Globals.pageCanvasRef.mainCanvas, Globals.currentFile);
        }

        public static void SaveProjectAs()
        {
            var dialog = new Microsoft.Win32.SaveFileDialog();

            if (Directory.Exists(ApplicationSettings.projectspath))
            {
                dialog.InitialDirectory = ApplicationSettings.projectspath;
            }
            else
            {
                dialog.InitialDirectory = @"C:\";
            }

            dialog.Title = "Выбор расположения проекта";
            dialog.Filter = "Файл проекта (*.xml) |*.xml";
            Nullable<bool> result = dialog.ShowDialog();
            if (result == true)
            {
                Globals.currentFile = dialog.FileName;

            }

            Globals.pageCanvasRef.SerializeToXML(Globals.pageCanvasRef.mainCanvas, Globals.currentFile);
        }

        public static void ExportProject()
        {
            string filename;
            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.Filter = "Сжатие без потерь (*.png) |*.png; | Высокая степень сжатия (*.jpeg) | *.jpeg; | Битмап без сжатия (*.bmp) | *.bmp";
            Nullable<bool> result = dialog.ShowDialog();
            if (result == true)
            {
                filename = dialog.FileName;
                var ext = System.IO.Path.GetExtension(filename);
                if (ext == ".png" || ext == ".jpeg" || ext == ".bmp")
                {
                    Globals.pageCanvasRef.ExportProject(ext, filename);
                }

            }
            else
            {
                return;
            }
        }

        public static void ImportToProject()
        {
            string filename;
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "Графические файлы (*.png, *.jpeg, *.jpg, *.bmp) |*.png; *.jpeg; *.jpg; *.bmp";
            Nullable<bool> result = dialog.ShowDialog();
            if (result == true)
            {
                if (CheckBeforeErasing() == 0) {
                    filename = dialog.FileName;
                    Globals.pageCanvasRef.ImportToProject(filename);
                }
            }
            else
            {
                return;
            }
        }

        public static void Undo()
        {
            Globals.pageCanvasRef.RemoveLastFigure();
        }
        public static void Redo()
        {
            Globals.pageCanvasRef.ReturnLastFigure();
        }

        public static void SelectAll()
        {
            Globals.pageCanvasRef.SelectAll();
        }
        public static void SelectClear()
        {
            Globals.pageCanvasRef.SelectClear();
        }
    }
}
