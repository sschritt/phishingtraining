using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PhishingTraining.Web.Entities;
using PhishingTraining.Web.Entities.Base;
using PhishingTraining.Web.Models.PhishingTemplate;
using PhishingTraining.Web.Models.Campaign;

namespace PhishingTraining.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            foreach (var entityType in typeof(IEntity).Assembly.GetTypes().Where(type => !type.IsAbstract && !type.IsInterface && type.IsAssignableTo(typeof(IEntity))))
            {
                builder.Entity(entityType);
            }
            
            builder.Entity<Campaign>().HasMany(e => e.Managers).WithMany(user => user.ManagedCampaigns).UsingEntity(j => j.ToTable("CampaignManager"));
            builder.Entity<Campaign>().HasMany(e => e.Participants).WithMany(user => user.ParticipatingCampaigns).UsingEntity(j => j.ToTable("CampaignParticipant"));
            builder.Entity<Campaign>().HasMany(e => e.TemplateUsage).WithMany(c => c.CampaignUsage).UsingEntity(j => j.ToTable("CampaignTemplateUsage"));
            //builder.Entity<SmsMessage>().HasNoKey();

            builder.Entity<PhishingTemplate>().HasMany(e => e.Parameters).WithMany(user => user.Templates).UsingEntity(j => j.ToTable("TemplateParameter"));
            builder.Entity<PhishingTemplate>().HasMany<PhishingMessage>().WithOne(x => x.PhishingTemplate)
                .OnDelete(DeleteBehavior.SetNull);
        }

        public IQueryable<TEntity> Query<TEntity>() where TEntity : class => Set<TEntity>();

        public async Task<TEntity> EnsureKeyedEntityExists<TEntity>(TEntity entity) where TEntity : KeyedEntity
        {
            var fromDb = await Set<TEntity>().FindAsync(entity.Id);
            if (fromDb == null)
            {
                fromDb = Add(entity).Entity;
            }
            else
            {
                Entry(fromDb).CurrentValues.SetValues(entity);
            }

            return fromDb;
        }        
    }
}
