using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace EikonDataAPI
{

    internal class EikonError
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class EikonException  :Exception
    {
        public HttpStatusCode ErrorCode { get; set; }
      //  public string ErrorMessage { get; set; }

        public EikonException()
        {
        }

        public EikonException(HttpStatusCode Code, string ErrorMessage)
        : base($"{Code.ToString()}: {ErrorMessage}")
        {
            ErrorCode = Code;
        }

        public EikonException(HttpStatusCode Code, string ErrorMessage, Exception inner)
        : base($"{Code.ToString()}: {ErrorMessage}", inner)
        {
            ErrorCode = Code;
        }
    }
    public class JSONRequest
    {
        private  HttpClient client;
        private  TimeSpan? timeout = null;
        private Profile profile;
        public JSONRequest(Profile _profile)
        {
            profile = _profile;
            client = new HttpClient();
        }
        
        public static Exception GetInnerMostException(Exception ex)
        {
            Exception currentEx = ex;
            while (currentEx.InnerException != null)
            {
                currentEx = currentEx.InnerException;
            }

            return currentEx;
        }
        public void SetTimeout(int milisec)
        {
            client.Timeout = new TimeSpan(0, 0, 0, 0, milisec);
        }
        public async Task<Tuple<string, EikonException>> SendJSONRequestAsync(ILogger logger,
            string entity,
            string payload,
            uint id = 123)
        {

            string udfRequest = $"{{\"Entity\":{{\"E\":\"{entity}\",\"W\":{payload}}},\"ID\":\"{id}\" }}";
            string jsonData;
            HttpResponseMessage response = null;
            EikonException error = null;
            logger?.LogDebug("UDF Request: {0}", udfRequest);
            //using (HttpClient client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, profile.Url);
                request.Headers.Add("x-tr-applicationid", profile.AppId);
                request.Content = new StringContent(udfRequest);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                try
                {
                    response = await client.SendAsync(request);
                }
                catch (Exception ex)
                {

                    error = new EikonException(HttpStatusCode.InternalServerError, JSONRequest.GetInnerMostException(ex).Message.ToString(),ex.InnerException);
                    error.Source = "JSONRequest";
                    jsonData = null;
                    logger?.LogError(JsonConvert.SerializeObject(error));
                    throw (error);
                    //return new Tuple<string, EikonException>(jsonData, error);
                }
                if (response.IsSuccessStatusCode)
                {
                    jsonData = await response.Content.ReadAsStringAsync();
                    logger?.LogDebug("UDF Response: {0}", jsonData);
                    if (jsonData.StartsWith("<") && jsonData.EndsWith(">"))
                    {
                        error = new EikonException(HttpStatusCode.InternalServerError, $"Invalid JSON: {jsonData}");
                        error.Source = "JSONRequest";
                        jsonData = null;
                        logger?.LogError(JsonConvert.SerializeObject(error));
                        throw (error);
                    }
                    else if (jsonData.Contains("ErrorCode") && jsonData.Contains("ErrorMessage"))
                    {
                        var eikonError = JsonConvert.DeserializeObject<EikonError>(jsonData);
                        error.ErrorCode = (HttpStatusCode)Enum.ToObject(typeof(HttpStatusCode), eikonError.ErrorCode);
                        error.Source = "JSONRequest";
                        jsonData = null;
                        logger?.LogError(JsonConvert.SerializeObject(error));
                        throw (error);
                    }
                    else if (jsonData.Contains("error") && jsonData.Contains("transactionId"))
                    {
                        error = new EikonException(HttpStatusCode.InternalServerError, jsonData);
                       
                        error.Source = "JSONRequest";
                        logger?.LogError(JsonConvert.SerializeObject(error));
                        throw (error);
                    }

                }
                else
                {
                    error = new EikonException(response.StatusCode, response.ToString());
                    jsonData = null;
                    logger?.LogError(JsonConvert.SerializeObject(error));

                }
                return new Tuple<string, EikonException>(jsonData, error);
            }

        }
        public  string SendJSONRequest(ILogger logger,
            string entity,
            string payload,
            uint id = 123)
        {
            //throw new NotImplementedException();

            
            string udfRequest = $"{{\"Entity\":{{\"E\":\"{entity}\",\"W\":{payload}}},\"ID\":\"{id}\" }}";
            string jsonData;
            HttpResponseMessage response = null;

            logger?.LogDebug("UDF Request: {0}", udfRequest);
            //using (HttpClient client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, profile.Url);                
                request.Headers.Add("x-tr-applicationid", profile.AppId);
                request.Content = new StringContent(udfRequest);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                try
                {
                    response = client.SendAsync(request).Result;
                }catch(Exception ex)
                {
                    EikonException error = new EikonException(HttpStatusCode.InternalServerError, JSONRequest.GetInnerMostException(ex).Message.ToString(), ex);
                    jsonData = null;
                    error.Source = "JSONRequest";
                    logger?.LogError(GetInnerMostException(ex).Message);
                    throw (error);
                    //return jsonData;
                }
                if (response.IsSuccessStatusCode)
                {
                    jsonData = response.Content.ReadAsStringAsync().Result;
                    logger?.LogDebug("UDF Response: {0}",jsonData);
                    if (jsonData.StartsWith("<") && jsonData.EndsWith(">"))
                    {
                        EikonException error = new EikonException(HttpStatusCode.InternalServerError, jsonData);
                        error.Source = "JSONRequest";
                        jsonData = null;
                        logger?.LogError(JsonConvert.SerializeObject(error)) ;
                        throw (error);
                    }else if(jsonData.Contains("ErrorCode") && jsonData.Contains("ErrorMessage"))
                    {
                        


                        var eikonError = JsonConvert.DeserializeObject<EikonError>(jsonData);
                        EikonException error = new EikonException((HttpStatusCode)Enum.ToObject(typeof(HttpStatusCode), eikonError.ErrorCode), eikonError.ErrorMessage);
                        error.Source = "JSONRequest";                        
                        jsonData = null;
                        logger?.LogError(JsonConvert.SerializeObject(error));
                        throw (error);
                    }
                    else if (jsonData.Contains("error") && jsonData.Contains("transactionId"))
                    {
                       
                        EikonException error = new EikonException(HttpStatusCode.InternalServerError, jsonData);                      
                        error.Source = "JSONRequest";
                        logger?.LogError(JsonConvert.SerializeObject(error));
                        throw (error);
                    }


                }
                else
                {
                    EikonException error = new EikonException(response.StatusCode, response.ToString());
                    error.Source = "JSONRequest";
                    jsonData = null;
                    logger?.LogError(JsonConvert.SerializeObject(error));
                    throw (error);

                }
                return jsonData;
            }
        }
    }
}
