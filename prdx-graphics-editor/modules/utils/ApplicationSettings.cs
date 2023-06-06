using System;
using System.IO;
using System.Text.Json;
using System.Windows.Media;
using System.Collections.Generic;
using prdx_graphics_editor.modules.canvas.PageCanvas;


namespace prdx_graphics_editor.modules.utils
{
    public class ApplicationSettings
    {
        public Color primaryColor { get; set; }
        public Color secondaryColor { get; set; }
        public string colorPickerDefaultColor { get; set; }
        public CanvasToolType activeTool { get; set; }
        public int brushSize { get; set; }
        public int borderSize { get; set; }
        public bool enableFigureBorder { get; set; }
        public bool enableFigureFill { get; set; }
        public List<Color> applicationTheme { get; set; }

        public static string applicationpath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\prdx-graphics-editor";
        public static string filepath = applicationpath + "\\settings.json";
        public static string projectspath = applicationpath + "\\projects";

        public ApplicationSettings()
        {
            primaryColor = Color.FromRgb(0, 0, 0);
            secondaryColor = Color.FromRgb(255, 255, 255);
            colorPickerDefaultColor = "#FFFFFF";
            activeTool = CanvasToolType.ToolPencil;
            applicationTheme = new List<Color> { 
                Color.FromRgb(47, 51, 56), 
                Color.FromRgb(67, 71, 77), 
                Color.FromRgb(234, 242, 255),
                Color.FromRgb(0, 0, 0)
            };

            brushSize = 10;
            borderSize = 3;
            enableFigureBorder = false;
            enableFigureFill = true;
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

                return JsonSerializer.Deserialize<ApplicationSettings>(jsonString);
            }
            else
            {
                return new ApplicationSettings();
            }
        }
    }
}
