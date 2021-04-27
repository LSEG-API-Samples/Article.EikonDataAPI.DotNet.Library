using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.Analysis;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace EikonDataAPI
{
    public partial class TimeSeries : EndPoint, ITimeSeries
    {
        private DataFrame CreateNonNormalizedFrame(TimeSeriesResponse response)
        {
            DataFrame TotalFrame = null;
            int totalIndex = 0;

            foreach (var timeseries in response.timeseriesData)
            {
                var ric = timeseries.ric;
                var errorCode = timeseries.statusCode;

                if (errorCode.ToString().ToLower() == "error")                
                {

                    _logger?.LogError("Error: {0} {1}", timeseries.ric, timeseries.errorMessage);
                    continue;
                }
                DataFrame f = new DataFrame();
                var symbolColumn = Enumerable.Repeat(ric, timeseries.dataPoints.Count()).ToList<string>();
                
                //f.Columns.Add(new PrimitiveDataFrameColumn<int>("Index", Enumerable.Range(totalIndex, timeseries.dataPoints.Count())));
                //f.Columns.Add(StringDataFrameColumn("Name", names);
                f.Columns.Add(new StringDataFrameColumn("Security", symbolColumn));                
                

                for (int i = 0; i < timeseries.fields.Count(); i++)
                {
                    var field = timeseries.fields[i];


                    var list = timeseries.dataPoints.Select(x => x[i]).ToList();
                    //f.AddColumn(field.name, CreateSeriesObject(list, totalIndex));
                    // Console.WriteLine(field.name);
                    if (field.type.ToLower() == "datetime")
                    {
                        //f.AddColumn(field.name, CreateSeries<DateTime>(list, totalIndex));
                        f.Columns.Add(new PrimitiveDataFrameColumn<DateTime>(field.name, list.Select(x=>x.ToObject<DateTime>()).ToList()));
                    }
                    else if (field.type.ToLower() == "double")
                    {
                        //f.AddColumn(field.name, CreateSeriesDouble(list, totalIndex));
                        f.Columns.Add(new PrimitiveDataFrameColumn<double>(field.name, list.Select(x => x.ToObject<double>()).ToList()));

                    }
                    else if (field.type.ToLower() == "integer" || field.type.ToLower() == "int")
                    {
                        //f.AddColumn(field.name, CreateSeries<int>(list, totalIndex));
                        f.Columns.Add(new PrimitiveDataFrameColumn<int>(field.name, list.Select(x => x.ToObject<int>()).ToList()));
                    }
                    else
                    {
                        //f.AddColumn(field.name, CreateSeries<string>(list, totalIndex));
                        f.Columns.Add(new StringDataFrameColumn(field.name, list.Select(x => x.ToString()).ToList()));
                    }


                }
                //foreach (var row in f.Rows)
                //{
                //    foreach (var col in row)
                //        Console.WriteLine(col);
                //}
                if (TotalFrame == null)
                {
                    TotalFrame = f;
                }
                else
                {
                    
                    

                    foreach (var col in f.Columns)
                    {
                        if (TotalFrame.Columns.Any(x => x.Name == col.Name) == false)
                        {
                            if (col.DataType == typeof(int))
                            {
                               
                                TotalFrame.Columns.Add(new PrimitiveDataFrameColumn<int>(col.Name, TotalFrame.Rows.Count));
                            }
                            else if (col.DataType == typeof(double))
                            {
                              

                                TotalFrame.Columns.Add(new PrimitiveDataFrameColumn<double>(col.Name, TotalFrame.Rows.Count));
                            }
                            else if (col.DataType == typeof(string))
                            {
                               

                                TotalFrame.Columns.Add(new StringDataFrameColumn(col.Name, TotalFrame.Rows.Count));
                            }

                        }
                    }
                    List<KeyValuePair<string, object>> list = new List<KeyValuePair<string, object>>();


                    for (long i = 0; i < f.Rows.Count; i++)
                    {
                        list.Clear();
                        foreach (var col in f.Columns)
                        {
                            KeyValuePair<string, object> kv = new KeyValuePair<string, object>(col.Name, col[i]);
                            list.Add(kv);
                        }
                        if(list.Count>0)
                            TotalFrame = TotalFrame.Append(list);
                    }
                    
                    
                    

                }
                
                //foreach (var row in TotalFrame.Rows)
                //{
                //    foreach (var col in row)
                //        Console.WriteLine(col);
                //}
                totalIndex = totalIndex + timeseries.dataPoints.Count();

            }

            return TotalFrame;
        }


        private DataFrame CreateFrame(TimeSeriesResponse response, bool normalize = false)
        {
            return CreateNonNormalizedFrame(response);
        }
        public DataFrame GetTimeSeries(IEnumerable<string> rics,
          DateTime startDate,
          DateTime endDate,
          Interval? interval = Interval.daily,
          IEnumerable<TimeSeriesField> fields = null,
          int? count = null,
          Calendar? calendar = null,
          Corax? corax = null)
        {
            var response = GetTimeSeriesRaw(rics, startDate, endDate, interval, fields, count, calendar, corax);
            if (response == null) new DataFrame();

            return CreateFrame(JsonConvert.DeserializeObject<TimeSeriesResponse>(response, new JsonSerializerSettings
            {
                Error = HandleDeserializationError
            }));
            

        }
        public DataFrame GetTimeSeries(string ric,
            DateTime startDate,
            DateTime endDate,
            Interval? interval = Interval.daily,
            IEnumerable<TimeSeriesField> fields = null,
            int? count = null,
            Calendar? calendar = null,
            Corax? corax = null)
        {
            var response = GetTimeSeriesRaw(ric, startDate, endDate, interval, fields, count, calendar, corax);
            if (response == null) return new DataFrame();

            return CreateFrame(JsonConvert.DeserializeObject<TimeSeriesResponse>(response, new JsonSerializerSettings
            {
                Error = HandleDeserializationError
            }));
        }

        public DataFrame GetTimeSeries(IEnumerable<string> rics,
            string startDate,
            string endDate,
            Interval? interval = Interval.daily,
            IEnumerable<TimeSeriesField> fields = null,
            int? count = null,
            Calendar? calendar = null,
            Corax? corax = null)
        {
            var response = GetTimeSeriesRaw(rics, startDate, endDate, interval, fields, count, calendar, corax);
            if (response == null) return new DataFrame();

            return CreateFrame(JsonConvert.DeserializeObject<TimeSeriesResponse>(response, new JsonSerializerSettings
            {
                Error = HandleDeserializationError
            }));
        }

        public DataFrame GetTimeSeries(string ric,
            string startDate,
            string endDate,
            Interval? interval = Interval.daily,
            IEnumerable<TimeSeriesField> fields = null,
            int? count = null,
            Calendar? calendar = null,
            Corax? corax = null)
        {
            var response = GetTimeSeriesRaw(ric, startDate, endDate, interval, fields, count, calendar, corax);
            if (response == null) return new DataFrame();

            return CreateFrame(JsonConvert.DeserializeObject<TimeSeriesResponse>(response, new JsonSerializerSettings
            {
                Error = HandleDeserializationError
            }));
        }
    }
}
