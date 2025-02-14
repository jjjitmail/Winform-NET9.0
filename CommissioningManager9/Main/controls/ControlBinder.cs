using Attributes;
using Interfaces;
using CommissioningManager2.Models.Base;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Entities;
using Lib;
using System.Reflection;
using System.Data.Entity.Infrastructure;
using CommissioningManager2.UIExtention;

namespace CommissioningManager2.Controls
{    
    public class ControlBinder<T> : ISource where T : class
    {
        public IResultCollection IResultCollection { get; set; }
        
        public static void UpdateGridCells<Model>(Model model) where Model : SourceModel<T>
        {
            if (model.DataList == null)
                model.DataList = new SortableBindingList<T>();

            var hasDouble = model.DoubleDataList?.Any();

            var hasDoubleSame = model.DoubleSameDataList?.Any();

            var randInts = GetRandomNumbers(model.DataList.Count() + 20);
            List<IDataStyle> DataStyles = new List<IDataStyle>();

            for (int i = 0; i < model.DataList.Count(); i++)
            {
                var subPros = model.DataList[i].GetType().GetProperties().Where(z => z.GetCustomAttributes(typeof(Conditions), false).Count() > 0);

                IDataStyle _iDataStyle = new DataStyle();
                
                for (int j = 0; j < subPros.Count(); j++)
                {
                    model.DataControl.DataGridView.Rows[i].Cells[j].Style.BackColor = Color.White;
                    var obj = subPros.Skip(j).First();
                    var objAttr = obj.GetCustomAttributes(typeof(Conditions), false);
                    if (objAttr.Count() > 0)
                    {
                        InitCompareCellsStyling(objAttr, _iDataStyle, obj, model, i, j);
                        HighLightExceptionCells(model, i, j);
                        HighLightMaxLenghtCells(model, objAttr, i, j);
                        HighLightReadonlyCells(model, objAttr, i, j);
                        InitColumnDisplayName(objAttr, model, j);                        
                    }
                }
                // highlight repeated data
                HighLightCellsWithRepeatedValue(hasDouble, DataStyles, randInts, _iDataStyle, model, i);
                
                DataStyles.Add(_iDataStyle);
                if (model.ProgressBar.Value < model.ProgressBar.Maximum)
                    model.ProgressBar.Value++;
            }
        }

