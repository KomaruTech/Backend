using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TemplateService.Domain.Entities;

namespace TemplateService.Infrastructure.Persistence.EntityConfigurations;

class DocumentConfiguration : IEntityTypeConfiguration<DocumentEntity>
{
    public void Configure(EntityTypeBuilder<DocumentEntity> builder)
    {
        builder.ToTable("TMP_Documents", opt =>
        {
            opt.HasComment("Документы");
        });

        builder.HasKey(t => t.Id);

        builder.HasIndex(t => t.Number).IsUnique();
    }
}