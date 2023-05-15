using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using prdx_graphics_editor.modules.actions;
using prdx_graphics_editor.modules.utils;
using prdx_graphics_editor.modules.canvas.PageCanvas;
using prdx_graphics_editor.modules.main;

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
            this.FrameCanvas.Content = new PageCanvas();
            this.FrameHistory.Content = new PageHistory();
            Globals.pageCanvasRef.OnFiguresChanged += Globals.pageHistoryRef.OnFiguresChanged;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Color? color = Actions.PickColor();
            if (color != null)
            {
                (sender as Button).Background = new SolidColorBrush(color.GetValueOrDefault());
            }
        }

        private void CreateProject(object sender, RoutedEventArgs e)
        {
            Actions.CreateProject();
        }

        private void ExportProject(object sender, RoutedEventArgs e)
        {
            Actions.ExportProject();
        }

        private void ImportToProject(object sender, RoutedEventArgs e)
        {
            Actions.ImportToProject();
        }

        private void ClickUndo(object sender, RoutedEventArgs e)
        {
            Actions.Undo();
        }
        private void ClickRedo(object sender, RoutedEventArgs e)
        {
            Actions.Redo();
        }

        private void SelectAll(object sender, RoutedEventArgs e)
        {
            Actions.SelectAll();
        }
        private void SelectClear(object sender, RoutedEventArgs e)
        {
            Actions.SelectClear();
        }

        private void CloseApplication(object sender, RoutedEventArgs e)
        {
            this.Close();
            //Actions.CloseApplication();
        }
    }
}
