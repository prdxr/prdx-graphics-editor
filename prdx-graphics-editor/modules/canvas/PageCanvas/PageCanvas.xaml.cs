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
using System.Windows.Markup;
using System.Xml;
using System.IO;

namespace prdx_graphics_editor.modules.canvas.PageCanvas
{
    /// <summary>
    /// Логика взаимодействия для MainCanvas.xaml
    /// </summary>
    public enum CanvasToolType
    {
        ToolPencil,
        ToolBrush,
        ToolEraser,
        ToolSelect,
        ToolFill,
        ToolSquare,
        ToolCircle,
        ToolTriangle,
        ToolLine,
        ToolArrow
    }

    public partial class PageCanvas : Page
    {
        bool IsMouseDown;
        CanvasToolType activeTool;
        (Point, Point) selectionPoints;
        Rectangle selectionRectangle;
        public bool isEmpty;
        Polyline currLine;
        public Shape lastShape;




        public static void SerializeToXML(Canvas canvas, string filename)
        {
            string mystrXAML = XamlWriter.Save(canvas);
            FileStream filestream = File.Create(filename);
            StreamWriter streamwriter = new StreamWriter(filestream);
            streamwriter.Write(mystrXAML);
            streamwriter.Close();
            filestream.Close();
        }


        public PageCanvas()
        {
            InitializeComponent();

            this.isEmpty = true;
            this.activeTool = Globals.applicationSettings.activeTool;
            this.selectionPoints = (new Point(0, 0), new Point(0, 0));
            this.selectionRectangle = new Rectangle();
            selectionRectangle.Fill = new SolidColorBrush(Colors.Transparent);
            selectionRectangle.Stroke = new SolidColorBrush(Colors.Black);
            double[] selectionDashes = { 10, 5 };
            selectionRectangle.StrokeDashArray = new DoubleCollection(selectionDashes);
            Globals.currentFile = null;
            currLine = null;
            lastShape = null;

            //this.figureDrawer = new Rectangle();
            //figureDrawer.Fill = new SolidColorBrush(Colors.Transparent);
            //figureDrawer.Stroke = new SolidColorBrush(Colors.Black);
            //double[] drawerDashes = { 10, 5 };
            //figureDrawer.StrokeDashArray = new DoubleCollection(drawerDashes);

            Canvas.SetLeft(selectionRectangle, 0);
            Canvas.SetTop(selectionRectangle, 0);
            Canvas.SetZIndex(selectionRectangle, 2);
            canvas1.Children.Add(selectionRectangle);

            //Canvas.SetLeft(figureDrawer, 0);
            //Canvas.SetTop(figureDrawer, 0);
            //Canvas.SetZIndex(figureDrawer, 1);
            //canvas1.Children.Add(figureDrawer);

            Globals.pageCanvasRef = this;
        }

        public void SetActiveTool(CanvasToolType toolType)
        {
            this.activeTool = toolType;
            Globals.applicationSettings.activeTool = this.activeTool;

            if (activeTool < CanvasToolType.ToolSelect && activeTool > CanvasToolType.ToolFill)
            {
                selectionRectangle.Width = 0;
                selectionRectangle.Height = 0;
            }
        }

        public void ResetCanvas()
        {
            canvas1.Children.Clear();
            ImageBrush brush = new ImageBrush();
            
            canvas1.Background = new SolidColorBrush(Colors.White);
            this.selectionPoints = (new Point(0, 0), new Point(0, 0));
            Canvas.SetLeft(selectionRectangle, 0);
            Canvas.SetTop(selectionRectangle, 0);
            canvas1.Children.Add(selectionRectangle);

            Globals.currentFile = null;
        }

        //public void DrawLine()
        //{
        //    Line myLine = new Line();
        //    myLine.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
        //    myLine.X1 = 1;
        //    myLine.X2 = 50;
        //    myLine.Y1 = 1;
        //    myLine.Y2 = 50;
        //    myLine.HorizontalAlignment = HorizontalAlignment.Left;
        //    myLine.VerticalAlignment = VerticalAlignment.Center;
        //    myLine.StrokeThickness = 2;
        //    //myLine.AddHandler();
        //    //myLine.MouseMove = this.OnCanvasMouseMove;
        //    canvas1.Children.Add(myLine);
        //}
        public void DrawLine(Polyline currLine)
        {
            //currLine.Points.Add
        }

