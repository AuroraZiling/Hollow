using Hollow.Abstractions.Models;
using Hollow.Abstractions.Models.HttpContrasts.Hakush;

namespace Hollow.Abstractions.Services;

public interface IMetadataService
{
    public Dictionary<string, HakushItemModel>? ItemsMetadata { get; set; }
    public Task LoadItemMetadata(IProgress<Response<string>> progress, bool force = false);
}