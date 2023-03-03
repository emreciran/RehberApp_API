using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Abstract
{
    public interface IPersonService
    {
        Task<List<Person>> GetAllPersons();

        Task<Person> GetPersonById(int id);

        Task<Person> NewPerson(Person person);

        Task<Person> UpdatePerson(Person person);

        Task DeletePerson(int id);
    }
}
