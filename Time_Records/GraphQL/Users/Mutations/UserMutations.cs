using HotChocolate.Language;
using Microsoft.AspNetCore.Identity;
using Time_Records.GraphQL.Types.Inputs;
using Time_Records.GraphQL.Users.Payloads;
using Time_Records.Models;
using Google.Apis.Auth;
using HotChocolate;
using HotChocolate.Types;
using Time_Records.DTO;
using Time_Records.Services;

// using Time_Records.Models;

namespace Time_Records.GraphQL.Users.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class UserMutations {
        // Dependency injection pro GoogleAccountService
        private readonly IGoogleAccountService _googleAccountService;

        public UserMutations(IGoogleAccountService googleAccountService) {
            _googleAccountService = googleAccountService;
        }

        /// <summary>
        /// Přihlášení přes Google token - vrátí JWT token pro aplikaci
        /// </summary>
        public async Task<GoogleLoginPayload> GoogleLogin(
            [GraphQLName("googleToken")] string importedGoogleLoginToken) {
            try {
                var loginResult = await _googleAccountService.GoogleLoginToken(importedGoogleLoginToken);
                
                return new GoogleLoginPayload {
                    GoogleLoginDto = loginResult,
                    Errors = new List<string>()
                };
            }
            catch (Exception ex) {
                Console.WriteLine($"GraphQL GoogleLogin failed: {ex.Message}");
                return new GoogleLoginPayload {
                    GoogleLoginDto = null,
                    Errors = { $"Přihlášení selhalo: {ex.Message}" }
                };
            }
        }

        /// <summary>
        /// Registrace nového uživatele přes Google token
        /// </summary>
        public async Task<AppUserPayload> RegisterUserFromGoogle(
            [GraphQLName("googleToken")] string importedGoogleLoginToken) {
            try {
                var newUser = await _googleAccountService.RegisterNewUserFromGoogleAsync(importedGoogleLoginToken);
                
                return new AppUserPayload {
                    User = newUser,
                    Errors = new List<string>()
                };
            }
            catch (Exception ex) {
                Console.WriteLine($"GraphQL RegisterUserFromGoogle failed: {ex.Message}");
                return new AppUserPayload {
                    User = null,
                    Errors = { $"Registrace selhala: {ex.Message}" }
                };
            }
        }
    }


