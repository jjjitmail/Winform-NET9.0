using Entities;
using Interfaces;
using Interfaces.IServices;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ExceptionServices : IExceptionServices
    {
        public IResultCollection ExecuteWithTryCatch(IResultCollection resultCollection, Func<int> action)
        {
            var resultList = resultCollection.ResultList;
            try
            {
                var count = action();
                resultList.Add(new Result { Message = string.Format("{0}: {1} records processed successfully", DateTime.Now, count) });                
            }
            catch (Exception err)
            {
                var EntityValidationErrors = err.GetType().GetProperty("EntityValidationErrors");
                if (EntityValidationErrors != null && EntityValidationErrors.GetValue(err, null) != null)
                {
                    var dbError = EntityValidationErrors.GetValue(err, null);
                    var Result = new Result { DbEntityValidationResults = (List<DbEntityValidationResult>)dbError };
                    var ValidationError = Result.DbEntityValidationResults.First().ValidationErrors.First();
                    Result.Message = string.Format("{0}: (Error) PropertyName: {1}}, ErrorMessage: {2}", DateTime.Now, ValidationError.PropertyName, ValidationError.ErrorMessage);

                    resultList.Add(Result);
                }
                else
                {
                    resultList.Add(new Result { Error = new Exception(string.Format("{0}:  (Error) System: {1}", DateTime.Now, err.Message)) });
                }
            }
            finally
            {
                resultCollection.ResultList = resultList;
            }
            return resultCollection;
        }
    }
}
