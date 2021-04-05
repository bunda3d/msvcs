using Basket.API.Repositories.Interfaces;
using Basket.API.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using EventBusRabbitMQ.Events;
using EventBusRabbitMQ.Producer;
using EventBusRabbitMQ.Common;

namespace Basket.API.Controllers
{
	[Route("api/v1/[controller]")]
	[ApiController]
	public class BasketController : ControllerBase
	{
		private readonly IBasketRepository _repository;
		private readonly IMapper _mapper;
		private readonly EventBusRabbitMQProducer _eventBus;

		public BasketController(IBasketRepository repository, IMapper mapper, EventBusRabbitMQProducer eventBus)
		{
			_repository = repository ?? throw new ArgumentNullException(nameof(repository));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
			_eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
		}

		[HttpGet]
		[ProducesResponseType(typeof(BasketCart), (int)HttpStatusCode.OK)]
		public async Task<ActionResult<BasketCart>> GetBasket(string userName)
		{
			var basket = await _repository.GetBasket(userName);
			//null check for empty cart, if so => return new empty cart in user's name
			return Ok(basket ?? new BasketCart(userName));
		}

		[HttpPost]
		[ProducesResponseType(typeof(BasketCart), (int)HttpStatusCode.OK)]
		public async Task<ActionResult<BasketCart>> UpdateBasket([FromBody] BasketCart basket)
		{
			return Ok(await _repository.UpdateBasket(basket));
		}

		[HttpDelete("{userName}")]
		[ProducesResponseType(typeof(BasketCart), (int)HttpStatusCode.OK)]
		public async Task<ActionResult> DeleteBasket(string userName)
		{
			return Ok(await _repository.DeleteBasket(userName));
		}

		[Route("[action]")]
		[HttpPost]
		[ProducesResponseType((int)HttpStatusCode.Accepted)]
		[ProducesResponseType((int)HttpStatusCode.BadRequest)]
		public async Task<ActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
		{
			// get total price of basket
			var basket = await _repository.GetBasket(basketCheckout.UserName);
			if (basket == null)
			{
				return BadRequest();
			}

			// remove the basket
			var basketRemoved = await _repository.DeleteBasket(basket.UserName);
			if (!basketRemoved)
			{
				return BadRequest();
			}

			// send checkout event to RabbitMQ
			var eventMessage = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
			eventMessage.RequestId = Guid.NewGuid();
			eventMessage.TotalPrice = basket.TotalPrice;
			try
			{
				_eventBus.PublishBasketCheckout(EventBusConstants.BasketCheckoutQueue, eventMessage);
			}
			catch (Exception)
			{
				throw;
			}

			return Accepted();

		}
	}
}
