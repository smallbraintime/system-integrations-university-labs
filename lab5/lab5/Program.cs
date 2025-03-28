using System.IO;
using ServiceReference;

static class Program {
	static async Task Main() {
		Console.WriteLine("My First Soap Client");
		var client = new SoapInterfaceClient();
	
		string text = await client.getHelloWorldAsStringAsync("Karol");
		Console.WriteLine(text);
	
		long days = await client.getDaysBetweenDatesAsync("01 01 2025", "05 01 2025");
		Console.WriteLine(days);
	}
}
