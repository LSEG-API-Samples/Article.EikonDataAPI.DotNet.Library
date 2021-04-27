
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace EikonDataAPI
{
    public partial interface IDataGrid
    {
        TRField TRField(string fieldname,
            Dictionary<string, string> parameters = null,
            SortField? sortField = null,
            int? priority = null);

        string GetDataRaw(IEnumerable<string> instruments,
            IEnumerable<TRField> fields,
            Dictionary<string, string> parameters = null);

        string GetDataRaw(IEnumerable<string> instruments,
            IEnumerable<string> fields,
            Dictionary<string, string> parameters = null);
        string GetDataRaw(string instrument,
            IEnumerable<TRField> fields,
            Dictionary<string, string> parameters = null);
        string GetDataRaw(string instrument,
            string field,
            Dictionary<string, string> parameters = null);
    }
}
