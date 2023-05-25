using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using prdx_graphics_editor.modules.colorPicker.WindowColorPicker;
using System.Windows.Media;
using prdx_graphics_editor.modules.canvas.PageCanvas;
using prdx_graphics_editor.modules.utils;
using System.IO;
using System.Windows.Shapes;


namespace prdx_graphics_editor.modules.actions
{
    static class Actions
    {
        public static Color? PickColor(string changingColor)
        {
            if (changingColor == "foreground")
            {
                Globals.changingColor = Globals.applicationSettings.primaryColor;
            }
            else if (changingColor == "background")
            {
                Globals.changingColor = Globals.applicationSettings.secondaryColor;
            }

            WindowColorPicker window = new WindowColorPicker
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
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
            string boxCaption = "Несохранённые изменения";
            string boxText = "Внимание! Есть несохранёные изменения, которые будут утеряны при продолжении. Сохранить проект?";
            MessageBoxButton boxButtons = MessageBoxButton.YesNoCancel;
            MessageBoxImage boxIcon = MessageBoxImage.Warning;
            MessageBoxResult boxResult = MessageBox.Show(boxText, boxCaption, boxButtons, boxIcon);

            if (boxResult == MessageBoxResult.Yes)
            {
                SaveProject();
                Globals.pageCanvasRef.ResetCanvas();
            }
            else if (boxResult == MessageBoxResult.No)
            {
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
                
                dialog.InitialDirectory = Directory.Exists(ApplicationSettings.projectspath) ? ApplicationSettings.projectspath : @"C:\";

                dialog.Title = "Выбор расположения проекта";
                dialog.Filter = "Файл проекта (*.xml) |*.xml";
                bool? result = dialog.ShowDialog();
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

                dialog.InitialDirectory = Directory.Exists(ApplicationSettings.projectspath) ? ApplicationSettings.projectspath : @"C:\";

                dialog.Title = "Выбор расположения проекта";
                dialog.Filter = "Файл проекта (*.xml) |*.xml";
                bool? result = dialog.ShowDialog();
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
            else
            {
                Globals.pageCanvasRef.SerializeToXML(Globals.pageCanvasRef.mainCanvas, Globals.currentFile);
            }
        }

        public static void SaveProjectAs()
        {
            var dialog = new Microsoft.Win32.SaveFileDialog();

            dialog.InitialDirectory = Directory.Exists(ApplicationSettings.projectspath) ? ApplicationSettings.projectspath : @"C:\";

            dialog.Title = "Выбор расположения проекта";
            dialog.Filter = "Файл проекта (*.xml) |*.xml";
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                Globals.currentFile = dialog.FileName;
                Globals.pageCanvasRef.SerializeToXML(Globals.pageCanvasRef.mainCanvas, Globals.currentFile);
            }
        }

        public static void ExportProject()
        {
            string filename;
            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.Filter = "Сжатие без потерь (*.png) |*.png; | Высокая степень сжатия (*.jpeg) | *.jpeg; | Битмап без сжатия (*.bmp) | *.bmp";
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                filename = dialog.FileName;
                string ext = System.IO.Path.GetExtension(filename);
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
            bool? result = dialog.ShowDialog();
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

        public static void CloseApplication(MainWindow caller)
        {
            if (CheckBeforeErasing() == 0)
            {
                caller.Close();
            }
        }
    }
}
