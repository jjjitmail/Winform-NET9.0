using Attributes;
using Interfaces;
using CommissioningManager2.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using Entities;
using CommissioningManager2.Controls;
using Filters;
using Lib;
using Newtonsoft.Json.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection;
using Interfaces.IServices;
using Services;
using System.Reactive.Linq;

namespace CommissioningManager2.Validation
{
    public class ModelValidator<T> : ISource where T : class
    {
        public IResultCollection IResultCollection { get; set; }

        public static Model Validate<Model>(Model model) where Model : SourceModel<T>
        {
            model.IResultCollection.ResultList = new List<IResult>();
            DataStyle DataStyleForCompareSame = new DataStyle();
            
            model.DataList = model.DataControl.DataGridView.DataSource as SortableBindingList<T>;
            
            var dataListAfterCompare = model.DataList.Distinct(new ModelComparer<T>());
            var doubleDataList = model.DataList.Except(dataListAfterCompare);

            var dataListAfterCompareSame = model.DataList.Distinct(new SameComparer<T>());
            var doubleSameDataList = model.DataList.Except(dataListAfterCompareSame);

            var dataListAfterCompareSameField = model.DataList.Distinct(new SameFieldComparer<T>());
            var doubleSameFieldDataList = model.DataList.Except(dataListAfterCompareSameField);

            AddException4DoubleValue(doubleDataList, model);
            AddException4SameValue(doubleSameDataList, doubleSameFieldDataList, model);

            for (int i = 0; i < model.DataList.Count; i++)
            {
                var data = model.DataList[i];
                var PropertyInfoList = data.GetType().GetProperties()
                .Where(z => z.GetCustomAttributes(typeof(Conditions), false).Count() > 0);

                DataStyle _DataStyle = new DataStyle();

                for (int j = 0; j < PropertyInfoList.Count(); j++)
                {
                    var item = PropertyInfoList.ToList()[j];
                    var obj = item.GetCustomAttributes(typeof(Conditions), false);

                    var value = data.GetType().GetProperty(item.Name)?.GetValue(data);

                    var collection = Observable.CombineLatest(
                        Observable.Start(() => AddException4RequiredCells(model, data, item, _DataStyle, i, j)),
                        Observable.Start(() => AddException4CompareRequiredCells(model, data, item, i, j)),
                        Observable.Start(() => AddException4CompareRequiredSameCells(model, data, item, i, j)),
                        Observable.Start(() => AddException4CompareRequiredSameFieldCells(model, data, item, i, j)),
                        Observable.Start(() =>
                        {
                            if (value != null && !string.IsNullOrEmpty(value.ToString()))
                            {
                                if ((obj.First() as Conditions).ConverterType != null)
                                {
                                    AddException4ConverterType(model, data, item, i, j);
                                }
                                else if ((obj.First() as Conditions).MaxLength > 0)
                                {
                                    AddException4MaxLength(model, data, item, i, j);
                                }
                                AddException4ExpectedType(model, data, item, i, j);
                            }
                        })
                        ).Finally(() => { });
                    collection.Wait();                    
                }
                if (model.ProgressBar.Value < model.ProgressBar.Maximum)
                    model.ProgressBar.Value++;
            }
            return model;
        }
        //
        private static void AddException4ExpectedType<M>(M model, T data, PropertyInfo item, int i, int j) where M : SourceModel<T>
        {
            var obj = item.GetCustomAttributes(typeof(Conditions), false);
            var value = data.GetType().GetProperty(item.Name)?.GetValue(data);

            ValidateServices.IsValidExpectedType(obj, value, () => 
            {
                Type type = (obj.First() as Conditions).ExpectedType;
                model.IResultCollection.ResultList.Add(new Result
                {
                    Error = new Exception(string.Format("{0}: (error) '{1}' of '{2}' is expected, but '{3}'('{4}') is found", DateTime.Now, type, item.Name, value, value.GetType()))
                            ,
                    DataStyle = new DataStyle { RowIndex = new List<int>() { i }, CellIndex = new List<int>() { j } }
                });
            });            
        }

        private static void AddException4MaxLength<M>(M model, T data, PropertyInfo item, int i, int j) where M : SourceModel<T>
        {
            var obj = item.GetCustomAttributes(typeof(Conditions), false);
            var value = data.GetType().GetProperty(item.Name)?.GetValue(data);

            ValidateServices.IsValidMaxLength(obj, value, data, item, () => 
            {
                
                int maxLenght = (obj.First() as Conditions).MaxLength;
                model.IResultCollection.ResultList.Add(new Result
                {
                    Error = new Exception(string.Format("{0}: (error) 'MaxLength' of '{1}' is expected to be '{2}', but 'Length' of '{3}}'('{4}}') is found", DateTime.Now, item.Name, maxLenght, value.ToString().Length, value.ToString()))
                    ,
                    DataStyle = new DataStyle { RowIndex = new List<int>() { i }, CellIndex = new List<int>() { j } }
                });
            });
        }

