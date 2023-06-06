using prdx_graphics_editor.modules.actions;
using prdx_graphics_editor.modules.canvas.PageCanvas;
using prdx_graphics_editor.modules.main;
using prdx_graphics_editor.modules.utils;
using System.Windows;
using System.Windows.Input;

namespace prdx_graphics_editor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            FrameCanvas.Content = new PageCanvas();
            FrameHistory.Content = new PageHistory();
            Globals.pageCanvasRef.OnFiguresChanged += Globals.pageHistoryRef.OnFiguresChanged;
        }

        private void CreateProject(object sender, ExecutedRoutedEventArgs e)
        {
            Actions.CreateProject();
        }

        private void OpenProject(object sender, ExecutedRoutedEventArgs e)
        {
            Actions.OpenProject();
        }

        private void SaveProject(object sender, ExecutedRoutedEventArgs e)
        {
            Actions.SaveProject();
        }

        private void SaveProjectAs(object sender, ExecutedRoutedEventArgs e)
        {
            Actions.SaveProjectAs();
        }

        private void ImportProject(object sender, ExecutedRoutedEventArgs e)
        {
            Actions.ImportProject();
        }

        private void ExportProject(object sender, ExecutedRoutedEventArgs e)
        {
            Actions.ExportProject();
        }

        private void CanvasSize(object sender, ExecutedRoutedEventArgs e)
        {
            Actions.CanvasSize();
        }

        private void ZoomIncrease(object sender, ExecutedRoutedEventArgs e)
        {
            Actions.ZoomIncrease();
        }

        private void ZoomDecrease(object sender, ExecutedRoutedEventArgs e)
        {
            Actions.ZoomDecrease();
        }

        private void ZoomReset(object sender, ExecutedRoutedEventArgs e)
        {
            Actions.ZoomReset();
        }

        private void HistoryUndo(object sender, ExecutedRoutedEventArgs e)
        {
            Actions.HistoryUndo();
        }

        private void HistoryRedo(object sender, ExecutedRoutedEventArgs e)
        {
            Actions.HistoryRedo();
        }

        private void Cut(object sender, ExecutedRoutedEventArgs e)
        {
            //Actions.Cut();
        }

        private void Copy(object sender, ExecutedRoutedEventArgs e)
        {
            //Actions.Copy();
        }

        private void Paste(object sender, ExecutedRoutedEventArgs e)
        {
            Actions.Paste();
        }

        private void SelectAll(object sender, ExecutedRoutedEventArgs e)
        {
            Actions.SelectAll();
        }

        private void SelectionReset(object sender, ExecutedRoutedEventArgs e)
        {
            Actions.SelectionReset();
        }

        private void ChangeTool(object sender, ExecutedRoutedEventArgs e)
        {
            Actions.ChangeTool(sender, e);
        }

        private void SwapColors(object sender, ExecutedRoutedEventArgs e)
        {
            Actions.SwapColors(sender, e);
        }

        private void CloseApplication(object sender, ExecutedRoutedEventArgs e)
        {
            Actions.CloseApplication(this);
        }

    }
}
