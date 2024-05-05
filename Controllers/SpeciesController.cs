using Ninject.Planning.Targets;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WildlifeMVC.Models;
using WildlifeMVC.Services;
using WildlifeMVC.ViewModels;
using static System.Net.WebRequestMethods;

namespace WildlifeMVC.Controllers
{
    public class SpeciesController : Controller
    {
        private readonly ISpeciesService speciesService;

        public SpeciesController(ISpeciesService species)
        {
            speciesService = species;
        }

        public static async Task<bool> IsVideoAvailable(string videoUrl)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    //SPLIT STRING OF URL AT v=
                    string googleAPI = "https://www.youtube.com/oembed?format=json&url=" + videoUrl;
                    var response = await httpClient.GetAsync(videoUrl);
                    return response.IsSuccessStatusCode;
                }
                catch
                {
                    return false;
                }
            }
        }

        [HttpGet]
        [Route("Species")]
        public async Task<ActionResult> Index()
        {
            List<Species> speciesList = await speciesService.GetAllSpecies();
            return View(speciesList);
        }

        [HttpGet]
        [Route("Species/Info/{name}/{id}")]
        public async Task<ActionResult> Details(string name, int? id)
        {
            if (name == null || id == null)
            {
                throw new HttpException(400, "Bad Request");
            }
            Species species = await speciesService.GetSpeciesByIdAsync((int)id);
            if (species == null)
            {
                throw new HttpException(404, "Resource Not Found");
            }

            bool validEmbed = await IsVideoAvailable(species.VideoURL);
            if (!validEmbed)
            {
                ViewBag.FallbackContent = "Video not available :(";
            }
            return View(species);
        }

        [Route("Species/AddSpecies")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Route("Species/AddSpecies")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,EnglishName,LatinName,ShortDescription,LongDescription,ImageURL,VideoURL")] Species species)
        {
            using (var db = new wildlife_DBEntities())
            {
                if (ModelState.IsValid)
                {
                    await speciesService.AddSpeciesAsync(species); 
                    return RedirectToAction("Index");
                }
                return View(species);
            }
        }

        [HttpGet]
        [Route("Species/EditSpecies/{name}/{id}")]
        public async Task<ActionResult> Edit(string name, int? id)
        {
            using (var db = new wildlife_DBEntities())
            {
                name = name.Replace("-", " ");
                if (id == null || name == null)
                {
                    throw new HttpException(400, "Bad Request");
                }
                Species species = await speciesService.GetSpeciesByIdAsync((int)id);
                if (species == null)
                {
                    throw new HttpException(404, "Resource Not Found");
                }
                return View(species);
            }
        }

        [HttpPost]
        [Route("Species/EditSpecies/{name}/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,EnglishName,LatinName,ShortDescription,LongDescription,ImageURL,VideoURL")] Species species)
        {
            using (var db = new wildlife_DBEntities())
            {
                if (ModelState.IsValid)
                {
                    await speciesService.UpdateSpeciesAsync(species);
                    return RedirectToAction("Index");
                }
                return View(species);
            }
        }

        [HttpGet]
        [Route("Species/DeleteSpecies/{name}/{id}")]
        public async Task<ActionResult> Delete(string name, int? id)
        {
            using (var db = new wildlife_DBEntities())
            {
                if (id == null || name == null)
                {
                    throw new HttpException(400, "Bad Request");
                }
                Species species = await speciesService.GetSpeciesByIdAsync((int)id);
                if (species == null)
                {
                    throw new HttpException(404, "Resource Not Found");
                }
                return View(species);
            }
        }

        [HttpPost, ActionName("Delete")]
        [Route("Species/DeleteSpecies/{name}/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            using (var db = new wildlife_DBEntities())
            {
                await speciesService.DeleteSpeciesAsync(id);
                return RedirectToAction("Index");
            }
        }

        protected override void Dispose(bool disposing)
        {
            using (var db = new wildlife_DBEntities())
            {
                if (disposing)
                {
                    db.Dispose();
                }
                base.Dispose(disposing);
            }
        }
    }
}