        public void FillSelection()
        {
            if (selectionRectangle.Width > 0 && selectionRectangle.Height > 0)
            {
                Rectangle rectangle = new Rectangle();
                rectangle.Fill = new SolidColorBrush(Globals.applicationSettings.primaryColor);
                rectangle.Width = selectionRectangle.Width;
                rectangle.Height = selectionRectangle.Height;
                Canvas.SetLeft(rectangle, Math.Min(selectionPoints.Item1.X, selectionPoints.Item2.X)); 
                Canvas.SetTop(rectangle, Math.Min(selectionPoints.Item1.Y, selectionPoints.Item2.Y));
                canvas1.Children.Add(rectangle);
            }
            else
            {
                string boxCaption = "Ошибка инструмента fill";
                string boxText = "Инструмент fill нельзя применить без активного выделения. Сначала воспользуйтесь инструментом select";
                MessageBoxButton boxButtons = MessageBoxButton.OK;
                MessageBoxImage boxIcon = MessageBoxImage.Warning;
                MessageBoxResult boxResult = MessageBox.Show(boxText, boxCaption, boxButtons, boxIcon);
            }
        }

        private void OnCanvasMouseMove(object sender, MouseEventArgs e)
        {
            switch (activeTool)
            {
                case CanvasToolType.ToolPencil:
                    OnCanvasMouseMoveDraw(sender, e, currLine);
                    break;
                case CanvasToolType.ToolBrush:
                    OnCanvasMouseMoveDraw(sender, e, currLine);
                    break;
                case CanvasToolType.ToolEraser:
                    OnCanvasMouseMoveDraw(sender, e, currLine);
                    break;
                case CanvasToolType.ToolSelect:
                    OnCanvasMouseMoveFigureMode(sender, e);
                    break;
                case CanvasToolType.ToolSquare:
                    OnCanvasMouseMoveFigureMode(sender, e);
                    break;
            }
        }

        Point canvasPointer = new Point();
        //Point canvasPointer = new Point(canvasGrid.Margin.Left, canvasGrid.Margin.Top);
        private void OnCanvasMouseMoveDraw(object sender, MouseEventArgs e, Polyline line)
        {
            Canvas canvas = (Canvas)sender;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                canvasPointer = GetCanvasPosition(sender, e);
                line.Points.Add(canvasPointer);
            }
        }

        private void OnCanvasMouseMoveFigureMode(object sender, MouseEventArgs e)
        { 
            if (IsMouseDown == true)
            {
                selectionPoints.Item2 = GetCanvasPosition(sender, e);

                double width = Math.Abs(selectionPoints.Item2.X - selectionPoints.Item1.X);
                double height = Math.Abs(selectionPoints.Item2.Y - selectionPoints.Item1.Y);

                Canvas.SetLeft(selectionRectangle, Math.Min(selectionPoints.Item1.X, selectionPoints.Item2.X));
                Canvas.SetTop(selectionRectangle, Math.Min(selectionPoints.Item1.Y, selectionPoints.Item2.Y));

                selectionRectangle.Width = width;
                selectionRectangle.Height = height;

                if (activeTool == CanvasToolType.ToolSelect)
                {
                    selectionRectangle.Fill = new SolidColorBrush(Colors.Transparent);
                }

                if (activeTool == CanvasToolType.ToolSquare)
                {
                    Color previewColor = Color.FromArgb(127, Globals.applicationSettings.primaryColor.R, Globals.applicationSettings.primaryColor.G, Globals.applicationSettings.primaryColor.B);
                    selectionRectangle.Fill = new SolidColorBrush(previewColor);
                }


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
            IsMouseDown = true;
            if (this.activeTool >= CanvasToolType.ToolSelect && this.activeTool != CanvasToolType.ToolFill)
            {
                selectionPoints.Item1 = GetCanvasPosition(sender, e);
                selectionPoints.Item2 = GetCanvasPosition(sender, e);
                selectionRectangle.Width = 0;
                selectionRectangle.Height = 0;
                Canvas.SetLeft(selectionRectangle, selectionPoints.Item1.X);
                Canvas.SetTop(selectionRectangle, selectionPoints.Item1.Y);
            }

            if (this.activeTool < CanvasToolType.ToolSelect)
            {
                canvasPointer = GetCanvasPosition(sender, e);
                Canvas canvas = (Canvas)sender;

                if (e.GetPosition(this).X > this.Width || e.GetPosition(this).Y > this.Height)
                {
                    return;
                }

                currLine = new Polyline();

                canvasPointer = GetCanvasPosition(sender, e);

                switch (activeTool)
                {
                    case CanvasToolType.ToolPencil:
                        currLine.StrokeThickness = 1;
                        currLine.Stroke = new SolidColorBrush(Globals.applicationSettings.primaryColor);
                        break;
                    case CanvasToolType.ToolBrush:
                        currLine.StrokeThickness = 10;
                        currLine.Stroke = new SolidColorBrush(Globals.applicationSettings.primaryColor);
                        break;
                    case CanvasToolType.ToolEraser:
                        currLine.StrokeThickness = 10;
                        currLine.Stroke = new SolidColorBrush(Globals.applicationSettings.secondaryColor);
                        break;
                }

                currLine.Points.Add(canvasPointer);

                canvas.Children.Add(currLine);
            }

            if (this.activeTool == CanvasToolType.ToolFill)
            {
                FillSelection();
            }
        }
        private void OnCanvasMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.activeTool < CanvasToolType.ToolSelect)
            {
                lastShape = currLine;
                currLine = null;
            }

