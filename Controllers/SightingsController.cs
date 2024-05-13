using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WildlifeMVC.Models;
using WildlifeMVC.Services;
using WildlifeMVC.ViewModels;

namespace WildlifeMVC.Controllers
{
    public class SightingsController : Controller
    {
        //list of Irish counties needed for our forms
       public static List<string> CountiesList = new List<string>
        {   "Antrim",
    "Armagh",
    "Carlow",
    "Cavan",
    "Clare",
    "Cork",
    "Derry",
    "Donegal",
    "Down",
    "Dublin",
    "Fermanagh",
    "Galway",
    "Kerry",
    "Kildare",
    "Kilkenny",
    "Laois",
    "Leitrim",
    "Limerick",
    "Longford",
    "Louth",
    "Mayo",
    "Meath",
    "Monaghan",
    "Offaly",
    "Roscommon",
    "Sligo",
    "Tipperary",
    "Tyrone",
    "Waterford",
    "Westmeath",
    "Wexford",
    "Wicklow"};

        private readonly ISightingService sightingsService;

        //uses Ninject dependency injection to insantiate the service layer
        public SightingsController(ISightingService sightings)
        {
            sightingsService = sightings;
        }

        //async tasks are better for performance, as other tasks can run while waiting for a response from the API
        [HttpGet]
        [Route("Sightings/SightingsList")]
        public async Task<ActionResult> SightingsIndex()
        {
            return View(await sightingsService.GetAllSightings()); //we get our data from the service layer, allowing for resuable code
        }

        [HttpGet]
        [Route("Sightings/SightingInfo/{id}")]
        public async Task<ActionResult> SightingDetails(int? id)
        {
            return View(await sightingsService.GetSightingDetailsData(id));
        }

        [HttpGet]
        [Route("Sightings/RecordSighting")]
        public async Task<ActionResult> CreateSighting()
        {
            using (var db = new wildlife_DBEntities())
            {
                ViewBag.Counties = new SelectList(CountiesList); //use a viewbag to hold our list of valid Irish counties for the form
                SightingViewModel viewModel = await sightingsService.GetSpeciesDropDownData();
                return View(viewModel);
            }
        }

        [HttpPost]
        [Route("Sightings/RecordSighting")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateSighting(SightingViewModel sightingViewModel)
        {

            if (!ModelState.IsValid)
            {
                throw new HttpException(400, "Form data was invalid");
            }

            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage response = await sightingsService.CreateSighting(sightingViewModel);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("SightingsIndex");
                }
                else
                {
                    throw new HttpException(400, "Bad Request");
                }
            }
        }

        [HttpGet]
        [Route("Sightings/UpdateSighting/{id}")]
        public async Task<ActionResult> UpdateSighting(int? id)
        {
            ViewBag.Counties = new SelectList(CountiesList);
            return View(await sightingsService.GetSightingViewModelData(id));
        }

        [HttpPost]
        [Route("Sightings/UpdateSighting/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateSighting([Bind(Include = "ID,SpeciesID,XCoordinate,YCoordinate,TimeStamp,Description,Location,County")] SightingViewModel sightingViewModel)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpException(400, "Bad Request");
            }

            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage response = await sightingsService.UpdateSighting(sightingViewModel);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("SightingsIndex");
                }
                else
                {
                    throw new HttpException(400, "Bad Request");
                }
            }
        }

        [HttpGet]
        [Route("Sightings/RemoveSighting/{id}")]
        public async Task<ActionResult> DeleteSighting(int? id)
        {
            return View(await sightingsService.GetSightingDetailsData(id));
        }

        [HttpPost, ActionName("DeleteSighting")]
        [Route("Sightings/RemoveSighting/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteSighting(int id) 
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage response = await sightingsService.DeleteSightingAsync($"DeleteSighting/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("SightingsIndex");
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
        }
    }
}