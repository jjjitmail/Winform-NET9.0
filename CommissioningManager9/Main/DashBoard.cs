using Data;
using Helpers;
using CommissioningManager2.Models;
using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Generic;
using Entities;
using System.ComponentModel.Design;
using Interfaces.IServices;
using Services;

namespace CommissioningManager2
{
    public partial class DashBoard : Form
    {
        #region private variables

        private LuxDataModel _luxDataModel = new LuxDataModel();
        private ScannerDataModel _scannerDataModel = new ScannerDataModel();
        private TeleControllerDataModel _teleControllerDataModel = new TeleControllerDataModel();

        public DataControl dataControlTeleController = new DataControl();
        public ProgressBar Progressbar = new ProgressBar();

        private readonly IFileServices iFileServices;
        #endregion

        public DashBoard(IFileServices _IFileServices)
        {
            InitializeComponent();
            Progressbar = progressBar1;
            iFileServices = _IFileServices;
        }
        
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DashBoard_Shown(object sender, EventArgs e)
        {
            Start();

            var DatabaseConnectionlist = Utls<BindingList<DatabaseModel>>.LoadFromXML(Environment.CurrentDirectory + @"\Files\DatabaseModel.xml");            
            comboBoxDatabaseName.DisplayMember = "DatabaseName";
            comboBoxDatabaseName.ValueMember = "DatabaseConnection";
            comboBoxDatabaseName.DataSource = DatabaseConnectionlist;
            textBoxDatabaseConnection.Text = comboBoxDatabaseName.SelectedValue.ToString();            
        }

        private void Start()
        {
            ReadLuxDataFiles();
            ReadScannerFiles();
            ReadTeleControllerFiles();

            MouseEvents();
        }
        
        private void MouseEvents()
        {
            _luxDataModel.ControlsEvents(this, ReadLuxDataFiles);
            _scannerDataModel.ControlsEvents(this, ReadScannerFiles);
            _teleControllerDataModel.ControlsEvents(this, ReadTeleControllerFiles);
        }

        #region read files
        private void ReadLuxDataFiles(bool preFillQuery = false)
        {
            _luxDataModel.DataControl = dataGridViewLuxData;

            if (preFillQuery)
            {
                _luxDataModel = _luxDataModel.ReadFiles();
                _luxDataModel.PreFillQuery();
            }
            _luxDataModel = _luxDataModel.ReadFiles();
        }

        private void ReadScannerFiles(bool preFillQuery = false)
        {
            _scannerDataModel.DataControl = dataControlScanData;

            if (preFillQuery)
            {
                _scannerDataModel = _scannerDataModel.ReadFiles();
                _scannerDataModel.PreFillQuery();
            }
            _scannerDataModel = _scannerDataModel.ReadFiles();
        }

        private void ReadTeleControllerFiles(bool preFillQuery = false)
        {
            _teleControllerDataModel.DataControl = dataControlTeleController;

            if (preFillQuery)
            {
                _teleControllerDataModel = _teleControllerDataModel.ReadFiles();
                _teleControllerDataModel.PreFillQuery();
            }
            _teleControllerDataModel = _teleControllerDataModel.ReadFiles();
        }
        #endregion

        private void comboBoxDatabaseName_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBoxDatabaseConnection.Text = comboBoxDatabaseName.SelectedValue.ToString();
        }

        private void btnTeleController_Click(object sender, EventArgs e)
        {
            var text = textBoxTeleController.Text;
            
            if (text.Trim() == "YZRKH")
            {
                ShowTeleController();
            }
        }

        private void ShowTeleController()
        {
            if (tabControl.Controls.Count > 2)
                tabControl.Controls.RemoveAt(2);
            
            TabPage _TeleControllerTabPage = new TabPage();
            _TeleControllerTabPage.Text = "TeleControllerData";
            _TeleControllerTabPage.Controls.Add(dataControlTeleController);
            tabControl.Controls.Add(_TeleControllerTabPage);
        }
    }
}