﻿using System;
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





namespace prdx_graphics_editor.modules.utils
{
    public class ApplicationSettings
    {
        public Color primaryColor { get; set; }
        public Color secondaryColor { get; set; }
        public string colorPickerDefaultColor { get; set; }
        public CanvasToolType activeTool { get; set; }
        public Canvas canvas { get; set; }

        public static string filepath = AppDomain.CurrentDomain.BaseDirectory + "/settings.json";

        public ApplicationSettings()
        {
            this.primaryColor = Color.FromRgb(0, 0, 0);
            this.secondaryColor = Color.FromRgb(255, 255, 255);
            this.colorPickerDefaultColor = "#FFFFFF";
            this.activeTool = CanvasToolType.ToolPencil;
            
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
