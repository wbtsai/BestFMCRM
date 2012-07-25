using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Collections;
using System.ComponentModel;
using BestRadioNet.Core.Data.Extensions;
using BestRadioNet.Core.Logic;
using System.Reflection;
using System.Data;

using BestRadioNet.Core.Data.Attributes;


namespace BestRadioNet.Core.Data
{
    public class DataContext
    {
        public const int QueryMode = 1;
        public const int InsertMode = 2;
        public const int UpdateMode = 4;

        public const string COMMAND_INSERT = "insert";
        public const string COMMAND_UPDATE = "update";
        public const string COMMAND_DELETE = "delete";

        /// <summary>
        /// 資料字串的Key(For Enterprise Library Using) 
        /// </summary>
        public string ConnectionStringKey { get; set; }

        /// <summary>
        /// DataBase 
        /// </summary>
        public Database DBase { get; set; }

        /// <summary>
        /// Constructor Default ConnectionString
        /// </summary>
        public DataContext()
        {
            this.DBase = DatabaseFactory.CreateDatabase();
        }

        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="connStringKey"></param>
        public DataContext(string connStringKey)
        {
            this.ConnectionStringKey = connStringKey;

            this.DBase = DatabaseFactory.CreateDatabase(this.ConnectionStringKey);
        }

        public List<T> QuerySelect<T>(string sql, string mapId, params object[] parameterValues)
        {
            
            EnumSQLContext mode =mapId.GetSQLContextMap<T>().SQLContextType;

            DataTable dt = Select(mode, sql, parameterValues);

            return dt.ToGenericList<T>(mapId);
        }

        /// <summary>
        /// Query By SQL String
        /// Return DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <returns>DataTable</returns>
        public DataTable QuerySelect(string sql, params object[] parameterValues)
        {
            return Select(EnumSQLContext.SQLString, sql, parameterValues);
        }

        /// <summary>
        /// 取回資料庫資料表
        /// DataTable
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="sql"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        public DataTable QuerySelect(string sql, EnumSQLContext mode ,params object[] parameterValues)
        {
            return Select(mode,sql, parameterValues);
        }

        /// <summary>
        /// Select By SQL String
        /// </summary>
        /// <param name="sql">SQL String or PackageName</param>
        /// <returns>DataTable</returns>
        private DataTable Select(EnumSQLContext mode, string sql,params object[] parameterValues)
        {
            Database db = DBase;

            DbCommand command = null;

            switch (mode)
            { 
                case EnumSQLContext.SQLString:
                    
                    if (parameterValues != null)
                    {
                        sql = String.Format(sql, parameterValues);
                    }
                    
                    command = db.GetSqlStringCommand(sql);
                    
                    break;
                case EnumSQLContext.StoreProcedure:
                    
                    if (parameterValues != null)
                    {
                        command = db.GetStoredProcCommand(sql,parameterValues);
                    }
                    else
                    {
                        command = db.GetStoredProcCommand(sql);
                    }
                    
                    break;
                default:
                    throw new Exception(String.Format("無法取得此類型={0}的實作，請查明",mode.ToString()));
            }

            DataSet ds = new DataSet();

            db.LoadDataSet(command, ds, "Table1");

            return ds.Tables["Table1"];
        }

         /// <summary>
        /// Save Command List
        /// </summary>
        /// <param name="SqlCommands"></param>
        /// <returns></returns>
        public bool SaveChange(Func<IEnumerable> SqlCommands)
        {
            Database db = this.DBase;

            IDbConnection connection = null;

            IDbTransaction transaction = null;

            try
            {
                connection = db.CreateConnection();

                connection.Open();

                transaction = connection.BeginTransaction();

                Int32 rowsEffected = 0;

                foreach (Object s in SqlCommands())
                {

                    DbCommand command = null;

                    if (s.GetType().Equals(typeof(String)))
                    {
                        command = db.GetSqlStringCommand(s as string);
                    }
                    else
                    {
                        DbCommand cmd = s as DbCommand;
                        command = cmd;
                    }

                    
                    command.Connection = (DbConnection)connection;
                    command.Transaction = (DbTransaction)transaction;
                    rowsEffected += command.ExecuteNonQuery();
                }

                transaction.Commit();

                return rowsEffected >= 0;
            }
            catch
            {
                transaction.Rollback();

                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }

            }
        }
    }
}
