using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public int brushSize { get; set; }
        public int borderSize { get; set; }
        public bool enableFigureBorder { get; set; }
        public bool enableFigureFill { get; set; }

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

            this.brushSize = 10;
            this.borderSize = 3;
            this.enableFigureBorder = false;
            this.enableFigureFill = true;
    }
        ~ApplicationSettings()
        {
            SaveToFile();
        }

        public void SaveToFile()
        {
            var settings = new JsonSerializerOptions() {
                WriteIndented = true 
            };

            string jsonString = JsonSerializer.Serialize(this, settings);

            using (StreamWriter stream = new StreamWriter(filepath))
            {
                stream.Write(jsonString);
            }
        }
        public static ApplicationSettings CreateApplicationSettings()
        {
            string jsonString;
            var settings = new JsonSerializerOptions()
            {
                WriteIndented = true
            };


            if (File.Exists(filepath))
            {
                using (StreamReader stream = new StreamReader(filepath))
                {
                    jsonString = stream.ReadToEnd();
                }

                var obj = JsonSerializer.Deserialize<ApplicationSettings>(jsonString, settings);

                return JsonSerializer.Deserialize<ApplicationSettings>(jsonString);
            }
            else
            {
                return new ApplicationSettings();
            }
        }
    }
}
