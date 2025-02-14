using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.IServices
{
    public interface IExceptionServices
    {
        IResultCollection ExecuteWithTryCatch(IResultCollection resultCollection, Func<int> action);
    }
}
