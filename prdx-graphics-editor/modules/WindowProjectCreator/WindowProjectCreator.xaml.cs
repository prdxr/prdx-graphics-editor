using prdx_graphics_editor.modules.actions;
using System;
using System.Collections.Generic;
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
        private static readonly Regex numbersMask = new Regex("[^0-9]+"); //regex that matches disallowed text

        public WindowProjectCreator()
        {
            InitializeComponent();
        }

        private void CreateProject(object sender, RoutedEventArgs e)
        {
            int width = Convert.ToInt32(widthInput.Text);
            int height = Convert.ToInt32(heightInput.Text);
            Actions.InitializeProject(width, height);
            Close();
        }

        private void checkForNumbers(object sender, TextCompositionEventArgs e)
        {
            e.Handled = numbersMask.IsMatch((sender as TextBox).Text);
        }

        private void CancelProjectCreation(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
