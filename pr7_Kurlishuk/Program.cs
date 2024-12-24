using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace pr7_Kurlishuk
{
    internal class Program
    {
        private static HttpClient _httpClient;
        private static string _cookie;

        static void Main(string[] args)
        {
            try
            {
                using (StreamWriter file = new StreamWriter("debug.log", true))
                {
                    Trace.Listeners.Add(new TextWriterTraceListener(file));
                    Trace.AutoFlush = true;
                    var handler = new HttpClientHandler();
                    handler.CookieContainer = new System.Net.CookieContainer();
                    _httpClient = new HttpClient(handler);
                    _cookie = await SignIn("admin", "admin");
                    Console.WriteLine($"Cookie: {_cookie}");
                    await AddRecord("Тест1", "Тест1", "https://www.permaviat.ru/_res/news_gallery/451pic.jpg");
                    string htmlCode = await GetHtmlFromUrl("http://127.0.0.1/main");
                    ParsingHtml(htmlCode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
                Trace.WriteLine($"Произошла ошибка: {ex.Message}");
            }
        }

        public static async Task<string> SignIn(string Login, string Password)
        {
            string url = "http://127.0.0.1/ajax/login.php";
            Trace.WriteLine($"Выполняем запрос: {url}");
            var formData = new Dictionary<string, string>
            {
                { "login", Login },
                { "password", Password }
            };
            var content = new FormUrlEncodedContent(formData);
            HttpResponseMessage response = await _httpClient.PostAsync(url, content);
            Trace.WriteLine($"Статус выполнения: {response.StatusCode}");
            string responseFromServer = await response.Content.ReadAsStringAsync(); 
            Console.WriteLine($"Ответ сервера: {responseFromServer}");
            if (response.Headers.TryGetValues("Set-Cookie", out var cookieValues))
            {
                string cookie = cookieValues.FirstOrDefault();
                return cookie;
            }
            return null;
        }
    }
}
