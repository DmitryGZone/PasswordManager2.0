using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace PasswordManagerWeb.Controllers
{
    public class UserController : Controller
    {
        private readonly HttpClient _httpClient;

        public UserController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        // Path to C++ backend
        private const string BackendUrl = "http://localhost:8080";

        // ===================== LOGIN GET ===========================
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // ===================== LOGIN POST ==========================
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var loginData = new { username = username, password = password };
            var json = JsonConvert.SerializeObject(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync($"{BackendUrl}/login", content);

                if (!response.IsSuccessStatusCode)
                {
                    ViewData["Error"] = "Invalid username or password";
                    return View();
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(responseContent);

                if (result == null)
                {
                    ViewData["Error"] = "Invalid server response.";
                    return View();
                }

                // save token
                string token = result.token;
                HttpContext.Session.SetString("jwt", token);

                return RedirectToAction("Profile");
            }
            catch
            {
                ViewData["Error"] = "Server unavailable.";
                return View();
            }
        }

        // ===================== PROFILE =============================
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            string? token = HttpContext.Session.GetString("jwt");
            if (token == null)
                return RedirectToAction("Login");

            var req = new HttpRequestMessage(HttpMethod.Get, $"{BackendUrl}/user");
            req.Headers.Add("Authorization", $"Bearer {token}");

            try
            {
                var response = await _httpClient.SendAsync(req);

                if (!response.IsSuccessStatusCode)
                {
                    ViewData["Error"] = "Session expired, please login again.";
                    return RedirectToAction("Login");
                }

                var json = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(json);

                return View(result);
            }
            catch
            {
                ViewData["Error"] = "Server unavailable.";
                return RedirectToAction("Login");
            }
        }

        // ===================== LOGOUT ==============================
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
