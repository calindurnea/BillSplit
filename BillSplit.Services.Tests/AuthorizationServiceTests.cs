using BillSplit.Domain.Models;
using BillSplit.Persistence.Caching;
using BillSplit.Services.Abstractions.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;

namespace BillSplit.Services.Tests;

public class AuthorizationServiceTests
{
    private readonly AuthorizationService _sut;

    private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock = new();
    private readonly Mock<UserManager<User>> _userManagerMock = MockUserManager<User>();
    private readonly Mock<ICacheManger> _cacheMangerMock = new();
    private readonly Mock<ILogger<AuthorizationService>> _loggerMock = new();
    
    public AuthorizationServiceTests()
    {
        _sut = new AuthorizationService(
            _jwtTokenGeneratorMock.Object,
            _userManagerMock.Object,
            _cacheMangerMock.Object,
            _loggerMock.Object);
    }

    public static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
    {
        var store = new Mock<IUserStore<TUser>>();
        var mgr = new Mock<UserManager<TUser>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        mgr.Object.UserValidators.Add(new UserValidator<TUser>());
        mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());
        return mgr;
    }
    
    [Fact]
    public void CanBuild()
    {
        Assert.NotNull(_sut);
    }
}