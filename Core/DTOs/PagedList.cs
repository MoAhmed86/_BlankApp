using System;
using System.Collections.Generic;
using System.Text;

namespace Core.DTOs
{
    public class PagedModel<T>
    {
        public PagingModel PagingModel { get; set; }
        public T Data { get; set; }
    }

    public class PagedListModel<T>
    {
        public PagingModel PagingModel { get; set; }
        public IList<T> ListObj { get; set; } = new List<T>();
    }

    public class PagingModel
    {
        public int CurrentPageInex { get; set; }
        public int PagesCount { get; set; }
        public int ItemsCount { get; set; }
    }
}
