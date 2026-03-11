using System;
using System.Collections.Generic;

namespace BaseApi.Domain.Common;

public class PagedResult<T> : Result<IEnumerable<T>>
{
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;

    public static PagedResult<T> Success(IEnumerable<T> data, int count, int pageNumber, int pageSize, string message = "")
    {
        return new PagedResult<T>
        {
            IsSuccess = true,
            Data = data,
            TotalCount = count,
            PageSize = pageSize,
            CurrentPage = pageNumber,
            TotalPages = (int)Math.Ceiling(count / (double)pageSize),
            Message = message
        };
    }
}
