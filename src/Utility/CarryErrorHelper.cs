using CarryOn.API.Common.Models;
using Vintagestory.API.Client;
using Vintagestory.API.Config;
using Vintagestory.API.Server;

namespace CarryOn.Utility
{
    /// <summary>
    /// Shows error messages resolved from failure codes using a single hybrid rule:
    /// a code containing a domain separator (<c>:</c>) is a full lang key owned by
    /// the calling mod and is resolved verbatim (e.g. "rackemup:shattered");
    /// a code without one is a bare code and is resolved as "carryon:&lt;code&gt;"
    /// (the standard CarryOn-scoped messages). Reserved sentinel codes from
    /// <see cref="CarryConstants.FailureCodes"/> are NOT evaluated here - callers
    /// route on them before calling this helper.
    /// </summary>
    public static class CarryErrorHelper
    {
        /// <summary>
        /// Applies the hybrid resolution rule: a key containing a domain separator
        /// is returned verbatim, otherwise the "carryon:" prefix is applied.
        /// </summary>
        /// <param name="failureCode">Full lang key or bare code, e.g. "rackemup:shattered".</param>
        public static string ResolveLangKey(string failureCode)
            => failureCode.Contains(':') ? failureCode : CarryConstants.GetCarryCode(failureCode);

        /// <summary>
        /// Shows the error on the client, using the code itself as the message key.
        /// </summary>
        /// <param name="api">The client API.</param>
        /// <param name="failureCode">Full lang key or bare code, e.g. "rackemup:shattered".</param>
        public static void ShowError(ICoreClientAPI api, string failureCode)
        {
            string domain = ExtractDomain(failureCode);
            api.TriggerIngameError(domain, failureCode, Lang.Get(ResolveLangKey(failureCode)));
        }

        /// <summary>
        /// Shows the error on the client using an explicit message key that may
        /// differ from the code.
        /// </summary>
        /// <param name="api">The client API.</param>
        /// <param name="errorCode">The error code reported to the game.</param>
        /// <param name="langKey">Full lang key or bare code for the message text.</param>
        public static void ShowError(ICoreClientAPI api, string errorCode, string langKey)
        {
            string domain = ExtractDomain(langKey);
            api.TriggerIngameError(domain, errorCode, Lang.Get(ResolveLangKey(langKey)));
        }

        /// <summary>
        /// Sends the error to a server player, using the code itself as the message key.
        /// The text is resolved in the target player's language, not the server's, so a
        /// server configured as <c>en</c> still toasts in the language its players chose.
        /// </summary>
        /// <param name="player">The target server player.</param>
        /// <param name="failureCode">Full lang key or bare code, e.g. "rackemup:shattered".</param>
        public static void SendError(IServerPlayer player, string failureCode)
        {
            player.SendIngameError(failureCode, ResolveLang(player, ResolveLangKey(failureCode)));
        }

        /// <summary>
        /// Sends the error to a server player using an explicit message key that may
        /// differ from the code. The text is resolved in the target player's language.
        /// </summary>
        /// <param name="player">The target server player.</param>
        /// <param name="errorCode">The error code reported to the player.</param>
        /// <param name="langKey">Full lang key or bare code for the message text.</param>
        public static void SendError(IServerPlayer player, string errorCode, string langKey)
        {
            player.SendIngameError(errorCode, ResolveLang(player, ResolveLangKey(langKey)));
        }

        /// <summary>
        /// Resolves a resolved lang key in the language the given player is playing in.
        /// <see cref="Lang.GetL(string, string, object[])"/> falls back to
        /// <see cref="Lang.DefaultLocale"/> when the player's language has no entry, so an
        /// untranslated key still renders as English rather than as the raw key.
        /// A player with no language code falls back to the server's own locale, which is
        /// the behaviour CarryOn had before this method existed.
        /// </summary>
        /// <param name="player">The player who will see the message.</param>
        /// <param name="langKey">An already-resolved lang key, e.g. "carryon:slot-not-empty".</param>
        internal static string ResolveLang(IServerPlayer player, string langKey)
        {
            string languageCode = player?.LanguageCode ?? string.Empty;
            return string.IsNullOrEmpty(languageCode)
                ? Lang.Get(langKey)
                : Lang.GetL(languageCode, langKey);
        }

        // "rackemup:shattered" -> "rackemup"; "slot-not-empty" -> CarryConstants.ModId
        private static string ExtractDomain(string langKey)
        {
            int idx = langKey.IndexOf(':');
            return idx > 0 ? langKey[..idx] : CarryConstants.ModId;
        }
    }
}
