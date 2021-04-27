
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;

namespace EikonDataAPI
{
    public partial class TimeSeries : EndPoint, ITimeSeries
    {
        //private string _endPoint = "TimeSeries";
        //private Profile _profile;
        //private ILogger _logger = null;

        public TimeSeries(Profile profile, JSONRequest request)
        {
            _endPoint = "TimeSeries";
            _profile = profile;
            _logger = _profile.CreateLogger<TimeSeries>();
            _jsonRequest = request;

        }
        public string GetTimeSeriesRaw(IEnumerable<string> rics,
            DateTime start,
            DateTime end,
            Interval? interval = Interval.daily,
            IEnumerable<TimeSeriesField> fields = null,
            int? count = null,
            Calendar? calendar = null,
            Corax? corax = null)
        {
            if (fields != null)
            {

                if (fields.Contains(TimeSeriesField.TIMESTAMP) == false)
                {
                    List<TimeSeriesField> temp = fields.ToList();
                    temp.Insert(0, TimeSeriesField.TIMESTAMP);

                    fields = temp;

                }
            }
            TimeSeriesRequest request = new TimeSeriesRequest
            {
                rics = rics?.ToList(),
                fields = fields?.ToList(),
                startdate = start.Kind == DateTimeKind.Utc ? start.ToString("yyyy-MM-ddTHH:mm:ssZ") : start.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                enddate = end.Kind == DateTimeKind.Utc ? end.ToString("yyyy-MM-ddTHH:mm:ssZ") : end.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                interval = interval,
                calendar = calendar,
                corax = corax,
                count = count
            };

            var response = SerializeAndSendRequest<TimeSeriesRequest>(request);
            return response;
            //if (response == null) return null;


            //return JsonConvert.DeserializeObject<TimeSeriesResponse>(response, new JsonSerializerSettings
            //{
            //    Error = HandleDeserializationError
            //});
        }
        public string GetTimeSeriesRaw(string ric,
            DateTime startDate,
            DateTime endDate,
            Interval? interval = Interval.daily,
            IEnumerable<TimeSeriesField> fields = null,
            int? count = null,
            Calendar? calendar = null,
            Corax? corax = null)
        {
            
            if (fields != null)
            {
                
                if (fields.Contains(TimeSeriesField.TIMESTAMP) == false)
                {
                    List<TimeSeriesField> temp = fields.ToList();
                    temp.Insert(0, TimeSeriesField.TIMESTAMP);

                    fields = temp;
                    
                }
            }
            TimeSeriesRequest request = new TimeSeriesRequest {
                rics = new List<string>{ ric },
                fields = fields?.ToList(),
                startdate = startDate.Kind==DateTimeKind.Utc ? startDate.ToString("yyyy-MM-ddTHH:mm:ssZ") : startDate.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                enddate = endDate.Kind==DateTimeKind.Utc ? endDate.ToString("yyyy-MM-ddTHH:mm:ssZ") : endDate.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                interval = interval,
                calendar = calendar,
                corax = corax,
                count = count
            };
  

            var response = SerializeAndSendRequest<TimeSeriesRequest>(request);
            return response;
            //if (response == null) return null;


            //return JsonConvert.DeserializeObject<TimeSeriesResponse>(response, new JsonSerializerSettings
            //{
            //    Error = HandleDeserializationError
            //});
        }

