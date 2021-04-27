using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.Analysis;
using Newtonsoft.Json;
using System.Linq;
namespace EikonDataAPI
{
    public partial class NewsHeadlines : EndPoint, INewsHeadlines
    {
        private DataFrame CreateFrame(NewsHeadlinesResponse response)
        {
            if (response == null) return new DataFrame() ;

            DataFrame headlinesFrame = new DataFrame();

            headlinesFrame.Columns.Add(new PrimitiveDataFrameColumn<DateTime>("FirstCreated", response.headlines.Select(h => h.firstCreated).ToList()));
            headlinesFrame.Columns.Add(new PrimitiveDataFrameColumn<DateTime>("VersionCreated", response.headlines.Select(h => h.versionCreated).ToList()));
            headlinesFrame.Columns.Add(new StringDataFrameColumn("Text", response.headlines.Select(h => h.text).ToList()));
            headlinesFrame.Columns.Add(new StringDataFrameColumn("StoryId", response.headlines.Select(h => h.storyId).ToList()));
            headlinesFrame.Columns.Add(new StringDataFrameColumn("SourceCode", response.headlines.Select(h => h.sourceCode).ToList()));




            return headlinesFrame;


        }
        public DataFrame GetNewsHeadlines(string query = "TOPALL AND LEN", uint count = 10)
        {
            var response = GetNewsHeadlinesRaw(query, count);

            return CreateFrame(JsonConvert.DeserializeObject<NewsHeadlinesResponse>(response, new JsonSerializerSettings
            {
                Error = HandleDeserializationError
            }));

        }
        public DataFrame GetNewsHeadlines(
            string query,
            uint? count,
            DateTime? dateFrom,
            DateTime? dateTo)
        {
            var response = GetNewsHeadlinesRaw(query, count, dateFrom, dateTo);

            return CreateFrame(JsonConvert.DeserializeObject<NewsHeadlinesResponse>(response, new JsonSerializerSettings
            {
                Error = HandleDeserializationError
            }));
        }

        public DataFrame GetNewsHeadlines(
            string query,
            uint? count,
            string dateFrom,
            string dateTo)
        {
            var response = GetNewsHeadlinesRaw(query, count, dateFrom, dateTo);

            return CreateFrame(JsonConvert.DeserializeObject<NewsHeadlinesResponse>(response, new JsonSerializerSettings
            {
                Error = HandleDeserializationError
            }));
        }
    }
}