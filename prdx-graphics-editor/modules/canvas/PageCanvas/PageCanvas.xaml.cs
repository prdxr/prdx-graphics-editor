using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Markup;
using prdx_graphics_editor.modules.utils;
using prdx_graphics_editor.modules.actions;
using System.Collections.Generic;

namespace prdx_graphics_editor.modules.canvas.PageCanvas
{
    // Перечисляемый тип для инструментов
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
        private CanvasToolType activeTool;
        private readonly Rectangle selectionRectangle;
        public bool isEmpty;
        public event EventHandler OnFiguresChanged;
        public event EventHandler OnZoomChanged;

        private Point handOffset;
        private bool isPlacingFigure = false;
        private readonly double[] selectionDashes = { 10, 5 };
        private Point canvasPointer = new Point();
        private bool mousePressedOnCanvas = false;

        // Названия инструментов
        readonly string[] CanvasToolDescription = new string[]
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

        // Координаты точек области прямоугольного выделения
        (Point, Point) selectionPoints;
        
        // Указатели на текущие редактируемые элементы каждого типа
        Polyline currentLine;
        public Shape lastShape;
        public Shape selectedShape;

        // Константы, определяющие масштабирование холста
        public const int ZOOM_PERCENT_STEP = 25;            // шаг изменения масштаба
        public const int ZOOM_PERCENT_MIN = 25;             // минимальный масштаб
        public const int ZOOM_PERCENT_MAX = 500;            // максимальный масштаб
        public const int ZOOM_PERCENT_DEFAULT = 100;        // значение масштаба по умолчанию

        // Константы, определяющие форму и размер стрелок
        private const double ARROW_BODY_WIDTH = 20;         // ширина тела стрелки
        private const double ARROW_BODY_LENGTH_MIN = 5;     // минимальная длина тела стрелки
        private const double ARROW_CAP_WIDTH = 60;          // ширина наконечника стрелки
        private const double ARROW_CAP_LENGTH = 30;         // длина наконечника стрелки

        double canvasZoom = 100;
        readonly ScaleTransform canvasScaleTransform = new ScaleTransform();

        // Обработчики событий для фигур
        readonly MouseButtonEventHandler OnFigureMouseDown;
        readonly MouseButtonEventHandler OnFigureMouseUp;
        readonly MouseEventHandler OnFigureMouseEnter;
        readonly MouseEventHandler OnFigureMouseLeave;

        public PageCanvas()
        {
            InitializeComponent();
            
            //При запуске холст пуст
            isEmpty = true;
            activeTool = Globals.applicationSettings.activeTool;

            // Инициализация обработчиков событий мыши для фигур
            OnFigureMouseDown = new MouseButtonEventHandler(FigureMouseDown);
            OnFigureMouseUp = new MouseButtonEventHandler(FigureMouseUp);
            OnFigureMouseEnter = new MouseEventHandler(FigureMouseEnter);
            OnFigureMouseLeave = new MouseEventHandler(FigureMouseLeave);

            // Инициализация прямоугольника выделения
            selectionPoints = (new Point(0, 0), new Point(0, 0));
            selectionRectangle = new Rectangle
            {
                Fill = new SolidColorBrush(Colors.Transparent),
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeDashArray = new DoubleCollection(selectionDashes)
            };

            selectionRectangle.MouseDown += OnFigureMouseDown;
            selectionRectangle.MouseUp += OnFigureMouseUp;
            selectionRectangle.MouseEnter += OnFigureMouseEnter;
            selectionRectangle.MouseLeave += OnFigureMouseLeave;

            Canvas.SetLeft(selectionRectangle, 0);
            Canvas.SetTop(selectionRectangle, 0);
            Canvas.SetZIndex(selectionRectangle, 2);
            mainCanvas.Children.Add(selectionRectangle);

            mainCanvas.LayoutTransform = canvasScaleTransform;

            Globals.pageCanvasRef = this;
            Globals.currentFile = null;
            currentLine = null;
            lastShape = null;
            selectedShape = null;
        }

