using Catalog.API.Entities;
using Catalog.API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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

		// Mongo BSON object Id length =24
		[HttpGet("{id:length(24)}", Name = "GetProduct")]
		[ProducesResponseType((int)HttpStatusCode.NotFound)]
		[ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
		public async Task<ActionResult<Product>> GetProductById(string id)
		{
			var item = await _repository.GetProduct(id);
			if (item == null)
			{
				_logger.LogError($"Product with id: {id} not found.");
				return NotFound();
			}
			return Ok(item);
		}

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

		[HttpPost]
		[ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
		public async Task<ActionResult<Product>> CreateProduct([FromBody] Product item)
		{
			await _repository.CreateProduct(item);

			return CreatedAtRoute("GetProduct", new { id = item.Id }, item);
		}




	}
}
