using Microsoft.CodeAnalysis;

namespace Nucleus.CodeAnalysis.Analyzers.Serializing
{
    public struct NetworkableType
    {
        // public TypeExposure Exposure;
        /// <summary>
        /// Type the network is for.
        /// </summary>
        public INamedTypeSymbol NamedTypeSymbol;
        /// <summary>
        /// Full name of the type.
        /// </summary>
        public string FullName;
        /// <summary>
        /// Full meta name of the type.
        /// </summary>
        public string FullMetadataName;
        
        public NetworkableType(INamedTypeSymbol namedTypeSymbol)
        {
            NamedTypeSymbol = namedTypeSymbol;
            FullName = "";
            FullMetadataName = "";

            // NamedTypeSymbol = namedTypeSymbol;
            // FullName = namedTypeSymbol.GetTypeSymbolFullNameWithNamedArguments(metadataName: false);
            // FullMetadataName = namedTypeSymbol.GetTypeSymbolFullNameWithNamedArguments(metadataName: true);
        }
    }
}