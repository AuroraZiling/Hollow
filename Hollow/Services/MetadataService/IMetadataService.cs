using System;
using System.Threading.Tasks;
using Hollow.Abstractions.Models.HttpContrasts;

namespace Hollow.Services.MetadataService;

public interface IMetadataService
{
    public Task LoadMetadata(IProgress<Response<string>> progress);
    public Task LoadMetadata(string metadataKey, IProgress<Response<string>> progress);
}