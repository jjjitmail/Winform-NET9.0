using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class ResultCollection : IResultCollection
    {
        public bool IsSuccess { get; set; }

        public List<IResult> ResultList { get; set; } = new List<IResult>();
    }
}
