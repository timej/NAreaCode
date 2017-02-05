using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace NAreaCode.Models
{
    class JsonUtils
    {
        public static readonly HttpClient Client = new HttpClient(new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        });

        public static void SaveToJson(string fileName, object data)
        {
            try
            {
                using (StreamWriter file = File.CreateText(fileName))
                {
                    JsonSerializer serializer = new JsonSerializer { NullValueHandling = NullValueHandling.Ignore };
                    serializer.Serialize(file, data);
                }
            }
            catch (Exception e2)
            {
                if (File.Exists(fileName))
                    File.Delete(fileName);
                Console.WriteLine("Jsonの保存に失敗しました。ファイル名: " + fileName + " " + e2.Message);
            }
        }

        public static T LoadFromJson<T>(string fileName)
        {
            try
            {
                using (StreamReader file = File.OpenText(fileName))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    return (T)serializer.Deserialize(file, typeof(T));
                }
            }
            catch (Exception e1)
            {
                Console.WriteLine("Jsonの読み込みに失敗しました。ファイル名: " + fileName + " " + e1.Message);
                return default(T);
            }
        }
    }
}
