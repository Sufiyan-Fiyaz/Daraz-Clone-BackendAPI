using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Products")]
public class Product
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("title")]
    public string Title { get; set; }

    [Column("description")]
    public string Description { get; set; }

    [Column("current_price")]
    public decimal CurrentPrice { get; set; }

    [Column("original_price")]
    public decimal OriginalPrice { get; set; }

    [Column("discount")]
    public string Discount { get; set; }

    [Column("rating")]
    public decimal Rating { get; set; }

    [Column("reviews")]
    public int Reviews { get; set; }

    [Column("badge")]
    public string Badge { get; set; }

    [Column("brand")]
    public string Brand { get; set; }

    [Column("installment")]
    public string Installment { get; set; }

    [Column("color_family")]
    public string ColorFamily { get; set; }

    [Column("storage_capacity")]
    public string StorageCapacity { get; set; }

    // Navigation properties
    public ICollection<ProductImages> Images { get; set; }
    public ICollection<ProductColors> Colors { get; set; }
    public ICollection<ProductStorageOptions> StorageOptions { get; set; }
    public ICollection<ProductDelivery> DeliveryOptions { get; set; }
    public ProductWarranty Warranty { get; set; }
    public ProductSeller Seller { get; set; }
}


[Table("ProductImages")]
public class ProductImages
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("image_url")]
    public string ImageUrl { get; set; }

    public Product Product { get; set; }
}


[Table("ProductColors")]
public class ProductColors
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("color_name")]
    public string ColorName { get; set; }

    [Column("color_code")]
    public string ColorCode { get; set; }

    public Product Product { get; set; }
}

[Table("ProductStorageOptions")]
public class ProductStorageOptions
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("storage_option")]
    public string StorageOption { get; set; }

    public Product Product { get; set; }
}


[Table("ProductDelivery")]
public class ProductDelivery
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("location")]
    public string Location { get; set; }

    [Column("standard_delivery_text")]
    public string StandardDeliveryText { get; set; }

    [Column("standard_delivery_time")]
    public string StandardDeliveryTime { get; set; }

    [Column("standard_delivery_price")]
    public string StandardDeliveryPrice { get; set; }

    [Column("cash_on_delivery")]
    public bool CashOnDelivery { get; set; }

    public Product Product { get; set; }
}


[Table("ProductWarranty")]
public class ProductWarranty
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("easy_return")]
    public string EasyReturn { get; set; }

    [Column("brand_warranty")]
    public string BrandWarranty { get; set; }

    public Product Product { get; set; }
}


[Table("ProductSeller")]
public class ProductSeller
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("seller_name")]
    public string SellerName { get; set; }

    [Column("seller_type")]
    public string SellerType { get; set; }

    [Column("chat_available")]
    public bool ChatAvailable { get; set; }

    public Product Product { get; set; }
}
