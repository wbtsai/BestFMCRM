using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BestRadioNet.Core.Data.Attributes
{
    /// <summary>
    /// SQL Command 前置字元的設定資料檔
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class SQLPrefixAttribute:Attribute
    {
        public SQLPrefixAttribute()
        {
            this.IsIncludeSQLColumnName = true;
        }

        /// <summary>
        /// SQL String 的前置字元
        /// </summary>
        public string SQLStringPrefix { get; set; }

        /// <summary>
        /// SQL String 裡面的Column Name 是否要加上
        /// </summary>
        public bool IsIncludeSQLColumnName { get; set; }

        /// <summary>
        /// 參數值
        /// </summary>
        public string ParameterPrefix { get; set; }



        /// <summary>
        /// Store Procedure 的前置字元
        /// </summary>
        public string StoreProcedure { get; set; }



        
    }
}
