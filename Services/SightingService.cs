using Newtonsoft.Json;
using Ninject.Planning.Targets;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WildlifeMVC.Models;
using WildlifeMVC.ViewModels;

namespace WildlifeMVC.Services
{
    public interface ISightingService
    {
        Task<HttpResponseMessage> DeleteSightingAsync(string target);
        Task<String> GetSpeciesNameByID(int speciesId);   
        Task<SightingAPIModel> GetSightingByID(int id);
        Task<IEnumerable<SightingDetailsViewModel>> GetAllSightings();
        Task<SightingViewModel> GetSpeciesDropDownData();
        Task<HttpResponseMessage> CreateSighting(SightingViewModel sightingViewModel);
        Task<HttpResponseMessage> UpdateSighting(SightingViewModel sightingViewModel);
        Task<SightingDetailsViewModel> GetSightingDetailsData(int? id);
        Task<SightingViewModel> GetSightingViewModelData(int? id);
    }

    public class SightingService : ISightingService
    {
        private readonly wildlife_DBEntities dbContext;

        public SightingService(wildlife_DBEntities db)
        {
            dbContext = db;
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

        public async Task<String> GetSpeciesNameByID(int speciesId)
        {
            using (var db = new wildlife_DBEntities())
            {
                Species species = await db.Species.FirstOrDefaultAsync(s => s.ID == speciesId);
                string speciesName = species != null ? species.EnglishName : "Unknown";
                return speciesName;
            }
        }

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
                    throw new HttpException(404, "Resource Not Found");
                }
            }
        }

        public async Task<IEnumerable<SightingDetailsViewModel>> GetAllSightings()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                IEnumerable<SightingAPIModel> apiData = new List<SightingAPIModel>();
                HttpResponseMessage response = await getHttpResponse("All");
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

                    return viewModelData;
                }
                else
                {
                    throw new HttpException(400, "Bad Request");
                }
            }
        }

        public async Task<HttpResponseMessage> UpdateSighting(SightingViewModel sightingViewModel) 
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
            return await postHttpResponse("UpdateSighting", data);
        }

        public async Task<SightingDetailsViewModel> GetSightingDetailsData(int? id)
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
            return viewModel;
        }

        public async Task<SightingViewModel> GetSightingViewModelData(int? id) 
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
                return viewModel;
            }
        }

        public async Task<HttpResponseMessage> CreateSighting(SightingViewModel sightingViewModel) 
        {
            //convert the ViewModel to the expected Model for the API
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
            return await postHttpResponse("CreateSighting", data);
        }

        public async Task<SightingViewModel> GetSpeciesDropDownData()
        {
            using (var db = new wildlife_DBEntities())
            {
                SightingViewModel viewModel = new SightingViewModel
                {
                    SpeciesList = await db.Species.Select(s => new SelectListItem
                    {
                        Value = s.ID.ToString(),
                        Text = s.EnglishName
                    }).ToListAsync()
                };
                return viewModel;
            }
        }

        public async Task<HttpResponseMessage> DeleteSightingAsync(string target)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("http://localhost:64341/api/");
                return await httpClient.DeleteAsync($"WildlifeSightings/{target}");
            }
        }
    }
}