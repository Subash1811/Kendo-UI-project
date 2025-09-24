using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace ShiplogixQuotation.ViewModels
{


    public class QuotationViewModel
    {
        [Key]
        public int QuotationId { get; set; }      
        public int? OrderTypeId { get; set; }
        public int? SalesExecutiveId { get; set; }
        public int? CustomerId { get; set; }
        public int? ModeCode { get; set; }
        public int? CurrencyId { get; set; }
        public int? SalesReferenceId { get; set; }
        // Display-only fields for grid/UI
        public string OrderTypeName { get; set; }      
        public string SalesExecutiveName { get; set; }     
        public string CustomerName { get; set; }    
        public string ShipmentModeName { get; set; }
        public string CurrencyName { get; set; }
        public string SalesOpsReference { get; set; }
        [Required(ErrorMessage = "QuotationDate  is required")]
        public DateTime? QuotationDate { get; set; }
        [Required(ErrorMessage = "QuotationValidity is required")]
        public DateTime? QuotationValidity { get; set; }
        public string RequestNumber { get; set; }
        public string CopyQuotation { get; set; }
        [Required(ErrorMessage = "Origin is required")]
        public string Origin { get; set; }
        [Required(ErrorMessage = "Destination is required")]
        public string Destination { get; set; }
        public string Remarks { get; set; }
       
        public DateTime? ValidTill { get; set; }
        public List<CommodityDetails> Commodities { get; set; } = new List<CommodityDetails>();
        public List<Route> Routes { get; set; } = new List<Route>();
        public List<Attachment> Attachments { get; set; } = new List<Attachment>();
    }


    

    public class RouteViewModel
    {
        public int RouteId { get; set; }
        public int QuotationId { get; set; }
        public string Mode { get; set; }
        public int? ProviderId { get; set; }
      
        public int? OriginPortId { get; set; }
        public string OriginPortName { get; set; }    // optional display
        public int? DestinationPortId { get; set; }
        public string DestinationPortName { get; set; }// optional display
        public int? TSP1PortId { get; set; }
        public int? TSP2PortId { get; set; }
        public int? TSP3PortId { get; set; }      
        public string Duration { get; set; }
        public decimal? BuyingRate { get; set; }
        public decimal? SellingRate { get; set; }
        public string Type { get; set; }
        //public int? CurrencyId { get; set; }
        public string Terms { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
    public class PortViewModel
    {
        public int PortId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Country { get; set; }
    }


    [Table("currencies")]
    public class Currency
    {
        [Key]
        [Column("cur_id")]
        public int CurrencyId { get; set; }       
        [Column("currency_code")]
        [StringLength(3)]
        public string CurrencyCode { get; set; }     
        [Column("currency_name")]
        [StringLength(50)]
        public string CurrencyName { get; set; }
        public bool IsActive { get; internal set; }
    }

    [Table("sales_references")]
    public class SalesReference
    {
        [Key]
        [Column("ref_id")]
        public int ReferenceId { get; set; }
        [Column("reference_number")]
        [StringLength(20)]
        public string ReferenceNumber { get; set; }       
        [Column("record_date")]
        public DateTime RecordDate { get; set; }       
        [Column("contact_name")]
        [StringLength(50)]
        public string ContactName { get; set; }
        [Column("company_name")]
        [StringLength(100)]
        public string CompanyName { get; set; }
        public bool IsActive { get; internal set; }
    }

    [Table("OrderType")]
    public class OrderType
    {     
        public int OrderTypeId { get; set; }
        [Required(ErrorMessage = "Order Type is required")]
        public string DocTypeDescription { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    [Table("SalesExecutive")]
    public class SalesExecutive
    {
        [Required(ErrorMessage = "Please select a Sales Executive.")]
        public int SalesExecutiveId { get; set; }
     
        public string Name { get; set; }
      
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    [Table("Customer")]
    public class Customer
    {
        public int CustomerId { get; set; }

        public string typ { get; set; }
        public string CustomerName { get; set; }
       
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    [Table("ShipmentMode")]
    public class ShipmentMode
    {
        [Key]
      
        public int ModeCode { get; set; }
        public string ModeName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    [Table("CommodityDetail", Schema = "dbo")]
    public class CommodityDetails
    {
        [Key] public int CommodityDetailId { get; set; }
        public int QuotationId { get; set; }
        public int CommodityId { get; set; }           
        public string UnitType { get; set; }                 
        public int TypeQty { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public decimal? Length { get; set; }
         public decimal? GrossWeight { get; set; }
         public decimal? Volume { get; set; }
        public decimal? VolumetricWeight { get; set; }
        public decimal? NetWeight { get; set; }      
        public bool Hazardous { get; set; }
        public bool OverSize { get; set; }
        public bool Reefer { get; set; }
        public bool Ventilation { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }


    [Table("Commodity")]
    public class Commodity
    {
        [Key]
        [Column("commodity_id")]
        public int CommodityId { get; set; }
        [Required]
        [Column("Name")]
        [StringLength(100)]
        public string CommodityName { get; set; }
        [Column("Isactive")]
        public bool IsActive { get; set; } = true;
        [Column("CreatedDate")]
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
    }

    [Table("Port")]
    public class Port
    {
        [Key]
        public int PortId { get; set; }
        [Required]
        [StringLength(20)]
        public string Code { get; set; }
        [Required]
        [StringLength(200)]
        public string Name { get; set; }
        [StringLength(50)]
        public string Type { get; set; }
        [StringLength(100)]
        public string Country { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    [Table("Route")]
    public class Route
    {
        [Key]
        public int RouteId { get; set; }
        [Required]
        public int QuotationId { get; set; }
        [StringLength(50)]
        public string Mode { get; set; }
        public int? ProviderId { get; set; }
        public int? OriginPortId { get; set; }      
        public int? DestinationPortId { get; set; }        
        public int? TSP1PortId { get; set; }
        public int? TSP2PortId { get; set; }
        public int? TSP3PortId { get; set; }       
        [StringLength(50)]
        public string Duration { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? BuyingRate { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? SellingRate { get; set; }
        [StringLength(50)]
        public string Type { get; set; }   
        [StringLength(250)]
        public string Terms { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; } = DateTime.UtcNow;
        [ForeignKey("OriginPortId")]
        public virtual Port OriginPort { get; set; }
        [ForeignKey("DestinationPortId")]
       public virtual Port DestinationPort { get; set; }
        
    }

    [Table("Attachment")]
    public class Attachment
    {
        [Key]
        public int Id { get; set; }
        public string FileType { get; set; }
        public string FileName { get; set; }
        // FK for linking with Quotation
        public int QuotationId { get; set; }

       
    }


}