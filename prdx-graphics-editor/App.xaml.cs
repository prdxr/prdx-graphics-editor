using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace prdx_graphics_editor
{
    public partial class App : Application
    {
        private ResourceDictionary themeDictionary => Resources.MergedDictionaries[0];

        public App()
        {
            InitializeComponent();
            
            themeDictionary.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("modules/utils/StylesDictionary.xaml", UriKind.Relative) });
        }
    }
}
