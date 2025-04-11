using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyDapr.Core.ResponseResult
{
    public class StandardApiResponse<T>
    {
        public T Data { get; set; } // 返回的具体数据
        public int Status { get; set; } // HTTP 状态码
        public string Code { get; set; } // 状态信息（如 Success 或 Failure）
        public string ErrorMessage { get; set; } // 错误信息
        public object ValidErrors { get; set; } // 验证错误信息

        public StandardApiResponse(T data, int status, string code, string errorMessage = null, object validErrors = null)
        {
            Data = data;
            Status = status;
            Code = code;
            ErrorMessage = errorMessage;
            ValidErrors = validErrors;
        }

        public static StandardApiResponse<T> Success(T data)
        {
            return new StandardApiResponse<T>(data, 200, "Success");
        }

        public static StandardApiResponse<T> Failure(string errorMessage, int status = 500, object validErrors = null)
        {
            return new StandardApiResponse<T>(default(T), status, "Failure", errorMessage, validErrors);
        }
    }
}
