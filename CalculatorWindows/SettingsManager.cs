using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace CalculatorWindows
{
    public class SettingsManager
    {
        private const string SettingsFilePath = "calculator_settings.json";

        // Metoda pentru salvarea setărilor
        public void SaveSettings(CalculatorSettings settings)
        {
            string json = JsonConvert.SerializeObject(settings);
            File.WriteAllText(SettingsFilePath, json);
        }

        // Metoda pentru încărcarea setărilor
        public CalculatorSettings LoadSettings()
        {
            if (File.Exists(SettingsFilePath))
            {
                string json = File.ReadAllText(SettingsFilePath);
                return JsonConvert.DeserializeObject<CalculatorSettings>(json);
            }
            return new CalculatorSettings(); // Returnează setări implicite dacă fișierul nu există
        }
    }
}
