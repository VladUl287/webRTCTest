using Web.Hubs.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Web.Hubs.Infrastructure.Database.Configuration;

public sealed class ChatUserConfiguration : IEntityTypeConfiguration<ChatUser>
{
    public void Configure(EntityTypeBuilder<ChatUser> builder)
    {
        builder.HasKey(x => new { x.ChatId, x.UserId });

        builder.Property(x => x.LastRead)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(x => x.Image)
            .HasMaxLength(255)
            .IsRequired(false);

        builder.HasOne(x => x.Chat)
            .WithMany(x => x.ChatUsers)
            .HasForeignKey(x => x.ChatId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
