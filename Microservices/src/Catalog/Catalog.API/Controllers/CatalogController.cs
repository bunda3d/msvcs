using Catalog.API.Entities;
using Catalog.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Catalog.API.Controllers
{	

	[Route("api/v1/[controller]")]	
	[ApiController]
	public class CatalogController : ControllerBase
	{
		//create repository object by getting it from the constructor via dependency injection (registered in startup.cs
		private readonly IProductRepository _repository;

		private readonly ILogger<CatalogController> _logger;

		public CatalogController(IProductRepository repository, ILogger<CatalogController> logger)
		{
			_repository = repository ?? throw new ArgumentNullException(nameof(repository));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		[HttpGet] //returning action result and so return status
		[ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
		public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
		{
			var products = await _repository.GetProducts();
			//Ok = 200 result
			return Ok(products);
		}

		[HttpGet("{id:length(24)}", Name = "GetProduct")] //returning action result and so return status or msg
		[ProducesResponseType((int)HttpStatusCode.NotFound)]
		[ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
		public async Task<ActionResult<Product>> GetProductById(string id) //mongo stores as object w/ string id type (not int).
		{
			var product = await _repository.GetProduct(id);
			if (product == null)
			{
				_logger.LogError($"Product with id: {id} not found.");
				return NotFound();
				//return NotFound($"Product with id: {id} not found.");
			}
			//Ok = 200 result
			return Ok(product);
		}

		[Route("[action]/{category}", Name = "GetProductByCategory")]
		[HttpGet] //returning action result and so return status
		[ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
		public async Task<ActionResult<IEnumerable<Product>>> GetProductByCategory(string category)
		{
			var products = await _repository.GetProductByCategory(category);
			//Ok = 200 result
			return Ok(products);
		}

		[HttpPost] //returning action result and so return status or msg
		[ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
		public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product) //expecting prod creation details from body, in JSON format.
		{
			await _repository.Create(product);

			//after creating item, retrieve id w/ GetProduct routing method (see above annotation)
			return CreatedAtRoute("GetProduct", new { id = product.Id }, product);
		}

		[HttpPut] //actionresult (return status, but no object this time)
		[ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
		public async Task<ActionResult> UpdateProduct([FromBody] Product product) //actionresult not returning <Product> this method.
		{
			return Ok(await _repository.Update(product));
		}

		[HttpDelete("{id:length(24)}")] //actionresult (return status, but no object this time)
		[ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
		public async Task<ActionResult> DeleteProductById(string id)
		{
			return Ok(await _repository.Delete(id));
		}
	}
}