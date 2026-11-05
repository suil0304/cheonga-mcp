namespace cheongdo_mcp.Education;

public interface IEducationDataProvider
{
    IReadOnlyList<DigitalLearningMaterial> SearchDigitalLearningMaterials(
        string topic,
        string? category,
        int pageSize);

    DigitalLearningMaterial? GetDigitalLearningMaterial(string materialId);

    IReadOnlyList<DigitalLearningCenter> SearchDigitalLearningCenters(
        string region,
        string? centerType,
        int pageSize);

    IReadOnlyList<WelfareProgram> SearchWelfarePrograms(
        string topic,
        string region,
        string? eligibilityCategory,
        int pageSize);

    WelfareProgram? GetWelfareProgram(string programId);
}
