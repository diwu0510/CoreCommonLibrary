using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HZC.Framework;
using HZC.MyDapper.Conditions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MvcTest.Models;
using MvcTest.Services;

namespace MvcTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly MyDapperContext _dbContext;

        public StudentsController(MyDapperContext context)
        {
            _dbContext = context;
        }

        public IEnumerable<Student> Index()
        {
            var data = _dbContext.Fetch<Student>(
                ConditionBuilder.New(),
                SortBuilder.New()
            );

            return data;
        }

        [Route("Create")]
        public Student Create(string name)
        {
            var student = new Student
            {
                Name = name
            };

            var result = _dbContext.Insert(student);
            return student;
        }

        [Route("Delete")]
        public string Delete(int id)
        {
            var result = _dbContext.Delete<Student>(id);
            return result > 0 ? "OK" : "Fail";
        }

        [Route("ChangeName")]
        public string ChangeName(int id, string name)
        {
            var result = _dbContext.SetByCondition<Student>(
                ConditionBuilder.New().AndEqual("Id", id),
                SoKeyValuePairs.New().Add("Name", name));
            return result > 0 ? "OK" : "Fail";
        }
    }
}