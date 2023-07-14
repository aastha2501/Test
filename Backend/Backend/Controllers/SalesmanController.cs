using DAL.Models;
using DAL.Models.DTO;
using DAL.Repository;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Backend.Controllers
{
   
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User, Admin")]
    public class SalesmanController : ControllerBase
    {
        private readonly IProductRepo _productRepo = null;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public SalesmanController(IProductRepo productRepo, SignInManager<User> signInManager, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _productRepo = productRepo;
            _signInManager = signInManager;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }
        //dashboard - display all the products
        [HttpGet("/dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var list = await _productRepo.GetAllProducts();
            if (list != null)
            {
                return Ok(list);
            }
            return BadRequest();
        }

        [HttpGet("prod/{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var prod = await _productRepo.GetProdById(id);
            if (prod != null)
            {
                return Ok(prod);
            }
            return BadRequest();
        }
          

            [HttpPost("invoice")]
             
            public async Task<IActionResult> SaveInvoice(RequestProductOorder orders)
            {
            
            var userId = User.FindFirstValue(ClaimTypes.Name);

            //quantity
            var qty = orders.RequestProducts.Sum(ip => ip.Quantity);

            var order = new Order() 
            {
                OrderId = Guid.NewGuid(),
                UserId = userId,
                Quantity = qty
             
            };
            order.OrderDetails = orders.RequestProducts.Select((ip) => new OrderDetails()
            {
                OrderDetailsId = Guid.NewGuid(),
                OrderId = order.OrderId,
                ProductId = ip.ProductId,
                Quantity = ip.Quantity
                
            }).ToList();

           
            foreach (var orderDetail in order.OrderDetails)
            {
                var prodId = orderDetail.ProductId;
                var qtyy = orderDetail.Quantity;

                var allProducts = await _productRepo.GetAllProducts();

                var productToUpdate = allProducts.FirstOrDefault(p => p.ProductId == prodId);
                if (productToUpdate != null)
                {
                    productToUpdate.Quantity -= qtyy;
                }

            }

            var res = await _productRepo.SaveInvoice(order);
            if (res != null)
            {
                return Ok(res);
            }
            return BadRequest();
        }


        [HttpPost("update")]
        public async Task<IActionResult> Update(List<Product> list)
        {
            var res = await _productRepo.Update(list);
            if (res != null)
            {
                return Ok(res);
            }
            return BadRequest();
        }

        [HttpGet("invoices/{id:guid}")]
        public async Task<IActionResult> GetInvoiceByUserId(Guid id)
        {
            var res = await _productRepo.GetInvoiceByUserId(id);
            if (res != null)
            {
                return Ok(res);
            }
            return BadRequest();
        }

        [HttpDelete("deleteInvoice/{id:guid}")]
        public async Task<IActionResult> CancleOrder(Guid id)
        {
            var res = await _productRepo.CancleOrder(id);
            if (res != null)
            {
                return Ok(res);
            }
            return BadRequest();
        }
        
    }
}
