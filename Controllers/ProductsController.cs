using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsAPI.DTO;
using ProductsAPI.Models;

namespace ProductsAPI.Controllers
{
    // localhost:5000/api/products
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController:ControllerBase
    {
        private readonly ProductsContext _context;

        public ProductsController(ProductsContext context)
        {
            _context=context;
        }

        // localhost:5000/api/products => GET
        [HttpGet]
        public async Task<IActionResult> GetProducts()// verileri getirme
        {
            var products=  await _context.Products.Where(i => i.IsActive).Select(p => ProductToDTO(p)).ToListAsync();
            
            return Ok(products);
        }

        // localhost:5000/api/products/1 => GET
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int? id)// id si verilen veriyi getirme
        {   
            if(id == null)
            {
                return NotFound();
            }

            var p = await _context.Products.Where(i=> i.ProductId==id).Select(p => ProductToDTO(p)).FirstOrDefaultAsync();

            if(p == null)
            {
                return NotFound(); //404 durum kodu gonderir
            }

            return Ok(p);
        }

         [HttpPost]
        public async Task<IActionResult> CreateProduct(Product entity) // veri ekleme
        {
            _context.Products.Add(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct),new { id = entity.ProductId }, entity);//201 durum kodu gonderir icerdeki parametrede ise product eklendikten sonra getproduct actionuna git dedik id si ile birlikte
        }


        // localhost:5000/api/products/5 => PUT
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product entity) //veri guncelleme
        {
            if(id != entity.ProductId)
            {
                return BadRequest();// 400 durum kodu gonderir
            }

            var product = await _context.Products.FirstOrDefaultAsync(i => i.ProductId == id);

            if(product == null)
            {
                return NotFound();
            }

            product.ProductName = entity.ProductName;
            product.Price = entity.Price;
            product.IsActive = entity.IsActive;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch(Exception)
            {
                return NotFound();
            }

            return NoContent();//204 durum kodu gonderir (guncelleme basarili)
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int? id) //veri silme
        {
            if(id == null)
            {
                return  NotFound();
            }

            var product = await _context.Products.FirstOrDefaultAsync(i => i.ProductId == id);

            if(product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch(Exception)
            {
                return NotFound();
            }
            return NoContent();//204 durum kodu gonderir (silme basarili)
        }

        private static ProductDTO ProductToDTO(Product p)
        {
            var entity = new ProductDTO();
            if(p != null) 
            {
                entity.ProductId = p.ProductId;
                entity.ProductName = p.ProductName;
                entity.Price = p.Price;
            }
            return entity;
        }
    }
}