using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ordering.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Data
{
	public class OrderContextSeed
	{
		public static async Task SeedAsync(
			OrderContext orderContext,
			ILoggerFactory loggerFactory,
			int? retry = 0)
		{
			int retryForAvailability = retry.Value;
			try
			{
				// make sure to create migrations folder in EF Core pkg mgr
				orderContext.Database.Migrate();

				if (!orderContext.Orders.Any())
				{
					orderContext.Orders.AddRange(GetPreconfiguredOrders());
					await orderContext.SaveChangesAsync();
				}
			}
			catch (Exception exception)
			{
				if (retryForAvailability < 15)
				{
					retryForAvailability++;
					var log = loggerFactory.CreateLogger<OrderContextSeed>();
					log.LogError(exception.Message);
					System.Threading.Thread.Sleep(2000);
					await SeedAsync(orderContext, loggerFactory, retryForAvailability);
				}
				throw;
			}
		}
		private static IEnumerable<Order> GetPreconfiguredOrders()
		{
			return new List<Order>()
			{
				new Order()
				{
					UserName = "Yolo",
					FirstName = "Yo",
					LastName = "Lo",
					EmailAddress = "dingdong@thewitchis.dead",
					AddressLine = "101 Dark Alley",
					Country = "US",
					State = "NY",
					City = "New York City",
					TotalPrice = 544
				},
				new Order()
				{
					UserName = "YoBroMexico",
					FirstName = "Yo",
					LastName = "BroMexico",
					EmailAddress = "dingdong@diddly.iddly",
					AddressLine = "120 Evergreen Terrace",
					Country = "US",
					State = "California",
					City = "Los Angeles",
					TotalPrice = 28
				}
			};
		}

	}
}

