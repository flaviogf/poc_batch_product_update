using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BatchProductUpdate.Api.ViewModels
{
    public class UpdateProductViewModel
    {
        [Required]
        public IFormFile File { get; set; }
    }
}
