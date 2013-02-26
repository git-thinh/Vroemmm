using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Vroemmm.Models
{
    public class EventModel
    {
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy HH:mm}")]
        public DateTime Start { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy HH:mm}")]
        public DateTime End { get; set; }

        public string Location { get; set; }

        public string Summary { get; set; }
    }
}
