using System;
using System.Collections.Generic;
using Sophie.Resource.Entities;

namespace Sophie.Repository.Interface
{
    public interface INewsRepository
    {
        News CreateNews(News item);
        News RestoreNews(News item);
        News DeleteNews(string newsId);
        List<News> ListNews(int pageIndex = 0, int pageSize = 99);
        News UpdateNews(News item);
        long TotalNews();

        News FindByIdNews(string newsId);
    }
}
