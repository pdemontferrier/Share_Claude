namespace DG244Cutting.A_Domain.Interfaces.Services.App
{
    /// <summary>
    /// Description :
    /// <para>
    /// Contrat du service de hachage : capacité technique transverse de calcul
    /// d'empreinte d'une chaîne de caractères, restituée sous forme de chaîne
    /// hexadécimale en minuscules.
    /// </para>
    ///
    /// Contexte :
    /// <para>
    /// Cette interface est implémentée par <see cref="DG244Cutting.B_UseCases.Services.App.SR_Hashing"/>
    /// dans la couche <c>B_UseCases</c>. Elle est injectée dans les UseCases orchestrateurs
    /// (typiquement un UseCase d'authentification) qui rattachent l'empreinte produite à une
    /// validation métier de leur ressort.
    /// Sa définition dans <c>A_Domain</c> garantit que la couche métier ne dépend d'aucune
    /// technologie de cryptographie tierce, l'algorithme concret restant confiné à
    /// l'implémentation.
    /// </para>
    ///
    /// Objectif :
    /// <para>
    /// Définir le contrat minimal permettant à un consommateur amont de calculer l'empreinte
    /// d'une chaîne neutre, sans exposer l'algorithme concret retenu ni aucune notion de
    /// mot de passe, de login ou de session.
    /// </para>
    ///
    /// Responsabilités :
    /// <list type="bullet">
    /// <item><description>Exposer une opération unitaire de calcul d'empreinte d'une chaîne.</description></item>
    /// <item><description>Restituer l'empreinte sous forme de chaîne hexadécimale en minuscules.</description></item>
    /// </list>
    ///
    /// Non-responsabilités :
    /// <list type="bullet">
    /// <item><description>Aucune règle métier (pas de notion de mot de passe, login ni session).</description></item>
    /// <item><description>Aucun rattachement de l'empreinte à une validation quelconque : cette
    /// orchestration relève d'un UseCase consommateur distinct.</description></item>
    /// <item><description>Aucune exposition de l'algorithme de hachage concret au contrat.</description></item>
    /// </list>
    ///
    /// Utilisateurs cibles :
    /// <para>
    /// UseCases orchestrateurs (<c>UC_*</c>) ayant besoin de comparer une empreinte à des
    /// empreintes déjà persistées.
    /// </para>
    /// </summary>
    public interface IS_Hashing
    {
        /// <summary>
        /// Description :
        /// <para>
        /// Calcule l'empreinte d'une chaîne de caractères et la restitue sous forme de
        /// chaîne hexadécimale en minuscules.
        /// </para>
        ///
        /// Contexte :
        /// <para>
        /// Cette méthode constitue le point d'entrée unique du service. Elle réalise un calcul
        /// pur en mémoire, sans effet de bord, sans I/O ni mutation d'état. L'empreinte produite
        /// est déterministe : deux appels sur une même entrée produisent une empreinte identique.
        /// </para>
        ///
        /// Objectif :
        /// <para>
        /// Restituer une empreinte hexadécimale minuscule exploitable par le consommateur amont
        /// pour une comparaison ultérieure.
        /// </para>
        ///
        /// Responsabilités :
        /// <list type="bullet">
        /// <item><description>Produire l'empreinte de la chaîne fournie.</description></item>
        /// <item><description>Restituer l'empreinte en minuscules.</description></item>
        /// <item><description>Traiter une entrée <c>null</c> ou vide comme la chaîne vide (robustesse).</description></item>
        /// </list>
        ///
        /// Non-responsabilités :
        /// <list type="bullet">
        /// <item><description>Aucune interprétation métier de la chaîne fournie.</description></item>
        /// </list>
        /// </summary>
        /// <param name="input">
        /// Chaîne dont l'empreinte est calculée. Une valeur <c>null</c> ou vide est admise et
        /// traitée comme la chaîne vide.
        /// </param>
        /// <returns>
        /// L'empreinte hexadécimale de <paramref name="input"/> en minuscules.
        /// </returns>
        string Hash(string input);
    }
}