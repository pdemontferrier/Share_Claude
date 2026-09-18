using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Interfaces.Settings.Presentation;

namespace DG244Cutting.A_Domain.Interfaces.Services.Presentation
{
    /// <summary>
    /// Description :
    /// <para>
    /// Contrat du service technique transverse de présentation responsable de
    /// l'affichage des notifications utilisateur et du pilotage de la fenêtre
    /// de dialogue non bloquante (étalon WPF) dans l'application.
    /// </para>
    ///
    /// Contexte :
    /// <para>
    /// L'interface réside en <c>A_Domain/Interfaces/Services/Presentation/</c>
    /// (couche d'appartenance unique pour les contrats de la famille IS_,
    /// invariant 1 de §2.1 du 0230). Son implémentation concrète
    /// <see cref="DG244Cutting.D_Presentation.Services.SR_Notification"/> réside
    /// en <c>D_Presentation/Services/</c>. Le service est consommé par
    /// injection IS_ depuis les ViewModels, les Menu Handlers et le UseCase
    /// terminal <c>UC_LogAndNotify</c> au titre du pipeline standard de
    /// gestion d'erreurs (§4.7.5 du 0230).
    /// </para>
    ///
    /// Objectif :
    /// <para>
    /// Définir un contrat unique pour l'affichage des notifications utilisateur
    /// à partir de clés dictionnaire, avec propagation explicite de la CallChain
    /// et du <see cref="CancellationToken"/> sur toutes les méthodes publiques
    /// (R-4.5.5, R-4.6.13 du 0231).
    /// </para>
    ///
    /// Rôle (cas Concept) :
    /// <para>
    /// Service technique transverse au sens du tableau de §4.7 du 0230, porteur
    /// d'un concept de présentation transverse (notification utilisateur) non
    /// rattaché à une entité unique. Conforme à la deuxième formulation amendée
    /// de la clause « Rôle » de §4.14.3 amendée pour le cas Concept.
    /// </para>
    ///
    /// Obligations contractuelles :
    /// <list type="bullet">
    /// <item><description>Résider en <c>A_Domain/Interfaces/Services/Presentation/</c>.</description></item>
    /// <item><description>Implémentation par <see cref="DG244Cutting.D_Presentation.Services.SR_Notification"/> active en <c>D_Presentation/Services/</c>.</description></item>
    /// <item><description>Signature de traçabilité sur toutes les méthodes publiques : <c>string caller</c> en tête et <c>CancellationToken ct = default</c> en queue (R-4.5.5).</description></item>
    /// <item><description>Aucune signature transactionnelle exposée (invariant 9 de §2.1).</description></item>
    /// <item><description>Pureté contractuelle : aucune référence à WPF, EF Core ou tout type technique étranger à A_Domain (I-2.4.1, I-2.4.2, IS5).</description></item>
    /// </list>
    ///
    /// Responsabilités :
    /// <list type="bullet">
    /// <item><description>Afficher des messages standards de notification (Information, Stop, Error, Question, Warning, NotValid, Confirmation, Success, ImportantInformation) à partir de clés dictionnaire.</description></item>
    /// <item><description>Afficher une boîte de confirmation et restituer la réponse utilisateur sous forme de <see cref="bool"/> (ConfirmationReturn).</description></item>
    /// <item><description>Piloter l'ouverture et la fermeture d'une fenêtre de dialogue non bloquante via les opérations atomiques d'<see cref="ISE_Window"/>.</description></item>
    /// </list>
    ///
    /// Non-responsabilités :
    /// <list type="bullet">
    /// <item><description>Aucune mutation persistante ni participation à la chaîne (1) d'écriture stricte (Service Presentation hors chaîne (1)).</description></item>
    /// <item><description>Aucune journalisation d'erreur applicative (responsabilité du pipeline terminal du UseCase via <c>UC_LogAndNotify</c>, R-4.7.14 du 0231).</description></item>
    /// <item><description>Aucune décision métier ni orchestration de scénario (I-4.14.6 du 0231).</description></item>
    /// <item><description>Aucune ouverture, validation ou annulation de transaction (I-4.10.1 du 0231).</description></item>
    /// </list>
    /// </summary>
    /// <seealso cref="DG244Cutting.D_Presentation.Services.SR_Notification"/>
    /// <seealso cref="ISE_Window"/>
    /// <seealso cref="IS_Dictionary"/>
    public interface IS_Notification
    {
        #region === Méthodes publiques ===

