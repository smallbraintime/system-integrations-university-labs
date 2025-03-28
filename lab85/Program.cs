using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Gtk;

class Program
{
		static async Task Main()
		{

				Application.Init();
				var window = new Window("lab8.5");
				window.SetDefaultSize(400, 300);
				window.DeleteEvent += (o, args) => Application.Quit();

				var hbox = new HBox();
				window.Add(hbox);

				var jwtResponse = new TextBuffer(new TextTagTable());
				var responseValid = false;

				{
						var vbox = new VBox();
						hbox.PackStart(vbox, false, false, 5);

						var usernameEntry = new Entry { PlaceholderText = "Username..." };
						vbox.PackStart(usernameEntry, false, false, 5);

						var passwordEntry = new Entry { PlaceholderText = "Password..." };
						vbox.PackStart(passwordEntry, false, false, 5);

						var button = new Button("Log in");
						button.Clicked += async (sender, e) =>
						{
								if (usernameEntry.Text != "" && passwordEntry.Text != "")
								{
										var response = await authenticate(new AuthRequest(usernameEntry.Text.Trim(), passwordEntry.Text.Trim()));
										jwtResponse.Text = response.Response;
										responseValid = response.IsValid;
								}
						};
						vbox.PackStart(button, false, false, 5);

						var viewLabel = new Label { Text = "JWT Token", Justify = Justification.Left, LineWrap = true };
						vbox.PackStart(viewLabel, false, false, 5);

						var tokenView = new TextView(jwtResponse) { Editable = false, WrapMode = WrapMode.WordChar };
						tokenView.SetSizeRequest(300, 200);
						vbox.PackStart(tokenView, false, false, 5);
				}

				{
						var resultResponse = new TextBuffer(new TextTagTable());

						var vbox = new VBox();
						hbox.PackStart(vbox, false, false, 5);

						var userCounterButton = new Button("Get user counter");
						userCounterButton.Clicked += async (sender, e) =>
						{
								if (responseValid)
										resultResponse.Text = await getRequest("/api/users/countusers", new GetRequest(jwtResponse.Text));
						};

						vbox.PackStart(userCounterButton, false, false, 5);

						var magicNumberButton = new Button("Get magic number");
						magicNumberButton.Clicked += async (sender, e) =>
						{
								if (responseValid)
										resultResponse.Text = await getRequest("/api/number/drawnumber", new GetRequest(jwtResponse.Text));
						};
						vbox.PackStart(magicNumberButton, false, false, 5);

						var usersButton = new Button("Get users");
						usersButton.Clicked += async (sender, e) =>
						{
								if (responseValid)
										resultResponse.Text = await getRequest("/api/users/getallusers", new GetRequest(jwtResponse.Text));
						};
						vbox.PackStart(usersButton, false, false, 5);

						var viewLabel = new Label { Text = "Result", Justify = Justification.Left, LineWrap = true };
						vbox.PackStart(viewLabel, false, false, 5);

						var resultView = new TextView(resultResponse) { Editable = false, WrapMode = WrapMode.WordChar };
						resultView.SetSizeRequest(300, 200);
						vbox.PackStart(resultView, false, false, 5);
				}

				window.ShowAll();
				Application.Run();
		}

		public class AuthRequest
		{
				public AuthRequest(string name, string password)
				{
						Name = name;
						Password = password;
				}

				[JsonPropertyName("username")]
				public string Name { get; set; }
				[JsonPropertyName("password")]
				public string Password { get; set; }
		}

		public class GetRequest
		{
				public GetRequest(string jwtToken)
				{
						JwtToken = jwtToken;
				}

				public string JwtToken { get; set; }
		}

		public class AuthResponse
		{
				public AuthResponse(string response, bool isValid = true)
				{
						Response = response;
						IsValid = isValid;
				}

				public string Response { get; set; }
				public bool IsValid { get; set; }
		}

		public static async Task<AuthResponse> authenticate(AuthRequest request)
		{
				Console.WriteLine("authenticate called");
				try
				{
						var client = new HttpClient();

						string json = JsonSerializer.Serialize(request);
						var content = new StringContent(json, Encoding.UTF8, "application/json");

						var response = await client.PostAsync("http://localhost:8080/api/users/authenticate", content);
						response.EnsureSuccessStatusCode();

						using (JsonDocument doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync()))
						{
								JsonElement root = doc.RootElement;
								return new AuthResponse(root.GetProperty("token").GetString());
						}
				}
				catch (Exception ex)
				{
						return new AuthResponse(ex.ToString(), false);
				}
		}

		public static async Task<string> getRequest(string route, GetRequest request)
		{
				Console.WriteLine("getRequest called");
				try
				{
						var client = new HttpClient { BaseAddress = new Uri("http://localhost:8080") };
						client.DefaultRequestHeaders.Authorization
							= new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", request.JwtToken);

						var response = await client.GetAsync(route);
						response.EnsureSuccessStatusCode();

						return await response.Content.ReadAsStringAsync();
				}
				catch (Exception ex)
				{
						return ex.ToString();
				}
		}

}
