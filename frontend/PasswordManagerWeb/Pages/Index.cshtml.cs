using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Http;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PasswordManagerWeb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public IndexModel()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://localhost:8080/");
        }

        [BindProperty]
        public string LoginUsername { get; set; }

        [BindProperty]
        public string LoginPassword { get; set; }

        [BindProperty]
        public string RegisterUsername { get; set; }

        [BindProperty]
        public string RegisterPassword { get; set; }

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public void OnGet(bool registered = false)
        {
            ErrorMessage = null;
            SuccessMessage = registered ? "Регистрация прошла успешно! Теперь вы можете войти." : null;
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostRegisterAsync()
        {
            if (string.IsNullOrWhiteSpace(RegisterUsername) || string.IsNullOrWhiteSpace(RegisterPassword))
            {
                ErrorMessage = "Пожалуйста, введите имя пользователя и пароль для регистрации.";
                return Page();
            }

            var registerData = new { username = RegisterUsername, password = RegisterPassword };
            var json = JsonConvert.SerializeObject(registerData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("register", content);
            if (response.IsSuccessStatusCode)
            {
                // Редирект с параметром для показа сообщения об успешной регистрации
                return RedirectToPage("/Index", new { registered = true });
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ErrorMessage = $"Ошибка регистрации: {response.StatusCode} {errorContent}";
                return Page();
            }
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostLoginAsync()
        {
            if (string.IsNullOrWhiteSpace(LoginUsername) || string.IsNullOrWhiteSpace(LoginPassword))
            {
                ErrorMessage = "Пожалуйста, введите имя пользователя и пароль для входа.";
                return Page();
            }

            var loginData = new { username = LoginUsername, password = LoginPassword };
            var json = JsonConvert.SerializeObject(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("login", content);
            if (response.IsSuccessStatusCode)
            {
                var responseText = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(responseText);
                string token = result?.token;

                if (!string.IsNullOrEmpty(token))
                {
                    HttpContext.Session.SetString("Token", token);
                    HttpContext.Session.SetString("Username", LoginUsername);

                    return RedirectToPage("/Dashboard");
                }
                else
                {
                    ErrorMessage = "Вход не удался: токен не получен.";
                    return Page();
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ErrorMessage = $"Ошибка входа: {response.StatusCode} {errorContent}";
                return Page();
            }
        }

        public IActionResult OnGetLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Index");
        }
    }
}
