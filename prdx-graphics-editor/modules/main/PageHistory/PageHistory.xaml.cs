using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using prdx_graphics_editor.modules.utils;
using prdx_graphics_editor.modules.actions;

namespace prdx_graphics_editor.modules.main
{

    public partial class PageHistory : Page
    {
        int oldSelection;
        int targetSelection;
        ObservableCollection<(object, string, Point)> historyList = new ObservableCollection<(object, string, Point)>();

        public PageHistory()
        {
            InitializeComponent();
            Globals.pageHistoryRef = this;
        }

        private void UndoAction(object sender, ExecutedRoutedEventArgs e)
        {
            Actions.HistoryUndo();
        }
        private void RedoAction(object sender, ExecutedRoutedEventArgs e)
        {
            Actions.HistoryRedo();
        }

        // Событие изменения фигур. Вызывает метод компиляции и отображения истории изменений
        public void OnFiguresChanged(object sender, EventArgs e)
        {
            ShowHistory();
        }
        // Компиляция и отображение истории изменений на соответствующей панели
        public void ShowHistory()
        {
            // Сначала в пустой лист добавляются элементы стэка отменённых изменений в обратном порядке, далее - элементы стэка применённых изменений в обычном порядке
            List<(object, string, Point)> mergeList = new List<(object, string, Point)>(); ObservableCollection<string> historyList = new ObservableCollection<string>();
            mergeList = mergeList.Concat(Globals.changeHistoryAfter.Reverse()).ToList();
            mergeList = mergeList.Concat(Globals.changeHistoryBefore).ToList();
            
            // Список historyList отображает только текстовые описания действий, в отличие от изначального mergeList, хранящего помимо описания сам объект и его положение
            foreach ((object, string, Point) element in mergeList)
            {
                historyList.Add(element.Item2);
            }
            // Добавление полученного списка в контрол ListView
            HistoryListView.ItemsSource = historyList;
            int currentIndex = Globals.changeHistoryBefore.Count;
            // Если отменённых изменений нет, то сброс выделения ListView, проект в таком случае не имеет правок, а значит сохранён
            if (currentIndex == 0)
            {
                HistoryListView.UnselectAll();
                Globals.isProjectSaved = true;
            }
            else
            {
                oldSelection = Globals.changeHistoryAfter.Count;
                HistoryListView.SelectedIndex = oldSelection;
                Globals.isProjectSaved = false;
            }
        }
        // При двойном нажатии ЛКМ по любому элементу ListView производится переход до выбранного действия
        private void GoToSelectedAction(object sender, MouseButtonEventArgs e)
        {
            targetSelection = (sender as ListView).SelectedIndex;
            while (oldSelection > targetSelection)
            {
                Actions.HistoryRedo();
            }
            while (oldSelection < targetSelection)
            {
                Actions.HistoryUndo();
            }
        }
    }

}
