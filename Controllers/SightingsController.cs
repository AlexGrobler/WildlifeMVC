using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WildlifeMVC.Models;
using WildlifeMVC.ViewModel;

namespace WildlifeMVC.Controllers
{
    public class SightingsController : Controller
    {
        // GET: Sightings
        public async Task<ActionResult> Sightings()
        {
            IEnumerable <SightingAPIModel> apiData = new List<SightingAPIModel>();
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    httpClient.BaseAddress = new Uri("http://localhost:64341/api/");
                    HttpResponseMessage response = await httpClient.GetAsync("WildlifeSightings/All");
                    if (response.IsSuccessStatusCode)
                    {
                        apiData = await response.Content.ReadAsAsync<IEnumerable<SightingAPIModel>>();
                    }
                }
                catch (Exception ex)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, ex.Message);
                }
            }

            return View(apiData);
        }

        // GET: PcTbls/Details/5
/*        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                // return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                throw new HttpException(400, "Bad Request: No ID Supplied");
            }
            PcTbl pcTbl = db.PcTbls.Find(id);
            if (pcTbl == null)
            {
                *//*return HttpNotFound();*//*
                throw new HttpException(404, "Bad Request: No Record Found");
            }

            WildlifeAPIModel apiData = new WildlifeAPIModel();
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    httpClient.BaseAddress = new Uri("http://localhost:54093/api/");
                    HttpResponseMessage response = await httpClient.GetAsync("GetPcInfo/" + id);
                    if (response.IsSuccessStatusCode)
                    {
                        apiData = await response.Content.ReadAsAsync<PcInfoTbl>();
                    }
                }
                catch (HttpRequestException)
                {
                    apiData.ProcessorType = "API Unavailable";
                    apiData.Memory = 0;
                    apiData.ServiceTag_PerfectPcs = "API Unavailable";
                }
            }
            return View(apiData);
        }*/

    }
}