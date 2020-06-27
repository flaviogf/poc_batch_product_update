using BatchProductUpdate.Api.Infrastructure;
using BatchProductUpdate.Api.ViewModels;
using System.Threading.Tasks;

namespace BatchProductUpdate.Api.Application
{
    public interface IProductApplication
    {
        public Task<Result> Update(string productCode, string gridCode, UpdateProductPriceAndQuantityViewModel viewModel);
    }
}
