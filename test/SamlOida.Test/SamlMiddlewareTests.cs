using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace SamlOida.Test
{
    [Collection("Integration")]
    public class SamlMiddlewareTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;
        public SamlMiddlewareTests()
        {
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>().UseUrls("http://localhost:50000", "https://localhost:50000"));
            _client = _server.CreateClient();
        }

        [Fact]
        public async Task Test1()
        {
            var response = await _client.GetAsync("/");

            Assert.Equal(302, (int)response.StatusCode);
            Assert.NotNull(response.Headers.Location);
        }

        [Fact]
        public async Task Test2()
        {
            var response = await _client.GetAsync("/");

            //Redirect to IdP
            var authResponse = await _client.GetAsync(response.Headers.Location);

            //Sanity check if IdP works properly
            Assert.Equal(302, (int)authResponse.StatusCode);
            Assert.Contains("SamlResponse", authResponse.Headers.Location.ToString(), StringComparison.OrdinalIgnoreCase);

            //Login Request
            var loginResponse = await _client.GetAsync(authResponse.Headers.Location);
            

            //Redirect to original URL
            var authenticatedRequest = new HttpRequestMessage(HttpMethod.Get, loginResponse.Headers.Location);
            authenticatedRequest.SetCookiesFrom(loginResponse);
            var authenticatedResponse = await _client.SendAsync(authenticatedRequest);

            Assert.Equal(200, (int)authenticatedResponse.StatusCode);
        }
    }

    public static class CookieHelper
    {
        public static IDictionary<string, string> ExtractCookies(this HttpResponseMessage response)
        {
            return response.Headers.GetValues("Set-Cookie")
                .Select(c => c.Trim().Split('='))
                .Aggregate(new Dictionary<string, string>(), (dictionary, strings) =>
                {
                    dictionary.Add(strings[0], strings[1].Substring(0, strings[1].IndexOf(';')));
                    return dictionary;
                });
        }

        public static void SetCookies(this HttpRequestMessage request, IDictionary<string, string> cookies)
        {
            request.Headers.Add("Cookie", string.Join(";", cookies.Select(d => $"{d.Key}={d.Value}")));
        }

        public static void SetCookiesFrom(this HttpRequestMessage request, HttpResponseMessage response)
        {
            var cookies = response.ExtractCookies();
            request.SetCookies(cookies);
        }
    }
}
