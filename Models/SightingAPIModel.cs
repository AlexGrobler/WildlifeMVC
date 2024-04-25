using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace WildlifeMVC.Models
{
    public class SightingAPIModel
    {
        public int ID { get; set; }
        public int SpeciesID { get; set; }
        public decimal XCoordinate { get; set; }
        public decimal YCoordinate { get; set; }
        [Display(Name = "Time Of Sighting")]
        public System.DateTime TimeStamp { get; set; }
        public string Description { get; set; }
        public string Locaton { get; set; }
        public string County { get; set; }
    }
}