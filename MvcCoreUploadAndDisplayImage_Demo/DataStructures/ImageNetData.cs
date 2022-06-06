using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.ML.Data;

namespace MvcCoreUploadAndDisplayImage_Demo.DataStructures
{
    public class ImageNetData
    {
        [LoadColumn(0)]
        public string ImagePath;


        //public static string GetAbsolutePath(string relativePath)
        //{
        //    FileInfo _dataRoot = new FileInfo(typeof(Program).Assembly.Location);
        //    string assemblyFolderPath = _dataRoot.Directory.FullName;

        //    string fullPath = Path.Combine(assemblyFolderPath, relativePath);

        //    return fullPath;
        //}

        public static IEnumerable<ImageNetData> ReadFromFile(string imageFolder, string image_guid)
        {
            yield return new ImageNetData { ImagePath = Path.Combine(imageFolder, image_guid) };
            ;
        }
    }
}
