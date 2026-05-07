using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graduation_Project.BLL.Common
{
    public class ServiceResult<T>
    {
        public bool Succeeded { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }

        public ServiceResult()
        {
            Errors = new List<string>();
        }

        public static ServiceResult<T> Success(T data, string message = null)
        {
            return new ServiceResult<T>
            {
                Succeeded = true,
                Data = data,
                Message = message
            };
        }

        public static ServiceResult<T> Failure(string error)
        {
            return new ServiceResult<T>
            {
                Succeeded = false,
                Errors = new List<string> { error }
            };
        }

        public static ServiceResult<T> Failure(List<string> errors)
        {
            return new ServiceResult<T>
            {
                Succeeded = false,
                Errors = errors
            };
        }
    }
}