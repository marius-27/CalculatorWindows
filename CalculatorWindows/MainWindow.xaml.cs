using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;

namespace CalculatorWindows
{
    public partial class MainWindow : Window
    {
        private Stack<double> rpnStack = new Stack<double>(); // Stiva pentru numere 
        private MemoryManager memoryManager = new MemoryManager(); // Memoria calculatorului 
        private string lastOperator = ""; // Ultimul operator apasat
        private string currentExpression = ""; // Expresia curenta
        private string currentNumber = ""; // Numarul curent
        private bool isDataGroupingActive = false;
        private double lastResult = 0; // Stocheaza rezultatul intermediar
        private bool isCascadeOperationActive = false; // Starea Cascade Operation (implicit dezactivat)
        private CalculatorOperations calculatorOperations = new CalculatorOperations();
        private SettingsManager settingsManager = new SettingsManager();
        //private ModeSelectorManager modeSelectorManager = new ModeSelectorManager();

        public MainWindow()
        {
            InitializeComponent();
            LoadSettings();
            DisplayText.PreviewKeyDown += DisplayText_PreviewKeyDown;
            DisplayText.ContextMenu = new ContextMenu(); 
            this.KeyDown += MainWindow_KeyDown;
            //ModeSelector.SelectionChanged += (sender, e) => modeSelectorManager.ModeSelector_SelectionChanged(sender, e, this);
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.D1:
                case Key.NumPad1:
                    Number_Click(Button1, null);
                    break;
                case Key.D2:
                case Key.NumPad2:
                    Number_Click(Button2, null);
                    break;
                case Key.D3:
                case Key.NumPad3:
                    Number_Click(Button3, null); 
                    break;
                case Key.D4:
                case Key.NumPad4:
                    Number_Click(Button4, null); 
                    break;
                case Key.D5:
                case Key.NumPad5:
                    Number_Click(Button5, null); 
                    break;
                case Key.D6:
                case Key.NumPad6:
                    Number_Click(Button6, null); 
                    break;
                case Key.D7:
                case Key.NumPad7:
                    Number_Click(Button7, null);
                    break;
                case Key.D8:
                case Key.NumPad8:
                    Number_Click(Button8, null); 
                    break;
                case Key.D9:
                case Key.NumPad9:
                    Number_Click(Button9, null); 
                    break;
                case Key.D0:
                case Key.NumPad0:
                    Number_Click(Button0, null); 
                    break;
                case Key.Add:
                    Operator_Click(ButtonAdd, null); 
                    break;
                case Key.Subtract:
                    // Verifica daca `-` trebuie tratat ca parte a unui numar negativ sau ca operator
                    if (string.IsNullOrEmpty(currentNumber) || currentExpression.EndsWith(" "))
                    {
                        currentNumber += "-";
                        currentExpression += "-";
                        UpdateDisplay();
                    }
                    else
                    {
                        // Trateaza `-` ca operator
                        Operator_Click(ButtonSubtract, null);
                    }
                    break;
                case Key.Multiply:
                    Operator_Click(ButtonMultiply, null); 
                    break;
                case Key.Divide:
                    Operator_Click(ButtonDivide, null);
                    break;
                case Key.Back:
                    Backspace_Click(null, null); 
                    break;
                case Key.Escape:
                    Clear_Click(null, null);
                    DisplayText.Text = "0"; 
                    break;
                case Key.OemPeriod:
                case Key.Decimal:
                    Decimal_Click(ButtonDecimal, null); 
                    break;
                case Key.OemPlus:
                case Key.Enter:
                    Equals_Click(ButtonEquals, null); 
                    break;
            }
        }

