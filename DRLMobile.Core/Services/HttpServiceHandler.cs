using DRLMobile.Core.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DRLMobile.Core.Services
{
    public static class HttpServiceHandler
    {
        public static async Task<WebServiceResponseModel> HttpPostCallWith(string uri, string username, string password, Dictionary<string, string> additionalHeaders, HttpContent content)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                Console.WriteLine("Request URI: {0}", uri);

                httpClient.Timeout = TimeSpan.FromSeconds(20);

                if (!string.IsNullOrWhiteSpace(username))
                {
                    Array byteArray = Encoding.ASCII.GetBytes("administrator:Pa55w0rd");

                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String((byte[])byteArray));
                }
                if (additionalHeaders != null)
                {
                    foreach (var header in additionalHeaders)
                    {
                        httpClient.DefaultRequestHeaders.Add("ContentType", "application/json");
                    }
                }

                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                var serverResponse = await httpClient.PostAsync(uri, content);
                var stringResponse = await serverResponse.Content.ReadAsStringAsync();

                Console.WriteLine("Server Response : {0}", stringResponse);

                WebServiceResponseModel responseModel = new WebServiceResponseModel();
                responseModel.ResponseSatusCode = serverResponse.StatusCode;
                responseModel.ServerResponse = stringResponse;

                return responseModel;
            }
        }
    }
}
