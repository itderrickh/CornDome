namespace CornDome.Models
{
    public sealed class ApiCollectionResponse<T>
    {
        public required IReadOnlyList<T> Data { get; init; }

        public required double Elapsed { get; init; }

        public string? Next { get; init; }

        public required int Total { get; init; }

        public required int Count { get; init; }

        public required int Limit { get; init; }

        public required int Offset { get; init; }
    }
}