        /// <summary>
        /// Description :
        /// <para>Affiche un message d'information standard.</para>
        /// </summary>
        /// <remarks>
        /// Contexte :
        /// <para>Méthode publique du service technique transverse de notification (cas Concept,
        /// dérogation typologiquement bornée au préfixe Execute admise au titre de SR20).</para>
        /// Objectif :
        /// <para>Afficher une notification d'information cohérente à l'utilisateur, avec
        /// résolution du titre standard <c>No_Ti_01</c> et du corps via clé dictionnaire.</para>
        /// </remarks>
        /// <param name="caller">CallChain amont propagée par le composant appelant.</param>
        /// <param name="messageKey">Clé dictionnaire du message principal à afficher.</param>
        /// <param name="additionalInfo">Texte complémentaire optionnel déjà résolu.</param>
        /// <param name="ct">Token d'annulation coopérative.</param>
        /// <exception cref="Ex_Business">Levée en cas de précondition structurelle violée (code <c>BU_ER_01</c> — argument null, vide ou whitespace sur <paramref name="caller"/> ou <paramref name="messageKey"/>).</exception>
        /// <exception cref="Ex_Infrastructure">Levée le cas échéant par requalification terminale via <c>IS_ExClassifier</c>.</exception>
        void Information(string caller, string messageKey, string? additionalInfo = null, CancellationToken ct = default);

        /// <summary>
        /// Description :
        /// <para>Affiche un message de type Stop signalant une situation bloquante.</para>
        /// </summary>
        /// <remarks>
        /// Contexte :
        /// <para>Méthode publique du service technique transverse de notification (cas Concept,
        /// dérogation typologiquement bornée au préfixe Execute admise au titre de SR20).</para>
        /// Objectif :
        /// <para>Afficher une notification d'arrêt à l'utilisateur, avec résolution du titre
        /// standard <c>No_Ti_02</c> et du corps via clé dictionnaire.</para>
        /// </remarks>
        /// <param name="caller">CallChain amont propagée par le composant appelant.</param>
        /// <param name="messageKey">Clé dictionnaire du message principal à afficher.</param>
        /// <param name="additionalInfo">Texte complémentaire optionnel déjà résolu.</param>
        /// <param name="ct">Token d'annulation coopérative.</param>
        /// <exception cref="Ex_Business">Levée en cas de précondition structurelle violée (code <c>BU_ER_01</c>).</exception>
        /// <exception cref="Ex_Infrastructure">Levée le cas échéant par requalification terminale via <c>IS_ExClassifier</c>.</exception>
        void Stop(string caller, string messageKey, string? additionalInfo = null, CancellationToken ct = default);

        /// <summary>
        /// Description :
        /// <para>Affiche un message d'erreur critique.</para>
        /// </summary>
        /// <remarks>
        /// Contexte :
        /// <para>Méthode publique du service technique transverse de notification (cas Concept,
        /// dérogation typologiquement bornée au préfixe Execute admise au titre de SR20).</para>
        /// Objectif :
        /// <para>Afficher une notification d'erreur cohérente à l'utilisateur, avec résolution
        /// du titre standard <c>No_Ti_03</c> et du corps via clé dictionnaire.</para>
        /// </remarks>
        /// <param name="caller">CallChain amont propagée par le composant appelant.</param>
        /// <param name="messageKey">Clé dictionnaire du message principal à afficher.</param>
        /// <param name="additionalInfo">Texte complémentaire optionnel déjà résolu.</param>
        /// <param name="ct">Token d'annulation coopérative.</param>
        /// <exception cref="Ex_Business">Levée en cas de précondition structurelle violée (code <c>BU_ER_01</c>).</exception>
        /// <exception cref="Ex_Infrastructure">Levée le cas échéant par requalification terminale via <c>IS_ExClassifier</c>.</exception>
        void Error(string caller, string messageKey, string? additionalInfo = null, CancellationToken ct = default);

