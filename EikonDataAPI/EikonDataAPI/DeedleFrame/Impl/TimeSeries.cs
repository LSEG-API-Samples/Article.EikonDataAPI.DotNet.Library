using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deedle;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;

namespace EikonDataAPI
{
    public partial class TimeSeries : EndPoint, ITimeSeries
    {
        private Frame<int, string> CreateNonNormalizedFrame(TimeSeriesResponse response)
        {
            Frame<int, string> TotalFrame = Frame.CreateEmpty<int, string>();
            int totalIndex = 0;

            foreach (var timeseries in response.timeseriesData)
            {
                var ric = timeseries.ric;
                var errorCode = timeseries.statusCode;

                if (errorCode.ToString().ToLower() == "error")
                {
                    _logger.LogError("Error: {0} {1}", timeseries.ric, timeseries.errorMessage);
                    continue;
                }
                Frame<int, string> f = Frame.CreateEmpty<int, string>();
                var symbolColumn = Enumerable.Repeat(ric, timeseries.dataPoints.Count()).ToList<string>();

                f.AddColumn("Security", CreateSeriesString(symbolColumn, totalIndex));

                for (int i = 0; i < timeseries.fields.Count(); i++)
                {
                    var field = timeseries.fields[i];


                    var list = timeseries.dataPoints.Select(x => x[i]).ToList();
                    //f.AddColumn(field.name, CreateSeriesObject(list, totalIndex));
                    // Console.WriteLine(field.name);
                    if (field.type.ToLower() == "datetime")
                    {
                        f.AddColumn(field.name, CreateSeries<DateTime>(list, totalIndex));

                    }
                    else if (field.type.ToLower() == "double")
                    {
                        f.AddColumn(field.name, CreateSeriesDouble(list, totalIndex));

                    }
                    else if (field.type.ToLower() == "integer" || field.type.ToLower() == "int")
                    {
                        f.AddColumn(field.name, CreateSeries<int>(list, totalIndex));
                    }
                    else
                    {
                        f.AddColumn(field.name, CreateSeries<string>(list, totalIndex));
                    }


                }

                TotalFrame = TotalFrame.Merge(f);
                totalIndex = totalIndex + timeseries.dataPoints.Count();

            }


            return TotalFrame;


        }
        private Frame<int, string> CreateFrame(TimeSeriesResponse response, bool normalize = false)
        {

            return CreateNonNormalizedFrame(response);

        }
        private Series<int, string> CreateSeriesString(List<string> list, int startIndex)
        {
            List<KeyValuePair<int, string>> seriesBuilder = new List<KeyValuePair<int, string>>();

            for (int i = 0; i < list.Count(); i++)
            {

                seriesBuilder.Add(new KeyValuePair<int, string>(startIndex + i, list[i]));
            }
            Series<int, string> s1 = new Series<int, string>(seriesBuilder);
            return s1;

        }
        private Series<int, double> CreateSeriesDouble(List<JValue> list, int startIndex)
        {
            List<KeyValuePair<int, double>> seriesBuilder = new List<KeyValuePair<int, double>>();

            for (int i = 0; i < list.Count(); i++)
            {
                double temp = 0;
                try
                {
                    if (list[i].Value == null)
                    {
                        temp = double.NaN;
                    }
                    else
                    {
                        temp = list[i].ToObject<double>();
                    }


                }
                catch (ArgumentException ex)
                {
                    temp = double.NaN;
                }
                seriesBuilder.Add(new KeyValuePair<int, double>(startIndex + i, temp));
            }
            Series<int, double> s1 = new Series<int, double>(seriesBuilder);
            return s1;

        }

        private Series<int, V> CreateSeries<V>(List<JValue> list, int startIndex)
        {
            List<KeyValuePair<int, V>> seriesBuilder = new List<KeyValuePair<int, V>>();

            for (int i = 0; i < list.Count(); i++)
            {

                seriesBuilder.Add(new KeyValuePair<int, V>(startIndex + i, list[i].ToObject<V>()));
            }
            Series<int, V> s1 = new Series<int, V>(seriesBuilder);
            return s1;

        }
        private Frame<int, string> CreateNormalizedFrame(TimeSeriesResponse response)
        {
            Frame<int, string> TotalFrame = Frame.CreateEmpty<int, string>();
            int totalIndex = 0;

            foreach (var timeseries in response.timeseriesData)
            {
                var ric = timeseries.ric;
                var errorCode = timeseries.statusCode;
                if (errorCode.ToString().ToLower() == "error")
                {
                    _logger.LogError("Error: {0} {1}", timeseries.ric, timeseries.errorMessage);
                    continue;
                }


                var fields = timeseries.fields.Select(x => x.name).ToList();
                var timeStampIndex = fields.IndexOf("TIMESTAMP");
                fields.Remove("TIMESTAMP");
                var timestamps = timeseries.dataPoints.Select(x => x[timeStampIndex]).ToList();
                timeseries.dataPoints.ForEach(x => x.RemoveAt(timeStampIndex));
                var fieldCount = fields.Count();

                var columnSize = timeseries.dataPoints.Count();
                var symbolColumn = Enumerable.Repeat(ric, fieldCount * columnSize).ToList<string>();
                var fieldsColumn = Enumerable.Repeat(fields, columnSize).ToList().SelectMany(i => i).ToList();
                var valueColumn = timeseries.dataPoints.SelectMany(i => i).ToList();
                var timeStampColumn = timestamps.Select(x => Enumerable.Repeat(x, fieldCount).ToList()).ToList().SelectMany(i => i).ToList();



                Frame<int, string> f = Frame.CreateEmpty<int, string>();

                f.AddColumn("Date", CreateSeries<DateTime>(timeStampColumn, totalIndex));
                f.AddColumn("Field", CreateSeriesString(fieldsColumn, totalIndex));
                f.AddColumn("Security", CreateSeriesString(symbolColumn, totalIndex));
                f.AddColumn("Value", CreateSeriesDouble(valueColumn, totalIndex));


                TotalFrame = TotalFrame.Merge(f);
                totalIndex = totalIndex + (fieldCount * columnSize);
            }
            return TotalFrame;
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

            var response = GetTimeSeriesRaw(rics, startDate, endDate, interval, fields, count, calendar, corax);
            if (response == null) return Frame.CreateEmpty<int, string>();

            return CreateFrame(JsonConvert.DeserializeObject<TimeSeriesResponse>(response, new JsonSerializerSettings
            {
                Error = HandleDeserializationError
            }));


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
            //  CreateLogger(eikon);
            var response = GetTimeSeriesRaw(ric, startDate, endDate, interval, fields, count, calendar, corax);
            if (response == null) return Frame.CreateEmpty<int, string>();

            return CreateFrame(JsonConvert.DeserializeObject<TimeSeriesResponse>(response, new JsonSerializerSettings
            {
                Error = HandleDeserializationError
            }));
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

            var response = GetTimeSeriesRaw(rics, startDate, endDate, interval, fields, count, calendar, corax);
            if (response == null) return Frame.CreateEmpty<int, string>();

            return CreateFrame(JsonConvert.DeserializeObject<TimeSeriesResponse>(response, new JsonSerializerSettings
            {
                Error = HandleDeserializationError
            }));
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

            var response = GetTimeSeriesRaw(ric, startDate, endDate, interval, fields, count, calendar, corax);
            if (response == null) return Frame.CreateEmpty<int, string>();

            return CreateFrame(JsonConvert.DeserializeObject<TimeSeriesResponse>(response, new JsonSerializerSettings
            {
                Error = HandleDeserializationError
            }));
        }
    }
}
