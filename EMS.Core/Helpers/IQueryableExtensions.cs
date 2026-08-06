using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
namespace EMS.Core.Helpers
{
    public static class IQueryableExtensions
    {
        public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
            this IQueryable<T> query,
            PaginationRequest request,
            CancellationToken cancellationToken = default)
        {
            var totalRecords = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip(request.Skip)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<T>
            {
                Items = items,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalRecords = totalRecords
            };
        }
    }
}
