using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hollow.Abstractions.Models.HttpContrasts;
using Hollow.Abstractions.Models.HttpContrasts.Hakush;

namespace Hollow.Services.MetadataService;

public interface IMetadataService
{
    public Dictionary<string, ItemModel>? ItemsMetadata { get; set; }
    public Task LoadItemMetadata(IProgress<Response<string>> progress, bool force = false);
}