using DG244Cutting.A_Domain.Common.Enums.Business;
using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.DTOs.App;

namespace DG244Cutting.A_Domain.Interfaces.Services.Business
{
    /// <summary>
    /// Contrat du service métier d'inscription, au journal métier <c>LifecycleAction</c>, d'une
    /// entrée décrivant une action structurante du parcours de production, située dans le
    /// contexte applicatif courant.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : interface définie dans <c>A_Domain</c>, domaine <c>Business</c>. Elle est
    /// consommée par injection de dépendances par les UseCases orchestrateurs du parcours de
    /// découpe (validation, refus ou validation avec défauts d'une barre, déclaration de rupture
    /// de stock, clôture de série, réalisation ou refus d'une découpe), qui en délèguent
    /// l'exécution au service concret
    /// <see cref="DG244Cutting.B_UseCases.Services.Business.SR_LifecycleAction_Add"/> résidant en
    /// <c>B_UseCases/Services/Business</c>.
    /// </para>
    /// <para>
    /// Objectif : les décisions qui jalonnent le parcours d'une série doivent pouvoir être
    /// retracées a posteriori — quelle action, sur quelle entité, par qui et quand. Le journal
    /// métier <c>LifecycleAction</c> porte cette traçabilité ; il enregistre des événements du
    /// cycle de vie des produits et se distingue du journal technique des incidents
    /// d'exécution. Le contrat en est le point d'écriture unique au sein de l'application : la
    /// table, partagée dans la base de production, peut être alimentée par d'autres
    /// applications, mais aucun autre composant de l'application n'y écrit.
    /// </para>
    /// <para>
    /// Répartition des rôles : l'appelant décrit l'événement (entité concernée par sa source et
    /// son identifiant, nature de l'action, commentaire éventuel, typiquement le motif d'un
    /// refus) et transmet le contexte applicatif en un objet unique
    /// (<see cref="DTO_AppContext"/>), qu'il a lui-même obtenu. Le service situe l'événement :
    /// il porte seul la correspondance entre ce contexte et les champs de contexte de l'entrée
    /// (application, utilisateur, horodatage métier, poste), de sorte que cette correspondance
    /// n'est jamais reproduite chez les appelants.
    /// </para>
    /// <para>
    /// Valeurs d'énumération : les types <see cref="En_LifecycleActionSource"/> et
    /// <see cref="En_LifecycleActionType"/>, dépourvus de valeur sentinelle, désignent le
    /// présent service comme responsable du rejet de toute valeur non déclarée et de la
    /// conversion explicite de leurs valeurs vers les identifiants <see langword="short"/> de
    /// l'entrée de journal. Le contrat assume ces deux responsabilités.
    /// </para>
    /// <para>
    /// Cohérence source / type non contrôlée : la correspondance admissible entre une source et
    /// une nature d'action n'est pas portée par le modèle de données ; l'inscrire dans le
    /// service en ferait une seconde source de vérité, à maintenir à chaque évolution des deux
    /// référentiels. Le risque assumé est qu'un couple incohérent soit inscrit sans être
    /// signalé ; les couples étant écrits en littéraux au point d'appel, leur cohérence se
    /// vérifie à la lecture du code appelant. La valeur <see cref="En_LifecycleActionType.BarRefused"/>
    /// couvre deux situations que la base ne distingue pas — le refus d'une barre par
    /// l'opérateur et la barre-déchet du chemin de validation avec défauts — ; c'est le
    /// commentaire qui porte la distinction.
    /// </para>
    /// <para>
    /// Deux horodatages distincts : l'horodatage de l'action est l'horodatage métier, issu du
    /// contexte applicatif transmis ; la date de création de la ligne est l'horodatage
    /// technique de l'insertion, positionné par le socle d'écriture générique.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Déclarer l'opération unitaire d'inscription d'une entrée au journal métier du cycle de vie.</description></item>
    /// <item><description>Rejeter les valeurs d'énumération non déclarées, les identifiants non strictement positifs et les commentaires excédant la capacité de la colonne.</description></item>
    /// <item><description>Imposer la propagation de la CallChain via le paramètre <c>caller</c> contractuel.</description></item>
    /// <item><description>Imposer le support de l'annulation coopérative via un <c>CancellationToken</c>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne persiste pas et ne pilote aucune transaction : l'entrée partage le sort de la transaction métier du UseCase appelant ; une action annulée ne laisse aucune trace, une action validée en laisse toujours une.</description></item>
    /// <item><description>Ne modifie ni ne supprime jamais une entrée existante : le journal est alimenté en insertion seule.</description></item>
    /// <item><description>Ne lit pas le contexte applicatif : celui-ci est obtenu par l'appelant et transmis en un objet unique.</description></item>
    /// <item><description>Ne contrôle pas la cohérence entre la source et la nature de l'action.</description></item>
    /// <item><description>Ne vérifie pas l'existence de l'entité désignée par la source et son identifiant.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="DG244Cutting.A_Domain.Interfaces.Handlers.Generic.IC_Generic{T}"/>
    /// <seealso cref="DG244Cutting.A_Domain.Entities.DIGIT_TRY.LifecycleAction"/>
    public interface IS_LifecycleAction_Add
    {
        // --- Groupe 1 : Inscription d'une entrée au journal métier ---

