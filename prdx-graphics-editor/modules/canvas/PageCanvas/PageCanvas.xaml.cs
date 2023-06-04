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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using prdx_graphics_editor.modules.utils;
using System.Windows.Markup;
using System.Xml;
using System.IO;
using System.Runtime.ExceptionServices;

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
        ToolArrow,
        ToolHand,
        ToolRotate,
        ToolSize
    }


    public partial class PageCanvas : Page
    {
        CanvasToolType activeTool;
        Rectangle selectionRectangle;
        public bool isEmpty;
        public event EventHandler OnFiguresChanged;
        string[] CanvasToolDescription;
        Point handOffset;
        bool isPlacingFigure = false;

        // Координаты точек области прямоугольного выделения
        (Point, Point) selectionPoints;
        
        // Указатели на текущие редактируемые элементы каждого типа
        Polyline currentLine;
        public Shape lastShape;

        // Константы, определяющие форму и размер стрелок
        const double ARROW_BODY_WIDTH = 20;         // ширина тела стрелки
        const double ARROW_BODY_LENGTH_MIN = 5;     // минимальная длина тела стрелки
        const double ARROW_CAP_WIDTH = 60;          // ширина наконечника стрелки
        const double ARROW_CAP_LENGTH = 30;         // длина наконечника стрелки

        MouseButtonEventHandler OnFigureMouseDown;
        MouseButtonEventHandler OnFigureMouseUp;

        public PageCanvas()
        {
            InitializeComponent();

            isEmpty = true;
            activeTool = Globals.applicationSettings.activeTool;

            // Инициализация прямоугольника выделения
            selectionPoints = (new Point(0, 0), new Point(0, 0));
            selectionRectangle = new Rectangle();
            selectionRectangle.Fill = new SolidColorBrush(Colors.Transparent);
            selectionRectangle.Stroke = new SolidColorBrush(Colors.Black);
            double[] selectionDashes = { 10, 5 };
            selectionRectangle.StrokeDashArray = new DoubleCollection(selectionDashes);

            selectionRectangle.MouseDown += new MouseButtonEventHandler(FigureMouseDown);
            selectionRectangle.MouseUp += new MouseButtonEventHandler(FigureMouseUp);

            Canvas.SetLeft(selectionRectangle, 0);
            Canvas.SetTop(selectionRectangle, 0);
            Canvas.SetZIndex(selectionRectangle, 2);
            mainCanvas.Children.Add(selectionRectangle);
            
            Globals.pageCanvasRef = this;
            Globals.currentFile = null;
            currentLine = null;
            lastShape = null;

            CanvasToolDescription = new string[]
            {
                "Карандаш",
                "Кисть",
                "Ластик",
                "Выделение",
                "Заливка",
                "Прямоугольник",
                "Эллипс",
                "Треугольник",
                "Прямая",
                "Стрелка",
                "Рука",
                "Вращение",
                "Масштаб"
            };

            OnFigureMouseDown = new MouseButtonEventHandler(FigureMouseDown);
            OnFigureMouseUp = new MouseButtonEventHandler(FigureMouseUp);
        }

        private void FigureMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (true || sender == lastShape)
            {
                UIElement trueSender = sender as UIElement;

                // Захватываем фокус мыши
                Mouse.Capture(trueSender);

                handOffset = e.GetPosition(trueSender);
            
                if (sender is Polygon && (sender as Polygon).Points.ToList().Count == 7)
                {
                    Point cursor = e.GetPosition(mainCanvas);
                    handOffset = new Point(cursor.X - Canvas.GetLeft(trueSender), cursor.Y - Canvas.GetTop(trueSender));
                }
            }
        }

        // Обработчик события отпускания кнопки мыши
        private void FigureMouseUp(object sender, MouseButtonEventArgs e)
        {
            // Освобождаем захваченную мышь
            Mouse.Capture(null);
            handOffset = new Point(0, 0);
        }

        public void SerializeToXML(Canvas canvas, string filename)
        {
            string mystrXAML = XamlWriter.Save(canvas);
            FileStream filestream = File.Create(filename);
            StreamWriter streamwriter = new StreamWriter(filestream);
            streamwriter.Write(mystrXAML);
            streamwriter.Close();
            filestream.Close();
        }
        public void DeserializeFromXML(string filename)
        {
            ResetCanvas();
            FileStream filestream = File.Open(filename, FileMode.Open);
            StreamReader streamreader = new StreamReader(filestream);
            string mystrXAML = streamreader.ReadToEnd();

            Canvas deserializedCanvas = XamlReader.Parse(mystrXAML) as Canvas;

            mainCanvas.Children.Clear();
            var childrenCopy = deserializedCanvas.Children.Cast<UIElement>().ToList();
            
            // Поэлементный перенос дочерних элементов из десериализованного холста в mainCanvas
            foreach (var child in childrenCopy)
            {
                deserializedCanvas.Children.Remove(child);
                mainCanvas.Children.Add(child);
            }

            streamreader.Close();
            filestream.Close();
        }

        public void SetActiveTool(CanvasToolType toolType)
        {
            activeTool = toolType;
            Globals.applicationSettings.activeTool = activeTool;

            if (activeTool == CanvasToolType.ToolHand)
            {
                mainCanvas.Cursor = Cursors.Hand;
            }
            else if (activeTool < CanvasToolType.ToolSelect || activeTool > CanvasToolType.ToolFill)
            {
                selectionRectangle.Width = 0;
                selectionRectangle.Height = 0;
            }
        }

        public void ResetCanvas(int width = 800, int height = 800)
        {
            mainCanvas.Children.Clear();
            ImageBrush brush = new ImageBrush();
            Globals.isProjectSaved = true;

            mainCanvas.Width = width;
            mainCanvas.Height = height;
            mainCanvas.Background = new SolidColorBrush(Colors.White);
            selectionPoints = (new Point(0, 0), new Point(0, 0));
            Canvas.SetLeft(selectionRectangle, 0);
            Canvas.SetTop(selectionRectangle, 0);
            mainCanvas.Children.Add(selectionRectangle);

            Globals.changeHistoryBefore.Clear();
            Globals.changeHistoryAfter.Clear();
            OnFiguresChanged.Invoke(this, null);
        }

        public void FillSelection(object sender)
        {
            if (sender is Shape)
            {
                Shape shape = sender as Shape;
                shape.Fill = new SolidColorBrush(Globals.applicationSettings.primaryColor);
                return;
            }
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
            Point newPosition = e.GetPosition(mainCanvas);
            Globals.pageInfoLineRef.SetPointerValues(newPosition);

            if (e.LeftButton == MouseButtonState.Released)
            {
                return;
            }

            if (activeTool == CanvasToolType.ToolHand)
            {
                // Проверяем, захвачена ли мышь
                if (Mouse.Captured is UIElement capturedElement)
                {

                    // Обновляем позицию фигуры на Canvas
                    Canvas.SetLeft(capturedElement, newPosition.X - handOffset.X);
                    Canvas.SetTop(capturedElement, newPosition.Y - handOffset.Y);
                }
            }

            else if (activeTool <= CanvasToolType.ToolEraser)
            {
                OnCanvasMouseMoveDraw(sender, e, currentLine);
            }
            else
            {
                OnCanvasMouseMoveFigureMode(sender, e);
            }
        }

        Point canvasPointer = new Point();
        private void OnCanvasMouseMoveDraw(object sender, MouseEventArgs e, Polyline line)
        {
            Canvas canvas = (Canvas)sender;

            canvasPointer = GetCanvasPosition(sender, e);


            if (e.LeftButton == MouseButtonState.Pressed)
            {
                line.Points.Add(canvasPointer);
            }
        }

        private void OnCanvasMouseMoveFigureMode(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
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

                    Color previewColor = Color.FromArgb(127, Globals.applicationSettings.secondaryColor.R, Globals.applicationSettings.secondaryColor.G, Globals.applicationSettings.secondaryColor.B);
                    lastShape.Fill = new SolidColorBrush(previewColor);
                    lastShape.Stroke = new SolidColorBrush(Colors.Black);
                    double[] selectionDashes = { 10, 5 };
                    lastShape.StrokeDashArray = new DoubleCollection(selectionDashes);
                }
                if (activeTool == CanvasToolType.ToolLine)
                {
                    (lastShape as Line).X2 = GetCanvasPosition(sender, e).X;
                    (lastShape as Line).Y2 = GetCanvasPosition(sender, e).Y;

                    Color previewColor = Color.FromArgb(127, Globals.applicationSettings.secondaryColor.R, Globals.applicationSettings.secondaryColor.G, Globals.applicationSettings.secondaryColor.B);
                    lastShape.Stroke = new SolidColorBrush(previewColor);
                    lastShape.StrokeThickness = Globals.applicationSettings.brushSize;
                }
                if (activeTool == CanvasToolType.ToolArrow)
                {
                    Color previewColor1 = Color.FromArgb(127, Globals.applicationSettings.secondaryColor.R, Globals.applicationSettings.secondaryColor.G, Globals.applicationSettings.secondaryColor.B);
                    Color previewColor2 = Color.FromArgb(127, Globals.applicationSettings.primaryColor.R, Globals.applicationSettings.primaryColor.G, Globals.applicationSettings.primaryColor.B);
                    lastShape.Fill = new SolidColorBrush(previewColor1);
                    lastShape.Stroke = new SolidColorBrush(previewColor2);
                    double deltaX = selectionPoints.Item2.X - selectionPoints.Item1.X;
                    double deltaY = selectionPoints.Item2.Y - selectionPoints.Item1.Y;
                    double rotateAngleRad = Math.Atan(deltaY / deltaX);
                    double rotateAngleDeg = rotateAngleRad * 180 / Math.PI;
                    double arrowFullLength = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
                    double arrowBodyLength = arrowFullLength - ARROW_CAP_LENGTH;
                    arrowBodyLength = (arrowBodyLength > ARROW_BODY_LENGTH_MIN) ? arrowBodyLength : ARROW_BODY_LENGTH_MIN;
                    int directionSwitch = (selectionPoints.Item2.X - selectionPoints.Item1.X >= 0) ? 1 : -1;
                    Polygon lastPolygon = lastShape as Polygon;
                    lastPolygon.Points[2] = new Point(arrowBodyLength * directionSwitch, -ARROW_BODY_WIDTH / 2);
                    lastPolygon.Points[3] = new Point(arrowBodyLength * directionSwitch, -ARROW_CAP_WIDTH / 2);
                    lastPolygon.Points[4] = new Point((arrowBodyLength + ARROW_CAP_LENGTH) * directionSwitch, 0);
                    lastPolygon.Points[5] = new Point(arrowBodyLength * directionSwitch, ARROW_CAP_WIDTH / 2);
                    lastPolygon.Points[6] = new Point(arrowBodyLength * directionSwitch, ARROW_BODY_WIDTH / 2);
                    (lastShape.RenderTransform as RotateTransform).Angle = rotateAngleDeg;
                }
                if (activeTool == CanvasToolType.ToolTriangle)
                {
                    Polygon lastPolygon = lastShape as Polygon;
                    double xMiddle = Math.Min(GetCanvasPosition(sender, e).X, lastPolygon.Points[0].X) + Math.Abs(GetCanvasPosition(sender, e).X - lastPolygon.Points[0].X) / 2;
                    Point point1 = lastPolygon.Points[0];
                    Point point2 = new Point(GetCanvasPosition(sender, e).X, lastPolygon.Points[1].Y);
                    Point point3 = new Point(xMiddle, GetCanvasPosition(sender, e).Y);

                    lastPolygon.Points.Clear();
                    lastPolygon.Points.Add(point1);
                    lastPolygon.Points.Add(point2);
                    lastPolygon.Points.Add(point3);

                    Color previewColor = Color.FromArgb(127, Globals.applicationSettings.secondaryColor.R, Globals.applicationSettings.secondaryColor.G, Globals.applicationSettings.secondaryColor.B);
                    lastShape.Fill = new SolidColorBrush(previewColor);
                    lastShape.Stroke = new SolidColorBrush(Colors.Black);
                    double[] selectionDashes = { 10, 5 };
                    lastShape.StrokeDashArray = new DoubleCollection(selectionDashes);
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
            if (activeTool == CanvasToolType.ToolHand)
            {
                return;
            }

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
                isPlacingFigure = true;
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
                        lastShape = new Polygon();
                        mainCanvas.Children.Add(lastShape);
                        PointCollection trianglePointCollection = new PointCollection();
                        Point start = GetCanvasPosition(sender, e);
                        trianglePointCollection.Add(start);
                        trianglePointCollection.Add(start);
                        trianglePointCollection.Add(start);
                        (lastShape as Polygon).Points = trianglePointCollection;
                        break;
                    case CanvasToolType.ToolLine:
                        lastShape = new Line();
                        mainCanvas.Children.Add(lastShape);
                        (lastShape as Line).X1 = GetCanvasPosition(sender, e).X;
                        (lastShape as Line).Y1 = GetCanvasPosition(sender, e).Y;
                        break;
                    case CanvasToolType.ToolArrow:
                        lastShape = new Polygon();
                        Point position = GetCanvasPosition(sender, e);
                        Canvas.SetLeft(lastShape, position.X);
                        Canvas.SetTop(lastShape, position.Y);
                        mainCanvas.Children.Add(lastShape);
                        Polygon lastPolygon = lastShape as Polygon;
                        lastPolygon.RenderTransform = new RotateTransform();
                        lastPolygon.Points.Add(new Point(0, ARROW_BODY_WIDTH / 2));
                        lastPolygon.Points.Add(new Point(0, -ARROW_BODY_WIDTH / 2));
                        lastPolygon.Points.Add(new Point(ARROW_BODY_LENGTH_MIN, -ARROW_BODY_WIDTH / 2));
                        lastPolygon.Points.Add(new Point(ARROW_BODY_LENGTH_MIN, -ARROW_CAP_WIDTH / 2));
                        lastPolygon.Points.Add(new Point(ARROW_BODY_LENGTH_MIN + ARROW_CAP_LENGTH, 0));
                        lastPolygon.Points.Add(new Point(ARROW_BODY_LENGTH_MIN, ARROW_CAP_WIDTH / 2));
                        lastPolygon.Points.Add(new Point(ARROW_BODY_LENGTH_MIN, ARROW_BODY_WIDTH / 2));
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
                currentLine.StrokeLineJoin = PenLineJoin.Round;
                currentLine.StrokeStartLineCap = PenLineCap.Round;
                currentLine.StrokeEndLineCap = PenLineCap.Round;
                string currentToolDescription = CanvasToolDescription[(int)activeTool];
                AddNewFigure(currentLine, currentToolDescription, new Point(0, 0));

                canvasPointer = GetCanvasPosition(sender, e);
                currentLine.Points.Add(canvasPointer);

                CheckFigureSettings(currentLine);

                switch (activeTool)
                {
                    case CanvasToolType.ToolPencil:
                        currentLine.Stroke = new SolidColorBrush(Globals.applicationSettings.primaryColor);
                        break;
                    case CanvasToolType.ToolBrush:
                        currentLine.Stroke = new SolidColorBrush(Globals.applicationSettings.primaryColor);
                        currentLine.SnapsToDevicePixels = false;
                        BlurEffect blurEffect = new BlurEffect();
                        blurEffect.Radius = 10;
                        currentLine.Effect = blurEffect;
                        break;
                    case CanvasToolType.ToolEraser:
                        currentLine.Stroke = new SolidColorBrush(Globals.applicationSettings.secondaryColor);
                        break;
                }
            }

            if (this.activeTool == CanvasToolType.ToolFill)
            {
                FillSelection(sender);
            }
        }

        void CheckFigureSettings(Shape targetShape, bool arrowCase = false)
        {
            if (targetShape.GetType() == typeof(Polyline) && !(arrowCase))
            {
                targetShape.StrokeThickness = Globals.applicationSettings.brushSize;
                return;
            }
            else if (targetShape.GetType() == typeof(Line))
            {
                (targetShape as Line).StrokeThickness = Globals.applicationSettings.brushSize;
                (targetShape as Line).Stroke = new SolidColorBrush(Globals.applicationSettings.secondaryColor);
                return;
            }

            targetShape.StrokeDashArray = null;
            if (Globals.applicationSettings.enableFigureFill)
            {
                targetShape.Fill = new SolidColorBrush(Globals.applicationSettings.secondaryColor);
            }
            else
            {
                targetShape.Fill = Brushes.Transparent;
            }

            if (Globals.applicationSettings.enableFigureBorder)
            {
                targetShape.StrokeThickness = Globals.applicationSettings.borderSize;
                targetShape.Stroke = new SolidColorBrush(Globals.applicationSettings.primaryColor);
            }
            else
            {
                targetShape.Stroke = null;
            }
        }

        private void OnCanvasMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (activeTool != CanvasToolType.ToolSelect && activeTool != CanvasToolType.ToolHand)
            {
                Globals.isProjectSaved = false;
            }

            if(activeTool < CanvasToolType.ToolSelect || activeTool > CanvasToolType.ToolFill)
            {
                selectionRectangle.Width = 0;
                selectionRectangle.Height = 0;
            }

            if (this.activeTool < CanvasToolType.ToolSelect)
            {
                if (currentLine.Points.Count < 2)
                {
                    
                    Ellipse brushPoint = new Ellipse();
                    brushPoint.Fill = currentLine.Stroke;
                    brushPoint.Width = currentLine.StrokeThickness;
                    brushPoint.Height = currentLine.StrokeThickness;
                    Point topLeftPoint = new Point(currentLine.Points[0].X - currentLine.StrokeThickness / 2, currentLine.Points[0].Y - currentLine.StrokeThickness / 2);
                    Canvas.SetLeft(brushPoint, topLeftPoint.X);
                    Canvas.SetTop(brushPoint, topLeftPoint.Y);

                    RemoveLastFigure();
                    AddNewFigure(brushPoint, CanvasToolDescription[(int)activeTool], topLeftPoint);
                }
                lastShape = currentLine;
                currentLine = null;
            }
            //else if (!(lastShape is Polygon) && !(lastShape.Width > 0 && lastShape.Height > 0))
            //else if (Canvas.GetRight(lastShape) + Canvas.GetLeft(lastShape) >= mainCanvas.Width || Canvas.GetTop(lastShape) + Canvas.GetBottom(lastShape) >= mainCanvas.Height)
            //{
            //    mainCanvas.Children.Remove(lastShape);
            //    return;
            //}

            if (this.activeTool >= CanvasToolType.ToolSquare && this.activeTool <= CanvasToolType.ToolCircle)
            {
                CheckFigureSettings(lastShape);

                string currentToolDescription = CanvasToolDescription[(int)activeTool];
                double x = Math.Min(selectionPoints.Item1.X, selectionPoints.Item2.X);
                double y = Math.Min(selectionPoints.Item1.Y, selectionPoints.Item2.Y);
                Point topLeftPoint = new Point(x, y);
                mainCanvas.Children.Remove(lastShape);
                AddNewFigure(lastShape, currentToolDescription, topLeftPoint);
            }
            else if (this.activeTool == CanvasToolType.ToolLine)
            {
                CheckFigureSettings(lastShape);

                string currentToolDescription = CanvasToolDescription[(int)activeTool];
                Point topLeftPoint = new Point(0,0);
                mainCanvas.Children.Remove(lastShape);
                AddNewFigure(lastShape, currentToolDescription, topLeftPoint);
            }

            else if (this.activeTool == CanvasToolType.ToolTriangle)
            {
                CheckFigureSettings(lastShape);

                string currentToolDescription = CanvasToolDescription[(int)activeTool];
                double x = Math.Min(selectionPoints.Item1.X, selectionPoints.Item2.X);
                double y = Math.Min(selectionPoints.Item1.Y, selectionPoints.Item2.Y);
                Point topLeftPoint = new Point(x, y);
                mainCanvas.Children.Remove(lastShape);
                AddNewFigure(lastShape, currentToolDescription, topLeftPoint);
            }
            else if (this.activeTool == CanvasToolType.ToolArrow)
            {
                CheckFigureSettings(lastShape, true);

                string currentToolDescription = CanvasToolDescription[(int)activeTool];
                double x = Math.Min(selectionPoints.Item1.X, selectionPoints.Item2.X);
                double y = Math.Min(selectionPoints.Item1.Y, selectionPoints.Item2.Y);
                Point topLeftPoint = new Point(x, y);
                mainCanvas.Children.Remove(lastShape);
                AddNewFigure(lastShape, currentToolDescription, topLeftPoint);
            }

            if (this.activeTool != CanvasToolType.ToolSelect)
            {
                this.isEmpty = false;
            }
            if (activeTool < CanvasToolType.ToolHand)
            {
                lastShape.MouseDown += OnFigureMouseDown;
                lastShape.MouseUp += OnFigureMouseUp;
            }
            isPlacingFigure = false;
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
            ResetCanvas();
            ImageBrush brush = new ImageBrush();
            BitmapImage img = new BitmapImage(new Uri(@filename, UriKind.Relative));
            brush.ImageSource = img;

            mainCanvas.Width = img.PixelWidth;
            mainCanvas.Height = img.PixelHeight;
            mainCanvas.Background = brush;
        }

        public void AddNewFigure(Shape figure, string operationDescription, Point position,  bool isRedo = false)
        {
            mainCanvas.Children.Add(figure);

            if (!(figure is Polygon))
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
            lastShape = figure;
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

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released && activeTool >= CanvasToolType.ToolSquare && isPlacingFigure)
            {
                OnCanvasMouseUp(sender, null);
            }
        }
        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            Globals.pageInfoLineRef.SetPointerValues(null);
        }






        private Double zoomMax = 5;
        private Double zoomMin = 0.5;
        private Double zoomSpeed = 0.001;
        private Double zoom = 1;
        //private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        //{
        //    zoom += zoomSpeed * e.Delta; // Ajust zooming speed (e.Delta = Mouse spin value )
        //    if (zoom < zoomMin) { zoom = zoomMin; } // Limit Min Scale
        //    if (zoom > zoomMax) { zoom = zoomMax; } // Limit Max Scale

        //    Point mousePos = e.GetPosition(mainCanvas);

        //    if (zoom > 1)
        //    {
        //        mainCanvas.RenderTransform = new ScaleTransform(zoom, zoom, mousePos.X, mousePos.Y); // transform Canvas size from mouse position
        //    }
        //    else
        //    {
        //        mainCanvas.RenderTransform = new ScaleTransform(zoom, zoom); // transform Canvas size
        //    }
        //}

        public void PasteClipboard()
        {
            IDataObject myDataObject = Clipboard.GetDataObject();
            string[] files = (string[])myDataObject.GetData(DataFormats.FileDrop);

            if (files != null)
            {
                try
                {
                    var imageSource = new BitmapImage(new Uri(files[0], UriKind.Absolute));
                    Image image = new Image();
                    Canvas.SetLeft(image, 0);
                    Canvas.SetTop(image, 0);
                    image.MouseDown += new MouseButtonEventHandler(FigureMouseDown);
                    image.MouseUp += new MouseButtonEventHandler(FigureMouseUp);
                    mainCanvas.Children.Add(image);
                    image.Source = imageSource;
                }
                catch { }
            }
        }

        public void CopyToClipboard()
        {
            //copy to clipboard
        }
        public void CutToClipboard()
        {
            //cut to clipboard
        }

        public void Rastrize()
        {
            //rastrize
        }
    }
}

