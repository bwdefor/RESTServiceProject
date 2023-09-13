using System;
using System.Text.Json.Serialization;

namespace RESTServiceProject.Models
{
	public class User
	{
		[JsonPropertyName("id")]
		public int id { get; set; }
		[JsonPropertyName("email")]
		public string? email { get; set; }
		[JsonPropertyName("userPassword")]
		public string? password { get; set; }
		[JsonPropertyName("date_added")]
		public DateTime dateAdded { get; set; }
	}
}
