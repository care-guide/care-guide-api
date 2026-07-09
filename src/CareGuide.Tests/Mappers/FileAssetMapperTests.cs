using AutoMapper;
using CareGuide.Models.DTOs.FileAsset;
using CareGuide.Models.Entities;
using CareGuide.Models.Mappers.FileAsset;
using Microsoft.Extensions.DependencyInjection;

namespace CareGuide.Tests.Mappers;

public class FileAssetMapperTests
{
    private readonly IMapper _mapper;

    public FileAssetMapperTests()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAutoMapper(cfg => cfg.AddProfile<FileAssetProfileMapper>());
        _mapper = services.BuildServiceProvider().GetRequiredService<IMapper>();
    }

    [Fact(DisplayName = "FileAsset→FileAssetDto: all fields map correctly")]
    public void Map_FileAssetToDto_MapsAllFields()
    {
        var entity = new FileAsset
        {
            Id = Guid.NewGuid(),
            Name = "report.pdf",
            Size = 2048,
            Hash = new string('a', 64),
            CreatedAt = new DateTime(2024, 1, 1),
            UpdatedAt = new DateTime(2024, 6, 1),
        };

        var dto = _mapper.Map<FileAssetDto>(entity);

        dto.Id.Should().Be(entity.Id);
        dto.Name.Should().Be("report.pdf");
        dto.Size.Should().Be(2048);
        dto.Hash.Should().Be(new string('a', 64));
        dto.CreatedAt.Should().Be(entity.CreatedAt);
        dto.UpdatedAt.Should().Be(entity.UpdatedAt);
    }

    [Fact(DisplayName = "CreateFileAssetDto→FileAsset: all fields map correctly")]
    public void Map_CreateFileAssetDtoToEntity_MapsFields()
    {
        var dto = new CreateFileAssetDto("photo.jpg", 4096, new string('b', 64));

        var entity = _mapper.Map<FileAsset>(dto);

        entity.Name.Should().Be("photo.jpg");
        entity.Size.Should().Be(4096);
        entity.Hash.Should().Be(new string('b', 64));
    }

    [Fact(DisplayName = "UpdateFileAssetDto→FileAsset: all fields map correctly")]
    public void Map_UpdateFileAssetDtoToEntity_MapsFields()
    {
        var dto = new UpdateFileAssetDto(Guid.NewGuid(), "invoice.pdf", 8192, new string('c', 64));

        var entity = _mapper.Map<FileAsset>(dto);

        entity.Name.Should().Be("invoice.pdf");
        entity.Size.Should().Be(8192);
        entity.Hash.Should().Be(new string('c', 64));
    }
}
