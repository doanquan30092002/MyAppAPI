using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Options;
using MyApp.Core.Models;

namespace MyApp.Application.Common.Services.SendMessage
{
    public class SmsToSendMessage : ISendMessage
    {
        public SendMessageChannel Channel => SendMessageChannel.SMS;

        private readonly SmsToOptions _options;
        private readonly HttpClient _httpClient;

        public SmsToSendMessage(IOptions<SmsToOptions> options, HttpClient httpClient)
        {
            _options = options.Value;
            _httpClient = httpClient;
        }

        public async Task<bool> SendAsync(string to, string subject, string content)
        {
            var url = $"https://api.sms.to/sms/send";
            var query =
                $"api_key={HttpUtility.UrlEncode(_options.ApiKey)}"
                + $"&bypass_optout=true"
                + $"&to={HttpUtility.UrlEncode(to)}"
                + $"&message={HttpUtility.UrlEncode(content)}"
                + $"&sender_id={HttpUtility.UrlEncode(_options.SenderId)}";

            var requestUri = $"{url}?{query}";

            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var response = await _httpClient.SendAsync(request))
            {
                return response.IsSuccessStatusCode;
            }
        }
    }
}
