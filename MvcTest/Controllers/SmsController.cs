using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HZC.Utils;
using HZC.Utils.YunPian;
using Microsoft.AspNetCore.Mvc;

namespace MvcTest.Controllers
{
    public class SmsController : Controller
    {
        private readonly YunPianService _service;

        public SmsController(YunPianService service)
        {
            _service = service;
        }

        public IActionResult Send(string mobile, string code)
        {
            var result = _service.SendVCode("13914258044", "1234");
            return Content(result.Result.Msg);
        }

        public IActionResult VCode()
        {
            var stream = new VerificationCodeUtil().Create(out var code);
            return File(stream.ToArray(), "image/png");
        }

        public IActionResult Test()
        {
            return View();
        }
    }
}