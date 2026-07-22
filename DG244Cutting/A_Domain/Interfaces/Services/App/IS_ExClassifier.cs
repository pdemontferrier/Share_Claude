using DG244Cutting.A_Domain.Common.Exceptions;

namespace DG244Cutting.A_Domain.Interfaces.Services.App
{
    /// <summary>
    /// Description :
    /// <para>
    /// Contrat du service de classification des exceptions .NET et tierces en exceptions
    /// typées du projet.
    /// </para>
    ///
    /// Contexte :
    /// <para>
    /// Cette interface est implémentée par <c>SR_ExClassifier</c> dans la couche
    /// <c>B_UseCases</c>. Elle est injectée dans les Services (<c>SR_*</c>) afin de
    /// permettre la classification des exceptions non contrôlées interceptées dans les
    /// blocs <c>catch (Exception ex)</c> terminaux.
    /// Sa définition dans <c>A_Domain</c> garantit que la couche métier ne dépend
    /// d'aucune technologie tierce (EF Core, sockets) tout en permettant à l'implémentation
    /// de les référencer librement dans <c>B_UseCases</c>.
    /// </para>
    ///
    /// Objectif :
    /// <para>
    /// Définir le contrat minimal permettant aux Services de classifier et contextualiser
    /// toute exception non prévue en une exception typée du projet, sans exposer
    /// les détails d'implémentation de la classification.
    /// </para>
    ///
    /// Utilisateurs cibles :
    /// <para>
    /// Services (<c>SR_*</c>) uniquement, dans leur bloc <c>catch (Exception ex)</c> terminal.
    /// </para>
    ///
    /// Exemple d'utilisation :
    /// <code>
    /// catch (Exception ex)
    /// {
    ///     throw _classifier.Execute(callChain, ex);
    /// }
    /// </code>
    /// </summary>
    public interface IS_ExClassifier
    {
        /// <summary>
        /// Description :
        /// <para>
        /// Classifie une exception selon sa nature et l'enveloppe dans le type d'exception
        /// correspondant, enrichi des informations techniques disponibles.
        /// </para>
        ///
        /// Contexte :
        /// <para>
        /// Cette méthode est invoquée depuis le bloc <c>catch (Exception ex)</c> terminal
        /// des Services. Elle ne reclassifie pas les exceptions déjà typées
        /// (<see cref="Ex_Business"/>, <see cref="Ex_Infrastructure"/>,
        /// <see cref="Ex_Unclassified"/>), qu'elle retourne immédiatement sans modification.
        /// </para>
        ///
        /// Objectif :
        /// <para>
        /// Retourner systématiquement une exception typée et contextualisée,
        /// avec l'exception d'origine enchaînée en <c>innerException</c>.
        /// </para>
        /// </summary>
        /// <param name="callChain">
        /// Chaîne d'appels complète transmise par le Service appelant au moment de l'interception.
        /// </param>
        /// <param name="ex">
        /// Exception d'origine à classifier. Ne doit pas être nulle.
        /// </param>
        /// <returns>
        /// Une instance contextualisée de <see cref="Ex_Business"/>,
        /// <see cref="Ex_Infrastructure"/> ou <see cref="Ex_Unclassified"/>,
        /// avec l'exception d'origine enchaînée en <c>innerException</c>.
        /// </returns>
        Exception Execute(string callChain, Exception ex);
    }
}