        /// <summary>
        /// Construit l'entrée de journal métier décrivant une action du cycle de vie sur une
        /// entité désignée, la situe dans le contexte applicatif fourni, puis la confie à
        /// l'écriture générique sans la persister.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par un UseCase orchestrateur du parcours de production, à
        /// l'intérieur de la transaction qu'il a ouverte, aux côtés des écritures métier que
        /// l'entrée documente. La mutation est déléguée au Command Handler générique
        /// <c>IC_Generic&lt;LifecycleAction&gt;</c>, qui positionne la date de création et
        /// inscrit l'événement associé ; l'entrée n'est enregistrée qu'à la validation de la
        /// transaction par l'appelant.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Vérifier, dans l'ordre : présence du contexte applicatif, identifiants d'application et d'utilisateur strictement positifs, source et nature d'action déclarées, identifiant de l'entité strictement positif, puis longueur du commentaire normalisé.</description></item>
        /// <item><description>Normaliser le commentaire : une valeur <see langword="null"/>, vide ou composée uniquement d'espaces devient <see langword="null"/> ; toute autre valeur est conservée telle quelle, sans être rognée ni tronquée.</description></item>
        /// <item><description>Convertir la source et la nature d'action vers les identifiants <see langword="short"/> de l'entrée.</description></item>
        /// <item><description>Reporter sur l'entrée l'application, l'utilisateur, l'horodatage métier et les informations de poste issus du contexte applicatif.</description></item>
        /// <item><description>Déléguer l'écriture au Command Handler générique.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Ne contrôle pas la cohérence du couple source / nature d'action.</description></item>
        /// <item><description>Ne contrôle pas la longueur des informations de poste.</description></item>
        /// <item><description>Ne positionne pas les champs d'audit, n'appelle pas <c>SaveChangesAsync</c> et ne relit pas l'entrée après écriture.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">
        /// Chaîne d'appel reçue de l'appelant, enrichie localement puis propagée à l'aval.
        /// Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="appContext">
        /// Contexte applicatif courant fournissant l'application, l'utilisateur, l'horodatage
        /// métier de l'action et les informations de poste. Ne doit pas être
        /// <see langword="null"/> ; ses identifiants d'application et d'utilisateur doivent être
        /// strictement positifs.
        /// </param>
        /// <param name="source">
        /// Entité métier sur laquelle porte l'action ; désigne la table dans laquelle rechercher
        /// la ligne identifiée par <paramref name="idSource"/>. Doit être une valeur déclarée de
        /// l'énumération.
        /// </param>
        /// <param name="type">
        /// Nature de l'action du cycle de vie. Doit être une valeur déclarée de l'énumération.
        /// </param>
        /// <param name="idSource">
        /// Identifiant technique de l'entité concernée dans la table désignée par
        /// <paramref name="source"/>. Doit être strictement positif : la colonne portant une
        /// valeur par défaut en base, une valeur nulle serait silencieusement remplacée à
        /// l'insertion et rattacherait l'entrée à une entité étrangère à l'action.
        /// </param>
        /// <param name="comments">
        /// Commentaire libre facultatif, typiquement le motif d'un refus ou la distinction entre
        /// les deux situations couvertes par <see cref="En_LifecycleActionType.BarRefused"/>.
        /// Après normalisation, sa longueur ne doit pas excéder 500 caractères ; un commentaire
        /// plus long est rejeté plutôt que tronqué, afin que son contenu ne soit jamais altéré.
        /// Par défaut <see langword="null"/>.
        /// </param>
        /// <param name="ct">Jeton d'annulation coopérative, propagé au Command Handler. Par défaut <see langword="default"/>.</param>
        /// <returns>Tâche représentant l'opération asynchrone, sans valeur de retour.</returns>
        /// <exception cref="Ex_Business">
        /// Levée avec le code <c>BU_ER_01</c> si <paramref name="appContext"/> est
        /// <see langword="null"/> ; avec le code <c>BU_ER_02</c> si l'identifiant d'application
        /// ou d'utilisateur du contexte n'est pas strictement positif, si
        /// <paramref name="source"/> ou <paramref name="type"/> n'est pas une valeur déclarée, si
        /// <paramref name="idSource"/> n'est pas strictement positif, ou si le commentaire
        /// normalisé excède 500 caractères.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">Levée si une défaillance technique survient lors de la délégation au Command Handler.</exception>
        /// <exception cref="Ex_Unclassified">Levée si une exception non prévue est interceptée et ne relève d'aucune catégorie typée après requalification.</exception>
        /// <exception cref="OperationCanceledException">Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.</exception>
        Task ExecuteAsync(
            string caller,
            DTO_AppContext appContext,
            En_LifecycleActionSource source,
            En_LifecycleActionType type,
            int idSource,
            string? comments = null,
            CancellationToken ct = default);
    }
}