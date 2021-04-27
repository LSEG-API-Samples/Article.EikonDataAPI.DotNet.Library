
using System;
using System.Collections.Generic;
using System.Text;

namespace EikonDataAPI
{
    public partial interface ISymbologySearch
    {
        string GetSymbologyRaw(IEnumerable<string> symbols,
            SymbologyType fromSymbolType = SymbologyType.RIC,
            IEnumerable<SymbologyType> toSymbolType = null,
           // uint? limit =null,
            bool bestMatch = true);

        string GetSymbologyRaw(string symbol,
            SymbologyType fromSymbolType = SymbologyType.RIC,
            IEnumerable<SymbologyType> toSymbolType = null,
         //   uint? limit = null,
            bool bestMatch = true);

    }
}
