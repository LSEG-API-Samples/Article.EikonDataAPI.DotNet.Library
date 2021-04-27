using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.Data.Analysis;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.Extensions.Logging;
namespace EikonDataAPI
{

    public partial class SymbologySearch : EndPoint, ISymbologySearch
    {
        private DataFrame CreateBestMatchFrame(SymbologySearchResponse response)
        {
            DataFrame TotalFrame = null;
            foreach (var symbol in response.mappedSymbols)
            {
                string ric = symbol.symbol;
                DataFrame f = new DataFrame();
                if (symbol.error != null)
                {
                    _logger?.LogInformation("Info: {0} {1}", ric, symbol.error);
                    continue;
                }

                f.Columns.Add(new StringDataFrameColumn("Security", new  List<string> { ric}));
                if (symbol.bestMatch != null)
                {
                    if(symbol.bestMatch.error != null)
                    {
                        _logger?.LogInformation("Info: {0} {1}", ric, symbol.bestMatch.error);
                        continue;
                    }
                    FieldInfo[] fields = typeof(BestMatch).GetFields();
                    foreach (var field in fields)
                    {
                        
                        var tmp = field.GetValue(symbol.bestMatch)?.ToString();
                        if (!string.IsNullOrEmpty(tmp))
                        {
                            f.Columns.Add(new StringDataFrameColumn(field.Name, new List<string> { field.GetValue(symbol.bestMatch).ToString() }));
                        }
                    }
                }
                if (TotalFrame == null)
                {
                    TotalFrame = f;
                }
                else
                {
                    List<KeyValuePair<string, object>> list = new List<KeyValuePair<string, object>>();
                    foreach(var col in f.Columns)
                    {
                        KeyValuePair<string, object> kv = new KeyValuePair<string, object>(col.Name, col.Length > 0 ? col[0] :  null);
                        list.Add(kv);
                        if (TotalFrame.Columns.Any(x => x.Name == col.Name) == false)
                        {                            
                            TotalFrame.Columns.Add(new StringDataFrameColumn(col.Name, TotalFrame.Rows.Count));
                        }
                    }
                    TotalFrame = TotalFrame.Append(list);
                }
            }
            return TotalFrame == null?new DataFrame(): TotalFrame;

                //List<KeyValuePair<string, Series<string, string>>> list = new List<KeyValuePair<string, Series<string, string>>>();
                //foreach (var symbol in response.mappedSymbols)
                //{
                //    string ric = symbol.symbol;
                //    SeriesBuilder<string, string> sb;

                //    if (symbol.bestMatch != null)
                //    {
                //        sb = new SeriesBuilder<string, string>();
                //        FieldInfo[] fields = typeof(BestMatch).GetFields();
                //        foreach (var field in fields)
                //        {
                //            var tmp = field.GetValue(symbol.bestMatch)?.ToString();

                //            if (!string.IsNullOrEmpty(tmp))
                //            {
                //                sb.Add(field.Name, field.GetValue(symbol.bestMatch).ToString());
                //            }

                //        }
                //        // sb.Series.Print();
                //        list.Add(KeyValue.Create(ric, sb.Series));
                //    }
                //}

                //return Frame.FromRows(list);
                //return new DataFrame();
        }
        private DataFrame CreateFrame(SymbologySearchResponse response)
        {
            DataFrame TotalFrame = null;
            foreach (var symbol in response.mappedSymbols)
            {
                string ric = symbol.symbol;
                DataFrame f = new DataFrame();
                if(symbol.error != null)
                {
                    _logger?.LogInformation("Info: {0} {1}", ric, symbol.error);
                    continue;
                }
               
                f.Columns.Add(new StringDataFrameColumn("Security", new List<string> { ric }));

                FieldInfo[] fields = typeof(MappedSymbol).GetFields();
                string temp = "";
                foreach (var field in fields)
                {
                    temp = "";
                    if (field.FieldType == typeof(List<string>))
                    {
                        List<string> tmpList = (List<string>)field.GetValue(symbol);
                        if (tmpList != null) temp = string.Join(",", tmpList);
                    }
                    else if (field.FieldType == typeof(BestMatch))
                    {
                        if (symbol.bestMatch != null)
                        {
                            if (symbol.bestMatch.error != null)
                            {
                                _logger?.LogInformation("Info: {0} {1}", ric, symbol.bestMatch.error);
                                continue;
                            }
                            FieldInfo[] bestMatchFields = typeof(BestMatch).GetFields();
                            foreach (var bestMatchField in bestMatchFields)
                            {
                                var tmp = bestMatchField.GetValue(symbol.bestMatch)?.ToString();
                                if (!string.IsNullOrEmpty(tmp))
                                {
                                    f.Columns.Add(new StringDataFrameColumn("BestMatch." + bestMatchField.Name, new List<string> { tmp }));
                                }
                            }
                            continue;
                        }
                    }
                    else
                    {
                        temp = field.GetValue(symbol)?.ToString();
                    }
                    if (!string.IsNullOrEmpty(temp) && field.Name != "symbol")
                    {
                        f.Columns.Add(new StringDataFrameColumn(field.Name, new List<string> { temp }));
                            
                    }
                    


                }
                if (TotalFrame == null)
                {
                    TotalFrame = f;
                }
                else
                {
                    List<KeyValuePair<string, object>> list = new List<KeyValuePair<string, object>>();
                    foreach (var col in f.Columns)
                    {
                        KeyValuePair<string, object> kv = new KeyValuePair<string, object>(col.Name, col.Length > 0 ? col[0] : null);
                        list.Add(kv);
                        if (TotalFrame.Columns.Any(x => x.Name == col.Name) == false)
                        {
                           
                            TotalFrame.Columns.Add(new StringDataFrameColumn(col.Name, TotalFrame.Rows.Count));
                        }
                    }
                    TotalFrame = TotalFrame.Append(list);
                }

            }
            return TotalFrame == null ? new DataFrame() : TotalFrame;
            //List<KeyValuePair<string, Series<string, string>>> list = new List<KeyValuePair<string, Series<string, string>>>();
            //SeriesBuilder<string, string> sb;
            //foreach (var symbol in response.mappedSymbols)
            //{
            //    string ric = symbol.symbol;

            //    sb = new SeriesBuilder<string, string>();
            //    FieldInfo[] fields = typeof(MappedSymbol).GetFields();
            //    string temp = "";
            //    foreach (var field in fields)
            //    {
            //        temp = "";
            //        if (field.FieldType == typeof(List<string>))
            //        {
            //            List<string> tmpList = (List<string>)field.GetValue(symbol);
            //            if (tmpList != null) temp = string.Join(",", tmpList);
            //        }
            //        else if (field.FieldType == typeof(BestMatch))
            //        {
            //            if (symbol.bestMatch != null)
            //            {
            //                // sb = new SeriesBuilder<string, string>();
            //                FieldInfo[] bestMatchFields = typeof(BestMatch).GetFields();
            //                foreach (var bestMatchField in bestMatchFields)
            //                {
            //                    var tmp = bestMatchField.GetValue(symbol.bestMatch)?.ToString();

            //                    if (!string.IsNullOrEmpty(tmp))
            //                    {
            //                        sb.Add("BestMatch." + bestMatchField.Name, tmp);
            //                    }

            //                }
            //                continue;
            //            }
            //        }
            //        else
            //        {
            //            temp = field.GetValue(symbol)?.ToString();

            //        }

            //        if (!string.IsNullOrEmpty(temp) && field.Name != "symbol")
            //        {
            //            sb.Add(field.Name, temp);
            //        }

            //    }

            //    list.Add(KeyValue.Create(ric, sb.Series));

            //}
            //return Frame.FromRows(list);

            //return new DataFrame();
        }
        public DataFrame GetSymbology(IEnumerable<string> symbols,
            SymbologyType fromSymbolType = SymbologyType.RIC,
            IEnumerable<SymbologyType> toSymbolType = null,
            //    uint? limit = null,
            bool bestMatch = true)
        {
            var response = GetSymbologyRaw(symbols, fromSymbolType, toSymbolType, bestMatch);
            var responseObj = JsonConvert.DeserializeObject<SymbologySearchResponse>(response, new JsonSerializerSettings
            {
                Error = HandleDeserializationError,
                NullValueHandling = NullValueHandling.Ignore
            });


            if (bestMatch == true)
            {
                return CreateBestMatchFrame(responseObj);
            }
            else
            {
                return CreateFrame(responseObj);
            }
        }

        public DataFrame GetSymbology(string symbol,
            SymbologyType fromSymbolType = SymbologyType.RIC,
            IEnumerable<SymbologyType> toSymbolType = null,
            //   uint? limit = null,
            bool bestMatch = true)
        {
            return GetSymbology(new List<string> { symbol }, fromSymbolType, toSymbolType, bestMatch);
            

        }
    }
}