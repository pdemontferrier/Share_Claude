using LeitTemporImport.A_Domain.Interfaces.Handlers.Generic;
using LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Entities;

namespace LeitTemporImport.A_Domain.Interfaces.Handlers.Commands
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// Contrat de CommandHandler (CH) dédié à l’entité <see cref="LifecycleAction"/>.
    /// Hérite de <see cref="IC_Generic{T}"/> afin d’exposer les commandes génériques (CRUD)
    /// appliquées à l’historisation du cycle de vie métier.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Utilisé par les UseCases/Services pour enregistrer des actions de cycle de vie
    /// des tables principales métiers(ex : série, commande client) dans la table  <see cref="LifecycleAction"/>.
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Fournir un point d’entrée typé pour les commandes génériques sur <see cref="LifecycleAction"/>,
    /// tout en conservant la mécanique standard de traçabilité (callChain) et d’historisation
    /// (UserAppEventStore) portée par <see cref="IC_Generic{T}"/>.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>UseCases / Services Application responsables d’écrire dans le cycle de vie.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Exposer les commandes génériques (Add/Update/Delete/SaveChanges).</description></item>
    /// <item><description>Permettre l’ajout ultérieur de commandes spécifiques si nécessaire.</description></item>
    /// </list>
    /// </summary>
    public interface IC_LifecycleAction : IC_Generic<LifecycleAction>
    {
        // Commandes spécifiques :
        // A compléter (si besoin ultérieur)
    }
}