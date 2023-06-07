using prdx_graphics_editor.modules.actions;
using prdx_graphics_editor.modules.utils;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace prdx_graphics_editor.modules.WindowProjectCreator
{
    public partial class WindowProjectCreator : Window
    {
        // Маска разрешённых символов в полях ввода. В данном случае - цифры от 0 до 9
        private static readonly Regex numbersMask = new Regex("^[0-9]+$");

        public WindowProjectCreator()
        {
            InitializeComponent();
        }

        // Создание проекта с выбранными значениями ширины и высоты
        private void CreateProject(object sender, RoutedEventArgs e)
        {
            int width = Convert.ToInt32(widthInput.Text);
            int height = Convert.ToInt32(heightInput.Text);
            Actions.InitializeProject(width, height);
            Close();
        }

        // Проверка введённых значений при любом изменении полей ввода ширины и высоты
        private void checkForNumbers(object sender, TextChangedEventArgs e)
        {
            if (!UtilityFunctions.CheckInputValidity(sender as TextBox, numbersMask, Globals.appcolorAccent2))
            {
                buttonApply.IsEnabled = false;
            }
            else if (buttonApply != null)
            {
                buttonApply.IsEnabled = true;
            }
        }

        // Отмена создания проекта. Закрывает окно
        private void CancelProjectCreation(object sender, RoutedEventArgs e)
        {
            Close();
        }
        // Изменение выбранного проекта. Если оставить поле пустым, проект создан не будет
        private void ChangeProjectPath(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog();

            dialog.InitialDirectory = Directory.Exists(ApplicationSettings.projectspath) ? ApplicationSettings.projectspath : @"C:\";

            dialog.Title = "Выбор расположения проекта";
            dialog.Filter = "Файл проекта (*.xml) |*.xml";
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                Globals.currentFile = dialog.FileName;
            }
            textBoxPath.Text = dialog.FileName;
        }
    }
}
