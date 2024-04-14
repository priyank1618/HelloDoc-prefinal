using DAL.DataModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModel
{
    public class CreateProviderAccount
    {
       
            [Required]
            public string Username { get; set; }
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
            [Required]
            public string Role { get; set; }
            [Required]
            public string Firstname { get; set; }
            public string? Lastname { get; set; }
            [Required]
            public string Email { get; set; }
            public string? Phone { get; set; }
            public string? MedLicense { get; set; }
            public string? NPINum { get; set; }
            public string? Address1 { get; set; }
            public string? Address2 { get; set; }
            public string? City { get; set; }
            public string? State { get; set; }
        public string? Zip { get; set; }
        
        public string? BillingPhone { get; set; }
          
            public string BusinessName { get; set; }
        
            public string BusinessWebsite { get; set; }
            public string? AdminNotes { get; set; }
            [AllowedExtensionsAttribute(".jpg", ".jpeg", ".png", ErrorMessage = "Only jpg, jpeg and png files are allowed.")]
            public IFormFile? Photo { get; set; }
            public IFormFile? ICA { get; set; }
            public IFormFile? BackgroundCheck { get; set; }
            public IFormFile? HIPAA { get; set; }
            public IFormFile? NonDisclosure { get; set; }
            public IFormFile? License { get; set; }
    
            public List<Region>? regions { get; set; }
            public List<Role>? roles { get; set; }

        }

        public class AllowedExtensionsAttribute : ValidationAttribute
        {
            private readonly string[] _extensions;

            public AllowedExtensionsAttribute(params string[] extensions)
            {
                _extensions = extensions;
            }

            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                if (value is IFormFile file)
                {
                    var extension = Path.GetExtension(file.FileName);
                    if (!IsValidExtension(extension))
                    {
                        return new ValidationResult("Only JPG and PNG files are allowed.");
                    }
                }

                return ValidationResult.Success;
            }

            private bool IsValidExtension(string extension)
            {
                return Array.Exists(_extensions, e => e.Equals(extension, StringComparison.OrdinalIgnoreCase));
            }
        }
    }



