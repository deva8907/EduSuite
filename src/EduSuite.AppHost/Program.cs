// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this

var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.EduSuite_Api>("apiservice");

builder.AddProject<Projects.EduSuite_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.AddProject<Projects.EduSuite_Identity>("edusuite-identity");

builder.Build().Run();
