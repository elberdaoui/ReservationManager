using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationManager.Models
{
    public class ReservationType
    {
        
        public string Id { get; set; }
        public string name { get; set; }
        public int accessNumber { get; set; }
        
    }
}
