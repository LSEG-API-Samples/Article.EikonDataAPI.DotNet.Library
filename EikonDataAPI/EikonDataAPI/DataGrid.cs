
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using System.Text;

namespace EikonDataAPI
{
    public partial class DataGrid : EndPoint, IDataGrid
    {
        

        public DataGrid(Profile profile, JSONRequest request) {
            //_endPoint = "DataGrid";
            _endPoint = "DataGrid_StandardAsync";
            _profile = profile;
            _logger = _profile.CreateLogger<DataGrid>();
            _jsonRequest = request;
        
        }

        private string WaitTicket(string response)
        {
            JObject json = JObject.Parse(response);

            if (json["responses"][0]["estimatedDuration"] != null)
            {
                string sleeptime = json["responses"][0]["estimatedDuration"].ToString();
                _logger.LogDebug($"Sleep {sleeptime}");
                System.Threading.Thread.Sleep(Int32.Parse(sleeptime));
            }



            if (json["responses"][0]["ticket"] != null)
            {
                string ticket = (string)json["responses"][0]["ticket"].ToString();
                string nextReq = $@"
                    {{""requests"": [{{""ticket"": ""{ticket}""}}]}}";              

                return WaitTicket(SendJSONRequest(nextReq));
            }
            return response;


        }
        public string GetDataRaw(
            string instrument,
            string field,            
            Dictionary<string, string> parameters = null)
        {
            //Return it as DataTable;


            DataGridRequest request = new DataGridRequest { instruments = new List<string>{ instrument }};
            request.parameters = MergeParameters(parameters);


            request.fields = new List<TRField>();

            request.fields.Add(TRField(field));

            DataGridRequests requests = new DataGridRequests();
            requests.requests = new List<DataGridRequest>();
            requests.requests.Add(request);


            //var response = SerializeAndSendRequest<DataGridRequest>(request);
            var response = SerializeAndSendRequest<DataGridRequests>(requests);

            return WaitTicket(response);
            
            //return JsonConvert.DeserializeObject<DataResponse>(response, new JsonSerializerSettings
            //{
            //    Error = HandleDeserializationError
            //});
        }
        //public void CreateLogger(ILoggerFactory factory)
        //{
        //    _logger = factory.CreateLogger<DataGrid>();
        //}
        private void SelectType(ref Type colType, JTokenType JsonType)
        {
            switch (JsonType)
            {
                case JTokenType.Integer:
                    if (colType == typeof(DBNull)) colType = typeof(Int64);
                    else if (colType == typeof(Int64)) colType = typeof(Int64);
                    else if (colType == typeof(Double)) colType = typeof(Double);
                    else colType = typeof(string);
                    break;
                case JTokenType.Float:
                    if (colType == typeof(DBNull)) colType = typeof(Double);
                    else if(colType == typeof(Double)) colType = typeof(Double);
                    else if(colType==typeof(Int64)) colType = typeof(Double);
                    else colType = typeof(string);
                    break;
                case JTokenType.Boolean:
                    if (colType == typeof(DBNull)) colType = typeof(Boolean);
                    else if (colType == typeof(Boolean)) colType = typeof(Boolean);                   
                    else colType = typeof(string);
                    break;
                case JTokenType.String:
                    colType = typeof(string);
                    break;
                case JTokenType.Date:
                    if (colType == typeof(DBNull)) colType = typeof(DateTime);
                    else if (colType == typeof(DateTime)) colType = typeof(DateTime);
                    else colType = typeof(string);
                    break;
            }
        }
        private void FindColumnType(ref Type[] colType, DataResponse response)
        {
            for (int i = 0; i < response.totalColumnsCount; i++)
            {
                colType[i] = typeof(DBNull);
            }
            foreach (var row in response.data)
            {
               
                for(int i=0;i<row.Count;i++)
                {
                    if (colType[i] == typeof(string)) continue;
                    SelectType(ref colType[i], row[i].Type);
                }
                  
            }
            for (int i = 0; i < response.totalColumnsCount; i++)
            {
                if (colType[i] == typeof(DBNull)) colType[i] = typeof(string);
                Console.WriteLine(colType[i]);
            }


        }
        private DataTable CreateDataTable(DataResponse response)
        {
            DataTable dt = new DataTable();
            dt.Clear();
            
            Type[] colType = new Type[response.totalColumnsCount];
            int i = 0;
            FindColumnType(ref colType, response);

            foreach (var header in response.headers)
            {
                foreach(Column col in header)
                {
                    dt.Columns.Add(col.displayName, colType[i++]);
                }
            }
            foreach(var data in response.data)
            {
                DataRow row = dt.NewRow();
                i = 0;
                foreach (var val in data)
                {

                    row[i] = val.ToObject(colType[i]);
                    i++;
                }
                dt.Rows.Add(row);
            }

            dt.ToString();
            return dt;
          //  throw new NotImplementedException();
        }
        public string GetDataRaw(
            IEnumerable<string> instruments,
            IEnumerable<string> fields,
            Dictionary<string, string> parameters = null)
        {
            DataGridRequest request = new DataGridRequest { instruments = instruments.ToList()};
            request.parameters = MergeParameters(parameters);

            request.fields = new List<TRField>();
            foreach (var field in fields)
            {
                request.fields.Add(TRField(field));
            }
            DataGridRequests requests = new DataGridRequests();
            requests.requests = new List<DataGridRequest>();
            requests.requests.Add(request);


            //var response = SerializeAndSendRequest<DataGridRequest>(request);
            var response = SerializeAndSendRequest<DataGridRequests>(requests);

            return WaitTicket(response);


            //return JsonConvert.DeserializeObject<DataResponse>(response, new JsonSerializerSettings
            //{
            //    Error = HandleDeserializationError
            //});


        }

