using System;
using System.Collections.Generic;
using Deedle;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EikonDataAPI
{
    public partial interface ITimeSeries
    {
        Frame<int, string> GetTimeSeries(IEnumerable<string> rics,
          DateTime startDate,
          DateTime endDate,
          Interval? interval = Interval.daily,
          IEnumerable<TimeSeriesField> fields = null,
          int? count = null,
          Calendar? calendar = null,
          Corax? corax = null);
        Frame<int, string> GetTimeSeries(string ric,
            DateTime startDate,
            DateTime endDate,
            Interval? interval = Interval.daily,
            IEnumerable<TimeSeriesField> fields = null,
            int? count = null,
            Calendar? calendar = null,
            Corax? corax = null);

        Frame<int, string> GetTimeSeries(IEnumerable<string> rics,
            string startDate,
            string endDate,
            Interval? interval = Interval.daily,
            IEnumerable<TimeSeriesField> fields = null,
            int? count = null,
            Calendar? calendar = null,
            Corax? corax = null);

        Frame<int, string> GetTimeSeries(string ric,
            string startDate,
            string endDate,
            Interval? interval = Interval.daily,
            IEnumerable<TimeSeriesField> fields = null,
            int? count = null,
            Calendar? calendar = null,
            Corax? corax = null);
    }
}
