using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Infrastructure.EntityConfigurations
{
    public class FriendshipConfiguration : IEntityTypeConfiguration<Friendship>
    {
        public void Configure(EntityTypeBuilder<Friendship> builder)
        {
            builder.HasKey(f => f.Id);

            builder.Property(f => f.Id)
                .IsRequired();

            builder.Property(f => f.Status)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(f => f.RequestedAt)
                .IsRequired();

            builder.Property(f => f.AcceptedAt)
                .IsRequired(false);
            
            builder.HasOne(f => f.Requester)
                .WithMany() 
                .HasForeignKey(f => f.RequesterId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasOne(f => f.Addressee)
                .WithMany()
                .HasForeignKey(f => f.AddresseeId)
                .OnDelete(DeleteBehavior.Cascade);
            
        }
    }
}