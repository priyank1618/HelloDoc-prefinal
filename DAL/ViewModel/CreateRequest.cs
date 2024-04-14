using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.DataModels;

namespace DAL.ViewModel
{
    public class CreateRequest
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }
        [Column(TypeName = "character varying")]
        public string PhoneNumber { get; set; }

        [StringLength(256)]
        public string Email { get; set; }

        [Column(TypeName = "timestamp without time zone")]
        public DateTime? BirthDate { get; set; }

        public DateTime? CreatedDate { get; set; }
       
        [StringLength(100)]
        public string? Street { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(100)]
        public string? State { get; set; }

        [StringLength(10)]
        public string? ZipCode { get; set; }

        public List<Region>? Region { get; set; }

        public string? AdminNote { get; set; }

        public List<Region>? regions { get; set; }

    }
}
