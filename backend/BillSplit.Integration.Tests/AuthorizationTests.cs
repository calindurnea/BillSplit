using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mail;
using AutoFixture;
using BillSplit.Api;
using BillSplit.Contracts.Authorization;
using BillSplit.Contracts.User;

namespace BillSplit.Integration.Tests;

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
public class AuthorizationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _httpClient;
    private readonly Fixture _fixture = new();

    public AuthorizationTests(CustomWebApplicationFactory<Program> factory)
    {
        _httpClient = factory.CreateClient();
    }

    [Fact]
    public async Task CanCreateUserAndAuthorize()
    {
        // Create new user
        var user = new UpsertUserDto(_fixture.Create<MailAddress>().Address, "some name", "some phone number");
        var createUserResponse = await _httpClient.PostAsJsonAsync("/api/users", user);

        Assert.Equal(HttpStatusCode.Created, createUserResponse.StatusCode);

        var userId = createUserResponse.Headers.Location?.ToString().Split("/").Last();
        Assert.NotNull(userId);
        Assert.True(int.TryParse(userId, out var parsedUserId));

        // Set initial password
        var password = _fixture.Create<string>();
        var initialPasswordDto = new SetInitialPasswordDto(parsedUserId, password, password);
        var setInitialPasswordResponse = await _httpClient.PostAsJsonAsync("/api/authorization/password", initialPasswordDto);
        Assert.Equal(HttpStatusCode.NoContent, setInitialPasswordResponse.StatusCode);

        // Login user
        var loginRequestDto = new LoginRequestDto(user.Email, password);
        var loginResponse = await _httpClient.PostAsJsonAsync("/api/authorization/login", loginRequestDto);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var loginResponseBody = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();
        Assert.NotNull(loginResponseBody?.Token);

        // Get current user info to validate token
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResponseBody.Token);
        var currentUserResponse = await _httpClient.GetAsync("/api/users/current");
        Assert.Equal(HttpStatusCode.OK, currentUserResponse.StatusCode);

        var currentUserResponseBody = await currentUserResponse.Content.ReadFromJsonAsync<UserDto>();
        var expectedUser = new UserDto(parsedUserId, user.Email, user.Name, user.PhoneNumber);
        Assert.Equal(expectedUser, currentUserResponseBody);

        // Update user password
        var newPassword = _fixture.Create<string>();
        var updatePasswordDto = new UpdatePasswordDto(password, newPassword, newPassword);
        var updatePasswordResponse = await _httpClient.PutAsJsonAsync("/api/authorization/password", updatePasswordDto);
        Assert.Equal(HttpStatusCode.NoContent, updatePasswordResponse.StatusCode);

        // Get current user info with the old token
        var oldTokenCurrentUserResponse = await _httpClient.GetAsync("/api/users/current");
        Assert.Equal(HttpStatusCode.Forbidden, oldTokenCurrentUserResponse.StatusCode);

        // Login with new password user
        var newLoginRequestDto = new LoginRequestDto(user.Email, newPassword);
        var newLoginResponse = await _httpClient.PostAsJsonAsync("/api/authorization/login", newLoginRequestDto);
        Assert.Equal(HttpStatusCode.OK, newLoginResponse.StatusCode);

        var newLoginResponseBody = await newLoginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();
        Assert.NotNull(newLoginResponseBody?.Token);

        // Get current user info to validate token
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", newLoginResponseBody.Token);
        var newCurrentUserResponse = await _httpClient.GetAsync("/api/users/current");
        Assert.Equal(HttpStatusCode.OK, newCurrentUserResponse.StatusCode);

        var newCurrentUserResponseBody = await newCurrentUserResponse.Content.ReadFromJsonAsync<UserDto>();
        Assert.Equal(expectedUser, newCurrentUserResponseBody);
        
        // Logout
        var logoutResponse = await _httpClient.PostAsync("/api/authorization/logout", null);
        Assert.Equal(HttpStatusCode.OK, logoutResponse.StatusCode);
        
        // Get current user info with the logged out token
        var currentUserAfterLogoutResponse = await _httpClient.GetAsync("/api/users/current");
        Assert.Equal(HttpStatusCode.Forbidden, currentUserAfterLogoutResponse.StatusCode);
    }
}