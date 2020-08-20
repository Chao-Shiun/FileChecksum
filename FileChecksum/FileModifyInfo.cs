using System;

namespace FileChecksum
{
    public class FileModifyInfo
    {
        public string FileName { get; set; }
        public string MD5 { get; set; }
        public DateTime CreateTime { get; set; }
    }
}