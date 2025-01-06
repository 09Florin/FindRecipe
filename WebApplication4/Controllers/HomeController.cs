using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationFoodManagerDbContext _dbContext;
        public HomeController(ILogger<HomeController> logger, ApplicationFoodManagerDbContext dbContext)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public IActionResult Index()
        {
            var adminministratorId = HttpContext.Session.GetInt32("AdministratorId");
            if (adminministratorId != null)
            {
                bool isAdmin = true;
                ViewData["IsAdmin"] = isAdmin;
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                ViewData["IsUserLoggedIn"] = true;
            }
            else
            {
                ViewData["IsUserLoggedIn"] = false;
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registration(RegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    // Map properties from RegistrationViewModel to Client
                    Username = model.Username,
                    Password = model.Password,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Birthday = model.Birthday,
                    CreatedAt = DateTime.UtcNow
                };

                _dbContext.Users?.Add(user);
                _dbContext.SaveChanges();

                HttpContext.Session.SetInt32("UserId", user.Id);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            var user = _dbContext.Users?.FirstOrDefault(u => u.Username == model.Username && u.Password == model.Password);

            if (user != null)
            {
                TempData["UserId"] = user.Id;
                ViewBag.userClient = user;
                HttpContext.Session.SetInt32("UserId", user.Id);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("Username", "Invalid username or password.");
                return View(model);
            }
        }

        public IActionResult Profile()
        {
            // Check if user is logged in
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Home"); // Redirect to login if not logged in
            }

            // Fetch the user with their favorite recipes
            var user = _dbContext.Users
                .Include(u => u.FavorteRecipes) // Include favorite recipes
                .FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return NotFound(); // Handle not found cases
            }

            return View("Profile", user); // Pass user to the view
        }


        [HttpGet]
        public IActionResult AdminLogin()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AdminLogin(LoginViewModel model)
        {
            var admin = _dbContext.Administrators?.FirstOrDefault(a => a.Username == model.Username && a.Password == model.Password);

            if (admin != null)
            {
                TempData["AdministratorId"] = admin.Id;
                ViewBag.userClient = admin;
                HttpContext.Session.SetInt32("AdministratorId", admin.Id);
                return RedirectToAction("Index", "Administrators");
            }
            else
            {
                ModelState.AddModelError("Username", "Invalid username or password.");
                return View(model);
            }
        }

        public IActionResult AdminProfile()
        {
            var adminId = HttpContext.Session.GetInt32("AdministratorId");
            // Fetch the user with their created recipes
            var administrator = _dbContext.Administrators
                .Include(a => a.AddedRecipes) // Include created recipes
                .FirstOrDefault(a => a.Id == adminId);

            if (administrator == null)
            {
                return NotFound(); // Handle not found cases
            }

            return View(administrator); // Pass admin to the view
        }

        public IActionResult Logout()
        {
            // Clear the user session
            HttpContext.Session.Remove("UserId");

            // Optionally, you can clear other session variables related to the user
            HttpContext.Session.Clear();

            // Redirect to the home page or login page
            return RedirectToAction("Index", "Home");
        }

    }
}
