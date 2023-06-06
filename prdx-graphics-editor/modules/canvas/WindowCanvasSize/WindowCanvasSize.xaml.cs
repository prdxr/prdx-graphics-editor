using System;
using System.Windows;
using System.Windows.Controls;

namespace prdx_graphics_editor.modules.canvas
{

    public partial class WindowCanvasSize : Window
    {
        public static void ChangeCanvasSize(Canvas canvas)
        {
            WindowCanvasSize window = new WindowCanvasSize();
            window.canvasRef = canvas;
            window.TextBoxWidth.Text = window.canvasRef.Width.ToString("F0");
            window.TextBoxHeight.Text = window.canvasRef.Height.ToString("F0");
            window.ShowDialog();
        }

        private Canvas canvasRef;

        public WindowCanvasSize()
        {
            InitializeComponent();
        }

        private void ButtonApply_Click(object sender, RoutedEventArgs e)
        {
            canvasRef.Width = Int32.Parse(TextBoxWidth.Text);
            canvasRef.Height = Int32.Parse(TextBoxHeight.Text);
            Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
