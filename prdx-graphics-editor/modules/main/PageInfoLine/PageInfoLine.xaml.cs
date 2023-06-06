using System.Windows;
using System.Windows.Controls;
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
                labelPointerPosition.Content = $"{pointer.Value.X}, {pointer.Value.Y}";
            }
        }

        public void SetCurrentProject()
        {
            textblockCurrentLocation.Text = Globals.currentFile;
            textblockUnsavedAsterisk.Visibility = Globals.isProjectSaved ? Visibility.Hidden : Visibility.Visible;
            //textblockUnsavedAsterisk.Visibility = Globals.isProjectSaved ? Visibility.Visible : Visibility.Visible;
        }
    }
}
