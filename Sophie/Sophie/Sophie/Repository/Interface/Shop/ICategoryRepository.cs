using System;
using System.Collections.Generic;
using Sophie.Resource.Entities;
using Sophie.Resource.Entities.Shop;

namespace Sophie.Repository.Interface
{
    public interface ICategoryRepository
    {
        Category CreateCategory(Category item);
        Category DeleteCategory(string categoryId);
        Category FindByIdCategory(string categoryId);
        List<Category> ListCategory(int pageIndex = 0, int pageSize = 99);
        List<Category> ListCategoryActive();
        Category RestoreCategory(Category item);
        long TotalCategory();
        Category UpdateCategory(Category item);
    }
}
