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
using System.Windows.Shapes;

namespace prdx_graphics_editor.modules.canvas
{
    /// <summary>
    /// Логика взаимодействия для WindowCanvasSize.xaml
    /// </summary>
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
