﻿namespace UniSphere.Api.DTOs.Subjects;

public sealed record SubjectDto
{
    public required Guid Id { get; init; }
    public required Guid MajorId { get; init; }
    public required string Name { get; init; }
    public  string? ProfessorName { get; init; }
    public required int Year { get; init; }
    public  double?  MidTermGrade{ get; init; }
    public  double?  FinalGrade{ get; init; }
    public required bool  IsPassed{ get; init; }
    public required bool  CanEnroll{ get; init; }
    public required bool  IsMultipleChoice{ get; init; }
    public required bool  DoesHaveALab{ get; init; }
    public required int Semester { get; init; }
    public required string? ImageUrl { get; init; }
    public required List<MaterialInfo> Materials { get; init; }
    public required double PassGrade { get; init; }
}

public sealed record MaterialInfo
{
    public required string Url { get; init; }
    public required string Type { get; init; }
    public required double PassGrade { get; init; }
}


