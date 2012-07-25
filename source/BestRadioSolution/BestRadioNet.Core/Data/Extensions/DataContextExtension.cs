using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;
using BestRadioNet.Core.Data.Attributes;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using Microsoft.Practices.EnterpriseLibrary.Data.Oracle;
using BestRadioNet.Core.Logic;
using BestRadioNet.Core.Helper;




namespace BestRadioNet.Core.Data.Extensions
{


    /// <summary>
    /// Data Context Extensions
    /// Support DataTable To Generic List
    /// </summary>
    public static class DataContextExtension
    {
       
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<TResult> ToGenericList<TResult>(this DataTable dt,string mapId) 
        {

            List<DataContextColumnAttribute> prlist = mapId.GetContextColumnList<TResult>();

            List<TResult> oblist = new List<TResult>();

            foreach (DataRow row in dt.Rows)
            {

                TResult ob = Activator.CreateInstance<TResult>();

                prlist.ForEach(p =>
                {

                    string colName =p.ColumnName;
                                       

                    if (row[colName] != DBNull.Value)
                    {
                        ob.GetType().GetProperty(p.ProperyName).SetValue(ob, Convert.ChangeType(row[colName], p.PropertyType), null);
                    }
                });

                oblist.Add(ob);
            
            }
            return oblist;
        }

        public static List<DataContextColumnAttribute> GetContextColumnList<TResult>(this string mapId)
        {
            List<DataContextColumnAttribute> prlist = new List<DataContextColumnAttribute>();

            Type t = typeof(TResult);

            Array.ForEach<PropertyInfo>(t.GetProperties(), p =>
            {
                var q = (from colAttr in (DataContextColumnAttribute[])p.GetCustomAttributes(typeof(DataContextColumnAttribute), false)
                         where colAttr.MapKey == mapId
                         select colAttr).ToList();

                if (q.Count > 0)
                {
                    DataContextColumnAttribute q1 = q.First();

                    q1.ProperyName = p.Name;
                    q1.PropertyType = p.PropertyType;

                    prlist.Add(q1); //Add DataContextColumnAttribute
                }

            });

            return prlist;
        }

        /// <summary>
        /// 由Generic List to DataTable
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="value"></param>
        /// <param name="sqlMode"></param>
        /// <returns></returns>
        public static DataTable ToGenericDataTable<TResult>(this IEnumerable<TResult> value,string mapId) where TResult : class
        {
            List<DataContextColumnAttribute> prlist = mapId.GetContextColumnList<TResult>();

            DataTable dt = new DataTable();

            prlist.ForEach(p => {
                dt.Columns.Add(p.ColumnName);
            });

            foreach (var item in value)
            {
                DataRow row = dt.NewRow();

                prlist.ForEach(p =>
                {
                    row[p.ColumnName] =item.GetType().GetProperty(p.ProperyName).GetValue(item, null);
                });

                dt.Rows.Add(row);
            }

            return dt;
        }

        /// <summary>
        /// 取得Map ID 的的設定程式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mapId"></param>
        /// <returns></returns>
        public static DataContextMapAttribute GetSQLContextMap<T>(this string mapId)
        {
            ReflectionLogic logic = new ReflectionLogic();

            List<ConstructorInfo> cList = logic.RetriveConstructorInfo(typeof(T));

            DataContextMapAttribute map = null;


            cList.ForEach(p =>
            {

                DataContextMapAttribute[] attrs = (DataContextMapAttribute[])p.GetCustomAttributes(typeof(DataContextMapAttribute), false);

                foreach (DataContextMapAttribute t in attrs)
                {
                    if (t.MapKey == mapId)
                    {
                        map = t;
                    }
                }
            });

            if (map == null)
            {
                throw new Exception("無法取得mapid=" + mapId + "的模式");
            }

            return map;
        }

