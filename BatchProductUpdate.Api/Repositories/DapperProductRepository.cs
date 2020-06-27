using BatchProductUpdate.Api.Infrastructure;
using BatchProductUpdate.Api.Models;
using Dapper;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BatchProductUpdate.Api.Repositories
{
    public class DapperProductRepository : IProductRepository
    {
        private readonly IUnitOfWork _uow;

        public DapperProductRepository(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<Result> Save(string productCode, Product product)
        {
            try
            {
                var query = @"
                    UPDATE [dbo].[Grade]
                    SET [Preco] = @Price,
	                [Estoque] = @Quantity
                    FROM [dbo].[Grade] g
                    JOIN [dbo].[Produto] p
                    ON g.[IdProduto] = p.[IdProduto]
                    WHERE g.[Ativo] = 1 AND g.[Excluido] = 0 AND g.[IdVendedorUsuario] = 1 AND p.[IdExterno] = @ProductCode
                ";

                await _uow.Connection.ExecuteAsync(query, new { product.Price, product.Quantity, ProductCode = productCode }, _uow.Transaction);

                return Result.Ok();
            }
            catch (Exception)
            {
                return Result.Fail("Had a problem");
            }
        }

        public async Task<Result> Save(string productCode, string gridCode, Grid grid)
        {
            try
            {
                var query = @"
                    UPDATE [dbo].[Grade]
                    SET [Preco] = @Price,
	                [Estoque] = @Quantity
                    FROM [dbo].[Grade] g
                    JOIN [dbo].[Produto] p
                    ON g.[IdProduto] = p.[IdProduto]
                    WHERE g.[Ativo] = 1 AND g.[Excluido] = 0 AND g.[IdVendedorUsuario] = 1 AND p.[IdExterno] = @ProductCode AND g.[IdExterno] = @GridCode
                ";

                await _uow.Connection.ExecuteAsync(query, new { grid.Price, grid.Quantity, ProductCode = productCode, GridCode = gridCode }, _uow.Transaction);

                return Result.Ok();
            }
            catch (Exception)
            {
                return Result.Fail("Had a problem");
            }
        }

        public async Task<Maybe<Product>> FindOne(string productCode)
        {
            try
            {
                var query = @"
                    SELECT TOP 1 * FROM
                    (SELECT p.[IdProduto] AS [Id], p.[IdExterno] AS [Code], g.[Preco] AS [Price], g.[Estoque] AS [Quantity], CAST(0 AS BIT) AS [HasGrid] FROM [dbo].[Produto] p
                    JOIN [dbo].[Grade] g
                    ON p.[IdProduto] = g.[IdProduto]
                    WHERE p.[Ativo] = 1 AND p.[Excluido] = 0 AND p.[IdVendedorUsuario] = 1 AND p.[TemGrade] = 0
                    UNION
                    SELECT DISTINCT p.[IdProduto] AS [Id], p.[IdExterno] AS [Code], NULL AS [Price], NULL AS [Quantity], CAST(1 AS BIT) AS [HasGrid] FROM [dbo].[Produto] p
                    JOIN [dbo].[Grade] g
                    ON p.[IdProduto] = g.[IdProduto]
                    WHERE p.[Ativo] = 1 AND p.[Excluido] = 0 AND p.[IdVendedorUsuario] = 1 AND p.[TemGrade] = 1) as p
                    WHERE p.[Code] = @ProductCode;
                ";

                var products = await _uow.Connection.QueryAsync<Product>(query, new { ProductCode = productCode }, _uow.Transaction);

                return products.FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Maybe<Grid>> FindOne(string productCode, string gridCode)
        {
            try
            {
                var query = @"
                    SELECT TOP 1 g.[IdGrade] AS [Id], g.[IdExterno] AS [Code], g.[Preco] AS [Price], g.[Estoque] AS [Quantity] FROM [dbo].[Grade] g
                    JOIN [dbo].[Produto] p
                    ON g.[IdProduto] = p.[IdProduto]
                    WHERE g.[Ativo] = 1 AND g.[Excluido] = 0 AND g.[IdVendedorUsuario] = 1 AND p.[IdExterno] = @ProductCode AND g.[IdExterno] = @GridCode;
                ";

                var grids = await _uow.Connection.QueryAsync<Grid>(query, new { ProductCode = productCode, GridCode = gridCode }, _uow.Transaction);

                return grids.FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
