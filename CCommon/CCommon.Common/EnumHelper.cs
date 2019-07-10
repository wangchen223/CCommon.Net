using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CCommon.Common
{
    /// <summary>
    /// 枚举通用类
    /// </summary>
    public class EnumHelper
    {
        //内部缓存
        private static List<EnumHelperDTO> dic = new List<EnumHelperDTO>();
        /// <summary>
        /// 获得描述
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescription(Enum value)
        {
            Type type = value.GetType();

            //如果缓存存在
            if (dic.Where(item => item.TypeName == type.ToString() && item.FieldName == value.ToString()).Count() > 0)
            {
                return dic.Where(item => item.TypeName == type.ToString() && item.FieldName == value.ToString()).FirstOrDefault().Description;
            }

            UpdateCache(type);
            //如果缓存存在
            if (dic.Where(item => item.TypeName == type.ToString() && item.FieldName == value.ToString()).Count() > 0)
            {
                return dic.Where(item => item.TypeName == type.ToString() && item.FieldName == value.ToString()).FirstOrDefault().Description;
            }
            else
            {
                return value.ToString();
            }
        }

        /// <summary>
        /// 根据枚举键名转换成相应枚举值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static T Parse<T>(string fieldName) where T : struct
        {
            return (T)Enum.Parse(typeof(T), fieldName, true);
        }

        /// <summary>
        /// 更新缓存
        /// </summary>
        /// <param name="type"></param>
        private static void UpdateCache(Type type)
        {
            foreach (var item in type.GetFields())
            {
                if (item != null)
                {
                    var atts = (DescriptionAttribute[])item.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    if (atts.Length > 0)
                    {
                        EnumHelperDTO dto = new EnumHelperDTO();
                        dto.TypeName = type.ToString();
                        dto.FieldName = item.Name;
                        dto.Description = atts[0].Description;
                        dto.FieldValue = Convert.ToInt32( System.Enum.Parse(type, item.Name));
                        dic.Add(dto);
                    }
                }
            }
        }
    }

    public class EnumHelperDTO
    {
        private string _typeName;
        private string _fieldName;
        private string _description;
        private string _name;
        private int _fieldValue;
        /// <summary>
        /// 类型
        /// </summary>
        public string TypeName
        {
            set { _typeName = value; }
            get { return _typeName; }
        }
        /// <summary>
        /// 字段
        /// </summary>
        public string FieldName
        {
            set { _fieldName = value; }
            get { return _fieldName; }
        }
        /// <summary>
        /// 值
        /// </summary>
        public int FieldValue
        {
            set { _fieldValue = value; }
            get { return _fieldValue; }
        }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description
        {
            set { _description = value; }
            get { return _description; }
        }
    }
    ///// <summary>
    ///// 枚举描述特性
    ///// </summary>
    //[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    //public class EnumDescriptionAttribute : Attribute
    //{
    //    public string Description { get; set; }
    //    public EnumDescriptionAttribute(string description)
    //    {
    //        Description = description;
    //    }
    //}
}
