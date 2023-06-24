using Web.Hubs.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Web.Hubs.Infrastructure.Database.Configuration;

public sealed class CallUserConfiguration : IEntityTypeConfiguration<CallUser>
{
    public void Configure(EntityTypeBuilder<CallUser> builder)
    {
        builder.HasKey(x => new { x.CallId, x.UserId });
    }
}
