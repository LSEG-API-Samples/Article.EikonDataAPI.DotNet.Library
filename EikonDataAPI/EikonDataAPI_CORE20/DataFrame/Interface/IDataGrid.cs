using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.Analysis;

namespace EikonDataAPI
{
    public partial interface IDataGrid
    {

        DataFrame GetData(IEnumerable<string> instruments,
            IEnumerable<TRField> fields,
            Dictionary<string, string> parameters = null);

        DataFrame GetData(IEnumerable<string> instruments,
            IEnumerable<string> fields,
            Dictionary<string, string> parameters = null);
        DataFrame GetData(string instrument,
            IEnumerable<TRField> fields,
            Dictionary<string, string> parameters = null);
        DataFrame GetData(string instrument,
            string field,
            Dictionary<string, string> parameters = null);


    }
}

