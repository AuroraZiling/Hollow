using System;
using System.Threading.Tasks;
using Hollow.Abstractions.Models.HttpContrasts;

namespace Hollow.Services.MetadataService;

public interface IMetadataService
{
    public Task LoadItemMetadata(IProgress<Response<string>> progress);
}