using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Vroemmm.Models
{
    public class TravelCostsModel
    {
        public IEnumerable<RouteModel> Routes { get; set; }
        
        [Display(Name = "Totaal aantal kilometers")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal TotalDistance { get; set; }

        [Display(Name = "Prijs per kilometer")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal CostsPerKilometer { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Totaal te declareren")]
        public decimal TotalCosts { get; set; }
    }
}
