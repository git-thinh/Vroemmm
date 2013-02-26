using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Vroemmm.Models
{
    public class RouteModel
    {
        [Display(Name = "Van (vertrek)")]
        public string Origin { get; set; }
        [Display(Name = "Naar (bestemming)")]
        public string Destination { get; set; }

        /// <summary>
        /// Distance in KM
        /// </summary>
        [Display(Name = "Aantal km.")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal Distance { get; set; }
        [Display(Name = "Datum")]
        [DisplayFormat(DataFormatString = "{0: dd-MMM-yy}")]
        public DateTime Date { get; set; }

        public EventModel OriginEvent { get; set; }

        public EventModel DestinationEvent { get; set; }

        [Display(Name = "Retour")]
        public string Return { get { return "nee"; } }
    }
}
