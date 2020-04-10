using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Saper.ApiAccess
{
    public class ApiRequests
    {
        public static readonly HttpClient client = HttpClientFactory.Create();

        static ApiRequests()
        {            
            if (!string.IsNullOrEmpty(Properties.Settings.Default.Token))
                client.DefaultRequestHeaders.Add("Auth-token", Properties.Settings.Default.Token);
        }

        public static async Task<bool> Post_RankRecord( string nickName, float Time, int Level)
        {
            RankRecord newRecord = new RankRecord(nickName, Time, Level);

            var url = "https://localhost:44358/api/rank";
            HttpResponseMessage httpResponse = await client.PostAsJsonAsync(url, newRecord);
            try { httpResponse.EnsureSuccessStatusCode(); }
            catch { return false; }

            return true;
        }

        public static async Task<RankRecord[]> Get_Ranking()
        {
            RankRecord[] rankArray;
            var url = "https://localhost:44358/api/rank";
            HttpResponseMessage httpResponse;
            try { httpResponse = await client.GetAsync(url); }
            catch { return null; }

            if (httpResponse.StatusCode == HttpStatusCode.OK)
            {
                var content = httpResponse.Content;
                rankArray = await content.ReadAsAsync<RankRecord[]>();
                rankArray.OrderBy(rR => rR.Time).ToArray();

                return rankArray;
            }
            else { return null; }
        }

        public static async Task<HttpResponseMessage> Login(LoginForm loginForm)
        {
            var url = "https://localhost:44358/api/Account/login";
            try { return await client.PostAsJsonAsync<LoginForm>(url, loginForm); }
            catch { return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable); }            
        }

        public static async Task<HttpResponseMessage> Register(RegisterForm registerForm)
        {
            var url = "https://localhost:44358/api/Account/register";
            try { return await client.PostAsJsonAsync<RegisterForm>(url, registerForm); }
            catch { return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable); }
        }

        public static async Task<HttpResponseMessage> TEST()
        {
            var url = "https://localhost:44358/api/Account/TEST";
            try
            {
                var asd = new { value = 5, nick = "asdd" };
                return await client.PostAsJsonAsync(url, asd);
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
            }

        }
    }
}
