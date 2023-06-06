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
            // Установка константных значений и обработчика события изменения коэффициента приближения холста
            SliderZoom.Minimum = PageCanvas.ZOOM_PERCENT_MIN;
            SliderZoom.Maximum = PageCanvas.ZOOM_PERCENT_MAX;
            SliderZoom.TickFrequency = PageCanvas.ZOOM_PERCENT_STEP;
            SliderZoom.Value = PageCanvas.ZOOM_PERCENT_DEFAULT;
            Globals.pageCanvasRef.OnZoomChanged += OnZoomChanged;
        }

        // Методы изменения коэффициента приближения с помощью кнопок и ползунка, а также метод сброса коэффициента до стандартного значения
        private void DecreaseZoomBy25(object sender, RoutedEventArgs e)
        {
            Actions.ZoomDecrease();
        }

        private void ResetToDefaultZoom(object sender, RoutedEventArgs e)
        {
            Actions.ZoomReset();
        }

        private void IncreaseZoomBy25(object sender, RoutedEventArgs e)
        {
            Actions.ZoomIncrease();
        }

        private void ChangeZoomSlider(object sender, RoutedPropertyChangedEventArgs<double> e)
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
