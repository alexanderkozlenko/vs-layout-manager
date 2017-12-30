﻿using Newtonsoft.Json;

namespace Community.VisualStudio.LayoutManager.Data
{
    /// <summary>Represents catalog product information.</summary>
    internal sealed class CatalogProductInfo
    {
        /// <summary>Initializes a new instance of the <see cref="CatalogProductInfo" /> class.</summary>
        public CatalogProductInfo()
        {
        }

        /// <summary>Gets or set semantic version.</summary>
        [JsonProperty("productSemanticVersion")]
        public string SemanticVersion
        {
            get;
            set;
        }
    }
}