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

        public static void CreateProject()
        {
            if (Globals.pageCanvasRef.isEmpty == false)
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
            }
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
    }
}
