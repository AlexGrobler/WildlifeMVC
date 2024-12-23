﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace WildlifeMVC.ViewModels
{
    public class SightingViewModel
    {
        public int ID { get; set; }
        [Display(Name = "Species")]
        public int SpeciesID { get; set; }
        public IEnumerable<SelectListItem> SpeciesList { get; set; }
        public decimal XCoordinate { get; set; }
        public decimal YCoordinate { get; set; }
        [Display(Name = "Date Of Sighting")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public System.DateTime TimeStamp { get; set; } = DateTime.Today;
        public string Description { get; set; }
        public string Location { get; set; }
        public string County { get; set; }
    }
}