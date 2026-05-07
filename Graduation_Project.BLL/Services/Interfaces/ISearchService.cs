using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Graduation_Project.BLL.Common;
using Graduation_Project.BLL.DTOs.Product;

namespace Graduation_Project.BLL.Services.Interfaces
{
    public interface ISearchService
    {
        Task<ServiceResult<SearchResultDto>> SearchAsync(string query);
    }
}