using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace WildlifeMVC.ViewModels
{
    public class SightingDetailsViewModel
    {
        public int ID { get; set; }
        [Display(Name = "Species Name")]
        public int SpeciesID { get; set; }
        public string SpeciesName { get; set; }
        public decimal XCoordinate { get; set; }
        public decimal YCoordinate { get; set; }
        [Display(Name = "Time Of Sighting")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        public System.DateTime TimeStamp { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string County { get; set; }
    }
}