        // Handler pentru evenimentul de clipboard (Ctrl+C, Ctrl+X, Ctrl+V)
        private void DisplayText_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.C && Keyboard.Modifiers == ModifierKeys.Control) // Ctrl+C
            {
                Copy_Click(sender, e);
                e.Handled = true; // Oprire propagare eveniment
            }
            else if (e.Key == Key.X && Keyboard.Modifiers == ModifierKeys.Control) // Ctrl+X
            {
                Cut_Click(sender, e);
                e.Handled = true;
            }
            else if (e.Key == Key.V && Keyboard.Modifiers == ModifierKeys.Control) // Ctrl+V
            {
                Paste_Click(sender, e);
                e.Handled = true;
            }
        }

        // Handler pentru butonul "Copy"
        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(DisplayText.Text))
            {
                Clipboard.SetText(DisplayText.Text); 
            }
        }

        // Handler pentru butonul "Cut"
        private void Cut_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(DisplayText.Text))
            {
                Clipboard.SetText(DisplayText.Text); 
                DisplayText.Text = ""; // Sterge textul din afisaj
                currentExpression = ""; // Reseteaza expresia curenta
                currentNumber = ""; // Reseteaza numarul curent
            }
        }

        // Handler pentru butonul "Paste"
        private void Paste_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Clipboard.ContainsText())
                {
                    string clipboardText = Clipboard.GetText();
                    if (double.TryParse(clipboardText, NumberStyles.Any, CultureInfo.CurrentCulture, out _)) // Verificam dacă textul este un numar valid
                    {
                        DisplayText.Text = clipboardText; // Adauga textul in afisaj
                        currentExpression = clipboardText; // Actualizeaza expresia curenta
                        currentNumber = clipboardText; // Actualizează numarul curent
                    }
                    else
                    {
                        MessageBox.Show("Clipboard does not contain a valid number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while pasting from clipboard.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Handler pentru butonul "Data Grouping"
        private void DataGrouping_Click(object sender, RoutedEventArgs e)
        {
            isDataGroupingActive = !isDataGroupingActive; // Comuta starea Data Grouping

            if (isDataGroupingActive)
            {
                // Aplica Data Grouping la numarul curent
                if (double.TryParse(DisplayText.Text, NumberStyles.Any, CultureInfo.CurrentCulture, out double value))
                {
                    string formattedValue = value.ToString("N2", CultureInfo.CurrentCulture); // Formateaza cu delimitatori
                    DisplayText.Text = formattedValue; // Afiseaza valoarea formatata
                    currentExpression = formattedValue; // Actualizeaza expresia curenta
                    currentNumber = value.ToString(CultureInfo.InvariantCulture); // Păstreaza valoarea numerica pentru calcule
                }
            }
            else
            {
                // Dezactivează Data Grouping si afișează numarul fara delimitatori
                if (double.TryParse(DisplayText.Text, NumberStyles.Any, CultureInfo.CurrentCulture, out double value))
                {
                    DisplayText.Text = value.ToString(CultureInfo.InvariantCulture); // Afiseaza valoarea fara delimitatori
                    currentExpression = value.ToString(CultureInfo.InvariantCulture); // Actualizeaza expresia curenta
                    currentNumber = value.ToString(CultureInfo.InvariantCulture); // Păstreaza valoarea numerica pentru calcule
                }
            }
            SaveSettings();
        }

        private void SaveSettings()
        {
            var settings = new CalculatorSettings
            {
                LastMode = "Standard", // Sau orice alt mod
                LastBase = "DEC",      // Sau orice altă bază
                IsDigitGroupingActive = isDataGroupingActive // Starea curentă a Digit Grouping
            };

            settingsManager.SaveSettings(settings);
        }

        private void LoadSettings()
        {
            var settings = settingsManager.LoadSettings();

            // Aplică starea optiunii Digit Grouping
            isDataGroupingActive = settings.IsDigitGroupingActive;

            // Actualizează afișajul în funcție de starea încărcată
            if (isDataGroupingActive && double.TryParse(DisplayText.Text, NumberStyles.Any, CultureInfo.CurrentCulture, out double value))
            {
                DisplayText.Text = value.ToString("N2", CultureInfo.CurrentCulture);
            }
        }

        // Handler pentru butonul "About"
        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Munteanu Marius-Iulian 10LF332", "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Handler pentru schimbarea modului (de exemplu, Programmer, Scientific)
        private void ModeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ModeSelector.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedMode = selectedItem.Content.ToString();

                switch (selectedMode)
                {
                    case "Programmer":
                        Programmer programmerWindow = new Programmer();
                        programmerWindow.Show();
                        this.Close();
                        break;
                    case "Scientific":
                        break;
                    default:
                        break;
                }
            }
        }

        // Handler pentru butoanele de numere (0-9)
        private void Number_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button)
                {
                    if (button.Content.ToString() == ".")
                    {
                        if (!currentNumber.Contains("."))
                        {
                            currentNumber += button.Content.ToString();
                            currentExpression += button.Content.ToString();
                        }
                    }
                    else
                    {
                        currentNumber += button.Content.ToString();
                        currentExpression += button.Content.ToString();
                    }
                    UpdateDisplay();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid input. Please enter a valid number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Handler pentru butoanele de operații (+, -, ×, ÷)
        private void Operator_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button)
                {
                    if (!string.IsNullOrEmpty(currentExpression))
                    {
                        // Formatează numărul curent în funcție de starea Data Grouping
                        if (double.TryParse(currentNumber, NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
                        {
                            string formattedNumber = isDataGroupingActive
                                ? value.ToString("N2", CultureInfo.CurrentCulture) // Cu Data Grouping
                                : value.ToString(CultureInfo.InvariantCulture); // Fără Data Grouping

                            currentExpression = currentExpression.Replace(currentNumber, formattedNumber);
                        }

                        // Dacă Cascade Operation este activ, calculează și afișează rezultatul intermediar
                        if (isCascadeOperationActive)
                        {
                            lastResult = calculatorOperations.CalculateExpression(currentExpression);
                            DisplayText.Text = lastResult.ToString();
                            currentExpression = lastResult.ToString();
                            currentNumber = "";
                        }

                        // Adaugă operatorul la expresie
                        currentExpression += " " + button.Content.ToString() + " ";
                        UpdateDisplay();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while processing the operator.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Metoda pentru calcularea expresiei
        private double CalculateExpression(string expression)
        {
            try
            {
                expression = expression
                    .Replace("×", "*")
                    .Replace("÷", "/")
                    .Replace(CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, "")
                    .Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, ".");

                var result = new System.Data.DataTable().Compute(expression, null);
                return Convert.ToDouble(result);
            }
            catch (DivideByZeroException)
            {
                MessageBox.Show("Division by zero is not allowed.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while calculating the expression.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return 0;
            }
        }

        // Handler pentru butonul "="
        private void Equals_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Elimină spațiile din expresie înainte de calcul
                string expressionToCalculate = currentExpression.Replace(" ", "");

                // Calculează rezultatul final folosind clasa CalculatorOperations
                lastResult = calculatorOperations.CalculateExpression(expressionToCalculate);
                DisplayText.Text = lastResult.ToString();
                currentExpression = lastResult.ToString();
                currentNumber = "";
            }
            catch (Exception ex)
            {
                DisplayText.Text = "Error";
                currentExpression = "";
                currentNumber = "";
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Metoda pentru actualizarea afisajului
        private void UpdateDisplay()
        {
            if (double.TryParse(currentExpression, NumberStyles.Any, CultureInfo.CurrentCulture, out double value))
            {
                // Formatează numărul în funcție de starea Data Grouping
                string formattedValue = isDataGroupingActive
                    ? value.ToString("N2", CultureInfo.CurrentCulture) // Cu Data Grouping
                    : value.ToString(CultureInfo.InvariantCulture); // Fără Data Grouping

                DisplayText.Text = formattedValue;
            }
            else
            {
                DisplayText.Text = currentExpression;
            }
        }

        // Handler pentru butonul "Enter"
        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (double.TryParse(DisplayText.Text, out double num))
                {
                    rpnStack.Push(num);
                    DisplayText.Text = ""; 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while pushing to the stack.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Handler pentru butonul "C" (clear)
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            DisplayText.Text = "0";
            rpnStack.Clear();
            lastOperator = "";
            currentExpression = "";
            currentNumber = "";
        }

        // Handler pentru butonul "CE" (clear entry)
        private void ClearEntry_Click(object sender, RoutedEventArgs e)
        {
            DisplayText.Text = ""; 
            currentExpression = ""; 
            currentNumber = ""; 
        }

        // Handler pentru butonul "Backspace"
        private void Backspace_Click(object sender, RoutedEventArgs e)
        {
            if (currentExpression.Length > 0)
            {
                currentExpression = currentExpression[..^1]; // Șterge ultimul caracter
                if (currentNumber.Length > 0)
                {
                    currentNumber = currentNumber[..^1]; // Șterge ultimul caracter din numărul curent
                }
                UpdateDisplay();
            }
        }

        // Handler pentru butonul "Memory Store" 
        private void MemoryStore_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (double.TryParse(DisplayText.Text, out double value))
                {
                    memoryManager.Store(value); 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while storing to memory.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Handler pentru butonul "Memory Recall" 
        private void MemoryRecall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double lastValue = memoryManager.Recall(); // Folosește MemoryManager pentru a recupera valoarea
                DisplayText.Text = lastValue.ToString();
                currentExpression = lastValue.ToString();
                currentNumber = lastValue.ToString();
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Memory is empty.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while recalling memory.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Handler pentru butonul "Memory Clear"
        private void MemoryClear_Click(object sender, RoutedEventArgs e)
        {
            memoryManager.Clear(); // Folosește MemoryManager pentru a sterge memoria
        }

        // Handler pentru butonul "Memory Add" 
        private void MemoryAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (double.TryParse(DisplayText.Text, out double value))
                {
                    memoryManager.Add(value); // Folosește MemoryManager pentru a adauga valoarea
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while adding to memory.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Handler pentru butonul "Memory Subtract" 
        private void MemorySubtract_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (double.TryParse(DisplayText.Text, out double value))
                {
                    memoryManager.Subtract(value); // Folosește MemoryManager pentru a scadea valoarea
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while subtracting from memory.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Handler pentru butonul "Memory View" 
        private void MemoryView_Click(object sender, RoutedEventArgs e)
        {
            DisplayText.Text = memoryManager.View(); 
        }

        // Handler pentru butonul "%" (procentul)
        private void Percent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (double.TryParse(DisplayText.Text, out double value))
                {
                    // Calculează procentul folosind clasa CalculatorOperations
                    double result = calculatorOperations.CalculatePercentage(value);
                    DisplayText.Text = result.ToString();
                    currentExpression = result.ToString();
                    currentNumber = result.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Handler pentru butonul "1/x" 
        private void Reciprocal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(currentNumber) && double.TryParse(currentNumber, out double value))
                {
                    // Calculează reciprocul folosind clasa CalculatorOperations
                    double result = calculatorOperations.CalculateReciprocal(value);
                    currentNumber = result.ToString();
                    currentExpression = ReplaceLastNumber(currentExpression, result.ToString());
                    UpdateDisplay();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Handler pentru butonul "x^2"
        private void Square_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(currentNumber) && double.TryParse(currentNumber, out double value))
                {
                    // Calculează pătratul folosind clasa CalculatorOperations
                    double result = calculatorOperations.CalculateSquare(value);
                    currentNumber = result.ToString();
                    currentExpression = ReplaceLastNumber(currentExpression, result.ToString());
                    UpdateDisplay();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Handler pentru butonul "sqrt" 
        private void SquareRoot_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(currentNumber) && double.TryParse(currentNumber, out double value))
                {
                    // Calculează rădăcina pătrată folosind clasa CalculatorOperations
                    double result = calculatorOperations.CalculateSquareRoot(value);
                    currentNumber = result.ToString();
                    currentExpression = ReplaceLastNumber(currentExpression, result.ToString());
                    UpdateDisplay();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Handler pentru butonul "+/-"
        private void Negate_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(currentExpression))
            {
                if (currentExpression.EndsWith(" "))
                {
                    currentExpression += "-";
                }
                else
                {
                    if (currentExpression.EndsWith("-"))
                    {
                        currentExpression = currentExpression[..^1]; 
                    }
                    else
                    {
                        currentExpression += "-"; 
                    }
                }
                UpdateDisplay(); 
            }
        }

        private void Decimal_Click(object sender, RoutedEventArgs e)
        {
            if (!currentNumber.Contains("."))
            {
                currentNumber += "."; 
                currentExpression += "."; 
                UpdateDisplay(); // Actualizează afisajul
            }
        }

        // Handler pentru butonul "Show Memory" 
        private void ShowMemory_Click(object sender, RoutedEventArgs e)
        {
            DisplayText.Text = memoryManager.View(); 
        }

        private string ReplaceLastNumber(string expression, string newNumber)
        {
            var regex = new System.Text.RegularExpressions.Regex(@"(\d+(\.\d+)?)$");
            return regex.Replace(expression, newNumber);
        }

        // Handler pentru butonul "Cascade Operation"
        private void CascadeOperation_Click(object sender, RoutedEventArgs e)
        {
            isCascadeOperationActive = !isCascadeOperationActive; 

            if (isCascadeOperationActive)
            {
                MessageBox.Show("Cascade Operation is now active.", "Cascade Operation", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Cascade Operation is now inactive.", "Cascade Operation", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        //private string FormatNumberWithGrouping(string number)
        //{
        //    if (double.TryParse(number, NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
        //    {
        //        return value.ToString("N2", CultureInfo.CurrentCulture); // Formatează cu delimitatori
        //    }
        //    return number; // Dacă nu este un număr valid, returnează textul original
        //}

        //// Metoda pentru evaluarea funcțiilor matematice
        //private string EvaluateFunctions(string expression)
        //{
        //    expression = System.Text.RegularExpressions.Regex.Replace(
        //        expression,
        //        @"1/(\d+(\.\d+)?)",
        //        match => "(1 / " + match.Groups[1].Value + ")"
        //    );

        //    expression = System.Text.RegularExpressions.Regex.Replace(
        //        expression,
        //        @"(\d+(\.\d+)?)%",
        //        match => "(" + match.Groups[1].Value + " / 100)"
        //    );

        //    expression = System.Text.RegularExpressions.Regex.Replace(
        //        expression,
        //        @"(\d+(\.\d+)?)\^2",
        //        match => "(" + match.Groups[1].Value + " * " + match.Groups[1].Value + ")"
        //    );

        //    expression = System.Text.RegularExpressions.Regex.Replace(
        //        expression,
        //        @"√(\d+(\.\d+)?)",
        //        match => "Math.Sqrt(" + match.Groups[1].Value + ")"
        //    );

        //    return expression;
        //}
    }
}