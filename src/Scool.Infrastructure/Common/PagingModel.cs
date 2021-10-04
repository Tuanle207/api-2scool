using System.Collections.Generic;

namespace Scool.Infrastructure.Common
{
    public class PagingModel<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
        public PagingModel(IEnumerable<T> items, int totalCount)
        {
            Items = items;
            TotalCount = totalCount;
        }
        public PagingModel(IEnumerable<T> items, int totalCount, int pageIndex, int pageSize)
        {
            Items = items;
            TotalCount = totalCount;
            PageIndex = pageIndex;
            PageSize = pageSize;
        }
    }
}