using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;


namespace EikonDataAPI
{
    public partial class Eikon : IEikon 
    {
      

        private Profile Profile { get; set; }


        private string _appId;
        private uint _port = 0;
        private DataGrid DataGrid { get; set; }
        private TimeSeries TimeSeries { get; set; }
        private NewsHeadlines NewsHeadlines { get; set; }
        private NewsStory NewsStory { get; set; }
        private SymbologySearch SymbologySearch { get; set; }
        private JSONRequest JSONRequest { get; set; }
        private ILogger Logger { get; set; }
        private Eikon() {
            Profile = new Profile();
            JSONRequest = new JSONRequest(Profile);
            DataGrid = new DataGrid(Profile, JSONRequest);
            TimeSeries = new TimeSeries(Profile, JSONRequest);
            NewsHeadlines = new NewsHeadlines(Profile, JSONRequest);
            NewsStory = new NewsStory(Profile, JSONRequest);
            SymbologySearch = new SymbologySearch(Profile, JSONRequest);
            Logger = Profile.CreateLogger<Eikon>();
        }
        public static IEikon CreateDataAPI()
        {
            return new Eikon();
        }
       

        public ILoggerFactory GetLoggerFactory()
        {
            return Profile.LoggerFactory;
        }
        public void SetAppKey(string appKey)
        {
            //throw new NotImplementedException();
            Profile.AppId = appKey;

        }
        public string GetAppKey()
        {
            return Profile.AppId;
        }

        public void SetTimeout(int millisec)
        {
            Profile.Timeout = millisec;
            JSONRequest.SetTimeout(millisec);
            //JSONRequest.SetTimeout(millisec);
        }
        public int GetTimeout()
        {
            return Profile.Timeout??0;
        }
        public string SendJSONRequest(string entity, string payload)
        {
            return JSONRequest.SendJSONRequest(Logger, entity, payload);
        }
        public string SendJSONRequest<T>(string entity, T obj, bool ignoreNull)
        {
            string payload = JsonConvert.SerializeObject(obj, new JsonSerializerSettings { NullValueHandling = ignoreNull?NullValueHandling.Ignore:NullValueHandling.Include });
            return JSONRequest.SendJSONRequest(Logger, entity, payload);
        }
        public void SetPort(uint port)
        {
            //throw new NotImplementedException();
            Profile.ForcePort = port;

        }

        public TRField TRField(string fieldname, Dictionary<string, string> parameters = null, SortField? sortField = null, int? priority = null)
        {
            //throw new NotImplementedException();
            return DataGrid.TRField(fieldname, parameters, sortField, priority);
        }

        public string GetDataRaw(IEnumerable<string> instruments, IEnumerable<TRField> fields, Dictionary<string, string> parameters = null)
        {
            //throw new NotImplementedException();
          
            return DataGrid.GetDataRaw(instruments, fields, parameters);
            
        }

        public string GetDataRaw(IEnumerable<string> instruments, IEnumerable<string> fields, Dictionary<string, string> parameters = null)
        {
            //throw new NotImplementedException();
           
            return  DataGrid.GetDataRaw(instruments, fields, parameters);
           
        }

        public string GetDataRaw( string instrument, IEnumerable<TRField> fields,  Dictionary<string, string> parameters = null)
        {
            //throw new NotImplementedException();
           
           return DataGrid.GetDataRaw(instrument, fields, parameters);
           
        }

        public string GetDataRaw(string instrument, string field, Dictionary<string, string> parameters = null)
        {
            // throw new NotImplementedException();
            
             return DataGrid.GetDataRaw(instrument, field, parameters);
            
        }

        public string GetTimeSeriesRaw(IEnumerable<string> rics, DateTime startDate, DateTime endDate, Interval? interval = Interval.daily, IEnumerable<TimeSeriesField> fields = null, int? count = null, Calendar? calendar = null, Corax? corax = null)
        {
            //   throw new NotImplementedException();
            return TimeSeries.GetTimeSeriesRaw(rics, startDate, endDate, interval, fields, count, calendar, corax);
        }

        public string GetTimeSeriesRaw(string ric, DateTime startDate, DateTime endDate, Interval? interval = Interval.daily, IEnumerable<TimeSeriesField> fields = null, int? count = null, Calendar? calendar = null, Corax? corax = null)
        {
            //throw new NotImplementedException();
            return TimeSeries.GetTimeSeriesRaw(ric, startDate, endDate, interval, fields, count, calendar, corax);
        }

        public string GetTimeSeriesRaw(IEnumerable<string> rics, string startDate, string endDate, Interval? interval = Interval.daily, IEnumerable<TimeSeriesField> fields = null, int? count = null, Calendar? calendar = null, Corax? corax = null)
        {
            //throw new NotImplementedException();
            return TimeSeries.GetTimeSeriesRaw(rics, startDate, endDate, interval, fields, count, calendar, corax);
        }

        public string GetTimeSeriesRaw(string ric, string startDate, string endDate, Interval? interval = Interval.daily, IEnumerable<TimeSeriesField> fields = null, int? count = null, Calendar? calendar = null, Corax? corax = null)
        {
            //throw new NotImplementedException();
            return TimeSeries.GetTimeSeriesRaw(ric, startDate, endDate, interval, fields, count, calendar, corax);
        }

        //public NewsHeadlinesResponse GetNewsHeadlinesRaw(string token)
        //{
        //    //throw new NotImplementedException();
        //   return NewsHeadlines.GetNewsHeadlinesRaw(token);
        //}

        public string GetNewsHeadlinesRaw(string query, uint? number, DateTime? from, DateTime? to)
        {
            // throw new NotImplementedException();
            return NewsHeadlines.GetNewsHeadlinesRaw(query, number, from, to);
        }

        public string GetNewsHeadlinesRaw(string query, uint? number, string from, string to)
        {
            //throw new NotImplementedException();
            return NewsHeadlines.GetNewsHeadlinesRaw(query, number, from, to);
        }
        public string GetNewsHeadlinesRaw(string query = "TOPALL AND LEN", uint number = 10)
        {
            return NewsHeadlines.GetNewsHeadlinesRaw(query, number);
        }
        public string GetNewsStoryRaw(string storyId)
        {
            //throw new NotImplementedException();
            return NewsStory.GetNewsStoryRaw(storyId);
        }

        public string GetNewsStory(string storyId)
        {
            //throw new NotImplementedException();
            return NewsStory.GetNewsStory(storyId);
        }


        public string GetSymbologyRaw(IEnumerable<string> symbols,
            SymbologyType fromSymbolType = SymbologyType.RIC,
            IEnumerable<SymbologyType> toSymbolType = null,
           // uint? limit = null,
            bool bestMatch = true)
        {
            
            return SymbologySearch.GetSymbologyRaw(symbols, fromSymbolType, toSymbolType, bestMatch);
        }

        public string GetSymbologyRaw(string symbol,
            SymbologyType fromSymbolType = SymbologyType.RIC,
            IEnumerable<SymbologyType> toSymbolType = null,
          //  uint? limit = null,
            bool bestMatch = true)
        {
            return SymbologySearch.GetSymbologyRaw(symbol, fromSymbolType, toSymbolType, bestMatch);
        }
    }
}
