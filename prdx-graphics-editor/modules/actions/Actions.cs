using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;
using prdx_graphics_editor.modules.canvas;
using prdx_graphics_editor.modules.utils;
using prdx_graphics_editor.modules.canvas.PageCanvas;
using prdx_graphics_editor.modules.colorPicker.WindowColorPicker;
using prdx_graphics_editor.modules.WindowHelp;

namespace prdx_graphics_editor.modules.actions
{
    static class Actions
    {
        // Методы установки основного цвета и цвета фона
        public static Color? PickForegroundColor()
        {
            Globals.changingColor = Globals.applicationSettings.primaryColor;
            WindowColorPicker window = new WindowColorPicker();
            window.ShowDialog();
            return window.color;
        }

        public static Color? PickBackgroundColor()
        {
            Globals.changingColor = Globals.applicationSettings.secondaryColor;
            WindowColorPicker window = new WindowColorPicker();
            window.ShowDialog();
            return window.color;
        }
        // Установка активного инструмента
        public static void SetActiveTool(CanvasToolType toolType)
        {
            Globals.pageCanvasRef.SetActiveTool(toolType);
        }
        // Оповещение пользователя в случае наличия несохранённых изменений и ожидание подтверждения/отмены операции
        private static bool CheckBeforeReset()
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
        // Создание проекта
        public static int CreateProject()
        {
            if (CheckBeforeReset())
            {
                var window = new WindowProjectCreator.WindowProjectCreator()
                {
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };
                window.ShowDialog();
            }
            return 0;
        }
        // Инициализация проекта, вызывается из окна создания проекта
        public static void InitializeProject(int width, int height)
        {
            Globals.pageInfoLineRef.SetCurrentProject();
            Globals.pageCanvasRef.ResetCanvas(width, height);
            Globals.isProjectSaved = true;
        }
        // Открытие проекта, расположение по умолчанию - <папка приложения>/<папка проектов> (задаётся в Globals.cs)
        public static int OpenProject()
        {
            if (CheckBeforeReset())
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
        // Сохранение проекта. Если проект не найден, вызывается SaveProjectAs()
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
        // Сохранение проекта с выбором расположения
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
        // Экспорт проекта с выбором расположения
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
        // Импорт изображения в качестве проекта
        public static void ImportProject()
        {
            string filename;
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "Графические файлы (*.png, *.jpeg, *.jpg, *.bmp) |*.png; *.jpeg; *.jpg; *.bmp";
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                if (CheckBeforeReset()) {
                    filename = dialog.FileName;
                    Globals.pageCanvasRef.ImportToProject(filename);
                }
            }
            else
            {
                return;
            }
        }
        // Методы работы с историей изменений
        public static void HistoryUndo()
        {
            Globals.pageCanvasRef.RemoveLastFigure();
        }
        public static void HistoryRedo()
        {
            Globals.pageCanvasRef.ReturnLastFigure();
        }
        // Методы работы с выделением
        public static void SelectAll()
        {
            Globals.pageCanvasRef.SelectAll();
        }
        public static void SelectionReset()
        {
            Globals.pageCanvasRef.SelectClear();
        }
        // Выход из приложения
        public static void CloseApplication(MainWindow caller)
        {
            if (CheckBeforeReset())
            {
                caller.Close();
            }
        }
        // Установка выбранного инструмента на панели инструментов
        public static void ChangeTool(object sender, ExecutedRoutedEventArgs e)
        {
            Globals.pageToolsRef.ChangeTool(sender, e);
        }
        // Смена цветов (основной цвет <-> цвет фона)
        public static void SwapColors(object sender, ExecutedRoutedEventArgs e)
        {
            Globals.pageToolsRef.SwapColors(sender, e);
        }
        // Вставка изображения
        public static void Paste()
        {
            Globals.pageCanvasRef.PasteClipboard();
        }
        // Копирование выделенной области
        public static void Copy()
        {
            Globals.pageCanvasRef.CopyToClipboard();
        }
        // Изменение размеров холста
        public static void CanvasSize()
        {
            Canvas canvas = Globals.pageCanvasRef.mainCanvas;
            WindowCanvasSize.ChangeCanvasSize(canvas);
        }
        // Отображение справки
        public static void ShowHelp()
        {
            WindowHelp.WindowHelp window = new WindowHelp.WindowHelp
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.ShowDialog();
        }
        // Увеличение коэффициента приближения холста
        public static void ZoomIncrease()
        {
            Globals.pageCanvasRef.ZoomIncrease();
        }
        // Уменьшение коэффициента приближения холста
        public static void ZoomDecrease()
        {
            Globals.pageCanvasRef.ZoomDecrease();
        }
        // Сброс коэффициента приближения холста до 100%
        public static void ZoomReset()
        {
            Globals.pageCanvasRef.SetCanvasZoom(100);
        }

    }
}
