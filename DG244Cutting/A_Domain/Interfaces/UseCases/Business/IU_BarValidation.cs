using DG244Cutting.A_Domain.Common.Enums.Business;
using DG244Cutting.A_Domain.Common.Exceptions;

namespace DG244Cutting.A_Domain.Interfaces.UseCases.Business
{
    /// <summary>
    /// Contrat du UseCase de validation de la barre de production présentée à l'opérateur :
    /// engagement de la matière et scellement du plan de coupe, recomposé autour des zones
    /// défectueuses signalées, ou mise à l'écart de la barre lorsque ces défauts la rendent
    /// improductive pour la série.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : interface définie dans <c>A_Domain</c>, domaine <c>Business</c>. Elle est
    /// consommée par le ViewModel <c>VM_Page20</c>, via <c>IS_UseCaseInvoker</c> en chaîne (1)
    /// directe, lorsque l'opérateur valide la barre qui lui est présentée. L'exécution est
    /// déléguée à l'implémentation concrète
    /// <see cref="DG244Cutting.B_UseCases.UseCases.Business.UC_BarValidation"/> résidant en
    /// <c>B_UseCases/UseCases/Business</c>.
    /// </para>
    /// <para>
    /// Objectif : l'atelier est approvisionné à la demande ; chaque barre est préparée juste
    /// avant d'être coupée, sous la forme d'une barre provisoire portant un plan de coupe
    /// provisoire et, pour une barre de chute, la réservation de la chute source. L'opérateur
    /// prend la barre désignée, en constate l'état, puis la valide. Ce geste engage la matière et
    /// fige le plan de coupe ; son aboutissement ouvre la découpe des pièces puis la
    /// qualification du reliquat.
    /// </para>
    /// <para>
    /// Sans défaut signalé, le plan provisoire devient définitif à l'identique : la barre et ses
    /// découpes sont scellées sans recomposition. Lorsque l'opérateur signale une ou deux zones
    /// défectueuses, repérées en millimètres depuis la tête de la barre, la barre est acceptée
    /// mais son plan provisoire n'est plus valable : il est entièrement défait, puis recomposé
    /// en contournant les zones perdues. Si au moins une découpe peut être placée, la barre est
    /// scellée avec son nouveau plan. Si aucune découpe du vivier n'est plaçable dans aucun
    /// segment sain, la barre est écartée pour défauts : c'est une issue régulière du
    /// traitement, et non un échec ; les découpes restent disponibles pour une barre suivante.
    /// </para>
    /// <para>
    /// Frontière transactionnelle : pour chaque issue, l'ensemble des écritures - validation de
    /// la barre, mise à jour des découpes, retrait de la chute source, indicateur
    /// d'approvisionnement de la série, inscription au journal du cycle de vie - est validé ou
    /// annulé ensemble. Une barre scellée dont les découpes ne le seraient pas, ou une chute
    /// consommée sans barre validée correspondante, laisserait la série incohérente.
    /// </para>
    /// <para>
    /// Traitement terminal des erreurs : toute défaillance applicative typée est annulée,
    /// journalisée et notifiée par le UseCase lui-même ; le consommateur reçoit une issue
    /// interprétable sans connaissance de la cause et n'a aucune notification d'erreur à
    /// émettre.
    /// </para>
    /// <para>
    /// Typologie : UseCase de cas Concept à méthode publique unique. Le concept porté, la
    /// validation d'une barre de production, orchestre des écritures sur cinq entités
    /// distinctes (barre de production, découpe, chute du stock, série de production, journal du
    /// cycle de vie), dont aucune n'est seule cible. La méthode publique conserve le nom par
    /// défaut <c>ExecuteAsync</c>, sans dérogation au préfixe ni à la multiplicité. Elle expose
    /// un retour signalable destiné à la présentation (R-4.14.22), sous la forme d'une
    /// énumération d'état dont la sémantique est documentée sur la méthode.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Déclarer le point d'entrée unique de la validation de la barre de production présentée, avec ou sans zones défectueuses.</description></item>
    /// <item><description>Restituer à la présentation une issue distinguant la barre scellée, la barre écartée pour défauts et l'échec applicatif traité.</description></item>
    /// <item><description>Imposer la propagation de la CallChain via le paramètre <c>caller</c> contractuel.</description></item>
    /// <item><description>Imposer le support de l'annulation coopérative via un <c>CancellationToken</c>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>N'implémente aucune règle métier propre : le contrôle de l'état de la barre et de la cohérence des zones défectueuses, ainsi que chaque écriture, relèvent des Services métier ; la recomposition du plan relève du moteur d'optimisation.</description></item>
    /// <item><description>N'écrit pas directement en base : chaque écriture est déléguée à son Service métier.</description></item>
    /// <item><description>N'émet aucun message d'information à l'opérateur sur une barre écartée pour défauts : ce message relève du ViewModel consommateur, à réception de l'issue.</description></item>
    /// <item><description>Ne prépare pas la barre suivante et ne découpe aucune pièce.</description></item>
    /// <item><description>Ne qualifie pas le reliquat de la barre et ne restitue aucun reste au stock.</description></item>
    /// <item><description>N'expose aucun type technique de persistance, conformément à la pureté contractuelle de <c>A_Domain</c>.</description></item>
    /// </list>
    /// </remarks>
    public interface IU_BarValidation
    {
        // --- Groupe 1 : Validation de la barre de production présentée ---

