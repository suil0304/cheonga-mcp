using System.Text.Json;
using cheongdo_mcp.Education;

namespace cheongdo_mcp.Tests;

public sealed class EducationToolsTests
{
    private readonly EducationTools _tools = new(new MockEducationDataProvider());

    [Fact]
    public void SearchDigitalLearningMaterials_ReturnsExplicitMockMetadata()
    {
        using var response = JsonDocument.Parse(
            _tools.SearchDigitalLearningMaterials("스마트폰 기초"));
        var root = response.RootElement;
        var material = root.GetProperty("data")[0];

        Assert.Equal("mock", root.GetProperty("status").GetString());
        Assert.True(root.GetProperty("isMock").GetBoolean());
        Assert.True(material.GetProperty("source").GetProperty("isMock").GetBoolean());
        Assert.Equal(JsonValueKind.Null, material.GetProperty("source").GetProperty("sourceUrl").ValueKind);
        Assert.Contains("개발용 예시", material.GetProperty("title").GetString());
    }

    [Fact]
    public void SearchDigitalLearningMaterials_RejectsContactInformation()
    {
        var exception = Assert.Throws<ModelContextProtocol.McpException>(
            () => _tools.SearchDigitalLearningMaterials("010-1234-5678 스마트폰"));

        Assert.Contains("연락처나 식별 정보", exception.Message);
    }

    [Fact]
    public void SearchDigitalLearningCenters_RejectsDetailedAddress()
    {
        var exception = Assert.Throws<ModelContextProtocol.McpException>(
            () => _tools.SearchDigitalLearningCenters("서울특별시 종로구 세종대로 1"));

        Assert.Contains("넓은 지역", exception.Message);
    }

    [Fact]
    public void SearchWelfarePrograms_RejectsFreeFormEligibility()
    {
        var exception = Assert.Throws<ModelContextProtocol.McpException>(
            () => _tools.SearchWelfarePrograms("복지 지원", "청도군", "주민등록번호 123456"));

        Assert.Contains("자격 범주", exception.Message);
    }

    [Fact]
    public void SearchDigitalLearningMaterials_RejectsUnboundedPageSize()
    {
        var exception = Assert.Throws<ModelContextProtocol.McpException>(
            () => _tools.SearchDigitalLearningMaterials("교육", pageSize: 21));

        Assert.Contains("1~20", exception.Message);
    }
}
