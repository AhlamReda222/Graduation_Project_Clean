using Graduation_Project.DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Graduation_Project.BLL.DTOs.BrandOwnerRequest
{
    public class UpdateRequestStatusDto
    {
        [Required]
        public RequestStatus RequestStatus { get; set; }
    }
}