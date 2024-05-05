using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WildlifeMVC.Models;
using WildlifeMVC.Services;
using WildlifeMVC.ViewModels;

namespace WildlifeMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISpeciesService speciesService;
        private readonly ISightingService sightingsService;

        public HomeController(ISpeciesService species, ISightingService sightings)
        {
            speciesService = species;
            sightingsService = sightings;
        }

        public async Task<ActionResult> HomePage()
        {
            HomeViewModel homeView = new HomeViewModel()
            {
                SpeciesList = await speciesService.GetAllSpecies(),
                SightingsList = await sightingsService.GetAllSightings()
            };
            return View(homeView);
        }
    }
}