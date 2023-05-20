using Web.Hub.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Web.Hub.Infrastructure.Database.Configuration;

public sealed class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Content)
            .HasMaxLength(5000)
            .IsRequired();

        builder.Property(x => x.DialogId)
            .IsRequired();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.Date)
            .IsRequired();

        builder.HasOne(x => x.Dialog)
            .WithMany()
            .HasForeignKey(x => x.DialogId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
