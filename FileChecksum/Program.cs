using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using UnitTestExample;

// ReSharper disable All

namespace FileChecksum
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileMD5Dic = GetFileSetting();
            CheckFileExists(fileMD5Dic);
            CheckFileMD5(fileMD5Dic);

            Console.ReadKey();
        }

        public static (string fileName, string md5) CalculateMD5(string filePath)
        {
            using var md5 = MD5.Create();
            using var stream = File.OpenRead(filePath);
            var hash = md5.ComputeHash(stream);
            return (Path.GetFileName(filePath), BitConverter.ToString(hash).Replace("-", "").ToUpperInvariant());
        }

        public static Dictionary<string, string> GetFileSetting()
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
            var fileList = config.GetSection("FilePath").AsEnumerable().ToList();
            fileList.RemoveAt(0);
            return fileList.Select(x => CalculateMD5(x.Value)).
                ToDictionary(x => x.fileName, x => x.md5);
        }

        public static void CheckFileExists(Dictionary<string, string> fileDic)
        {
            Console.ForegroundColor = ConsoleColor.White;
            List<string> deleteKey = new List<string>();
            IDBTool dbTool = new LiteDBTool();
            foreach (var item in fileDic)
            {
                if (!dbTool.GetData<FileModifyInfo>(x => x.FileName == item.Key).Any())
                {
                    deleteKey.Add(item.Key);
                    dbTool.Insert(new FileModifyInfo
                    {
                        FileName = item.Key,
                        MD5 = item.Value,
                        CreateTime = DateTime.Now
                    });
                    Console.WriteLine($"新增{item.Key}紀錄 MD5為{item.Value}");
                }
            }
            deleteKey.ForEach(x => fileDic.Remove(x));
        }

        public static void CheckFileMD5(Dictionary<string, string> fileDic)
        {
            if (fileDic.Count == 0)
            {
                return;
            }

            IDBTool dbTool = new LiteDBTool();
            foreach (var item in fileDic)
            {
                var fileInfo = dbTool.GetData<FileModifyInfo>(x => x.FileName == item.Key).SingleOrDefault();
                if (fileInfo.MD5 != item.Value)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{item.Key}檔案異動\n本次MD5：{item.Value}\n上次MD5：{fileInfo.MD5}");
                    dbTool = new LiteDBTool();
                    fileInfo.MD5 = item.Value;
                    if (dbTool.Update(fileInfo))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"{item.Key} 已更新MD5 {item.Value}");
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{item.Key}檔案無異動\n本次MD5：{item.Value}\n上次MD5：{fileInfo.MD5}");
                }
            }
        }
    }
}
