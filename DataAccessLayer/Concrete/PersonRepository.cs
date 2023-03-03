using DataAccessLayer.Abstract;
using DataAccessLayer.Context;
using EntityLayer.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete
{
    public class PersonRepository : IPersonRepository
    {
        private readonly ApplicationDbContext db;

        public PersonRepository(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<List<Person>> GetAllPersons()
        {
            return await db.Persons.ToListAsync();
        }

        public async Task<Person> GetPersonById(int id)
        {
            var person = await db.Persons.FindAsync(id);
            return person;
        }

        public async Task<Person> NewPerson(Person person)
        {
            db.Persons.Add(person);
            await db.SaveChangesAsync();
            return person;
        }

        public async Task<Person> UpdatePerson(Person person)
        {
            db.Persons.Update(person);
            await db.SaveChangesAsync();
            return person;
        }

        public async Task DeletePerson(int id)
        {
            var deletedPerson = await GetPersonById(id);
            db.Persons.Remove(deletedPerson);
            await db.SaveChangesAsync();
        }
    }
}