        /// <summary>
        /// Description :
        /// <para>Affiche une question à l'utilisateur (boîte Yes/No sans retour).</para>
        /// </summary>
        /// <remarks>
        /// Contexte :
        /// <para>Méthode publique du service technique transverse de notification (cas Concept,
        /// dérogation typologiquement bornée au préfixe Execute admise au titre de SR20).</para>
        /// Objectif :
        /// <para>Présenter une question standard à l'utilisateur, avec résolution du titre
        /// standard <c>No_Ti_04</c> et du corps via clé dictionnaire. Le retour n'est
        /// pas exploité par la signature (cf. <see cref="ConfirmationReturn"/> pour
        /// le retour explicite).</para>
        /// </remarks>
        /// <param name="caller">CallChain amont propagée par le composant appelant.</param>
        /// <param name="messageKey">Clé dictionnaire du message principal à afficher.</param>
        /// <param name="additionalInfo">Texte complémentaire optionnel déjà résolu.</param>
        /// <param name="ct">Token d'annulation coopérative.</param>
        /// <exception cref="Ex_Business">Levée en cas de précondition structurelle violée (code <c>BU_ER_01</c>).</exception>
        /// <exception cref="Ex_Infrastructure">Levée le cas échéant par requalification terminale via <c>IS_ExClassifier</c>.</exception>
        void Question(string caller, string messageKey, string? additionalInfo = null, CancellationToken ct = default);

        /// <summary>
        /// Description :
        /// <para>Affiche un message d'avertissement non bloquant.</para>
        /// </summary>
        /// <remarks>
        /// Contexte :
        /// <para>Méthode publique du service technique transverse de notification (cas Concept,
        /// dérogation typologiquement bornée au préfixe Execute admise au titre de SR20).</para>
        /// Objectif :
        /// <para>Afficher une notification d'avertissement à l'utilisateur, avec résolution
        /// du titre standard <c>No_Ti_05</c> et du corps via clé dictionnaire.</para>
        /// </remarks>
        /// <param name="caller">CallChain amont propagée par le composant appelant.</param>
        /// <param name="messageKey">Clé dictionnaire du message principal à afficher.</param>
        /// <param name="additionalInfo">Texte complémentaire optionnel déjà résolu.</param>
        /// <param name="ct">Token d'annulation coopérative.</param>
        /// <exception cref="Ex_Business">Levée en cas de précondition structurelle violée (code <c>BU_ER_01</c>).</exception>
        /// <exception cref="Ex_Infrastructure">Levée le cas échéant par requalification terminale via <c>IS_ExClassifier</c>.</exception>
        void Warning(string caller, string messageKey, string? additionalInfo = null, CancellationToken ct = default);

        /// <summary>
        /// Description :
        /// <para>Affiche un message de type valeur non valide.</para>
        /// </summary>
        /// <remarks>
        /// Contexte :
        /// <para>Méthode publique du service technique transverse de notification (cas Concept,
        /// dérogation typologiquement bornée au préfixe Execute admise au titre de SR20).</para>
        /// Objectif :
        /// <para>Signaler à l'utilisateur une valeur ou un état non conforme côté présentation,
        /// avec résolution du titre standard <c>No_Ti_06</c> et du corps via clé dictionnaire.</para>
        /// </remarks>
        /// <param name="caller">CallChain amont propagée par le composant appelant.</param>
        /// <param name="messageKey">Clé dictionnaire du message principal à afficher.</param>
        /// <param name="additionalInfo">Texte complémentaire optionnel déjà résolu.</param>
        /// <param name="ct">Token d'annulation coopérative.</param>
        /// <exception cref="Ex_Business">Levée en cas de précondition structurelle violée (code <c>BU_ER_01</c>).</exception>
        /// <exception cref="Ex_Infrastructure">Levée le cas échéant par requalification terminale via <c>IS_ExClassifier</c>.</exception>
        void NotValid(string caller, string messageKey, string? additionalInfo = null, CancellationToken ct = default);

        /// <summary>
        /// Description :
        /// <para>Affiche un message de confirmation (Yes/No sans retour).</para>
        /// </summary>
        /// <remarks>
        /// Contexte :
        /// <para>Méthode publique du service technique transverse de notification (cas Concept,
        /// dérogation typologiquement bornée au préfixe Execute admise au titre de SR20).</para>
        /// Objectif :
        /// <para>Demander une validation explicite à l'utilisateur, avec résolution du titre
        /// standard <c>No_Ti_07</c> et du corps via clé dictionnaire. Le retour n'est pas
        /// exploité par la signature (cf. <see cref="ConfirmationReturn"/>).</para>
        /// </remarks>
        /// <param name="caller">CallChain amont propagée par le composant appelant.</param>
        /// <param name="messageKey">Clé dictionnaire du message principal à afficher.</param>
        /// <param name="additionalInfo">Texte complémentaire optionnel déjà résolu.</param>
        /// <param name="ct">Token d'annulation coopérative.</param>
        /// <exception cref="Ex_Business">Levée en cas de précondition structurelle violée (code <c>BU_ER_01</c>).</exception>
        /// <exception cref="Ex_Infrastructure">Levée le cas échéant par requalification terminale via <c>IS_ExClassifier</c>.</exception>
        void Confirmation(string caller, string messageKey, string? additionalInfo = null, CancellationToken ct = default);

