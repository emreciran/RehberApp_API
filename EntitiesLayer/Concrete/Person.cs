using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace EntityLayer.Concrete
{
    public class Person
    {
        [Key]
        public int PERSON_ID { get; set; }

        public string? PERSON_NAME { get; set; }

        public string? PERSON_SURNAME { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string? PERSON_PHONE { get; set; }

        public string? PERSON_DETAILS { get; set; }

        [Required]
        public int USER_ID { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string ImageName { get; set; } = "DefaultPerson.png";

        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        [NotMapped]
        public string ImageSrc { get; set; } = string.Empty;
    }
}
