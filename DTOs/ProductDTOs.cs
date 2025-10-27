using System.Text.Json.Serialization;


namespace Daraz_CloneAgain.DTOs

{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal OriginalPrice { get; set; }
        public string Discount { get; set; }
        public decimal Rating { get; set; }
        public int Reviews { get; set; }
        public string Badge { get; set; }
        public string Brand { get; set; }
        public string Installment { get; set; }
        public string ColorFamily { get; set; }
        public string StorageCapacity { get; set; }

        public List<ProductImageDto> Images { get; set; }
        public List<ProductColorDto> Colors { get; set; }
        public List<ProductStorageOptionDto> StorageOptions { get; set; }
        public List<ProductDeliveryDto> DeliveryOptions { get; set; }
        public ProductWarrantyDto Warranty { get; set; }
        public ProductSellerDto Seller { get; set; }
    }


    public class ProductImageDto
    {
        public int Id { get; set; }
        public string? ImageUrl { get; set; } // ✅ Now nullable
        public IFormFile? File { get; set; }
    }



    public class ProductColorDto
    {
        public int Id { get; set; }
        public string ColorName { get; set; }
        public string ColorCode { get; set; }
    }


    public class ProductStorageOptionDto
    {
        public int Id { get; set; }
        public string StorageOption { get; set; }
    }


    public class ProductDeliveryDto
    {
        public int Id { get; set; }
        public string Location { get; set; }
        public string StandardDeliveryText { get; set; }
        public string StandardDeliveryTime { get; set; }
        public string StandardDeliveryPrice { get; set; }
        public bool CashOnDelivery { get; set; }
    }


    public class ProductWarrantyDto
    {
        public int Id { get; set; }
        public string EasyReturn { get; set; }
        public string BrandWarranty { get; set; }
    }


    public class ProductSellerDto
    {
        public int Id { get; set; }
        public string SellerName { get; set; }
        public string SellerType { get; set; }
        public bool ChatAvailable { get; set; }
    }

}
