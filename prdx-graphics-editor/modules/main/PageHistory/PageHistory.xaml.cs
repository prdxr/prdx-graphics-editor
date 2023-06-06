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

        public void OnFiguresChanged(object sender, EventArgs e)
        {
            ShowHistory();
        }

        public void ShowHistory()
        {
            //List<(object, string, Point)> mergeList = new List<(object, string, Point)>();
            List<(object, string, Point)> mergeList = new List<(object, string, Point)>(); ObservableCollection<string> historyList = new ObservableCollection<string>();
            mergeList = mergeList.Concat(Globals.changeHistoryAfter.Reverse()).ToList();
            mergeList = mergeList.Concat(Globals.changeHistoryBefore).ToList();
            
            //foreach ((object, string, Point) element in mergeList)
            foreach ((object, string, Point) element in mergeList)
            {
                historyList.Add(element.Item2);
            }
            
            HistoryListView.ItemsSource = historyList;
            int currentIndex = Globals.changeHistoryBefore.Count;
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

        private void GoToSelectedAction(object sender, MouseButtonEventArgs e)
        {
            targetSelection = (sender as ListView).SelectedIndex;
            while (oldSelection > targetSelection)
            {
                Console.WriteLine($"selection > index -- {oldSelection} {targetSelection}");
                Actions.HistoryRedo();
            }
            while (oldSelection < targetSelection)
            {
                Console.WriteLine($"selection < index -- {oldSelection} {targetSelection}");
                Actions.HistoryUndo();
            }
        }
    }

}
