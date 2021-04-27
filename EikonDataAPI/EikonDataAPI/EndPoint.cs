using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Threading.Tasks;
using System.Net;

namespace EikonDataAPI
{
    public abstract class EndPoint
    {
        protected string _endPoint;
        protected Profile _profile;
        protected ILogger _logger = null;
        protected JSONRequest _jsonRequest = null;

        protected string SendJSONRequest(string request)
        {
            var jsonResponse = _jsonRequest.SendJSONRequest(_logger, _endPoint, request, 123);

            if (jsonResponse == null)
            {
                _logger?.LogDebug("Response is null");
            }

            return jsonResponse;
        }
        protected string SerializeAndSendRequest<T>(T request)
        {
            string payload = JsonConvert.SerializeObject(request, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            

            var jsonResponse = _jsonRequest.SendJSONRequest(_logger, _endPoint, payload, 123);

            if (jsonResponse == null)
            {
                _logger?.LogDebug("Response is null");
            }
           
            return jsonResponse;

        }

        protected async Task<Tuple<string, EikonException>> SerializeAndSendRequestAsync<T>(T request)
        {
            string payload = JsonConvert.SerializeObject(request, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });



            var jsonResponse = await _jsonRequest.SendJSONRequestAsync(_logger, _endPoint, payload, 123);

            if (jsonResponse.Item1 == null)
            {
                _logger?.LogDebug("Response is null");
            }

            return jsonResponse;

        }
        protected void HandleDeserializationError(object sender, ErrorEventArgs errorArgs)
        {
            var currentError = errorArgs.ErrorContext.Error.Message;
            errorArgs.ErrorContext.Handled = true;
            _logger?.LogError("Deserialization Error: {0}", currentError);
            EikonException error = new EikonException(HttpStatusCode.InternalServerError, $"Deserialization Error: {currentError}");
            error.Source = "JSON Deserialization";
            throw (error);

        }
    }
}
