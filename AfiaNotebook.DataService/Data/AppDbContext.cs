﻿using AfiaNotebook.Entities.DbSet;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfiaNotebook.DataService.Data;
public class AppDbContext : IdentityDbContext
{
    public virtual DbSet<User> Users {  get; set; }
    public virtual DbSet<HealthRecord> HealthRecords {  get; set; }
    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
