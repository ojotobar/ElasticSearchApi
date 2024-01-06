namespace ElasticSearchApi.Entities.DTO
{
    public class PaginatedListDto<T>
    {
        public MetaData? MetaData { get; set; }
        public IEnumerable<T> Data { get; set; }
        public PaginatedListDto()
        {
            Data = new List<T>();
        }

        public static PaginatedListDto<T> Paginate(IEnumerable<T> data, MetaData metaData)
        {
            return new PaginatedListDto<T>
            {
                Data = data,
                MetaData = metaData
            };
        }
    }
}
