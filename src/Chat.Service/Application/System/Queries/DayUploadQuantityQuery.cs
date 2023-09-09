namespace Chat.Service.Application.System.Queries;

public record DayUploadQuantityQuery(Guid UserId) : Query<long>
{
    public override long Result { get; set; }
} 