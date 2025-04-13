using System;
using System.Data;
using System.Globalization;
using System.Linq;

namespace CalculatorWindows
{
    public class CalculatorOperations
    {
        // Metoda pentru calcularea unei expresii
        public double CalculateExpression(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                throw new InvalidOperationException("Expression cannot be empty or null.");
            }

            // Elimină spațiile și înlocuiește caracterele speciale
            expression = expression
                .Replace(" ", "") // Elimină spațiile
                .Replace("×", "*")
                .Replace("÷", "/")
                .Replace(CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, "")
                .Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, ".");

            // Verifică dacă expresia conține caractere invalide
            if (!IsValidExpression(expression))
            {
                throw new InvalidOperationException("Expression contains invalid characters.");
            }

            try
            {
                var result = new DataTable().Compute(expression, null);
                return Convert.ToDouble(result);
            }
            catch (DivideByZeroException)
            {
                throw new InvalidOperationException("Division by zero is not allowed.");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while calculating the expression: " + ex.Message);
            }
        }

        // Metoda pentru verificarea validității expresiei
        private bool IsValidExpression(string expression)
        {
            // Lista de caractere permise
            var allowedChars = "0123456789+-*/.()";
            return expression.All(c => allowedChars.Contains(c));
        }

        // Metoda pentru calcularea procentului
        public double CalculatePercentage(double value)
        {
            return value / 100;
        }

        // Metoda pentru calcularea reciprocului
        public double CalculateReciprocal(double value)
        {
            if (value == 0)
            {
                throw new InvalidOperationException("Cannot divide by zero.");
            }
            return 1 / value;
        }

        // Metoda pentru calcularea patratului
        public double CalculateSquare(double value)
        {
            return value * value;
        }

        // Metoda pentru calcularea rădăcinii pătrate
        public double CalculateSquareRoot(double value)
        {
            if (value < 0)
            {
                throw new InvalidOperationException("Cannot calculate square root of a negative number."); 
            }
            return Math.Sqrt(value);
        }
    }
}