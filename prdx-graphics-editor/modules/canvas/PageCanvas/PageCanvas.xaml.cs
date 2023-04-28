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

namespace prdx_graphics_editor.modules.canvas.PageCanvas
{
    /// <summary>
    /// Логика взаимодействия для MainCanvas.xaml
    /// </summary>
    public partial class PageCanvas : Page
    {
        public PageCanvas()
        {
            InitializeComponent();

            System.Windows.Shapes.Rectangle rect;
            rect = new System.Windows.Shapes.Rectangle();
            rect.Stroke = new SolidColorBrush(Colors.Black);
            rect.StrokeThickness = 3;
            rect.Fill = new SolidColorBrush(Colors.White);
            rect.Width = 200;
            rect.Height = 200;
            Canvas.SetLeft(rect, 0);
            Canvas.SetTop(rect, 0);
            canvas1.Children.Add(rect);
        }


        public void DrawLine()
        {
            Line myLine = new Line();
            myLine.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
            myLine.X1 = 1;
            myLine.X2 = 50;
            myLine.Y1 = 1;
            myLine.Y2 = 50;
            myLine.HorizontalAlignment = HorizontalAlignment.Left;
            myLine.VerticalAlignment = VerticalAlignment.Center;
            myLine.StrokeThickness = 2;
            canvas1.Children.Add(myLine);
        }

        Point canvasPointer1 = new Point();
        Point canvasPointer2 = new Point();
        //Point canvasPointer = new Point(canvasGrid.Margin.Left, canvasGrid.Margin.Top);
        private void OnCanvasMouseMove(object sender, MouseEventArgs e)
        {
            Canvas canvas = (Canvas)sender;


            if (e.GetPosition(this).X > this.Width || e.GetPosition(this).Y > this.Height)
            {
                return;
            }

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Line line = new Line();

                canvasPointer2 = GetCanvasPosition(sender, e);

                line.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
                line.X1 = canvasPointer1.X;
                line.Y1 = canvasPointer1.Y;
                line.X2 = canvasPointer2.X;
                line.Y2 = canvasPointer2.Y;

                canvasPointer1 = GetCanvasPosition(sender, e);

                canvas.Children.Add(line);
            }
        }

        public Point GetCanvasPosition(object sender, MouseEventArgs e)
        {
            Canvas canvas = (Canvas)sender;
            Point canvasPointer = e.GetPosition(this);
            canvasPointer.Y -= canvas.Margin.Top;
            canvasPointer.X -= canvas.Margin.Left;
            return canvasPointer;
        }

        private void OnCanvasMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                canvasPointer1 = GetCanvasPosition(sender, e);
        }
    }
}
