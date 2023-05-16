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

namespace prdx_graphics_editor.modules.main.ToolParams
{
    /// <summary>
    /// Логика взаимодействия для ToolParams.xaml
    /// </summary>
    public partial class ToolParams : Page
    {
        public ToolParams()
        {
            InitializeComponent();
            mainLabel.Foreground = Globals.colorTextBright;
            paramGrid.Background = Globals.colorAccent2;
            labelThickness.Foreground = Globals.colorTextBright;
            textboxThickness.Background = Globals.colorAccent1;
            textboxThickness.Foreground = Globals.colorTextBright;

        }
    }
}
