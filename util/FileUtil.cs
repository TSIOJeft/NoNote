using System;
using System.IO;

namespace NoNote.util
{
    public class FileUtil
    {
        public static string getTextFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    // 读取整个文件内容
                    string fileContent = reader.ReadToEnd();
                    reader.Close();
                    return fileContent;
                }
            }

            return null;
        }

        public static void saveTextFile(string filePath, string content)
        {   if(filePath==null)return;
            string parentFolder = Path.GetDirectoryName(filePath);
            if (parentFolder != null)
            {
                if (!Directory.Exists(parentFolder))
                    Directory.CreateDirectory(parentFolder);
            }

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.Write(content);
                writer.Close();
            }
        }

        public static string saveImgFile(string filePath, byte[] data)
        {
            string parentFolder = Path.GetDirectoryName(filePath);
            if (parentFolder != null)
            {
                if (!Directory.Exists(parentFolder))
                    Directory.CreateDirectory(parentFolder);
            }

            if (File.Exists(filePath))
            {
                filePath = parentFolder + "\\" + DateTimeOffset.Now.ToUnixTimeMilliseconds() + "_" +
                           new FileInfo(filePath).Name;
                return saveImgFile(filePath, data);
            }

            File.WriteAllBytes(filePath, data);
            return filePath;
        }
    }
}