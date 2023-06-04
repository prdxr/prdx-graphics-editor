﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using prdx_graphics_editor.modules.actions;
using prdx_graphics_editor.modules.canvas.PageCanvas;
using prdx_graphics_editor.modules.utils;

//не используются, после релиза удалить.
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace prdx_graphics_editor
{
    /// <summary>
    /// Логика взаимодействия для tools.xaml
    /// </summary>
    public partial class PageTools : Page
    {
        public PageTools()
        {
            InitializeComponent();
            ChangeColorListener();

            Globals.pageToolsRef = this;

            int currentTool = (int)Globals.applicationSettings.activeTool;
            int count = VisualTreeHelper.GetChildrenCount(toolGrid);
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(toolGrid, i);
                if (child is RadioButton && (Convert.ToInt32((child as RadioButton).CommandParameter) == currentTool))
                {
                    (child as RadioButton).IsChecked = true;
                    break;
                }
            }
        }

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

            int count = VisualTreeHelper.GetChildrenCount(toolGrid);
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(toolGrid, i);

                if (child is RadioButton && (Convert.ToInt32((child as RadioButton).CommandParameter) == Convert.ToInt32(e.Parameter)))
                {
                    (child as RadioButton).IsChecked = true;
                    break;
                }
            }


            List<RadioButton> radioButtons = LogicalTreeHelper.GetChildren(toolGrid).OfType<RadioButton>().ToList();
            RadioButton selected = radioButtons.FirstOrDefault(x => x.CommandParameter == e.Parameter);
            int index = Array.IndexOf(buttons, selected);
            //CanvasToolType toolType = (CanvasToolType) index;

            Actions.SetActiveTool(toolType);
        }

        public void ChangeColorListener()
        {
            buttonForegroundColor.Background = new SolidColorBrush(Globals.applicationSettings.primaryColor);
            buttonBackgroundColor.Background = new SolidColorBrush(Globals.applicationSettings.secondaryColor);
        }

        private void ChangeForegroundColor(object sender, RoutedEventArgs e)
        {
            Color? color = Actions.PickColor("foreground");
            if (color != null)
            {
                Color result = color.GetValueOrDefault();
                (sender as Button).Background = new SolidColorBrush(result);
                Globals.applicationSettings.primaryColor = result;
            }
        }
        private void ChangeBackgroundColor(object sender, RoutedEventArgs e)
        {
            Color? color = Actions.PickColor("background");
            if (color != null)
            {
                Color result = color.GetValueOrDefault();
                (sender as Button).Background = new SolidColorBrush(result);
                Globals.applicationSettings.secondaryColor = result;
            }
        }
        public void SwitchColors(object sender, RoutedEventArgs e)
        {
            Color buff = Globals.applicationSettings.secondaryColor;
            Globals.applicationSettings.secondaryColor = Globals.applicationSettings.primaryColor;
            Globals.applicationSettings.primaryColor = buff;
            ChangeColorListener();
        }
    }
}
