using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.Analysis;
namespace EikonDataAPI
{
    public partial interface ISymbologySearch
    {
        DataFrame GetSymbology(IEnumerable<string> symbols,
            SymbologyType fromSymbolType = SymbologyType.RIC,
            IEnumerable<SymbologyType> toSymbolType = null,
        //    uint? limit = null,
            bool bestMatch = true);

        DataFrame GetSymbology(string symbol,
            SymbologyType fromSymbolType = SymbologyType.RIC,
            IEnumerable<SymbologyType> toSymbolType = null,
            //   uint? limit = null,
            bool bestMatch = true);
    }
}
