using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class DashboardModel : PageModel
{
    private readonly HttpClient _httpClient;

    public DashboardModel()
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri("http://localhost:8080/");
    }

    public string Username { get; set; } = "User";
    public List<Account>? Accounts { get; set; }
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        // Get username from session or default to "User"
        Username = HttpContext.Session.GetString("Username") ?? "User";

        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrEmpty(token))
        {
            ErrorMessage = "User is not logged in.";
            Accounts = null;
            return;
        }

        // Set authorization header with bearer token
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        try
        {
            // Request accounts list from backend API
            var response = await _httpClient.GetAsync("accounts");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Accounts = JsonConvert.DeserializeObject<List<Account>>(json);
            }
            else
            {
                ErrorMessage = $"Failed to load accounts: {response.StatusCode}";
            }
        }
        catch (HttpRequestException ex)
        {
            ErrorMessage = $"Request error: {ex.Message}";
        }
    }

    public class Account
    {
        public int Id { get; set; }
        public string AccountName { get; set; } = "";
        public string Login { get; set; } = "";
        public string Password_encrypted { get; set; } = "";
        public string Url { get; set; } = "";
        public string Notes { get; set; } = "";
    }
}
