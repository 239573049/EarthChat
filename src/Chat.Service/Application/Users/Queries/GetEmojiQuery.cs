namespace Chat.Service.Application.Users.Queries;

public record GetEmojiQuery() : Query<IReadOnlyList<EmojiDto>>
{
    public override IReadOnlyList<EmojiDto> Result { get; set; }
}