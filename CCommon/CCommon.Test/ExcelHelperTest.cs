﻿using CCommon.Common.Excel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CCommon.Test
{
    public class User
    {
        public string Name { get; set; }
    }
    /// <summary>
    /// Excel导出
    /// </summary>
    [TestClass]
    public class ExcelHelperTest
    {

        [TestMethod]
        public void ListTest()
        {
            List<ExportFieldInfo<User>> flist = new List<ExportFieldInfo<User>>();

            flist.Add(new ExportFieldInfo<User>
            {
                DisplayName = "姓名",
                DataType = EDataType.String,
                FieldValue = info => info.Name
            });


            var userList = new List<User>();
            userList.Add(new User { Name = "张三" });
            userList.Add(new User { Name = "李四" });
            
            ExcelHelper<User>.ToExcel(userList, flist);
        }

        [TestMethod]
        public void DataTableTest()
        {
            
            List<ExportFieldInfo<DataRow>> flist = new List<ExportFieldInfo<DataRow>>();
            flist.Add(new ExportFieldInfo<DataRow>
            {
                DisplayName = "姓名",
                DataType = EDataType.String,
                FieldValue = info => info["Name"].ToString()
            });

            ExcelHelper.ToExcel(TestData(), flist);

        }

        private DataTable TestData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Name");
            DataRow dr = dt.NewRow();
            dr["Name"] = "张三";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Name"] = "李四";
            dt.Rows.Add(dr);
            return dt;
        }
    }
}
