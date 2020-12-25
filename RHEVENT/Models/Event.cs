using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RHEVENT.Models
{
    public class Event
    {   
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public string Color { get; set; }
        public string TextColor { get; set; }
        public string Current_User { get; set; }
        
    }
}