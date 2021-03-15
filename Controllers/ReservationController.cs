using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationManager.Controllers
{
    public class ReservationController : Controller
    {
        private readonly ReservationContext _res;
        private readonly UserManager<IdentityUser> _userManager;

        public ReservationController(ReservationContext res, UserManager<IdentityUser> userManager)
        {
            _res = res;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var list = _res.Reservations.Include(s => s.Student).Include(rt => rt.ReservationType);
            return View(list.ToList());
        }

        public IActionResult Create()
        {
            //ViewBag.userId = _userManager.GetUserId(HttpContext.User);
            var list = _res.ReservationTypes;
            ViewBag.types = list.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                var type = _res.ReservationTypes.Where(r => r.Id == reservation.ReservationType.name).FirstOrDefault();

                var student = await _userManager.GetUserAsync(HttpContext.User);
                
                var reser = new Reservation
                {
                    Status = reservation.Status,
                    Date = reservation.Date,
                    Cause = reservation.Cause,
                    ReservationType = type,
                    Student = (Student)student
                    

                };
                
                
                
                _res.Add(reser);
                
               await _res.SaveChangesAsync();
                return RedirectToAction("index");
            }
            
            return View(reservation);
        }


    }
}
