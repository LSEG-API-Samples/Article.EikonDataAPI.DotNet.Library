using System;
using System.Collections.Generic;
using System.Text;

namespace EikonDataAPI
{
    public interface INewsStory
    {
        string GetNewsStoryRaw(string storyId);
        string GetNewsStory(string storyId);
    }
}
