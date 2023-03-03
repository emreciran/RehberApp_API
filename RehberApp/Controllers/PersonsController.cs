using BusinessLayer.Abstract;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RehberApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PersonsController : ControllerBase
    {
        private IPersonService _personService;
        private readonly IWebHostEnvironment _hostEnvironment;

        public PersonsController(IPersonService personService, IWebHostEnvironment hostEnvironment)
        {
            _personService = personService;
            this._hostEnvironment = hostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPersons()
        {
            var persons = await _personService.GetAllPersons();
            if(!persons.Any()) return Ok("Person not found!");

            return Ok(persons);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPersonById(int id)
        {
            var person = await _personService.GetPersonById(id);
            if(person == null) return BadRequest("Person not found!");

            return Ok(person);
        }

        [HttpPost]
        public async Task<IActionResult> NewPerson([FromForm]Person person)
        {
            if (person.ImageFile != null)
            {
                person.ImageName = await SaveImage(person.ImageFile);
            }

            var createdPerson = await _personService.NewPerson(person);
            if(createdPerson == null) return BadRequest();

            return CreatedAtAction("GetAllPersons", new { id = createdPerson.PERSON_ID }, createdPerson);
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePerson(Person person)
        {
            var updatedPerson = await _personService.UpdatePerson(person);
            return Ok(updatedPerson);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerson(int id)
        {
            var person = await _personService.GetPersonById(id);

            if (person != null)
            {
                DeleteImage(person.ImageName);

                await _personService.DeletePerson(id);
                return Ok();
            }

            return BadRequest();
        }

        [NonAction]
        public async Task<string> SaveImage(IFormFile file)
        {
            string imageName = new String(Path.GetFileNameWithoutExtension(file.FileName).Take(10).ToArray()).Replace(' ', '-');
            imageName = imageName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(file.FileName);
            var imagePath = Path.Combine(_hostEnvironment.ContentRootPath, "Images", imageName);
            using (var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return imageName;
        }

        [NonAction]
        public void DeleteImage(string imageName)
        {
            var imagePath = Path.Combine(_hostEnvironment.ContentRootPath, "Images", imageName);
            if (System.IO.File.Exists(imagePath))
                System.IO.File.Delete(imagePath);
        }
    }
}
