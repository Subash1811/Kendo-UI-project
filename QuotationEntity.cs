using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace ShiplogixQuotation.Models
{
    [Table("quotations")]
    public class QuotationEntity
    {
        [Key]
        public int QuotationId { get; set; }

        // Foreign Keys
        
        public int? OrderTypeId { get; set; }
        public int? SalesExecutiveId { get; set; }
        public int? CustomerId { get; set; }
        public int? ModeCode { get; set; }
        public int? CurrencyId { get; set; }
        public int? SalesReferenceId { get; set; }
        
        public string OrderType { get; set; }
        public string SalesExecutive { get; set; }
        public string Customer { get; set; }
        public string ShipmentMode { get; set; }
        public string BaseCurrency { get; set; }
        public string SalesOpsReference { get; set; }

        // Other fields
        public DateTime? QuotationDate { get; set; }
        public DateTime? QuotationValidity { get; set; }
        public string RequestNumber { get; set; }
        public string CopyQuotation { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
        public DateTime? ValidTill { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

    
        
    }
}
