using System;
using System.Collections.Generic;

namespace SCustomers.Models
{
    public class PagedList<T>
    {
        public PagedList(List<T> items,string query, int count, int pageNumber, int pageSize)
        {
            TotalCounts = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int) Math.Ceiling(count / (double)pageSize);
            Query = query;

            Data = items;
        }
        public List<T> Data { get; private set; }
        public string Query { get; private set; }
        public int CurrentPage { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCounts { get; private set; }
        public bool HasPrevious { get { return CurrentPage > 1; } }
        public bool HasNext { get { return CurrentPage < TotalPages; } }
    }
}
