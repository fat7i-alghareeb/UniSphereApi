﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database;

public sealed class ApplicationIdentityDbContext(DbContextOptions<ApplicationIdentityDbContext> options)
    : IdentityDbContext<ApplicationUser>(options){
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema(Schemes.Identity);
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("asp_net_users");
            entity.HasIndex(e => e.StudentId).IsUnique();
        });
        builder.Entity<IdentityRole>().ToTable("asp_net_roles");
        builder.Entity<IdentityUserRole<string>>().ToTable("asp_net_user_roles");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("asp_net_role_claims");
        builder.Entity<IdentityUserClaim<string>>().ToTable("asp_net_user_claims");
        builder.Entity<IdentityUserLogin<string>>().ToTable("asp_net_user_logins");
        builder.Entity<IdentityUserToken<string>>().ToTable("asp_net_user_tokens");
        builder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Token).HasMaxLength(1000);
            entity.HasIndex(e => e.Token).IsUnique();
            entity.Property(e => e.UserId).HasMaxLength(450);
            entity.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        });


    }
}
