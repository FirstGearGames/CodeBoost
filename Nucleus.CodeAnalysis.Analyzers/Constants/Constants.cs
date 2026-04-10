namespace FirstGearGames.FishNet.CodeAnalysis.Constants
{
    public static class DiagnosticIds
    {
        /// <summary>
        /// Network network types must be declared public or their base type must be partial.
        /// </summary>
        public const string LV0001 = nameof(LV0001);
    }

    public static class DiagnosticCategories
    {
        /// <summary>
        /// Indicates a change in scope is required.
        /// </summary>
        public const string Scope = "Scope";
    }
}