        /// <summary>
        /// 取得一筆Command 資料
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="db"></param>
        /// <param name="mapId"></param>
        /// <returns></returns>
        public static DbCommand GetDBCommand<T>(this T obj, Database db, string mapId)
        {
            DbCommand comm = null;

            DataContextMapAttribute map=mapId.GetSQLContextMap<T>();

            switch (map.SQLContextType)
            { 
                case EnumSQLContext.SQLString:

                    //取回資料庫連線
                    string sql =obj.QuerySqlString<T>(db,mapId);

                    //取回Command
                    comm = db.GetSqlStringCommand(sql);

                    //取回有多少參數
                    List<DbParameter> pList=obj.GetDBParameterList<T>(db,comm,mapId,false);

                    pList.ForEach(p => {
                        comm.Parameters.Add(p);
                    });

                    break;
                
                case EnumSQLContext.StoreProcedure:
                    
                    comm = db.GetStoredProcCommand(map.PKNameOrTableName);

                    pList = obj.GetDBParameterList<T>(db,comm, mapId,true);

                    pList.ForEach(p =>
                    {
                        comm.Parameters.Add(p);
                    });

                    break;
                default:
                    throw new Exception("尚未實作此項目");
            }

            return comm;

        }

        /// <summary>
        /// 取得Command List 的Command List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="db"></param>
        /// <param name="mapId"></param>
        /// <returns></returns>
        public static List<DbCommand> GetDBCommand<T>(this List<T> list, Database db, string mapId)
        {
            List<DbCommand> commList = new List<DbCommand>();

            list.ForEach(p => {
                
                DbCommand comm = p.GetDBCommand<T>(db, mapId);
                
                commList.Add(comm);
            });

            return commList;
        }

        

        /// <summary>
        /// 這是針對SQL String
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="db"></param>
        /// <param name="SQLCommand"></param>
        /// <returns></returns>
        public static string QuerySqlString<T>(this T obj, Database db,string mapId)
        {
            DataContextMapAttribute map = mapId.GetSQLContextMap<T>();

            string sql = "";

            switch (map.SQLCommand)
            { 
                case EnumSQLCommand.Insert:

                    sql = GetInsertString<T>(db,mapId);
                    
                    break;
                case EnumSQLCommand.Update:
                    
                    sql = GetUpdateString<T>(db, mapId,obj);
                    
                    break;
                case EnumSQLCommand.Delete:
                    
                    sql = GetDeleteString<T>(db, mapId,obj);
                    
                    break;
            }

            return sql;


        }

        /// <summary>
        /// 取得刪除的SQL String
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="db"></param>
        /// <param name="mapId"></param>
        /// <returns></returns>
        private static string GetDeleteString<T>(Database db, string mapId,T obj)
        {
            DataContextMapAttribute map = mapId.GetSQLContextMap<T>();

            SQLPrefixAttribute pre = db.DbProviderFactory.GetType().Name.StringToEnum<EnumSQLDataBaseProvider>().GetAttribute<SQLPrefixAttribute>();

            string sql = " delete from ";

            sql += map.PKNameOrTableName;

            //Array.ForEach(map.Parameters,c=>{
                
            //    c=pre.SQLStringPrefix+c;
            
            //});

            object[] pa1 = new object[map.Parameters.Length];

            int i = 0;

            foreach (string s in map.Parameters)
            {
                pa1[i] = obj.GetType().GetProperty(s).GetValue(obj, null);

                i++;
            }

            sql += String.Format(map.WhereSQL, pa1);


            return sql;            
        }

        /// <summary>
        /// 取得目前的資料庫格式
        /// For Oracle DataBase
        /// For SQL  DataBase
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        //public static EnumSQLDataBaseProvider GetDBProvider(this Database db)
        //{
        //    EnumSQLDataBaseProvider p = EnumSQLDataBaseProvider.None;

        //    p = db.DbProviderFactory.GetType().Name.StringToEnum<EnumSQLDataBaseProvider>();

        //    //if (db.GetType().Equals(typeof(OracleDatabase)))
        //    //{
        //    //    p = EnumSQLDataBaseProvider.OracleDatabase;
        //    //}
        //    //else if (db.GetType().Equals(typeof(SqlDatabase)))
        //    //{
        //    //    p = EnumSQLDataBaseProvider.SqlDatabase;
        //    //}


        //    return p;
        //}

        

