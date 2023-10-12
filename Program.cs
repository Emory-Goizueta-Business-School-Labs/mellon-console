// See https://aka.ms/new-console-template for more information
using System.Net.Http.Headers;
using System.Net.Http.Json;
using MellonConsole;

Console.WriteLine("Hello, World!");

string clientId = "";
string clientSecret = "";
string audience = "";
string tokenEndpoint = "";
string apiEndpoint = "";

using (HttpClient client = new HttpClient())
{
    Console.WriteLine("Acquiring access token.");

    HttpResponseMessage response = await client.PostAsJsonAsync(tokenEndpoint, new { client_id = clientId, client_secret = clientSecret, audience, grant_type = "client_credentials" });

    response.EnsureSuccessStatusCode();

    TokenResponse? tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
    string responseContent = await response.Content.ReadAsStringAsync();

    if (tokenResponse is null)
    {
        throw new Exception("Token Response is null.");
    }

    Console.WriteLine("Access token acquired.");

    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.access_token);

    Console.WriteLine("Getting To Dos.");

    List<ToDo>? toDos = await client.GetFromJsonAsync<List<ToDo>>($"{apiEndpoint}/api/ToDos");

    if (toDos is null || !toDos.Any())
    {
        Console.WriteLine("No to dos.");
        return;
    }

    Console.WriteLine("To Dos retrieved.");

    IEnumerable<ToDo> toDosToDelete = toDos.Where(_ => _.IsDone == true);

    Console.WriteLine($"{toDosToDelete.Count()} completed To Dos to delete.");

    List<Task<HttpResponseMessage>> responseTasks = new();

    foreach (ToDo toDo in toDosToDelete)
    {
        responseTasks.Add(client.DeleteAsync($"{apiEndpoint}/api/Todos/{toDo.Id}"));
    }

    Task.WaitAll(responseTasks.ToArray());

    Console.WriteLine("Completed To Dos deleted.");

    await client.PostAsJsonAsync($"{apiEndpoint}/api/ToDos", new ToDo()
    {
        Text = $"Completed To Dos cleared: {DateTime.UtcNow.ToString()}",
        IsDone = true
    });
}