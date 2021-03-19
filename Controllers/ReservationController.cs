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

        
        public IActionResult Idx()
        {
            var list = _res.Reservations.Include(s => s.Student).Include(rt => rt.ReservationType);
            ViewBag.role = new IdentityRole();
            return View(list.ToList());
        }
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Student"))
                {
                    await GetDataByUser();
                }
                else if (User.IsInRole("Admin"))
                {
                    Idx();
                }
                else
                {
                    return NotFound();
                }
            }
            return View();
            
        }

        public async Task<IActionResult> GetDataByUser()
        {
            var student = await _userManager.GetUserAsync(HttpContext.User);
            var list = _res.Reservations.Include(s => s.Student).Include(rt => rt.ReservationType).Where(s => s.StudentId == student.Id);
            return View("Index", list.ToList());
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
                

                var reser = new Reservation();
                reser.Status = reservation.Status;
                reser.Date = reservation.Date;
                reser.Cause = reservation.Cause;
                reser.StudentId = student.Id;
                reser.ReservationTypeId = type.Id;
                

                _res.Add(reser);

                await _res.SaveChangesAsync();
                return RedirectToAction("index");
            }
            
            return View(reservation);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            var reser_details =  _res.Reservations.Find(id);
            
            var list = _res.ReservationTypes;
            
            ViewBag.types = list.ToList();
            return View(reser_details);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Reservation reservation)
        {

            if (ModelState.IsValid)
            {
                var type = _res.ReservationTypes.Where(r => r.Id == reservation.ReservationTypeId).FirstOrDefault();
                //reservation.ReservationType.Id = type.ToString();
                var student = await _userManager.GetUserAsync(HttpContext.User);
                //var studentId = student.Id;


                reservation.StudentId = student.Id;
                reservation.ReservationTypeId = type.Id;

                _res.Update(reservation);
                await _res.SaveChangesAsync();
                return RedirectToAction("index");
            }

            return View(reservation);
        }


        public  IActionResult Delete(int? id)
        {
            var list = _res.Reservations.Include(s => s.Student).Include(rt => rt.ReservationType);
            ViewBag.data = list.AsEnumerable();
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            var del = _res.Reservations.Find(id);
            return View(del);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public  IActionResult Delete(int id)
        {

            var del =  _res.Reservations.Find(id);
            _res.Reservations.Remove(del);
             _res.SaveChanges();
            return RedirectToAction("Index");
        }

        public async void Increment(int id)
        {
            var usr = _res.Reservations.Find(id);
            //var res = new Student();
            //var inc = usr.Student.resCount;
            var incr = new Student().resCount;
            
            var student = await _userManager.GetUserAsync(HttpContext.User);
            var u = await _res.Students.FirstOrDefaultAsync(s => s.Id == usr.StudentId);
            u.resCount = incr + 1;
            //int inc = Convert.ToInt32(usr.Student.resCount.ToString());
            //res.resCount += inc;
            //usr.Student.resCount = incr + 1;
            _res.Update(usr);
            _res.Update(u);
            await _res.SaveChangesAsync();
        }

        public IActionResult Approved(int id)
        {
            var resr = _res.Reservations.Find(id);
            Increment(id);
            //var app = new Reservation();
            resr.Status = "Approved";
            _res.Update(resr);
            _res.SaveChanges();
            return RedirectToAction("index");
        }
    }
}
