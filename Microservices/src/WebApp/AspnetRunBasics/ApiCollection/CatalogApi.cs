using AspnetRunBasics.ApiCollection.Infrastructure;
using AspnetRunBasics.ApiCollection.Interfaces;
using AspnetRunBasics.Models;
using AspnetRunBasics.Settings;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AspnetRunBasics.ApiCollection
{
	public class CatalogApi : BaseHttpClientWithFactory, ICatalogApi
	{
		private readonly IApiSettings _settings;

		public CatalogApi(IHttpClientFactory factory, IApiSettings settings) : base(factory)
		{
			_settings = settings;
		}

		// base address is api gateway http://localhost:7000/
		public async Task<IEnumerable<CatalogModel>> GetCatalog()
		{
			// GET http://localhost:7000/Catalog
			var message = new HttpRequestBuilder(_settings.BaseAddress)
				.SetPath(_settings.CatalogPath)
				.HttpMethod(HttpMethod.Get)
				.GetHttpMessage();
			
			return await SendRequest<IEnumerable<CatalogModel>>(message);
		}

		public async Task<CatalogModel> GetCatalog(string id)
		{
			// GET http://localhost:7000/Catalog/id
			var message = new HttpRequestBuilder(_settings.BaseAddress)
				.SetPath(_settings.CatalogPath)
				.AddToPath(id)
				.HttpMethod(HttpMethod.Get)
				.GetHttpMessage();

			return await SendRequest<CatalogModel>(message);
		}

		public async Task<IEnumerable<CatalogModel>> GetCatalogByCategory(string category)
		{
			// GET http://localhost:7000/Catalog/GetProductByCategory/{category}
			var message = new HttpRequestBuilder(_settings.BaseAddress)
				.SetPath(_settings.CatalogPath)
				.AddToPath("GetProductByCategory")
				.AddToPath(category)
				.HttpMethod(HttpMethod.Get)
				.GetHttpMessage();

			return await SendRequest<IEnumerable<CatalogModel>>(message);
		}

		public async Task<CatalogModel> CreateCatalog(CatalogModel catalogModel)
		{
			// POST http://localhost:7000/Catalog
			var message = new HttpRequestBuilder(_settings.BaseAddress)
				.SetPath(_settings.CatalogPath)
				.HttpMethod(HttpMethod.Post)
				.GetHttpMessage();

			var json = JsonConvert.SerializeObject(catalogModel);
			message.Content = new StringContent(json, Encoding.UTF8, "application/json");

			return await SendRequest<CatalogModel>(message);
		}


	}
}