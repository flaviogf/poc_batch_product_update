using BatchProductUpdate.Api.Infrastructure;
using BatchProductUpdate.Api.Models;
using BatchProductUpdate.Api.Repositories;
using BatchProductUpdate.Api.ViewModels;
using System.Threading.Tasks;

namespace BatchProductUpdate.Api.Application
{
    public class DefaultProductApplication : IProductApplication
    {
        private readonly IProductRepository _productRepository;

        public DefaultProductApplication(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Result> Update(string productCode, string gridCode, UpdateProductPriceAndQuantityViewModel viewModel)
        {
            if (string.IsNullOrEmpty(productCode))
            {
                return Result.Fail("Product code must be informed");
            }

            var maybeProduct = await _productRepository.FindOne(productCode);

            if (maybeProduct.HasNoValue)
            {
                return Result.Fail($"Product with {productCode} does not exist");
            }

            var product = maybeProduct.Value;

            if (!product.HasGrid)
            {
                UpdatePrice(product, viewModel.Price);
                UpdateQuantity(product, viewModel.Quantity);

                return await _productRepository.Save(productCode, product);
            }

            if (string.IsNullOrEmpty(gridCode))
            {
                return Result.Fail("Grid code must be informed");
            }

            var maybeGrid = await _productRepository.FindOne(productCode, gridCode);

            if (maybeGrid.HasNoValue)
            {
                return Result.Fail($"Grid with {gridCode} does not exist");
            }

            var grid = maybeGrid.Value;

            UpdatePrice(grid, viewModel.Price);
            UpdateQuantity(grid, viewModel.Quantity);

            return await _productRepository.Save(productCode, gridCode, grid);
        }

        private void UpdatePrice(Product product, decimal? price)
        {
            if (!price.HasValue)
            {
                return;
            }

            if (price.Value <= 0)
            {
                return;
            }

            product.Price = price.Value;
        }

        private void UpdateQuantity(Product product, int? quantity)
        {
            if (!quantity.HasValue)
            {
                return;
            }

            if (quantity.Value < 0)
            {
                return;
            }

            product.Quantity = quantity.Value;
        }

        private void UpdatePrice(Grid grid, decimal? price)
        {
            if (!price.HasValue)
            {
                return;
            }

            if (price.Value <= 0)
            {
                return;
            }

            grid.Price = price.Value;
        }

        private void UpdateQuantity(Grid grid, int? quantity)
        {
            if (!quantity.HasValue)
            {
                return;
            }

            if (quantity.Value < 0)
            {
                return;
            }

            grid.Quantity = quantity.Value;
        }
    }
}