        // Событие нажатия ЛКМ по фигуре
        private void FigureMouseDown(object sender, MouseButtonEventArgs e)
        {
            UIElement trueSender = sender as UIElement;

            // Захват фокуса мыши
            Mouse.Capture(trueSender);
            
            // При использовании инструмента "Рука" объект становится полупрозрачным для большего удобства пользователя
            if (activeTool == CanvasToolType.ToolHand)
            {
                trueSender.Opacity = 0.5;
            }

            // Отступ относительно начала координат фигуры - для перетаскивания рукой за место захвата
            handOffset = e.GetPosition(trueSender);

            // Особый случай для стрелок
            if (sender is Polygon && (sender as Polygon).Points.ToList().Count == 7)
            {
                Point cursor = e.GetPosition(mainCanvas);
                handOffset = new Point(cursor.X - Canvas.GetLeft(trueSender), cursor.Y - Canvas.GetTop(trueSender));
            }

            // Заливка области выделения
            if (sender == selectionRectangle && activeTool == CanvasToolType.ToolFill)
            {
                FillSelection();
            }
        }

        // Событие захода мыши на фигуру, смена курсора в зависимости от инструмента
        private void FigureMouseEnter(object sender, MouseEventArgs e)
        {
            if (activeTool == CanvasToolType.ToolFill && sender == selectionRectangle)
            {
                mainCanvas.Cursor = Cursors.Cross;
            }
            else if (activeTool >= CanvasToolType.ToolHand)
            {
                mainCanvas.Cursor = Cursors.Hand;
            }
        }
        // Событие выхода мыши с фигуры, смена курсора в зависимости от инструмента
        private void FigureMouseLeave(object sender, MouseEventArgs e)
        {
            if (activeTool == CanvasToolType.ToolFill && sender == selectionRectangle)
            {
                mainCanvas.Cursor = Cursors.No;
                return;
            }
            else if (activeTool == CanvasToolType.ToolHand)
            {
                mainCanvas.Cursor = Cursors.Arrow;
            }
        }
        // Событие отпускания ЛКМ с фигуры, очистка захвата мыши и сброс 
        private void FigureMouseUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            handOffset = new Point(0, 0);

