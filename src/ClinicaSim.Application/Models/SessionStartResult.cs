namespace ClinicaSim.Application.Models;

public sealed record SessionStartResult(
    string SessionCode,
    string Status,
    DateTimeOffset StartedAt,
    CaseListItem Case,
    IReadOnlyCollection<SessionSectionItem> Sections);

public sealed record SessionSectionItem(Guid SectionId, string Name, IReadOnlyCollection<SessionCategoryItem> Categories);
public sealed record SessionCategoryItem(Guid CategoryId, string Name, IReadOnlyCollection<SessionQuestionItem> Questions);
public sealed record SessionQuestionItem(Guid QuestionId, string Text);
