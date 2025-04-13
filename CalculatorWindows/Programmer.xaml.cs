using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CalculatorWindows
{
    public partial class Programmer : Window
    {
        private string currentInput = "0";
        private string currentBase = "DEC"; // Baza curentă (HEX, DEC, OCT, BIN)
        private Stack<string> stack = new Stack<string>();
        private List<string> memoryStorage = new List<string>();
        private string currentExpression = ""; // Stochează întreaga expresie
        private const string SettingsFilePath = "calculator_settings.json";
        private MemoryManager memoryManager = new MemoryManager();
        private SettingsManager settingsManager = new SettingsManager();
        //private ModeSelectorManager modeSelectorManager = new ModeSelectorManager();

        // Listă de butoane care trebuie blocate în funcție de bază
        private List<Button> buttonsToDisable = new List<Button>();

        public Programmer()
        {
            InitializeComponent();
            LoadSettings();
            UpdateDisplay();
            InitializeButtonsToDisable();
            UpdateButtonStates();
            this.KeyDown += Programmer_KeyDown; // Asigură-te că această linie există 
            this.MouseRightButtonUp += Programmer_MouseRightButtonUp;
            //ModeSelector.SelectionChanged += (sender, e) => modeSelectorManager.ModeSelector_SelectionChanged(sender, e, this);

        }

        private void SaveSettings()
        {
            var settings = new CalculatorSettings
            {
                LastMode = "Programmer",
                LastBase = currentBase
            };

            settingsManager.SaveSettings(settings);
        }

        private void LoadSettings()
        {
            var settings = settingsManager.LoadSettings();

            if (settings.LastMode == "Programmer")
            {
                currentBase = settings.LastBase;
                UpdateButtonStates();
                UpdateDisplay();
            }
        }

        private void Programmer_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Creăm un meniu contextual
            ContextMenu contextMenu = new ContextMenu();

            // Adăugăm opțiuni pentru Copy, Cut, Paste
            MenuItem copyMenuItem = new MenuItem();
            copyMenuItem.Header = "Copy";
            copyMenuItem.Click += (s, args) => Copy_Click(null, null);
            contextMenu.Items.Add(copyMenuItem);

            MenuItem cutMenuItem = new MenuItem();
            cutMenuItem.Header = "Cut";
            cutMenuItem.Click += (s, args) => Cut_Click(null, null);
            contextMenu.Items.Add(cutMenuItem);

            MenuItem pasteMenuItem = new MenuItem();
            pasteMenuItem.Header = "Paste";
            pasteMenuItem.Click += (s, args) => Paste_Click(null, null);
            contextMenu.Items.Add(pasteMenuItem);

            // Afișăm meniul contextual la poziția cursorului
            contextMenu.IsOpen = true;
        }

        private void Programmer_KeyDown(object sender, KeyEventArgs e)
        {
            // Verificăm dacă tasta Ctrl este apăsată pentru operațiuni de Copy, Cut, Paste
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                switch (e.Key)
                {
                    case Key.C: // Ctrl+C pentru Copy
                        Copy_Click(null, null);
                        e.Handled = true; // Marchează evenimentul ca fiind gestionat
                        break;
                    case Key.X: // Ctrl+X pentru Cut
                        Cut_Click(null, null);
                        e.Handled = true;
                        break;
                    case Key.V: // Ctrl+V pentru Paste
                        Paste_Click(null, null);
                        e.Handled = true;
                        break;
                }
            }
            else
            {
                // Gestionăm tastele individuale pentru numere, operatori și ștergere
                switch (e.Key)
                {
                    case Key.D0:
                    case Key.NumPad0:
                        NumberButton_Click(ZeroButton, null);
                        e.Handled = true;
                        break;
                    case Key.D1:
                    case Key.NumPad1:
                        NumberButton_Click(OneButton, null);
                        e.Handled = true;
                        break;
                    case Key.D2:
                    case Key.NumPad2:
                        NumberButton_Click(TwoButton, null);
                        e.Handled = true;
                        break;
                    case Key.D3:
                    case Key.NumPad3:
                        NumberButton_Click(ThreeButton, null);
                        e.Handled = true;
                        break;
                    case Key.D4:
                    case Key.NumPad4:
                        NumberButton_Click(FourButton, null);
                        e.Handled = true;
                        break;
                    case Key.D5:
                    case Key.NumPad5:
                        NumberButton_Click(FiveButton, null);
                        e.Handled = true;
                        break;
                    case Key.D6:
                    case Key.NumPad6:
                        NumberButton_Click(SixButton, null);
                        e.Handled = true;
                        break;
                    case Key.D7:
                    case Key.NumPad7:
                        NumberButton_Click(SevenButton, null);
                        e.Handled = true;
                        break;
                    case Key.D8:
                    case Key.NumPad8:
                        NumberButton_Click(EightButton, null);
                        e.Handled = true;
                        break;
                    case Key.D9:
                    case Key.NumPad9:
                        NumberButton_Click(NineButton, null);
                        e.Handled = true;
                        break;
                    case Key.A:
                        NumberButton_Click(AButton, null);
                        e.Handled = true;
                        break;
                    case Key.B:
                        NumberButton_Click(BButton, null);
                        e.Handled = true;
                        break;
                    case Key.C:
                        NumberButton_Click(CButton, null);
                        e.Handled = true;
                        break;
                    case Key.D:
                        NumberButton_Click(DButton, null);
                        e.Handled = true;
                        break;
                    case Key.E:
                        NumberButton_Click(EButton, null);
                        e.Handled = true;
                        break;
                    case Key.F:
                        NumberButton_Click(FButton, null);
                        e.Handled = true;
                        break;
                    case Key.Add:
                    case Key.OemPlus:
                        OperatorButton_Click(PlusButton, null);
                        e.Handled = true;
                        break;
                    case Key.Subtract:
                    case Key.OemMinus:
                        OperatorButton_Click(MinusButton, null);
                        e.Handled = true;
                        break;
                    case Key.Multiply:
                        OperatorButton_Click(MultiplyButton, null);
                        e.Handled = true;
                        break;
                    case Key.Divide:
                        OperatorButton_Click(DivideButton, null);
                        e.Handled = true;
                        break;
                    case Key.Enter:
                        EqualsButton_Click(EqualsButton, null);
                        e.Handled = true;
                        break;
                    case Key.Escape: // ESC pentru ștergere
                        ClearButton_Click(ClearButton, null);
                        e.Handled = true;
                        break;
                    case Key.Back: // Backspace pentru ștergerea ultimului caracter
                        BackspaceButton_Click(BackspaceButton, null);
                        e.Handled = true;
                        break;
                }
            }
        }

        private void InitializeButtonsToDisable()
        {
            // Adăugăm toate butoanele care pot fi blocate în funcție de bază
            buttonsToDisable.AddRange(new[]
            {
                AButton, BButton, CButton, DButton, EButton, FButton,
                EightButton, NineButton
            });
        }

        private void UpdateButtonStates()
        {
            // Lista de butoane care pot fi blocate în funcție de bază
            var buttonsToDisable = new List<Button>
    {
        AButton, BButton, CButton, DButton, EButton, FButton,
        TwoButton, ThreeButton, FourButton, FiveButton, SixButton, SevenButton, EightButton, NineButton
    };

            // Reactivăm toate butoanele și resetăm culoarea
            foreach (var button in buttonsToDisable)
            {
                button.IsEnabled = true;
                button.Background = Brushes.Transparent; // Resetăm culoarea de fundal
            }

            // Dezactivăm butoanele care nu sunt valide în baza curentă
            switch (currentBase)
            {
                case "BIN":
                    // În baza BIN, blochează butoanele A-F, 2-9
                    foreach (var button in buttonsToDisable)
                    {
                        if (button.Content.ToString() == "2" || button.Content.ToString() == "3" ||
                            button.Content.ToString() == "4" || button.Content.ToString() == "5" ||
                            button.Content.ToString() == "6" || button.Content.ToString() == "7" ||
                            button.Content.ToString() == "8" || button.Content.ToString() == "9" ||
                            IsHexDigit(button.Content.ToString()[0]))
                        {
                            button.IsEnabled = false;
                            button.Background = Brushes.Red; // Colorăm în rosu
                        }
                    }
                    break;

                case "OCT":
                    // În baza OCT, blochează butoanele A-F, 8, 9
                    foreach (var button in buttonsToDisable)
                    {
                        if (button.Content.ToString() == "8" || button.Content.ToString() == "9" ||
                            IsHexDigit(button.Content.ToString()[0]))
                        {
                            button.IsEnabled = false;
                            button.Background = Brushes.Red; // Colorăm în rosu
                        }
                    }
                    break;

                case "DEC":
                    // În baza DEC, blochează butoanele A-F
                    foreach (var button in buttonsToDisable)
                    {
                        if (IsHexDigit(button.Content.ToString()[0]))
                        {
                            button.IsEnabled = false;
                            button.Background = Brushes.Red; // Colorăm în rosu
                        }
                    }
                    break;

                case "HEX":
                    // În baza HEX, toate butoanele sunt valide
                    break;
            }
        }

        private bool IsHexDigit(char c)
        {
            return (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f');
        }

        private void UpdateDisplay()
        {
            // Dacă suntem în modul binar și input-ul este gol, afișăm un șir gol
            if (currentBase == "BIN" && string.IsNullOrEmpty(currentInput))
            {
                DisplayText.Text = "";
            }
            else
            {
                // În celelalte baze, afișăm "0" dacă input-ul este gol
                DisplayText.Text = string.IsNullOrEmpty(currentInput) ? "0" : currentInput;
            }

            // Actualizăm valorile în alte baze
            UpdateBaseValues();
        }

        private void UpdateBaseValues()
        {
            try
            {
                if (currentBase == "BIN")
                {
                    // Pentru baza BIN, afișăm input-ul exact așa cum a fost introdus
                    BinValue.Text = currentInput; // Afișăm input-ul original

                    // Convertim șirul binar într-un număr întreg doar pentru alte baze
                    if (!string.IsNullOrEmpty(currentInput))
                    {
                        long value = Convert.ToInt64(currentInput, 2); // Conversie din binar în long
                        HexValue.Text = Convert.ToString(value, 16).ToUpper();
                        DecValue.Text = value.ToString();
                        OctValue.Text = Convert.ToString(value, 8);
                    }
                    else
                    {
                        HexValue.Text = "0";
                        DecValue.Text = "0";
                        OctValue.Text = "0";
                    }
                }
                else
                {
                    // Pentru celelalte baze, convertim input-ul în număr și afișăm valorile corespunzătoare
                    long value = Convert.ToInt64(currentInput, GetBase(currentBase));
                    HexValue.Text = Convert.ToString(value, 16).ToUpper();
                    DecValue.Text = value.ToString();
                    OctValue.Text = Convert.ToString(value, 8);
                    BinValue.Text = Convert.ToString(value, 2);
                }
            }
            catch
            {
                HexValue.Text = "0";
                DecValue.Text = "0";
                OctValue.Text = "0";
                BinValue.Text = "0";
            }
        }

        private int GetBase(string baseType)
        {
            switch (baseType)
            {
                case "HEX": return 16;
                case "DEC": return 10;
                case "OCT": return 8;
                case "BIN": return 2;
                default: return 10;
            }
        }

        private void ModeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ModeSelector.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedMode = selectedItem.Content.ToString();

                if (selectedMode == "Standard")
                {
                    SaveSettings();
                    MainWindow standardWindow = new MainWindow();
                    standardWindow.Show();
                    this.Close();
                }
            }
        }

        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string number = button.Content.ToString();

            // Verificăm dacă numărul este valid pentru baza curentă
            if (!IsValidInput(number, GetBase(currentBase)))
            {
                MessageBox.Show($"Invalid input for base {currentBase}.", "Error");
                return;
            }

            // Dacă baza curentă este BIN, verificăm dacă numărul este 0 sau 1
            if (currentBase == "BIN" && number != "0" && number != "1")
            {
                MessageBox.Show("Invalid input for binary base. Only 0 and 1 are allowed.", "Error");
                return;
            }

            // Dacă input-ul curent este "0" și se apasă "0", adăugăm "0" (pentru a permite zerouri în față)
            if (currentInput == "0" && number == "0")
            {
                currentInput += number; // Adăugăm "0" la input-ul existent
            }
            else if (currentInput == "0" && number != "0")
            {
                currentInput = number; // Înlocuim "0" cu numărul nou
            }
            else
            {
                currentInput += number; // Adăugăm numărul la input-ul existent
            }

            // Actualizăm afișajul
            UpdateDisplay();
        }

        private void OperatorButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string op = button.Content.ToString();

            // Adăugăm numărul curent și operatorul la expresie
            currentExpression += currentInput + " " + op + " ";
            currentInput = "0"; // Resetăm input-ul pentru următorul număr

            UpdateDisplay();
        }

        private void EqualsButton_Click(object sender, RoutedEventArgs e)
        {
            // Adăugăm ultimul număr la expresie
            currentExpression += currentInput;

            try
            {
                // Evaluăm expresia folosind forma poloneză inversată
                string result = EvaluateExpression(currentExpression);
                currentInput = result;
                currentExpression = ""; // Resetăm expresia
                UpdateDisplay();
            }
            catch
            {
                currentInput = "Error";
                currentExpression = "";
                UpdateDisplay();
            }
        }

        private string EvaluateExpression(string expression)
        {
            try
            {
                // Convertim expresia în forma poloneză inversată (RPN)
                var rpn = ConvertToRPN(expression);
                // Evaluăm RPN
                return EvaluateRPN(rpn);
            }
            catch (DivideByZeroException ex)
            {
                MessageBox.Show(ex.Message, "Error");
                return "Error";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid expression.", "Error");
                return "Error";
            }
        }

        private List<string> ConvertToRPN(string expression)
        {
            var output = new List<string>();
            var operators = new Stack<string>();

            foreach (var token in expression.Split(' '))
            {
                if (string.IsNullOrWhiteSpace(token))
                    continue;

                if (IsNumber(token))
                {
                    output.Add(token);
                }
                else if (IsOperator(token))
                {
                    while (operators.Count > 0 && IsOperator(operators.Peek()))
                    {
                        output.Add(operators.Pop());
                    }
                    operators.Push(token);
                }
                else
                {
                    throw new InvalidOperationException("Invalid token in expression.");
                }
            }

            while (operators.Count > 0)
            {
                output.Add(operators.Pop());
            }

            return output;
        }

        private string EvaluateRPN(List<string> rpn)
        {
            Stack<string> numbers = new Stack<string>();

            foreach (var token in rpn)
            {
                if (IsNumber(token))
                {
                    numbers.Push(token);
                }
                else if (IsOperator(token))
                {
                    string b = numbers.Pop();
                    string a = numbers.Pop();

                    // Convertim numerele în double pentru calcule
                    double aDec = ConvertToDouble(a, GetBase(currentBase));
                    double bDec = ConvertToDouble(b, GetBase(currentBase));
                    double resultDec = 0;

                    switch (token)
                    {
                        case "+": resultDec = aDec + bDec; break;
                        case "-": resultDec = aDec - bDec; break;
                        case "×": resultDec = aDec * bDec; break;
                        case "÷":
                            if (bDec == 0)
                            {
                                throw new DivideByZeroException("Division by zero is not allowed.");
                            }
                            resultDec = aDec / bDec;
                            break;
                        case "%": resultDec = aDec % bDec; break;
                        default: throw new InvalidOperationException("Invalid operator");
                    }

                    // Convertim rezultatul înapoi în baza curentă
                    string result = ConvertFromDouble(resultDec, GetBase(currentBase));

                    numbers.Push(result);
                }
            }

            return numbers.Pop();
        }

        private double ConvertToDouble(string number, int baseType)
        {
            if (baseType == 10)
            {
                return double.Parse(number);
            }
            else
            {
                // Pentru bazele non-zecimale, convertim întâi în baza 10
                long integerPart = Convert.ToInt64(number, baseType);
                return (double)integerPart;
            }
        }

        private string ConvertFromDouble(double number, int baseType)
        {
            if (baseType == 10)
            {
                return number.ToString();
            }
            else
            {
                // Pentru bazele non-zecimale, convertim întâi în întreg
                long integerPart = (long)number;
                return Convert.ToString(integerPart, baseType);
            }
        }

        private bool IsNumber(string token)
        {
            try
            {
                // Verificăm dacă șirul este un număr valid în baza curentă
                if (currentBase == "DEC")
                {
                    double.Parse(token); // Verificăm dacă este un număr zecimal valid
                }
                else
                {
                    Convert.ToInt64(token, GetBase(currentBase)); // Verificăm pentru bazele non-zecimale
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool IsOperator(string token)
        {
            return token == "+" || token == "-" || token == "×" || token == "÷" || token == "%";
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentBase == "BIN")
            {
                currentInput = ""; // Resetați la șir gol în modul binar
            }
            else
            {
                currentInput = "0"; // Resetați la "0" în celelalte baze
            }

            currentExpression = ""; // Resetați expresia curentă
            stack.Clear(); // Goliți stiva
            UpdateDisplay(); // Actualizați afișajul
        }

        private void BackspaceButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentInput.Length > 1)
            {
                currentInput = currentInput.Substring(0, currentInput.Length - 1); // Ștergeți ultimul caracter
            }
            else
            {
                if (currentBase == "BIN")
                {
                    currentInput = ""; // Resetați la șir gol în modul binar
                }
                else
                {
                    currentInput = "0"; // Resetați la "0" în celelalte baze
                }
            }

            UpdateDisplay(); // Actualizați afișajul
        }

        private void BaseButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            currentBase = button.Content.ToString();

            // Dacă trecem în modul binar, resetăm input-ul la gol
            if (currentBase == "BIN")
            {
                currentInput = "";
            }
            else
            {
                // În celelalte baze, resetăm input-ul la "0"
                currentInput = "0";
            }

            // Actualizăm starea butoanelor și afișajul
            UpdateButtonStates();
            UpdateDisplay();
            SaveSettings();
        }

        private void PlusMinusButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentInput != "0")
            {
                if (currentInput.StartsWith("-"))
                    currentInput = currentInput.Substring(1);
                else
                    currentInput = "-" + currentInput;

                UpdateDisplay();
            }
        }

        private void DecimalButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentBase == "DEC" && !currentInput.Contains("."))
            {
                currentInput += ".";
                UpdateDisplay();
            }
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(currentInput))
            {
                Clipboard.SetText(currentInput);
            }
        }

        private void Cut_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(currentInput))
            {
                Clipboard.SetText(currentInput); // Copiază valoarea curentă în clipboard
                currentInput = "0"; // Resetează input-ul curent
                UpdateDisplay(); // Actualizează afișajul
            }
        }

        private void Paste_Click(object sender, RoutedEventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                string clipboardText = Clipboard.GetText();
                if (IsValidInput(clipboardText, GetBase(currentBase)))
                {
                    currentInput = clipboardText; // Inserează textul din clipboard
                    UpdateDisplay(); // Actualizează afișajul
                }
                else
                {
                    MessageBox.Show("Invalid input for the current base.", "Error");
                }
            }
        }

        private void DataGrouping_Click(object sender, RoutedEventArgs e)
        {
            // Implement data grouping logic here
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Munteanu Marius-Iulian");
        }

        // Handler pentru butonul "MS" (Memory Store)
        private void MS_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Convertim input-ul curent într-un număr și îl stocăm în memorie
                double value = Convert.ToDouble(currentInput);
                memoryManager.Store(value);
                MessageBox.Show("Value saved to memory.", "Memory Store");
            }
            catch
            {
                MessageBox.Show("Invalid value for memory storage.", "Error");
            }
        }

        // Handler pentru butonul "Mv" (Memory Recall)
        private void Mv_Click(object sender, RoutedEventArgs e)
        {
            // Afișăm conținutul memoriei folosind metoda View() din MemoryManager
            string memoryContent = memoryManager.View();
            MessageBox.Show(memoryContent, "Memory View");
        }

        // Handler pentru butonul "%" (Procent)
        private void PercentButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                long value = Convert.ToInt64(currentInput, GetBase(currentBase));
                long percentValue = value / 100; // Calculăm 1% din valoare
                currentInput = percentValue.ToString();
                UpdateDisplay();
            }
            catch
            {
                currentInput = "Error";
                UpdateDisplay();
            }
        }

        private bool IsValidInput(string input, int baseType)
        {
            foreach (char c in input)
            {
                if (baseType == 2 && (c != '0' && c != '1'))
                    return false; // Invalid pentru baza 2
                if (baseType == 8 && (c < '0' || c > '7'))
                    return false; // Invalid pentru baza 8
                if (baseType == 10 && (c < '0' || c > '9'))
                    return false; // Invalid pentru baza 10
                if (baseType == 16 && !((c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f')))
                    return false; // Invalid pentru baza 16
            }
            return true; // Input valid
        }
    }
}