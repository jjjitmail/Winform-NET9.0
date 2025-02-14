using Attributes;
using Entities;
using Services;

namespace TestProject1
{
    [TestClass]
    public sealed class Test1
    {
        [TestMethod]
        public void TestMethod1()
        {
            LuxData _LuxData = new LuxData();
            _LuxData.Mastnumber = "12345678901234567890123456";

            var PropertyInfoList = _LuxData.GetType().GetProperty("Mastnumber");

            var obj = PropertyInfoList.GetCustomAttributes(typeof(Conditions), false);
            var value = PropertyInfoList.GetValue(_LuxData);


            var result = ValidateServices.IsValidMaxLength(obj, value, _LuxData, PropertyInfoList, null);
            Assert.IsFalse(result);
        }
    }
}
