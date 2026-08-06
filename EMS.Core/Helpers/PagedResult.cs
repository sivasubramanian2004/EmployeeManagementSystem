using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Core.Helpers
{
    public class PagedResult<T>
    {
        public IReadOnlyList<T> Items { get; init; } = [];

        public int PageNumber { get; init; }

        public int PageSize { get; init; }

        public int TotalRecords { get; init; }

        public int TotalPages =>
            (int)Math.Ceiling((double)TotalRecords / PageSize);

        public bool HasPreviousPage => PageNumber > 1;

        public bool HasNextPage => PageNumber < TotalPages;
    }
}
