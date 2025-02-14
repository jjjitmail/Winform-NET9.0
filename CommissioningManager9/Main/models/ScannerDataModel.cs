using CommissioningManager2.Controls;
using CommissioningManager2.Models.Base;
using Enum;
using Lib;
using Newtonsoft.Json;
using System.IO;

namespace CommissioningManager2.Models
{
    public class ScannerDataModel : SourceModel<ScannerData>
    {
        public override FileType FileType => FileType.ScanData;

        public ScannerDataModel() : base() { }

        internal override Model InternalProcess<Model>()
        {
            var thisModel = new Model();

            var model = Process(file =>
            {                
                thisModel.DataList = JsonConvert.DeserializeObject<SortableBindingList<ScannerData>>(File.ReadAllText(file.FullName));
                return thisModel;
            });
            return model();
        }

        public ScannerDataModel ReadFiles()
        {
            return InternalProcess<ScannerDataModel>();
        }
    }
}