using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using prdx_graphics_editor.modules.colorPicker.WindowColorPicker;
using prdx_graphics_editor.modules.WindowProjectCreator;
using System.Windows.Media;
using prdx_graphics_editor.modules.canvas.PageCanvas;
using prdx_graphics_editor.modules.utils;
using System.IO;
using System.Windows.Shapes;
using System.Windows.Input;


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

        private static bool CheckBeforeErasing()
        {
            if (!Globals.isProjectSaved)
                {

                string boxCaption = "Несохранённые изменения";
                string boxText = "Внимание! Есть несохранёные изменения, которые будут утеряны при продолжении. Сохранить проект?";
                MessageBoxButton boxButtons = MessageBoxButton.YesNoCancel;
                MessageBoxImage boxIcon = MessageBoxImage.Warning;
                MessageBoxResult boxResult = MessageBox.Show(boxText, boxCaption, boxButtons, boxIcon);

                if (boxResult == MessageBoxResult.Yes)
                {
                    SaveProject();
                    return true;
                }
                else if (boxResult == MessageBoxResult.No)
                {
                    return true;
                }
                else if (boxResult == MessageBoxResult.Cancel)
                {
                    return false;
                }
            }
            return true;
        }

        public static int CreateProject()
        {
            if (CheckBeforeErasing())
            {
                var window = new WindowProjectCreator.WindowProjectCreator()
                {
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };
                window.ShowDialog();
            }
            return 0;
        }

        public static void InitializeProject(int width, int height)
        {
            Globals.pageInfoLineRef.SetCurrentProject();
            Globals.pageCanvasRef.ResetCanvas(width, height);
            Globals.isProjectSaved = true;
        }

        public static int OpenProject()
        {
            if (CheckBeforeErasing())
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
                Globals.isProjectSaved = true;
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
                Globals.isProjectSaved = true;
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
                if (CheckBeforeErasing()) {
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
            if (CheckBeforeErasing())
            {
                caller.Close();
            }
        }

        public static void ChangeTool(object sender, ExecutedRoutedEventArgs e)
        {
            Globals.pageToolsRef.ChangeTool(sender, e);
        }
        public static void SwitchColors(object sender, ExecutedRoutedEventArgs e)
        {
            Globals.pageToolsRef.SwitchColors(sender, e);
        }

        public static void Paste()
        {
            Globals.pageCanvasRef.PasteClipboard();
        }
    }
}
