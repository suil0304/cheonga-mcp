namespace cheongdo_mcp.Education;

public sealed record SourceMetadata(
    string Provider,
    Uri? SourceUrl,
    DateTimeOffset? UpdatedAt,
    bool IsMock);

public sealed record DigitalLearningMaterial(
    string Id,
    string Title,
    string Summary,
    string Category,
    IReadOnlyList<string> AppliesTo,
    IReadOnlyList<string> Steps,
    SourceMetadata Source);

public sealed record DigitalLearningCenter(
    string Id,
    string Name,
    string Region,
    string Type,
    string Address,
    string Status,
    SourceMetadata Source);

public sealed record WelfareProgram(
    string Id,
    string Name,
    string Topic,
    string Region,
    string TargetCategory,
    string Eligibility,
    string ApplicationMethod,
    SourceMetadata Source);

public sealed record MockToolResponse<T>(
    string Status,
    string Notice,
    bool IsMock,
    T Data);
