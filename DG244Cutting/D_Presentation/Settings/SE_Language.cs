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

        // --- Table de correspondance codePays (ISO 3166-1 alpha-2) → cultureCode (.NET BCL) ---
        // Référentiel constitué à partir du CLDR de l'Unicode Consortium (élément territoryInfo),
        // par sélection de la langue à officialStatus="official" présentant le populationPercent
        // le plus élevé pour chaque pays (avec dérogation nominative explicite sur NZ). Couvre les
        // 27 pays de l'UE et une sélection des principaux pays hors UE dont la langue principale
        // ne pose pas d'ambiguïté. Les cultureCodes sont tous valides au sens BCL .NET 8.
        private static readonly Dictionary<string, string> _countryToCulture = new()
        {
            // Union européenne (27 pays)
            ["AT"] = "de-AT",   // Autriche            — allemand (97%, officielle unique)
            ["BE"] = "nl-BE",   // Belgique            — néerlandais (55%, tie-break population)
            ["BG"] = "bg-BG",   // Bulgarie            — bulgare (100%, officielle unique)
            ["CY"] = "el-CY",   // Chypre              — grec (95%, tie-break population)
            ["CZ"] = "cs-CZ",   // République tchèque  — tchèque (98%, officielle unique)
            ["DE"] = "de-DE",   // Allemagne           — allemand (91%, officielle unique)
            ["DK"] = "da-DK",   // Danemark            — danois (93%, officielle unique)
            ["EE"] = "et-EE",   // Estonie             — estonien (71%, officielle unique)
            ["ES"] = "es-ES",   // Espagne             — espagnol (99%, officielle unique)
            ["FI"] = "fi-FI",   // Finlande            — finnois (94%, tie-break population)
            ["FR"] = "fr-FR",   // France              — français (97%, officielle unique)
            ["GR"] = "el-GR",   // Grèce               — grec (99%, officielle unique)
            ["HR"] = "hr-HR",   // Croatie             — croate (99%, officielle unique)
            ["HU"] = "hu-HU",   // Hongrie             — hongrois (100%, officielle unique)
            ["IE"] = "en-IE",   // Irlande             — anglais (98%, tie-break population)
            ["IT"] = "it-IT",   // Italie              — italien (95%, officielle unique)
            ["LT"] = "lt-LT",   // Lituanie            — lituanien (86%, officielle unique)
            ["LU"] = "fr-LU",   // Luxembourg          — français (92%, tie-break population)
            ["LV"] = "lv-LV",   // Lettonie            — letton (61%, officielle unique)
            ["MT"] = "mt-MT",   // Malte               — maltais (100%, tie-break population)
            ["NL"] = "nl-NL",   // Pays-Bas            — néerlandais (100%, officielle unique)
            ["PL"] = "pl-PL",   // Pologne             — polonais (96%, officielle unique)
            ["PT"] = "pt-PT",   // Portugal            — portugais (96%, officielle unique)
            ["RO"] = "ro-RO",   // Roumanie            — roumain (90%, officielle unique)
            ["SE"] = "sv-SE",   // Suède               — suédois (95%, officielle unique)
            ["SI"] = "sl-SI",   // Slovénie            — slovène (87%, officielle unique)
            ["SK"] = "sk-SK",   // Slovaquie           — slovaque (90%, officielle unique)

            // Amérique du Nord et Centrale
            ["CA"] = "en-CA",   // Canada              — anglais (87%, tie-break net 3.0x vs fr 29%)
            ["MX"] = "es-MX",   // Mexique             — espagnol (83%, de_facto_official)
            ["US"] = "en-US",   // États-Unis          — anglais (96%, de_facto_official)

            // Amérique latine
            ["AR"] = "es-AR",   // Argentine           — espagnol (100%, officielle unique)
            ["BR"] = "pt-BR",   // Brésil              — portugais (91%, officielle unique)
            ["CL"] = "es-CL",   // Chili               — espagnol (98%, officielle unique)
            ["CO"] = "es-CO",   // Colombie            — espagnol (93%, officielle unique)
            ["CR"] = "es-CR",   // Costa Rica          — espagnol (95%, officielle unique)
            ["CU"] = "es-CU",   // Cuba                — espagnol (100%, officielle unique)
            ["DO"] = "es-DO",   // Rép. dominicaine    — espagnol (78%, officielle unique)
            ["EC"] = "es-EC",   // Équateur            — espagnol (96%, tie-break net 5.6x vs qu 17%)
            ["GT"] = "es-GT",   // Guatemala           — espagnol (88%, officielle unique)
            ["HN"] = "es-HN",   // Honduras            — espagnol (91%, officielle unique)
            ["NI"] = "es-NI",   // Nicaragua           — espagnol (78%, officielle unique)
            ["PA"] = "es-PA",   // Panama              — espagnol (69%, officielle unique)
            ["PE"] = "es-PE",   // Pérou               — espagnol (73%, tie-break net 4.9x vs qu 15%)
            ["SV"] = "es-SV",   // Salvador            — espagnol (89%, officielle unique)
            ["UY"] = "es-UY",   // Uruguay             — espagnol (88%, officielle unique)
            ["VE"] = "es-VE",   // Venezuela           — espagnol (82%, officielle unique)

            // Asie de l'Est
            ["CN"] = "zh-CN",   // Chine               — chinois simplifié (90%, officielle unique)
            ["JP"] = "ja-JP",   // Japon               — japonais (95%, officielle unique)
            ["KR"] = "ko-KR",   // Corée du Sud        — coréen (100%, officielle unique)

            // Asie du Sud-Est et du Sud
            ["BD"] = "bn-BD",   // Bangladesh          — bengali (98%, officielle unique)
            ["ID"] = "id-ID",   // Indonésie           — indonésien (64%, officielle unique)
            ["MY"] = "ms-MY",   // Malaisie            — malais (75%, officielle unique)
            ["TH"] = "th-TH",   // Thaïlande           — thaï (80%, officielle unique)
            ["VN"] = "vi-VN",   // Vietnam             — vietnamien (86%, officielle unique)

            // Asie occidentale et Moyen-Orient
            ["AE"] = "ar-AE",   // Émirats arabes unis — arabe (78%, officielle unique)
            ["IL"] = "he-IL",   // Israël              — hébreu (100%, tie-break net 5.0x vs ar 20%)
            ["IR"] = "fa-IR",   // Iran                — persan (75%, officielle unique)
            ["SA"] = "ar-SA",   // Arabie saoudite     — arabe (100%, officielle unique)
            ["TR"] = "tr-TR",   // Turquie             — turc (93%, officielle unique)

            // Océanie
            ["AU"] = "en-AU",   // Australie           — anglais (96%, de_facto_official)
            ["NZ"] = "en-NZ",   // Nouvelle-Zélande    — anglais (dérogation nominative : CLDR liste mi-NZ
                                //                       à 2.8% comme seule officielle législative ; substitution
                                //                       en-NZ par cohérence avec les pays anglo-saxons)

            // Europe non-UE
            ["AL"] = "sq-AL",   // Albanie             — albanais (100%, officielle unique)
            ["GB"] = "en-GB",   // Royaume-Uni         — anglais (98%, officielle unique)
            ["IS"] = "is-IS",   // Islande             — islandais (100%, officielle unique)
            ["LI"] = "de-LI",   // Liechtenstein       — allemand (100%, officielle unique)
            ["MD"] = "ro-MD",   // Moldavie            — roumain (63%, officielle unique)
            ["MK"] = "mk-MK",   // Macédoine du Nord   — macédonien (67%, officielle unique)
            ["RU"] = "ru-RU",   // Russie              — russe (99%, officielle unique)
            ["UA"] = "uk-UA",   // Ukraine             — ukrainien (65%, officielle unique)

            // Afrique du Nord
            ["DZ"] = "ar-DZ",   // Algérie             — arabe (74%, tie-break 2.2x vs fr 33%)
            ["EG"] = "ar-EG",   // Égypte              — arabe (94%, officielle unique)
        };

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
        /// <para>Contexte : Appelée par <c>UC_Language_Apply</c> avant le chargement du dictionnaire.</para>
        /// <para>Objectif : Centraliser la correspondance culture → URI. Si le code culture
        /// fourni n'est pas reconnu parmi les langues supportées (fr-FR, en-GB, de-DE, es-ES,
        /// it-IT, pt-PT), la méthode retourne silencieusement l'URI du dictionnaire anglais
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

        /// <inheritdoc/>
        /// <remarks>
        /// <para>Contexte : Appelée par le composant en charge de la détermination de la langue
        /// d'affichage à partir d'un identifiant pays issu d'un profil utilisateur ou d'une
        /// configuration régionale.</para>
        /// <para>Objectif : Lookup direct dans la table privée statique <see cref="_countryToCulture"/>
        /// après normalisation de l'entrée en majuscules par <see cref="string.ToUpperInvariant"/>.
        /// Pré-traitement défensif par <see cref="string.IsNullOrWhiteSpace(string?)"/>. Si l'entrée
        /// est non exploitable ou si le code pays normalisé n'est pas inscrit dans la table,
        /// la méthode retourne silencieusement <c>"en-GB"</c> par défaut, cohérent avec le repli
        /// implémenté par <see cref="GetDictionaryUri"/>.</para>
        /// </remarks>
        public string GetCultureCodeFromCountryCode(string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode)) return "en-GB";

            string key = countryCode.ToUpperInvariant();
            return _countryToCulture.TryGetValue(key, out var culture) ? culture : "en-GB";
        }

        /// <inheritdoc/>
        /// <remarks>
        /// <para>Contexte : Appelée par le composant en charge de l'extraction d'un identifiant
        /// pays normalisé à partir d'un code culture, pour interrogation de référentiels indexés
        /// par pays.</para>
        /// <para>Objectif : Extraction par recherche du dernier séparateur <c>'-'</c> via
        /// <see cref="string.LastIndexOf(char)"/>, suivie de la conversion en majuscules par
        /// <see cref="string.ToUpperInvariant"/>. Le recours à <c>LastIndexOf</c> et non
        /// <c>IndexOf</c> est délibéré pour préserver la robustesse face aux codes culture
        /// BCP-47 à trois segments présents dans la BCL .NET (<c>"zh-Hans-CN"</c> → <c>"CN"</c>,
        /// <c>"sr-Latn-RS"</c> → <c>"RS"</c>, <c>"uz-Latn-UZ"</c> → <c>"UZ"</c>). Pré-traitement
        /// défensif par <see cref="string.IsNullOrWhiteSpace(string?)"/>. Si l'entrée est non
        /// exploitable, la méthode retourne silencieusement <c>"GB"</c> par défaut. Si l'entrée
        /// ne contient aucun séparateur ou se termine par un séparateur sans segment suivant,
        /// le code culture complet est retourné en majuscules.</para>
        /// </remarks>
        public string ExtractCountryCodeFromCulture(string cultureCode)
        {
            if (string.IsNullOrWhiteSpace(cultureCode)) return "GB";

            int index = cultureCode.LastIndexOf('-');
            return index >= 0 && index < cultureCode.Length - 1
                ? cultureCode[(index + 1)..].ToUpperInvariant()
                : cultureCode.ToUpperInvariant();
        }

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}