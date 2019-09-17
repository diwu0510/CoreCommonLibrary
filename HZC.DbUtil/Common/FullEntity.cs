using HZC.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZC.DbUtil
{
    public class FullEntity : BaseEntity, ICreationAudited, IModificationAudited, ISoftDelete
    {
        [DataColumn(UpdateIgnore = true)]
        public bool IsDel { get; set; } = false;

        [DataColumn(UpdateIgnore = true)]
        [JsonConverter(typeof(DateTimeFormatConverter))]
        public DateTime CreateAt { get; set; } = DateTime.Now;

        [DataColumn(UpdateIgnore = true)]
        public string Creator { get; set; }

        [JsonConverter(typeof(DateTimeFormatConverter))]
        public DateTime UpdateAt { get; set; } = DateTime.Now;

        public string Updator { get; set; }

        public void BeforeCreate(string userName)
        {
            CreateAt = DateTime.Now;
            Creator = userName;
            UpdateAt = DateTime.Now;
            Updator = userName;
        }

        public void BeforeUpdate(string userName)
        {
            UpdateAt = DateTime.Now;
            Updator = userName;
        }
    }
}
