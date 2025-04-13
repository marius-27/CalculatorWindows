using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace CalculatorWindows
{
    public class ModeSelectorManager
    {
        public void ModeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e, Window currentWindow)
        {
            if (sender is ComboBox modeSelector && modeSelector.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedMode = selectedItem.Content.ToString();

                switch (selectedMode)
                {
                    case "Standard":
                        OpenStandardMode(currentWindow);
                        break;
                    case "Programmer":
                        OpenProgrammerMode(currentWindow);
                        break;
                    default:
                        // Poți adăuga un mesaj de eroare sau un log aici dacă este necesar
                        break;
                }
            }
        }

        private void OpenStandardMode(Window currentWindow)
        {
            MainWindow standardWindow = new MainWindow();
            standardWindow.Show();
            currentWindow.Close();
        }

        private void OpenProgrammerMode(Window currentWindow)
        {
            Programmer programmerWindow = new Programmer();
            programmerWindow.Show();
            currentWindow.Close();
        }
    }
}
