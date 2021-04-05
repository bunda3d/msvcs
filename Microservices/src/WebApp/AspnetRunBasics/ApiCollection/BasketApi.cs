using AspnetRunBasics.ApiCollection.Infrastructure;
using AspnetRunBasics.ApiCollection.Interfaces;
using AspnetRunBasics.Models;
using AspnetRunBasics.Settings;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AspnetRunBasics.ApiCollection
{
	public class BasketApi : BaseHttpClientWithFactory, IBasketApi
	{

		private readonly IApiSettings _settings;

		public BasketApi(IHttpClientFactory factory, IApiSettings settings) : base(factory)
		{
			_settings = settings;
		}

		// base address is api gateway http://localhost:7000/
		public async Task<BasketModel> GetBasket(string userName)
		{
			// GET http://localhost:7000/Basket/{userName}
			var message = new HttpRequestBuilder(_settings.BaseAddress)
				.SetPath(_settings.BasketPath)
				.AddQueryString("username", userName)
				.HttpMethod(HttpMethod.Get)
				.GetHttpMessage();

			return await SendRequest<BasketModel>(message);
		}

		public async Task<BasketModel> UpdateBasket(BasketModel model)
		{
			// POST http://localhost:7000/Basket
			var message = new HttpRequestBuilder(_settings.BaseAddress)
				.SetPath(_settings.BasketPath)
				.HttpMethod(HttpMethod.Post)
				.GetHttpMessage();

			var json = JsonConvert.SerializeObject(model);
			message.Content = new StringContent(json, Encoding.UTF8, "application/json");

			return await SendRequest<BasketModel>(message);
		}

		public async Task CheckoutBasket(BasketCheckoutModel model)
		{
			// POST http://localhost:7000/Basket/Checkout
			var message = new HttpRequestBuilder(_settings.BaseAddress)
				.SetPath(_settings.BasketPath)
				.AddToPath("Checkout")
				.HttpMethod(HttpMethod.Post)
				.GetHttpMessage();

			var json = JsonConvert.SerializeObject(model);
			message.Content = new StringContent(json, Encoding.UTF8, "application/json");

			await SendRequest<BasketCheckoutModel>(message);
		}
	}
}
