using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hollow.Abstractions.Models;
using Hollow.Abstractions.Models.HttpContrasts.Hakush;

namespace Hollow.Services.MetadataService;

public interface IMetadataService
{
    public Dictionary<string, HakushItemModel>? ItemsMetadata { get; set; }
    public Task LoadItemMetadata(IProgress<Response<string>> progress, bool force = false);
}