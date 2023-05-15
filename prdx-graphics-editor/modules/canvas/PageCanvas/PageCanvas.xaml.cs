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
        Polyline currentLine;
        public Shape lastShape;
        public event EventHandler OnFiguresChanged;
        string[] CanvasToolDescription;
        public Line lastLine;
        public Polygon lastTriangle;

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
            currentLine = null;
            lastShape = null;
            lastLine = new Line();

            CanvasToolDescription = new string[]
            {
                "Карандаш",
                "Кисть",
                "Ластик",
                "Выделение",
                "Заливка",
                "Прямоугольник",
                "Круг",
                "Треугольник",
                "Прямая",
                "Стрелка"
            };

        //this.figureDrawer = new Rectangle();
        //figureDrawer.Fill = new SolidColorBrush(Colors.Transparent);
        //figureDrawer.Stroke = new SolidColorBrush(Colors.Black);
        //double[] drawerDashes = { 10, 5 };
        //figureDrawer.StrokeDashArray = new DoubleCollection(drawerDashes);

        Canvas.SetLeft(selectionRectangle, 0);
            Canvas.SetTop(selectionRectangle, 0);
            Canvas.SetZIndex(selectionRectangle, 2);
            mainCanvas.Children.Add(selectionRectangle);

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
            mainCanvas.Children.Clear();
            ImageBrush brush = new ImageBrush();
            
            mainCanvas.Background = new SolidColorBrush(Colors.White);
            this.selectionPoints = (new Point(0, 0), new Point(0, 0));
            Canvas.SetLeft(selectionRectangle, 0);
            Canvas.SetTop(selectionRectangle, 0);
            mainCanvas.Children.Add(selectionRectangle);

            Globals.currentFile = null;
            Globals.changeHistoryBefore.Clear();
            Globals.changeHistoryAfter.Clear();
            OnFiguresChanged.Invoke(this, null);
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

                double x = Math.Min(selectionPoints.Item1.X, selectionPoints.Item2.X);
                double y = Math.Min(selectionPoints.Item1.Y, selectionPoints.Item2.Y);
                string currentToolDescription = CanvasToolDescription[(int)activeTool];
                AddNewFigure(rectangle, currentToolDescription, new Point(x, y));
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
            if (activeTool <= CanvasToolType.ToolEraser)
            {
                OnCanvasMouseMoveDraw(sender, e, currentLine);
            }
            else
            {
                OnCanvasMouseMoveFigureMode(sender, e);
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
                
                if (activeTool == CanvasToolType.ToolSquare || activeTool == CanvasToolType.ToolCircle)
                {
                    Canvas.SetLeft(lastShape, Math.Min(selectionPoints.Item1.X, selectionPoints.Item2.X));
                    Canvas.SetTop(lastShape, Math.Min(selectionPoints.Item1.Y, selectionPoints.Item2.Y));
                    lastShape.Width = width;
                    lastShape.Height = height;

                    Color previewColor = Color.FromArgb(127, Globals.applicationSettings.primaryColor.R, Globals.applicationSettings.primaryColor.G, Globals.applicationSettings.primaryColor.B);
                    lastShape.Fill = new SolidColorBrush(previewColor);
                }
                if (activeTool == CanvasToolType.ToolTriangle)
                {
                    Color previewColor = Color.FromArgb(127, Globals.applicationSettings.primaryColor.R, Globals.applicationSettings.primaryColor.G, Globals.applicationSettings.primaryColor.B);
                }
                if (activeTool == CanvasToolType.ToolLine)
                {
                    lastLine.X2 = GetCanvasPosition(sender, e).X;
                    lastLine.Y2 = GetCanvasPosition(sender, e).Y;

                    Color previewColor = Color.FromArgb(127, Globals.applicationSettings.primaryColor.R, Globals.applicationSettings.primaryColor.G, Globals.applicationSettings.primaryColor.B);
                    lastLine.Stroke = new SolidColorBrush(previewColor);
                }
                if (activeTool == CanvasToolType.ToolTriangle)
                {
                    PointCollection updatedPointCollection = new PointCollection();
                    double xMiddle = Math.Min(GetCanvasPosition(sender, e).X, lastTriangle.Points[0].X) + Math.Abs(GetCanvasPosition(sender, e).X - lastTriangle.Points[0].X) / 2;

                    Point point1 = lastTriangle.Points[0];
                    Point point2 = new Point(GetCanvasPosition(sender, e).X, lastTriangle.Points[1].Y);
                    Point point3 = new Point(xMiddle, GetCanvasPosition(sender, e).Y);

                    lastTriangle.Points.Clear();
                    lastTriangle.Points.Add(point1);
                    lastTriangle.Points.Add(point2);
                    lastTriangle.Points.Add(point3);

                    Color previewColor = Color.FromArgb(127, Globals.applicationSettings.primaryColor.R, Globals.applicationSettings.primaryColor.G, Globals.applicationSettings.primaryColor.B);
                    lastTriangle.Fill = new SolidColorBrush(previewColor);
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
            if (this.activeTool >= CanvasToolType.ToolSquare)
            {
                switch (this.activeTool)
                {
                    case CanvasToolType.ToolSquare:
                        lastShape = new Rectangle();
                        Canvas.SetLeft(lastShape, selectionPoints.Item1.X);
                        Canvas.SetTop(lastShape, selectionPoints.Item1.Y);
                        mainCanvas.Children.Add(lastShape);
                        break;
                    case CanvasToolType.ToolCircle:
                        lastShape = new Ellipse();
                        Canvas.SetLeft(lastShape, selectionPoints.Item1.X);
                        Canvas.SetTop(lastShape, selectionPoints.Item1.Y);
                        mainCanvas.Children.Add(lastShape);
                        break;
                    case CanvasToolType.ToolTriangle:
                        lastTriangle = new Polygon();
                        mainCanvas.Children.Add(lastTriangle);
                        PointCollection trianglePointCollection = new PointCollection();
                        Point start = GetCanvasPosition(sender, e);
                        trianglePointCollection.Add(start);
                        trianglePointCollection.Add(start);
                        trianglePointCollection.Add(start);
                        lastTriangle.Points = trianglePointCollection;
                        break;
                    case CanvasToolType.ToolLine:
                        lastLine = new Line();
                        mainCanvas.Children.Add(lastLine);
                        lastLine.X1 = GetCanvasPosition(sender, e).X;
                        lastLine.Y1 = GetCanvasPosition(sender, e).Y;
                        break;
                    case CanvasToolType.ToolArrow:
                        //arrow
                        break;
                }
            }

            if (this.activeTool < CanvasToolType.ToolSelect)
            {
                canvasPointer = GetCanvasPosition(sender, e);
                Canvas canvas = (Canvas)sender;

                if (e.GetPosition(this).X > this.Width || e.GetPosition(this).Y > this.Height)
                {
                    return;
                }

                currentLine = new Polyline();
                string currentToolDescription = CanvasToolDescription[(int)activeTool];
                AddNewFigure(currentLine, currentToolDescription, new Point(0, 0));

                canvasPointer = GetCanvasPosition(sender, e);
                currentLine.Points.Add(canvasPointer);

                switch (activeTool)
                {
                    case CanvasToolType.ToolPencil:
                        currentLine.StrokeThickness = 1;
                        currentLine.Stroke = new SolidColorBrush(Globals.applicationSettings.primaryColor);
                        break;
                    case CanvasToolType.ToolBrush:
                        currentLine.StrokeThickness = 10;
                        currentLine.Stroke = new SolidColorBrush(Globals.applicationSettings.primaryColor);
                        //currentLine.Stroke = new LinearGradientBrush(Globals.applicationSettings.primaryColor, Colors.Transparent);
                        //currentLine.StrokeStartLineCap = PenLineCap.Round;
                        //currentLine.StrokeEndLineCap = PenLineCap.Round;
                        break;
                    case CanvasToolType.ToolEraser:
                        currentLine.StrokeThickness = 10;
                        currentLine.Stroke = new SolidColorBrush(Globals.applicationSettings.secondaryColor);
                        break;
                }


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
                lastShape = currentLine;
                currentLine = null;
            }

            if (this.activeTool >= CanvasToolType.ToolSquare && this.activeTool <= CanvasToolType.ToolCircle)
            {
                lastShape.Fill = new SolidColorBrush(Globals.applicationSettings.primaryColor);

                string currentToolDescription = CanvasToolDescription[(int)activeTool];
                double x = Math.Min(selectionPoints.Item1.X, selectionPoints.Item2.X);
                double y = Math.Min(selectionPoints.Item1.Y, selectionPoints.Item2.Y);
                Point topLeftPoint = new Point(x, y);
                mainCanvas.Children.Remove(lastShape);
                AddNewFigure(lastShape, currentToolDescription, topLeftPoint);
            }
            else if (this.activeTool == CanvasToolType.ToolLine)
            {
                lastLine.Stroke = new SolidColorBrush(Globals.applicationSettings.primaryColor);

                string currentToolDescription = CanvasToolDescription[(int)activeTool];
                double x = Math.Min(selectionPoints.Item1.X, selectionPoints.Item2.X);
                double y = Math.Min(selectionPoints.Item1.Y, selectionPoints.Item2.Y);
                Point topLeftPoint = new Point(x, y);
                mainCanvas.Children.Remove(lastLine);
                AddNewFigure(lastLine, currentToolDescription, topLeftPoint);
            }

            else if (this.activeTool == CanvasToolType.ToolTriangle)
            {
                lastTriangle.Fill = new SolidColorBrush(Globals.applicationSettings.primaryColor);

                string currentToolDescription = CanvasToolDescription[(int)activeTool];
                double x = Math.Min(selectionPoints.Item1.X, selectionPoints.Item2.X);
                double y = Math.Min(selectionPoints.Item1.Y, selectionPoints.Item2.Y);
                Point topLeftPoint = new Point(x, y);
                mainCanvas.Children.Remove(lastTriangle);
                AddNewFigure(lastTriangle, currentToolDescription, topLeftPoint);
            }

            if (this.activeTool != CanvasToolType.ToolSelect)
            {
                this.isEmpty = false;
            }
            IsMouseDown = false;
            //SerializeToXML(canvas1, AppDomain.CurrentDomain.BaseDirectory + "/cnv.xml");
        }

        public void ExportProject(string exportType, string filename)
        {
            Globals.currentFile = filename;
            selectionRectangle.Visibility = Visibility.Hidden;
            Rect rect = new Rect(0, 0, mainCanvas.ActualWidth, mainCanvas.ActualHeight);

            RenderTargetBitmap renderBmp = new RenderTargetBitmap(
                (int)rect.Right,
                (int)rect.Bottom,
                96d,
                96d,
                PixelFormats.Default);

            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext ctx = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(mainCanvas);
                ctx.DrawRectangle(vb, null,
                    new Rect(new Point(mainCanvas.Margin.Left, mainCanvas.Margin.Top), new Point(mainCanvas.ActualWidth + mainCanvas.Margin.Left, mainCanvas.ActualHeight + mainCanvas.Margin.Top)));
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
                    case ".jpg":
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

                mainCanvas.Width = img.PixelWidth;
                mainCanvas.Height = img.PixelHeight;
                mainCanvas.Background = brush;
        }

        public void AddNewFigure(Shape figure, string operationDescription, Point position,  bool isRedo = false)
        {
            mainCanvas.Children.Add(figure);

            if (!(figure is Line || figure is Polygon))
            {
                Canvas.SetLeft(figure, position.X);
                Canvas.SetTop(figure, position.Y);
            }
            Globals.changeHistoryBefore.Push((figure, operationDescription, position));
            if (!isRedo)
            {
                Globals.changeHistoryAfter.Clear();
            }
            if (OnFiguresChanged != null)
            {
                OnFiguresChanged.Invoke(this, null);
            }
        }

        public void RemoveLastFigure()
        {
            if (Globals.changeHistoryBefore.Count == 0)
            {
                return;
            }
            (Shape figure, string operationDescription, Point position) = Globals.changeHistoryBefore.Pop();
            
            selectionRectangle.Width = 0;
            selectionRectangle.Height = 0;

            mainCanvas.Children.Remove(figure);
            Globals.changeHistoryAfter.Push((figure, operationDescription, position));

            if (OnFiguresChanged != null)
            {
                OnFiguresChanged.Invoke(this, null);
            }
        }
        public void ReturnLastFigure()
        {
            if (Globals.changeHistoryAfter.Count == 0)
            {
                return;
            }
            (Shape figure, string operationDescription, Point position) = Globals.changeHistoryAfter.Pop();

            AddNewFigure(figure, operationDescription, position, true);

            if (OnFiguresChanged != null)
            {
                OnFiguresChanged.Invoke(this, null);
            }
        }

        public void SelectClear()
        {
            selectionRectangle.Width = 0;
            selectionRectangle.Height = 0;
        }
        public void SelectAll()
        {
            Canvas.SetTop(selectionRectangle, 0);
            Canvas.SetLeft(selectionRectangle, 0);
            selectionRectangle.Width = mainCanvas.ActualWidth;
            selectionRectangle.Height = mainCanvas.ActualHeight;
        }
    }
}

