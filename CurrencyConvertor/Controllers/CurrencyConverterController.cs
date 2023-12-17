using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace CurrencyConverterAPI.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class CurrencyConverterController : ControllerBase
	{
		private readonly Dictionary<string, decimal> exchangeRates = new Dictionary<string, decimal>();

		public CurrencyConverterController()
		{
			LoadExchangeRates();
		}
	private void LoadExchangeRates()
		{
			try
			{				
				string json = System.IO.File.ReadAllText(Path.Combine("Data", "exchangeRates.json"));
				var rates = JObject.Parse(json);

				foreach (var rate in rates)
				{
					JToken? value = rate.Value;
					if(value != null)
					exchangeRates[rate.Key] = (decimal)value;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error reading exchange rates: " + ex.Message);
			}
		}

		[HttpGet("convert")]
		public IActionResult ConvertCurrency(string sourceCurrency, string targetCurrency, decimal amount)
		{
			if (string.IsNullOrWhiteSpace(sourceCurrency) || string.IsNullOrWhiteSpace(targetCurrency))
			{
				return BadRequest("Source and target currencies are required.");
			}

			string key = $"{sourceCurrency.ToUpper()}_TO_{targetCurrency.ToUpper()}";
			if (!exchangeRates.ContainsKey(key))
			{
				return BadRequest("Conversion not supported for the specified currencies.");
			}

			decimal exchangeRate = exchangeRates[key];
			decimal convertedAmount = amount * exchangeRate;

			var result = new
			{
				exchangeRate,
				convertedAmount
			};

			return Ok(result);
		}

	}
}