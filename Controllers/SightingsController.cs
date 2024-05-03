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
using WildlifeMVC.ViewModels;

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

        public async Task<HttpResponseMessage> GetHttpResponse(string target)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("http://localhost:64341/api/");
                return await httpClient.GetAsync($"WildlifeSightings/{target}");
            }
        }

        private async Task<String> GetSpeciesNameByID(int speciesId)
        {
            using (var db = new wildlife_DBEntities())
            {
                Species species = await db.Species.FirstOrDefaultAsync(s => s.ID == speciesId);
                string speciesName = species != null ? species.EnglishName : "Unknown";
                return speciesName; 
            }
        }

        [HttpGet]
        public async Task<SightingAPIModel> GetSightingByID(int id)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage response = await GetHttpResponse($"BySighting/{id}");
                SightingAPIModel apiData = await response.Content.ReadAsAsync<SightingAPIModel>();
                if (response.IsSuccessStatusCode)
                {
                    return apiData;
                }
                else
                {
                    throw new HttpException(404, "Resource Not Found");
                }
            }
        }

        [HttpGet]
        [Route("Sightings/SightingsList")]
        public async Task<ActionResult> SightingsIndex()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                IEnumerable<SightingAPIModel> apiData = new List<SightingAPIModel>();
                HttpResponseMessage response = await GetHttpResponse("All");
                apiData = await response.Content.ReadAsAsync<IEnumerable<SightingAPIModel>>();

                if (response.IsSuccessStatusCode)
                {
                    //I can't run the async method GetSpeciesNameByID in the select statement... 
                    //and it would also be ineffecient to get that data from the MVC db with every iteration...
                    //vs. getting all the species names first.
                    List<int> speciesIds = apiData.Select(s => s.SpeciesID).Distinct().ToList();
                    Dictionary<int, string> speciesNames = new Dictionary<int, string>();
                    foreach (int id in speciesIds)
                    {
                        speciesNames[id] = await GetSpeciesNameByID(id);
                    }

                    IEnumerable<SightingDetailsViewModel> viewModelData = apiData.Select(s => new SightingDetailsViewModel
                    {
                        ID = s.ID,
                        SpeciesID = s.SpeciesID,
                        SpeciesName = speciesNames[s.SpeciesID],
                        XCoordinate = s.XCoordinate,
                        YCoordinate = s.YCoordinate,    
                        Description = s.Description,
                        TimeStamp = s.TimeStamp,
                        Location = s.Location,
                        County = s.County
                    }).ToList();


                    return View(viewModelData);
                }
                else
                {
                    throw new HttpException(400, "Bad Request");
                }
            }
        }

        [HttpGet]
        [Route("Sightings/SightingInfo/{id}")]
        public async Task<ActionResult> SightingDetails(int? id)
        {
            if (id == null)
            {
                throw new HttpException(400, "Bad Request");
            }
            SightingAPIModel sighting = await GetSightingByID((int)id);
            if (sighting == null)
            {
                throw new HttpException(404, "Resource Not Found");
            }

            string speciesName = await GetSpeciesNameByID(sighting.SpeciesID);
            SightingDetailsViewModel viewModel = new SightingDetailsViewModel
            {
                ID = sighting.ID,
                SpeciesName = speciesName,
                XCoordinate = sighting.XCoordinate,
                YCoordinate = sighting.YCoordinate,
                TimeStamp = sighting.TimeStamp,
                Description = sighting.Description,
                Location = sighting.Location,
                County = sighting.County
            };
            return View(viewModel);
        }

        [HttpGet]
        [Route("Sightings/RecordSighting")]
        public ActionResult CreateSighting()
        {
            using (var db = new wildlife_DBEntities())
            {
                SightingViewModel viewModel = new SightingViewModel
                {
                    SpeciesList = db.Species.Select(s => new SelectListItem
                    {
                        Value = s.ID.ToString(),
                        Text = s.EnglishName
                    }).ToList()
                };
                return View(viewModel);
            }
        }

        [HttpPost]
        [Route("Sightings/RecordSighting")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateSighting(SightingViewModel sightingViewModel)
        {
            //should instead return create view, and have the form display warnrings about invalid inputs
            if (!ModelState.IsValid)
            {
                throw new HttpException(400, "Form data was invalid");
            }

            using (HttpClient httpClient = new HttpClient())
            {
                //concert the ViewModel to the expected Model for the API
                SightingAPIModel sighting = new SightingAPIModel() 
                {
                    SpeciesID = sightingViewModel.SpeciesID,
                    TimeStamp = sightingViewModel.TimeStamp,
                    XCoordinate = sightingViewModel.XCoordinate,    
                    YCoordinate = sightingViewModel.YCoordinate,
                    Description = sightingViewModel.Description,
                    Location = sightingViewModel.Location,
                    County = sightingViewModel.County
                };
                StringContent data = new StringContent(JsonConvert.SerializeObject(sighting), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await postHttpResponse("CreateSighting", data);
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
            if (id == null)
            {
                throw new HttpException(400, "Bad Request");
            }
            SightingAPIModel sighting = await GetSightingByID((int)id);
            if (sighting == null)
            {
                return HttpNotFound();
            }
            using (var db = new wildlife_DBEntities())
            {
                SightingViewModel viewModel = new SightingViewModel
                {
                    SpeciesList = db.Species.Select(s => new SelectListItem
                    {
                        Value = s.ID.ToString(),
                        Text = s.EnglishName
                    }).ToList(),
                    ID = sighting.ID,
                    SpeciesID = sighting.SpeciesID,
                    XCoordinate = sighting.XCoordinate,
                    YCoordinate = sighting.YCoordinate, 
                    TimeStamp = sighting.TimeStamp,
                    Description = sighting.Description, 
                    Location = sighting.Location, 
                    County = sighting.County
                };
                return View(viewModel);
            }
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
                SightingAPIModel sighting = new SightingAPIModel()
                {
                    ID = sightingViewModel.ID,
                    SpeciesID = sightingViewModel.SpeciesID,
                    TimeStamp = sightingViewModel.TimeStamp,
                    XCoordinate = sightingViewModel.XCoordinate,
                    YCoordinate = sightingViewModel.YCoordinate,
                    Description = sightingViewModel.Description,
                    Location = sightingViewModel.Location,
                    County = sightingViewModel.County
                };

                StringContent data = new StringContent(JsonConvert.SerializeObject(sighting), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await postHttpResponse("UpdateSighting", data);
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
            if (id == null)
            {
                throw new HttpException(400, "Bad Request");
            }
            SightingAPIModel sighting = await GetSightingByID((int)id);
            if (sighting == null)
            {
                throw new HttpException(404, "Resource Not Found");
            }

            string speciesName = await GetSpeciesNameByID(sighting.SpeciesID);
            SightingDetailsViewModel viewModel = new SightingDetailsViewModel
            {
                SpeciesName = speciesName,
                XCoordinate = sighting.XCoordinate,
                YCoordinate = sighting.YCoordinate,
                TimeStamp = sighting.TimeStamp,
                Description = sighting.Description,
                Location = sighting.Location,
                County = sighting.County
            };
            return View(viewModel);
        }

        [HttpPost, ActionName("DeleteSighting")]
        [Route("Sightings/RemoveSighting/{id}")]
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