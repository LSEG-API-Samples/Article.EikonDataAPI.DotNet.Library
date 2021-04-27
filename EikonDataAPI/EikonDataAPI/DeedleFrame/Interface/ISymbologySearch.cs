using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deedle;


namespace EikonDataAPI
{
    public partial interface ISymbologySearch
    {
        Frame<string, string> GetSymbology(IEnumerable<string> symbols,
            SymbologyType fromSymbolType = SymbologyType.RIC,
            IEnumerable<SymbologyType> toSymbolType = null,
        //    uint? limit = null,
            bool bestMatch = true);

        Frame<string, string> GetSymbology(string symbol,
            SymbologyType fromSymbolType = SymbologyType.RIC,
            IEnumerable<SymbologyType> toSymbolType = null,
         //   uint? limit = null,
            bool bestMatch = true);
    }
}
