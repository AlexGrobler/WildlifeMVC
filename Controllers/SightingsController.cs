using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WildlifeMVC.Models;
using WildlifeMVC.ViewModel;

namespace WildlifeMVC.Controllers
{
    public class SightingsController : Controller
    {
        [HttpGet]
        public async Task<ActionResult> SightingsIndex()
        {
            IEnumerable <SightingAPIModel> apiData = new List<SightingAPIModel>();
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    httpClient.BaseAddress = new Uri("http://localhost:64341/api/");
                    HttpResponseMessage response = await httpClient.GetAsync("WildlifeSightings/All");
                    response.EnsureSuccessStatusCode(); //only proceeds if response is successful 
                    apiData = await response.Content.ReadAsAsync<IEnumerable<SightingAPIModel>>();
                }
                catch (Exception ex)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, ex.Message);
                }
            }

            return View(apiData);
        }

        public ActionResult CreateSighting()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateSighting(SightingAPIModel sighting)
        {
            try
            {
                //should instead return create view, and have the form display warnrings about invalid inputs
                if (!ModelState.IsValid)
                {
                    throw new HttpException(400, "Form data was invalid");
                }

                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri("http://localhost:64341/api/");
                    StringContent data = new StringContent(JsonConvert.SerializeObject(sighting), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await httpClient.PostAsync("WildlifeSightings/CreateSighting", data);
                    response.EnsureSuccessStatusCode();
                    return RedirectToAction("SightingsIndex");
                }
            }
            catch (Exception ex) 
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        public async Task<SightingAPIModel> GetSightingByID(int id)
        {
            SightingAPIModel apiData;
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    httpClient.BaseAddress = new Uri("http://localhost:64341/api/");
                    HttpResponseMessage response = await httpClient.GetAsync($"WildlifeSightings/BySighting/{id}");
                    response.EnsureSuccessStatusCode(); //only proceeds if response is successful 
                    apiData = await response.Content.ReadAsAsync<SightingAPIModel>();
                }
                catch
                {
                    throw new HttpException(404, "No sighting found with that ID");
                }
            }

            return apiData;
        }

        public async Task<ActionResult> UpdateSighting(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SightingAPIModel sighting = await GetSightingByID((int)id);
            if (sighting == null)
            {
                return HttpNotFound();
            }
            return View(sighting);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateSighting([Bind(Include = "ID,SpeciesID,XCoordinate,YCoordinate,TimeStamp,Description,Locaton,County")] SightingAPIModel sighting)
        {
            try
            {
                //should instead return create view, and have the form display warnrings about invalid inputs
                if (!ModelState.IsValid)
                {
                    throw new HttpException(400, "Update data was invalid");
                }

                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri("http://localhost:64341/api/");
                    StringContent data = new StringContent(JsonConvert.SerializeObject(sighting), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await httpClient.PostAsync("WildlifeSightings/UpdateSighting", data);
                    response.EnsureSuccessStatusCode();
                    return RedirectToAction("SightingsIndex");
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}