using HZC.Utils.UpYun;
using System;
using System.IO;
using System.Linq;
using System.Net;

namespace HZC.Utils.UEditor
{
    public class Crawler
    {
        public string SourceUrl { get; set; }
        public string ServerUrl { get; set; }
        public string State { get; set; }

        private readonly UEditorConfig _config;
        private readonly string _webRootPath;
        private readonly UpYunService _client;


        public Crawler(string sourceUrl, string webRootPath, UEditorConfig config, UpYunService client = null)
        {
            SourceUrl = sourceUrl;
            _config = config;
            _webRootPath = webRootPath;
            _client = client;
        }

        public Crawler Fetch()
        {
            if (!IsExternalIpAddress(SourceUrl))
            {
                State = "INVALID_URL";
                return this;
            }
            var request = WebRequest.Create(SourceUrl) as HttpWebRequest;
            // ReSharper disable once PossibleNullReferenceException
            using (var response = request.GetResponse() as HttpWebResponse)
            {
                // ReSharper disable once PossibleNullReferenceException
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    State = "Url returns " + response.StatusCode + ", " + response.StatusDescription;
                    return this;
                }
                if (response.ContentType.IndexOf("image", StringComparison.Ordinal) == -1)
                {
                    State = "Url is not an image";
                    return this;
                }

                try
                {
                    var stream = response.GetResponseStream();
                    var reader = new BinaryReader(stream);
                    byte[] bytes;
                    using (var ms = new MemoryStream())
                    {
                        var buffer = new byte[4096];
                        int count;
                        while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            ms.Write(buffer, 0, count);
                        }
                        bytes = ms.ToArray();
                    }

                    ServerUrl = PathFormatter.Format(Path.GetFileName(SourceUrl), _config.catcherPathFormat);
                    if (_client == null)
                    {
                        var savePath = Path.Combine(_webRootPath, ServerUrl);
                        if (!Directory.Exists(Path.GetDirectoryName(savePath)))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                        }
                        
                        File.WriteAllBytes(savePath, bytes);
                        State = "SUCCESS";
                    }
                    else
                    {
                        var result = _client.WriteFileAsync("/" + ServerUrl, bytes, true).Result;
                        if(result)
                        {
                            State = "SUCCESS";
                        }
                        else
                        {
                            State = "上传失败";
                        }
                    }
                }
                catch (Exception e)
                {
                    State = "抓取错误：" + e.Message;
                }
                return this;
            }
        }

        private string GetFileExtension(string fileName)
        {
            var temp = fileName.Split(".").Last();
            if(string.IsNullOrWhiteSpace(temp))
            {
                return ".jpg";
            }
            else
            {
                temp = temp.ToLower();
                if(fileName.Contains("jpg"))
                {
                    return ".jpg";
                }
                else if(fileName.Contains("png"))
                {
                    return ".png";
                }
                else if(fileName.Contains("gif"))
                {
                    return ".gif";
                }
                else
                {
                    return ".jpg";
                }
            }
        }

        private bool IsExternalIpAddress(string url)
        {
            var uri = new Uri(url);
            if (uri.HostNameType == UriHostNameType.Dns)
            {
                var ipHostEntry = Dns.GetHostEntry(uri.DnsSafeHost);
                foreach (var ipAddress in ipHostEntry.AddressList)
                {
                    if (ipAddress.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork) continue;

                    if (!IsPrivateIP(ipAddress))
                    {
                        return true;
                    }
                }
            }
            else if (uri.HostNameType == UriHostNameType.IPv4)
            {
                return !IsPrivateIP(IPAddress.Parse(uri.DnsSafeHost));
            }

            return false;
        }

        private bool IsPrivateIP(IPAddress myIpAddress)
        {
            if (IPAddress.IsLoopback(myIpAddress)) return true;
            if (myIpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                var ipBytes = myIpAddress.GetAddressBytes();

                // 10.0.0.0/24 
                if (ipBytes[0] == 10)
                {
                    return true;
                }

                // 172.16.0.0/16
                if (ipBytes[0] == 172 && ipBytes[1] == 16)
                {
                    return true;
                }

                // 192.168.0.0/16
                if (ipBytes[0] == 192 && ipBytes[1] == 168)
                {
                    return true;
                }

                // 169.254.0.0/16
                if (ipBytes[0] == 169 && ipBytes[1] == 254)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
