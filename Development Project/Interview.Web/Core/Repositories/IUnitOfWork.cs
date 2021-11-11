using System;
namespace Interview.Web.Core.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        ProductRepository ProductRepository { get; }
        ProductAttributesRepository ProductAttributesRepository{ get; }
        CategoryRepository CategoryRepository { get; }
        int Complete();
    }
}