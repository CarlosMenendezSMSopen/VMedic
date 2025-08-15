using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMedic.Interfaces;
using VMedic.MVVM.Models.Data;

namespace VMedic.Utilities
{
    public class BaseRepository<T> : IRepositorioDB<T> where T : TableData, new()
    {
        SQLiteConnection connection { get; set; } = new SQLiteConnection("", ConfigDBFile.Flags);
        public string? statusMessage { get; set; }
        public BaseRepository()
        {
            try
            {
                connection = new SQLiteConnection(ConfigDBFile.DatabasePath, ConfigDBFile.Flags);
                connection.CreateTable<T>();
            }
            catch (Exception ex)
            {
                PressedPreferences.EndPressed();
                ExceptionMessageMaker.Make("Error Repository", ex.ToString(), ex.Message, App.Current?.MainPage);
            }
        }
        public void CreateManualy()
        {
            connection.DropTable<T>();
            connection.CreateTable<T>();
        }

        public void DeleteItem(T? id)
        {
            connection.Delete(id);
        }

        public void Dispose()
        {
            connection.Close();
        }

        public void DeleteItems()
        {
            connection.DeleteAll<T>();
        }

        public void DeleteItems(IEnumerable<T> items)
        {
            foreach (T id in items)
            {
                connection.Delete(id);
            }
        }

        public T GetItem(T item)
        {
            return connection.Table<T>().FirstOrDefault(item);
        }

        public bool ItemExists(T item)
        {
            return connection.Table<T>().Where(t => t.TableID == item.TableID).ToList().Count == 1;
        }

        public T GetItem()
        {
            return connection.Table<T>().FirstOrDefault();
        }

        public List<T>? GetItems()
        {
            return connection.Table<T>().ToList();
        }

        public void InsertItem(T item)
        {
            int result = 0;
            result = connection.Insert(item);
            statusMessage = $"{result} row(s) added";
            Console.WriteLine("Result: " + statusMessage);
        }

        public void InsertItems(IEnumerable<T> items)
        {
            int result = 0;
            result = connection.InsertAll(items);
            statusMessage = $"{result} row(s) added";
            Console.WriteLine("Result: " + statusMessage);
        }

        public void UpdateITEM(T? item)
        {
            int result = 0;
            result = connection.Update(item);
            statusMessage = $"{result} row(s) added";
        }

        public bool IsEmpty()
        {
            return connection.Table<T>().ToList().Count == 0;
        }
    }

    public class ConfigDBFile
    {
        private const string DBFile = "VMedic.db3";

        public const SQLiteOpenFlags Flags =
            SQLiteOpenFlags.ReadWrite |
            SQLiteOpenFlags.Create |
            SQLiteOpenFlags.SharedCache;

        public static string DatabasePath
        {
            get
            {
                return Path.Combine(FileSystem.AppDataDirectory, DBFile);
            }
        }
    }
}
