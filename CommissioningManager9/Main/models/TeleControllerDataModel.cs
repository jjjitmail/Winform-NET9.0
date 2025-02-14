using CommissioningManager2.Controls;
using CommissioningManager2.Models.Base;
using Enum;
using Lib;
using Newtonsoft.Json;
using System.IO;

namespace CommissioningManager2.Models
{
    public class TeleControllerDataModel : SourceModel<TeleControllerData>
    {
        public override FileType FileType => FileType.TeleControllerData;

        public TeleControllerDataModel(): base() { }

        internal override Model InternalProcess<Model>()
        {
            var thisModel = new Model();

            var model = Process(file =>
            {
                thisModel.DataList = JsonConvert.DeserializeObject<SortableBindingList<TeleControllerData>>(File.ReadAllText(file.FullName));
                return thisModel;
            });
            return model();
        }

        public TeleControllerDataModel ReadFiles()
        {
            return InternalProcess<TeleControllerDataModel>();
        }
    }
}