namespace Chat.Contracts.Eto.Semantic;

public class IntelligentAssistantEto
{
    /// <summary>
    /// 是否群组。
    /// </summary>
    public bool Group { get; set; }
    
    /// <summary>
    /// 内容
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// 发送人Id
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// 链接Id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 提问内容Id
    /// </summary>
    public Guid RevertId { get; set; }
}