        /// <summary>
        /// 產生Update Data
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="db"></param>
        /// <param name="mapId"></param>
        /// <returns></returns>
        private static string GetUpdateString<T>(Database db, string mapId,T obj)
        {
            DataContextMapAttribute map = mapId.GetSQLContextMap<T>();

            List<DataContextColumnAttribute> prList = mapId.GetContextColumnList<T>();

            SQLPrefixAttribute pre = db.DbProviderFactory.GetType().Name.StringToEnum<EnumSQLDataBaseProvider>().GetAttribute<SQLPrefixAttribute>();

            string sql = " update ";

            sql += map.PKNameOrTableName;

            sql += " set ";

            prList.ForEach(p => {

                sql += p.ColumnName +"="+pre.SQLStringPrefix;

                if (pre.IsIncludeSQLColumnName)
                {
                    sql += p.ColumnName;
                }

                sql +=  ",";

            });

            if (sql.Length > 0)
            {
                sql = sql.Substring(0, sql.Length - 1);
            }

            object[] pa1=new object[map.Parameters.Length];

            int i = 0;

            foreach (string s in map.Parameters)
            {
                pa1[i] = obj.GetType().GetProperty(s).GetValue(obj, null);

                i++;
            }

            sql += String.Format(map.WhereSQL, pa1);

            return sql; 
        }

        /// <summary>
        /// 產生Insert 資料
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="db"></param>
        /// <param name="mapId"></param>
        /// <returns></returns>
        private static string GetInsertString<T>(Database db, string mapId)
        {
            DataContextMapAttribute map = mapId.GetSQLContextMap<T>();

            List<DataContextColumnAttribute> prList = mapId.GetContextColumnList<T>();

            SQLPrefixAttribute pre = db.DbProviderFactory.GetType().Name.StringToEnum<EnumSQLDataBaseProvider>().GetAttribute<SQLPrefixAttribute>();

            string sql = " insert into ";

            sql += map.PKNameOrTableName;

            sql += " ( ";

            string paraSQL="";

            prList.ForEach(p =>
            {

                paraSQL += p.ColumnName + ","; 

            });

            if (paraSQL.Length > 0)
            {
                paraSQL = paraSQL.Substring(0, paraSQL.Length - 1);
            }

            sql += paraSQL;

            sql += " ) ";

            sql += "values";

            sql += " ( ";

            string objSQL = "";

            prList.ForEach(p =>
            {

                objSQL +=pre.SQLStringPrefix;

                if (pre.IsIncludeSQLColumnName)
                {
                    objSQL += p.ColumnName; 
                }

                objSQL += ",";

            });

            if (objSQL.Length > 0)
            {
                objSQL = objSQL.Substring(0, objSQL.Length - 1);
            }

            sql += objSQL;

            sql += " ) ";

            return sql; 
        }

        
        /// <summary>
        /// 取得一個物件的參數列表
        /// </summary>
        /// <typeparam name="Tobj"></typeparam>
        /// <param name="obj"></param>
        /// <param name="db"></param>
        /// <param name="mapId"></param>
        /// <returns></returns>
        public static List<DbParameter> GetDBParameterList<Tobj>(this Tobj obj, Database db,DbCommand comm ,string mapId,bool isForStoreProcedure)
        {
            List<DbParameter> list = new List<DbParameter>();

            List<DataContextColumnAttribute> prList = mapId.GetContextColumnList<Tobj>();

            SQLPrefixAttribute pre = db.DbProviderFactory.GetType().Name.StringToEnum<EnumSQLDataBaseProvider>().GetAttribute<SQLPrefixAttribute>();

            string preFix=pre.ParameterPrefix;

            if (isForStoreProcedure)
            {
                preFix = pre.StoreProcedure;
            }

            EnumSQLDataBaseProvider provider = db.DbProviderFactory.GetType().Name.StringToEnum<EnumSQLDataBaseProvider>(); 

            prList.ForEach(p =>
            {

                DbParameter p1 = comm.CreateParameter();

                p1.ParameterName = preFix + p.ColumnName;

                p1.Value = obj.GetType().GetProperty(p.ProperyName).GetValue(obj, null);

                list.Add(p1);
            });


            //switch (provider)
            //{ 
            //    case EnumSQLDataBaseProvider.OracleDatabase:

            //        prList.ForEach(p => {
                        
            //            OracleParameter p1 = new OracleParameter();

            //            p1.ParameterName = preFix + p.ColumnName;
                        
            //            p1.OracleType = p.PropertyType.GetOracleDBType();

            //            p1.Value = obj.GetType().GetProperty(p.ProperyName).GetValue(obj, null);

            //            list.Add(p1);
            //        });
                    
            //        break;
            //    case EnumSQLDataBaseProvider.SqlDatabase:

            //        prList.ForEach(p =>
            //        {

            //            SqlParameter p1 = new SqlParameter();

            //            p1.ParameterName = preFix + p.ColumnName;

            //            p1.SqlDbType = p.PropertyType.GetSQLDBType();

            //            p1.Value = obj.GetType().GetProperty(p.ProperyName).GetValue(obj, null);

            //            list.Add(p1);
            //        });

            //        break;
            //    case EnumSQLDataBaseProvider.SQLiteDatabase:
            //        prList.ForEach(p =>
            //        {

            //            SQLiteParameter p1 = new SQLiteParameter();

            //            p1.ParameterName = preFix + p.ColumnName;

            //            p1.DbType = p.PropertyType.GetSQLiteType();

            //            p1.Value = obj.GetType().GetProperty(p.ProperyName).GetValue(obj, null);

            //            list.Add(p1);
            //        });
            //        break;
            //    default:
            //        throw new Exception("無法取得此資料庫的實作模式，請查明");
            //}


            return list;
        }

