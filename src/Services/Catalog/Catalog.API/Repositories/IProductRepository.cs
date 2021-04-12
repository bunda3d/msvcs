using Catalog.API.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catalog.API.Repositories
{
	public interface IProductRepository
	{
		// CRUD

		Task CreateProduct(Product item);

		Task<Product> GetProduct(string id);

		Task<bool> UpdateProduct(Product item);

		Task<bool> DeleteProduct(string id);

		// Lists

		Task<IEnumerable<Product>> GetProducts();

		Task<IEnumerable<Product>> GetProductByName(string name);

		Task<IEnumerable<Product>> GetProductByCategory(string categoryName);
	}
}