namespace cheongdo_mcp.Education;

public sealed class MockEducationDataProvider : IEducationDataProvider
{
    private static readonly SourceMetadata MockSource = new(
        "개발용 Mock 자료",
        SourceUrl: null,
        UpdatedAt: null,
        IsMock: true);

    private static readonly DigitalLearningMaterial[] Materials =
    [
        new(
            "mock-smartphone-basics",
            "스마트폰 기초 디지털 교육 자료 (개발용 예시)",
            "검색과 결과 표시 흐름을 확인하기 위한 가상 자료입니다. 실제 교육 절차가 아닙니다.",
            "digital-literacy",
            ["스마트폰"],
            [
                "개발 흐름 확인용 예시 단계 1 (실제 이용 안내 아님)",
                "개발 흐름 확인용 예시 단계 2 (실제 이용 안내 아님)"
            ],
            MockSource)
    ];

    private static readonly DigitalLearningCenter[] Centers =
    [
        new(
            "mock-learning-center",
            "디지털 교육장 (개발용 예시)",
            "인천",
            "테스트 유형",
            "실제 주소가 아닌 테스트 값",
            "실제 운영 상태 미확인",
            MockSource)
    ];

    private static readonly WelfareProgram[] WelfarePrograms =
    [
        new(
            "mock-welfare-program",
            "복지 지원 사업 (개발용 예시)",
            "복지 지원 검색 흐름 검증",
            "청도군",
            "senior",
            "실제 자격 요건이 아닌 개발용 예시입니다.",
            "실제 신청 방법이 아닌 개발용 예시입니다.",
            MockSource)
    ];

    public IReadOnlyList<DigitalLearningMaterial> SearchDigitalLearningMaterials(
        string topic,
        string? category,
        int pageSize)
    {
        var terms = SplitTerms(topic);

        return Materials
            .Where(material =>
                string.IsNullOrWhiteSpace(category)
                || string.Equals(material.Category, category.Trim(), StringComparison.OrdinalIgnoreCase))
            .Where(material =>
                terms.Length == 0
                || terms.Any(term =>
                    material.Title.Contains(term, StringComparison.OrdinalIgnoreCase)
                    || material.Summary.Contains(term, StringComparison.OrdinalIgnoreCase)
                    || material.AppliesTo.Contains(term, StringComparer.OrdinalIgnoreCase)))
            .Take(pageSize)
            .ToArray();
    }

    public DigitalLearningMaterial? GetDigitalLearningMaterial(string materialId) =>
        Materials.FirstOrDefault(material =>
            string.Equals(material.Id, materialId, StringComparison.Ordinal));

    public IReadOnlyList<DigitalLearningCenter> SearchDigitalLearningCenters(
        string region,
        string? centerType,
        int pageSize) =>
        Centers
            .Where(center => string.Equals(center.Region, region, StringComparison.OrdinalIgnoreCase))
            .Where(center =>
                string.IsNullOrWhiteSpace(centerType)
                || string.Equals(center.Type, centerType.Trim(), StringComparison.OrdinalIgnoreCase))
            .Take(pageSize)
            .ToArray();

    public IReadOnlyList<WelfareProgram> SearchWelfarePrograms(
        string topic,
        string region,
        string? eligibilityCategory,
        int pageSize)
    {
        var terms = SplitTerms(topic);

        return WelfarePrograms
            .Where(program => string.Equals(program.Region, region, StringComparison.OrdinalIgnoreCase))
            .Where(program =>
                string.IsNullOrWhiteSpace(eligibilityCategory)
                || string.Equals(
                    program.TargetCategory,
                    eligibilityCategory,
                    StringComparison.OrdinalIgnoreCase))
            .Where(program =>
                terms.Length == 0
                || terms.Any(term =>
                    program.Name.Contains(term, StringComparison.OrdinalIgnoreCase)
                    || program.Topic.Contains(term, StringComparison.OrdinalIgnoreCase)))
            .Take(pageSize)
            .ToArray();
    }

    public WelfareProgram? GetWelfareProgram(string programId) =>
        WelfarePrograms.FirstOrDefault(program =>
            string.Equals(program.Id, programId, StringComparison.Ordinal));

    private static string[] SplitTerms(string value) =>
        value.Split(
            [' ', ',', '.', '，', '、'],
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
