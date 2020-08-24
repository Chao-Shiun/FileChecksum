using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using LiteDB;

namespace UnitTestExample
{
    public interface IDBTool
    {
        bool Exists<T>(Expression<Func<T, bool>> predicate);
        List<T> GetData<T>(Expression<Func<T, bool>> predicate);
        void Insert<T>(T data);
        bool Update<T>(T data);
        bool Delete<T>(int id);
        int DeleteAll<T>();
    }

    public class LiteDBTool : IDBTool
    {
        private LiteDatabase GetDB(BsonMapper bsonMapper = null)
        {
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent?.FullName;
            var db = new LiteDatabase(new ConnectionString(projectDirectory + @"\Debug\netcoreapp3.1" + @"\DB.db"), bsonMapper);
            return db;
        }

        public bool Exists<T>(Expression<Func<T, bool>> predicate)
        {
            using var db = GetDB();
            var col = db.GetCollection<T>(nameof(T));
            var isExists = col.Query().Where(predicate).ToList().Any();
            return isExists;
        }

        public List<T> GetData<T>(Expression<Func<T, bool>> predicate)
        {
            using var db = GetDB();
            var col = db.GetCollection<T>(typeof(T).Name);
            var result = col.Query().Where(predicate).ToList();
            return result;
        }

        public void Insert<T>(T data)
        {
            using var db = GetDB();
            _ = db.GetCollection<T>().Insert(data);
        }

        public bool Update<T>(T data)
        {
            using var db = GetDB();
            return db.GetCollection<T>().Update(data);
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