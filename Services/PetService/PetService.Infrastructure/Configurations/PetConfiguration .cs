using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetService.Infrastructure.Configurations
{
    public class PetConfiguration : IEntityTypeConfiguration<Pet>
    {
        public void Configure(EntityTypeBuilder<Pet> builder)
        {
            builder.ToTable("Pets");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Species).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Breed).HasMaxLength(100);
            builder.Property(x => x.Age).IsRequired();
            builder.Property(x => x.OwnerId).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();
        }
    }
}
