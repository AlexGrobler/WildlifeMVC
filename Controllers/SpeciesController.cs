using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WildlifeMVC.Models;

namespace WildlifeMVC.Controllers
{
    public class SpeciesController : Controller
    {
        private wildlife_DBEntities db = new wildlife_DBEntities();

        [HttpGet]
        [Route("Species")]
        public async Task<ActionResult> Index()
        {
            List<Species> speciesList = await db.Species.ToListAsync();
            return View(speciesList);
        }

        [HttpGet]
        [Route("Species/Info/{name}/{id}")]
        public async Task<ActionResult> Details(string name, int? id)
        {
            if (name == null || id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Species species = await db.Species.FindAsync(id);
            if (species == null)
            {
                return HttpNotFound();
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
            if (ModelState.IsValid)
            {
                db.Species.Add(species);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(species);
        }

        [HttpGet]
        [Route("Species/EditSpecies/{name}/{id}")]
        public async Task<ActionResult> Edit(string name, int? id)
        {
            name = name.Replace("-", " ");
            if (id == null || name == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Species species = await db.Species.FindAsync(id);
            if (species == null)
            {
                return HttpNotFound();
            }
            return View(species);
        }

        [HttpPost]
        [Route("Species/EditSpecies/{name}/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,EnglishName,LatinName,ShortDescription,LongDescription,ImageURL,VideoURL")] Species species)
        {
            if (ModelState.IsValid)
            {
                db.Entry(species).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(species);
        }

        [HttpGet]
        [Route("Species/DeleteSpecies/{name}/{id}")]
        public async Task<ActionResult> Delete(string name, int? id)
        {
            if (id == null || name == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Species species = await db.Species.FindAsync(id);
            if (species == null)
            {
                return HttpNotFound();
            }
            return View(species);
        }

        [HttpPost, ActionName("Delete")]
        [Route("Species/DeleteSpecies/{name}/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Species species = await db.Species.FindAsync(id);
            db.Species.Remove(species);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
