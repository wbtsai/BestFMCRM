using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BestRadioNet.Core.Data.Attributes
{
    /// <summary>
    /// 資料Mapping 機制
    /// 希能在此設定上更能方便
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor,AllowMultiple=true)]
    public class DataContextMapAttribute:Attribute
    {
        /// <summary>
        /// Constructor 
        /// </summary>
        public DataContextMapAttribute()
        {
            this.SQLContextType = EnumSQLContext.None;
            
            this.PKNameOrTableName = "";

            this.WhereSQL = "";
        }

        /// <summary>
        /// Key 值
        /// </summary>
        public string MapKey { get; set; }

        /// <summary>
        /// PK Name 或者是Table Name
        /// </summary>
        public string PKNameOrTableName { get; set; }

        /// <summary>
        /// SQL Context Type
        /// </summary>
        public EnumSQLContext SQLContextType { get; set; }

        /// <summary>
        /// 取得SQL Command 的
        /// </summary>
        public EnumSQLCommand SQLCommand { get; set; }

        /// <summary>
        /// 針對Update 以及Delete
        /// </summary>
        public string WhereSQL { get; set; }

        /// <summary>
        /// 參數列表，直接用object[]表示
        /// 
        /// </summary>
        /// <example>
        ///     new object[]{"PropertyName","PropertyName2"}
        /// </example>
        public object[] Parameters { get; set; }
                
    }
}
