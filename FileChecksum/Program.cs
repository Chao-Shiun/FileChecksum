using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;

namespace FileChecksum
{
    class Program
    {
        static void Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
            var fileList = config.GetSection("FilePath").AsEnumerable().ToList();
            fileList.RemoveAt(0);
            foreach (var item in fileList)
            {
                Console.WriteLine(CalculateMD5(item.Value));
            }

            var fileMD5Dic = fileList.Select(x => CalculateMD5(x.Value)).
                ToDictionary(x => x.fileName, x => x.md5);

            Console.ReadKey();
        }

        public static (string fileName, string md5) CalculateMD5(string filePath)
        {
            using var md5 = MD5.Create();
            using var stream = File.OpenRead(filePath);
            var hash = md5.ComputeHash(stream);
            return (Path.GetFileName(filePath), BitConverter.ToString(hash).Replace("-", "").ToUpperInvariant());
        }
    }
}
