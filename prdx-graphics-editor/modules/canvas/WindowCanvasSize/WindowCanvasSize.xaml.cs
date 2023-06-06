using System;
using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using prdx_graphics_editor.modules.utils;


namespace prdx_graphics_editor.modules.canvas
{

    public partial class WindowCanvasSize : Window
    {
        // Маска допустимых элементов в полях ввода. В данном случае - цифры от 0 до 9
        private static readonly Regex numbersMask = new Regex("^[0-9]+$");
        // Статический метод вызова окна изменения размеров холста
        public static void ChangeCanvasSize(Canvas canvas)
        {
            WindowCanvasSize window = new WindowCanvasSize();
            window.canvasRef = canvas;
            window.heightInput.Text = window.canvasRef.Width.ToString("F0");
            window.widthInput.Text = window.canvasRef.Height.ToString("F0");
            window.ShowDialog();
        }

        private Canvas canvasRef;

        public WindowCanvasSize()
        {
            InitializeComponent();
        }

        // Проверка значений введённых данных, отключение кнопки применения при некорректных значениях
        private void checkForNumbers(object sender, TextChangedEventArgs e)
        {
            if (!UtilityFunctions.CheckInputValidity(sender as TextBox, numbersMask, Globals.appcolorAccent2) && buttonApply != null)
            {
                buttonApply.IsEnabled = false;
            }
            else if (buttonApply != null)
            {
                buttonApply.IsEnabled = true;
            }
        }

        // Применение изменений
        private void ApplyNewSize(object sender, RoutedEventArgs e)
        {
            canvasRef.Width = Convert.ToInt32(widthInput.Text);
            canvasRef.Height = Convert.ToInt32(heightInput.Text);
            Close();
        }
        // Отмена изменений
        private void CancelNewSize(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
