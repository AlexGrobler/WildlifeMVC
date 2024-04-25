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

        private async Task<HttpResponseMessage> deleteHttpResponse(string target)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("http://localhost:64341/api/");
                return await httpClient.DeleteAsync($"WildlifeSightings/{target}");
            }
        }

        private async Task<HttpResponseMessage> postHttpResponse(string target, StringContent data)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("http://localhost:64341/api/");
                return await httpClient.PostAsync($"WildlifeSightings/{target}", data);
            }
        }

        private async Task<HttpResponseMessage> getHttpResponse(string target)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("http://localhost:64341/api/");
                return await httpClient.GetAsync($"WildlifeSightings/{target}");
            }
        }

        [HttpGet]
        public async Task<SightingAPIModel> GetSightingByID(int id)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage response = await getHttpResponse($"BySighting/{id}");
                SightingAPIModel apiData = await response.Content.ReadAsAsync<SightingAPIModel>();
                if (response.IsSuccessStatusCode)
                {
                    return apiData;
                }
                else
                {
                    throw new HttpException(404, "No sighting found with that ID");
                }
            }
        }


        [HttpGet]
        public async Task<ActionResult> SightingsIndex()
        {
        
            using (HttpClient httpClient = new HttpClient())
            {
                IEnumerable<SightingAPIModel> apiData = new List<SightingAPIModel>();
                HttpResponseMessage response = await getHttpResponse("All");
                apiData = await response.Content.ReadAsAsync<IEnumerable<SightingAPIModel>>();
                if (response.IsSuccessStatusCode)
                {
                    return View(apiData);
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
        }

        public async Task<ActionResult> SightingDetails(int? id)
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

        public ActionResult CreateSighting()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateSighting(SightingAPIModel sighting)
        {
            //should instead return create view, and have the form display warnrings about invalid inputs
            if (!ModelState.IsValid)
            {
                throw new HttpException(400, "Form data was invalid");
            }

            using (HttpClient httpClient = new HttpClient())
            {
                StringContent data = new StringContent(JsonConvert.SerializeObject(sighting), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await postHttpResponse("CreateSighting", data);
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
            //should instead return create view, and have the form display warnrings about invalid inputs
            if (!ModelState.IsValid)
            {
                throw new HttpException(400, "Update data was invalid");
            }

            using (HttpClient httpClient = new HttpClient())
            {
                StringContent data = new StringContent(JsonConvert.SerializeObject(sighting), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await postHttpResponse("UpdateSighting", data);
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

        public async Task<ActionResult> DeleteSighting(int? id)
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

        [HttpPost, ActionName("DeleteSighting")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteSighting(int id) 
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage response = await deleteHttpResponse($"DeleteSighting/{id}");
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