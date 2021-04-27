using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.Analysis;
namespace EikonDataAPI
{
    public partial interface INewsHeadlines
    {
        DataFrame GetNewsHeadlines(string query = "TOPALL AND LEN", uint count = 10);
        DataFrame GetNewsHeadlines(
            string query,
            uint? count,
            DateTime? dateFrom,
            DateTime? dateTo);

        DataFrame GetNewsHeadlines(
            string query,
            uint? count,
            string dateFrom,
            string dateTo);
    }
}