        public String GetTimeSeriesRaw(IEnumerable<string> rics,
            string startDate,
            string endDate,
           Interval? interval = Interval.daily,
            IEnumerable<TimeSeriesField> fields = null,
            int? count = null,
            Calendar? calendar = null,
            Corax? corax = null)
        {
            EikonException error = null;
            string[] formats = { "yyyy-MM-dd", "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-ddTHH:mm:sszzz", "yyyy-MM-ddTHH:mm:ssZ" };
            DateTime expectedDate;
            if (!DateTime.TryParseExact(startDate, formats, null, DateTimeStyles.None, out expectedDate))
            {
                error = new EikonException(HttpStatusCode.BadRequest, "Unsupported start Date Format. (yyyy-MM-dd, yyyy-MM-ddTHH:mm:SS, or yyyy-MM-ddTHH:mm:SSzzz" );
                _logger?.LogError(error.Message);
                error.Source = "TimeSeries";
                throw (error);
               // return null;
            }
            if (!DateTime.TryParseExact(endDate, formats, null, DateTimeStyles.None, out expectedDate))
            {
                error = new EikonException(HttpStatusCode.BadRequest,  "Unsupported end Date Format. (yyyy-MM-dd, yyyy-MM-ddTHH:mm:SS, or yyyy-MM-ddTHH:mm:SSzzz" );
                _logger?.LogError(error.Message);
                error.Source = "TimeSeries";
                throw (error);
                //return null;
            }
            if (fields != null)
            {

                if (fields.Contains(TimeSeriesField.TIMESTAMP) == false)
                {
                    List<TimeSeriesField> temp = fields.ToList();
                    temp.Insert(0, TimeSeriesField.TIMESTAMP);

                    fields = temp;

                }
            }
            TimeSeriesRequest request = new TimeSeriesRequest
            {
                rics = rics?.ToList(),
                fields = fields?.ToList(),
                startdate = startDate,
                enddate = endDate,
                interval = interval,
                calendar = calendar,
                corax = corax,
                count = count
            };
 

            var response = SerializeAndSendRequest<TimeSeriesRequest>(request);
            return response;
            //if (response == null) return null;


            //return JsonConvert.DeserializeObject<TimeSeriesResponse>(response, new JsonSerializerSettings
            //{
            //    Error = HandleDeserializationError
            //});
        }
        public string GetTimeSeriesRaw(string ric,
            string startDate,
            string endDate,
            Interval? interval = Interval.daily,
            IEnumerable<TimeSeriesField> fields = null,
            int? count = null,
            Calendar? calendar = null,
            Corax? corax = null)
        {
          
            string[] formats = { "yyyy-MM-dd", "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-ddTHH:mm:sszzz" };
            DateTime expectedDate;
            if (!DateTime.TryParseExact(startDate, formats, null, DateTimeStyles.None, out expectedDate))
            {
                EikonException error = new EikonException(HttpStatusCode.BadRequest, "Unsupported start Date Format. (yyyy-MM-dd, yyyy-MM-ddTHH:mm:SS, or yyyy-MM-ddTHH:mm:SSzzz" );
                _logger?.LogError(error.Message);
                error.Source = "TimeSeries";
                throw (error);
            }
            if (!DateTime.TryParseExact(endDate, formats, null, DateTimeStyles.None, out expectedDate))
            {
                EikonException error = new EikonException(HttpStatusCode.BadRequest, "Unsupported end Date Format. (yyyy-MM-dd, yyyy-MM-ddTHH:mm:SS, or yyyy-MM-ddTHH:mm:SSzzz" );
                _logger?.LogError(error.Message);
                error.Source = "TimeSeries";
                throw (error);
            }
            if (fields != null)
            {

                if (fields.Contains(TimeSeriesField.TIMESTAMP) == false)
                {
                    List<TimeSeriesField> temp = fields.ToList();
                    temp.Insert(0, TimeSeriesField.TIMESTAMP);

                    fields = temp;

                }
            }
            TimeSeriesRequest request = new TimeSeriesRequest
            {
                rics = new List<string> { ric },
                fields = fields?.ToList(),
                startdate = startDate,
                enddate = endDate,
                interval = interval,
                calendar = calendar,
                corax = corax,
                count = count
            };


            var response = SerializeAndSendRequest<TimeSeriesRequest>(request);
            return response;

        }

    }
    public class TimeSeriesResponse
    {
        public List<TimeSeriesData> timeseriesData;
    }
    public class TimeSeriesData
    {
        public List<List<JValue>> dataPoints;
        public List<TimeSeriesDataField> fields;
        public string ric;
        public string statusCode;
        public string errorCode;
        public string errorMessage;
    }
    public class TimeSeriesDataField
    {
        public string name;
        public string type;
    }

    internal class TimeSeriesRequest
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public Calendar? calendar;
        [JsonConverter(typeof(StringEnumConverter))]
        public Corax? corax;
        public int? count;
        public string startdate;
        public string enddate;
        [JsonProperty("fields", ItemConverterType = typeof(StringEnumConverter))]
        public List<TimeSeriesField> fields;
        [JsonConverter(typeof(StringEnumConverter))]
        public Interval? interval;
        public List<string> rics;
        
    }
    public enum Calendar { native, tradingdays, calendardays }
    public enum Corax { adjusted, unadjusted }
    public enum TimeSeriesField { TIMESTAMP, VALUE, VOLUME, HIGH, LOW, OPEN, CLOSE, COUNT}
    public enum Interval { tick, minute, hour, daily, weekly, monthly, quaterly, yearly}
}
