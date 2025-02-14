using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IDataStyle
    {
        string Value { get; set; }
        Color Color { get; set; }
        List<int> RowIndex { get; set; }
        List<int> CellIndex { get; set; }
    }
}
