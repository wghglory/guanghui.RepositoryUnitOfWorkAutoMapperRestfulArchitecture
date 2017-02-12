//using System.Web.Script.Serialization;
using System.Data;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;

namespace Guanghui.WebApi.Helpers
{
    public static class JsonHelper
    {
        #region Public extension methods.
        /// <summary>
        /// Extented method of object class
        /// Converts an object to a json string.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SerializeObject(this object obj)
        {
            //var serializer = new JavaScriptSerializer();
            //try
            //{
            //    return serializer.Serialize(obj);
            //}

            //json.net
            try
            {
                return JsonConvert.SerializeObject(obj);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        #endregion
    }
}