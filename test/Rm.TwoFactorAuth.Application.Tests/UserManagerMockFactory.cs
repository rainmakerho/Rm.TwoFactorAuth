using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace Rm.TwoFactorAuth;

public static class UserManagerMockFactory
{
    public static Mock<UserManager<IdentityUser>> Create()
    {
        var store = new Mock<IUserStore<IdentityUser>>();
        var options = Options.Create(new IdentityOptions());
        var passwordHasher = new Mock<IPasswordHasher<IdentityUser>>();
        var userValidators = new List<IUserValidator<IdentityUser>>();
        var pwdValidators = new List<IPasswordValidator<IdentityUser>>();
        var keyNormalizer = new Mock<ILookupNormalizer>();
        var errors = new IdentityErrorDescriber();
        var services = new Mock<IServiceProvider>();
        var logger = new Mock<ILogger<UserManager<IdentityUser>>>();

        return new Mock<UserManager<IdentityUser>>(
            store.Object,
            options,
            passwordHasher.Object,
            userValidators,
            pwdValidators,
            keyNormalizer.Object,
            errors,
            services.Object,
            logger.Object
        );
    }
}
