using DAL.DbContextClass;
using DAL.Models;
using DAL.Models.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class ProductRepo: IProductRepo
    {
        private readonly ApplicationContext _context = null;
        
        public ProductRepo(ApplicationContext context)
        {
            _context = context; 
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            return _context.Products.ToList();
        }

        public async Task<Guid> AddProduct(Product product)
        {
            await _context.AddAsync(product);
            await _context.SaveChangesAsync();
            return product.ProductId;
        }

        public async Task<bool> DeleteProduct(Guid id)
        {
            var prod = await _context.Products.FindAsync(id);
            if (prod != null)
            {
                _context.Remove(prod);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<Product> GetProdById(Guid id)
        {
            var prod = await _context.Products.FindAsync(id);
            if(prod != null)
            {
                return prod;
            }
            return null;
        }

        public async Task<Guid> SaveInvoice(Order order)
        {
            await _context.AddAsync(order);
            await _context.SaveChangesAsync();
            return order.OrderId; 
        }

        public async Task<List<Product>> Update(List<Product> list)
        {
            var allProd = _context.Products.ToList();
            var updatedList = list.Where(x => allProd.Any(y => y.ProductId == x.ProductId));

            foreach (var item in updatedList)
            {
                var product = allProd.FirstOrDefault(y => y.ProductId == item.ProductId);
                var qty = list.FirstOrDefault(product => product.ProductId == item.ProductId);
                if (product != null && product.Quantity > 0)
                {
                    product.Quantity = product.Quantity - qty.Quantity;
                }
                else
                {
                    allProd.Remove(product);
                }
            }
            await _context.SaveChangesAsync();
            return allProd;
        }

        public async Task<List<UserOrdersRequestModel>> GetInvoiceByUserId(Guid id)
        {
            var allOrdersList = _context.Orders.Include(x=> x.OrderDetails).ThenInclude(y => y.Product).ToList();
            var userOrdersList = allOrdersList.Where((i) => Guid.Parse(i.UserId) == id).ToList();
            
            List<UserOrdersRequestModel> invoiceList = new List<UserOrdersRequestModel>();

            foreach (var order in userOrdersList)
            {
                var productIds = order.OrderDetails.Select(od => od.ProductId).ToList();

                var productDetails = _context.Products
                    .Where(p => productIds.Contains(p.ProductId));

                var userOrderProductsList = order.OrderDetails.Select(od => new UserOrderProductsRequestModel
                {
                    Quantity = od.Quantity,
                    Price = productDetails.FirstOrDefault(p => p.ProductId == od.ProductId).Price,
                    ProductName = productDetails.FirstOrDefault(p => p.ProductId == od.ProductId)?.Name,
                    
                }).ToList();

                decimal totalPrice = 0;

                foreach (var userOrderProduct in userOrderProductsList)
                {
                    decimal productPrice = userOrderProduct.Price;
                    int productQuantity = userOrderProduct.Quantity;
                    decimal productTotalPrice = productPrice * productQuantity;

                    totalPrice += productTotalPrice;
                }

                UserOrdersRequestModel userOrder = new UserOrdersRequestModel
                {
                    OrderId = order.OrderId,
                    UserOrderProductsRequestModels = userOrderProductsList,
                    TotalProducts = userOrderProductsList.Count(),
                    TotalPrice = totalPrice
                };

                invoiceList.Add(userOrder);
            }

            return invoiceList;
        }

        public async Task<Guid> CancleOrder(Guid id)
        {
            var prod = await _context.Orders.FindAsync(id);
            if (prod != null)
            {
                var orderDetails = _context.OrderDetails.Where(x => x.OrderId == id).ToList();

                foreach (var orderDetail in orderDetails)
                {
                    var productId = orderDetail.ProductId;
                    var quantity = orderDetail.Quantity;

                    var product = _context.Products.FirstOrDefault(x => x.ProductId == productId);

                    if (product != null)
                    {
                        product.Quantity += quantity;
                    }
                }

                _context.Remove(prod);
                await _context.SaveChangesAsync();
            }
            return id;
        }
    
        public async Task<List<GetAllOrders>> GetAllUsersOrder()
        {
            var allOrdersList = _context.Orders.Include(x => x.OrderDetails).ThenInclude(y => y.Product).ToList();
            List<GetAllOrders> getAllOrders = new List<GetAllOrders>();

            foreach (var order in allOrdersList)
            {   
                var productIds = order.OrderDetails.Select(od => od.ProductId).ToList();

                var productDetails = _context.Products
                    .Where(p => productIds.Contains(p.ProductId));

                var userOrderProductsList = order.OrderDetails.Select(od => new UserOrderProductsRequestModel
                {
                    Quantity = od.Quantity,
                    Price = productDetails.FirstOrDefault(p => p.ProductId == od.ProductId).Price,
                    ProductName = productDetails.FirstOrDefault(p => p.ProductId == od.ProductId)?.Name,

                }).ToList();

                decimal totalPrice = 0;

                foreach (var userOrderProduct in userOrderProductsList)
                {
                    decimal productPrice = userOrderProduct.Price;
                    int productQuantity = userOrderProduct.Quantity;
                    decimal productTotalPrice = productPrice * productQuantity;

                    totalPrice += productTotalPrice;
                }

                GetAllOrders allOrders = new GetAllOrders
                {
                    OrderId = order.OrderId,
                    UserOrderProductsRequestModels = userOrderProductsList,
                    TotalProducts = userOrderProductsList.Count(),
                    TotalPrice = totalPrice,
                    UserId = Guid.Parse(order.UserId)
                };

                getAllOrders.Add(allOrders);
            }
            return getAllOrders;
        }
    }
}
