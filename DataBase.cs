using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;
using CallBook.Methods;
using System.Threading;

namespace CallBook
{
    public class DataBaseItem
    {
        public int id;

        private string _firstName = string.Empty;
        public string firstName
        {
            get { return _firstName; }
            set { if (!string.IsNullOrEmpty(value)) _firstName = value; }
        }
            
        public string secondName = string.Empty;
        public string lastName = string.Empty;
        public string phone = string.Empty;
        public string email = string.Empty;
        public string county = string.Empty;
        public string city = string.Empty;
        public string street = string.Empty;
        public string house = string.Empty;
        public int flat = -1;
    }


    class DataBase
    {
        public static string dbName = "DataBase.db";
        private static SQLiteConnection connection = null;
        
        private static object m_locker = new object();
        private static Task SQLRequestHandlerTask = null;
        public static bool SQLRequestHandlerAlive
        {
            get
            {
                return SQLRequestHandlerTask != null;
            }
        }
        
        public static bool Init()
        {
            try
            {
                if (!File.Exists(dbName))
                {
                    SQLiteConnection.CreateFile(dbName);
                }

                try //Критинская сехция 
                {
                    /*
                    id  
                    firstName фамилия 
                    secondName имя 
                    lastName отчество
                    phone - телефон строка
                    email 
                    //addr
                    county - страна
                    city - город 
                    street
                    house 
                    flat - число

                     */
                    string sql = "CREATE TABLE main (id integer primary key, " +
                     "firstName varchar(24) not null," +
                     "secondName varchar(24)," +
                     "lastName varchar(24)," +
                     "phone varchar(80) not null," +
                     "email varchar(256) not null," + //RFC 5321
                     "county varchar(24)," +
                     "city varchar(60)," +
                     "street varchar(80)," +
                     "house varchar(16)," +
                     "flat integer);";

                    connection = new SQLiteConnection("Data Source=" + dbName);
                    connection.Open();

                    SQLiteCommand command = new SQLiteCommand(connection);
                    command.CommandText = sql;
                    command.ExecuteNonQuery();

                }
                catch (Exception e)
                {

                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }
        //-------------------------------------------------------------------------------------------------
        public static bool Add(DataBaseItem dataBaseItem)
        {
            if (dataBaseItem == null) return false;

            if (string.IsNullOrEmpty(dataBaseItem.firstName)) return false;
            if (string.IsNullOrEmpty(dataBaseItem.phone)) return false;
            if (string.IsNullOrEmpty(dataBaseItem.email)) return false;

            try
            {

                string sql = "INSERT INTO main (`id`, `firstName`, `secondName`,`lastName`,`phone`,`email`,`county`,`city`,`street`,`house`,`flat`) VALUES " +
                    "(NULL," +
                    "'" + dataBaseItem.firstName + "'," +
                    "'" + dataBaseItem.secondName + "'," +
                    "'" + dataBaseItem.lastName + "'," +
                    "'" + dataBaseItem.phone + "'," +
                    "'" + dataBaseItem.email + "'," +
                    "'" + dataBaseItem.county + "'," +
                    "'" + dataBaseItem.city + "'," +
                    "'" + dataBaseItem.street + "'," +
                    "'" + dataBaseItem.house + "'," +
                    "'" + dataBaseItem.flat + "');";
                SQLiteCommand command = new SQLiteCommand(connection);
                command.CommandText = sql;
                command.ExecuteNonQuery();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }

        public static bool Edit(DataBaseItem item)
        {
            bool result = false;
            try
            {
                string sql = "UPDATE main SET" +
                    " firstName = '" + item.firstName + "'," +
                    " secondName = '" + item.secondName + "'," +
                    " lastName  = '" + item.lastName + "'," +
                    " phone  = '" + item.phone + "'," +
                    " email = '" + item.email + "'," +
                    " county = '" + item.county + "'," +
                    " city = '" + item.city + "'," +
                    " street = '" + item.street + "'," +
                    " house = '" + item.house + "'," +
                    " flat = '" + item.flat + "'" +
                    " WHERE id='" + item.id + "';";

                SQLiteCommand command = new SQLiteCommand(connection);
                command.CommandText = sql;
                command.ExecuteNonQuery();

                return true;

            }
            catch (Exception e)
            {

            }
            return result;
        }

        public static bool Delete(int id)
        {
            bool result = false;
            try
            {
                string sql = "DELETE FROM main WHERE id='" + id + "';";

                SQLiteCommand command = new SQLiteCommand(connection);
                command.CommandText = sql;
                command.ExecuteNonQuery();

                return true;

            }
            catch (Exception e)
            {

            }
            return result;

        }

        //SELECT* FROM main WHERE `firstName` LIKE '%Andrey%' OR flat = -1;
        public static bool LoadAll(object sender)
        {
            if (sender.GetType() != typeof(ItemsPreloader)) return false;
            try
            {
                //if (SQLRequestHandlerTask != null)
                //    while (SQLRequestHandlerAlive) Thread.Sleep(200);
                string sql = "SELECT * FROM main ORDER BY `firstName` ASC;";
                TaskSelectSQLRequestAndAddToPreloader(sql);

            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            return true;
        }

        public static bool Load(object sender,string firstName)
        {
            if (sender.GetType() != typeof(ItemSearching)) return false; 
            try
            {
                //Console.WriteLine("");
                string sql = "SELECT * FROM main WHERE firstName LIKE '%" + firstName + "%' or phone LIKE '%" + firstName + "%' ORDER BY `firstName` ASC;";
                //string sql = "SELECT * FROM main WHERE firstName LIKE '%" + firstName + "%' ORDER BY `firstName` ASC;";
                TaskSelectSQLRequestAndAddToPreloader(sql);

            }
            catch (Exception e)
            {
                Console.WriteLine("===== Load ==== " + e);
                return false;
            }
            return true;
        }

        private static async void TaskSelectSQLRequestAndAddToPreloader(string sql)
        {
            await RequestTask(sql);
            SQLRequestHandlerTask = null;

        }

        private static Task RequestTask(string sql)
        {
            SQLRequestHandlerTask = Task.Run(async delegate
            {
                SQLiteCommand command = new SQLiteCommand(connection);
                command.CommandText = sql;
                SQLiteDataReader reader = command.ExecuteReader();
                //if(ItemsPreloader.BufferEmpty)
                //ItemsPreloader.Clear().Wait();
                while (await reader.ReadAsync())
                {
                    DataBaseItem dataBaseItem = new DataBaseItem();

                    dataBaseItem.id = Convert.ToInt32(reader["id"]);
                    dataBaseItem.firstName = (string)reader["firstName"];
                    dataBaseItem.secondName = (string)reader["secondName"];
                    dataBaseItem.lastName = (string)reader["lastName"];
                    dataBaseItem.phone = (string)reader["phone"];
                    dataBaseItem.email = (string)reader["email"];
                    dataBaseItem.county = (string)reader["county"];
                    dataBaseItem.city = (string)reader["city"];
                    dataBaseItem.street = (string)reader["street"];
                    dataBaseItem.house = (string)reader["house"];
                    dataBaseItem.flat = Convert.ToInt32(reader["flat"]);
                    ItemsPreloader.AddToBuffer(dataBaseItem);
                }
            });
            return SQLRequestHandlerTask;
        }
    }
}
