using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IResultCollection
    {
        bool IsSuccess { get; set; }

        List<IResult> ResultList { get; set; }
    }
}
