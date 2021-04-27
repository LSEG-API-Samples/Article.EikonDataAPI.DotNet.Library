using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deedle;


namespace EikonDataAPI
{
    public partial class Eikon : IEikon
    {
        public Frame<int, string> GetData(IEnumerable<string> instruments, IEnumerable<TRField> fields, Dictionary<string, string> parameters = null)
        {
            return DataGrid.GetData(instruments, fields, parameters);
        }

        public Frame<int, string> GetData(IEnumerable<string> instruments, IEnumerable<string> fields, Dictionary<string, string> parameters = null)
        {
            return DataGrid.GetData(instruments, fields, parameters);
        }

        public Frame<int, string> GetData(string instrument, IEnumerable<TRField> fields, Dictionary<string, string> parameters = null)
        {
            return DataGrid.GetData(instrument, fields, parameters);
        }

        public Frame<int, string> GetData(string instrument, string field, Dictionary<string, string> parameters = null)
        {
            return DataGrid.GetData(instrument, field, parameters);
        }
        public Frame<int, string> GetNewsHeadlines(string query = "TOPALL AND LEN", uint count = 10)
        {
            return NewsHeadlines.GetNewsHeadlines(query, count);
        }
        public Frame<int, string> GetNewsHeadlines(
            string query,
            uint? count,
            DateTime? dateFrom,
            DateTime? dateTo)
        {
            return NewsHeadlines.GetNewsHeadlines(query, count, dateFrom, dateTo);
        }

        public Frame<int, string> GetNewsHeadlines(
            string query,
            uint? count,
            string dateFrom,
            string dateTo)
        {
            return NewsHeadlines.GetNewsHeadlines(query, count, dateFrom, dateTo);
        }

        public Frame<string, string> GetSymbology(IEnumerable<string> symbols,
            SymbologyType fromSymbolType = SymbologyType.RIC,
            IEnumerable<SymbologyType> toSymbolType = null,
         //   uint? limit = null,
            bool bestMatch = true)
        {
            return SymbologySearch.GetSymbology(symbols, fromSymbolType, toSymbolType, bestMatch);
        }
        public Frame<string, string> GetSymbology(string symbol,
            SymbologyType fromSymbolType = SymbologyType.RIC,
            IEnumerable<SymbologyType> toSymbolType = null,
          //  uint? limit = null,
            bool bestMatch = true)
        {
            return SymbologySearch.GetSymbology(symbol, fromSymbolType, toSymbolType, bestMatch);
        }
        public Frame<int, string> GetTimeSeries(IEnumerable<string> rics,
          DateTime startDate,
          DateTime endDate,
        Interval? interval = Interval.daily,
          IEnumerable<TimeSeriesField> fields = null,
          int? count = null,
          Calendar? calendar = null,
          Corax? corax = null)
        {
            return TimeSeries.GetTimeSeries(rics, startDate, endDate, interval, fields, count, calendar, corax);
        }
        public Frame<int, string> GetTimeSeries(string ric,
            DateTime startDate,
            DateTime endDate,
            Interval? interval = Interval.daily,
            IEnumerable<TimeSeriesField> fields = null,
            int? count = null,
            Calendar? calendar = null,
            Corax? corax = null)
        {
            return TimeSeries.GetTimeSeries(ric, startDate, endDate, interval, fields, count, calendar, corax);
        }

        public Frame<int, string> GetTimeSeries(IEnumerable<string> rics,
            string startDate,
            string endDate,
           Interval? interval = Interval.daily,
            IEnumerable<TimeSeriesField> fields = null,
            int? count = null,
            Calendar? calendar = null,
            Corax? corax = null)
        {
            return TimeSeries.GetTimeSeries(rics, startDate, endDate, interval, fields, count, calendar, corax);
        }

        public Frame<int, string> GetTimeSeries(string ric,
            string startDate,
            string endDate,
           Interval? interval = Interval.daily,
            IEnumerable<TimeSeriesField> fields = null,
            int? count = null,
            Calendar? calendar = null,
            Corax? corax = null)
        {
            return TimeSeries.GetTimeSeries(ric, startDate, endDate, interval, fields, count, calendar, corax);
        }
    }
}
