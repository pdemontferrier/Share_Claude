using System.Windows;
using DG244Cutting.A_Domain.Interfaces.Settings.Presentation;

namespace DG244Cutting.D_Presentation.Settings
{
    /// <summary>
    /// Centralise l'état du dictionnaire de langue actif et encapsule son chargement WPF.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Composant Singleton de présentation injectable via <see cref="ISE_Language"/>,
    /// enregistré dans le Composition Root. Consommé par <c>SR_Language</c> pour appliquer une langue
    /// et par <c>SR_Dictionary</c> pour résoudre les textes traduits.</para>
    /// <para>Objectif : Fournir un point d'accès unique à l'état linguistique de l'application,
    /// en isolant complètement la dépendance WPF (<see cref="ResourceDictionary"/>) de toutes les autres couches.</para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item>Résoudre l'URI du fichier XAML selon le code culture demandé.</item>
    /// <item>Charger et injecter le dictionnaire dans les ressources WPF fusionnées.</item>
    /// <item>Exposer l'accès aux textes traduits sans référencer <see cref="ResourceDictionary"/> à l'extérieur.</item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item>Aucune logique métier.</item>
    /// <item>Aucun accès aux données.</item>
    /// <item>Aucune orchestration de flux applicatif.</item>
    /// <item>Aucune propagation de CallChain.</item>
    /// </list>
    /// </remarks>
    public class SE_Language : ISE_Language
    {
        #region === Propriétés privées ===

        private ResourceDictionary? _currentDictionary;

        // --- URI internes des dictionnaires de langue ---
        // Constantes de configuration interne du SE_Language. Ces URI ne sont pas
        // exposées à l'extérieur : elles ne sont consommées qu'en interne par
        // GetDictionaryUri pour la résolution culture → URI.

        private static readonly Uri _dic_fr = new(
            "pack://application:,,,/DG244Cutting;component/D_Presentation/Resources/Languages/RE_Language.fr.xaml",
            UriKind.Absolute);

        private static readonly Uri _dic_en = new(
            "pack://application:,,,/DG244Cutting;component/D_Presentation/Resources/Languages/RE_Language.en.xaml",
            UriKind.Absolute);

        private static readonly Uri _dic_de = new(
            "pack://application:,,,/DG244Cutting;component/D_Presentation/Resources/Languages/RE_Language.de.xaml",
            UriKind.Absolute);

        private static readonly Uri _dic_es = new(
            "pack://application:,,,/DG244Cutting;component/D_Presentation/Resources/Languages/RE_Language.es.xaml",
            UriKind.Absolute);

        private static readonly Uri _dic_it = new(
            "pack://application:,,,/DG244Cutting;component/D_Presentation/Resources/Languages/RE_Language.it.xaml",
            UriKind.Absolute);

        private static readonly Uri _dic_pt = new(
            "pack://application:,,,/DG244Cutting;component/D_Presentation/Resources/Languages/RE_Language.pt.xaml",
            UriKind.Absolute);

        #endregion

        #region === Dépendances privées ===

        // A compléter

        #endregion

        #region === Propriétés publiques ===

        // A compléter

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="SE_Language"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Instanciée une seule fois par le conteneur DI au démarrage de l'application.</para>
        /// <para>Objectif : Garantir un état initial neutre — aucun dictionnaire chargé tant que
        /// <see cref="LoadDictionary"/> n'a pas été appelé.</para>
        /// </remarks>
        public SE_Language()
        {
            _currentDictionary = null;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <inheritdoc/>
        /// <remarks>
        /// <para>Contexte : Appelée par <c>SR_Language</c> avant le chargement du dictionnaire.</para>
        /// <para>Objectif : Centraliser la correspondance culture → URI. Si le code culture
        /// fourni n'est pas reconnu parmi les langues supportées (fr-FR, en-GB, de-DE, es-ES,
        /// it-IT, pt-PT), la méthode retourne silencieusement l'URI du dictionnaire français
        /// par défaut.</para>
        /// </remarks>
        public Uri GetDictionaryUri(string cultureCode)
        {
            return cultureCode switch
            {
                "fr-FR" => _dic_fr,
                "en-GB" => _dic_en,
                "de-DE" => _dic_de,
                "es-ES" => _dic_es,
                "it-IT" => _dic_it,
                "pt-PT" => _dic_pt,
                _ => _dic_en
            };
        }

        /// <inheritdoc/>
        /// <remarks>
        /// <para>Contexte : Appelée par <c>SR_Language</c> lors du démarrage de l'application
        /// ou d'un changement de langue explicite.</para>
        /// <para>Objectif : Remplacer atomiquement le dictionnaire actif dans
        /// <see cref="Application.Current"/>.<see cref="Application.Resources"/>.<see cref="ResourceDictionary.MergedDictionaries"/>,
        /// effet de bord global assumé sur l'application WPF, et conserver une référence locale
        /// pour la résolution ultérieure des clés via <see cref="GetEntry"/>.</para>
        /// </remarks>
        public void LoadDictionary(Uri dictionaryUri)
        {
            var dictionary = new ResourceDictionary { Source = dictionaryUri };

            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(dictionary);

            _currentDictionary = dictionary;
        }

        /// <inheritdoc/>
        /// <remarks>
        /// <para>Contexte : Appelée par <c>SR_Dictionary</c> pour résoudre chaque clé de traduction.</para>
        /// <para>Objectif : Retourner la valeur associée à la clé dans le dictionnaire actif,
        /// ou <see langword="null"/> silencieusement si aucun dictionnaire n'est chargé, si la clé
        /// est absente, ou si la valeur stockée n'est pas une chaîne. La responsabilité de réagir
        /// à un retour <see langword="null"/> revient à l'appelant.</para>
        /// </remarks>
        public string? GetEntry(string key)
        {
            if (_currentDictionary is null) return null;
            if (!_currentDictionary.Contains(key)) return null;
            return _currentDictionary[key] as string;
        }

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}