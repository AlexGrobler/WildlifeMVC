using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WildlifeMVC.Models;

namespace WildlifeMVC.Services
{
    //interface is required for registering dependencies which will be injected into the controller classes
    public interface ISpeciesService
    {
        Task<Species> GetSpeciesByIdAsync(int id);
        Task<List<Species>> GetAllSpecies();
        Task AddSpeciesAsync(Species species);
        Task UpdateSpeciesAsync(Species species);
        Task DeleteSpeciesAsync(int id);
    }

    //service players allow for resuable code that can be used between multiple views, like in the home controller
    public class SpeciesService : ISpeciesService
    {
        private readonly wildlife_DBEntities dbContext;

        public SpeciesService(wildlife_DBEntities db)
        {
            dbContext = db;
        }

        public async Task<Species> GetSpeciesByIdAsync(int id)
        {
            return await dbContext.Species.FindAsync(id);
        }

        public async Task<List<Species>> GetAllSpecies() 
        {
            return  await dbContext.Species.ToListAsync();
        }

        public async Task AddSpeciesAsync(Species species)
        {
            dbContext.Species.Add(species);
            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateSpeciesAsync(Species species)
        {
            dbContext.Entry(species).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteSpeciesAsync(int id)
        {
            Species species = await dbContext.Species.FindAsync(id);
            dbContext.Species.Remove(species);
            dbContext.SaveChanges();
        }
    }
}