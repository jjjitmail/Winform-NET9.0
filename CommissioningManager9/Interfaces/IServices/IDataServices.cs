using Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.IServices
{
    public interface IDataServices
    {
        IResultCollection Save<T>(SortableBindingList<T> list, String DatabaseConnectionString,
            Func<bool> PreValidateAction, Func<bool> AfterAction) where T : class;
    }
}
