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

namespace prdx_graphics_editor.modules.main.PageZoom
{
    /// <summary>
    /// Логика взаимодействия для PageZoom.xaml
    /// </summary>
    public partial class PageZoom : Page
    {
        public PageZoom()
        {
            InitializeComponent();
            Globals.pageCanvasRef.OnZoomChanged += OnZoomChanged;
        }

        public void OnZoomChanged(object sender, EventArgs e)
        {
            double canvasZoom = Globals.pageCanvasRef.getCanvasZoom();
            LabelZoom.Content = $"{canvasZoom}%";
        }

        private void ButtonZoomMinus_Click(object sender, RoutedEventArgs e)
        {
            Actions.CanvasZoomMinus();
        }

        private void ButtonZoomActual_Click(object sender, RoutedEventArgs e)
        {
            Actions.CanvasZoomActual();
        }

        private void ButtonZoomPlus_Click(object sender, RoutedEventArgs e)
        {
            Actions.CanvasZoomPlus();
        }

    }
}
