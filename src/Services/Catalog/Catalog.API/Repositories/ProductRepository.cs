using Catalog.API.Data;
using Catalog.API.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catalog.API.Repositories
{
	public class ProductRepository : IProductRepository
	{

		private readonly ICatalogContext _context;

		public ProductRepository(ICatalogContext context)
		{
			// null coalescing operator "??", if left =null, use right val
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		// CRUD ops
		// Create
		public async Task CreateProduct(Product item)
		{
			await _context.Products.InsertOneAsync(item);
		}

		// Read / Retrieve
		public async Task<Product> GetProduct(string id)
		{
			return await _context.Products
				.Find(p => p.Id == id)
				.FirstOrDefaultAsync();
		}


		// Update
		public async Task<bool> UpdateProduct(Product item)
		{
			var updateResult = await _context.Products
				.ReplaceOneAsync(
					filter: g => g.Id == item.Id,
					replacement: item
				);

			return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
		}

		// Delete
		public async Task<bool> DeleteProduct(string id)
		{
			FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Id, id);

			DeleteResult deleteResult = await _context.Products
				.DeleteOneAsync(filter);

			return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
		}

		// List Ops

		public async Task<IEnumerable<Product>> GetProducts()
		{
			// LIMITING TO 5 RECORDS for testing
			int qtyOfRecords = 5;

			return await _context.Products
				.Find(p => true)
				.Limit(qtyOfRecords)
				.ToListAsync();
		}

		public async Task<IEnumerable<Product>> GetProductByName(string name)
		{
			FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Name, name);

			return await _context.Products
				.Find(filter)
				.ToListAsync();
		}

		public async Task<IEnumerable<Product>> GetProductByCategory(string categoryName)
		{
			FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Category, categoryName);

			return await _context.Products
				.Find(filter)
				.ToListAsync();
		}


	}
}