        //public static DbType GetSQLiteType(this Type theType)
        //{
        //    SQLiteParameter p1;
        //    TypeConverter tc;
        //    p1 = new SQLiteParameter();
        //    tc = TypeDescriptor.GetConverter(p1.DbType);
        //    if (tc.CanConvertFrom(theType))
        //    {
        //        p1.DbType = (DbType)tc.ConvertFrom(theType.Name);
        //    }
        //    else
        //    {
        //        //Try brute force 
        //        try
        //        {
        //            p1.DbType = (DbType)tc.ConvertFrom(theType.Name);
        //        }
        //        catch (Exception ex)
        //        {
        //            //Do Nothing 
        //        }
        //    }
        //    return p1.DbType;
        //}

        //public static SqlDbType GetSQLDBType(this Type theType)
        //{
        //    SqlParameter p1;
        //    TypeConverter tc;
        //    p1 = new SqlParameter();
        //    tc = TypeDescriptor.GetConverter(p1.DbType);
        //    if (tc.CanConvertFrom(theType))
        //    {
        //        p1.DbType = (DbType)tc.ConvertFrom(theType.Name);
        //    }
        //    else
        //    {
        //        //Try brute force 
        //        try
        //        {
        //            p1.DbType = (DbType)tc.ConvertFrom(theType.Name);
        //        }
        //        catch (Exception ex)
        //        {
        //            //Do Nothing 
        //        }
        //    }
        //    return p1.SqlDbType;
        //}

        ///// <summary>
        ///// 取得Oracle DB Type
        ///// </summary>
        ///// <param name="theType"></param>
        ///// <returns></returns>
        //public static OracleType GetOracleDBType(this Type theType)
        //{
        //    OracleParameter p1;
        //    TypeConverter tc;
        //    p1 = new OracleParameter();
        //    tc = TypeDescriptor.GetConverter(p1.DbType);
        //    if (tc.CanConvertFrom(theType))
        //    {
        //        p1.DbType = (DbType)tc.ConvertFrom(theType.Name);
        //    }
        //    else
        //    {
        //        //Try brute force 
        //        try
        //        {
        //            p1.DbType = (DbType)tc.ConvertFrom(theType.Name);
        //        }
        //        catch (Exception ex)
        //        {
        //            //Do Nothing 
        //        }
        //    }
        //    return p1.OracleType;
        //}
    }
}
