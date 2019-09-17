using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HZC.Utils.UpYun
{
    // 又拍云上传图片
    public class UpYunService
    {
        private readonly string _bucketName;
        private readonly string _username;
        private readonly string _password;
        private bool _upAuth;
        private string _apiDomain = "v0.api.upyun.com";
        private const string Dl = "/";
        private Dictionary<string, object> _tmpInfos = new Dictionary<string, object>();
        private string _fileSecret;
        private string _contentMd5;
        private bool _autoMkdir;
        public string Domain { get; private set; }
        public string Version() { return "1.0.1"; }

        /**
        * 初始化 UpYun 存储接口
        * @param $bucketName 空间名称
        * @param $username 操作员名称
        * @param $password 密码
        * return UpYun object
        */
        public UpYunService(IOptions<UpYunSettings> options)
        {
            var settings = options.Value;
            _bucketName = settings.BucketName;
            _username = settings.UserName;
            _password = settings.Password;
            Domain = settings.Domain;
            if (!string.IsNullOrWhiteSpace(settings.ApiDomain))
            {
                _apiDomain = settings.ApiDomain;
            }
        }

        /**
        * 切换 API 接口的域名
        * @param $domain {默认 v0.api.upyun.com 自动识别, v1.api.upyun.com 电信, v2.api.upyun.com 联通, v3.api.upyun.com 移动}
        * return null;
        */
        public void SetApiDomain(string domain)
        {
            _apiDomain = domain;
        }

        /**
        * 是否启用 又拍签名认证
        * @param upAuth {默认 false 不启用(直接使用basic auth)，true 启用又拍签名认证}
        * return null;
        */
        public void SetAuthType(bool upAuth)
        {
            _upAuth = upAuth;
        }

        private void UpYunAuth(ByteArrayContent requestContent, string method, string uri)
        {
            var dt = DateTime.UtcNow;
            var date = dt.ToString("ddd, dd MMM yyyy HH':'mm':'ss 'GMT'", new CultureInfo("en-US"));

            requestContent.Headers.Add("Date", date);
            var body = requestContent.ReadAsStringAsync().Result;
            var auth = !string.IsNullOrEmpty(body)
                ? Md5(method + '&' + uri + '&' + date + '&' + requestContent.ReadAsByteArrayAsync().Result.Length +
                      '&' + Md5(_password))
                : Md5(method + '&' + uri + '&' + date + '&' + 0 + '&' + Md5(_password));
            requestContent.Headers.Add("Authorization", "UpYun " + _username + ':' + auth);
        }

        private static string Md5(string str)
        {
            using (var m = MD5.Create())
            {
                var s = m.ComputeHash(Encoding.UTF8.GetBytes(str));
                var result = BitConverter.ToString(s);
                result = result.Replace("-", "");
                return result.ToLower();
            }
        }
        private async Task<bool> DeleteAsync(string path, Dictionary<string, object> headers)
        {
            var resp = await NewWorker("DELETE", Dl + _bucketName + path, null, headers);
            return resp.StatusCode == System.Net.HttpStatusCode.OK;
        }


        private async Task<HttpResponseMessage> NewWorker(string method, string url, byte[] postData, Dictionary<string, object> headers)
        {
            using (var handler = new HttpClientHandler { UseProxy = false })
            using (var httpClient = new HttpClient(handler))
            using (var byteContent = new ByteArrayContent(postData))
            {
                httpClient.BaseAddress = new Uri("http://" + _apiDomain);

                if (_autoMkdir)
                {
                    byteContent.Headers.Add("mkdir", "true");
                    _autoMkdir = false;
                }

                if (postData != null)
                {
                    if (_contentMd5 != null)
                    {
                        byteContent.Headers.Add("Content-MD5", _contentMd5);
                        _contentMd5 = null;
                    }
                    if (_fileSecret != null)
                    {
                        byteContent.Headers.Add("Content-Secret", _fileSecret);
                        _fileSecret = null;
                    }
                }

                if (_upAuth)
                {
                    UpYunAuth(byteContent, method, url);
                }
                else
                {
                    //byteContent.Headers.Add("Authorization", "Basic " +
                    //Convert.ToBase64String(new System.Text.ASCIIEncoding().GetBytes(this.username + ":" + this.password)));
                    var value = Convert.ToBase64String(new ASCIIEncoding().GetBytes(_username + ":" + _password));
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", value);
                }
                foreach (var kv in headers)
                {
                    byteContent.Headers.Add(kv.Key, kv.Value.ToString());
                }

                HttpResponseMessage responseMsg;
                if ("Get".Equals(method, StringComparison.OrdinalIgnoreCase))
                {
                    responseMsg = await httpClient.GetAsync(url);
                }
                else if ("Post".Equals(method, StringComparison.OrdinalIgnoreCase))
                {
                    responseMsg = await httpClient.PostAsync(url, byteContent);
                }
                else if ("PUT".Equals(method, StringComparison.OrdinalIgnoreCase))
                {
                    responseMsg = await httpClient.PutAsync(url, byteContent);
                }
                else if ("Delete".Equals(method, StringComparison.OrdinalIgnoreCase))
                {
                    responseMsg = await httpClient.DeleteAsync(url);
                }
                else
                {
                    throw new Exception("未知method：" + method);
                }

                _tmpInfos = new Dictionary<string, object>();
                foreach (var header in responseMsg.Headers)
                {
                    if (header.Key.Length > 7 && header.Key.Substring(0, 7) == "x-upyun")
                    {
                        _tmpInfos.Add(header.Key, header.Value);
                    }
                }

                return responseMsg;
            }
        }

        /**
        * 获取总体空间的占用信息
        * return 空间占用量，失败返回 null
        */

        public async Task<long> GetFolderUsageAsync(string url)
        {
            var headers = new Dictionary<string, object>();
            long size;
            using (var resp = await NewWorker("GET", Dl + _bucketName + url + "?usage", null, headers))
            {
                try
                {
                    var strHtml = await resp.Content.ReadAsStringAsync();
                    size = long.Parse(strHtml);
                }
                catch (Exception)
                {
                    size = 0;
                }
            }
            return size;
        }

        /**
           * 获取某个子目录的占用信息
           * @param $path 目标路径
           * return 空间占用量，失败返回 null
           */
        public async Task<long> GetBucketUsageAsync()
        {
            return await GetFolderUsageAsync("/");
        }
        /**
        * 创建目录
        * @param $path 目录路径
        * return true or false
        */
        public async Task<bool> MkDirAsync(string path, bool autoMkdir)
        {
            _autoMkdir = autoMkdir;
            var headers = new Dictionary<string, object>
            {
                { "folder", "create" }
            };

            using (var resp = await NewWorker("POST", Dl + _bucketName + path, null, headers))
            {
                return resp.StatusCode == System.Net.HttpStatusCode.OK;
            }
        }

        /**
        * 删除目录
        * @param $path 目录路径
        * return true or false
        */
        public async Task<bool> RmDirAsync(string path)
        {
            var headers = new Dictionary<string, object>();
            return await DeleteAsync(path, headers);
        }

        /**
        * 读取目录列表
        * @param $path 目录路径
        * return array 数组 或 null
        */
        public async Task<List<FolderItem>> ReadDirAsync(string url)
        {
            var headers = new Dictionary<string, object>();
            using (var resp = await NewWorker("GET", Dl + _bucketName + url, null, headers))
            {
                var strHtml = await resp.Content.ReadAsStringAsync();
                strHtml = strHtml.Replace("\t", "\\");
                strHtml = strHtml.Replace("\n", "\\");
                var ss = strHtml.Split('\\');
                var i = 0;
                var list = new List<FolderItem>();
                while (i < ss.Length)
                {
                    var fi = new FolderItem(ss[i], ss[i + 1], int.Parse(ss[i + 2]), int.Parse(ss[i + 3]));
                    list.Add(fi);
                    i += 4;
                }
                return list;
            }
        }


        /**
        * 上传文件
        * @param $file 文件路径（包含文件名）
        * @param $data 文件内容 或 文件IO数据流
        * return true or false
        */
        public async Task<bool> WriteFileAsync(string path, byte[] data, bool autoMkdir)
        {
            var headers = new Dictionary<string, object>();
            _autoMkdir = autoMkdir;
            using (var resp = await NewWorker("POST", Dl + _bucketName + path, data, headers))
            {
                return resp.StatusCode == System.Net.HttpStatusCode.OK;
            }
        }
        /**
        * 删除文件
        * @param $file 文件路径（包含文件名）
        * return true or false
        */
        public async Task<bool> DeleteFileAsync(string path)
        {
            var headers = new Dictionary<string, object>();
            return await DeleteAsync(path, headers);
        }

        /**
        * 读取文件
        * @param $file 文件路径（包含文件名）
        * @param $output_file 可传递文件IO数据流（默认为 null，结果返回文件内容，如设置文件数据流，将返回 true or false）
        * return 文件内容 或 null
        */
        public async Task<byte[]> ReadFileAsync(string path)
        {
            var headers = new Dictionary<string, object>();

            using (var resp = await NewWorker("GET", Dl + _bucketName + path, null, headers))
            {
                return await resp.Content.ReadAsByteArrayAsync();
            }
        }
        /**
        * 设置待上传文件的 Content-MD5 值（如又拍云服务端收到的文件MD5值与用户设置的不一致，将回报 406 Not Acceptable 错误）
        * @param $str （文件 MD5 校验码）
        * return null;
        */
        public void SetContentMd5(string str)
        {
            _contentMd5 = str;
        }
        /**
        * 设置待上传文件的 访问密钥（注意：仅支持图片空！，设置密钥后，无法根据原文件URL直接访问，需带 URL 后面加上 （缩略图间隔标志符+密钥） 进行访问）
        * 如缩略图间隔标志符为 ! ，密钥为 bac，上传文件路径为 /folder/test.jpg ，那么该图片的对外访问地址为： http://空间域名/folder/test.jpg!bac
        * @param $str （文件 MD5 校验码）
        * return null;
        */
        public void SetFileSecret(string str)
        {
            _fileSecret = str;
        }
        /**
        * 获取文件信息
        * @param $file 文件路径（包含文件名）
        * return array('type'=> file | folder, 'size'=> file size, 'date'=> unix time) 或 null
        */
        public async Task<Dictionary<string, object>> GetFileInfoAsync(string file)
        {
            var headers = new Dictionary<string, object>();
            using (await NewWorker("HEAD", Dl + _bucketName + file, null, headers))
            {
                Dictionary<string, object> ht;
                try
                {
                    ht = new Dictionary<string, object>
                    {
                        {"type", _tmpInfos["x-upyun-file-type"]},
                        {"size", _tmpInfos["x-upyun-file-size"]},
                        {"date", _tmpInfos["x-upyun-file-date"]}
                    };
                }
                catch (Exception)
                {
                    ht = new Dictionary<string, object>();
                }
                return ht;
            }
        }
        //获取上传后的图片信息（仅图片空间有返回数据）
        public object GetUploadedFileInfo(string key)
        {
            return _tmpInfos == new Dictionary<string, object>() ? "" : _tmpInfos[key];
        }

        //计算文件的MD5码
        public static string GetFileMd5(string pathName)
        {
            using (var md5 = MD5.Create())
            {
                var oFileStream = new FileStream(pathName, FileMode.Open,
                    FileAccess.Read, FileShare.ReadWrite);

                var fileHashValue = md5.ComputeHash(oFileStream);
                //由以连字符分隔的十六进制对构成的String，其中每一对表示value 中对应的元素；例如“F-2C-4A”
                var strHashData = BitConverter.ToString(fileHashValue);
                //替换-
                strHashData = strHashData.Replace("-", "");
                var strResult = strHashData;
                return strResult.ToLower();
            }
        }
    }

    public class FolderItem
    {
        public string FileName;
        public string FileType;
        public int Size;
        public int Number;
        public FolderItem(string filename, string fileType, int size, int number)
        {
            FileName = filename;
            FileType = fileType;
            Size = size;
            Number = number;
        }
    }

}
