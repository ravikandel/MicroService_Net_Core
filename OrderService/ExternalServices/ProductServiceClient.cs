using Microsoft.Extensions.Options;
using OrderService.Common;
using OrderService.DTOs;
using Polly;
using Polly.Retry;
using System.Net.Http.Headers;

namespace OrderService.ExternalServices
{
    public class ProductServiceClient(HttpClient httpClient, IOptions<ApiGatewayOptions> options, IHttpContextAccessor httpContextAccessor) : IProductServiceClient
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly string _baseUrl = options.Value.BaseUrl;
        private readonly AsyncRetryPolicy _retryPolicy = Policy
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task<ProductDto?> GetProductAsync(int productId)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {

                var url = $"{_baseUrl}/product/api/v1/Product/{productId}"; // Gateway URL

                var request = new HttpRequestMessage(HttpMethod.Get, url);

                // Extract the Bearer token from incoming request
                var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();

                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));
                }

                // Use SendAsync instead of GetFromJsonAsync to include custom headers
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<ProductDto>>();
                    return apiResponse?.StatusCode == 0 ? apiResponse.Data : null;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException(
                        $"Status Code: {(int)response.StatusCode} ({response.StatusCode}), Response: {errorContent}"
                    );
                }

            });
        }
    }
}