        private static void HighLightCellsWithRepeatedValue<M>(bool? hasDouble, List<IDataStyle> DataStyles, IEnumerable<int>? randInts, IDataStyle _iDataStyle, M model, int i) where M : SourceModel<T>
        {
            if (hasDouble.HasValue)
            {
                var dataStyles = DataStyles.Where(x => x.Value == _iDataStyle.Value && !string.IsNullOrEmpty(x.Value));
                if (dataStyles.Count() > 0)
                {
                    int FirstInt = 256;
                    int SecondInt = 256;
                    int ThirdInt = 256;
                    if (i > 250)
                    {
                        FirstInt = randInts.Skip(250).First();
                        SecondInt = randInts.Skip(251).First();
                        ThirdInt = randInts.Skip(252).First();
                    }
                    else
                    {
                        FirstInt = randInts.Skip(i).First();
                        SecondInt = randInts.Skip(i + 1).First();
                        ThirdInt = randInts.Skip(i + 2).First();
                    }
                    _iDataStyle.Color = Color.FromArgb(FirstInt, SecondInt, ThirdInt);

                    for (int z = 0; z < dataStyles.Count(); z++)
                    {
                        var rowIndex = dataStyles.Skip(z).First().RowIndex.First();
                        for (int y = 0; y < dataStyles.Skip(z).First().CellIndex.Count; y++)
                        {
                            var cellIndex = dataStyles.Skip(z).First().CellIndex.Skip(y).First();
                            model.DataControl.DataGridView.Rows[rowIndex].Cells[cellIndex].Style.BackColor = _iDataStyle.Color;
                            model.DataControl.DataGridView.Rows[rowIndex].Cells[cellIndex].Style.ForeColor = ContrastColor(_iDataStyle.Color);
                        }
                    }

                    for (int s = 0; s < _iDataStyle.CellIndex.Count; s++)
                    {
                        model.DataControl.DataGridView.Rows[_iDataStyle.RowIndex.First()].Cells[_iDataStyle.CellIndex.Skip(s).First()].Style.BackColor = _iDataStyle.Color;
                        model.DataControl.DataGridView.Rows[_iDataStyle.RowIndex.First()].Cells[_iDataStyle.CellIndex.Skip(s).First()].Style.ForeColor = ContrastColor(_iDataStyle.Color);
                    }
                }
            }
        }
        private static void InitColumnDisplayName<M>(object[] objAttr, M model, int j) where M : SourceModel<T>
        {
            if (((Conditions)objAttr[0]).DisplayName != null)
            {
                model.DataControl.DataGridView.Columns[j].HeaderText = ((Conditions)objAttr[0]).DisplayName;
            }
        }
        private static void InitCompareCellsStyling<M>(object[] objAttr, IDataStyle _iDataStyle, PropertyInfo obj, M model, int i, int j) where M : SourceModel<T>
        {
            if (((Conditions)objAttr[0]).Compare)
            {
                _iDataStyle.Value += obj.GetValue(model.DataList[i])?.ToString().Trim();
                _iDataStyle.RowIndex.Add(i);
                _iDataStyle.CellIndex.Add(j);
            }
        }
        private static void HighLightExceptionCells<M>(M model, int i, int j) where M : SourceModel<T>
        {
            if (model.IResultCollection.ResultList != null && model.IResultCollection.ResultList.Where(x => x.DataStyle.RowIndex.Any(y => y == i) && x.DataStyle.CellIndex.Any(z => z == j)).Any())
            {
                model.DataControl.DataGridView.Rows[i].Cells[j].Style.BackColor = Color.Red;
            }
        }
        private static void HighLightMaxLenghtCells<M>(M model, object[] objAttr, int i, int j) where M : SourceModel<T>
        {
            if (((Conditions)objAttr[0]).MaxLength > 0)
            {
                model.DataControl.DataGridView.Rows[i].Cells[j].Value = model.DataControl.DataGridView.Rows[i].Cells[j].Value?.ToString().Trim();
                if (model.DataControl.DataGridView.Rows[i].Cells[j].Value?.ToString().Length > ((Conditions)objAttr[0]).MaxLength)
                {
                    model.DataControl.DataGridView.Rows[i].Cells[j].Style.BackColor = Color.Red;
                }
            }
        }
        private static void HighLightReadonlyCells<M>(M model, object[] objAttr, int i, int j) where M : SourceModel<T>
        {
            if (((Conditions)objAttr[0]).ReadOnly)
            {
                model.DataControl.DataGridView.Rows[i].Cells[j].ReadOnly = true;
                if (model.DataControl.DataGridView.Rows[i].Cells[j].Style.BackColor == Color.White)
                {
                    model.DataControl.DataGridView.Rows[i].Cells[j].Style.BackColor = Color.LightGray;
                }
            }
        }

        public static Color ContrastColor(Color color)
        {
            int d = 0;

            double a = 1 - (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;

            if (a < 0.5)
                d = 0;
            else
                d = 255;

            return Color.FromArgb(d, d, d);
        }

        private static IEnumerable<int> GetRandomNumbers(int numberOfRandoms)
        {
            if (numberOfRandoms > 255)
                numberOfRandoms = 255;
            Random rnd = new Random();
            var unique = new HashSet<int>();
            do
            {
                int num = rnd.Next(1, 256);
                if (unique.Contains(num))
                    continue;
                unique.Add(num);
            }
            while (unique.Count < numberOfRandoms);
            return unique.ToList();
        }

        public static void DataGridView<Model>(Model model) where Model : SourceModel<T>
        {
            model.DataControl.DataGridView.DataSource = model.DataList;
            UpdateGridCells(model);
        }
    }
}