using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Web.SessionState;
using System.Web.Configuration;
using System.Configuration;
/// <summary>
/// Summary description for GlobeDAL
/// </summary>
public class PfizerDAL
{
    private int connectiontimeout;
    HttpSessionState Session;
    string poolingstr = string.Empty;
    string minpoolsize = string.Empty;
    string maxpoolsize = string.Empty;
    SqlConnection con = new SqlConnection();
    SqlCommand com;
    SqlDataAdapter ada;
    SqlTransaction transaction;
    public PfizerDAL(HttpSessionState CurrentSession)
    {
        Session = CurrentSession;
    }
    private int ConnectionTimeOut
    {
        set
        {
            connectiontimeout = 30;
        }
    }
    private SqlConnection GetConnection()
    {

        string connectionstring = ConfigurationManager.ConnectionStrings["MyConn"].ConnectionString;
        if (WebConfigurationManager.AppSettings.Get("mode") == "Disconnected")
        {
            minpoolsize = WebConfigurationManager.AppSettings.Get("Minconnections");
            maxpoolsize = WebConfigurationManager.AppSettings.Get("Maxconnections");
            poolingstr = "Pooling=true;Min Pool Size=" + minpoolsize + ";Max Pool Size=" + maxpoolsize;
            con.ConnectionString = poolingstr;
            if (con.State == 0)
                con.Open();

        }

        if (WebConfigurationManager.AppSettings.Get("mode") == "Connected" || WebConfigurationManager.AppSettings.Get("mode") == null)
        {
            if (Session["ConnectionObj"] != null)
            {
                con = (SqlConnection)Session["ConnectionObj"];
                if (con.State == 0)
                    con.Open();
            }
            else
            {
                con.ConnectionString = connectionstring;
                if (con.State == 0)
                    con.Open();
                Session.Add("ConnectionObj", con);
            }
        }
        return con;

    }
    public void BeginTransaction()
    {
        SqlConnection con = GetConnection();
        transaction = con.BeginTransaction();
        Session.Add("Transaction", transaction);
    }
    public void CommitTransaction()
    {
        transaction = (SqlTransaction)Session["Transaction"];
        transaction.Commit();
        Session["Transaction"] = null;
    }
    public void RollbackTransaction()
    {
        transaction = (SqlTransaction)Session["Transaction"];
        if (transaction != null)
        {
            transaction.Rollback();
            Session["Transaction"] = null;
        }
    }
    public int ExecuteDML(string ComandText, CommandType Commandtype, params SqlParameter[] parameters_values)
    {
        com = new SqlCommand();
        if (Session["Transaction"] != null)
        {
            com.Transaction = transaction;
        }
        com.Connection = GetConnection();
        com.CommandText = ComandText;
        com.CommandType = Commandtype;
        com.CommandTimeout = connectiontimeout;
        
        foreach (SqlParameter param in parameters_values)
        {
            com.Parameters.Add(param);
        }

        int rowsaffected = com.ExecuteNonQuery();
        com.Dispose();
        return rowsaffected;
    }
    public void ReturnDataSet(string CommandText, CommandType Commandtype, DataSet ds, params SqlParameter[] parameters_values)
    {
        com = new SqlCommand();
        if (Session["Transaction"] != null)
        {
            com.Transaction = transaction;
        }
        com.Connection = GetConnection();
        com.CommandText = CommandText;
        com.CommandType = Commandtype;
        com.CommandTimeout = connectiontimeout;
        foreach (SqlParameter param in parameters_values)
        {
            com.Parameters.Add(param);
        }
        ada = new SqlDataAdapter(com);
        ada.Fill(ds);
        com.Dispose();
        ada.Dispose();

    }
    public void ReturnDataTable(string CommandText, CommandType Commandtype, DataTable dt, params SqlParameter[] parameters_values)
    {
        com = new SqlCommand();

        if (Session["Transaction"] != null)
        {
            com.Transaction = transaction;
        }
        com.Connection = GetConnection();
        com.CommandText = CommandText;
        com.CommandType = Commandtype;
        com.CommandTimeout = connectiontimeout;
        foreach (SqlParameter param in parameters_values)
        {

            com.Parameters.Add(param);
        }
        ada = new SqlDataAdapter(com);
        ada.Fill(dt);
        com.Dispose();
        ada.Dispose();

    }
 


    public string ReturnScalar(string CommandText, CommandType Commandtype, params SqlParameter[] parameters_values)
    {
        com = new SqlCommand();

        if (Session["Transaction"] != null)
        {
            com.Transaction = transaction;
        }
        com.Connection = GetConnection();
        com.CommandText = CommandText;
        com.CommandType = Commandtype;
        com.CommandTimeout = connectiontimeout;
        foreach (SqlParameter param in parameters_values)
        {

            com.Parameters.Add(param);
        }
        object obj = com.ExecuteScalar();
        com.Dispose();
        if (obj == null)
            return "";
        else
            return obj.ToString();
    }
    public SqlDataReader ReturnReader(string CommandText, CommandType Commandtype, params SqlParameter[] parameters_values)
    {
        com = new SqlCommand();

        if (Session["Transaction"] != null)
        {
            com.Transaction = transaction;
        }
        com.Connection = GetConnection();
        com.CommandText = CommandText;
        com.CommandType = Commandtype;
        com.CommandTimeout = connectiontimeout;
        foreach (SqlParameter param in parameters_values)
        {
            com.Parameters.Add(param);
        }
        SqlDataReader sqldatareader = com.ExecuteReader();
        com.Dispose();
        return sqldatareader;
    }
}