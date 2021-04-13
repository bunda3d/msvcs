using Catalog.API.Entities;
using Catalog.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Catalog.API.Controllers
{
	// business layer
	[ApiController]
	[Route("api/v1/[controller]")]
	public class CatalogController : ControllerBase
	{
		private readonly IProductRepository _repository;
		private readonly ILogger<CatalogController> _logger;

		public CatalogController(IProductRepository repository, ILogger<CatalogController> logger)
		{
			_repository = repository ?? throw new ArgumentNullException(nameof(repository));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		#region CRUD
		/// <summary>
		/// Creates a new item
		/// </summary>
		/// <param name="item"></param>
		/// <remarks>Create Product</remarks>
		[HttpPost(Name = "CreateProduct")]
		[ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
		public async Task<ActionResult<Product>> CreateProduct([FromBody] Product item)
		{
			await _repository.CreateProduct(item);

			return CreatedAtRoute("GetProduct", new { id = item.Id }, item);
		}

		/// <summary>
		/// Retrieves a specific item by unique id
		/// </summary>
		/// <remarks>GetProduct</remarks>
		/// <param name="id"></param>  
		/// <remarks>Get Product</remarks>
		// Mongo BSON object Id length =24
		[HttpGet("{id:length(24)}", Name = "GetProduct")]
		[ProducesResponseType((int)HttpStatusCode.NotFound)]
		[ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
		public async Task<ActionResult<Product>> GetProductById(string id)
		{
			var item = await _repository.GetProduct(id);
			if (item == null)
			{
				_logger.LogError(
					$"Product with id: Environment.NewLine" +
					$"{id} Environment.NewLine" +
					$"not found. Environment.NewLine"
					);
				return NotFound();
			}
			return Ok(item);
		}

		/// <summary>
		/// Updates item properties
		/// </summary>
		/// <param name="item"></param> 
		/// <remarks>Update Product</remarks>
		[HttpPut(Name = "UpdateProduct")]
		[ProducesResponseType((int)HttpStatusCode.NotFound)]
		[ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
		public async Task<IActionResult> UpdateProduct([FromBody] Product item)
		{
			if (item == null)
			{
				_logger.LogError($"Description: {item} not found.");
				return NotFound();
			}
			return Ok(await _repository.UpdateProduct(item));
		}

		/// <summary>
		/// Deletes a specific item by unique id
		/// </summary>
		/// <param name="id"></param>
		/// <remarks>Delete Product</remarks>
		[HttpDelete("{id:length(24)}", Name = "DeleteProduct")]
		[ProducesResponseType((int)HttpStatusCode.NotFound)]
		[ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
		public async Task<ActionResult> DeleteProductById(string id)
		{
			var item = await _repository.GetProduct(id);
			if (item == null)
			{
				_logger.LogError($"Product with id: {id} not found.");
				return NotFound();
			}
			return Ok(await _repository.DeleteProduct(id));
		}

		#endregion CRUD

		#region CRUD Lists
		/// <summary>
		/// Retrieves all items
		/// </summary>
		/// <remarks>Get Products</remarks>
		[HttpGet]
		[ProducesResponseType((int)HttpStatusCode.NotFound)]
		[ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
		public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
		{
			var items = await _repository.GetProducts();
			if (items == null)
			{
				_logger.LogError($"Products not found.");
				return NotFound();
			}
			return Ok(items);
		}

		/// <summary>
		/// Retrieves all items by name
		/// </summary>
		/// <param name="name"></param>
		/// <remarks>Get Products by Name</remarks>
		[HttpGet]
		[Route("[action]/{name}", Name = "GetProductByName")]
		[ProducesResponseType((int)HttpStatusCode.NotFound)]
		[ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
		public async Task<ActionResult<IEnumerable<Product>>> GetProductByName(string name)
		{
			var items = await _repository.GetProductByName(name);
			if (items == null)
			{
				_logger.LogError($"Products with name: {name} not found.");
				return NotFound();
			}
			return Ok(items);
		}

		/// <summary>
		/// Retrieves all items by category
		/// </summary>
		/// <param name="category"></param>
		/// <remarks>Get Products by Category</remarks>
		[Route("[action]/{category}", Name = "GetProductByCategory")]
		[HttpGet]
		[ProducesResponseType((int)HttpStatusCode.NotFound)]
		[ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
		public async Task<ActionResult<IEnumerable<Product>>> GetProductByCategory(string category)
		{
			var items = await _repository.GetProductByCategory(category);
			if (items == null)
			{
				_logger.LogError($"Products with category: {category} not found.");
				return NotFound();
			}
			return Ok(items);
		}

		#endregion CRUD Lists
	}
}