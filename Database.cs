using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLTSGenerator
{
    class Database : IDisposable
    {
        string connectionString = "server=127.0.0.1;uid=bhtrang;pwd=13111997;database=classicmodels";
        MySqlConnection connection;
        public Database()
        {
            connection = new MySqlConnection(connectionString);
        }
        public void Dispose()
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }
        public MySqlDataReader ExecuteReader(string query)
        {
            MySqlDataReader dataReader;
            connection.Open();
            MySqlCommand command = new MySqlCommand(query, connection);
            dataReader = command.ExecuteReader();
            return dataReader;
        }
    }
}