        /// <summary>
        /// Description :
        /// <para>Affiche un message de succès.</para>
        /// </summary>
        /// <remarks>
        /// Contexte :
        /// <para>Méthode publique du service technique transverse de notification (cas Concept,
        /// dérogation typologiquement bornée au préfixe Execute admise au titre de SR20).</para>
        /// Objectif :
        /// <para>Confirmer à l'utilisateur le bon déroulement d'une opération, avec résolution
        /// du titre standard <c>No_Ti_08</c> et du corps via clé dictionnaire.</para>
        /// </remarks>
        /// <param name="caller">CallChain amont propagée par le composant appelant.</param>
        /// <param name="messageKey">Clé dictionnaire du message principal à afficher.</param>
        /// <param name="additionalInfo">Texte complémentaire optionnel déjà résolu.</param>
        /// <param name="ct">Token d'annulation coopérative.</param>
        /// <exception cref="Ex_Business">Levée en cas de précondition structurelle violée (code <c>BU_ER_01</c>).</exception>
        /// <exception cref="Ex_Infrastructure">Levée le cas échéant par requalification terminale via <c>IS_ExClassifier</c>.</exception>
        void Success(string caller, string messageKey, string? additionalInfo = null, CancellationToken ct = default);

        /// <summary>
        /// Description :
        /// <para>Affiche un message d'information importante.</para>
        /// </summary>
        /// <remarks>
        /// Contexte :
        /// <para>Méthode publique du service technique transverse de notification (cas Concept,
        /// dérogation typologiquement bornée au préfixe Execute admise au titre de SR20).</para>
        /// Objectif :
        /// <para>Afficher une information requérant une attention particulière de l'utilisateur,
        /// avec résolution du titre standard <c>No_Ti_09</c> et du corps via clé dictionnaire.</para>
        /// </remarks>
        /// <param name="caller">CallChain amont propagée par le composant appelant.</param>
        /// <param name="messageKey">Clé dictionnaire du message principal à afficher.</param>
        /// <param name="additionalInfo">Texte complémentaire optionnel déjà résolu.</param>
        /// <param name="ct">Token d'annulation coopérative.</param>
        /// <exception cref="Ex_Business">Levée en cas de précondition structurelle violée (code <c>BU_ER_01</c>).</exception>
        /// <exception cref="Ex_Infrastructure">Levée le cas échéant par requalification terminale via <c>IS_ExClassifier</c>.</exception>
        void ImportantInformation(string caller, string messageKey, string? additionalInfo = null, CancellationToken ct = default);

        /// <summary>
        /// Description :
        /// <para>Affiche une boîte de confirmation et retourne la réponse utilisateur sous forme de <see cref="bool"/>.</para>
        /// </summary>
        /// <remarks>
        /// Contexte :
        /// <para>Méthode publique du service technique transverse de notification (cas Concept,
        /// dérogation typologiquement bornée au préfixe Execute admise au titre de SR20).
        /// Retour purifié vers <see cref="bool"/> au titre du refactoring de pureté contractuelle
        /// d'A_Domain (IS5 / I-2.4.1, I-2.4.2). Aucune méthode du contrat n'expose désormais
        /// de type technique WPF en frontière.</para>
        /// Objectif :
        /// <para>Présenter une boîte de dialogue Yes/No et restituer le choix utilisateur,
        /// avec résolution du titre standard <c>No_Ti_07</c> et du corps via clé dictionnaire.
        /// Toute défaillance technique remonte au consommateur amont sous forme d'exception
        /// requalifiée — aucune valeur de repli silencieuse.</para>
        /// </remarks>
        /// <param name="caller">CallChain amont propagée par le composant appelant.</param>
        /// <param name="messageKey">Clé dictionnaire du message principal à afficher.</param>
        /// <param name="additionalInfo">Texte complémentaire optionnel déjà résolu.</param>
        /// <param name="ct">Token d'annulation coopérative.</param>
        /// <returns><see langword="true"/> si l'utilisateur a répondu Yes ; <see langword="false"/> dans tous les autres cas (No, fermeture forcée).</returns>
        /// <exception cref="Ex_Business">Levée en cas de précondition structurelle violée (code <c>BU_ER_01</c>).</exception>
        /// <exception cref="Ex_Infrastructure">Levée le cas échéant par requalification terminale via <c>IS_ExClassifier</c>.</exception>
        bool ConfirmationReturn(string caller, string messageKey, string? additionalInfo = null, CancellationToken ct = default);

