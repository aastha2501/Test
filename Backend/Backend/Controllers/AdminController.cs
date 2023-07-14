using DAL.Models;
using DAL.Models.DTO;
using DAL.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IProductRepo _productRepo = null;
        public AdminController(IProductRepo productRepo)
        {
            _productRepo = productRepo;
        }

        

        //add
        [HttpPost("addproduct")]
        public async Task<IActionResult> Add(ProductRequestModel model)
        {
            var product = new Product()
            {
                ProductId = Guid.NewGuid(),
                Name = model.Name,
                Quantity = model.Quantity,
                Price = model.Price,
                Description = model.Description,
                
            };
            var res = await _productRepo.AddProduct(product);
            if (res != null)
            {
                return Ok(product);
            }
            return BadRequest();
        }

        //delete
        [HttpDelete("delete/{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            bool res = await _productRepo.DeleteProduct(id);
            if (res)
            {
                return Ok(res);
            }
            return NotFound();
        }
        [HttpGet("allOrders")]
        public async Task<IActionResult> GetAllOrders()
        {
            var res = await _productRepo.GetAllUsersOrder();
            if (res != null)
            {
                return Ok(res);
            }
            return BadRequest();
        }
    }
}
