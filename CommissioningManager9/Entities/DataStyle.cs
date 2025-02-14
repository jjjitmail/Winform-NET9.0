using Interfaces;
using System.Collections.Generic;
using System.Drawing;

namespace Entities
{
    public class DataStyle : IDataStyle
    {
        public DataStyle()
        {
            RowIndex = new List<int>();
            CellIndex = new List<int>();
        }
        public string Value { get; set; }
        public Color Color { get; set; }
        public List<int> RowIndex { get; set; }
        public List<int> CellIndex { get; set; }
    }
}
