using Web.Hub.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Web.Hub.Infrastructure.Database.Configuration;

public sealed class DialogConfiguration : IEntityTypeConfiguration<Dialog>
{
    public void Configure(EntityTypeBuilder<Dialog> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.Image)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.Access)
            .IsRequired();

        builder.Property(x => x.Date)
            .IsRequired();
    }
}
