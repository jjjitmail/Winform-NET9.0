//using Data;
using Entities;
using Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;

namespace CommissioningManager2.Controls
{
    public class CheckedListBoxEx : CheckedListBox
    {
        public string FileModelPath = Environment.CurrentDirectory + @"\Files\FileModel.xml";

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<FileModel> FileList { get; set; }

        public CheckedListBoxEx()
        {
            //FileList = new List<FileModel>();
            FileList = Utls<List<FileModel>>.LoadFromXML(FileModelPath);

            DoubleBuffered = true;
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            Size checkSize = CheckBoxRenderer.GetGlyphSize(e.Graphics, System.Windows.Forms.VisualStyles.CheckBoxState.MixedNormal);
            int dx = (e.Bounds.Height - checkSize.Width) / 2;
            e.DrawBackground();

            if (e.Index > -1)
            {
                bool isChecked = GetItemChecked(e.Index);
                Color repeatItemColor = ForeColor;
                if (FileList.Any(x => x.Name.ToLower().Trim() == Items[e.Index].ToString().ToLower().Trim()))
                {
                    repeatItemColor = RepeatItemColor;
                }
                CheckBoxRenderer.DrawCheckBox(e.Graphics, new Point(dx, e.Bounds.Top + dx), isChecked ? System.Windows.Forms.VisualStyles.CheckBoxState.CheckedNormal : System.Windows.Forms.VisualStyles.CheckBoxState.UncheckedNormal);
                using (StringFormat sf = new StringFormat { LineAlignment = StringAlignment.Center })
                {
                    using (Brush brush = new SolidBrush(repeatItemColor))
                    {
                        e.Graphics.DrawString(Items[e.Index].ToString(), Font, brush, new Rectangle(e.Bounds.Height, e.Bounds.Top, e.Bounds.Width - e.Bounds.Height, e.Bounds.Height), sf);
                    }
                }
            }
            //base.OnDrawItem(e);
        }

        Color repeatItemColor = Color.Red;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color RepeatItemColor
        {
            get { return repeatItemColor; }
            set
            {
                repeatItemColor = value;
                Invalidate();
            }
        }
    }
}
