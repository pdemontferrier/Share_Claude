using CommonResources.Settings;

namespace BatchCutting_DG.D_Presentation.Settings
{
    public static class SE_Flags
    {
        // Drapeau affiché
        public static Uri DefaultFlagUri = CR_FlagsSettings.FlagUnited_kingdom;
        public static Uri AppFlagUri = DefaultFlagUri;

        // Exposition du dictionnaire des drapeaux en privenance de CommonResources.Settings
        public static Dictionary<string, Uri> ReferenceFlag => CR_FlagsSettings.ReferenceFlag;
    }
}