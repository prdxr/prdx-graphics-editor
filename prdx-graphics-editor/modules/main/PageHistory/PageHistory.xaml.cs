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
        public PageHistory()
        {
            InitializeComponent();
            Globals.pageHistoryRef = this;
        }

        ObservableCollection<(Shape, string, Point)> historyList = new ObservableCollection<(Shape, string, Point)>();

        EventHandler historyChanged;

    private void ClickUndo(object sender, RoutedEventArgs e)
        {
            Actions.Undo();
        }
        private void ClickRedo(object sender, RoutedEventArgs e)
        {
            Actions.Redo();
        }

        public void OnFiguresChanged(object sender, EventArgs e)
        {
            ShowHistory();
        }
        public void ShowHistory()
        {
            //string boxCaption = "a";
            //string boxText = "a";
            //MessageBoxButton boxButtons = MessageBoxButton.OK;
            //MessageBoxImage boxIcon = MessageBoxImage.Warning;
            //MessageBoxResult boxResult = MessageBox.Show(boxText, boxCaption, boxButtons, boxIcon);

            List<(Shape, string, Point)> mergeList = new List<(Shape, string, Point)>();
            ObservableCollection<string> historyList = new ObservableCollection<string>();
            mergeList = mergeList.Concat(Globals.changeHistoryAfter.Reverse()).ToList();
            mergeList = mergeList.Concat(Globals.changeHistoryBefore).ToList();
            //mergeList = mergeList.ConvertAll(new Converter<(Shape, string, Point), string>(PointFToPoint));
            //Concat(Globals.changeHistoryAfter.Reverse()).ToList();
            foreach ((Shape, string, Point) element in mergeList)
            {
                historyList.Add(element.Item2);
            }
            
            //historyList = new ObservableCollection<string>(mergeList.);
            HistoryListView.ItemsSource = historyList;
            HistoryListView.SelectedIndex = Globals.changeHistoryAfter.Count;
        }
        public static string PointFToPoint((Shape, string, Point) element)
        {
            return element.Item2;
        }
    }
}
