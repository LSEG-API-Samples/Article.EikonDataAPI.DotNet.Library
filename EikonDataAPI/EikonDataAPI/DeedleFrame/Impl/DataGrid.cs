using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deedle;
using Newtonsoft.Json;

namespace EikonDataAPI
{
    public partial class DataGrid : EndPoint,  IDataGrid
    {
        private bool HasFloatInColumn(DataResponse response, int column)
        {
           
            for (int i = 0; i < response.data.Count; i++)
            {
                if(response.data[i][column].Type == Newtonsoft.Json.Linq.JTokenType.Float)
                {
                    return true;
                }
            }
            return false;
        }

        private void CheckAndConvertFirstIntToFloat(DataResponse response)
        {
            if (response.data.Count >= 0)
            {
                for (int j = 0; j < response.headers.First().Count(); j++)
                {
                   // if(response.data[0][j].Type == Newtonsoft.Json.Linq.JTokenType.Integer)
                   //Fix the problem when column contains float but the first item is integer or missing (string)
                    {
                        if (HasFloatInColumn(response, j))
                        {
                            if (response.data[0][j].Type == Newtonsoft.Json.Linq.JTokenType.Integer)
                            {
                                //When the first item is integer
                                response.data[0][j].Value = (double)(response.data[0][j].ToObject<int>() * 1.0);
                            }
                            else
                            {
                                //When the first item is missing (string).
                                for(int i = 1;i < response.data.Count; i++)
                                {
                                    if (response.data[i][j].Type == Newtonsoft.Json.Linq.JTokenType.Integer)
                                    {
                                        response.data[i][j].Value = (double)(response.data[i][j].ToObject<int>() * 1.0);
                                        break;
                                    }
                                }
                            }
                        }

                        // if(response.data[0][j].ToObject<int>() == 0)
                        //{
                        //   if(HasFloatInColumn(response, j))
                        //   {
                        //       response.data[0][j].Value = 0.0;
                        //  }
                        //}

                    }
                }
            }
        }
        private Frame<int, string> CreateFrame(DataResponse response)
        {
            if (response != null)
            {
                CheckAndConvertFirstIntToFloat(response);
                var rows = Enumerable.Range(0, response.data.Count).Select(i =>
                {
                    // Build each row using series builder & return 
                    // KeyValue representing row key with row data
                    var sb = new SeriesBuilder<string>();
                    for (int j = 0; j < response.headers.First().Count(); j++)
                    {
                        string displayName;
                        if (string.IsNullOrEmpty(response.headers.First()[j].displayName))
                        {
                            displayName = "None";
                        }
                        else
                        {
                            displayName = response.headers.First()[j].displayName;
                        }
                       
                        sb.Add(displayName, response.data[i][j].Value);
                       


                    }
                   
                    return KeyValue.Create(i, sb.Series);
                });

                return Frame.FromRows(rows);
            }
            else
            {
                return Frame.CreateEmpty<int, string>();
            }
        }

        //public static Frame<int, string> GetDataFrame(DataResponse response)
        //{
        //    CreateLogger(eikon);
        //    if (response == null) return Frame.CreateEmpty<int, string>();
        //    else return CreateFrame(response);
        //}
        public Frame<int, string> GetData(string instrument,
            string field,
            // DataGridParameters parameters,
            Dictionary<string, string> parameters = null)
        {

            var responses = GetDataRaw(instrument, field, parameters);

            var dataResponses = JsonConvert.DeserializeObject<DataResponses>(responses, new JsonSerializerSettings
            {
                Error = HandleDeserializationError
            });
            var response = dataResponses.responses[0];
            return CreateFrame(response);

            //return CreateFrame(JsonConvert.DeserializeObject<DataResponse>(response, new JsonSerializerSettings
            //{
            //    Error = HandleDeserializationError
            //}));
        }

        public Frame<int, string> GetData(IEnumerable<string> instruments,
    IEnumerable<TRField> fields,
    // DataGridParameters parameters,
    Dictionary<string, string> parameters = null)
        {

            var responses = GetDataRaw(instruments, fields, parameters);

            var dataResponses = JsonConvert.DeserializeObject<DataResponses>(responses, new JsonSerializerSettings
            {
                Error = HandleDeserializationError
            });
            var response = dataResponses.responses[0];
            return CreateFrame(response);
            //return CreateFrame(JsonConvert.DeserializeObject<DataResponse>(response, new JsonSerializerSettings
            //{
            //    Error = HandleDeserializationError
            //}));

        }

        public Frame<int, string> GetData(IEnumerable<string> instruments,
            IEnumerable<string> fields,
            //DataGridParameters parameters,
            Dictionary<string, string> parameters = null)
        {

            var responses = GetDataRaw(instruments, fields, parameters);
            var dataResponses = JsonConvert.DeserializeObject<DataResponses>(responses, new JsonSerializerSettings
            {
                Error = HandleDeserializationError
            });
            var response = dataResponses.responses[0];
            return CreateFrame(response);
            //return CreateFrame(JsonConvert.DeserializeObject<DataResponse>(response, new JsonSerializerSettings
            //{
            //    Error = HandleDeserializationError
            //}));
        }
        public Frame<int, string> GetData(string instrument,
            IEnumerable<TRField> fields,
            //DataGridParameters parameters,
            Dictionary<string, string> parameters = null)
        {

            var responses = GetDataRaw(instrument, fields, parameters);
            var dataResponses = JsonConvert.DeserializeObject<DataResponses>(responses, new JsonSerializerSettings
            {
                Error = HandleDeserializationError
            });
            var response = dataResponses.responses[0];
            return CreateFrame(response);
            //return CreateFrame(JsonConvert.DeserializeObject<DataResponse>(response, new JsonSerializerSettings
            //{
            //    Error = HandleDeserializationError
            //}));
        }
    }
}
