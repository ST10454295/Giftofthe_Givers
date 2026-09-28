using GiftOfTheGivers.Data;
using GiftOfTheGivers.Models;
using Microsoft.AspNetCore.Mvc;

namespace GiftOfTheGivers.Controllers
{
    public class VolunteerController : Controller
    {
        private readonly ApplicationDbContext _context;


        public VolunteerController(
            ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(
            Volunteer volunteer)
        {
            if (!ModelState.IsValid)
            {
                return View(volunteer);
            }


            volunteer.RegistrationDate =
                DateTime.Now;


            _context.Volunteers.Add(volunteer);

            await _context.SaveChangesAsync();


            return RedirectToAction(
                "Confirmation");
        }


        public IActionResult Confirmation()
        {
            return View();
        }
    }
}