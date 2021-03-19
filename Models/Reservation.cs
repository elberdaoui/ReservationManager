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
        [ForeignKey("StudentId,ReservationTypeId")]
        public int Id { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public string Cause { get; set; }

        public string StudentId { get; set; }
        public string ReservationTypeId { get; set; }
        public virtual Student Student { get; set; }
        public virtual ReservationType ReservationType { get; set; }

    }
}
