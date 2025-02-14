using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IResult
    {
        Exception Error { get; set; }

        string Message { get; set; }

        IDataStyle DataStyle { get; set; }

        bool Success { get; set; }
    }
}
