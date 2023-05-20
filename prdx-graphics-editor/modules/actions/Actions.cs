using System;
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
                string filename;
                var dialog = new Microsoft.Win32.SaveFileDialog();
                
                string path; // this is the path that you are checking.
                if (Directory.Exists(Globals.currentFile))
                {
                    dialog.InitialDirectory = Globals.currentFile;
                }
                else
                {
                    dialog.InitialDirectory = @"C:\";
                }

                dialog.Title = "Выбор места проекта";
                dialog.Filter = "Файл проекта (*.xml) |*.xml";
                Nullable<bool> result = dialog.ShowDialog();
                if (result == true)
                {
                    if (CreateProject() == -1)
                    {
                        return 1;
                    }
                    filename = dialog.FileName;
                    Globals.pageCanvasRef.ImportToProject(filename);

                }
                else
                {
                    return 1;
                }
            }

            

            return 0;
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
                if (CreateProject() == -1) { 
                    return;
                }
                filename = dialog.FileName;
                Globals.pageCanvasRef.ImportToProject(filename);

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
