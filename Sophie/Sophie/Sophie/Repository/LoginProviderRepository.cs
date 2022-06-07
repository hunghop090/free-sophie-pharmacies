using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using App.SharedLib.Repository;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RestSharp;
using Sophie.Repository.Interface;
using Sophie.Resource.Model;
using Sophie.Units;

namespace Sophie.Repository
{
    public class LoginProviderRepository : BaseRepository, ILoginProviderRepository
    {
        public LoginProviderRepository() : base()
        {
        }

        public async Task<object> GoogleTest(string access_token, string id_token)
        {
            //https://developers.google.com/oauthplayground
            var client = new RestClient("https://www.googleapis.com");

            // v1: https://www.googleapis.com/oauth2/v1/userinfo
            var request_v1 = new RestRequest("/oauth2/v1/userinfo", Method.GET);
            request_v1.AddHeader("Authorization", "Bearer " + access_token);
            var response_v1 = await client.ExecuteAsync(request_v1);
            GoogleModel result_v1 = null;
            if (!string.IsNullOrEmpty(response_v1.Content) && response_v1.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result_v1 = JsonConvert.DeserializeObject<GoogleModel>(response_v1.Content, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
            }

            // v3: https://www.googleapis.com/oauth2/v3/tokeninfo?id_token={0}
            var request_v3 = new RestRequest("/oauth2/v3/tokeninfo", Method.GET);
            request_v3.AddQueryParameter("id_token", id_token);
            var response_v3 = await client.ExecuteAsync(request_v3);
            GoogleModel result_v3 = null;
            if (!string.IsNullOrEmpty(response_v3.Content) && response_v3.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result_v3 = JsonConvert.DeserializeObject<GoogleModel>(response_v3.Content, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
            }

            return new { result_v1, result_v3 };
        }

        public async Task<object> FacebookTest(string access_token)
        {
            //https://developers.facebook.com/tools/explorer
            var client = new RestClient("https://graph.facebook.com");

            var request_v1 = new RestRequest("/me", Method.GET);
            request_v1.AddQueryParameter("fields", "id,first_name,last_name,name,email,gender,birthday");
            request_v1.AddQueryParameter("access_token", access_token);

            var response_v1 = await client.ExecuteAsync(request_v1);
            FacebookModel result_v1 = null;
            if (!string.IsNullOrEmpty(response_v1.Content) && response_v1.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result_v1 = JsonConvert.DeserializeObject<FacebookModel>(response_v1.Content, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
            }

            return new { result_v1 };
        }

        public object AppleTest(string identityToken)
        {
            //Read the token and get it's claims using System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler
            var jwtToken = identityToken;
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = tokenHandler.ReadToken(jwtToken) as JwtSecurityToken;
            var claims = jwtSecurityToken.Claims;

            var result_v1 = new AppleModel();
            result_v1.Aud = claims.FirstOrDefault(claim => claim.Type == "aud")?.Value;
            result_v1.Auth_time = long.Parse(claims.FirstOrDefault(claim => claim.Type == "auth_time")?.Value ?? "0");
            result_v1.C_hash = claims.FirstOrDefault(claim => claim.Type == "c_hash")?.Value;
            result_v1.Exp = long.Parse(claims.FirstOrDefault(claim => claim.Type == "exp")?.Value ?? "0");
            result_v1.Email_verified = bool.Parse(claims.FirstOrDefault(claim => claim.Type == "email_verified")?.Value ?? "false");
            result_v1.Is_private_email = bool.Parse(claims.FirstOrDefault(claim => claim.Type == "is_private_email")?.Value ?? "false");
            result_v1.Sub = claims.FirstOrDefault(claim => claim.Type == "sub")?.Value;
            result_v1.Nonce = claims.FirstOrDefault(claim => claim.Type == "nonce")?.Value;
            result_v1.Nonce_supported = bool.Parse(claims.FirstOrDefault(claim => claim.Type == "nonce_supported")?.Value ?? "false");
            result_v1.Email = claims.FirstOrDefault(claim => claim.Type == "email")?.Value;
            result_v1.Iat = long.Parse(claims.FirstOrDefault(claim => claim.Type == "iat")?.Value ?? "0");

            return new { result_v1 };
        }

        public async Task<GoogleTokenResponse> VerifyGoogleToken(GoogleSocialCommand command)
        {
            var access_token = command.access_token;
            var id_token = command.id_token;

            GoogleModel result = null;
            if (!string.IsNullOrWhiteSpace(access_token))
            {
                var client = new RestClient("https://www.googleapis.com");
                var request_v1 = new RestRequest("/oauth2/v1/userinfo", Method.GET);
                request_v1.AddHeader("Authorization", "Bearer " + access_token);
                var response_v1 = await client.ExecuteAsync(request_v1);
                if (!string.IsNullOrEmpty(response_v1.Content) && response_v1.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    result = JsonConvert.DeserializeObject<GoogleModel>(response_v1.Content, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
                }
            }
            if (!string.IsNullOrWhiteSpace(id_token))
            {
                var client = new RestClient("https://www.googleapis.com");
                var request_v3 = new RestRequest("/oauth2/v3/tokeninfo", Method.GET);
                request_v3.AddQueryParameter("id_token", id_token);
                var response_v3 = await client.ExecuteAsync(request_v3);
                if (!string.IsNullOrEmpty(response_v3.Content) && response_v3.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    result = JsonConvert.DeserializeObject<GoogleModel>(response_v3.Content, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
                }
            }
            if (result == null) return null;
            result.Id = command.Profile?.Id;

            // validate
            List<string> error = new List<string>();
            result.Email = command.Profile?.Email;
            if (string.IsNullOrEmpty(result.Email))
            {
                error.Add("Email is requied");
            }
            result.Mobile = command.Profile?.Mobile;
            result.Nationality = command.Profile?.Nationality;

            var claims = new List<Claim>
            {
                new Claim("Id", result.Id ?? string.Empty),
                new Claim("Name", result.Name ?? string.Empty),
                new Claim("GivenName", result.Given_name ?? string.Empty),
                new Claim("FamilyName", result.Family_name ?? string.Empty),
                new Claim("Email", result.Email ?? string.Empty),
                new Claim("Mobile", result.Mobile ?? string.Empty),
                //new Claim("MobileCountryCode", result.MobileCountryCode ?? string.Empty)
            };

            return new GoogleTokenResponse() { Profile = result, Claims = claims, Error = error };
        }

        public async Task<FacebookTokenResponse> VerifyFacebookToken(FacebookLoginSocialCommand command)
        {
            var client = new RestClient("https://graph.facebook.com");
            var request = new RestRequest("/me", Method.GET);
            request.AddQueryParameter("fields", "id,first_name,last_name,name,email,gender,birthday");
            request.AddQueryParameter("access_token", command.access_token);

            var requestResponse = await client.ExecuteAsync(request);
            if (string.IsNullOrEmpty(requestResponse.Content) || requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return null;
            }
            FacebookModel result = JsonConvert.DeserializeObject<FacebookModel>(requestResponse.Content, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
            result.Id = result.Id ?? command.Profile?.Id;
            result.Name = result.Name ?? command.Profile?.Name;
            result.First_name = result.First_name ?? command.Profile?.First_name;
            result.Last_name = result.Last_name ?? command.Profile?.Last_name;

            // validate
            List<string> error = new List<string>();
            result.Email = command.Profile?.Email;
            if (string.IsNullOrEmpty(result.Email))
            {
                error.Add("Email is requied");
            }
            result.Mobile = command.Profile?.Mobile;
            result.Nationality = command.Profile?.Nationality;

            var claims = new List<Claim>
            {
                  new Claim("Id", result.Id ?? string.Empty),
                  new Claim("Name", result.Name ?? string.Empty),
                  new Claim("GivenName",result.First_name ?? string.Empty),
                  new Claim("FamilyName",result.Last_name ?? string.Empty),
                  new Claim("Email", result.Email ?? string.Empty),
                  new Claim("Mobile",result.Mobile ?? string.Empty),
                  //new Claim("MobileCountryCode", result.MobileCountryCode ?? string.Empty)
            };

            return new FacebookTokenResponse() { Profile = result, Claims = claims, Error = error };
        }

        public Task<AppleTokenResponse> VerifyAppleToken(AppleLoginSocialCommand command)
        {
            //Read the token and get it's claims using System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler
            var jwtToken = command.IdentityToken;
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = tokenHandler.ReadToken(jwtToken) as JwtSecurityToken;

            AppleModel result = new AppleModel();
            result.Aud = jwtSecurityToken.Claims.FirstOrDefault(claim => claim.Type == "aud")?.Value;
            result.Auth_time = long.Parse(jwtSecurityToken.Claims.FirstOrDefault(claim => claim.Type == "auth_time")?.Value ?? "0");
            result.C_hash = jwtSecurityToken.Claims.FirstOrDefault(claim => claim.Type == "c_hash")?.Value;
            result.Exp = long.Parse(jwtSecurityToken.Claims.FirstOrDefault(claim => claim.Type == "exp")?.Value ?? "0");
            result.Email_verified = bool.Parse(jwtSecurityToken.Claims.FirstOrDefault(claim => claim.Type == "email_verified")?.Value ?? "false");
            result.Is_private_email = bool.Parse(jwtSecurityToken.Claims.FirstOrDefault(claim => claim.Type == "is_private_email")?.Value ?? "false");
            result.Sub = jwtSecurityToken.Claims.FirstOrDefault(claim => claim.Type == "sub")?.Value;
            result.Nonce = jwtSecurityToken.Claims.FirstOrDefault(claim => claim.Type == "nonce")?.Value;
            result.Nonce_supported = bool.Parse(jwtSecurityToken.Claims.FirstOrDefault(claim => claim.Type == "nonce_supported")?.Value ?? "false");
            result.Email = jwtSecurityToken.Claims.FirstOrDefault(claim => claim.Type == "email")?.Value;
            result.Iat = long.Parse(jwtSecurityToken.Claims.FirstOrDefault(claim => claim.Type == "iat")?.Value ?? "0");

            result.Sub = command.Profile?.Sub;
            result.Name = result.Name ?? command.Profile?.Name;
            result.Given_name = result.Given_name ?? command.Profile?.Given_name;
            result.Family_name = result.Family_name ?? command.Profile?.Family_name;

            // validate
            List<string> error = new List<string>();
            result.Email = command.Profile?.Email;
            if (string.IsNullOrEmpty(result.Email))
            {
                error.Add("Email is requied");
            }
            result.Mobile = command.Profile?.Mobile;
            result.Nationality = command.Profile?.Nationality;

            var claims = new List<Claim>
            {
                new Claim("Id", result.Sub ?? string.Empty),
                new Claim("Name", $"{result.Given_name ?? string.Empty} {result.Family_name ?? string.Empty}"),
                new Claim("GivenName", result.Given_name ?? string.Empty),
                new Claim("FamilyName", result.Family_name ?? string.Empty),
                new Claim("Email", result.Email ?? string.Empty),
                new Claim("Mobile", result.Mobile ?? string.Empty),
                //new Claim("MobileCountryCode", result.MobileCountryCode ?? string.Empty)
            };

            return Task.FromResult(new AppleTokenResponse() { Profile = result, Claims = claims, Error = error });
        }
    }
}
