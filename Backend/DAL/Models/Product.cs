using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Product
    {
        [Key]
        public Guid ProductId { get; set; }
        public string Name { get; set; }    
        public string? Description { get; set; } 
        public int Quantity { get; set; }
        public decimal Price { get; set; }
          
    }

}
