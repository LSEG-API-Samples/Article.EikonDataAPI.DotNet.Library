using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;

namespace EikonDataAPI
{
    public partial class SymbologySearch : EndPoint, ISymbologySearch
    {
        public SymbologySearch(Profile profile, JSONRequest request)
        {
            _endPoint = "SymbologySearch";
            _profile = profile;
            _logger = _profile.CreateLogger<SymbologySearch>();
            _jsonRequest = request;

        }
        public string GetSymbologyRaw(IEnumerable<string> symbols,
            SymbologyType fromSymbolType = SymbologyType.RIC,
            IEnumerable<SymbologyType> toSymbolType = null,
       //     uint? limit = null,
            bool bestMatch = true)
        {
            
            SymbologySearchRequest request = new SymbologySearchRequest
            {
                symbols = symbols.ToList(),
                from = fromSymbolType,
                to = toSymbolType?.ToList(),
                limit = null,
                bestMatchOnly = bestMatch

            };

            var response = SerializeAndSendRequest<SymbologySearchRequest>(request);
            return response;
        }

        public string GetSymbologyRaw(string symbol,
            SymbologyType fromSymbolType = SymbologyType.RIC,
            IEnumerable<SymbologyType> toSymbolType = null,
         //   uint? limit = null,
            bool bestMatch = true)
        {
            return GetSymbologyRaw(new List<string> { symbol }, fromSymbolType, toSymbolType, bestMatch);
        }

    }
    internal class SymbologySearchRequest
    {
        public bool? bestMatchOnly;
        [JsonConverter(typeof(StringEnumConverter))]
        public SymbologyType from;
        [JsonProperty("to", ItemConverterType = typeof(StringEnumConverter))]
        public List<SymbologyType> to;
        public uint? limit;
        public List<string> symbols;

    }
    public class SymbologySearchResponse
    {
        public List<MappedSymbol> mappedSymbols;
     

    }
    public class BestMatch
    {
        public string CUSIP;
        public string IMO;
        public string ISIN;
        public string OAPermID;
        public string RIC;
        public string SEDOL;
        public string error;
        public string lipperID;
        public string primaryRIC;
        public string ticker;
    }
    public class MappedSymbol
    {
        public List<string> CUSIPs;
        public List<string> IMOs;
        public List<string> ISINs;
        public List<string> OAPermIDs;
        public List<string> RICs;
        public List<string> SEDOLs;
        public List<string> lipperIDs;
        public List<string> primaryRICs;
        public List<string> tickers;
        public BestMatch bestMatch;
        public string symbol;
        public string error;
    }
    public enum SymbologyType { RIC,ISIN,CUSIP,SEDOL,ticker,lipperID,OAPermID,IMO}
}
