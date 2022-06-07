using System;
using System.Collections.Generic;
using Sophie.Resource.Entities;

namespace Sophie.Repository.Interface
{
    public interface IInforRepository
    {
        Info CreateInfo(Info item);
        Info RestoreInfo(Info item);
        Info DeleteInfo(string infoId);
        List<Info> ListInfo(int pageIndex = 0, int pageSize = 99);
        Info UpdateInfo(Info item);
        long TotalInfo();

        Info FindByIdInfo(string infoId);
        Info FindByIdAccount(string accountId);
    }
}
