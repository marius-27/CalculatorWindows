using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CalculatorWindows
{
    public class MemoryManager
    {
        private List<double> memory = new List<double>();

        public void Store(double value)
        {
            memory.Add(value);
        }

        public double Recall()
        {
            if (memory.Count > 0)
            {
                return memory[^1];
            }
            throw new InvalidOperationException("Memory is empty.");
        }

        public void Clear()
        {
            memory.Clear();
        }

        public void Add(double value)
        {
            if (memory.Count > 0)
            {
                memory[^1] += value;
            }
            else
            {
                memory.Add(value);
            }
        }

        public void Subtract(double value)
        {
            if (memory.Count > 0)
            {
                memory[^1] -= value;
            }
            else
            {
                memory.Add(-value);
            }
        }

        public string View()
        {
            if (memory.Count > 0)
            {
                return string.Join(Environment.NewLine, memory);
            }
            return "Memory is empty";
        }
    }
}
