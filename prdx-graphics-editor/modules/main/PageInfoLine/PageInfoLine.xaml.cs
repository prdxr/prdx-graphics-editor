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
using prdx_graphics_editor.modules.utils;

namespace prdx_graphics_editor.modules.main.PageInfoLine
{
    /// <summary>
    /// Логика взаимодействия для PageInfoLine.xaml
    /// </summary>
    public partial class PageInfoLine : Page
    {
        public PageInfoLine()
        {
            InitializeComponent();
            Globals.pageInfoLineRef = this;
            SetCurrentProject();
        }

        public void SetPointerValues(Point? pointer)
        {
            if (pointer is null)
            {
                labelPointerPosition.Content = "Вне холста";
            }
            else
            {
                labelPointerPosition.Content = $"{Math.Round(pointer.Value.X, 1)}, {Math.Round(pointer.Value.Y, 1)}";
            }
        }

        public void SetCurrentProject()
        {
            textblockCurrentLocation.Text = Globals.currentFile == null ? "[без проекта]" : Globals.currentFile;
            textblockUnsavedAsterisk.Visibility = Globals.isProjectSaved ? Visibility.Hidden : Visibility.Visible;
            //textblockUnsavedAsterisk.Visibility = Globals.isProjectSaved ? Visibility.Visible : Visibility.Visible;
        }
    }
}
