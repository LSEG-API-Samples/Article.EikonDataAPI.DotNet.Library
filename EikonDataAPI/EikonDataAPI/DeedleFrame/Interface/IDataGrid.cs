using Deedle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EikonDataAPI
{
    public partial interface IDataGrid
    {

        Frame<int, string> GetData(IEnumerable<string> instruments,
IEnumerable<TRField> fields,
Dictionary<string, string> parameters = null);

        Frame<int, string> GetData(IEnumerable<string> instruments,
            IEnumerable<string> fields,
            Dictionary<string, string> parameters = null);
        Frame<int, string> GetData(string instrument,
            IEnumerable<TRField> fields,
            Dictionary<string, string> parameters = null);
        Frame<int, string> GetData(string instrument,
            string field,
            Dictionary<string, string> parameters = null);


    }
}
