using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.Analysis;
namespace EikonDataAPI
{
    public partial interface ITimeSeries
    {
        DataFrame GetTimeSeries(IEnumerable<string> rics,
          DateTime startDate,
          DateTime endDate,
          Interval? interval = Interval.daily,
          IEnumerable<TimeSeriesField> fields = null,
          int? count = null,
          Calendar? calendar = null,
          Corax? corax = null);
        DataFrame GetTimeSeries(string ric,
            DateTime startDate,
            DateTime endDate,
            Interval? interval = Interval.daily,
            IEnumerable<TimeSeriesField> fields = null,
            int? count = null,
            Calendar? calendar = null,
            Corax? corax = null);

        DataFrame GetTimeSeries(IEnumerable<string> rics,
            string startDate,
            string endDate,
            Interval? interval = Interval.daily,
            IEnumerable<TimeSeriesField> fields = null,
            int? count = null,
            Calendar? calendar = null,
            Corax? corax = null);

        DataFrame GetTimeSeries(string ric,
            string startDate,
            string endDate,
            Interval? interval = Interval.daily,
            IEnumerable<TimeSeriesField> fields = null,
            int? count = null,
            Calendar? calendar = null,
            Corax? corax = null);
    }
}

