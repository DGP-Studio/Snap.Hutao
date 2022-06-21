// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore.Design;
using Snap.Hutao.Context.FileSystem;

namespace Snap.Hutao.Context.Database;

public class AppDbContextDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        MyDocumentContext myDocument = new(new());
        myDocument.EnsureDirectory();

        string dbFile = myDocument.Locate("Userdata.db");
        string sqlConnectionString = $"Data Source={dbFile}";

        return AppDbContext.CreateFrom(sqlConnectionString);
    }
}
