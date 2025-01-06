﻿namespace WebApplication4.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string City { get; set; }
        public DateTime Birthday { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<Recipe> FavorteRecipes { get; set; } = new List<Recipe>();
        public User()
        {
            // initialize the City property with a non-null value
            Username = string.Empty;
            Email = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            Password = string.Empty;
            City = string.Empty;
        }
    }
}
