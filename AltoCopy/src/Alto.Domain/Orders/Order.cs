using System;
using System.Collections.Generic;

namespace Alto.Domain.Orders
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public AltoUser User { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }

        public string PhoneNumber { get; set; }
        public List<OrderProduct> Products { get; set; }
        public string BuyerNote { get; set; }
        public string SellerNote { get; set; }  
        public string SaleId { get; set; }      
        public string InvoiceId { get; set; }  
        public string PaymentId { get; set; }
        public decimal TotalPrice { get; set; } 
        public decimal TotalShippingPrice { get; set; }
        public string Variations { get; set; }

        #region Address
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        #endregion

        public DateTime InsertDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }
}
