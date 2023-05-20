using Web.Hub.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Web.Hub.Infrastructure.Database.Configuration;

public sealed class DialogUserConfiguration : IEntityTypeConfiguration<DialogUser>
{
    public void Configure(EntityTypeBuilder<DialogUser> builder)
    {
        builder.HasKey(x => new { x.DialogId, x.UserId });

        builder.Property(x => x.Right)
            .IsRequired();

        builder.Property(x => x.LastRead)
            .IsRequired();
    }
}
