using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReadSphere.Data;
using Models;
using ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ReadSphere.Controllers
{
    public class AllClubsController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<User> _userManager;

        public AllClubsController(ApplicationDBContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /AllClubs
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var clubs = await _context.Clubs
                    .Include(c => c.Users)
                    .ToListAsync();

                var viewModel = new ClubViewModel
                {
                    Clubs = clubs,
                    Count = clubs.Count
                };

                return View("AllClubs", viewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return View("Index", "Home");

            }
        }

        // POST: /AllClubs/Join[HttpPost]
        public async Task<IActionResult> Join(int clubId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return RedirectToAction("Login", "Account");

                var club = await _context.Clubs.FindAsync(clubId);
                if (club == null)
                    return NotFound();

                // Load user’s joined clubs
                await _context.Entry(user).Collection(u => u.Clubs).LoadAsync();

                // Prevent duplicate join
                if (user.Clubs.Any(c => c.Id == clubId))
                    return RedirectToAction("Index");

                user.Clubs.Add(club);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
