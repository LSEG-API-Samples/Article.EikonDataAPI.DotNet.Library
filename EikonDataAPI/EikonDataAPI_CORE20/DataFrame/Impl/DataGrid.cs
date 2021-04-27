using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.Analysis;
using Newtonsoft.Json;
using System.Linq;
using System.IO;

namespace EikonDataAPI
{
    public partial class DataGrid : EndPoint, IDataGrid
    {

        private Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
        private string CreateCSVFromDataResponse(DataResponse response)
        {
            StringBuilder sbuilder = new StringBuilder();

            sbuilder.AppendLine(string.Join(",",response.headers.First().Select(col => {
                return col.displayName;
            })));

            foreach(var row in response.data)
            {
                sbuilder.AppendLine(string.Join(",", row.Select(value => { return value.Value; })));
            }


            return sbuilder.ToString();
        }
        private DataFrame CreateDataFrame(DataResponse response)
        {
            if (response != null)
            {
                var str = CreateCSVFromDataResponse(response);
                var stream = GenerateStreamFromString(str);
                return DataFrame.LoadCsv(stream);
            }
            else
            {
                return new DataFrame();
            }
        }
        public DataFrame GetData(IEnumerable<string> instruments,
    IEnumerable<TRField> fields,
    Dictionary<string, string> parameters = null)
        {
            var responses = GetDataRaw(instruments, fields, parameters);

            var dataResponses = JsonConvert.DeserializeObject<DataResponses>(responses, new JsonSerializerSettings
            {
                Error = HandleDeserializationError
            });
            var response = dataResponses.responses[0];

            return CreateDataFrame(response);
        }

        public DataFrame GetData(IEnumerable<string> instruments,
            IEnumerable<string> fields,
            Dictionary<string, string> parameters = null)
        {
            var responses = GetDataRaw(instruments, fields, parameters);
            var dataResponses = JsonConvert.DeserializeObject<DataResponses>(responses, new JsonSerializerSettings
            {
                Error = HandleDeserializationError
            });
            var response = dataResponses.responses[0];

            return CreateDataFrame(response);
        }
        public DataFrame GetData(string instrument,
            IEnumerable<TRField> fields,
            Dictionary<string, string> parameters = null)
        {
            var responses = GetDataRaw(instrument, fields, parameters);
            var dataResponses = JsonConvert.DeserializeObject<DataResponses>(responses, new JsonSerializerSettings
            {
                Error = HandleDeserializationError
            });
            var response = dataResponses.responses[0];
            return CreateDataFrame(response);
        }
        public DataFrame GetData(string instrument,
            string field,
            Dictionary<string, string> parameters = null)
        {
            var responses = GetDataRaw(instrument, field, parameters);

            var dataResponses = JsonConvert.DeserializeObject<DataResponses>(responses, new JsonSerializerSettings
            {
                Error = HandleDeserializationError
            });
            var response = dataResponses.responses[0];
            return CreateDataFrame(response);
        }
    }
}
