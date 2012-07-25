using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BestRadioNet.Core.Data;

namespace BestRadioNet.Core.Data.Attributes
{
    /// <summary>
    /// Column Mapping Attribute
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnNameAttribute:Attribute
    {
        private string _Name;
        private int _SQLMode = DataContext.QueryMode;
        private int _SQLMode1 = 0;
        private string _ColumnDesc;

        public ColumnNameAttribute(string columnName)
        {
            this._Name = columnName;
        }

        public ColumnNameAttribute(string columnName, int sqlMode)
        {
            this._Name = columnName;

            this._SQLMode1 = sqlMode;
        }

        public ColumnNameAttribute(string columnName, int sqlMode, string columnDesc)
        {
            this._Name = columnName;

            this._SQLMode1 = sqlMode;

            this._ColumnDesc = columnDesc;
        }

        public string ColumnName
        {
            get { return _Name; }
        }

        public bool IsQuery
        {
            get { return true; }
        }

        public bool IsUpdate
        {
            get { return IsAccordTrue(_SQLMode + _SQLMode1, DataContext.UpdateMode); }
        }

        public bool IsInsert
        {
            get { return IsAccordTrue(_SQLMode + _SQLMode1, DataContext.InsertMode); }
        }

        private bool IsAccordTrue(int intValue, int intCompare)
        {

            return (intValue & intCompare) == intCompare;
        }

        /// <summary>
        /// 欄位中文名稱
        /// </summary>
        public string ColumnDesc
        {
            get { return _ColumnDesc; }
        }
    }
}
