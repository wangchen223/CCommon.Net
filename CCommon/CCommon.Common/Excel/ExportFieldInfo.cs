using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCommon.Common.Excel
{
    public class ExportFieldInfo<T>
    {
        /// <summary>
        /// 中文名，用于导出标题
        /// </summary>
        public string DisplayName { get; set; }

        private EDataType _dataType = EDataType.String;
        /// <summary>
        /// 数据类型，用于强制转换，并进行格式化,其实利用反射也可以获取到数据类型，此处因为要处理Date和Date的显示格式
        /// </summary>
        public EDataType DataType
        {
            get { return _dataType; }
            set { _dataType = value; }
        }
        /// <summary>
        /// 列值
        /// </summary>
        public Func<T,string> FieldValue { get; set; }
    }
}