        /// <summary>
        /// Description :
        /// <para>Ouvre une fenêtre de dialogue non bloquante centrée sur la fenêtre principale.</para>
        /// </summary>
        /// <remarks>
        /// Contexte :
        /// <para>Méthode publique du service technique transverse de notification (cas Concept,
        /// dérogation typologiquement bornée au préfixe Execute admise au titre de SR20).
        /// La méthode mobilise les opérations atomiques d'<see cref="ISE_Window"/>
        /// (<see cref="ISE_Window.OpenDialog(string, string)"/>) qui sont la seule voie
        /// d'écriture admise sur les propriétés <c>DW_Title</c>, <c>DW_Content</c> et
        /// <c>DW_IsOpen</c> exposées en lecture seule par le contrat.</para>
        /// Objectif :
        /// <para>Afficher une fenêtre secondaire persistante unique sans multiplier les
        /// instances (garde de non-réentrance interne). Le titre et le contenu sont résolus
        /// en interne par le service via <see cref="IS_Dictionary"/> à partir des clés
        /// dictionnaire transmises, en cohérence avec la résolution interne pratiquée par
        /// les autres méthodes publiques de notification du service.</para>
        /// </remarks>
        /// <param name="caller">CallChain amont propagée par le composant appelant.</param>
        /// <param name="titleKey">Clé dictionnaire du titre à afficher.</param>
        /// <param name="contentKey">Clé dictionnaire du contenu à afficher.</param>
        /// <param name="ct">Token d'annulation coopérative.</param>
        /// <exception cref="Ex_Business">Levée en cas de précondition structurelle violée (code <c>BU_ER_01</c> — argument null, vide ou whitespace sur <paramref name="caller"/>, <paramref name="titleKey"/> ou <paramref name="contentKey"/>).</exception>
        /// <exception cref="Ex_Infrastructure">Levée le cas échéant par requalification terminale via <c>IS_ExClassifier</c>.</exception>
        void OpenDialogWindow(string caller, string titleKey, string contentKey, CancellationToken ct = default);

        /// <summary>
        /// Description :
        /// <para>Ferme la fenêtre de dialogue si elle est ouverte et réactive la fenêtre principale.</para>
        /// </summary>
        /// <remarks>
        /// Contexte :
        /// <para>Méthode publique du service technique transverse de notification (cas Concept,
        /// dérogation typologiquement bornée au préfixe Execute admise au titre de SR20).
        /// La méthode mobilise l'opération atomique <see cref="ISE_Window.CloseDialog"/>
        /// qui repositionne <c>DW_Title</c>/<c>DW_Content</c> à chaîne vide et
        /// <c>DW_IsOpen</c> à <see langword="false"/>.</para>
        /// Objectif :
        /// <para>Rétablir l'état standard de l'interface après affichage d'une fenêtre
        /// de dialogue non bloquante : fermeture de la fenêtre WPF si visible,
        /// réactivation inconditionnelle de la fenêtre principale, réinitialisation
        /// atomique de l'état dialogue d'<see cref="ISE_Window"/>.</para>
        /// </remarks>
        /// <param name="caller">CallChain amont propagée par le composant appelant.</param>
        /// <param name="ct">Token d'annulation coopérative.</param>
        /// <exception cref="Ex_Business">Levée en cas de précondition structurelle violée (code <c>BU_ER_01</c> — argument null, vide ou whitespace sur <paramref name="caller"/>).</exception>
        /// <exception cref="Ex_Infrastructure">Levée le cas échéant par requalification terminale via <c>IS_ExClassifier</c>.</exception>
        void CloseDialogWindow(string caller, CancellationToken ct = default);

        #endregion
    }
}