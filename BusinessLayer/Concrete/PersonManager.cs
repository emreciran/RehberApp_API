using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
    public class PersonManager : IPersonService
    {
        IPersonRepository _personRepository;

        public PersonManager(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        public async Task DeletePerson(int id)
        {
            await _personRepository.DeletePerson(id);
        }

        public async Task<List<Person>> GetAllPersons()
        {
            return await _personRepository.GetAllPersons();
        }

        public async Task<Person> GetPersonById(int id)
        {
            return await _personRepository.GetPersonById(id);
        }

        public async Task<Person> NewPerson(Person person)
        {
            return await _personRepository.NewPerson(person);
        }

        public async Task<Person> UpdatePerson(Person person)
        {
            return await (_personRepository.UpdatePerson(person));
        }
    }
}
