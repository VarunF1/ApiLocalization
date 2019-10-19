using System;
using System.ComponentModel.DataAnnotations;
using Api.Resources;

namespace Api.Models
{
    public class Employee
    {
        [Required(ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "NameRequired")]
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime Timestamp { get; set; }
    }
}