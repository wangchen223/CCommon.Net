using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCommon.Common.Excel
{
    /// <summary>
    /// Excel导出
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ExcelHelper
    {
        /// <summary>
        /// 导出到内存流
        /// </summary>
        /// <param name="datas">需要导出的模型列表</param>
        /// <param name="fieldInfies">导出的字段列表信息</param>
        /// <param name="sheetRows">每个工作表的行数</param>
        /// <returns></returns>
        public static MemoryStream ToExcel(DataTable datas, List<ExportFieldInfo<DataRow>> fieldInfies, int sheetRows = 65536)
        {
            List<DataRow> drList = new List<DataRow>();
            foreach (DataRow dr in datas.Rows)
            {
                drList.Add(dr);
            }

            return ExcelHelper<DataRow>.ToExcel(drList, fieldInfies, sheetRows);
        }
    }
    /// <summary>
    /// Excel导出
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ExcelHelper<T>
    {

        /// <summary>
        /// 导出到内存流
        /// </summary>
        /// <param name="datas">需要导出的模型列表</param>
        /// <param name="fieldInfies">导出的字段列表信息</param>
        /// <param name="sheetRows">每个工作表的行数</param>
        /// <returns></returns>
        public static MemoryStream ToExcel(List<T> datas, List<ExportFieldInfo<T>> fieldInfies, int sheetRows = 65536)
        {
            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //需要生成工作溥总簿
            int sheetCount = datas.Count / sheetRows + 1;
            int rowCount = datas.Count;
            for (int i = 0; i < sheetCount; i++)
            {
                //添加一个sheet
                NPOI.SS.UserModel.ISheet sheet = book.CreateSheet("Sheet" + Convert.ToString(i));
                //给sheet添加第一行的头部标题
                NPOI.SS.UserModel.IRow rowTitle = sheet.CreateRow(0);
                for (int k = 0; k < fieldInfies.Count; k++)
                {
                    rowTitle.CreateCell(k).SetCellValue(fieldInfies.ElementAt(k).DisplayName);
                }
                //处理Excel一张工作簿只能放65536行记录的问题
                //因为头部占一行，所以要减1
                for (int j = 0; j < sheetRows - 1; j++)
                {
                    //将数据逐步写入sheet各个行
                    NPOI.SS.UserModel.IRow rowtemp = sheet.CreateRow(j + 1);
                    int dataIndex = i * (sheetRows - 1) + j;
                    for (int k = 0; k < fieldInfies.Count; k++)
                    {
                        //获取类型
                        Type type = typeof(T); //datas[dataIndex].GetType();
                                               //获取指定名称的属性
                                               //System.Reflection.PropertyInfo propertyInfo = type.GetProperty(fieldInfies.ElementAt(k).FieldName);
                                               //if (propertyInfo == null)
                                               //{
                                               //    throw new ArgumentNullException(string.Format("您所指定的{0}属性不存在,请检查FieldName是否正确", fieldInfies.ElementAt(k).FieldName));
                                               //}
                        if (datas.Count > 0)
                        {
                            //获取属性值
                            var value = fieldInfies.ElementAt(k).FieldValue(datas[dataIndex]); //propertyInfo.GetValue(datas[dataIndex], null);
                            switch (fieldInfies.ElementAt(k).DataType)
                            {
                                case EDataType.Int:
                                    rowtemp.CreateCell(k).SetCellValue(Convert.ToInt32(value));
                                    break;
                                case EDataType.Float:
                                case EDataType.Double:
                                    rowtemp.CreateCell(k).SetCellValue(Convert.ToDouble(value));
                                    break;
                                case EDataType.String:
                                    rowtemp.CreateCell(k).SetCellValue(Convert.ToString(value));
                                    break;
                                case EDataType.DateTime:
                                    rowtemp.CreateCell(k).SetCellValue(Convert.ToDateTime(value));
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                    //所有记录循环完成
                    if (i * (sheetRows - 1) + (j + 1) == rowCount || rowCount == 0)
                    {
                        AutoSizeColumn(sheet, fieldInfies);
                        // 写入到客户端 
                        System.IO.MemoryStream ms = new System.IO.MemoryStream();
                        book.Write(ms);
                        ms.Seek(0, SeekOrigin.Begin);
                        return ms;
                    }
                }

            }

            return null;
        }

        /// <summary>
        /// 列宽度自动适配
        /// </summary>
        /// <param name="paymentSheet"></param>
        /// <param name="fieldInfies"></param>
        private static void AutoSizeColumn(NPOI.SS.UserModel.ISheet paymentSheet, List<ExportFieldInfo<T>> fieldInfies)
        {
            //获取当前列的宽度，然后对比本列的长度，取最大值
            for (int columnNum = 0; columnNum <= fieldInfies.Count; columnNum++)
            {
                int columnWidth = paymentSheet.GetColumnWidth(columnNum) / 256;
                for (int rowNum = 1; rowNum <= paymentSheet.LastRowNum; rowNum++)
                {
                    IRow currentRow;
                    //当前行未被使用过
                    if (paymentSheet.GetRow(rowNum) == null)
                    {
                        currentRow = paymentSheet.CreateRow(rowNum);
                    }
                    else
                    {
                        currentRow = paymentSheet.GetRow(rowNum);
                    }

                    if (currentRow.GetCell(columnNum) != null)
                    {
                        ICell currentCell = currentRow.GetCell(columnNum);
                        int length = Encoding.Default.GetBytes(currentCell.ToString()).Length;
                        if (columnWidth < length)
                        {
                            columnWidth = length;
                        }
                    }
                }
                columnWidth = Math.Min(columnWidth, 255);//excel最大宽度255,超出引发 The maximum column width for an individual cell is 255 charaters 异常
                paymentSheet.SetColumnWidth(columnNum, columnWidth * 256);
            }
        }
    }
}
