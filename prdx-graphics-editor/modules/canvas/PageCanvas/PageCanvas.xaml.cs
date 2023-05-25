﻿using System;
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
        ToolHand
    }


    public partial class PageCanvas : Page
    {
        bool IsMouseDown;
        CanvasToolType activeTool;
        Rectangle selectionRectangle;
        public bool isEmpty;
        public event EventHandler OnFiguresChanged;
        string[] CanvasToolDescription;
        Point handOffset;

        // Координаты точек области прямоугольного выделения
        (Point, Point) selectionPoints;
        
        // Указатели на текущие редактируемые элементы каждого типа
        Polyline currentLine;
        public Shape lastShape;
        public Line lastLine;
        public Polygon lastTriangle;
        Polygon arrowPolygon;

        // Константы, определяющие форму и размер стрелок
        const double ARROW_BODY_WIDTH = 20;         // ширина тела стрелки
        const double ARROW_BODY_LENGTH_MIN = 5;     // минимальная длина тела стрелки
        const double ARROW_CAP_WIDTH = 60;          // ширина наконечника стрелки
        const double ARROW_CAP_LENGTH = 30;         // длина наконечника стрелки

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
            lastLine = new Line();

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
                "Рука"
            };
        }

        private void FigureMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Захватываем фокус мыши
            Mouse.Capture((UIElement)sender);

            // Преобразуем координаты относительно Canvas
            Point startPoint = e.GetPosition(mainCanvas);
            handOffset = e.GetPosition(sender as Shape);

            // Сохраняем начальные координаты фигуры
            //Canvas.SetLeft((UIElement)sender, startPoint.X);
            //Canvas.SetTop((UIElement)sender, startPoint.Y);
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

        public void ResetCanvas()
        {
            mainCanvas.Children.Clear();
            ImageBrush brush = new ImageBrush();

            mainCanvas.Background = new SolidColorBrush(Colors.White);
            selectionPoints = (new Point(0, 0), new Point(0, 0));
            Canvas.SetLeft(selectionRectangle, 0);
            Canvas.SetTop(selectionRectangle, 0);
            mainCanvas.Children.Add(selectionRectangle);

            Globals.currentFile = null;
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

            if (activeTool == CanvasToolType.ToolHand)
            {
                // Проверяем, захвачена ли мышь
                if (Mouse.Captured is UIElement capturedElement)
                {
                    // Преобразуем координаты относительно Canvas
                    Point newPosition = e.GetPosition(mainCanvas);

                    // Обновляем позицию фигуры на Canvas
                    Canvas.SetLeft(capturedElement, newPosition.X - handOffset.X);
                    Canvas.SetTop(capturedElement, newPosition.Y - handOffset.Y);
                }
            }

            if (!IsMouseDown)
            {
                return;
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

                    Color previewColor = Color.FromArgb(127, Globals.applicationSettings.secondaryColor.R, Globals.applicationSettings.secondaryColor.G, Globals.applicationSettings.secondaryColor.B);
                    lastShape.Fill = new SolidColorBrush(previewColor);
                    lastShape.Stroke = new SolidColorBrush(Colors.Black);
                    double[] selectionDashes = { 10, 5 };
                    lastShape.StrokeDashArray = new DoubleCollection(selectionDashes);
                }
                if (activeTool == CanvasToolType.ToolLine)
                {
                    lastLine.X2 = GetCanvasPosition(sender, e).X;
                    lastLine.Y2 = GetCanvasPosition(sender, e).Y;

                    Color previewColor = Color.FromArgb(127, Globals.applicationSettings.primaryColor.R, Globals.applicationSettings.primaryColor.G, Globals.applicationSettings.primaryColor.B);
                    lastLine.Stroke = new SolidColorBrush(previewColor);
                }
                if (activeTool == CanvasToolType.ToolArrow)
                {
                    Color previewColor1 = Color.FromArgb(127, Globals.applicationSettings.secondaryColor.R, Globals.applicationSettings.secondaryColor.G, Globals.applicationSettings.secondaryColor.B);
                    Color previewColor2 = Color.FromArgb(127, Globals.applicationSettings.primaryColor.R, Globals.applicationSettings.primaryColor.G, Globals.applicationSettings.primaryColor.B);
                    arrowPolygon.Fill = new SolidColorBrush(previewColor1);
                    arrowPolygon.Stroke = new SolidColorBrush(previewColor2);
                    double deltaX = selectionPoints.Item2.X - selectionPoints.Item1.X;
                    double deltaY = selectionPoints.Item2.Y - selectionPoints.Item1.Y;
                    double rotateAngleRad = Math.Atan(deltaY / deltaX);
                    double rotateAngleDeg = rotateAngleRad * 180 / Math.PI;
                    double arrowFullLength = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
                    double arrowBodyLength = arrowFullLength - ARROW_CAP_LENGTH;
                    arrowBodyLength = (arrowBodyLength > ARROW_BODY_LENGTH_MIN) ? arrowBodyLength : ARROW_BODY_LENGTH_MIN;
                    int directionSwitch = (selectionPoints.Item2.X - selectionPoints.Item1.X >= 0) ? 1 : -1;
                    arrowPolygon.Points[2] = new Point(arrowBodyLength * directionSwitch, -ARROW_BODY_WIDTH / 2);
                    arrowPolygon.Points[3] = new Point(arrowBodyLength * directionSwitch, -ARROW_CAP_WIDTH / 2);
                    arrowPolygon.Points[4] = new Point((arrowBodyLength + ARROW_CAP_LENGTH) * directionSwitch, 0);
                    arrowPolygon.Points[5] = new Point(arrowBodyLength * directionSwitch, ARROW_CAP_WIDTH / 2);
                    arrowPolygon.Points[6] = new Point(arrowBodyLength * directionSwitch, ARROW_BODY_WIDTH / 2);
                    (arrowPolygon.RenderTransform as RotateTransform).Angle = rotateAngleDeg;
                }
                if (activeTool == CanvasToolType.ToolTriangle)
                {
                    double xMiddle = Math.Min(GetCanvasPosition(sender, e).X, lastTriangle.Points[0].X) + Math.Abs(GetCanvasPosition(sender, e).X - lastTriangle.Points[0].X) / 2;

                    Point point1 = lastTriangle.Points[0];
                    Point point2 = new Point(GetCanvasPosition(sender, e).X, lastTriangle.Points[1].Y);
                    Point point3 = new Point(xMiddle, GetCanvasPosition(sender, e).Y);

                    lastTriangle.Points.Clear();
                    lastTriangle.Points.Add(point1);
                    lastTriangle.Points.Add(point2);
                    lastTriangle.Points.Add(point3);

                    Color previewColor = Color.FromArgb(127, Globals.applicationSettings.secondaryColor.R, Globals.applicationSettings.secondaryColor.G, Globals.applicationSettings.secondaryColor.B);
                    lastTriangle.Fill = new SolidColorBrush(previewColor);
                    lastTriangle.Stroke = new SolidColorBrush(Colors.Black);
                    double[] selectionDashes = { 10, 5 };
                    lastTriangle.StrokeDashArray = new DoubleCollection(selectionDashes);
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
                        arrowPolygon = new Polygon();
                        Point position = GetCanvasPosition(sender, e);
                        Canvas.SetLeft(arrowPolygon, position.X);
                        Canvas.SetTop(arrowPolygon, position.Y);
                        mainCanvas.Children.Add(arrowPolygon);
                        arrowPolygon.RenderTransform = new RotateTransform();
                        arrowPolygon.Points.Add(new Point(0, ARROW_BODY_WIDTH / 2));
                        arrowPolygon.Points.Add(new Point(0, -ARROW_BODY_WIDTH / 2));
                        arrowPolygon.Points.Add(new Point(ARROW_BODY_LENGTH_MIN, -ARROW_BODY_WIDTH / 2));
                        arrowPolygon.Points.Add(new Point(ARROW_BODY_LENGTH_MIN, -ARROW_CAP_WIDTH / 2));
                        arrowPolygon.Points.Add(new Point(ARROW_BODY_LENGTH_MIN + ARROW_CAP_LENGTH, 0));
                        arrowPolygon.Points.Add(new Point(ARROW_BODY_LENGTH_MIN, ARROW_CAP_WIDTH / 2));
                        arrowPolygon.Points.Add(new Point(ARROW_BODY_LENGTH_MIN, ARROW_BODY_WIDTH / 2));
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
            if (!IsMouseDown)
            {
                return;
            }

            if (this.activeTool < CanvasToolType.ToolSelect)
            {
                if (currentLine.Points.Count == 0)
                {
                    
                    Ellipse brushPoint = new Ellipse();
                    brushPoint.Fill = currentLine.Fill;
                    brushPoint.StrokeThickness = currentLine.StrokeThickness;
                    Point topLeftPoint = new Point(currentLine.Points[0].X - currentLine.StrokeThickness / 2, currentLine.Points[0].Y - currentLine.StrokeThickness / 2);
                    Canvas.SetLeft(brushPoint, topLeftPoint.X);
                    Canvas.SetTop(brushPoint, topLeftPoint.Y);

                    RemoveLastFigure();
                    AddNewFigure(brushPoint, CanvasToolDescription[(int)activeTool], topLeftPoint);
                }
                lastShape = currentLine;
                currentLine = null;
            }

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
                CheckFigureSettings(lastLine);

                string currentToolDescription = CanvasToolDescription[(int)activeTool];
                double x = Math.Min(selectionPoints.Item1.X, selectionPoints.Item2.X);
                double y = Math.Min(selectionPoints.Item1.Y, selectionPoints.Item2.Y);
                Point topLeftPoint = new Point(x, y);
                mainCanvas.Children.Remove(lastLine);
                AddNewFigure(lastLine, currentToolDescription, topLeftPoint);
            }

            else if (this.activeTool == CanvasToolType.ToolTriangle)
            {
                CheckFigureSettings(lastTriangle);

                string currentToolDescription = CanvasToolDescription[(int)activeTool];
                double x = Math.Min(selectionPoints.Item1.X, selectionPoints.Item2.X);
                double y = Math.Min(selectionPoints.Item1.Y, selectionPoints.Item2.Y);
                Point topLeftPoint = new Point(x, y);
                mainCanvas.Children.Remove(lastTriangle);
                AddNewFigure(lastTriangle, currentToolDescription, topLeftPoint);
            }
            else if (this.activeTool == CanvasToolType.ToolArrow)
            {
                CheckFigureSettings(arrowPolygon, true);

                string currentToolDescription = CanvasToolDescription[(int)activeTool];
                double x = Math.Min(selectionPoints.Item1.X, selectionPoints.Item2.X);
                double y = Math.Min(selectionPoints.Item1.Y, selectionPoints.Item2.Y);
                Point topLeftPoint = new Point(x, y);
                mainCanvas.Children.Remove(arrowPolygon);
                AddNewFigure(arrowPolygon, currentToolDescription, topLeftPoint);
            }

            if (this.activeTool != CanvasToolType.ToolSelect)
            {
                this.isEmpty = false;
            }
            IsMouseDown = false;
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

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (IsMouseDown && activeTool >= CanvasToolType.ToolSquare)
            {
                OnCanvasMouseUp(sender, null);
            }
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
    }
}

