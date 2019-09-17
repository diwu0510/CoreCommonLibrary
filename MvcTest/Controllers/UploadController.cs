using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HZC.Framework;
using HZC.Utils.UEditor;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MvcTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IHostingEnvironment _hosting;

        public UploadController(IHostingEnvironment hosting)
        {
            _hosting = hosting;
        }

        [HttpPost("Image")]
        public async Task<IActionResult> Image()
        {
            var files = Request.Form.Files;
            IFormFile file;
            if(files == null || files.Count == 0)
            {
                return new JsonResult(new { Code = 0, Message = "没有图片" });
            }

            file = files[0];

            UploadResult result = new UploadResult();

            result.OriginFileName = Path.GetFileName(file.FileName);
            string fileExt = Path.GetExtension(result.OriginFileName);

            var basePath = Path.Combine(_hosting.WebRootPath, "Upload", "Images");
            if(!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }

            var newFileName = Guid.NewGuid().ToString("N");
            var filePath = Path.Combine(basePath, $"{newFileName}{fileExt}");

            using(var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return new JsonResult(new { Code = 200, Message = newFileName });
        }
    }
}