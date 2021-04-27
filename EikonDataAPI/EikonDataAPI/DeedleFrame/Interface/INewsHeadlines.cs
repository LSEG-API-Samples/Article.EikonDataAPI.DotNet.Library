using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deedle;

namespace EikonDataAPI
{
    public partial interface INewsHeadlines
    {
        Frame<int, string> GetNewsHeadlines(string query = "TOPALL AND LEN", uint count = 10);
        Frame<int, string> GetNewsHeadlines(
            string query,
            uint? count,
            DateTime? dateFrom,
            DateTime? dateTo);

        Frame<int, string> GetNewsHeadlines(
            string query,
            uint? count,
            string dateFrom,
            string dateTo);
    }
}
