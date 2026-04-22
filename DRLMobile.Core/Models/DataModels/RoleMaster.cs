using Newtonsoft.Json;
using SQLite;
using System;

namespace DRLMobile.Core.Models.DataModels
{
    public class RoleMaster
    {
        [JsonProperty("roleid")]
        public int RoleID { get; set; }

        [JsonProperty("rolename")]
        public string RoleName { get; set; }

        [JsonProperty("updateddate")]
        public string UpdateDate { get; set; }
    }
}