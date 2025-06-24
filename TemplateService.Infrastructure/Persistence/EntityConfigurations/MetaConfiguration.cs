using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TemplateService.Domain.Entities;

namespace TemplateService.Infrastructure.Persistence.EntityConfigurations;

class MetaConfiguration : IEntityTypeConfiguration<MetaEntity>
{
    public void Configure(EntityTypeBuilder<MetaEntity> builder)
    {
        builder.ToTable("TMP_Metas", opt =>
        {
            opt.HasComment("Мета-описание документа");
        });

        builder.HasKey(t => t.Id);

        builder.HasOne(t => t.Document)
            .WithMany(t => t.Metas)
            .HasForeignKey(t => t.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
