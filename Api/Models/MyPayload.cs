using System;
using System.ComponentModel.DataAnnotations;
using Api.Resources;

namespace Api.Models
{
    public class Employee
    {
        [Required(ErrorMessageResourceType = typeof(AmazingResource), AllowEmptyStrings = false, ErrorMessageResourceName = "NameRequired")]
        public string Name { get; set; }

        [Required(ErrorMessageResourceType = typeof(AmazingResource), AllowEmptyStrings = false, ErrorMessageResourceName = "DescriptionRequired")]
        public string Description { get; set; }

        [Required(ErrorMessageResourceType = typeof(AmazingResource), AllowEmptyStrings = false, ErrorMessageResourceName = "TimestampRequired")]
        public DateTime Timestamp { get; set; }
    }
}