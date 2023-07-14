using DAL.Models;
using DAL.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public interface IProductRepo
    {
        Task<Guid> AddProduct(Product product);
        Task<bool> DeleteProduct(Guid id);
        Task<IEnumerable<Product>> GetAllProducts();
        Task<Product> GetProdById(Guid id);
        Task<Guid> SaveInvoice(Order order);
        Task<List<Product>> Update(List<Product> list);
        Task<List<UserOrdersRequestModel>> GetInvoiceByUserId(Guid id);
        Task<Guid> CancleOrder(Guid id);
        Task<List<GetAllOrders>> GetAllUsersOrder();
    }
}
