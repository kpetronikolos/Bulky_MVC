﻿namespace BulkyBook.DataAccess.Repository.IRepository
{
     public interface IUnitOfWork
     {
          ICategoryRepository Category { get; }
          IProductRepository Product { get; }
          ICompanyRepository Company { get; }
          IShoppingCartRepository ShoppingCart { get; }

          void Save();
     }
}
