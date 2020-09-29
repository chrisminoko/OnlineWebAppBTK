using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Data.Entity;
using System.Web.Mvc;

namespace OnlineWebApp.Models
{
    [Bind(Exclude = "OrderId")]
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ScaffoldColumn(false)]
        [Display(Name = "Order ID")]
        public int Order_Id { get; set; }
        [ScaffoldColumn(false)]
        public string Username { get; set; }
        [Required]
        [Display(Name = "First Name:")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name:")]
        public string LastName { get; set; }
        [Required]
        [Display(Name = "Address:")]
        public string Address { get; set; }
        [Required]
        [Phone]
        [Display(Name = "Cell Number:")]
        public string Phone { get; set; }
        [ScaffoldColumn(false)]
        public decimal Total { get; set; }
        [ScaffoldColumn(false)]
        public System.DateTime OrderDate { get; set; }
        [Required]
        [StringLength(40)]
        public string Country { get; set; }
        [Required]
        [StringLength(40)]
        public string City { get; set; }
        [Required]
        [DisplayName("Postal Code")]
        [StringLength(10)]
        public string PostalCode { get; set; }
        [Required]
        [DisplayName("Email Address")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [ScaffoldColumn(false)]
        public string Client_Id { get; set; }
        public virtual Client Client { get; set; }
        public List<OrderDetails> OrderDetails { get; set; }
    }
}
