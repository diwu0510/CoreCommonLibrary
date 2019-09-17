using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using MvcTest.Models;

namespace MvcTest.Services
{
    public class StudentService
    {
        public int Create(Student student)
        {
            var sql =
                "IF NOT EXISTS (SELECT * FROM Student WITH (HOLDLOCK) WHERE Name=@name) INSERT INTO Student (Name) VALUES (@name);SELECT @@IDENTITY;";
            using (var conn = new SqlConnection("Data Source=.;Database=MvcTest;User Id=sa;Password=790825"))
            {
                var idStr = conn.ExecuteScalar<int>(sql, new {name = student.Name});
                return (int) idStr;
            }
        }
    }
}
