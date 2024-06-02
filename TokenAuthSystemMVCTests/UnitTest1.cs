using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;

namespace TokenAuthSystemMVCTests
{
    public class UnitTest1
    {
        private readonly HttpClient _httpClient;
        private readonly WebApplicationFactory<Program> _factory;

        public UnitTest1()
        {
            _factory = new WebApplicationFactory<Program>();
            _httpClient = _factory.CreateClient();
        }

        [Fact]
        public async Task CanAuthenticateUser()
        {
            var client = _factory.CreateClient();
            var loginData = new { Username = "testuser", Password = "Test@123" };
            var response = await client.PostAsJsonAsync("/api/auth/login", loginData);

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

            Assert.NotNull(result.Token);
        }

        [Fact]
        public async Task CanRegisterUser()
        {
            var client = _factory.CreateClient();
            var registerData = new { Username = "newuser", Password = "NewUser@123", Email = "newuser@example.com" };
            var response = await client.PostAsJsonAsync("/api/auth/register", registerData);

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<RegisterResponse>();

            Assert.True(result.Success);
        }

        [Fact]
        public async Task CanAccessProtectedResource()
        {
            var client = _factory.CreateClient();
            var loginData = new { Username = "testuser", Password = "Test@123" };
            var loginResponse = await client.PostAsJsonAsync("/api/auth/login", loginData);
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.Token);
            var response = await client.GetAsync("/api/protected/resource");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            Assert.Equal("Protected content", content);
        }

        [Fact]
        public async Task AuthenticationPerformanceTest()
        {
            var client = _factory.CreateClient();
            var tasks = new List<Task<HttpResponseMessage>>();

            for (int i = 0; i < 100; i++)
            {
                var loginData = new { Username = $"user{i}", Password = "Password@123" };
                tasks.Add(client.PostAsJsonAsync("/api/auth/login", loginData));
            }

            var responses = await Task.WhenAll(tasks);
            Assert.All(responses, response => Assert.True(response.IsSuccessStatusCode));
        }

        [Fact]
        public async Task ReliabilityTest()
        {
            var client = _factory.CreateClient();
            var loginData = new { Username = "testuser", Password = "Test@123" };

            // Simulate system failure
            // (This part is abstracted; in a real test, you might restart services or databases)

            var response = await client.PostAsJsonAsync("/api/auth/login", loginData);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            Assert.NotNull(result.Token);

            // Additional checks can be performed here after recovery
        }

        [Fact]
        public async Task SqlInjectionTest()
        {
            var client = _factory.CreateClient();
            var maliciousData = new { Username = "'; DROP TABLE Users; --", Password = "password" };
            var response = await client.PostAsJsonAsync("/api/auth/login", maliciousData);

            Assert.False(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task XssTest()
        {
            var client = _factory.CreateClient();
            var maliciousData = new { Username = "<script>alert('xss');</script>", Password = "password" };
            var response = await client.PostAsJsonAsync("/api/auth/register", maliciousData);

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            Assert.DoesNotContain("<script>alert('xss');</script>", content);
        }
    }

    class LoginResponse
    {
        public string Token;
    }

    class RegisterResponse
    {
        public string Token;
        public bool Success;
    }
}