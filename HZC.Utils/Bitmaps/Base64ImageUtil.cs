using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace HZC.Utils
{
    /// <summary>
    /// Base64图片操作类
    /// </summary>
    public static class Base64ImageUtil
    {
        /// <summary>
        /// Base64字符串转JPEG
        /// </summary>
        /// <param name="inputStr"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool Base64StringToImage(string inputStr, string filePath)
        {
            try
            {
                var dummyData = inputStr.Trim().Replace("%", "").Replace(",", "").Replace(" ", "+");
                if (dummyData.Length % 4 > 0)
                {
                    dummyData = dummyData.PadRight(dummyData.Length + 4 - dummyData.Length % 4, '=');
                }
                var arr = Convert.FromBase64String(dummyData);
                var ms = new MemoryStream(arr);
                var bmp = new Bitmap(ms);

                var localPath = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(localPath))
                {
                    Directory.CreateDirectory(localPath);
                }

                bmp.Save(filePath, ImageFormat.Jpeg);
                ms.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 根据图片路径获取图片后转Base64
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string ImageToBase64(string filePath)
        {
            var m = new MemoryStream();
            var bp = new Bitmap(filePath);
            bp.Save(m, bp.RawFormat);
            var b = m.GetBuffer();
            var base64String = Convert.ToBase64String(b);
            return base64String;
        }

        /// <summary>
        /// Bitmap转Base64
        /// </summary>
        /// <param name="img">要转换的图片</param>
        /// <returns></returns>
        public static string ImageToBase64(Bitmap img)
        {
            var m = new MemoryStream();
            img.Save(m, img.RawFormat);
            var b = m.GetBuffer();
            var base64String = Convert.ToBase64String(b);
            return base64String;
        }
    }
}
