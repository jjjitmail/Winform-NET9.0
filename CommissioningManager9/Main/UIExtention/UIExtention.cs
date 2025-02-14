using CommissioningManager2.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommissioningManager2.UIExtention
{
    public static class UIExtention
    {
        public static void AppendAppText(this RichTextBox box, string text)
        {
            Color color = Color.Black;
            if (text.ToLower().Contains("error"))
            {
                color = Color.Red;
            }
            else if (text.ToLower().Contains("warning"))
            {
                color = Color.OrangeRed;
            }
            else if ((text.ToLower().Contains("passed")))
            {
                color = Color.Green;
            }

            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
            box.ScrollToCaret();
            box.ResumeLayout();
        }

        public static void InitHighLightException<T>(this T model, int i, int j) where T : SourceModel<T>
        {
            if (model.IResultCollection.ResultList != null && model.IResultCollection.ResultList.Where(x => x.DataStyle.RowIndex.Any(y => y == i) && x.DataStyle.CellIndex.Any(z => z == j)).Any())
            {
                model.DataControl.DataGridView.Rows[i].Cells[j].Style.BackColor = Color.Red;
            }
        }
    }
}
