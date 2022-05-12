using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyRates.Models
{
    public class Response<T>
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public static Response<T> DoMethod(Action<Response<T>> action)
        {
            Response<T> response = new Response<T>();
            try
            {
                response.Code = 1;
                response.Message = "success";
                action(response);
            }
            catch (Exception e)
            {
                response.Message = e.Message;
                response.Code = -1;
            }

            return response;
        }
    }
}
