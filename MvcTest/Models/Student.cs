using System;
using HZC.Framework.Datas;
using HZC.MyDapper;

namespace MvcTest.Models
{
    [DataTable("Students")]
    public class Student : IEntity, ICreateAudit<int>, IUpdateAudit<int>, IDeleteAudit<int>, ISoftDelete
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [DataColumn(UpdateIgnore = true)]
        public bool IsDel { get; set; }

        [DataColumn(UpdateIgnore = true)]
        public int CreateBy { get; set; }

        [DataColumn(UpdateIgnore = true)]
        public DateTime CreateAt { get; set; }

        public int UpdateBy { get; set; }

        public DateTime UpdateAt { get; set; }

        [DataColumn(UpdateIgnore = true)]
        public int DeleteBy { get; set; }

        [DataColumn(UpdateIgnore = true)]
        public DateTime? DeleteAt { get; set; }
    }
}
