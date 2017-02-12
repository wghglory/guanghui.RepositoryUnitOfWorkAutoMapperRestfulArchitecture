using System;
using System.Net;
using System.Runtime.Serialization;


namespace Guanghui.WebApi.ExceptionHandling
{
    /// <summary>
    /// Business Exception
    /// </summary>
    //[Serializable]
    //[DataContract]
    public class BusinessException : Exception, ICustomException
    {
        #region Public Serializable properties.
        //[DataMember]
        public int ErrorCode { get; set; }
        //[DataMember]
        public string ErrorDescription { get; set; }
        //[DataMember]
        public HttpStatusCode HttpStatus { get; set; }

        string reasonPhrase = "BusinessException";

        //[DataMember]
        public string ReasonPhrase
        {
            get { return this.reasonPhrase; }

            set { this.reasonPhrase = value; }
        }
        #endregion

        #region Public Constructor.
        /// <summary>
        /// Public constructor for Business Exception
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="errorDescription"></param>
        /// <param name="httpStatus"></param>
        public BusinessException(int errorCode, string errorDescription, HttpStatusCode httpStatus)
        {
            ErrorCode = errorCode;
            ErrorDescription = errorDescription;
            HttpStatus = httpStatus;
        }
        #endregion

    }


}