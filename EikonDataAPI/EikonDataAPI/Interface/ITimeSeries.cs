using System;
using System.Collections.Generic;
using System.Text;

namespace EikonDataAPI
{
    public partial interface ITimeSeries
    {
        string GetTimeSeriesRaw(IEnumerable<string> rics,
            DateTime startDate,
            DateTime endDate,
            Interval? interval = Interval.daily,
            IEnumerable<TimeSeriesField> fields = null,
            int? count = null,
            Calendar? calendar = null,
            Corax? corax = null);
        string GetTimeSeriesRaw(string ric,
            DateTime startDate,
            DateTime endDate,
           Interval? interval = Interval.daily,
            IEnumerable<TimeSeriesField> fields = null,
            int? count = null,
            Calendar? calendar = null,
            Corax? corax = null);

        string GetTimeSeriesRaw(IEnumerable<string> rics,
            string startDate,
            string endDate,
           Interval? interval = Interval.daily,
            IEnumerable<TimeSeriesField> fields = null,
            int? count = null,
            Calendar? calendar = null,
            Corax? corax = null);

        string GetTimeSeriesRaw(string ric,
            string startDate,
            string endDate,
            Interval? interval = Interval.daily,
            IEnumerable<TimeSeriesField> fields = null,
            int? count = null,
            Calendar? calendar = null,
            Corax? corax = null);

    }
}
