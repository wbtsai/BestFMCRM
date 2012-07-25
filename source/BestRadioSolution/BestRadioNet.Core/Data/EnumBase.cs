using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BestRadioNet.Core.Data.Attributes;

namespace BestRadioNet.Core.Data
{
    /// <summary>
    /// 查詢SQL 的模式
    /// </summary>
    public enum EnumSQLContext
    { 
        None,
        /// <summary>
        /// SQL String
        /// </summary>
        SQLString,

        /// <summary>
        /// StoreProcedure
        /// </summary>
        StoreProcedure
    }

    /// <summary>
    /// 資料庫
    /// </summary>
    public enum EnumSQLDataBaseProvider
    {
        /// <summary>
        /// None
        /// </summary>
        None,

        /// <summary>
        /// SQL Server
        /// </summary>
        [SQLPrefix(SQLStringPrefix = "@",ParameterPrefix="@", StoreProcedure = "")]
        SqlClientFactory,

        /// <summary>
        /// Oracle Server
        /// </summary>
        [SQLPrefix(SQLStringPrefix = ":", ParameterPrefix = ":", StoreProcedure = "")]
        OracleClientFactory,

        /// <summary>
        /// SQLite Server
        /// </summary>
        [SQLPrefix(SQLStringPrefix = "@", ParameterPrefix = "@", StoreProcedure = "")]
        SQLiteFactory,

        /// <summary>
        /// OleDB Factory
        /// </summary>
        [SQLPrefix(SQLStringPrefix = "@", ParameterPrefix = "@", StoreProcedure = "")]
        OleDbFactory,

        /// <summary>
        /// ODBC Factory
        /// </summary>
        [SQLPrefix(SQLStringPrefix = "@", ParameterPrefix = "?",IsIncludeSQLColumnName=false, StoreProcedure = "")]
        Odbcfactory,
    }

    /// <summary>
    /// SQL Command
    /// </summary>
    public enum EnumSQLCommand
    { 
        /// <summary>
        /// Insert Command
        /// </summary>
        Insert,

        /// <summary>
        /// UPdate Command
        /// </summary>
        Update,

        /// <summary>
        /// Delete Command
        /// </summary>
        Delete
    }
}
