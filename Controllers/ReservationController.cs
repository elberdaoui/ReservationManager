using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using ReservationManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationManager.Controllers
{
    [Authorize]
    public class ReservationController : Controller
    {
        private readonly ReservationContext _res;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IToastNotification _toastNotification;

        public ReservationController(ReservationContext res, UserManager<IdentityUser> userManager, IToastNotification toastNotification)
        {
            _res = res;
            _userManager = userManager;
            _toastNotification = toastNotification;
        }

        #region Reservation list

        #region Pending list
        public IActionResult Idx()
        {
            
            var list = _res.Reservations.Include(s => s.Student).Include(rt => rt.ReservationType)
                .OrderBy(c => c.Student.resCount);
            //ViewBag.role = new IdentityRole();
            return View(list.ToList()
                .Where(d => d.Date >= DateTime.Today && d.Status == "Pending"));
        }

        public async Task<IActionResult> GetDataByUser()
        {
            var student = await _userManager.GetUserAsync(HttpContext.User);
            var list = _res.Reservations.Include(s => s.Student).Include(rt => rt.ReservationType).Where(s => s.StudentId == student.Id);
            return View("Index", list.ToList());
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
        #endregion

        #region Approved list
        public IActionResult ApprovedListAdmin()
        {

            var list = _res.Reservations.Include(s => s.Student).Include(rt => rt.ReservationType)
                .OrderBy(c => c.Date);
            //ViewBag.role = new IdentityRole();
            return View(list.ToList()
                .Where(d => d.Status == "Approved"));
        }

        public async Task<IActionResult> ApprovedListUser()
        {
            var student = await _userManager.GetUserAsync(HttpContext.User);
            var list = _res.Reservations.Include(s => s.Student).Include(rt => rt.ReservationType).Where(s => s.StudentId == student.Id);
            return View("Index", list.ToList().OrderBy(d => d.Date));
        }

        public async Task<IActionResult> ApprovedList()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Student"))
                {
                    await ApprovedListUser();
                }
                else if (User.IsInRole("Admin"))
                {
                    ApprovedListAdmin();
                }
                else
                {
                    return NotFound();
                }
            }
            return View("Index");
        }

        #endregion

        #region Declined list
        public IActionResult DeclinedListAdmin()
        {

            var list = _res.Reservations.Include(s => s.Student).Include(rt => rt.ReservationType)
                .OrderBy(c => c.Date);
            //ViewBag.role = new IdentityRole();
            return View(list.ToList()
                .Where(d => d.Status == "Declined"));
        }

        public async Task<IActionResult> DeclinedListUser()
        {
            var student = await _userManager.GetUserAsync(HttpContext.User);
            var list = _res.Reservations.Include(s => s.Student).Include(rt => rt.ReservationType).Where(s => s.StudentId == student.Id);
            return View("Index", list.ToList().OrderBy(d => d.Date));
        }

        public async Task<IActionResult> DeclinedList()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Student"))
                {
                    await DeclinedListUser();
                }
                else if (User.IsInRole("Admin"))
                {
                    DeclinedListAdmin();
                }
                else
                {
                    return NotFound();
                }
            }
            return View("Index");
        }

        #endregion

        #endregion

        #region Create Reservation
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
                _toastNotification.AddSuccessToastMessage("You reserve your place successfully");
                return RedirectToAction("index");
            }
            
            return View(reservation);
        }
        #endregion

        #region Edit Reservation
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
                _toastNotification.AddSuccessToastMessage("You modified reserve successfully");
                return RedirectToAction("index");
            }

            return View(reservation);
        }
        #endregion

        #region Delete Reservation
        public IActionResult Delete(int? id)
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
            _toastNotification.AddWarningToastMessage("You deleted your reservation");
            return RedirectToAction("Index");
        }
        #endregion

        #region Confirm and Decline methods
        public void Increment(int id)
        {
            var usr = _res.Reservations.Find(id);
            //var res = new Student();
            //var inc = usr.Student.resCount;
            //var incr = new Student().resCount;
            
            //var student = await _userManager.GetUserAsync(HttpContext.User);
            var u = _res.Students.FirstOrDefault(s => s.Id == usr.StudentId);
            var inc = usr.Student.resCount;
            u.resCount = inc + 1;
            //int inc = Convert.ToInt32(usr.Student.resCount.ToString());
            //res.resCount += inc;
            //usr.Student.resCount = incr + 1;
            _res.Update(usr);
            _res.Update(u);
             _res.SaveChanges();
        }

        public async Task<IActionResult> Confirm(int id)
        {
            var resr = _res.Reservations.Find(id);
            if(resr.Status != "Approved")
            {
                Increment(id);
                //var app = new Reservation();
                resr.Status = "Approved";
                _res.Update(resr);
                await _res.SaveChangesAsync();
                _toastNotification.AddSuccessToastMessage("Reservation approved");
            }
            else
            {
                _toastNotification.AddErrorToastMessage("Reservation already approved");
            }
            
            return RedirectToAction("index");
        }

        public IActionResult Decline(int id)
        {
            var resr = _res.Reservations.Find(id);

            if (resr.Status != "Declined")
            {
                //var app = new Reservation();
                resr.Status = "Declined";
                _res.Update(resr);
                _res.SaveChanges();
                _toastNotification.AddWarningToastMessage("Reservation declined");
                
            }
            else
            {
                _toastNotification.AddErrorToastMessage("Reservation already declined");
            }

            return RedirectToAction("index");

        }
        #endregion
    }
}