            // Сброс прозрачности
            if (activeTool == CanvasToolType.ToolHand)
            {
                (sender as UIElement).Opacity = 1;
            }
        }

        // Смена курсора в зависимости от текущего инструмента
        private void SwitchCursor()
        {
            if (activeTool <= CanvasToolType.ToolEraser)
            {
                mainCanvas.Cursor = Cursors.Pen;
            }
            else if (activeTool == CanvasToolType.ToolFill)
            {
                mainCanvas.Cursor = Cursors.No;
            }
            else if (activeTool <= CanvasToolType.ToolArrow)
            {
                mainCanvas.Cursor = Cursors.Cross;
            }
            else
            {
                mainCanvas.Cursor = Cursors.Arrow;
            }
        }


        // Сохранение проекта в файл XML с помощью сериализации
        public void SerializeToXML(Canvas canvas, string filename)
        {
            string mystrXAML = XamlWriter.Save(canvas);
            FileStream filestream = File.Create(filename);
            StreamWriter streamwriter = new StreamWriter(filestream);
            streamwriter.Write(mystrXAML);
            streamwriter.Close();
            filestream.Close();
        }
        // Открытие проекта в формате XML с помощью десериализации
        public void DeserializeFromXML(string filename)
        {
            ResetCanvas();
            FileStream filestream = File.Open(filename, FileMode.Open);
            StreamReader streamreader = new StreamReader(filestream);
            string mystrXAML = streamreader.ReadToEnd();

            Canvas deserializedCanvas = XamlReader.Parse(mystrXAML) as Canvas;

            // Изменение размеров mainCanvas в соответствии с размерами десериализованного холста
            mainCanvas.Width = deserializedCanvas.Width;
            mainCanvas.Height = deserializedCanvas.Height;

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

        // Установка текущего инструмента
        public void SetActiveTool(CanvasToolType toolType)
        {
            activeTool = toolType;
            SwitchCursor();
            Globals.applicationSettings.activeTool = activeTool;

            // Выделение сбрасывается при выборе любого инструмента, кроме "Выделение" и "Заливка"
            if (activeTool < CanvasToolType.ToolSelect || activeTool > CanvasToolType.ToolFill)
            {
                selectionRectangle.Width = 0;
                selectionRectangle.Height = 0;
            }
        }

        // Сброс холста
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

        // Заливка выделенной области
        public void FillSelection()
        {
            if (selectionRectangle.Width > 0 && selectionRectangle.Height > 0)
            {
                Rectangle rectangle = new Rectangle
                {
                    Fill = new SolidColorBrush(Globals.applicationSettings.primaryColor),
                    Width = selectionRectangle.Width,
                    Height = selectionRectangle.Height
                };
                selectionRectangle.Width = 0;
                selectionRectangle.Height = 0;

                double x = Math.Min(selectionPoints.Item1.X, selectionPoints.Item2.X);
                double y = Math.Min(selectionPoints.Item1.Y, selectionPoints.Item2.Y);
                string currentToolDescription = CanvasToolDescription[(int)activeTool];
                AddNewFigure(rectangle, currentToolDescription, new Point(x, y));
            }
            }

        // Событие движения мыши по холсту
        private void OnCanvasMouseMove(object sender, MouseEventArgs e)
        {
            Point newPosition = e.GetPosition(mainCanvas);
            // Отображение текущей позиции курсора на информационной строке
            Globals.pageInfoLineRef.SetPointerValues(newPosition);
            // Если ЛКМ не нажата, выбран инструмент "Заливка" или в данный момент не производится рисование фигуры, выход из метода
            if (!mousePressedOnCanvas || e.LeftButton == MouseButtonState.Released || activeTool == CanvasToolType.ToolFill)
            {
                return;
            }

            if (activeTool == CanvasToolType.ToolHand)
            {
                // Проверка захвата мыши
                if (Mouse.Captured is UIElement capturedElement)
                {
                    // Обновление позиции фигуры относительно холста
                    Canvas.SetLeft(capturedElement, newPosition.X - handOffset.X);
                    Canvas.SetTop(capturedElement, newPosition.Y - handOffset.Y);
                }
            }
            else if (activeTool <= CanvasToolType.ToolEraser)
            {
                // Отдельный метод для инструментов рисования от руки: "Карандаш", "Кисть", "Ластик"
                OnCanvasMouseMoveDraw(sender, e, currentLine);
            }
            else
            {
                // Отдельный метод для инструментов работы с фигурами
                OnCanvasMouseMoveFigureMode(sender, e);
            }
        }

        // Метод обработки движения мыши по холсту при рисовании от руки
        private void OnCanvasMouseMoveDraw(object sender, MouseEventArgs e, Polyline line)
        {
            canvasPointer = GetCanvasPosition(sender, e);

            // Если зажата ЛКМ, текущая точка добавляется к линии
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                line.Points.Add(canvasPointer);
            }
        }
        // Метод обработки движения мыши по холсту при работе с фигурами
        private void OnCanvasMouseMoveFigureMode(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                selectionPoints.Item2 = GetCanvasPosition(sender, e);

                // Размеры области выделения
                double width = Math.Abs(selectionPoints.Item2.X - selectionPoints.Item1.X);
                double height = Math.Abs(selectionPoints.Item2.Y - selectionPoints.Item1.Y);

                // Сдвиг прямоугольника выделения относительно холста на расстояние до ближайшей точки, позволяет вести выделение в любую сторону
                Canvas.SetLeft(selectionRectangle, Math.Min(selectionPoints.Item1.X, selectionPoints.Item2.X));
                Canvas.SetTop(selectionRectangle, Math.Min(selectionPoints.Item1.Y, selectionPoints.Item2.Y));
                selectionRectangle.Width = width;
                selectionRectangle.Height = height;

                // Цвет предпросмотра - результирующий цвет фигуры с коэффициентом прозрачности 127/255. Позволяет видеть занимаемую область под размещаемой фигурой
                Color previewColorSecondary = Color.FromArgb(127, Globals.applicationSettings.secondaryColor.R, Globals.applicationSettings.secondaryColor.G, Globals.applicationSettings.secondaryColor.B);
                Color previewColorPrimary = Color.FromArgb(127, Globals.applicationSettings.primaryColor.R, Globals.applicationSettings.primaryColor.G, Globals.applicationSettings.primaryColor.B);
                SolidColorBrush previewBrushSecondary = new SolidColorBrush(previewColorSecondary);
                SolidColorBrush previewBrushPrimary = new SolidColorBrush(previewColorPrimary);

                // Логика работы для прямоугольников и эллипсов
                if (activeTool == CanvasToolType.ToolSquare || activeTool == CanvasToolType.ToolCircle)
                {
                    Canvas.SetLeft(lastShape, Math.Min(selectionPoints.Item1.X, selectionPoints.Item2.X));
                    Canvas.SetTop(lastShape, Math.Min(selectionPoints.Item1.Y, selectionPoints.Item2.Y));
                    lastShape.Width = width;
                    lastShape.Height = height;

                    lastShape.Fill = previewBrushSecondary;
                    lastShape.Stroke = previewBrushPrimary;
                    lastShape.StrokeThickness = Globals.applicationSettings.borderSize;
                    lastShape.StrokeDashArray = new DoubleCollection(selectionDashes);
                }
                // Логика работы для прямых линий
                if (activeTool == CanvasToolType.ToolLine)
                {
                    (lastShape as Line).X2 = GetCanvasPosition(sender, e).X;
                    (lastShape as Line).Y2 = GetCanvasPosition(sender, e).Y;

                    lastShape.Fill = previewBrushPrimary;
                    lastShape.Stroke = previewBrushSecondary;
                    lastShape.StrokeThickness = Globals.applicationSettings.brushSize;
                }
                // Логика работы для стрелок
                if (activeTool == CanvasToolType.ToolArrow)
                {
                    lastShape.Fill = previewBrushSecondary;
                    lastShape.Stroke = previewBrushPrimary;
                    lastShape.StrokeThickness = Globals.applicationSettings.borderSize;
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
                // Логика работы для треугольников
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

                    lastShape.Fill = previewBrushSecondary;
                    lastShape.Stroke = previewBrushPrimary;
                    lastShape.StrokeDashArray = new DoubleCollection(selectionDashes);
                    lastShape.StrokeThickness = Globals.applicationSettings.borderSize;
                }
            }
        }
        // Получение положения курсора относительно холста
        public Point GetCanvasPosition(object sender, MouseEventArgs e)
        {
            Canvas canvas = (Canvas)sender;
            Point canvasPointer = e.GetPosition(this);
            canvasPointer.Y -= canvas.Margin.Top;
            canvasPointer.X -= canvas.Margin.Left;
            canvasPointer.Y /= canvasZoom / 100;
            canvasPointer.X /= canvasZoom / 100;
            return canvasPointer;
        }
        // Событие нажатия ЛКМ по холсту
        private void OnCanvasMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // ЛКМ нажата в границах холста, позволяет в дальнейшем продолжить или закончить рисование фигуры при выходе мыши с холста
            mousePressedOnCanvas = true;
            // Движение холста инструментом "Рука" не допускается
            if (activeTool == CanvasToolType.ToolHand && sender is Canvas)
            {
                return;
            }

            // Выделение изменяется при любом инструменте работы с фигурами, кроме "Заливки"
            if (activeTool >= CanvasToolType.ToolSelect && activeTool != CanvasToolType.ToolFill)
            {
                selectionPoints.Item1 = GetCanvasPosition(sender, e);
                selectionPoints.Item2 = GetCanvasPosition(sender, e);
                selectionRectangle.Width = 0;
                selectionRectangle.Height = 0;
                Canvas.SetLeft(selectionRectangle, selectionPoints.Item1.X);
                Canvas.SetTop(selectionRectangle, selectionPoints.Item1.Y);
            }
            // Логика работы алгоритма создания фигур
            if (activeTool >= CanvasToolType.ToolSquare)
            {
                isPlacingFigure = true;
                switch (activeTool)
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

            // Логика работы алгоритма рисования от руки
            if (activeTool < CanvasToolType.ToolSelect)
            {
                canvasPointer = GetCanvasPosition(sender, e);
                if (e.GetPosition(this).X > Width || e.GetPosition(this).Y > Height)
                {
                    return;
                }

                currentLine = new Polyline
                {
                    StrokeLineJoin = PenLineJoin.Round,
                    StrokeStartLineCap = PenLineCap.Round,
                    StrokeEndLineCap = PenLineCap.Round
                };
                string currentToolDescription = CanvasToolDescription[(int)activeTool];
                AddNewFigure(currentLine, currentToolDescription, new Point(0, 0));

                canvasPointer = GetCanvasPosition(sender, e);
                currentLine.Points.Add(canvasPointer);

                CheckFigureSettings(currentLine);

                // Установка настроек линии в зависимости от выбранного инструмента
                switch (activeTool)
                {
                    case CanvasToolType.ToolPencil:
                        currentLine.Stroke = new SolidColorBrush(Globals.applicationSettings.primaryColor);
                        break;
                    case CanvasToolType.ToolBrush:
                        currentLine.Stroke = new SolidColorBrush(Globals.applicationSettings.primaryColor);
                        currentLine.SnapsToDevicePixels = false;
                        BlurEffect blurEffect = new BlurEffect
                        {
                            Radius = 10
                        };
                        currentLine.Effect = blurEffect;
                        break;
                    case CanvasToolType.ToolEraser:
                        currentLine.Stroke = new SolidColorBrush(Globals.applicationSettings.secondaryColor);
                        break;
                }
            }

        }

        // Проверка выбранных параметров инструмента
        void CheckFigureSettings(Shape targetShape, bool arrowCase = false)
        {
            // Установка параметров для инструментов рисования от руки
            if (targetShape.GetType() == typeof(Polyline) && !arrowCase)
            {
                targetShape.StrokeThickness = Globals.applicationSettings.brushSize;
                return;
            }
            // Установка параметров для прямой линии
            else if (targetShape.GetType() == typeof(Line))
            {
                (targetShape as Line).StrokeThickness = Globals.applicationSettings.brushSize;
                (targetShape as Line).Stroke = new SolidColorBrush(Globals.applicationSettings.secondaryColor);
                return;
            }

            // Удаление пунктира, отображаемого при предпросмотре
            targetShape.StrokeDashArray = null;
            // Установка параметров для остальных фигур (прямоугольник, эллипс, треугольник, стрелка)
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

        // Событие отпускания ЛКМ на холсте
        private void OnCanvasMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Если ЛКМ изначально была нажата не на холсте, выход из метода
            if (!mousePressedOnCanvas)
            {
                return;
            }
            mousePressedOnCanvas = false;

            string currentToolDescription = CanvasToolDescription[(int)activeTool];
            // Если не было операций рисования, проект не изменён
            if (activeTool != CanvasToolType.ToolSelect && activeTool != CanvasToolType.ToolHand)
            {
                Globals.isProjectSaved = false;
            }

            // Сброс выделения при любом инструменте, кроме "Заливки" и "Выделения"
            if (activeTool < CanvasToolType.ToolSelect || activeTool > CanvasToolType.ToolFill)
            {
                selectionRectangle.Width = 0;
                selectionRectangle.Height = 0;
            }

            // Логика работы для инструментов рисования от руки
            if (activeTool < CanvasToolType.ToolSelect)
            {
                // Отрисовка точки, если между нажатием и отпусканием ЛКМ не было движения мышью
                if (currentLine.Points.Count < 2)
                {
                    RadialGradientBrush blur = new RadialGradientBrush(Globals.applicationSettings.primaryColor, Colors.Transparent);
                    
                    Ellipse brushPoint = new Ellipse
                    {
                        Width = currentLine.StrokeThickness,
                        Height = currentLine.StrokeThickness
                    };
                    brushPoint.Fill = activeTool == CanvasToolType.ToolBrush ? blur : currentLine.Stroke;
                    Point topLeftPoint = new Point(currentLine.Points[0].X - currentLine.StrokeThickness / 2, currentLine.Points[0].Y - currentLine.StrokeThickness / 2);
                    Canvas.SetLeft(brushPoint, topLeftPoint.X);
                    Canvas.SetTop(brushPoint, topLeftPoint.Y);

                    RemoveLastFigure();
                    AddNewFigure(brushPoint, currentToolDescription, topLeftPoint);
                }
                lastShape = currentLine;
                currentLine = null;
            }

            // Логика работы для инструментов "Прямоугольник" и "Эллипс"
            if (activeTool >= CanvasToolType.ToolSquare && activeTool <= CanvasToolType.ToolCircle)
            {
                CheckFigureSettings(lastShape);

                double x = Math.Min(selectionPoints.Item1.X, selectionPoints.Item2.X);
                double y = Math.Min(selectionPoints.Item1.Y, selectionPoints.Item2.Y);
                Point topLeftPoint = new Point(x, y);
                mainCanvas.Children.Remove(lastShape);
                AddNewFigure(lastShape, currentToolDescription, topLeftPoint);
            }
            // Логика работы для инструмента "Прямая"
            else if (activeTool == CanvasToolType.ToolLine)
            {
                CheckFigureSettings(lastShape);

                Point topLeftPoint = new Point(0,0);
                mainCanvas.Children.Remove(lastShape);
                AddNewFigure(lastShape, currentToolDescription, topLeftPoint);
            }
            // Логика работы для инструмента "Треугольник"
            else if (activeTool == CanvasToolType.ToolTriangle)
            {
                CheckFigureSettings(lastShape);

                double x = Math.Min(selectionPoints.Item1.X, selectionPoints.Item2.X);
                double y = Math.Min(selectionPoints.Item1.Y, selectionPoints.Item2.Y);
                Point topLeftPoint = new Point(x, y);
                mainCanvas.Children.Remove(lastShape);
                AddNewFigure(lastShape, currentToolDescription, topLeftPoint);
            }
            // Логика работы для инструмента "Стрелка"
            else if (activeTool == CanvasToolType.ToolArrow)
            {
                CheckFigureSettings(lastShape, true);

                double x = Math.Min(selectionPoints.Item1.X, selectionPoints.Item2.X);
                double y = Math.Min(selectionPoints.Item1.Y, selectionPoints.Item2.Y);
                Point topLeftPoint = new Point(x, y);
                mainCanvas.Children.Remove(lastShape);
                AddNewFigure(lastShape, currentToolDescription, topLeftPoint);
            }
            // Если был отрисован любой элемент, кроме выделения, то холст не пустой
            if (activeTool != CanvasToolType.ToolSelect)
            {
                isEmpty = false;
            }
            // Всем линиям и фигурам (кроме выделения) добавляются обработчики событий фигур
            if (activeTool < CanvasToolType.ToolHand && activeTool != CanvasToolType.ToolSelect && activeTool != CanvasToolType.ToolFill)
            {
                lastShape.MouseDown += OnFigureMouseDown;
                lastShape.MouseUp += OnFigureMouseUp;
                lastShape.MouseEnter += OnFigureMouseEnter;
                lastShape.MouseLeave += OnFigureMouseLeave;
            }
            isPlacingFigure = false;
        }

        // Экспорт холста в графический файл
        public void ExportProject(string exportType, string filename)
        {
            // Временное сокрытие прямоугольника выделения, чтобы избежать его попадания на рендер
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
                // Использование соответствующего кодировщика в зависимости от выбранного пользователем формата
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
                // Возвращение видимости прямоугольника выделения
                selectionRectangle.Visibility = Visibility.Visible;
            }
        }
        // Импорт изображений в проект (подразумевает перезапись проекта, для вставки изображений в проект см. PasteClipboard())
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

        // Метод добавления фигур на холст с занесением в историю изменений
        public void AddNewFigure(Shape figure, string operationDescription, Point position,  bool isRedo = false)
        {
            mainCanvas.Children.Add(figure);

            // Логика установки координат для линий, прямоугольников и эллипсов
            if (!(figure is Polygon))
            {
                Canvas.SetLeft(figure, position.X);
                Canvas.SetTop(figure, position.Y);
            }
            // Добавление фигуры в стэк применённых изменений
            Globals.changeHistoryBefore.Push((figure, operationDescription, position));

            // В случае нового действия (а не повторного применения правок) необходимо очистить стэк отменённых изменений
            if (!isRedo)
            {
                Globals.changeHistoryAfter.Clear();
            }
            if (OnFiguresChanged != null)
            {
                OnFiguresChanged.Invoke(this, null);
            }
        }
        // Перегрузка метода добавления фигур на холст с занесением в историю изменений. Обрабатывает изображения (Image)
        public void AddNewFigure(Image image, string operationDescription, Point position, bool isRedo = false)
        {
            mainCanvas.Children.Add(image);

            // Добавление фигуры в стэк применённых изменений
            Globals.changeHistoryBefore.Push((image, operationDescription, position));

            // В случае нового действия (а не повторного применения правок) необходимо очистить стэк отменённых изменений
            if (!isRedo)
            {
                Globals.changeHistoryAfter.Clear();
            }
            if (OnFiguresChanged != null)
            {
                OnFiguresChanged.Invoke(this, null);
            }
        }

        // Удаление последней фигуры с холста с занесением в историю изменений
        public void RemoveLastFigure()
        {
            // Если отменять нечего, выход из метода
            if (Globals.changeHistoryBefore.Count == 0)
            {
                return;
            }
            // Удаление последнего элемента в стэке применённых изменений и получение его параметров
            (object figure, string operationDescription, Point position) = Globals.changeHistoryBefore.Pop();

            selectionRectangle.Width = 0;
            selectionRectangle.Height = 0;

            // Разнесение по типам, т.к. Canvas.Children.Remove не принимает класс, объединяющий Shape и Image
            if (figure is Shape)
            {
                mainCanvas.Children.Remove(figure as Shape);
            }
            else if (figure is Image)
            {
                mainCanvas.Children.Remove(figure as Image);
            }
            // Добавление удалённого объекта в стэк отменённых изменений
            Globals.changeHistoryAfter.Push((figure, operationDescription, position));

            if (OnFiguresChanged != null)
            {
                OnFiguresChanged.Invoke(this, null);
            }
        }
        // Возврат последней фигуры на холст с занесением в историю изменений
        public void ReturnLastFigure()
        {
            // Если возвращать нечего, выход из метода
            if (Globals.changeHistoryAfter.Count == 0)
            {
                return;
            }
            // Удаление последнего элемента в стэке отменённых изменений и получение его параметров
            (object figure, string operationDescription, Point position) = Globals.changeHistoryAfter.Pop();

            // Разнесение по типам, т.к. Canvas.Children.Remove не принимает класс, объединяющий Shape и Image
            // Вызывается собственный метод добавления фигуры (AddNewFigure()), занесение в стэк реализовано в нём
            if (figure is Shape)
            {
                lastShape = figure as Shape;
                AddNewFigure(lastShape, operationDescription, position, true);
            }
            else if (figure is Image)
            {
                AddNewFigure(figure as Image, operationDescription, position, true);
            }

            if (OnFiguresChanged != null)
            {
                OnFiguresChanged.Invoke(this, null);
            }
        }

        // Очистка выделения
        public void SelectClear()
        {
            selectionRectangle.Width = 0;
            selectionRectangle.Height = 0;
        }
        // Выделение всего холста
        public void SelectAll()
        {
            Canvas.SetTop(selectionRectangle, 0);
            Canvas.SetLeft(selectionRectangle, 0);
            selectionRectangle.Width = mainCanvas.ActualWidth;
            selectionRectangle.Height = mainCanvas.ActualHeight;
        }
        // Событие захода мыши на холст
        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            // Отрисовка фигур "как есть", если ЛКМ была отпущена за границами холста
            if (e.LeftButton == MouseButtonState.Released && activeTool >= CanvasToolType.ToolSquare && isPlacingFigure)
            {
                OnCanvasMouseUp(sender, null);
            }
        }
        // Событие выхода мыши с холста
        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            // Установка "пустого" значения текущего положения курсора относительно холста - на информационной панели выводится "[вне холста]"
            Globals.pageInfoLineRef.SetPointerValues(null);
        }

        // Вставка содержимого буфера обмена. Только изображения
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
                    image.MouseDown += OnFigureMouseDown;
                    image.MouseUp += OnFigureMouseUp;
                    image.MouseEnter += OnFigureMouseEnter;
                    image.MouseLeave += OnFigureMouseLeave;
                    image.Source = imageSource;
                    AddNewFigure(image, "Вставка", new Point(0, 0));
                }
                catch { }
                return;
            }
            BitmapSource img = Clipboard.GetImage();
            if (img != null)
            {
                Image image = new Image();
                Canvas.SetLeft(image, 0);
                Canvas.SetTop(image, 0);
                image.MouseDown += OnFigureMouseDown;
                image.MouseUp += OnFigureMouseUp;
                image.MouseEnter += OnFigureMouseEnter;
                image.MouseLeave += OnFigureMouseLeave;
                image.Source = img;
                AddNewFigure(image, "Вставка", new Point(0, 0));
        }
        }
        // Копирование выделенной области в буфер обмена
        public void CopyToClipboard()
        {
            if (selectionRectangle.Width <= 0 && selectionRectangle.Height <= 0)
            {
                return;
        }
            else
            {
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
                Int32Rect rect2 = new Int32Rect(Convert.ToInt32(Canvas.GetLeft(selectionRectangle)), Convert.ToInt32(Canvas.GetTop(selectionRectangle)),
                                          Convert.ToInt32(selectionRectangle.Width), Convert.ToInt32(selectionRectangle.Height));
                CroppedBitmap crop = new CroppedBitmap(renderBmp, rect2);

                Clipboard.SetImage(crop);
                selectionRectangle.Visibility = Visibility.Visible;
            }
        }
        // Вызов окна изменения размеров холста
        private void OnClickMenuItemCanvasSize(object sender, RoutedEventArgs e)
        {
            Actions.CanvasSize();
        }
        // Получение текущего коэффициента приближения холста
        public double GetCanvasZoom()
        {
            return canvasZoom;
        }
        // Установка коэффициента приближения холста
        public void SetCanvasZoom(double canvasZoom)
        {
            // Ограничение константами
            if (canvasZoom >= ZOOM_PERCENT_MIN && canvasZoom <= ZOOM_PERCENT_MAX) 
            {
                this.canvasZoom = canvasZoom;
                canvasScaleTransform.ScaleX = canvasZoom / 100;
                canvasScaleTransform.ScaleY = canvasZoom / 100;
                if (OnZoomChanged != null)
                {
                    OnZoomChanged.Invoke(this, null);
                }
            }
        }

        public void ZoomIncrease()
        {
            SetCanvasZoom(canvasZoom + ZOOM_PERCENT_STEP);
        }

        public void ZoomDecrease()
        {
            SetCanvasZoom(canvasZoom - ZOOM_PERCENT_STEP);
        }

        // Событие движения колёсика мыши
        private void OnCanvasMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Если нажат или зажат левый/правый контрол, происходит зум, зависит от направления движения колёсика
            KeyStates leftCtrl = Keyboard.GetKeyStates(Key.LeftCtrl);
            KeyStates rightCtrl = Keyboard.GetKeyStates(Key.RightCtrl);
            List<KeyStates> allowedStates = new List<KeyStates>() { KeyStates.Down | KeyStates.Toggled, KeyStates.Down, KeyStates.Toggled };
            if (allowedStates.Contains(leftCtrl) || allowedStates.Contains(rightCtrl))
            {
                if (e.Delta > 0)
                {
                    ZoomIncrease();
                }
                else
                {
                    ZoomDecrease();
                }
                return;
            }
        }

    }
}

