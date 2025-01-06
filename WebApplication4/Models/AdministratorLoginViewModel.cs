using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication4.Models
{
    public class AdministratorLoginViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public AdministratorLoginViewModel()
        {
            Username = string.Empty;
            Password = string.Empty;
        }

    }
}
