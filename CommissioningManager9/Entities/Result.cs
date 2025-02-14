//using Controls;
using Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;

namespace Entities
{
    public class Result : IResult
    {
        public Result()
        {
            Error = new Exception("");
            DataStyle = new DataStyle();
        }

        public List<DbEntityValidationResult> DbEntityValidationResults { get; set; }

        public Exception Error { get; set; }

        public bool Success { get; set; }

        private string _Message;
        public string Message {
            get
            {
                if (!string.IsNullOrEmpty(Error.Message))
                {
                    return Error.Message;
                }
                return _Message;
            }
            set { _Message = value; }
        }

        public IDataStyle DataStyle { get; set; }
    }
}