        private static void AddException4ConverterType<M>(M model, T data, PropertyInfo item, int i, int j) where M : SourceModel<T>
        {
            var obj = item.GetCustomAttributes(typeof(Conditions), false);
            var value = data.GetType().GetProperty(item.Name)?.GetValue(data);

            ValidateServices.IsValidConverterType(obj, value, data, item, (err) => 
            {
                model.IResultCollection.ResultList.Add(new Result
                {
                    Error = new Exception(string.Format("{0}: (error) type of '{1}' is not correct, conversion was failed) Details: {2}", DateTime.Now, item.Name, err.Message))
                ,
                    DataStyle = new DataStyle { RowIndex = new List<int>() { i }, CellIndex = new List<int>() { j } }
                });
            });
        }

        private static void AddException4CompareRequiredSameFieldCells<M>(M model, T data, PropertyInfo item, int i, int j) where M : SourceModel<T>
        {
            var obj = item.GetCustomAttributes(typeof(Conditions), false);
            //var value = data.GetType().GetProperty(item.Name)?.GetValue(data);

            if ((obj.First() as Conditions).CompareRequiredSameField)
            {
                if (model.InvalidDoubleSameDataList?.Count() > 0)
                {
                    var foundOne = model.InvalidDoubleSameDataList.FirstOrDefault(x => RequiredSameValue(x) == RequiredSameValue(data));
                    if (foundOne != null)
                    {
                        model.IResultCollection.ResultList.Add(new Result
                        {
                            Error = new Exception(string.Format("{0}: (error) '{1}' has different value, see highlighted red cell(s) ", DateTime.Now, item.Name))
                        ,
                            DataStyle = new DataStyle { RowIndex = new List<int>() { i }, CellIndex = new List<int>() { j } }
                        });
                    }
                }
            }
        }
        private static void AddException4CompareRequiredSameCells<M>(M model, T data, PropertyInfo item, int i, int j) where M : SourceModel<T>
        {
            var obj = item.GetCustomAttributes(typeof(Conditions), false);
            //var value = data.GetType().GetProperty(item.Name)?.GetValue(data);

            if ((obj.First() as Conditions).CompareRequiredSame)
            {
                if (model.InvalidDoubleSameDataList?.Count() > 0)
                {
                    var foundOne = model.InvalidDoubleSameDataList.FirstOrDefault(x => RequiredSameValue(x) == RequiredSameValue(data));
                    if (foundOne != null)
                    {
                        model.IResultCollection.ResultList.Add(new Result
                        {
                            Error = new Exception(string.Format("{0}: (error) '{1}' should have same child value, see highlighted red cell(s) ", DateTime.Now, item.Name))
                        ,
                            DataStyle = new DataStyle { RowIndex = new List<int>() { i }, CellIndex = new List<int>() { j } }
                        });
                    }
                }
            }
        }
        private static void AddException4CompareRequiredCells<M>(M model, T data, PropertyInfo item, int i, int j) where M : SourceModel<T>
        {
            var obj = item.GetCustomAttributes(typeof(Conditions), false);
            //var value = data.GetType().GetProperty(item.Name)?.GetValue(data);

            if ((obj.First() as Conditions).CompareRequired)
            {
                var cellvalue = model.DataControl.DataGridView.Rows[i].Cells[j].Value;

                if (cellvalue == null || string.IsNullOrWhiteSpace(cellvalue?.ToString()))
                {
                    if (HasDoubleValue(model.DoubleDataList?.ToList(), data, CompareValue))
                    {
                        model.IResultCollection.ResultList.Add(new Result
                        {
                            Error = new Exception(DateTime.Now + ": (error) '" + item.Name + "' is required, see highlighted red cell(s) ")
                        ,
                            DataStyle = new DataStyle { RowIndex = new List<int>() { i }, CellIndex = new List<int>() { j } }
                        });
                    }
                }
            }
        }
        private static void AddException4RequiredCells<M>(M model, T data, PropertyInfo item, DataStyle _DataStyle, int i, int j) where M : SourceModel<T>
        {
            var obj = item.GetCustomAttributes(typeof(Conditions), false);
            var value = data.GetType().GetProperty(item.Name)?.GetValue(data);

            if ((obj.First() as Conditions).Required)
            {
                if (data.GetType().GetProperty(item.Name) == null)
                {
                    model.IResultCollection.ResultList.Add(new Result
                    {
                        Error = new Exception(string.Format("{0}: (error) '{1}' is not found", DateTime.Now, item.Name))
                        ,
                        DataStyle = new DataStyle { RowIndex = new List<int>() { i }, CellIndex = new List<int>() { j } }
                    });
                }
                else
                {
                    if (string.IsNullOrEmpty(value?.ToString().Trim()))
                    {
                        model.IResultCollection.ResultList.Add(new Result
                        {
                            Error = new Exception(string.Format("{0}: (error) '{1}' is required", DateTime.Now, item.Name))
                            ,
                            DataStyle = new DataStyle { RowIndex = new List<int>() { i }, CellIndex = new List<int>() { j } }
                        });
                    }
                    else
                    {
                        _DataStyle.Value += value.ToString().Trim();
                        _DataStyle.RowIndex.Add(i);
                        _DataStyle.CellIndex.Add(j);
                    }
                }
            }
        }

