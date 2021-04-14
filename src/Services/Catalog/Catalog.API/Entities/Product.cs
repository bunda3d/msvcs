using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Catalog.API.Entities
{
	public class Product
	{
		/// <example>6073ca0bd932aa3291eb524f</example>
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }
		/// <example>Surface Go</example>
		public string Name { get; set; }
		/// <example>Tablets</example>
		public string Category { get; set; }
		/// <example>10.5in touchscreen tablet with keyboard and stylus</example>
		public string Summary { get; set; }
		/// <example>Lorem ipsum dolor sit amet, consectetur adipisicing elit. Ut, tenetur natus doloremque laborum quos iste ipsum rerum obcaecati impedit odit illo dolorum ab tempora nihil dicta earum fugiat. Temporibus, voluptatibus. Lorem ipsum dolor sit amet, consectetur adipisicing elit.</example>
		public string Description { get; set; }
		/// <example>product-1.png</example>
		public string ImageFile { get; set; }
		/// <example>490.99</example>
		public decimal Price { get; set; }
	}
}