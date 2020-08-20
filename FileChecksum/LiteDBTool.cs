using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using LiteDB;

namespace UnitTestExample
{
    public interface IDBTool
    {
        bool Exists<T>(Expression<Func<T, bool>> predicate);
        T GetData<T>(Expression<Func<T, bool>> predicate);
        void Insert<T>(T data);
        bool Delete<T>(int id);
        int DeleteAll<T>();
    }

    public class LiteDBTool : IDBTool
    {
        private LiteDatabase GetDB()
        {
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent?.FullName;
            var db = new LiteDatabase(projectDirectory + @"\DB.db");
            return db;
        }

        public bool Exists<T>(Expression<Func<T, bool>> predicate)
        {
            using var db = GetDB();
            var col = db.GetCollection<T>(nameof(T));
            var isExists = col.Query().Where(predicate).Exists();
            return isExists;
        }

        public T GetData<T>(Expression<Func<T, bool>> predicate)
        {
            using var db = GetDB();
            var col = db.GetCollection<T>(typeof(T).Name);
            var result = col.Query().Where(predicate).ToList().SingleOrDefault();
            return result;
        }

        public void Insert<T>(T data)
        {
            using var db = GetDB();
            _ = db.GetCollection<T>().Insert(data);
        }

        public bool Delete<T>(int id)
        {
            using var db = GetDB();
            var bson = new BsonValue(id);
            var isDelete = db.GetCollection<T>().Delete(bson);
            return isDelete;
        }

        public int DeleteAll<T>()
        {
            using var db = GetDB();
            var deleteCount = db.GetCollection<T>().DeleteAll();
            return deleteCount;
        }
    }
}