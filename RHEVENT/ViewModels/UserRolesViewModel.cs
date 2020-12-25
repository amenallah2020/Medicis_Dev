using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RHEVENT.ViewModels
{
    public class UserRolesViewModel
    {
        IEnumerable<string> roles { get; set;}
        string Id { get; set; }

        public UserRolesViewModel(IEnumerable<string> roles,string Id)
        {
            this.roles = roles;
            this.Id = Id;
        }




    }

    
}