        /// <summary>
        /// Valide la barre de production présentée et scelle son plan de coupe, en le recomposant
        /// autour des zones défectueuses signalées, ou écarte la barre lorsque ces défauts la
        /// rendent improductive pour la série.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : exécuté par <c>VM_Page20</c> lorsque l'opérateur valide la barre qui lui
        /// est présentée. Précondition : la barre a été préparée par le UseCase de préparation de
        /// barre et porte un plan de coupe provisoire non vide.
        /// </para>
        /// <para>
        /// Discriminant : une première borne de début de zone renseignée désigne le traitement
        /// avec défauts ; son absence désigne le traitement sans défaut. La cohérence des quatre
        /// bornes - complétude, ordre, non-chevauchement, inclusion dans la longueur de la barre -
        /// est contrôlée par le Service de validation de la barre.
        /// </para>
        /// <para>
        /// Effets observables, validés ensemble ou pas du tout :
        /// </para>
        /// <list type="bullet">
        /// <item><description>sans défaut : barre validée et scellée ; découpes rattachées scellées et marquées approvisionnées ; chute source retirée du stock pour une barre de chute ; indicateur d'approvisionnement de la série posé s'il ne l'était pas ; action de cycle de vie <c>BarValidated</c> inscrite sans commentaire ;</description></item>
        /// <item><description>avec défauts, au moins une découpe placée : zones défectueuses inscrites ; plan provisoire défait ; découpes retenues rattachées dans l'ordre calculé, le rang valant position de coupe, et scellées ; plan recomposé inscrit sur la barre et barre scellée ; chute source retirée du stock pour une barre de chute ; indicateur d'approvisionnement posé s'il ne l'était pas ; action de cycle de vie <c>BarWithDefectsValidated</c> inscrite sans commentaire ;</description></item>
        /// <item><description>avec défauts, aucune découpe plaçable : zones défectueuses inscrites et barre validée ; découpes détachées et rendues au vivier ; barre écartée pour défauts avec le motif <c>DEFECTS_NO_PLACEABLE_CUT</c> ; chute source retirée du stock pour une barre de chute ; aucun indicateur d'approvisionnement posé ; action de cycle de vie <c>BarRefused</c> inscrite avec le commentaire <c>DEFECTS_NO_PLACEABLE_CUT</c>.</description></item>
        /// </list>
        /// <para>
        /// Sur échec applicatif ou annulation, aucun effet ne persiste.
        /// </para>
        /// <para>
        /// Échecs métier traités terminalement : l'exécution est annulée, journalisée et notifiée,
        /// et l'issue vaut <see cref="En_BarValidationOutcome.Failed"/>, lorsque l'identifiant de
        /// barre n'est pas strictement positif (<see cref="Ex_Business"/>, code
        /// <c>BU_ER_02</c>) ; lorsque la barre est introuvable, ou lorsque la borne de fin de la
        /// première zone est absente sur le traitement avec défauts (<see cref="Ex_Business"/>,
        /// code <c>BU_ER_03</c>) ; lorsqu'une barre de chute ne référence aucune chute source, ou
        /// lorsque le moteur d'optimisation rend une issue inexploitable - anomalie de données,
        /// vivier vide, issue non valorisée ou inconnue (<see cref="Ex_Business"/>, code
        /// <c>BU_ER_04</c>). Les rejets émis en aval par les lectures, par le moteur
        /// d'optimisation et par les Services métier - notamment une barre déjà validée, épuisée,
        /// en rupture de stock ou supprimée, ou des zones défectueuses incohérentes -, ainsi que
        /// les défaillances techniques et imprévues typées (<see cref="Ex_Infrastructure"/>,
        /// <see cref="Ex_Unclassified"/>), suivent le même traitement.
        /// </para>
        /// <para>
        /// Défaillances non typées : une défaillance de persistance transitoire est rejouée par la
        /// stratégie d'exécution ; une défaillance de persistance non transitoire, ou l'épuisement
        /// des réexécutions, n'est pas typée par le UseCase et remonte au consommateur, où elle est
        /// captée par le filet de sécurité <c>VM_Generic.ExecuteSafeAsync</c>. Aucun effet ne
        /// persiste dans ce cas.
        /// </para>
        /// </remarks>
        /// <param name="caller">
        /// Chaîne d'appel reçue de l'appelant, enrichie localement selon le format normatif de la
        /// section 4.5. Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="idProductionBar">
        /// Identifiant de la barre de production présentée à l'opérateur. Doit être strictement
        /// positif.
        /// </param>
        /// <param name="defectStart1">
        /// Début de la première zone défectueuse, en millimètres depuis la tête de la barre.
        /// Renseigné, il désigne le traitement avec défauts ; <see langword="null"/>, il désigne
        /// le traitement sans défaut.
        /// </param>
        /// <param name="defectEnd1">
        /// Fin de la première zone défectueuse, en millimètres depuis la tête de la barre.
        /// Requise lorsque <paramref name="defectStart1"/> est renseigné.
        /// </param>
        /// <param name="defectStart2">
        /// Début de la seconde zone défectueuse, en millimètres depuis la tête de la barre.
        /// Facultatif.
        /// </param>
        /// <param name="defectEnd2">
        /// Fin de la seconde zone défectueuse, en millimètres depuis la tête de la barre.
        /// Requise lorsque <paramref name="defectStart2"/> est renseigné.
        /// </param>
        /// <param name="ct">
        /// Jeton d'annulation coopérative, propagé à tous les appels asynchrones en aval. Par
        /// défaut <see langword="default"/>.
        /// </param>
        /// <returns>
        /// Une tâche dont le résultat, restitué à la présentation, se lit ainsi :
        /// <list type="bullet">
        /// <item><description><see cref="En_BarValidationOutcome.BarSealed"/> : barre validée et scellée, avec ou sans défauts, après validation de la transaction ; le consommateur ouvre le choix entre la découpe des pièces et la qualification du reliquat ;</description></item>
        /// <item><description><see cref="En_BarValidationOutcome.BarUnusable"/> : barre écartée pour défauts, après validation de la transaction ; le consommateur informe l'opérateur et rafraîchit la page de découpe pour la barre suivante ;</description></item>
        /// <item><description><see cref="En_BarValidationOutcome.Failed"/> : échec applicatif annulé, journalisé et notifié ; le consommateur revient au tableau de bord des séries, sans notification à émettre.</description></item>
        /// </list>
        /// La valeur <see cref="En_BarValidationOutcome.Undetermined"/> n'est jamais retournée.
        /// </returns>
        /// <exception cref="OperationCanceledException">
        /// Seule exception applicative propagée à l'appelant, lorsque l'annulation coopérative est
        /// signalée via <paramref name="ct"/>, conformément à §4.6. Aucun effet ne persiste.
        /// </exception>
        Task<En_BarValidationOutcome> ExecuteAsync(
            string caller,
            int idProductionBar,
            int? defectStart1,
            int? defectEnd1,
            int? defectStart2,
            int? defectEnd2,
            CancellationToken ct = default);
    }
}