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
using System.Collections.ObjectModel;
using prdx_graphics_editor.modules.utils;
using prdx_graphics_editor.modules.actions;

namespace prdx_graphics_editor.modules.main
{
    /// <summary>
    /// Логика взаимодействия для PageHistory.xaml
    /// </summary>
    public partial class PageHistory : Page
    {
        int oldSelection;
        int targetSelection;
        public PageHistory()
        {
            InitializeComponent();
            Globals.pageHistoryRef = this;
            mainLabel.Foreground = Globals.colorTextBright;
            buttonUndo.Background = Globals.colorAccent1;
            buttonRedo.Background = Globals.colorAccent1;
            HistoryListView.Foreground = Globals.colorTextBright;
        }

        ObservableCollection<(Shape, string, Point)> historyList = new ObservableCollection<(Shape, string, Point)>();

        private void ClickUndo(object sender, ExecutedRoutedEventArgs e)
        {
            Actions.Undo();
        }
        private void ClickRedo(object sender, ExecutedRoutedEventArgs e)
        {
            Actions.Redo();
        }

        public void OnFiguresChanged(object sender, EventArgs e)
        {
            ShowHistory();
        }
        public void ShowHistory()
        {
            List<(Shape, string, Point)> mergeList = new List<(Shape, string, Point)>();
            ObservableCollection<string> historyList = new ObservableCollection<string>();
            mergeList = mergeList.Concat(Globals.changeHistoryAfter.Reverse()).ToList();
            mergeList = mergeList.Concat(Globals.changeHistoryBefore).ToList();
            
            foreach ((Shape, string, Point) element in mergeList)
            {
                historyList.Add(element.Item2);
            }
            
            HistoryListView.ItemsSource = historyList;
            int currentIndex = Globals.changeHistoryBefore.Count;
            if (currentIndex == 0)
            {
                HistoryListView.UnselectAll();
            }
            else
            {
                oldSelection = Globals.changeHistoryAfter.Count;
                HistoryListView.SelectedIndex = oldSelection;
            }
        }
        private void GoToSelectedAction(object sender, MouseButtonEventArgs e)
        {
            targetSelection = (sender as ListView).SelectedIndex;
            while (oldSelection > targetSelection)
            {
                Console.WriteLine($"selection > index -- {oldSelection} {targetSelection}");
                Actions.Redo();
            }
            while (oldSelection < targetSelection)
            {
                Console.WriteLine($"selection < index -- {oldSelection} {targetSelection}");
                Actions.Undo();
            }
        }
    }
}
