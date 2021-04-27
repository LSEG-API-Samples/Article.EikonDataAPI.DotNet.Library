using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;


namespace EikonDataAPI
{
    public class NewsStory : EndPoint, INewsStory
    {
        public NewsStory(Profile profile, JSONRequest request)
        {
            _endPoint = "News_Story";
            _profile = profile;
            _logger = _profile.CreateLogger<NewsStory>();
            _jsonRequest = request;

        }
        public string GetNewsStory(string storyId)
        {
            var response = GetNewsStoryRaw(storyId);

            var responseObj = JsonConvert.DeserializeObject<NewsStoryResponse>(response, new JsonSerializerSettings
            {
                Error = HandleDeserializationError
            });

            return responseObj?.story?.storyHtml;
        }
        public string GetNewsStoryRaw(string storyId)
        {
           
           

            NewsStoryRequest request = new NewsStoryRequest
            {
                storyId = storyId
            };

            var response = SerializeAndSendRequest<NewsStoryRequest>(request);
            return response;
            //if (response == null) return null;


            //return JsonConvert.DeserializeObject<NewsStoryResponse>(response, new JsonSerializerSettings
            //{
            //    Error = HandleDeserializationError
            //});
        }

    }
    internal class NewsStoryRequest
    {
        public string storyId;
    }
    public class NewsStoryResponse
    {
        public Story story;
    }
    public class Story
    {
        public string headlineHtml;
        public string storyHtml;
        public string storyInfoHtml;
    }
    
}
