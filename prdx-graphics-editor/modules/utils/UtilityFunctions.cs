using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace prdx_graphics_editor.modules.utils
{
    // Статический класс, содержащий функции, часто используемые в множестве классов
    public static class UtilityFunctions
    {
        public static bool CheckInputValidity(TextBox sender, Regex mask, SolidColorBrush baseColor)
        {
            if (sender.Text.Length == 0 || !mask.IsMatch(sender.Text))
            {
                sender.Background = Brushes.DarkRed;
                return false;
            }

            sender.Background = baseColor;
            return true;
        }
    }
}
