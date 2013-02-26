using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Vroemmm.Models
{
    public class CalendarModel
    {
        [Display(Name = "Kalendernaam")]
        public string Name { get; set; }
        public string Id { get; set; }
    }
}
