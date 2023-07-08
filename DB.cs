using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class DB
{

    private static string DBFilePath = "tasks.db";
    private static string ConnectionString = "";

    public DB()
    {
        InitDB();
    }


    private void InitDB()
    {
        if (!System.IO.File.Exists(DBFilePath))
        {
            SQLiteConnection.CreateFile(DBFilePath);

            ConnectionString = GetConnectionString();

            CheckTables();
        }
        else
        {
            ConnectionString = GetConnectionString();
            CheckTables();
        }
    }

    private String GetConnectionString()
    {
        SQLiteConnectionStringBuilder connectionStringBuilder = new SQLiteConnectionStringBuilder();
        connectionStringBuilder.DataSource = DBFilePath;
        connectionStringBuilder.Version = 3;

        ConnectionString = connectionStringBuilder.ToString();
        return ConnectionString;
    }

    private SQLiteConnection GetConnection()
    {
        SQLiteConnection connection = new SQLiteConnection(ConnectionString);
        connection.Open();

        return connection;
    }

    private void CheckTables()
    {
        using (SQLiteConnection connection = GetConnection())
        {
            
            string createTableQuery = @"
                
                CREATE TABLE IF NOT EXISTS CDV (
                    CODE INTEGER PRIMARY KEY,
                    DESC TEXT
                );

                CREATE TABLE IF NOT EXISTS TASKS (
                    TASKID INTEGER PRIMARY KEY AUTOINCREMENT,
                    CDVCODE TEXT,
                    STARTTIME TEXT,
                    ENDTIME TEXT
                 );

                CREATE TABLE IF NOT EXISTS OBJECTIVES (
                    CDV_CODE INTEGER,
                    DESCRIPTION TEXT,
                    FOREIGN KEY (CDV_CODE) REFERENCES CDV(CODE)
                );
                ";

            using (SQLiteCommand command = new SQLiteCommand(createTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }

    public DataTable SelectDT(string query)
    {
        DataTable dataTable = new DataTable();

        if (IsSelectQuery(query))
        {
            using (SQLiteConnection connection = GetConnection())
            {
             
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }
        }
        else
        {
            throw new Exception("Not a SELECT query");
        }

        return dataTable;
    }

    private bool IsSelectQuery(string query)
    {
        Boolean isSelect = false;
        query = query.Trim();
        string upperQuery = query.ToUpper();
        if (upperQuery.StartsWith("SELECT "))
        {
            if (!upperQuery.StartsWith("SELECT COUNT(*)"))
            {
                isSelect = true;
            }
        }
        return isSelect;
    }

    public void ExecuteNonQuery(string query, Dictionary<string, object> parameters)
    {
        using (SQLiteConnection connection = GetConnection())
        {
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                foreach (KeyValuePair<string, object> parameter in parameters)
                {
                    command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                }
               
                command.ExecuteNonQuery();
            }
        }
    }

   
}

