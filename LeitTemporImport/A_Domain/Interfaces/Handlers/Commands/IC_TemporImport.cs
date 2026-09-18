using LeitTemporImport.A_Domain.Interfaces.Handlers.Generic;
using LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Entities;

namespace LeitTemporImport.A_Domain.Interfaces.Handlers.Commands
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// CommandHandler (CH) dédié à l’entité <see cref="Tempor_Import"/>.
    /// Hérite de <see cref="CH_Generic{T}"/> afin de fournir les commandes génériques (CRUD)
    /// via un <see cref="IR_Generic{T}"/> et la journalisation des écritures via <c>Tempor_Import</c>.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Utilisé par les UseCases/Services lors de l’exécution batch afin d’enregistrer les événements
    /// de cycle de vie (ex : Start/Stop, checkpoints).
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Exposer un CH typé pour <see cref="Tempor_Import"/> sans ajouter de logique spécifique
    /// à ce stade, tout en respectant les conventions de traçabilité et de structure.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>Couche UseCases : Services et UseCases qui écrivent dans la table Tempor_Import.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Initialiser le handler typé.</description></item>
    /// <item><description>Déléguer les commandes génériques au parent <see cref="CH_Generic{T}"/>.</description></item>
    /// </list>
    /// </summary>
    public interface IC_TemporImport : IC_Generic<Tempor_Import>
    {
    }
}