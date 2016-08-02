using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;
using MyHomework.WebApp.Configurations;

namespace MyHomework.WebApp.DatabaseModels
{
    public partial class MyHomeworkDBContext : DbContext
    {
        public MyHomeworkDBContext(DbContextOptions<MyHomeworkDBContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Attachment>(entity =>
            {
                entity.Property(e => e.StorageUrl).IsRequired();

                entity.HasOne(d => d.Message)
                    .WithMany(p => p.Attachment)
                    .HasForeignKey(d => d.MessageId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Attachment_ToMessage");
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasIndex(e => e.CreatedDateTime)
                    .HasName("IX_Message_CreateDateTime");

                entity.Property(e => e.Content).IsRequired();

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.Title).IsRequired();

                entity.Property(e => e.UpdatedBy).HasMaxLength(50);

                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime");
            });
        }

        public virtual DbSet<Attachment> Attachment { get; set; }
        public virtual DbSet<Message> Message { get; set; }
    }
}