using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationManager.Models
{
    public class ReservationContext : IdentityDbContext
    {
        public ReservationContext(DbContextOptions<ReservationContext> options) : base(options)
        {

        }
        public DbSet<Student> Students { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ReservationType> ReservationTypes { get; set; }
    }
}
