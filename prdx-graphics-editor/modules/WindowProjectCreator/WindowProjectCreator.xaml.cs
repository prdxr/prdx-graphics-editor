using prdx_graphics_editor.modules.actions;
using prdx_graphics_editor.modules.utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace prdx_graphics_editor.modules.WindowProjectCreator
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class WindowProjectCreator : Window
    {
        private static readonly Regex numbersMask = new Regex("^[0-9]+$"); //regex that matches disallowed text

        public WindowProjectCreator()
        {
            InitializeComponent();
         
            Background = Globals.colorAccent1;
            buttonApply.Background = Globals.colorAccent2;
            buttonApply.Foreground = Globals.colorTextBright;
            buttonCancel.Background = Globals.colorAccent2;
            buttonCancel.Foreground = Globals.colorTextBright;
            widthInput.Background = Globals.colorAccent2;
            widthInput.Foreground = Globals.colorTextBright;
            heightInput.Background = Globals.colorAccent2;
            heightInput.Foreground = Globals.colorTextBright;
            labelWidth.Foreground = Globals.colorTextBright;
            labelHeight.Foreground = Globals.colorTextBright;
            textBoxPath.Background = Globals.colorAccent2;
            textBoxPath.Foreground = Globals.colorTextBright;
            buttonChangePath.Background = Globals.colorAccent2;
            buttonChangePath.Foreground = Globals.colorTextBright;
        }

        private void CreateProject(object sender, RoutedEventArgs e)
        {
            int width = Convert.ToInt32(widthInput.Text);
            int height = Convert.ToInt32(heightInput.Text);
            Actions.InitializeProject(width, height);
            Close();
        }

        private void checkForNumbers(object sender, TextChangedEventArgs e)
        {
            if (!UtilityFunctions.CheckInputValidity(sender as TextBox, numbersMask, Globals.colorAccent2))
            {
                buttonApply.IsEnabled = false;
            }
            else if (buttonApply != null)
            {
                buttonApply.IsEnabled = true;
            }
        }

        private void CancelProjectCreation(object sender, RoutedEventArgs e)
        {
            Close();
        }

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