        private JObject MergeParameters (Dictionary<string, string> parameters)
        {
            JObject obj  = null;
            //if (parameters != null)
            //{
            //    obj= JObject.FromObject(parameters, new JsonSerializer { NullValueHandling = NullValueHandling.Ignore });
            //}
            if(parameters != null)
            {
                if (obj != null)
                {
                    obj.Merge(JObject.FromObject(parameters), new JsonMergeSettings
                    {
                        MergeArrayHandling = MergeArrayHandling.Union
                    });
                }
                else
                {
                    obj = JObject.FromObject(parameters, new JsonSerializer { NullValueHandling = NullValueHandling.Ignore });
                }
            }

            return obj;
        }
        public string GetDataRaw(
            IEnumerable<string> instruments,
            IEnumerable<TRField> fields,
            Dictionary<string, string> parameters = null)
        {
            
            DataGridRequest request = new DataGridRequest { instruments = instruments.ToList(), fields = fields.ToList()};
            request.parameters = MergeParameters(parameters);

            DataGridRequests requests = new DataGridRequests();
            requests.requests = new List<DataGridRequest>();
            requests.requests.Add(request);


            //var response = SerializeAndSendRequest<DataGridRequest>(request);
            var response = SerializeAndSendRequest<DataGridRequests>(requests);
            return WaitTicket(response);


        }

        public string GetDataRaw(
            string instrument,
            IEnumerable<TRField> fields,           
            Dictionary<string, string> parameters = null)
        {
            DataGridRequest request = new DataGridRequest { instruments = new List<string>{ instrument }, fields = fields.ToList()};
            request.parameters = MergeParameters(parameters);
            DataGridRequests requests = new DataGridRequests();
            requests.requests = new List<DataGridRequest>();
            requests.requests.Add(request);


            //var response = SerializeAndSendRequest<DataGridRequest>(request);
            var response = SerializeAndSendRequest<DataGridRequests>(requests);
            return WaitTicket(response);

        }



        public TRField TRField(string fieldname, Dictionary<string, string> parameters = null, SortField? sortField = null, int? priority = null)
        {          
            return new TRField { name = fieldname, parameters=parameters, sort=sortField, sortPriority=priority };
        }

    }
    internal class DataGridRequests
    {
        public List<DataGridRequest> requests;
    }
    internal class DataGridRequest
    {
        public List<string> instruments;
        public List<TRField> fields;
        public DataGridInclude include;        
        public JObject parameters;
        //public Dictionary<string, JValue> parameters;
        //public string parameters;
    }
    public class DataGridInclude
    {
        public bool streamingParameters;
    }
    public class TRField 
    {
       // public string label;
        public string name;
        public Dictionary<string, string> parameters;
        [JsonConverter(typeof(StringEnumConverter))]
        public SortField? sort;
        public int? sortPriority;
       

    }
    public enum SortField { none, asc, desc }

    public class DataResponses
    {
        public List<DataResponse> responses;
    }
    public class DataResponse 
    {
        public int columnHeadersCount;
        public int rowHeadersCount;
        public int totalColumnsCount;
        public int totalRowsCount;
        public string headerOrientation;
        public List<List<Column>> headers;
        public List<List<JValue>> data;
        
    }
    public class Column
    {
        public string displayName;
        public string field;
        public string label;
    }
    //public class DataGridParameters
    //{
    //    public string PERIOD;
    //    public string SDate;
    //    public string EDate;
    //    public string FRQ;
    //    public bool? RollPeriods;
    //    public string AlignType;
    //    public string Reporting­State;
    //    public string CURN;
    //    public UInt16? SCALE;
    //    public string Methodology;
    //    public string PARCON;
    //    public string OutputOp­tions;
    //    public string BROKER;
    //    public string CH;
    //    public string CODE;
    //    public string CONVERTCODE;
    //    public string END;
    //    public string UPDFRQ;
    //    public string IGNE;
    //    public string LIVE;
    //    public string NULL;
    //    public string ONTIME;
    //    public string RET;
    //    public string RH;
    //    public string RTFEED;
    //    public string SERIES;
    //    public string SKIP;
    //    public string SORT;
    //    public string START;
    //    public string TRANSPOSE;
    //    public string TRIM;
    //    public string TYPE;
    //    public string UWC;
    //}
}
