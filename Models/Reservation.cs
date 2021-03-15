using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationManager.Models
{
    public class Reservation
    {
        [Key]
        [ForeignKey("Student,ReservationType")]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public string Cause { get; set; }

        public virtual Student Student { get; set; }
        public virtual ReservationType ReservationType { get; set; }

    }
}