        private static void AddException4SameValue<M>(IEnumerable<T> doubleSameDataList, IEnumerable<T> doubleSameFieldDataList, M model) where M : SourceModel<T>
        {
            if (doubleSameDataList.Count() > 0)
            {
                model.DoubleSameDataList = doubleSameDataList;
                model.InvalidDoubleSameDataList = new List<T>();
                model.InvalidDoubleSameDataList = doubleSameDataList.Except(doubleSameFieldDataList);
            }
        }

        private static void AddException4DoubleValue<M>(IEnumerable<T> doubleDataList, M model) where M : SourceModel<T>
        {
            if (doubleDataList.Count() > 0)
            {
                model.DoubleDataList = new List<T>();
                model.DoubleDataList = doubleDataList;

                model.IResultCollection.ResultList.Add(new Result { Message = string.Format("{0}: (warning) Double records found, see highlighted cells", DateTime.Now) });
            }
        }

        private static string RequiredValue(T data)
        {
            var PropertyInfoList = data.GetType().GetProperties()
                .Where(z => z.GetCustomAttributes(typeof(Conditions), false).Count() > 0);
            var value = "";
            for (int j = 0; j < PropertyInfoList.Count(); j++)
            {
                var item = PropertyInfoList.ToList()[j];
                var obj = item.GetCustomAttributes(typeof(Conditions), false);
                if ((obj.First() as Conditions).Required)
                {
                    value += data.GetType().GetProperty(item.Name)?.GetValue(data);
                }
            }
            return value;
        }

        private static string CompareValue(T data)
        {
            var PropertyInfoList = data.GetType().GetProperties()
                .Where(z => z.GetCustomAttributes(typeof(Conditions), false).Count() > 0);
            var value = "";
            for (int j = 0; j < PropertyInfoList.Count(); j++)
            {
                var item = PropertyInfoList.ToList()[j];
                var obj = item.GetCustomAttributes(typeof(Conditions), false);
                if ((obj.First() as Conditions).Compare)
                {
                    value += data.GetType().GetProperty(item.Name)?.GetValue(data);
                }
            }
            return value;
        }

        private static string RequiredSameValue(T data)
        {
            var PropertyInfoList = data.GetType().GetProperties()
                .Where(z => z.GetCustomAttributes(typeof(Conditions), false).Count() > 0);
            var value = "";
            for (int j = 0; j < PropertyInfoList.Count(); j++)
            {
                var item = PropertyInfoList.ToList()[j];
                var obj = item.GetCustomAttributes(typeof(Conditions), false);
                if ((obj.First() as Conditions).CompareRequiredSame)
                {
                    value += data.GetType().GetProperty(item.Name)?.GetValue(data);
                }
            }
            return value;
        }

        private static string RequiredSameFieldValue(T data)
        {
            var PropertyInfoList = data.GetType().GetProperties()
                .Where(z => z.GetCustomAttributes(typeof(Conditions), false).Count() > 0);
            var value = "";
            for (int j = 0; j < PropertyInfoList.Count(); j++)
            {
                var item = PropertyInfoList.ToList()[j];
                var obj = item.GetCustomAttributes(typeof(Conditions), false);
                if ((obj.First() as Conditions).CompareRequiredSameField)
                {
                    value += data.GetType().GetProperty(item.Name)?.GetValue(data);
                }
            }
            return value;
        }

        private static bool HasDoubleValue(List<T> list, T dataValue)
        {
            if (list == null)
                return false;
            var datavalue = RequiredValue(dataValue);

            for (int i = 0; i < list.Count; i++)
            {
                var newValue = RequiredValue(list[i]);
                if (datavalue == newValue)
                    return true;
            }
            return false;
        }

        private static bool HasDoubleValue(List<T> list, T dataValue, Func<T, string> func)
        {
            if (list == null)
                return false;
            var datavalue = func(dataValue);

            for (int i = 0; i < list.Count; i++)
            {
                var newValue = func(list[i]);
                if (datavalue == newValue)
                    return true;
            }
            return false;
        }
    }
}