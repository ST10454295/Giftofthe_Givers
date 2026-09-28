using GiftOfTheGivers.Data;
using GiftOfTheGivers.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GiftOfTheGivers.Controllers
{
    [Authorize(Roles = "Employee")]
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var projects = await _context.ProjectUpdates
                    .OrderByDescending(x => x.PostedDate)
                    .ToListAsync();

            ViewBag.VolunteerCount = await _context.Volunteers.CountAsync();

            ViewBag.TotalDonations = await _context.Donations
                    .Where(x => x.Currency == "ZAR")
                    .SumAsync(x => x.Amount);

            return View(projects);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostUpdate(ProjectUpdate update)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Dashboard");
            }

            // FIX: FindFirstValue returns a string, but EmployeeId is an int
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (int.TryParse(userIdString, out int employeeId))
            {
                update.EmployeeId = employeeId;
            }
            else
            {
                return RedirectToAction("Dashboard");
            }

            update.PostedDate = DateTime.UtcNow;

            _context.ProjectUpdates.Add(update);
            await _context.SaveChangesAsync();

            return RedirectToAction("Dashboard");
        }
    }
}