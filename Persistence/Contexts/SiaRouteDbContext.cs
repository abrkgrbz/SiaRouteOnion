using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Utils;

namespace Persistence.Contexts
{
    public class SiaRouteDbContext : DbContext
    {

        private readonly IDateTimeService _dateTime;
        private readonly IAuthenticatedUserService _authenticatedUser;
        public DbSet<Question> Questions { get; set; }
        public DbSet<Response> Responses { get; set; }
        public DbSet<PrintStudy> PrintStudies { get; set; }
        public DbSet<UserProject> UserProjects { get; set; }
        public DbSet<AutditLogs> AutditLogs { get; set; }
        public DbSet<ProjectProcess> ProjectProcesses { get; set; }
        public DbSet<ProjectOfficers> ProjectOfficers { get; set; }
        public DbSet<ProjectNotes> ProjectNotes { get; set; }
        public DbSet<SCM> SCM { get; set; }
        public DbSet<ProjectMethods> ProjectMethods { get; set; }
        public DbSet<ErrorLog> ErrorLog { get; set; }

        public SiaRouteDbContext(DbContextOptions<SiaRouteDbContext> options, IAuthenticatedUserService authenticatedUser,
            IDateTimeService dateTime) : base(options)
        {
            _authenticatedUser = authenticatedUser;
            _dateTime = dateTime;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>()
                .HasMany(p => p.Questions)
                .WithOne(q => q.Project)
                .HasForeignKey(q => q.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Project>()
                .HasMany(p => p.Responses)
                .WithOne(r => r.Project)
                .HasForeignKey(r => r.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Project>()
                .HasMany(p => p.PrintStudies)
                .WithOne(ps => ps.Project)
                .HasForeignKey(ps => ps.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.ProjectProcess)
                .WithOne(pp => pp.Project)
                .HasForeignKey<ProjectProcess>(pp => pp.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Project>()
                .HasMany(p => p.ProjectOfficers)
                .WithOne(pp => pp.Project)
                .HasForeignKey(pp => pp.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

             

            modelBuilder.Entity<Project>()
                .HasMany(p => p.ProjectNotes)
                .WithOne(pp => pp.Project)
                .HasForeignKey(pp => pp.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProjectNotes>()
                .Property(x => x.Note)
                .HasColumnType("text");


            modelBuilder.Entity<ProjectSCM>()
                .HasKey(pf => new { pf.ProjectId, pf.SCMId });

            modelBuilder.Entity<ProjectSCM>()
                .HasOne(pf => pf.Project)
                .WithMany(p => p.ProjectScms)
                .HasForeignKey(pf => pf.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProjectSCM>()
                .HasOne(pf => pf.Scm)
                .WithMany(f => f.ProjectScms)
                .HasForeignKey(pf => pf.SCMId);

            modelBuilder.Entity<Project>()
                .HasMany(p => p.ProjectMethods)
                .WithOne(pp => pp.Project)
                .HasForeignKey(pp => pp.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);


        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<AuditableBaseEntity>())
            {

                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.Created = _dateTime.NowUtc;
                        entry.Entity.CreatedBy = _authenticatedUser.UserId;
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModified = _dateTime.NowUtc;
                        entry.Entity.LastModifiedBy = _authenticatedUser.UserId;
                        break;
                 
                }
            }
            var auditEntries = BeforeSaveChanges();
            var result = await base.SaveChangesAsync(cancellationToken);
            await OnAfterSaveChanges(auditEntries);
            return result;
        }

        private List<AuditEntry> BeforeSaveChanges()
        {
            ChangeTracker.DetectChanges();
            var auditEntries = new List<AuditEntry>();
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is AutditLogs || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;
                var auditEntry = new AuditEntry(entry);
                auditEntry.TableName = entry.Entity.GetType().Name;
                auditEntries.Add(auditEntry);
                foreach (var property in entry.Properties)
                {

                    if (property.IsTemporary)
                    {
                        auditEntry.TemporaryProperties.Add(property);
                        continue;
                    }
                    string propertyName = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[propertyName] = property.CurrentValue;
                        continue;
                    }
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.AuditType = AuditType.Create;
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                            auditEntry.UserId = _authenticatedUser.UserId;
                            auditEntry.DateTime = _dateTime.NowUtc;
                            break;
                        case EntityState.Deleted:
                           
                            auditEntry.AuditType = AuditType.Delete;
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            auditEntry.UserId = _authenticatedUser.UserId;
                            auditEntry.DateTime = _dateTime.NowUtc;
                            break;
                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                auditEntry.ChangedColumns.Add(propertyName);
                                auditEntry.AuditType = AuditType.Update;
                                auditEntry.OldValues[propertyName] = property.OriginalValue;
                                auditEntry.NewValues[propertyName] = property.CurrentValue;
                                auditEntry.UserId = _authenticatedUser.UserId;
                                auditEntry.DateTime = _dateTime.NowUtc;
                            }
                            break;
                    }
                }
            } 
            foreach (var auditEntry in auditEntries.Where(_ => !_.HasTemporaryProperties))
            {
                AutditLogs.Add(auditEntry.ToAudit());
            } 
            return auditEntries.Where(_ => _.HasTemporaryProperties).ToList();
        }

        private async Task OnAfterSaveChanges(List<AuditEntry> auditEntries)
        {
            if (auditEntries == null || auditEntries.Count == 0)
                return;

            foreach (var auditEntry in auditEntries)
            { 
                foreach (var prop in auditEntry.TemporaryProperties)
                {
                    if (prop.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                    else
                    {
                        auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                }
                 
                AutditLogs.Add(auditEntry.ToAudit());
            }

            await base.SaveChangesAsync();
        }
    }
}
