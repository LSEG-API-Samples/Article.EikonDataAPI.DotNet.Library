using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deedle;
using Newtonsoft.Json;

namespace EikonDataAPI
{
    public partial class NewsHeadlines : EndPoint, INewsHeadlines
    {
        private Frame<int, string> CreateFrame(NewsHeadlinesResponse response)
        {
            if (response == null) return Frame.CreateEmpty<int, string>();

            Frame<int, string> headlinesFrame = Frame.CreateEmpty<int, string>();

            headlinesFrame.AddColumn("FirstCreated", CreateSeriesDateTime(response.headlines.Select(h => h.firstCreated).ToList(), 0));
            headlinesFrame.AddColumn("VersionCreated", CreateSeriesDateTime(response.headlines.Select(h => h.versionCreated).ToList(), 0));
            headlinesFrame.AddColumn("Text", CreateSeriesString(response.headlines.Select(h => h.text).ToList(), 0));
            headlinesFrame.AddColumn("StoryId", CreateSeriesString(response.headlines.Select(h => h.storyId).ToList(), 0));
            headlinesFrame.AddColumn("SourceCode", CreateSeriesString(response.headlines.Select(h => h.sourceCode).ToList(), 0));



            return headlinesFrame;


        }

        private Series<int, string> CreateSeriesString(List<string> list, int startIndex)
        {
            List<KeyValuePair<int, string>> seriesBuilder = new List<KeyValuePair<int, string>>();

            for (int i = 0; i < list.Count(); i++)
            {
                seriesBuilder.Add(new KeyValuePair<int, string>(startIndex + i, list[i]));
            }
            Series<int, string> s1 = new Series<int, string>(seriesBuilder);
            return s1;

        }

        private Series<int, DateTime> CreateSeriesDateTime(List<DateTime> list, int startIndex)
        {
            List<KeyValuePair<int, DateTime>> seriesBuilder = new List<KeyValuePair<int, DateTime>>();

            for (int i = 0; i < list.Count(); i++)
            {

                seriesBuilder.Add(new KeyValuePair<int, DateTime>(startIndex + i, list[i]));
            }
            Series<int, DateTime> s1 = new Series<int, DateTime>(seriesBuilder);
            return s1;

        }

        public Frame<int, string> GetNewsHeadlines(string query = "TOPALL AND LEN", uint count = 10)
        {

            var response = GetNewsHeadlinesRaw(query, count);

            return CreateFrame(JsonConvert.DeserializeObject<NewsHeadlinesResponse>(response, new JsonSerializerSettings
            {
                Error = HandleDeserializationError
            }));
        }
        public Frame<int, string> GetNewsHeadlines(
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

        public Frame<int, string> GetNewsHeadlines(
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
