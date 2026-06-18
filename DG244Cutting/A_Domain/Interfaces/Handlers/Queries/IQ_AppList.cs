using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Generic;

namespace DG244Cutting.A_Domain.Interfaces.Handlers.Queries
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// Contrat du QueryHandler (IQ) dédié à l’entité <see cref="AppList"/> dans le
    /// cadre du modèle CQRS. Hérite du socle de lecture <see cref="IQ_Generic{T}"/>
    /// (six lectures de base, non redéclarées ici) et ajoute une lecture spécialisée
    /// par clé fonctionnelle : l’accessibilité d’une application.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Consommé en lecture par les composants amont via injection du contrat
    /// <see cref="IQ_AppList"/>. La couche de résidence du consommateur direct est
    /// fonction des chaînes d’appel autorisées par §4.14.9 du 0230.
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Fournir une lecture CQRS dédiée, traçable (CallChain) et robuste
    /// (classification d’exceptions homogène), sans repository spécialisé : le
    /// filtrage est délégué au socle générique hérité (sous-cas (i) de §4.14.5).
    /// </para>
    /// </summary>
    public interface IQ_AppList : IQ_Generic<AppList>
    {
        /// <summary>
        /// <para>Description</para>
        /// <para>
        /// Indique si l’application identifiée par <paramref name="appId"/> est
        /// actuellement accessible (champ <c>Accessible == true</c>) et non
        /// supprimée logiquement (<c>IsDeleted == false</c>).
        /// </para>
        /// <para>Sémantique du résultat</para>
        /// <para>
        /// La réponse est strictement binaire : <see langword="true"/> si au moins
        /// un enregistrement <see cref="AppList"/> satisfait conjointement les
        /// trois critères <c>Id == appId</c>, <c>Accessible == true</c> et
        /// <c>IsDeleted == false</c> ; <see langword="false"/> sinon. Aucune
        /// distinction n’est exposée entre « l’application n’existe pas » et
        /// « l’application existe mais n’est pas accessible » (volonté
        /// fonctionnelle assumée).
        /// </para>
        /// </summary>
        /// <param name="caller">CallChain construite par le composant appelant.</param>
        /// <param name="appId">
        /// Identifiant entier de l’application à interroger. Doit être strictement
        /// positif.
        /// </param>
        /// <param name="ct">
        /// Jeton d’annulation permettant d’interrompre l’opération de manière
        /// coopérative.
        /// </param>
        /// <returns>
        /// <see langword="true"/> si l’application est accessible et non supprimée ;
        /// <see langword="false"/> sinon (y compris si l’application n’existe pas).
        /// </returns>
        /// <exception cref="Ex_Business">
        /// Levée (code <c>BU_ER_02</c>) si <paramref name="appId"/> est inférieur
        /// ou égal à zéro (précondition structurelle).
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique survient lors de la lecture.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l’annulation est signalée via <paramref name="ct"/> avant ou
        /// pendant l’exécution.
        /// </exception>
        Task<bool> HandleAppAccessibilityAsync(string caller, int appId, CancellationToken ct = default);
    }
}