            if (this.activeTool > CanvasToolType.ToolSelect && this.activeTool != CanvasToolType.ToolFill)
            {
                Rectangle figureResult = new Rectangle();
                figureResult.Fill = new SolidColorBrush(Globals.applicationSettings.primaryColor);
                canvas1.Children.Add(figureResult);

                Canvas.SetLeft(figureResult, selectionPoints.Item1.X);
                Canvas.SetTop(figureResult, selectionPoints.Item1.Y);

                figureResult.Width = Math.Abs(selectionPoints.Item2.X - selectionPoints.Item1.X);
                figureResult.Height = Math.Abs(selectionPoints.Item2.Y - selectionPoints.Item1.Y);

                lastShape = figureResult;
                selectionRectangle.Fill = new SolidColorBrush(Colors.Transparent);
            }



            if (this.activeTool != CanvasToolType.ToolSelect)
            {
                this.isEmpty = false;
                Globals.changeHistoryBefore.Enqueue(lastShape);
            }
            IsMouseDown = false;
            //SerializeToXML(canvas1, AppDomain.CurrentDomain.BaseDirectory + "/cnv.xml");
        }

        public void ExportProject(string exportType, string filename)
        {
            Globals.currentFile = filename;
            selectionRectangle.Visibility = Visibility.Hidden;
            Rect rect = new Rect(0, 0, canvas1.ActualWidth, canvas1.ActualHeight);

            RenderTargetBitmap renderBmp = new RenderTargetBitmap(
                (int)rect.Right,
                (int)rect.Bottom,
                96d,
                96d,
                PixelFormats.Default);

            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext ctx = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(canvas1);
                ctx.DrawRectangle(vb, null,
                    new Rect(new Point(canvas1.Margin.Left, canvas1.Margin.Top), new Point(canvas1.ActualWidth + canvas1.Margin.Left, canvas1.ActualHeight + canvas1.Margin.Top)));
            }

            renderBmp.Render(dv);

            using (FileStream fs = new FileStream(filename, FileMode.Create))
            {
                BitmapEncoder encoder;
                switch (exportType)
                {
                    case ".png":
                        encoder = new PngBitmapEncoder();
                        break;
                    case ".jpeg":
                        encoder = new JpegBitmapEncoder();
                        break;
                    case ".bmp":
                        encoder = new BmpBitmapEncoder();
                        break;
                    default:
                        return;
                }

                encoder.Frames.Add(BitmapFrame.Create(renderBmp));
                encoder.Save(fs);
                selectionRectangle.Visibility = Visibility.Visible;
            }
        }

        public void ImportToProject(string filename)
        {
            ImageBrush brush = new ImageBrush();
            BitmapImage img = new BitmapImage(new Uri(@filename, UriKind.Relative));
            //TEMP RESTRICTION TO 800x800
            if (img.PixelWidth > 800 || img.PixelHeight > 800)
            {
                //throw new Exception("TEMP: Максимальное разрешение - 800x800 пикселей");
            }
                brush.ImageSource = img;

                canvas1.Width = img.PixelWidth;
                canvas1.Height = img.PixelHeight;
                canvas1.Background = brush;
        }
    }
}
