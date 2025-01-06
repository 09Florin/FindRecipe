﻿using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication4.Models
{
    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public LoginViewModel()
        {
            Username = string.Empty;
            Password = string.Empty;
        }

    }
}
