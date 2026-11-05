using System.ComponentModel;
using System.Text.Json;
using System.Text.RegularExpressions;
using ModelContextProtocol;
using ModelContextProtocol.Server;

namespace cheongdo_mcp.Education;

[McpServerToolType]
public sealed class EducationTools(IEducationDataProvider dataProvider)
{
    private const int DefaultPageSize = 5;
    private const int MaximumPageSize = 20;
    private const string MockNotice =
        "개발용 Mock 응답입니다. 공식 자료나 실제 주소, 자격 판단, 신청 안내로 사용하지 마세요.";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private static readonly Regex ObviousIdentifierPattern = new(
        @"(?:\b[\w.+-]+@[\w.-]+\.[A-Za-z]{2,}\b|\b\d{2,3}[- ]?\d{3,4}[- ]?\d{4}\b|\b\d{6}[- ]?[1-4]\d{6}\b)",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly HashSet<string> EligibilityCategories = new(
        ["general", "senior", "low_income", "disability"],
        StringComparer.OrdinalIgnoreCase);

    [McpServerTool(
        Name = "search_digital_learning_materials",
        Title = "공식 디지털 교육 자료 검색",
        ReadOnly = true,
        Idempotent = true,
        OpenWorld = false)]
    [Description("정규화된 주제와 선택적 교육 분야로 디지털 교육 자료를 검색합니다. 현재는 개발용 Mock만 반환하며 실제 공식 자료가 아닙니다. 원문 대화, 이름, 연락처, 식별 정보는 입력하지 마세요.")]
    public string SearchDigitalLearningMaterials(
        [Description("원문 대화가 아닌 짧고 비식별화된 주제 또는 서비스명입니다. 최대 120자입니다.")] string topic,
        [Description("선택적 교육 분야 코드입니다. 예: digital-literacy")] string? category = null,
        [Description("반환할 최대 항목 수입니다. 1~20이며 기본값은 5입니다.")] int pageSize = DefaultPageSize)
    {
        var normalizedTopic = NormalizeText(topic, nameof(topic), 120);
        var normalizedCategory = NormalizeOptionalText(category, nameof(category), 60);
        var size = ValidatePageSize(pageSize);
        var results = dataProvider.SearchDigitalLearningMaterials(
            normalizedTopic,
            normalizedCategory,
            size);

        return Serialize(new MockToolResponse<IReadOnlyList<DigitalLearningMaterial>>(
            "mock",
            MockNotice,
            IsMock: true,
            results));
    }

    [McpServerTool(
        Name = "get_digital_learning_material",
        Title = "디지털 교육 자료 상세 조회",
        ReadOnly = true,
        Idempotent = true,
        OpenWorld = false)]
    [Description("자료 ID로 개발용 교육 자료 예시를 조회합니다. 결과는 공식 자료나 실제 이용 절차가 아닙니다.")]
    public string GetDigitalLearningMaterial(
        [Description("검색 결과에서 받은 자료 ID입니다.")] string materialId)
    {
        var id = NormalizeText(materialId, nameof(materialId), 100);
        var result = dataProvider.GetDigitalLearningMaterial(id);

        return Serialize(new MockToolResponse<DigitalLearningMaterial?>(
            result is null ? "not_found" : "mock",
            result is null ? "자료를 찾지 못했습니다." : MockNotice,
            IsMock: true,
            result));
    }

    [McpServerTool(
        Name = "search_digital_learning_centers",
        Title = "디지털 교육장 검색",
        ReadOnly = true,
        Idempotent = true,
        OpenWorld = false)]
    [Description("시·군·구 등 거친 지역으로 개발용 교육장 예시를 검색합니다. 실제 장소·주소·운영 상태가 아니며, 상세 주소나 개인 정보는 입력하지 마세요.")]
    public string SearchDigitalLearningCenters(
        [Description("시·군·구 또는 광역시 등 상세 주소가 아닌 넓은 지역입니다. 최대 40자입니다.")] string region,
        [Description("선택적 교육장 유형입니다. 최대 60자입니다.")] string? centerType = null,
        [Description("반환할 최대 항목 수입니다. 1~20이며 기본값은 5입니다.")] int pageSize = DefaultPageSize)
    {
        var normalizedRegion = NormalizeRegion(region);
        var normalizedType = NormalizeOptionalText(centerType, nameof(centerType), 60);
        var size = ValidatePageSize(pageSize);
        var results = dataProvider.SearchDigitalLearningCenters(
            normalizedRegion,
            normalizedType,
            size);

        return Serialize(new MockToolResponse<IReadOnlyList<DigitalLearningCenter>>(
            "mock",
            MockNotice,
            IsMock: true,
            results));
    }

    [McpServerTool(
        Name = "search_welfare_programs",
        Title = "복지 사업 검색",
        ReadOnly = true,
        Idempotent = true,
        OpenWorld = false)]
    [Description("정규화된 복지 주제, 거친 지역, 선택적 비식별 자격 범주로 개발용 예시를 검색합니다. 실제 복지 사업이나 자격 판단·신청 안내가 아닙니다. 원문 대화와 개인 식별 정보는 입력하지 마세요.")]
    public string SearchWelfarePrograms(
        [Description("원문 대화가 아닌 짧고 비식별화된 지원 주제입니다. 최대 120자입니다.")] string topic,
        [Description("시·군·구 등 상세 주소가 아닌 넓은 지역입니다. 최대 40자입니다.")] string region,
        [Description("선택적 범주입니다: general, senior, low_income, disability 중 하나입니다.")] string? eligibilityCategory = null,
        [Description("반환할 최대 항목 수입니다. 1~20이며 기본값은 5입니다.")] int pageSize = DefaultPageSize)
    {
        var normalizedTopic = NormalizeText(topic, nameof(topic), 120);
        var normalizedRegion = NormalizeRegion(region);
        var normalizedEligibility = NormalizeEligibilityCategory(eligibilityCategory);
        var size = ValidatePageSize(pageSize);
        var results = dataProvider.SearchWelfarePrograms(
            normalizedTopic,
            normalizedRegion,
            normalizedEligibility,
            size);

        return Serialize(new MockToolResponse<IReadOnlyList<WelfareProgram>>(
            "mock",
            MockNotice,
            IsMock: true,
            results));
    }

    [McpServerTool(
        Name = "get_welfare_program",
        Title = "복지 사업 상세 조회",
        ReadOnly = true,
        Idempotent = true,
        OpenWorld = false)]
    [Description("사업 ID로 개발용 복지 사업 예시를 조회합니다. 실제 자격 요건이나 신청 절차가 아닙니다.")]
    public string GetWelfareProgram(
        [Description("검색 결과에서 받은 사업 ID입니다.")] string programId)
    {
        var id = NormalizeText(programId, nameof(programId), 100);
        var result = dataProvider.GetWelfareProgram(id);

        return Serialize(new MockToolResponse<WelfareProgram?>(
            result is null ? "not_found" : "mock",
            result is null ? "자료를 찾지 못했습니다." : MockNotice,
            IsMock: true,
            result));
    }

    private static string NormalizeText(string value, string parameterName, int maximumLength)
    {
        ArgumentNullException.ThrowIfNull(value, parameterName);

        var normalized = string.Join(
            ' ',
            value.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
        if (normalized.Length is 0 || normalized.Length > maximumLength)
        {
            throw new McpException($"{parameterName}은(는) 1자 이상 {maximumLength}자 이하여야 합니다.");
        }

        if (ObviousIdentifierPattern.IsMatch(normalized))
        {
            throw new McpException($"{parameterName}에 연락처나 식별 정보를 입력하지 마세요.");
        }

        return normalized;
    }

    private static string? NormalizeOptionalText(string? value, string parameterName, int maximumLength) =>
        string.IsNullOrWhiteSpace(value)
            ? null
            : NormalizeText(value, parameterName, maximumLength);

    private static string NormalizeRegion(string value)
    {
        var region = NormalizeText(value, nameof(value), 40);
        if (region.Any(char.IsDigit)
            || region.Contains("번지", StringComparison.Ordinal)
            || region.EndsWith("로", StringComparison.Ordinal)
            || region.EndsWith("길", StringComparison.Ordinal))
        {
            throw new McpException("상세 주소가 아닌 시·군·구 등 넓은 지역만 입력하세요.");
        }

        return region;
    }

    private static string? NormalizeEligibilityCategory(string? value)
    {
        var category = NormalizeOptionalText(value, nameof(value), 30);
        if (category is not null && !EligibilityCategories.Contains(category))
        {
            throw new McpException(
                "자격 범주는 general, senior, low_income, disability 중 하나만 입력하세요.");
        }

        return category;
    }

    private static int ValidatePageSize(int pageSize)
    {
        if (pageSize is < 1 or > MaximumPageSize)
        {
            throw new McpException($"pageSize는 1~{MaximumPageSize} 범위여야 합니다.");
        }

        return pageSize;
    }

    private static string Serialize<T>(T value) => JsonSerializer.Serialize(value, JsonOptions);
}
