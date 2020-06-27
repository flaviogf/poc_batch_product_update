using BatchProductUpdate.Api.Application;
using BatchProductUpdate.Api.Extensions;
using BatchProductUpdate.Api.Infrastructure;
using BatchProductUpdate.Api.ViewModels;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace BatchProductUpdate.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IProductApplication _productApplication;

        public ProductController(IUnitOfWork uow, IProductApplication productApplication)
        {
            _uow = uow;
            _productApplication = productApplication;
        }

        [HttpPatch]
        public async Task<IActionResult> Update([FromForm] UpdateProductViewModel viewModel)
        {
            try
            {
                using var reader = ExcelReaderFactory.CreateReader(viewModel.File.OpenReadStream());

                var configuration = new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (it) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true
                    }
                };

                using var workbook = reader.AsDataSet(configuration);

                var tasks = new List<Task<Result>>();

                foreach (DataTable sheet in workbook.Tables)
                {
                    foreach (DataRow row in sheet.Rows)
                    {
                        var productCode = row.GetValue<string>("codigo");
                        var gridCode = row.GetValue<string>("codigo sku");
                        var price = row.GetValue<decimal?>("preço");
                        var quantity = row.GetValue<int?>("estoque");

                        var updatePriceAndQuantityViewModel = new UpdateProductPriceAndQuantityViewModel
                        {
                            Price = price,
                            Quantity = quantity
                        };

                        tasks.Add(_productApplication.Update(productCode, gridCode, updatePriceAndQuantityViewModel));
                    }
                }

                var result = await Task.WhenAll(tasks);

                _uow.Commit();

                return Ok(result);
            }
            catch (Exception)
            {
                _uow.Rollback();

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
