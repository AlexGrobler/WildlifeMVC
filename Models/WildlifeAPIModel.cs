using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WildlifeMVC.Models
{
    public class WildlifeAPIModel
    {
        public int ID { get; set; }
        public int SpeciesID { get; set; }
        public decimal XCoordinate { get; set; }
        public decimal YCoordinate { get; set; }
        public System.DateTime TimeStamp { get; set; }
        public string Description { get; set; }
        public string Locaton { get; set; }
        public string County { get; set; }
    }
}