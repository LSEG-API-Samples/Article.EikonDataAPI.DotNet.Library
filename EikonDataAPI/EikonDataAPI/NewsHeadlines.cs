using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;


namespace EikonDataAPI
{
    public partial class NewsHeadlines : EndPoint, INewsHeadlines
    {
        public NewsHeadlines(Profile profile, JSONRequest request)
        {
            _endPoint = "News_Headlines";
            _profile = profile;
            _logger = _profile.CreateLogger<NewsHeadlines>();
            _jsonRequest = request;

        }

        public string GetNewsHeadlinesRaw(string query,
            uint? number,
            DateTime? from,
            DateTime? to)
        {

            string startDateFormatString = "yyyy-MM-ddTHH:mm:sszzz";
            string endDateFormatString = "yyyy-MM-ddTHH:mm:sszzz";

            if (from?.Kind == DateTimeKind.Utc)
            {
                startDateFormatString = "yyyy-MM-ddTHH:mm:ssZ";
            }

            if (to?.Kind == DateTimeKind.Utc)
            {
                endDateFormatString = "yyyy-MM-ddTHH:mm:ssZ";
            }


            if (query == null)
            {
                query = "TOPALL AND LEN";

            }
            NewsHeadlinesRequest request = new NewsHeadlinesRequest
            {
                query = query,
                number = number?.ToString(),
                dateFrom = from != null ? from?.ToString(startDateFormatString) : null,
                dateTo = to != null ? to?.ToString(endDateFormatString) : null,
                // timezone = timezone,
                repository = null
            };

            var response = SerializeAndSendRequest<NewsHeadlinesRequest>(request);
            return response;

        }
        public string GetNewsHeadlinesRaw(string query,
            uint? number,
            string from,
            string to)
        {


            string[] formats = { "yyyy-MM-dd", "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-ddTHH:mm:sszzz", "yyyy-MM-ddTHH:mm:ssZ" };

            if (query == null)
            {
                query = "TOPALL AND LEN";

            }

            DateTime expectedDate;
            if (from != null)
            {
                if (!DateTime.TryParseExact(from, formats, null, DateTimeStyles.None, out expectedDate))
                {
                    EikonException error = new EikonException(HttpStatusCode.BadRequest, "Unsupported from Date Format. (yyyy-MM-dd, yyyy-MM-ddTHH:mm:SS, yyyy-MM-ddTHH:mm:SSzzz, or yyyy-MM-ddTHH:mm:ssZ");
                    _logger?.LogError(error.Message);
                    error.Source = "NewsHeadlines";
                    throw (error);
                }
            }
            if (to != null)
            {
                if (!DateTime.TryParseExact(to, formats, null, DateTimeStyles.None, out expectedDate))
                {
                    EikonException error = new EikonException(HttpStatusCode.BadRequest, "Unsupported to Date Format. (yyyy-MM-dd, yyyy-MM-ddTHH:mm:SS, yyyy-MM-ddTHH:mm:SSzzz, or yyyy-MM-ddTHH:mm:ssZ");
                   
                    _logger?.LogError(error.Message);
                    error.Source = "NewsHeadlines";
                    throw (error);

                }
            }
            NewsHeadlinesRequest request = new NewsHeadlinesRequest
            {
                query = query,
                number = number?.ToString(),
                dateFrom = from,
                dateTo = to,
                // timezone = timezone,
                repository = null
            };

            var response = SerializeAndSendRequest<NewsHeadlinesRequest>(request);
            return response;

        }

        public string GetNewsHeadlinesRaw(string query = "TOPALL AND LEN", uint number = 10)
        {
          
            NewsHeadlinesRequest request = new NewsHeadlinesRequest
            {
                query = query,
                number = number.ToString()
            };
            var response = SerializeAndSendRequest<NewsHeadlinesRequest>(request);
            return response;


        }

    }

    public class NewsHeadlinesResponse
    {
        public List<NewsHeadline> headlines;
        public string newer;
        public string older;
       
    }

    public class NewsHeadline
    {
        public string author;
        public string brokerName;
        public string displayDirection;
        public string documentID;
        public string documentType;
        public DateTime firstCreated;
        public DateTime versionCreated;
        public string headlineType;
        public bool isAlert;
        public string language;
        public string numberOfPages;
        public string reportCode;
        public string sourceCode;
        public string sourceName;
        public string storyId;
        public string text;

    }
    internal class NewsHeadlinesRequest
    {
        public string query;
        public string number;
        public string dateFrom;
        public string dateTo;
       // public string timezone;
        //[JsonConverter(typeof(StringEnumConverter))]
        [JsonConverter(typeof(ListEnumToCsvConverter<NewsRepository>))]
        public List<NewsRepository> repository;
        public string payload;
    }
    public enum NewsRepository { NewsWire, NewsRoom, WebNews }
}
