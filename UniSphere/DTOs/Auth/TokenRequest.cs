﻿namespace UniSphere.Api.DTOs.Auth;

public sealed record TokenRequest(Guid? StudentId , IEnumerable<string> Roles);

