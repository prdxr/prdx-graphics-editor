using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.IO;
using System.Windows.Media;





namespace prdx_graphics_editor.modules.utils
{
    public class ApplicationSettings
    {
        public Color primaryColor { get; set; }
        public Color secondaryColor { get; set; }

        static string filepath = "../TEST/settings.ini";

        public ApplicationSettings()
        {
            primaryColor = Color.FromRgb(0, 0, 0);
            secondaryColor = Color.FromRgb(255, 255, 255);
        }
        ~ApplicationSettings()
        {
            SaveToFile();
        }

        public void SaveToFile()
        {
            string jsonString = JsonSerializer.Serialize(this);

            using (StreamWriter stream = new StreamWriter(filepath))
            {
                stream.Write(jsonString);
            }
        }
    }
}
