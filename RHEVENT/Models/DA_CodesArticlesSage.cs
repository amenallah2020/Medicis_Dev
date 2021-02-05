using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace RHEVENT.Models
{
    public class DA_CodesArticlesSage
    {
        [key]
        public int Id { get; set; }

        [Display(Name = "Code Sage")]
        [Required]
        public string Code { get; set; }
    }
}