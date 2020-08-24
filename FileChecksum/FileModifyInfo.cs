using System;
using LiteDB;

namespace FileChecksum
{
    public class FileModifyInfo
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string FileName { get; set; }
        public string MD5 { get; set; }
        public DateTime CreateTime { get; set; }
    }
}