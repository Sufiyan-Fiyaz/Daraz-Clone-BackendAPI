using AutoMapper;
using Daraz_CloneAgain.DTOs;
using Daraz_CloneAgain.Models;
using Daraz_CloneAgain.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Daraz_Clone.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ProductController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/product
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
        {
            var products = await _context.Products
                .Include(p => p.Images)
                .Include(p => p.Colors)
                .Include(p => p.StorageOptions)
                .Include(p => p.DeliveryOptions)
                .Include(p => p.Warranty)
                .Include(p => p.Seller)
                .ToListAsync();

            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);

            // Ensure image URLs are properly formatted for frontend consumption
            foreach (var productDto in productDtos)
            {
                if (productDto.Images != null)
                {
                    foreach (var image in productDto.Images)
                    {
                        // Make sure URLs are properly formatted
                        if (!string.IsNullOrEmpty(image.ImageUrl) && !image.ImageUrl.StartsWith("http"))
                        {
                            // Ensure the URL starts with / for relative paths
                            if (!image.ImageUrl.StartsWith("/"))
                                image.ImageUrl = "/" + image.ImageUrl;
                        }
                    }
                }
            }

            return Ok(productDtos);
        }

        // GET: api/product/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetById(int id)
        {
            var product = await _context.Products
                .Include(p => p.Images)
                .Include(p => p.Colors)
                .Include(p => p.StorageOptions)
                .Include(p => p.DeliveryOptions)
                .Include(p => p.Warranty)
                .Include(p => p.Seller)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound($"Product with ID {id} not found.");

            var productDto = _mapper.Map<ProductDto>(product);

            // Ensure image URLs are properly formatted
            if (productDto.Images != null)
            {
                foreach (var image in productDto.Images)
                {
                    if (!string.IsNullOrEmpty(image.ImageUrl) && !image.ImageUrl.StartsWith("http"))
                    {
                        if (!image.ImageUrl.StartsWith("/"))
                            image.ImageUrl = "/" + image.ImageUrl;
                    }
                }
            }

            return Ok(productDto);
        }

        // POST: api/product
        [HttpPost]
        public async Task<ActionResult<ProductDto>> Create([FromForm] ProductDto productDto)
        {
            try
            {
                // Handle image uploads
                var imageDtos = new List<ProductImageDto>();

                if (productDto.Images != null)
                {
                    // Ensure the directory exists
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products");
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    foreach (var img in productDto.Images)
                    {
                        if (img.File != null && img.File.Length > 0)
                        {
                            // Validate file type
                            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                            var fileExtension = Path.GetExtension(img.File.FileName).ToLowerInvariant();

                            if (!allowedExtensions.Contains(fileExtension))
                            {
                                return BadRequest($"File type {fileExtension} is not allowed. Allowed types: {string.Join(", ", allowedExtensions)}");
                            }

                            // Validate file size (5MB max)
                            if (img.File.Length > 5 * 1024 * 1024)
                            {
                                return BadRequest("File size cannot exceed 5MB.");
                            }

                            // Generate unique filename
                            var fileName = Guid.NewGuid() + fileExtension;
                            var filePath = Path.Combine(uploadPath, fileName);

                            // Save file to disk
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await img.File.CopyToAsync(stream);
                            }

                            // Add to list with the generated URL
                            imageDtos.Add(new ProductImageDto
                            {
                                Id = 0, // Will be set by database
                                ImageUrl = "/images/products/" + fileName
                            });
                        }
                    }
                }

                // Replace the images with the processed ones
                productDto.Images = imageDtos;

                var product = _mapper.Map<Product>(productDto);

                // Map other child collections
                if (productDto.Colors != null)
                    product.Colors = _mapper.Map<List<ProductColors>>(productDto.Colors);
                if (productDto.StorageOptions != null)
                    product.StorageOptions = _mapper.Map<List<ProductStorageOptions>>(productDto.StorageOptions);
                if (productDto.DeliveryOptions != null)
                    product.DeliveryOptions = _mapper.Map<List<ProductDelivery>>(productDto.DeliveryOptions);
                if (productDto.Warranty != null)
                    product.Warranty = _mapper.Map<ProductWarranty>(productDto.Warranty);
                if (productDto.Seller != null)
                    product.Seller = _mapper.Map<ProductSeller>(productDto.Seller);

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                var createdDto = _mapper.Map<ProductDto>(product);
                return CreatedAtAction(nameof(GetById), new { id = createdDto.Id }, createdDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/product/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] ProductDto updatedDto)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.Images)
                    .Include(p => p.Colors)
                    .Include(p => p.StorageOptions)
                    .Include(p => p.DeliveryOptions)
                    .Include(p => p.Warranty)
                    .Include(p => p.Seller)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (product == null) return NotFound($"Product with ID {id} not found.");

                // Handle image updates
                if (updatedDto.Images != null)
                {
                    // Delete old image files from disk
                    foreach (var oldImage in product.Images)
                    {
                        if (!string.IsNullOrEmpty(oldImage.ImageUrl))
                        {
                            var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", oldImage.ImageUrl.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }
                    }

                    // Remove old images from database
                    _context.ProductImages.RemoveRange(product.Images);

                    // Process new images
                    var newImageDtos = new List<ProductImageDto>();
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products");
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    foreach (var img in updatedDto.Images)
                    {
                        if (img.File != null && img.File.Length > 0)
                        {
                            // Validate file type
                            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                            var fileExtension = Path.GetExtension(img.File.FileName).ToLowerInvariant();

                            if (!allowedExtensions.Contains(fileExtension))
                            {
                                return BadRequest($"File type {fileExtension} is not allowed. Allowed types: {string.Join(", ", allowedExtensions)}");
                            }

                            // Validate file size (5MB max)
                            if (img.File.Length > 5 * 1024 * 1024)
                            {
                                return BadRequest("File size cannot exceed 5MB.");
                            }

                            // Generate unique filename
                            var fileName = Guid.NewGuid() + fileExtension;
                            var filePath = Path.Combine(uploadPath, fileName);

                            // Save file to disk
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await img.File.CopyToAsync(stream);
                            }

                            newImageDtos.Add(new ProductImageDto
                            {
                                Id = 0,
                                ImageUrl = "/images/products/" + fileName
                            });
                        }
                        else if (!string.IsNullOrEmpty(img.ImageUrl))
                        {
                            // Keep existing image (no new file uploaded)
                            newImageDtos.Add(img);
                        }
                    }

                    updatedDto.Images = newImageDtos;
                    product.Images = _mapper.Map<List<ProductImages>>(updatedDto.Images);
                }

                // Map main properties (excluding images as we handled them above)
                updatedDto.Id = id; // Ensure ID matches
                _mapper.Map(updatedDto, product);

                // Update other child collections
                if (updatedDto.Colors != null)
                {
                    _context.ProductColors.RemoveRange(product.Colors);
                    product.Colors = _mapper.Map<List<ProductColors>>(updatedDto.Colors);
                }
                if (updatedDto.StorageOptions != null)
                {
                    _context.ProductStorageOptions.RemoveRange(product.StorageOptions);
                    product.StorageOptions = _mapper.Map<List<ProductStorageOptions>>(updatedDto.StorageOptions);
                }
                if (updatedDto.DeliveryOptions != null)
                {
                    _context.ProductDelivery.RemoveRange(product.DeliveryOptions);
                    product.DeliveryOptions = _mapper.Map<List<ProductDelivery>>(updatedDto.DeliveryOptions);
                }
                if (updatedDto.Warranty != null)
                {
                    if (product.Warranty != null)
                        _mapper.Map(updatedDto.Warranty, product.Warranty);
                    else
                        product.Warranty = _mapper.Map<ProductWarranty>(updatedDto.Warranty);
                }
                if (updatedDto.Seller != null)
                {
                    if (product.Seller != null)
                        _mapper.Map(updatedDto.Seller, product.Seller);
                    else
                        product.Seller = _mapper.Map<ProductSeller>(updatedDto.Seller);
                }

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/product/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.Images)
                    .Include(p => p.Colors)
                    .Include(p => p.StorageOptions)
                    .Include(p => p.DeliveryOptions)
                    .Include(p => p.Warranty)
                    .Include(p => p.Seller)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (product == null) return NotFound($"Product with ID {id} not found.");

                // Delete image files from disk before removing from database
                if (product.Images != null && product.Images.Any())
                {
                    foreach (var image in product.Images)
                    {
                        if (!string.IsNullOrEmpty(image.ImageUrl))
                        {
                            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", image.ImageUrl.TrimStart('/'));
                            if (System.IO.File.Exists(imagePath))
                            {
                                try
                                {
                                    System.IO.File.Delete(imagePath);
                                }
                                catch (Exception ex)
                                {
                                    // Log the error but don't fail the entire operation
                                    Console.WriteLine($"Warning: Could not delete image file {imagePath}: {ex.Message}");
                                }
                            }
                        }
                    }
                }

                // Remove child collections first (EF Core will handle this due to relationships, but being explicit)
                if (product.Images.Any())
                    _context.ProductImages.RemoveRange(product.Images);
                if (product.Colors.Any())
                    _context.ProductColors.RemoveRange(product.Colors);
                if (product.StorageOptions.Any())
                    _context.ProductStorageOptions.RemoveRange(product.StorageOptions);
                if (product.DeliveryOptions.Any())
                    _context.ProductDelivery.RemoveRange(product.DeliveryOptions);
                if (product.Warranty != null)
                    _context.ProductWarranty.Remove(product.Warranty);
                if (product.Seller != null)
                    _context.ProductSeller.Remove(product.Seller);

                // Finally remove the product
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Helper method to get image file path
        private string GetImageFilePath(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return null;

            var relativePath = imageUrl.TrimStart('/');
            return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);
        }

        // Optional: GET method to serve images directly (if not using static files middleware)
        [HttpGet("image/{fileName}")]
        public IActionResult GetImage(string fileName)
        {
            try
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products", fileName);

                if (!System.IO.File.Exists(imagePath))
                {
                    return NotFound("Image not found.");
                }

                var imageBytes = System.IO.File.ReadAllBytes(imagePath);
                var contentType = GetContentType(fileName);

                return File(imageBytes, contentType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving image: {ex.Message}");
            }
        }

        // ProductsController.cs

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Ok(new List<ProductDto>());

            var products = await _context.Products
                .Include(p => p.Images)
                .Include(p => p.Colors)
                .Include(p => p.StorageOptions)
                .Include(p => p.DeliveryOptions)
                .Include(p => p.Warranty)
                .Include(p => p.Seller)
                    .Where(p =>
        EF.Functions.Like(EF.Functions.Collate(p.Title, "SQL_Latin1_General_CP1_CI_AS"), $"%{query}%") ||
        EF.Functions.Like(EF.Functions.Collate(p.Description, "SQL_Latin1_General_CP1_CI_AS"), $"%{query}%")
    )

                .Take(10) // max 10 results for performance
                .ToListAsync();

            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);

            // Ensure image URLs are properly formatted
            foreach (var productDto in productDtos)
            {
                if (productDto.Images != null)
                {
                    foreach (var image in productDto.Images)
                    {
                        if (!string.IsNullOrEmpty(image.ImageUrl) && !image.ImageUrl.StartsWith("http"))
                        {
                            if (!image.ImageUrl.StartsWith("/"))
                                image.ImageUrl = "/" + image.ImageUrl;
                        }
                    }
                }
            }

            return Ok(productDtos);
        }


        // Helper method to determine content type based on file extension
        private string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }
    }
}