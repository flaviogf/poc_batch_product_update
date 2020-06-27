using BatchProductUpdate.Api.Infrastructure;
using BatchProductUpdate.Api.Models;
using System.Threading.Tasks;

namespace BatchProductUpdate.Api.Repositories
{
    public interface IProductRepository
    {
        Task<Result> Save(string productCode, Product product);

        Task<Result> Save(string productCode, string gridCode, Grid grid);

        Task<Maybe<Product>> FindOne(string productCode);

        Task<Maybe<Grid>> FindOne(string productCode, string gridCode);
    }
}
