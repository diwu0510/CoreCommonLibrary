using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MvcTest.Models;
using MvcTest.Services;

namespace MvcTest.Controllers
{
    public class DbTestController : Controller
    {
        public IActionResult Create()
        {
            var service = new StudentService();

            Parallel.For(0, 200, (idx, state) =>
            {
                for (var i = 0; i < 100; i++)
                {
                    var student = new Student {Name = $"张三_{i}"};
                    var id = service.Create(student);
                    if (id > 0)
                    {
                        Console.WriteLine($"线程【{idx}】创建学生【张三_{idx}_{i}】成功");
                    }
                }
            });
            return Content("完成");
        }
    }
}