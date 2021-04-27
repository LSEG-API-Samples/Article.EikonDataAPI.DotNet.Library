using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deedle;
using Newtonsoft.Json;
using System.Reflection;

namespace EikonDataAPI
{
    public partial class SymbologySearch : EndPoint, ISymbologySearch
    {
        private Frame<string, string> CreateFrame(SymbologySearchResponse response)
        {
            List<KeyValuePair<string, Series<string, string>>> list = new List<KeyValuePair<string, Series<string, string>>>();
            SeriesBuilder<string, string> sb;
            foreach (var symbol in response.mappedSymbols)
            {
                string ric = symbol.symbol;

                sb = new SeriesBuilder<string, string>();
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
                            // sb = new SeriesBuilder<string, string>();
                            FieldInfo[] bestMatchFields = typeof(BestMatch).GetFields();
                            foreach (var bestMatchField in bestMatchFields)
                            {
                                var tmp = bestMatchField.GetValue(symbol.bestMatch)?.ToString();

                                if (!string.IsNullOrEmpty(tmp))
                                {
                                    sb.Add("BestMatch." + bestMatchField.Name, tmp);
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
                        sb.Add(field.Name, temp);
                    }

                }

                list.Add(KeyValue.Create(ric, sb.Series));

            }
            return Frame.FromRows(list);

        }
        private Frame<string, string> CreateBestMatchFrame(SymbologySearchResponse response)
        {

            List<KeyValuePair<string, Series<string, string>>> list = new List<KeyValuePair<string, Series<string, string>>>();
            foreach (var symbol in response.mappedSymbols)
            {
                string ric = symbol.symbol;
                SeriesBuilder<string, string> sb;

                if (symbol.bestMatch != null)
                {
                    sb = new SeriesBuilder<string, string>();
                    FieldInfo[] fields = typeof(BestMatch).GetFields();
                    foreach (var field in fields)
                    {
                        var tmp = field.GetValue(symbol.bestMatch)?.ToString();

                        if (!string.IsNullOrEmpty(tmp))
                        {
                            sb.Add(field.Name, field.GetValue(symbol.bestMatch).ToString());
                        }

                    }
                    // sb.Series.Print();
                    list.Add(KeyValue.Create(ric, sb.Series));
                }
            }

            return Frame.FromRows(list);
        }
        //public static Frame<string, string> GetSymbology(this IEikon eikon, SymbologySearchResponse response, bool bestMatch = true)
        //{
        //    if (bestMatch == true)
        //    {
        //        return CreateBestMatchFrame(response);
        //    }
        //    else
        //    {
        //        return CreateFrame(response);
        //    }
        //}
       public Frame<string, string> GetSymbology(string symbol,
            SymbologyType fromSymbolType = SymbologyType.RIC,
            IEnumerable<SymbologyType> toSymbolType = null,
      //      uint? limit = null,
            bool bestMatch = true)
        {
            return GetSymbology(new List<string> { symbol }, fromSymbolType, toSymbolType, bestMatch);
        }
        public Frame<string, string> GetSymbology(IEnumerable<string> symbols,
            SymbologyType fromSymbolType = SymbologyType.RIC,
            IEnumerable<SymbologyType> toSymbolType = null,
           // uint? limit = null,
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

            // Frame.FromRecords()

        }
    }
}
