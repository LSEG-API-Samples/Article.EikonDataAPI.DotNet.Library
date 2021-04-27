
using System;
using System.Collections.Generic;
using System.Text;

namespace EikonDataAPI
{
    public partial interface INewsHeadlines
    {
      //  NewsHeadlinesResponse GetNewsHeadlinesRaw(string token);
        string GetNewsHeadlinesRaw(string query = "TOPALL AND LEN", uint count = 10);
        string GetNewsHeadlinesRaw(
            string query,
            uint? count,
            DateTime? dateFrom,
            DateTime? dateTo);

        string GetNewsHeadlinesRaw(
            string query,
            uint? count,
            string dateFrom,
            string dateTo);


    }
}
