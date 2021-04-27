using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace EikonDataAPI
{
    public interface IEikon : IDataGrid, ITimeSeries, ISymbologySearch, INewsHeadlines, INewsStory
    {
        void SetAppKey(string appKey);
        string GetAppKey();
        void SetPort(uint port);
        void SetTimeout(int millisec);
        int GetTimeout();
        ILoggerFactory GetLoggerFactory();
        string SendJSONRequest(string entity, string payload);
        string SendJSONRequest<T>(string entity, T obj, bool ignoreNull);



    }
}
