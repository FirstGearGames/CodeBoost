namespace Nucleus.CodeAnalysis.Analyzers.Serializing
{
    public class NetworkableFinder
    {
        // public event IsNotNetworkableAccessibleDel OnIsNotNetworkableAccessible;
        //
        // public delegate void IsNotNetworkableAccessibleDel(SyntaxNodeAnalysisContext context, ISymbol source, string message = "");
        //
        // public readonly HashSet<NetworkableType> TypesNeedingSerializers = new();
        //
        // public void AddNetworkableTypes(object context, StructDeclarationSyntax structDeclarationSyntax)
        // {
        //     if (context.GetSemanticModel() is not { } sm) return;
        //     
        //     if (sm.GetDeclaredSymbol(structDeclarationSyntax) is not INamedTypeSymbol namedTypeSymbol) return;
        //
        //     AddNamedTypeSymbolNetworkablesWithIdentifier(context, namedTypeSymbol, null);
        // }
        //
        // public void AddNetworkableTypes(object context, ClassDeclarationSyntax classDeclarationSyntax)
        // {
        //     if (context.GetSemanticModel() is not { } sm) return;
        //
        //     if (sm.GetDeclaredSymbol(classDeclarationSyntax) is not INamedTypeSymbol namedTypeSymbol) return;
        //
        //     AddNamedTypeSymbolNetworkablesWithIdentifier(context, namedTypeSymbol, null);
        // }
        //
        // /// <summary>
        // /// Finds networked for an INamedTypeSymbol which may not be bound to specific mechanics, such as RPC.
        // /// </summary>
        // private void AddNamedTypeSymbolNetworkablesWithIdentifier(object context, INamedTypeSymbol namedTypeSymbol, ISymbol source)
        // {
        //     if (!namedTypeSymbol.HasNetworkableIdentifier())
        //         return;
        //
        //     /* FullNames added this iteration.
        //      * This is used to prevent endless loops. */
        //     HashSet<string> addedFullNames = new();
        //     AddSelfAndBaseTypeNetworkables(context, namedTypeSymbol, addedFullNames, source);
        // }
        //
        // /// <summary>
        // /// Adds a type to networkTypes.
        // /// Returns true if added, false if already existed.
        // /// </summary>
        // private bool AddNetworkableType(object context, INamedTypeSymbol namedTypeSymbol, ISymbol source)
        // {
        //     const bool isMetadataName = false;
        //
        //     //Has exclude serialization attribute.
        //     if (namedTypeSymbol.HasAttribute(FishNetConstants.ExcludeSerializationAttribute_FullName, isMetadataName)) return false;
        //
        //     string fullName = namedTypeSymbol.GetTypeSymbolFullName(isMetadataName);
        //     //Few other checks for types we want to ignore.
        //     if (fullName == typeof(System.ValueType).FullName) return false;
        //     if (fullName == typeof(System.Object).FullName) return false;
        //
        //     //Check if already added.
        //     foreach (NetworkableType st in TypesNeedingSerializers)
        //     {
        //         if (st.NamedTypeSymbol == namedTypeSymbol)
        //             return false;
        //     }
        //     
        //     if (!namedTypeSymbol.HasPublicAccessibility())// && !namedTypeSymbol.ContainingType.HasPartialModifier())
        //     {
        //         if (context is SyntaxNodeAnalysisContext analysisContext)
        //             OnIsNotNetworkableAccessible?.Invoke(analysisContext, source, string.Empty);
        //
        //         return false;
        //     }
        //
        //     if (TypesNeedingSerializers.Add(new NetworkableType(namedTypeSymbol)))
        //         Log($"Added {namedTypeSymbol.GetTypeSymbolFullName(isMetadataName)} to types needing serializers.");
        //
        //     return true;
        // }
        //
        // /// <summary>
        // /// Iterates up the base types of a named symbol and adds them to networked.
        // /// </summary>
        // /// <param name="foundNames">A collection reference to store already found networked during the iteration.</param>
        // private void AddSelfAndBaseTypeNetworkables(object context, INamedTypeSymbol namedTypeSymbol, HashSet<string> foundNames, ISymbol source)
        // {
        //     while (true)
        //     {
        //         if (!TryAdd(namedTypeSymbol))
        //             return;
        //
        //         namedTypeSymbol = namedTypeSymbol?.BaseType;
        //     }
        //
        //     bool TryAdd(INamedTypeSymbol lNamedTypeSymbol)
        //     {
        //         //No more base types, or cannot be networked.s
        //         if (lNamedTypeSymbol is not INamedTypeSymbol)
        //             return false;
        //
        //         string fullName = lNamedTypeSymbol.GetTypeSymbolFullNameWithNamedArguments(metadataName: false);
        //
        //         //Already added.
        //         if (foundNames.Contains(fullName))
        //             return false;
        //
        //         /* The method indicated it could not add some reason.
        //          * Maybe the type had an attribute. */
        //         if (!AddNetworkableType(context, lNamedTypeSymbol, source))
        //             return false;
        //
        //         foundNames.Add(fullName);
        //
        //         return true;
        //     }
        // }
        //
        // private void Log(string txt)
        // {
        //     if (txt.Length == 0)
        //         Debugg.Log(txt);
        //     else
        //         Debugg.Log($"[SerializerReceiver] {txt}");
        // }
        
        #region Commented out methods.
        
        //
        // /// <summary>
        // /// Finds networked for methods which implement RPC attributes.
        // /// </summary>
        // public void AddRpcNetworkables(object context, MethodDeclarationSyntax methodDeclarationSyntax)
        // {
        //     SemanticModel sm = context.GetSemanticModel();
        //
        //     ISymbol? symbol = sm?.GetDeclaredSymbol(methodDeclarationSyntax);
        //
        //     if (symbol is not IMethodSymbol methodSymbol) return;
        //
        //     List<IParameterSymbol> networked = GetRpcNetworkableParameters(context, methodSymbol);
        //
        //     foreach (IParameterSymbol parameterSymbol in networked)
        //     {
        //         if (parameterSymbol.Type is INamedTypeSymbol namedSymbol)
        //             AddNetworkableType(context, namedSymbol, methodSymbol);
        //     }
        // }
        //
        // /// <summary>
        // /// Returns possible networked in over all RPC attributes for a method.
        // /// </summary>
        // /// <remarks>If there are multiple attributes on the RPC then serializers will be added accordingly for each attribute.</remarks>
        // public List<IParameterSymbol> GetRpcNetworkableParameters(object context, IMethodSymbol methodSymbol)
        // {
        //     List<IParameterSymbol> networkResults = new();
        //
        //     if (!methodSymbol.HasRpcAttributes(out List<RpcAttributeData> results)) return networkResults;
        //
        //     List<IParameterSymbol> parameters = methodSymbol.Parameters.ToList();
        //
        //     foreach (RpcAttributeData item in results)
        //         networkResults.AddRange(GetRpcNetworkableParameters(context, methodSymbol, item));
        //
        //     return parameters;
        // }
        //
        // public List<IParameterSymbol> GetRpcNetworkableParameters(object context, IMethodSymbol methodSymbol, RpcAttributeData rpcAttributeData)
        // {
        //     List<IParameterSymbol> parameters = methodSymbol.Parameters.ToList();
        //     int parametersCount = parameters.Count;
        //
        //     const NameType nameType = false;
        //
        //     //ServerRpc.
        //     if (rpcAttributeData.RPCType == RPCType.Server)
        //         RemoveTrailingNetworkConnection();
        //     //TargetRpc.
        //     else if (rpcAttributeData.RPCType == RPCType.Target)
        //         RemoveLeadingNetworkConnection();
        //
        //     //All Rpcs support optional channel.
        //     RemoveTrailingChannel();
        //
        //     //Removes networkConnection if the first parameter.
        //     void RemoveLeadingNetworkConnection()
        //     {
        //         if (parametersCount == 0) return;
        //         //Remove channel from network.
        //         if (parameters[0].Type.GetTypeSymbolFullName(nameType) == FishNetConstants.NetworkConnection_FullName)
        //             parameters.RemoveAt(--parametersCount);
        //     }
        //
        //     //Removes networkConnection if the last parameter.
        //     void RemoveTrailingNetworkConnection()
        //     {
        //         if (parametersCount == 0) return;
        //         //Remove channel from network.
        //         if (parameters[parametersCount - 1].Type.GetTypeSymbolFullName(nameType) == FishNetConstants.NetworkConnection_FullName)
        //             parameters.RemoveAt(--parametersCount);
        //     }
        //
        //     //Removes channel if the last parameter.
        //     void RemoveTrailingChannel()
        //     {
        //         if (parametersCount == 0) return;
        //         //Remove channel from network.
        //         if (parameters[parametersCount - 1].Type.GetTypeSymbolFullName(nameType) == FishNetConstants.Channel_FullName)
        //             parameters.RemoveAt(--parametersCount);
        //     }
        //
        //     for (int i = 0; i < parameters.Count; i++)
        //     {
        //         if (parameters[i].Type is not INamedTypeSymbol)
        //             parameters.RemoveAt(i--);
        //     }
        //
        //     return parameters;
        // }

        //
        // /// <summary>
        // /// Finds SyncType networked within a namedTypeSymbol that inherits NetowrkBehaviour.
        // /// </summary>
        // private void CheckSyncTypeNetworkables(object context, INamedTypeSymbol namedTypeSymbol, SemanticModel semanticModel)
        // {
        //     //Named type must inherit NetworkBehaviour to look for SyncTypes.
        //     if (!namedTypeSymbol.InheritsClass(FishNetConstants.NetworkBehaviour_FullName))
        //         return;
        //
        //     List<IFieldSymbol> fieldSymbols = namedTypeSymbol.GetFieldMembers();
        //
        //     //Check all field types to see if they inherit SyncBase.
        //     foreach (IFieldSymbol fieldSymbol in fieldSymbols)
        //     {
        //         //This is returning false (not synctype) on synctypes.
        //         if (!fieldSymbol.IsSyncType()) continue;
        //
        //         //This should always pass given this is checked within 'IsSyncType'.
        //         if (fieldSymbol.Type is not INamedTypeSymbol fieldNamedTypeSymbol) continue;
        //
        //         //Get SyncType.
        //         SyncTypeType stt = fieldNamedTypeSymbol.GetSyncType();
        //
        //         //No syncType.
        //         if (stt == SyncTypeType.Unset)
        //             continue;
        //
        //         //Custom.
        //         if (stt == SyncTypeType.Custom)
        //             CheckCustomSyncTypeNetworkable(fieldNamedTypeSymbol, fieldSymbol);
        //         //SyncType is built into FishNet with generics.
        //         else
        //             CheckIncludedGenericSyncTypeNetworkable(fieldNamedTypeSymbol, fieldSymbol);
        //     }
        //
        //     //Finds networked for generic synctypes such as SyncVar<T0>.
        //     void CheckIncludedGenericSyncTypeNetworkable(INamedTypeSymbol fieldNamedTypeSymbol, IFieldSymbol fieldSymbol)
        //     {
        //         List<ITypeSymbol> genericArgumentTypeSymbols = fieldNamedTypeSymbol.GetGenericArgumentsOfNamedTypeSymbol();
        //         /* FullNames added this iteration.
        //          * This is used to prevent endless loops. */
        //         HashSet<string> addedFullNames = new();
        //
        //         foreach (ITypeSymbol typeSymbol in genericArgumentTypeSymbols)
        //         {
        //             //Must be named to be added as a network.
        //             if (typeSymbol is not INamedTypeSymbol genericArgumentNamedTypeSymbol)
        //                 continue;
        //
        //             AddSelfAndBaseTypeNetworkables(context, genericArgumentNamedTypeSymbol, addedFullNames, fieldSymbol);
        //         }
        //     }
        //
        //     //Finds networked on types which implement ICustomSync by reading the GetNetworkedType method.
        //     void CheckCustomSyncTypeNetworkable(INamedTypeSymbol fieldNamedTypeSymbol, IFieldSymbol fieldSymbol)
        //     {
        //         IMethodSymbol methodSymbol = fieldNamedTypeSymbol.GetMethod(FishNetConstants.ICustomSync_GetNetworkedType_Name, nameType: false);
        //         if (methodSymbol == null) return;
        //
        //         //Default return type should be System.Object, exit if not the case.
        //         if (methodSymbol.ReturnType.GetTypeSymbolFullName(metadataName: false) != NativeConstants.Object_FullName) return;
        //
        //         List<ExpressionSyntax> returnExpressions = methodSymbol.GetReturnedExpressionSyntaxes();
        //         //No entries, or first entry is not named.
        //         if (returnExpressions.Count == 0 || returnExpressions[0] is not TypeOfExpressionSyntax typeOfExpressionSyntax) return;
        //
        //         ITypeSymbol returnedTypeSymbol = typeOfExpressionSyntax.GetTypeIdentifier(semanticModel);
        //         if (returnedTypeSymbol == null || returnedTypeSymbol is not INamedTypeSymbol returnedNamedTypeSymbol) return;
        //         /* FullNames added this iteration.
        //          * This is used to prevent endless loops. */
        //         HashSet<string> addedFullNames = new();
        //         //Add first entry.
        //         AddSelfAndBaseTypeNetworkables(context, returnedNamedTypeSymbol, addedFullNames, fieldSymbol);
        //     }
        // }

        #endregion
    }
}