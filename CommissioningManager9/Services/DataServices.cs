using Data;
using Entities;
using Interfaces;
using Interfaces.IServices;
using Lib;
using System;
using System.Reactive.Linq;

namespace Services
{
    public class DataServices : IDataServices
    {
        public IResultCollection Save<T>(SortableBindingList<T> list, String DatabaseConnectionString, 
            Func<bool> PreValidateAction, Func<bool> AfterAction) where T : class 
        {
            var _exception = new ExceptionServices();
            IResultCollection _iResultCollection = new ResultCollection();
            var pocess = Observable.Start(() =>
            {
                _iResultCollection = _exception.ExecuteWithTryCatch(_iResultCollection , () =>
                {
                    if (PreValidateAction())
                    {
                        using (var context = new DataContext<T>(DatabaseConnectionString))
                        {
                            if (!context.Database.Exists())
                            {
                                throw new Exception("Database connection error");
                            }
                            foreach (var item in list)
                            {
                                context.Set(typeof(T)).Add(item);
                            }
                            return context.SaveChanges();
                        }
                    }                    
                    return 0;
                });
            });
            pocess.Wait();
            _iResultCollection.IsSuccess = AfterAction();
            return _iResultCollection;
        }
    }
}
