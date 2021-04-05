using AspnetRunBasics.ApiCollection.Interfaces;
using AspnetRunBasics.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;

namespace AspnetRunBasics
{
	public class CheckOutModel : PageModel
	{
		private readonly IBasketApi _basketApi;
		private readonly ICatalogApi _catalogApi;

		public CheckOutModel(IBasketApi basketApi, ICatalogApi catalogApi)
		{
			_basketApi = basketApi ?? throw new ArgumentNullException(nameof(basketApi));
			_catalogApi = catalogApi ?? throw new ArgumentNullException(nameof(catalogApi));
		}

		[BindProperty]
		public BasketCheckoutModel Order { get; set; }

		public BasketModel Cart { get; set; } = new BasketModel();

		public async Task<IActionResult> OnGetAsync()
		{
			var userName = "Yolo";
			Cart = await _basketApi.GetBasket(userName);

			return Page();
		}

		public async Task<IActionResult> OnPostCheckOutAsync()
		{
			var userName = "Yolo";
			Cart = await _basketApi.GetBasket(userName);

			if (!ModelState.IsValid)
			{
				return Page();
			}

			Order.UserName = userName;
			Order.TotalPrice = Cart.TotalPrice;

			await _basketApi.CheckoutBasket(Order);

			return RedirectToPage("Confirmation", "OrderSubmitted");
		}
	}
}