using System;
using System.Collections.Generic;
using Sophie.Resource.Entities;

namespace Sophie.Repository.Interface
{
    public interface IAccountRepository
    {
        //=== Account
        Account CreateAccount(Account item);
        Account RestoreAccount(Account item);
        Account DeleteAccount(string accountId);
        List<Account> ListAccount(int pageIndex = 0, int pageSize = 99);
        Account UpdateAccount(Account item);
        long TotalAccount();

        Account FindByIdAccount(string accountId);
        Account FindByEmailAccount(string email, TypeLogin? typeLogin = null);
        Account FindByPhoneAccount(string phonenumber, TypeLogin? typeLogin = null);
        Account FindByUsernameAccount(string username);
        Account FindByVideoCallIdAccount(string videoCallId);

        //=== RefreshToken
        RefreshToken CreateToken(RefreshToken refreshToken);
        RefreshToken UpdateToken(RefreshToken refreshToken);
        List<RefreshToken> FindTokenByIdAccount(string userId);

        RefreshToken FindByRefreshToken(string token);
        RefreshToken RefreshToken(string token, string deviceName, string deviceId, int dayExpiration);
        RefreshToken RevokeToken(string token);

        List<RefreshToken> ListAllToken();
        List<RefreshToken> RemoveAllToken();
        List<RefreshToken> RemoveAllToken(string accountId);
    }
}
