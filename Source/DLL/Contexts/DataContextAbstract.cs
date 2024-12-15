using BLL.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLL.Contexts {
    public abstract class DataContextAbstract : DbContext {


        #region Constructors
        protected DataContextAbstract(string? connectionString = null) {
            ConnectionString = connectionString;
        }
        #endregion

        #region Properties
        public virtual string ConnectionString { get; set; }
        public DbSet<Guild> Servers { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<Embed> Embeds { get; set; }
        public DbSet<Settings> Settings { get; set; }
        #endregion

        #region Methods
        protected override void OnModelCreating(ModelBuilder builder) {
            // Server
            builder.Entity<Guild>().ToTable("Guilds");
            builder.Entity<Guild>()
                .HasKey(s => s.GuildId);
            builder.Entity<Guild>()
                .Property(s => s.GuildId)
                .ValueGeneratedNever();
            builder.Entity<Guild>()
                .HasMany(s => s.Messages)
                .WithOne(m => m.guild)
                .HasForeignKey(m => m.GuildId);
            builder.Entity<Guild>()
                .HasMany(s => s.Templates)
                .WithOne()
                .HasForeignKey(m => m.GuildId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Guild>()
                .HasOne(s => s.Settings)
                .WithOne()
                .HasForeignKey<Settings>(s => s.GuildId);

            // Message
            builder.Entity<Message>().ToTable("Messages");
            builder.Entity<Message>(entity => {
                entity.HasKey(m => new { m.MessageId, m.GuildId });
                entity.Property(e => e.MessageId).ValueGeneratedNever();
                entity.Property(e => e.GuildId).ValueGeneratedNever();
                entity.HasOne(m => m.guild)
                    .WithMany(g => g.Messages)
                    .HasForeignKey(m => m.GuildId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            builder.Entity<Message>()
                .HasIndex(m => new { m.GuildId, m.MessageId });
            builder.Entity<Message>()
                .Property(m => m.Data)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<Dictionary<string, string>>(v))
                .Metadata.SetValueComparer(new ValueComparer<Dictionary<string, string>>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToDictionary(k => k.Key, v => v.Value)));
            builder.Entity<Message>()
                .Property(m => m.Childs)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<Dictionary<ulong, ulong>>(v))
                .Metadata.SetValueComparer(new ValueComparer<Dictionary<ulong, ulong>>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToDictionary(k => k.Key, v => v.Value)));

            // Embed
            builder.Entity<Embed>().ToTable("Embeds");
            builder.Entity<Embed>()
                .HasKey(e => new { e.GuildId, e.MessageId });
            builder.Entity<Embed>()
                .HasOne(e => e.Message)
                .WithOne(m => m.Embed)
                .HasForeignKey<Embed>(e => new { e.GuildId, e.MessageId })
                .OnDelete(DeleteBehavior.Cascade); ;
            builder.Entity<Embed>()
                .Property(e => e.Fields)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<List<(string, string, bool)>>(v))
                .Metadata.SetValueComparer(new ValueComparer<List<(string Title, string Value, bool Inline)>>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()));


            // Template
            builder.Entity<Template>().ToTable("Templates");
            builder.Entity<Template>(entity => {
                entity.HasKey(t => new { t.GuildId, t.Name });
                entity.Property(t => t.GuildId).ValueGeneratedNever();
                entity.Property(t => t.Name).ValueGeneratedNever();
                entity.Property(t => t.Message).HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<Message>(v));
                entity.Property(t => t.Name)
                    .HasMaxLength(32);
            });

            // Settings
            builder.Entity<Settings>().ToTable("Settings");
            builder.Entity<Settings>(entity => {
                entity.HasKey(s => s.GuildId);
                entity.Property(s => s.GuildId).ValueGeneratedNever();
            });

            // Return
            base.OnModelCreating(builder);
        }
        #endregion
    }
}
