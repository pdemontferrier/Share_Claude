using DG244Cutting.A_Domain.Interfaces.Settings.Presentation;

namespace DG244Cutting.D_Presentation.Settings
{
    /// <summary>
    /// Composant Singleton de présentation exposant le référentiel des sections de profil de barre.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : composant Singleton de présentation injectable via
    /// <see cref="ISE_BarProfilSection"/>, enregistré dans le Composition Root. Consommé exclusivement
    /// par les composants de présentation (ViewModels, contrôles visuels) chargés de l'affichage de
    /// l'image d'une section de profil de barre à partir d'une référence textuelle métier.</para>
    /// <para>Objectif : intermédier l'accès au référentiel statique <see cref="RS_BarProfilSection"/>
    /// en exposant un contrat injectable et testable, et fournir la résolution centralisée d'une
    /// référence textuelle de section vers son URI avec repli explicite sur la section par défaut.</para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item>Exposer l'URI de la section de profil de barre par défaut (lecture seule, valeur stable)</item>
    /// <item>Exposer le référentiel des sections de profil de barre indexé par référence textuelle</item>
    /// <item>Résoudre une référence textuelle vers son URI avec repli sur la section par défaut</item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item>Aucune logique métier ni orchestration applicative</item>
    /// <item>Aucun accès aux données ou services externes</item>
    /// <item>Aucune propagation de CallChain</item>
    /// <item>Aucun état mutable observable (Setting immuable au sens strict, aucun héritage INPC)</item>
    /// </list>
    /// </remarks>
    public class SE_BarProfilSection : ISE_BarProfilSection
    {
        #region === Propriétés privées ===

        // A compléter

        #endregion

        #region === Dépendances privées ===

        // A compléter

        #endregion

        #region === Propriétés publiques ===

        /// <summary>
        /// Obtient l'URI de la section de profil de barre par défaut utilisée en cas de repli.
        /// </summary>
        /// <value>URI absolue de la section par défaut, stable sur toute la durée de vie de l'application.</value>
        /// <remarks>
        /// <para>Contexte : valeur stable, lue une fois à la construction depuis le référentiel statique
        /// <see cref="RS_BarProfilSection.DefaultBarProfilSection_Source"/>, puis conservée sans
        /// modification possible.</para>
        /// <para>Objectif : garantir un affichage de repli cohérent lorsque la référence demandée à
        /// <see cref="GetBarProfilSectionUriOrDefault(string)"/> est inconnue, vide ou nulle.</para>
        /// </remarks>
        public Uri DefaultBarProfilSectionUri { get; } = RS_BarProfilSection.DefaultBarProfilSection_Source;

        /// <summary>
        /// Obtient le référentiel des sections de profil de barre disponibles, indexé par référence textuelle.
        /// </summary>
        /// <value>Dictionnaire en lecture seule associant la référence textuelle de la section
        /// (par exemple <c>"02CJ"</c>, <c>"10200"</c>, <c>"P813760"</c>) à l'URI absolue de la
        /// ressource correspondante.</value>
        /// <remarks>
        /// <para>Contexte : utilisé en interne par <see cref="GetBarProfilSectionUriOrDefault(string)"/>
        /// et exposé aux consommateurs nécessitant l'accès direct au référentiel complet.</para>
        /// <para>Objectif : fournir un accès centralisé en lecture seule au dictionnaire des sections
        /// de profil de barre sans exposer l'implémentation interne du référentiel statique sous-jacent
        /// <see cref="RS_BarProfilSection"/>.</para>
        /// </remarks>
        public IReadOnlyDictionary<string, Uri> ReferenceBarProfilSection => RS_BarProfilSection.ReferenceBSP;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="SE_BarProfilSection"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : instanciée exclusivement par le conteneur d'injection de dépendances au
        /// démarrage de l'application, en portée Singleton (jalon 1 de la séquence de démarrage,
        /// §3.10.4).</para>
        /// <para>Objectif : Setting immuable au sens strict — aucune initialisation locale au-delà
        /// de la valeur par défaut posée à la déclaration de <see cref="DefaultBarProfilSectionUri"/>.
        /// Aucune dépendance injectée, aucun état mutable à initialiser.</para>
        /// </remarks>
        public SE_BarProfilSection()
        {
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Retourne l'URI de la section de profil de barre correspondant à la référence donnée, ou la section par défaut.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : appelée par les ViewModels et les composants visuels lors de l'affichage
        /// d'une section de profil de barre à partir de sa référence textuelle métier (référence
        /// portée par une pièce ou par une barre).</para>
        /// <para>Objectif : fournir une résolution centralisée d'une référence textuelle vers son URI,
        /// avec repli explicite sur <see cref="DefaultBarProfilSectionUri"/> lorsque la référence
        /// n'est pas reconnue, est vide, blanche ou <see langword="null"/>.</para>
        /// </remarks>
        /// <param name="reference">Référence textuelle de la section de profil de barre à résoudre
        /// (par exemple <c>"02CJ"</c>, <c>"10200"</c>, <c>"P813760"</c>). Une valeur
        /// <see langword="null"/>, vide, blanche ou non trouvée dans le référentiel entraîne le
        /// retour de la section par défaut.</param>
        /// <returns>URI absolue de la section de profil de barre correspondant à la référence,
        /// ou <see cref="DefaultBarProfilSectionUri"/> si la référence n'est pas reconnue.</returns>
        public Uri GetBarProfilSectionUriOrDefault(string reference)
        {
            if (string.IsNullOrWhiteSpace(reference))
                return DefaultBarProfilSectionUri;

            return ReferenceBarProfilSection.TryGetValue(reference, out Uri? uri)
                ? uri
                : DefaultBarProfilSectionUri;
        }

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}