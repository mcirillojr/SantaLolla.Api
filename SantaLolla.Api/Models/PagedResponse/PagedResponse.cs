using System.Text.Json.Serialization;

namespace SantaLolla.Api.Models.PagedResponse
{
    public class PagedResponse<T>
    {
        [JsonPropertyOrder(1)]
        public int TotalPages { get; set; }

        [JsonPropertyOrder(2)]
        public int CurrentPage { get; set; }

        [JsonPropertyOrder(3)]
        public int PageSize { get; set; }

        [JsonPropertyOrder(4)]
        public int Total { get; set; }

        [JsonPropertyOrder(5)]
        public IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();

        public static PagedResponse<T> Create(
            IEnumerable<T> data,
            int total,
            int currentPage,
            int pageSize)
        {
            return new PagedResponse<T>
            {
                TotalPages = pageSize > 0
                    ? (int)Math.Ceiling(total / (double)pageSize)
                    : 0,

                CurrentPage = currentPage,
                PageSize = pageSize,
                Total = total,
                Data = data
            };
        }
    }
}