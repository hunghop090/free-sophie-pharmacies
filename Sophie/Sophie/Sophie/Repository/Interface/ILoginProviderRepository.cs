using System;
using System.Threading.Tasks;
using Sophie.Resource.Model;

namespace Sophie.Repository.Interface
{
    public interface ILoginProviderRepository
    {
        Task<object> GoogleTest(string access_token, string id_token);
        Task<object> FacebookTest(string access_token);
        object AppleTest(string identityToken);

        Task<GoogleTokenResponse> VerifyGoogleToken(GoogleSocialCommand command);
        Task<FacebookTokenResponse> VerifyFacebookToken(FacebookLoginSocialCommand command);
        Task<AppleTokenResponse> VerifyAppleToken(AppleLoginSocialCommand command);
    }
}
