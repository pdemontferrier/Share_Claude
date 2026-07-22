namespace DG244Cutting.A_Domain.Interfaces.Settings.Presentation
{
    /// <summary>
    /// Contrat du composant Singleton de présentation exposant le référentiel des sections de profil de barre.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : contrat défini dans <c>A_Domain</c>, consommé par injection dans les ViewModels
    /// et les composants visuels chargés de l'affichage de l'image d'une section de profil de barre.
    /// Constitue l'unique point d'entrée vers
    /// <see cref="DG244Cutting.D_Presentation.Settings.SE_BarProfilSection"/>.</para>
    /// <para>Objectif : exposer le référentiel des URI des sections de profil de barre et fournir la
    /// résolution centralisée d'une référence textuelle de section vers son URI, avec repli explicite
    /// sur la section par défaut lorsque la référence demandée n'est pas reconnue.</para>
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
    /// <item>Aucun état mutable observable (Setting immuable au sens strict)</item>
    /// </list>
    /// </remarks>
    public interface ISE_BarProfilSection
    {
        // --- Groupe 1 : Section de profil de barre par défaut (immuable) ---

        /// <summary>
        /// Obtient l'URI de la section de profil de barre par défaut utilisée en cas de repli.
        /// </summary>
        /// <value>URI absolue de la section par défaut, stable sur toute la durée de vie de l'application.</value>
        /// <remarks>
        /// <para>Contexte : valeur stable, initialisée au démarrage à partir du référentiel statique
        /// sous-jacent, utilisée lorsque la référence fournie à
        /// <see cref="GetBarProfilSectionUriOrDefault(string)"/> n'est pas reconnue dans le référentiel.</para>
        /// <para>Objectif : garantir un affichage de repli cohérent lorsque la référence demandée est
        /// inconnue, invalide, vide ou nulle.</para>
        /// </remarks>
        Uri DefaultBarProfilSectionUri { get; }

        // --- Groupe 2 : Référentiel des sections de profil de barre (lecture seule) ---

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
        /// de profil de barre, sans exposer l'implémentation interne du référentiel statique
        /// sous-jacent <c>RS_BarProfilSection</c>.</para>
        /// </remarks>
        IReadOnlyDictionary<string, Uri> ReferenceBarProfilSection { get; }

        // --- Groupe 3 : Opérations ---

        /// <summary>
        /// Retourne l'URI de la section de profil de barre correspondant à la référence donnée, ou la section par défaut.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : appelée par les ViewModels et les composants visuels lors de l'affichage
        /// d'une section de profil de barre à partir de sa référence textuelle métier (référence
        /// portée par une pièce ou par une barre).</para>
        /// <para>Objectif : fournir une résolution centralisée d'une référence textuelle vers son URI,
        /// avec repli explicite sur <see cref="DefaultBarProfilSectionUri"/> lorsque la référence
        /// n'est pas reconnue, est vide ou est <see langword="null"/>.</para>
        /// </remarks>
        /// <param name="reference">Référence textuelle de la section de profil de barre à résoudre
        /// (par exemple <c>"02CJ"</c>, <c>"10200"</c>, <c>"P813760"</c>). Une valeur
        /// <see langword="null"/>, vide, blanche ou non trouvée dans le référentiel entraîne le
        /// retour de la section par défaut.</param>
        /// <returns>URI absolue de la section de profil de barre correspondant à la référence,
        /// ou <see cref="DefaultBarProfilSectionUri"/> si la référence n'est pas reconnue.</returns>
        Uri GetBarProfilSectionUriOrDefault(string reference);
    }
}