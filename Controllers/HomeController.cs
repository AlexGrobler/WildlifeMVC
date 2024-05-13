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
    //homw view requires data from both the API and main DB, so service layers and a view model are required
    public class HomeController : Controller
    {
        private readonly ISpeciesService speciesService;
        private readonly ISightingService sightingsService;

        //uses Ninject dependency injection to insantiate the service layers
        public HomeController(ISpeciesService species, ISightingService sightings)
        {
            speciesService = species;
            sightingsService = sightings;
        }

        public async Task<ActionResult> HomePage()
        {
            //insantiate and return the home viewmodel with all the data we need
            HomeViewModel homeView = new HomeViewModel()
            {
                SpeciesList = await speciesService.GetAllSpecies(),
                SightingsList = await sightingsService.GetAllSightings()
            };
            return View(homeView);
        }
    }
}