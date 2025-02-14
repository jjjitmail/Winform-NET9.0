using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.IServices
{
    public interface IValidateServices
    {
        bool IsValidMaxLength(object[] obj, object value, Action exceptionAction = null);
    }
}
