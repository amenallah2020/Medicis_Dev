using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RHEVENT.Models
{
    public class RoleUsersViewModel
    {
        public string RoleId { get; set; }

        public string RoleName { get; set; }

        public bool IsSelected { get; set; }
    }
}