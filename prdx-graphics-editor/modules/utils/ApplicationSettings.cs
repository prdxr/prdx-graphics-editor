using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.IO;
using System.Windows.Media;
using System.Windows.Controls;
using prdx_graphics_editor.modules.canvas.PageCanvas;
using System.Windows.Shapes;





namespace prdx_graphics_editor.modules.utils
{
    public class ApplicationSettings
    {
        public Color primaryColor { get; set; }
        public Color secondaryColor { get; set; }
        public string colorPickerDefaultColor { get; set; }
        public CanvasToolType activeTool { get; set; }
        public Canvas canvas { get; set; }

        //accent1, accent2, accent1Text, accent2Text
        public List<Color> applicationTheme { get; set; }

        public static string applicationpath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\prdx-graphics-editor";
        public static string filepath = applicationpath + "\\settings.json";
        public static string projectspath = applicationpath + "\\projects";

        public ApplicationSettings()
        {
            this.primaryColor = Color.FromRgb(0, 0, 0);
            this.secondaryColor = Color.FromRgb(255, 255, 255);
            this.colorPickerDefaultColor = "#FFFFFF";
            this.activeTool = CanvasToolType.ToolPencil;
            this.applicationTheme = new List<Color> { 
                Color.FromRgb(47, 51, 56), 
                Color.FromRgb(67, 71, 77), 
                Color.FromRgb(234, 242, 255),
                Color.FromRgb(0, 0, 0)
            };
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
        public static ApplicationSettings CreateApplicationSettings()
        {
            string jsonString;

            if (File.Exists(filepath))
            {
                using (StreamReader stream = new StreamReader(filepath))
                {
                    jsonString = stream.ReadToEnd();
                }

                var obj = JsonSerializer.Deserialize<ApplicationSettings>(jsonString);

                return JsonSerializer.Deserialize<ApplicationSettings>(jsonString);
            }
            else
            {
                return new ApplicationSettings();
            }
        }
    }
}
