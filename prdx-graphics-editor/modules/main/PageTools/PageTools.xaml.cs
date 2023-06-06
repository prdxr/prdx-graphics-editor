using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.Generic;
using prdx_graphics_editor.modules.actions;
using prdx_graphics_editor.modules.canvas.PageCanvas;
using prdx_graphics_editor.modules.utils;


namespace prdx_graphics_editor
{
    public partial class PageTools : Page
    {
        public PageTools()
        {
            InitializeComponent();
            ChangeColorListener();

            Globals.pageToolsRef = this;

            int currentTool = (int)Globals.applicationSettings.activeTool;
            int count = VisualTreeHelper.GetChildrenCount(GridTools);
            // Первоначальная установка активного инструмента в таблице инструментов при инициализации страницы
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(GridTools, i);
                if (child is RadioButton && (Convert.ToInt32((child as RadioButton).CommandParameter) == currentTool))
                {
                    (child as RadioButton).IsChecked = true;
                    break;
                }
            }
        }
        // Изменение активного инструмента
        public void ChangeTool(object sender, ExecutedRoutedEventArgs e)
        {
            int a = Convert.ToInt32(e.Parameter);

            CanvasToolType toolType = (CanvasToolType)a;

            RadioButton[] buttons = { 
                ButtonPencil,
                ButtonBrush,
                ButtonEraser,
                ButtonSelect,
                ButtonFill,
                ButtonSquare,
                ButtonCircle,
                ButtonTriangle,
                ButtonLine,
                ButtonArrow
            };

            int count = VisualTreeHelper.GetChildrenCount(GridTools);
            // Поскольку метод может быть вызван нажатием горячих клавиш, повторная установка активного инструмента в таблице инструментов
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(GridTools, i);

                if (child is RadioButton && (Convert.ToInt32((child as RadioButton).CommandParameter) == Convert.ToInt32(e.Parameter)))
                {
                    (child as RadioButton).IsChecked = true;
                    break;
                }
            }

            List<RadioButton> radioButtons = LogicalTreeHelper.GetChildren(GridTools).OfType<RadioButton>().ToList();
            RadioButton selected = radioButtons.FirstOrDefault(x => x.CommandParameter == e.Parameter);
            int index = Array.IndexOf(buttons, selected);
            // Установка инструмента с помощью его числового ID, позже преобразуемового в перечисляемый тип
            Actions.SetActiveTool(toolType);
        }

        // Обрабочтик события изменения цветов, вызывается при изменении цвета фона или основного цвета. Меняет подсказки к кнопкам выбора цвета на соответствующий HEX-код
        public void ChangeColorListener()
        {
            ButtonColorForeground.Background = new SolidColorBrush(Globals.applicationSettings.primaryColor);
            ButtonColorBackground.Background = new SolidColorBrush(Globals.applicationSettings.secondaryColor);
            ButtonColorForeground.ToolTip = "Основной цвет: #" + Globals.applicationSettings.primaryColor.ToString().Substring(3);
            ButtonColorBackground.ToolTip = "Фоновый цвет: #" + Globals.applicationSettings.secondaryColor.ToString().Substring(3);
        }
        
        //Методы изменения основного цвета и цвета фона, а также метод смены цвета
        private void ChangeForegroundColor(object sender, RoutedEventArgs e)
        {
            Color? color = Actions.PickForegroundColor();
            if (color != null)
            {
                Color result = color.GetValueOrDefault();
                (sender as Button).Background = new SolidColorBrush(result);
                Globals.applicationSettings.primaryColor = result;
            }
        }
        private void ChangeBackgroundColor(object sender, RoutedEventArgs e)
        {
            Color? color = Actions.PickBackgroundColor();
            if (color != null)
            {
                Color result = color.GetValueOrDefault();
                (sender as Button).Background = new SolidColorBrush(result);
                Globals.applicationSettings.secondaryColor = result;
            }
        }
        public void SwapColors(object sender, RoutedEventArgs e)
        {
            Color buff = Globals.applicationSettings.secondaryColor;
            Globals.applicationSettings.secondaryColor = Globals.applicationSettings.primaryColor;
            Globals.applicationSettings.primaryColor = buff;
            ChangeColorListener();
        }
    }
}
