﻿using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace SimpleORM.Util {
   public class SQLExecutor {
        public static DataTable GetDataTable(string query, string connectionString, params object[] parameters) {
            try {
                Database db = new SqlDatabase(connectionString);
                DbCommand cmd = db.GetSqlStringCommand(query);
                using (cmd.Connection) {
                    Debug.WriteLine("ConnectionString  = " + connectionString);
                    if ((parameters != null)) {
                        for (int i = 1; i <= parameters.Length; i++) {
                            if (parameters[i - 1] == null) {
                                cmd.Parameters.Add(new SqlParameter("@" + i, DBNull.Value));
                            } else {
                                cmd.Parameters.Add(new SqlParameter("@" + i, parameters[i - 1]));
                            }
                        }
                    }
                    DataSet ds = db.ExecuteDataSet(cmd);
                    var index = query.ToUpper().IndexOf("from".ToUpper());
                    var tableName = Regex.Match(query.Substring(index + "from".Length), @"^\s*(\w*)").Groups[1].Value;
                    ds.Tables[0].TableName = tableName;
                    return ds.Tables[0].Copy();
                }
            } catch (System.Exception ex) {
                throw new System.Exception("Error Occurs in function:  " + System.Reflection.MethodInfo.GetCurrentMethod().ToString(), ex);
            }
        }

        public static int ExecuteNonQuery(string query, string connectionString, params object[] parameters) {
            try {
                Database db = new SqlDatabase(connectionString);

                DbCommand cmd = db.GetSqlStringCommand(query);
                Debug.WriteLine("ConnectionString  = " + connectionString);
                using (cmd.Connection) {

                    if ((parameters != null)) {
                        for (int i = 1; i <= parameters.Length; i++) {
                            if (parameters[i - 1] == null) {
                                cmd.Parameters.Add(new SqlParameter("@" + i, DBNull.Value));
                            } else {
                                cmd.Parameters.Add(new SqlParameter("@" + i, parameters[i - 1]));
                            }
                        }
                    }
                    int ret = db.ExecuteNonQuery(cmd);
                    cmd.Connection.Close();
                    return ret;
                }
            } catch (System.Exception ex) {
                throw new System.Exception("Error Occurs in function:  " + System.Reflection.MethodInfo.GetCurrentMethod().ToString() , ex);
            }
        }
    }
}
