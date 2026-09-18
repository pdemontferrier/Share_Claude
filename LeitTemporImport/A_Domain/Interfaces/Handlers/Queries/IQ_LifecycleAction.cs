using LeitTemporImport.A_Domain.Interfaces.Handlers.Generic;
using LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Entities;

namespace LeitTemporImport.A_Domain.Interfaces.Handlers.Queries
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// Contrat de QueryHandler (QH) dédié à l’entité <see cref="LifecycleAction"/>.
    /// Hérite de <see cref="IQ_Generic{T}"/> afin de fournir les opérations de lecture génériques
    /// (ex : GetById, GetAll, GetFirstOrDefault) conformément au modèle CQRS.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Utilisé par la couche UseCases pour consulter les actions de cycle de vie enregistrées
    /// dans la base (ex : contrôles, diagnostic, cohérence de flux).
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Exposer un point d’accès typé aux lectures génériques de <see cref="LifecycleAction"/>
    /// sans ajouter de requêtes spécifiques à ce stade.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>UseCases / Services Application nécessitant des lectures sur le cycle de vie.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Fournir le contrat QH typé pour <see cref="LifecycleAction"/>.</description></item>
    /// <item><description>Hériter des lectures génériques via <see cref="IQ_Generic{T}"/>.</description></item>
    /// </list>
    /// </summary>
    public interface IQ_LifecycleAction : IQ_Generic<LifecycleAction>
    {
        // Requêtes spécifiques :
        // A compléter (si besoin ultérieur)
    }
}