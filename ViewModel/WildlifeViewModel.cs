using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace WildlifeMVC.ViewModel
{
    public class WildlifeViewModel
    {
        public int ID { get; set; }
        public int SpeciesID { get; set; }
        public decimal XCoordinate { get; set; }
        public decimal YCoordinate { get; set; }
        public System.DateTime TimeStamp { get; set; }
        public string Description { get; set; }
        public string Locaton { get; set; }
        public string County { get; set; }
        [Display(Name = "English Name")]
        public string EnglishName { get; set; }
        [Display(Name = "Latin Name")]
        public string LatinName { get; set; }
        [Display(Name = "Short Description")]
        public string ShortDescription { get; set; }
        [Display(Name = "Long Description")]
        public string LongDescription { get; set; }
        public string ImageURL { get; set; }
        public string VideoURL { get; set; }
    }
}