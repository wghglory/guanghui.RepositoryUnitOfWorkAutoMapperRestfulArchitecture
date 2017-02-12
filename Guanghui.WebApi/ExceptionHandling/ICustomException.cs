using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Guanghui.WebApi.ExceptionHandling
{

    public interface ICustomException
    {

        int ErrorCode { get; set; }

        string ErrorDescription { get; set; }

        HttpStatusCode HttpStatus { get; set; }

        string ReasonPhrase { get; set; }
    }
}
