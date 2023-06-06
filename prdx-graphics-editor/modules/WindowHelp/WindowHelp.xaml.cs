using prdx_graphics_editor.modules.actions;
using prdx_graphics_editor.modules.utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace prdx_graphics_editor.modules.WindowHelp
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class WindowHelp : Window
    {
        public WindowHelp()
        {
            string headerText = "\n\nНа левой панели представлены инструменты и настройки инструмента, для выбора инструмента можно воспользоваться таблицей инструментов в верхней части панели или горячими клавишами (список горячих клавиш в правой части окна справки). На правой панели представлена история изменений, для перехода к конкретному изменению можно использовать двойной клик левой кнопкой мыши. На нижней панели отображается текущее положение курсора относительно холста и состояние редактируемого документа. На верхней панели расположено выпадающее меню.\n\n" +
                                "Важные уточнения работы программы:\n" +
                                "   - для использования инструмента 'заливка' требуется выделить область;\n" +
                                "   - использование инструмента 'рука' на прямоугольнике выделения разрешено, но на результате заливки - нет;\n" +
                                "   - красная звёздочка на нижней панели появляется, если документ имеет несохранённые изменения;\n" +
                                "   - поддерживаемые форматы импорта и экспорта: PNG, JPG, BMP. Формат проектов - XML;\n" +
                                "   - основной цвет (верхний прямоугольник на панели цветов) отвечает за цвет кистей и заливки, а также обводку фигур;\n" +
                                "   - дополнительный цвет (нижний прямоугольник на панели цветов) отвечает за цвет ластика и фон фигур\n\n";

            string hotkeys = "Общее:\n" +
                             "    Создать проект - Ctrl+N\n" +
                             "    Открыть проект - Ctrl+O\n" +
                             "    Сохранить проект - Ctrl+S\n" +
                             "    Сохранить проект как... - Ctrl+Shift+S\n" +
                             "    Импорт изображения - Ctrl+I\n" +
                             "    Экспорт изображения - Ctrl+E\n" +
                             "    Экспорт изображения - Ctrl+Z\n" +
                             "    Экспорт изображения - Ctrl+Y\n" +
                             "    Выделить весь холст - Ctrl+A\n" +
                             "    Снять выделение - Ctrl+Shift+A\n" +
                             "    Увеличить масштаб холста - Ctrl+Плюс\n" +
                             "    Уменьшить масштаб холста - Ctrl+Минус\n" +
                             "    Сбьросить масштаб холста - Ctrl+0\n" +
                             "    Показать окно справки - F1\n" +
                             "    Скрыть/показать боковые панели - TAB\n" +
                             "    Закрыть приложение - Shift+Esc\n\n" +
                             "Инструменты:\n" +
                             "    Карандаш - P\n" +
                             "    Кисть - N\n" +
                             "    Ластик - E\n" +
                             "    Выделение - S\n" +
                             "    Заливка - F\n" +
                             "    Прямоугольник - R\n" +
                             "    Эллипс - C\n" +
                             "    Треугольник - T\n" +
                             "    Прямая линия - L\n" +
                             "    Стрелка - A\n" +
                             "    Рука - H\n" +
                             "    Поменять цвета местами - X";
            InitializeComponent();
            TextboxHeader.Text = headerText;
            TextboxHotkeys.Text = hotkeys;
            TextblockContact.Foreground = Globals.appcolorText;
        }

        private void OnNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }

        private void CloseWindow(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }
    }
}
