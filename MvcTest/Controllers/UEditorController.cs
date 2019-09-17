using HZC.Utils.UEditor;
using HZC.Utils.UpYun;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MvcTest.Controllers
{
    public class UEditorController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        private readonly UEditorConfig _config;

        private readonly UpYunService _upYun;

        public UEditorController(IHostingEnvironment environment, IOptions<UEditorConfig> option, UpYunService upYun)
        {
            _hostingEnvironment = environment;
            _config = option.Value;
            _upYun = upYun;
        }

        public IActionResult Config()
        {
            return Ok(_config);
        }

        #region 上传图片
        /// <summary>
        /// 上传图片
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> UploadImage1(string callback)
        {
            UploadResult result = new UploadResult();

            var files = Request.Form.Files;
            if (files.Count == 0 || files[0].Length == 0)
            {
                result.State = GetStateMessage(UploadState.Unknown);
                return WriteResult(callback, result);
            }

            var file = files[0];

            result.OriginFileName = Path.GetFileName(file.FileName);
            string fileExt = Path.GetExtension(result.OriginFileName);

            if (!CheckFileType(fileExt, _config.imageAllowFiles))
            {
                result.State = GetStateMessage(UploadState.TypeNotAllow);
                return WriteResult(callback, result);
            }

            if (!CheckFileSize(file.Length, _config.imageMaxSize))
            {
                result.State = GetStateMessage(UploadState.SizeLimitExceed);
                return WriteResult(callback, result);
            }

            string webRootPath = _hostingEnvironment.WebRootPath;
            string filePath = PathFormatter.Format(file.FileName, _config.imagePathFormat);
            string savePath = Path.Combine(webRootPath, filePath);

            string folderPath = Path.GetDirectoryName(savePath);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var thumb = new Bitmap(file.OpenReadStream());
            thumb.GetThumbnailImage(120, 120, () => false, IntPtr.Zero);
            using(var thumbStream = new FileStream(Path.Combine(webRootPath, "thumb", filePath), FileMode.Create))
            {
                thumb.Save(thumbStream, ImageFormat.Jpeg);
            }

            using (var stream = new FileStream(savePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);

                result.State = GetStateMessage(UploadState.Success);
                result.Url = filePath;

                return WriteResult(callback, result);
            }
        }
        #endregion

        #region 上传图片-又拍
        public async Task<IActionResult> UploadImage(string callback)
        {
            var result = new UploadResult();

            var files = Request.Form.Files;
            if (files.Count == 0 || files[0].Length == 0)
            {
                result.State = GetStateMessage(UploadState.Unknown);
                return WriteResult(callback, result);
            }

            var file = files[0];

            result.OriginFileName = Path.GetFileName(file.FileName);
            var fileExt = Path.GetExtension(result.OriginFileName);

            if (!CheckFileType(fileExt, _config.imageAllowFiles))
            {
                result.State = GetStateMessage(UploadState.TypeNotAllow);
                return WriteResult(callback, result);
            }

            if (!CheckFileSize(file.Length, _config.imageMaxSize))
            {
                result.State = GetStateMessage(UploadState.SizeLimitExceed);
                return WriteResult(callback, result);
            }

            var newFileName = Guid.NewGuid() + fileExt;
            var path = "/" + DateTime.Today.ToString("yyMM") + "/" + newFileName;

            using (var stream = file.OpenReadStream())
            {
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, (int)stream.Length);

                var success = await _upYun.WriteFileAsync(path, bytes, true);
                if (success)
                {
                    result.State = GetStateMessage(UploadState.Success);
                    result.Url = _upYun.Domain + path;
                    return WriteResult(callback, result);
                }
                else
                {
                    result.State = GetStateMessage(UploadState.Unknown);
                    result.Url = "";
                    return WriteResult(callback, result);
                }
            }
        }
        #endregion

        #region 上传涂鸦
        /// <summary>
        /// 上传图片
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> UploadScrawl(string callback)
        {
            var result = new UploadResult();

            var files = Request.Form.Files;
            if (files.Count == 0 || files[0].Length == 0)
            {
                result.State = GetStateMessage(UploadState.Unknown);
                result.ErrorMessage = "请选择要上传的图片";
                return WriteResult(callback, result);
            }

            var file = files[0];

            result.OriginFileName = Path.GetFileName(file.FileName);

            if (!CheckFileSize(file.Length, _config.imageMaxSize))
            {
                result.State = GetStateMessage(UploadState.SizeLimitExceed);
                return WriteResult(callback, result);
            }

            var webRootPath = _hostingEnvironment.WebRootPath;
            var filePath = PathFormatter.Format(file.FileName, _config.scrawlPathFormat);
            var savePath = Path.Combine(webRootPath, filePath);

            var folderPath = Path.GetDirectoryName(savePath);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            using (var stream = new FileStream(savePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);

                result.State = GetStateMessage(UploadState.Success);
                result.Url = filePath;
            }
            return WriteResult(callback, result);
        }
        #endregion

        #region 抓取远程图片
        /// <summary>
        /// 上传图片
        /// </summary>
        /// <returns></returns>
        public IActionResult CatchImage(string callback)
        {
            var result = new UploadResult();

            var sources = Request.Form["source[]"];
            if (string.IsNullOrWhiteSpace(sources))
            {
                result.State = "参数错误：没有指定抓取源";
                return WriteResult(callback, result);
            }
            else
            {
                var crawlers = sources.Select(x => new Crawler(x, _hostingEnvironment.WebRootPath, _config, _upYun).Fetch()).ToArray();
                return WriteResult(callback, new
                {
                    state = "SUCCESS",
                    list = crawlers.Select(x => new
                    {
                        state = x.State,
                        source = x.SourceUrl,
                        url = x.ServerUrl
                    })
                });
            }
        }
        #endregion

        public IActionResult WriteResult(string cbName, object result)
        {
            var setting = new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            };
            if (!string.IsNullOrWhiteSpace(cbName))
            {
                HttpContext.Response.ContentType = "application/javascript";
                return Content($"{cbName}({JsonConvert.SerializeObject(result, Formatting.None, setting)})");
            }
            else
            {
                HttpContext.Response.ContentType = "text/plain";
                return Content(JsonConvert.SerializeObject(result, Formatting.None, setting));
            }
        }

        private string GetStateMessage(UploadState state)
        {
            switch (state)
            {
                case UploadState.Success:
                    return "SUCCESS";
                case UploadState.FileAccessError:
                    return "文件访问出错，请检查写入权限";
                case UploadState.SizeLimitExceed:
                    return "文件大小超出服务器限制";
                case UploadState.TypeNotAllow:
                    return "不允许的文件格式";
                case UploadState.NetworkError:
                    return "网络错误";
            }
            return "未知错误";
        }

        private bool CheckFileType(string extName, string[] exts)
        {
            return _config.imageAllowFiles.Select(x => x.ToLower()).Contains(extName);
        }

        private bool CheckFileSize(long size, long limit)
        {
            return size < _config.imageMaxSize;
        }
    }
}