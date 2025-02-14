using Attributes;
using Entities;
using Interfaces;
using Interfaces.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Services
{
    public static class ValidateServices
    {
        public static bool IsValidConverterType<T>(object[] obj, object value, T data, PropertyInfo item, Action<Exception> exceptionAction = null)
        {
            bool result = false;
            Type type = (obj.First() as Conditions).ConverterType;

            try
            {
                var typeValue = ((IConverter)Activator.CreateInstance(type)).Convert(value);
                data.GetType().GetProperty(item.Name).SetValue(data, typeValue.ToString());
                result = true;
            }
            catch (Exception err)
            {
                exceptionAction?.Invoke(err);
                result =  false;
            }
            return result;
        }

        public static bool IsValidMaxLength<T>(object[] obj, object value, T data, PropertyInfo item, Action exceptionAction = null)
        {
            data.GetType().GetProperty(item.Name).SetValue(data, value.ToString().Trim()); // Fill the cell
            int maxLenght = (obj.First() as Conditions).MaxLength;
            if (value.ToString().Length > maxLenght)
            {
                exceptionAction?.Invoke();
                return false;
            }
            return true;
        }

        public static bool IsValidExpectedType(object[] obj, object value, Action exceptionAction = null)
        {
            bool result = false;
            if ((obj.First() as Conditions).ExpectedType != null)
            {
                Type type = (obj.First() as Conditions).ExpectedType;

                if (value.GetType() != type)
                {
                    try
                    {
                        Convert.ChangeType(value, type);
                        result = true;
                    }
                    catch (Exception err)
                    {
                        exceptionAction?.Invoke();
                    }
                }
            }
            return result;
        }
    }
}
