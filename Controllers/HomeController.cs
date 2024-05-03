using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WildlifeMVC.Models;
using WildlifeMVC.Services;

namespace WildlifeMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISpeciesService speciesService;

        public HomeController(ISpeciesService speciesService)
        {
            this.speciesService = speciesService;
        }

        public async Task<ActionResult> HomePage()
        {
            List<Species> speciesList = await speciesService.GetAllSpecies();
            return View(speciesList);
        }
    }
}