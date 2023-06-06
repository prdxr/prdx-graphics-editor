using System;
using System.Windows;
using System.Windows.Controls;
using prdx_graphics_editor.modules.actions;
using prdx_graphics_editor.modules.canvas.PageCanvas;
using prdx_graphics_editor.modules.utils;

namespace prdx_graphics_editor.modules.main.PageZoom
{

    public partial class PageZoom : Page
    {
        public PageZoom()
        {
            InitializeComponent();
            SliderZoom.Minimum = PageCanvas.ZOOM_PERCENT_MIN;
            SliderZoom.Maximum = PageCanvas.ZOOM_PERCENT_MAX;
            SliderZoom.TickFrequency = PageCanvas.ZOOM_PERCENT_STEP;
            SliderZoom.Value = PageCanvas.ZOOM_PERCENT_DEFAULT;
            Globals.pageCanvasRef.OnZoomChanged += OnZoomChanged;
        }

        private void ButtonZoomDecrease_Click(object sender, RoutedEventArgs e)
        {
            Actions.ZoomDecrease();
        }

        private void ButtonZoomReset_Click(object sender, RoutedEventArgs e)
        {
            Actions.ZoomReset();
        }

        private void ButtonZoomIncrease_Click(object sender, RoutedEventArgs e)
        {
            Actions.ZoomIncrease();
        }

        private void SliderZoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Globals.pageCanvasRef.SetCanvasZoom(SliderZoom.Value);
        }

        public void OnZoomChanged(object sender, EventArgs e)
        {
            double canvasZoom = Globals.pageCanvasRef.GetCanvasZoom();
            SliderZoom.Value = canvasZoom;
            LabelZoom.Content = $"{canvasZoom}%";
        }

    }
}
