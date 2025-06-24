using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel;
using System.Reflection;
using TemplateService.Domain.Entities;
using TemplateService.Domain.Enums;

namespace TemplateService.Infrastructure.Persistence.EntityConfigurations;

class MetaTypeConfiguration : IEntityTypeConfiguration<MetaTypeEntity>
{
    public void Configure(EntityTypeBuilder<MetaTypeEntity> builder)
    {
        builder.ToTable("TMP_MetaTypes", opt =>
        {
            opt.HasComment("Тип мета-данных");
        });

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name).IsRequired().HasMaxLength(20);
        builder.Property(t => t.Description).HasMaxLength(100);

        builder.HasIndex(t => t.Name).IsUnique();

        builder.HasData(
           Enum.GetValues(typeof(MetaTypeEnum))
           .Cast<MetaTypeEnum>()
           .Select(x => new MetaTypeEntity { Id = x, Name = x.ToString().ToLower(), Description = GetDescription(x) })
           );
    }

    private static string GetDescription(Enum value)
    {
        Type type = value.GetType();
        string name = Enum.GetName(type, value);
        if (name != null)
        {
            FieldInfo field = type.GetField(name);
            if (field != null)
                if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attr)
                    return attr.Description;
        }
        return null;
    }
}
