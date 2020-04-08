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

        public async Task<bool> Post_RankRecord(string NickName, float Time, int Level)
        {
            RankRecord newRecord = new RankRecord(NickName, Time, Level);

            var url = "https://localhost:44358/api/rank";
            HttpResponseMessage httpResponse = await client.PostAsJsonAsync<RankRecord>(url, newRecord);
            try { httpResponse.EnsureSuccessStatusCode(); }
            catch { return false; }

            return true;
        }

        public async Task<RankRecord[]> Get_Ranking()
        {
            RankRecord[] rankArray;
            var url = "https://localhost:44358/api/rank";
            HttpResponseMessage httpResponse = await client.GetAsync(url);

            if (httpResponse.StatusCode == HttpStatusCode.OK)
            {
                var content = httpResponse.Content;
                rankArray = await content.ReadAsAsync<RankRecord[]>();
                rankArray.OrderBy(rR => rR.Time).ToArray();

                return rankArray;
            }
            else { return null; }
        }

        public async Task<RankRecord[]> Login(string nickName, string password)
        {

        }


    }
}
