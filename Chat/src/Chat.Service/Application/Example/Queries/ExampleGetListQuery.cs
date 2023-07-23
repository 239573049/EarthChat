namespace Chat.Service.Application.Example.Queries;

public record ExampleGetListQuery(string? keyword, string? sort, int pageIndex = 1, int pageDataCount = 10) : Query<PaginatedListBase<ExampleGetListDto>>
{
    public override PaginatedListBase<ExampleGetListDto> Result { get; set; }
}
