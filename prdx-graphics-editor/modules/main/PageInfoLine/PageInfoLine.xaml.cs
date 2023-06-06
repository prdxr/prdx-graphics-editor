using System;
using System.Windows;
using System.Windows.Controls;
using prdx_graphics_editor.modules.utils;

namespace prdx_graphics_editor.modules.main.PageInfoLine
{
    public partial class PageInfoLine : Page
    {

        public PageInfoLine()
        {
            InitializeComponent();
            Globals.pageInfoLineRef = this;
            SetCurrentProject();
        }
        // Установка значений текущего положения курсора относительно холста. Если курсор вне холста, выводится "[Вне холста]"
        public void SetPointerValues(Point? pointer)
        {
            if (pointer is null)
            {
                labelPointerPosition.Content = "[Вне холста]";
            }
            else
            {
                labelPointerPosition.Content = $"{Math.Round(pointer.Value.X, 1)}, {Math.Round(pointer.Value.Y, 1)}";
            }
        }
        // Установка текущего проекта. Если проекта нет, то выводится "[Без проекта]", если есть несохранённые изменения, появляется красная звёздочка
        public void SetCurrentProject()
        {
            textblockCurrentLocation.Text = Globals.currentFile == null ? "[Без проекта]" : Globals.currentFile;
            textblockUnsavedAsterisk.Visibility = Globals.isProjectSaved ? Visibility.Hidden : Visibility.Visible;
        }
    }
}
