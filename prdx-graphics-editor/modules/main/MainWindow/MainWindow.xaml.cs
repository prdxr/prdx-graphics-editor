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
            mainMenu.Background = Globals.colorAccent2;
            mainMenuFile.Foreground = Globals.colorTextBright;
            mainMenuEdit.Foreground = Globals.colorTextBright;
            mainMenuSelect.Foreground = Globals.colorTextBright;
            mainGrid.Background = Globals.colorAccent1;
            leftDockPanel.Background = Globals.colorAccent2;
            topDockPanel.Background = Globals.colorAccent2;
            rightDockPanel.Background = Globals.colorAccent2;
            bottomDockPanel.Background = Globals.colorAccent2;

            //var setter = (topDockPanel.Resources["MenuStyle"] as Style).Setters.OfType<Setter>().FirstOrDefault(s => s.Property == MenuItem.BackgroundProperty);
            //setter.Value = Globals.colorAccent1;

            //foreach (var menuItem in topDockPanel.Children.OfType<MenuItem>())
            //{
            //    if (menuItem == null)
            //    {
            //        menuItem.Style = topDockPanel.Resources["MenuStyle"] as Style;
            //    }
            //}

            ////var setter = topDockPanel.Resources;
            ////["Style"] as Style).Setters.OfType<Setter>().FirstOrDefault(s => s.Value == MenuItem.BackgroundProperty) as Setter;
            //if (setter != null)
            //{
            //    setter.Value = Globals.colorTextBright;
            //}
        }

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    Color? color = Actions.PickColor();
        //    if (color != null)
        //    {
        //        (sender as Button).Background = new SolidColorBrush(color.GetValueOrDefault());
        //    }
        //}

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

        private void ImportToProject(object sender, ExecutedRoutedEventArgs e)
        {
            Actions.ImportToProject();
        }
        private void ExportProject(object sender, ExecutedRoutedEventArgs e)
        {
            Actions.ExportProject();
        }
        private void UndoAction(object sender, ExecutedRoutedEventArgs e)
        {
            Actions.Undo();
        }

        private void RedoAction(object sender, ExecutedRoutedEventArgs e)
        {
            Actions.Redo();
        }

        private void SelectAll(object sender, ExecutedRoutedEventArgs e)
        {
            Actions.SelectAll();
        }

        private void DeselectAll(object sender, ExecutedRoutedEventArgs e)
        {
            Actions.SelectClear();
        }

        private void ExitApp(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
            //Actions.CloseApplication();
        }
    }
}
