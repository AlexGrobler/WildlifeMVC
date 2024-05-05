using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WildlifeMVC.Models;

namespace WildlifeMVC.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<Species> SpeciesList { get; set; }
        public IEnumerable<SightingDetailsViewModel> SightingsList { get; set; }
    }
}