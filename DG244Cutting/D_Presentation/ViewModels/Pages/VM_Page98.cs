using DG244Cutting.A_Domain.Interfaces.Services.Presentation;
using DG244Cutting.A_Domain.Interfaces.Settings.App;
using DG244Cutting.A_Domain.Interfaces.UseCases.App;
using DG244Cutting.D_Presentation.ViewModels.Generic;

namespace DG244Cutting.D_Presentation.ViewModels.Pages
{
    /// <summary>
    /// ViewModel de la page de présentation générale <c>Page98</c> de
    /// l'application DG244Cutting, exposant à la vue les libellés
    /// multilingues de présentation de l'application et le numéro de
    /// version courant lu via le UseCase <see cref="IU_GetApplicationVersion"/>.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Composant de la famille VM_Page de la couche
    /// <c>D_Presentation</c>, ViewModel concret de la page de
    /// présentation
    /// <see cref="DG244Cutting.D_Presentation.Views.Pages.Page98"/>. La
    /// page est consultée librement par tout utilisateur de
    /// l'application et n'expose aucune commande utilisateur ; elle
    /// présente le contexte fonctionnel, l'architecture logicielle et
    /// les principes de développement de l'application, ainsi que son
    /// numéro de version courant. La sortie s'effectue exclusivement
    /// via les boutons transverses du menu horizontal portés par le
    /// couple <c>VM_MH_Generic</c>/<c>MH_Generic</c>.</para>
    ///
    /// <para>Objectif : Exposer à la vue
    /// <see cref="DG244Cutting.D_Presentation.Views.Pages.Page98"/> :</para>
    /// <list type="bullet">
    ///   <item><description>55 propriétés observables
    ///   <c>Label_P98_01</c> à <c>Label_P98_55</c> liées aux clés
    ///   homonymes du dictionnaire actif, alimentées par la mécanique
    ///   multilingue factorisée par <see cref="VM_Generic"/> :
    ///   premier chargement au constructeur via
    ///   <see cref="VM_Generic.InitializeLabels"/>, rechargement
    ///   automatique à tout changement de langue dynamique par le
    ///   handler interne d'abonnement INPC à
    ///   <see cref="ISE_App.AppCultureCode"/> de l'ancêtre commun,
    ///   conformément à §4.11.5 du 0230 et à R-4.11.9 du 0231.</description></item>
    ///   <item><description>La propriété observable
    ///   <see cref="VersionNumber"/>, donnée fonctionnelle alimentée
    ///   par invocation du UseCase
    ///   <see cref="IU_GetApplicationVersion"/> via le composant
    ///   <see cref="IS_UseCaseInvoker"/> (EA-11), au moment de l'appel
    ///   asynchrone <see cref="LoadAsync"/> déclenché par le
    ///   code-behind de <c>Page98</c> au point d'extension
    ///   <c>OnLoadedAsync</c> exposé par <c>Page_Generic</c> (§4.15.7
    ///   du 0230).</description></item>
    /// </list>
    ///
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Exposer les 55 propriétés observables
    ///   <c>Label_P98_01</c> à <c>Label_P98_55</c> et la propriété
    ///   observable <see cref="VersionNumber"/> en accès public en
    ///   lecture, écriture privée via le helper hérité
    ///   <c>SetProperty&lt;T&gt;</c>.</description></item>
    ///   <item><description>Redéfinir
    ///   <see cref="VM_Generic.LoadLabels"/> pour résoudre les 55 clés
    ///   <c>P98_01</c> à <c>P98_55</c> via
    ///   <see cref="VM_Generic._dictionary"/> hérité et affecter les
    ///   valeurs résolues aux 55 propriétés <c>Label_P98_NN</c>,
    ///   conformément à R-4.11.8 du 0231.</description></item>
    ///   <item><description>Redéfinir
    ///   <see cref="VM_Page_Generic.LoadAsync"/> pour charger la
    ///   donnée fonctionnelle <see cref="VersionNumber"/> par
    ///   invocation du UseCase
    ///   <see cref="IU_GetApplicationVersion"/> via
    ///   <see cref="IS_UseCaseInvoker"/>, en encapsulation par le
    ///   filet hérité <see cref="VM_Generic.ExecuteSafeAsync"/>
    ///   (§4.7.3 du 0230). Le hook est invoqué depuis le code-behind
    ///   de <c>Page98</c> au point d'extension <c>OnLoadedAsync</c>
    ///   exposé par <c>Page_Generic</c>.</description></item>
    ///   <item><description>Déléguer à <see cref="VM_Generic"/> la
    ///   cérémonie multilingue complète (premier chargement,
    ///   abonnement INPC filtré sur
    ///   <see cref="ISE_App.AppCultureCode"/>, marshalling Dispatcher
    ///   défensif, rechargement) par l'unique appel à
    ///   <see cref="VM_Generic.InitializeLabels"/> en dernière
    ///   instruction du constructeur, conformément à I-4.11.11 et
    ///   R-4.11.8 du 0231.</description></item>
    /// </list>
    ///
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Aucune logique métier ni règle
    ///   décisionnelle. La page est un rendu visuel statique de
    ///   présentation, sans calcul, sans accès direct aux données et
    ///   sans appel à un Service métier.</description></item>
    ///   <item><description>Aucune décision de navigation : la règle
    ///   R-4.12.2 du 0231 réserve la décision de navigation aux
    ///   UseCases. <see cref="VM_Page98"/> n'injecte ni
    ///   <c>IU_Navigation</c> ni <c>IS_Navigation</c>. La sortie de la
    ///   page est portée par les commandes transverses du menu
    ///   horizontal (Home, Previous), hors périmètre du présent
    ///   ViewModel.</description></item>
    ///   <item><description>Aucune commande utilisateur : la page est
    ///   non interactive ; aucun <c>ICommand</c> n'est exposé.</description></item>
    ///   <item><description>Aucun désabonnement explicite ni aucune
    ///   cérémonie multilingue locale : l'abonnement INPC à
    ///   <see cref="ISE_App"/> est désormais branché par
    ///   <see cref="VM_Generic.InitializeLabels"/> et porté par le
    ///   handler interne de l'ancêtre commun, conformément à I-4.11.11
    ///   du 0231 ; aucun désabonnement n'est requis du dérivé. La
    ///   P4-bis (§4.10.10 du 0230) garantit par ailleurs la
    ///   libération naturelle de l'abonnement à l'arrêt de
    ///   l'application.</description></item>
    ///   <item><description>Aucun champ propre ni handler propre lié à
    ///   <see cref="ISE_App"/> : l'encapsulation de la dépendance est
    ///   intégralement portée par <see cref="VM_Generic"/> en champ
    ///   <c>private</c> non hérité (I-4.11.11 du 0231) ; le présent
    ///   dérivé n'accède jamais directement à
    ///   <see cref="ISE_App"/>.</description></item>
    ///   <item><description>Aucune logique locale de fallback en cas
    ///   de clé absente du dictionnaire ni try/catch local dans
    ///   <see cref="LoadLabels"/> : la logique de repli est portée
    ///   exclusivement par <c>SR_Dictionary</c> conformément à
    ///   R-4.11.6 et R-4.11.10 du 0231.</description></item>
    /// </list>
    ///
    /// <para>Note sur les exceptions architecturales : Aucune exception
    /// architecturale propre n'est portée par <see cref="VM_Page98"/>.
    /// L'injection de <see cref="ISE_App"/> au constructeur de la base
    /// relève exclusivement de la mécanique multilingue factorisée par
    /// <see cref="VM_Generic"/> (§4.15.5 du 0230, R-4.11.9 du 0231)
    /// et n'est pas une dérogation propre au présent dérivé : aucune
    /// cérémonie multilingue locale n'est portée par
    /// <see cref="VM_Page98"/>, conformément à I-4.11.11 du 0231.
    /// L'injection directe de <see cref="IU_LogAndNotify"/> par le
    /// ViewModel relève quant à elle de l'exception architecturale
    /// propre du socle <see cref="VM_Generic"/> (EA-01, §4.15.5 du
    /// 0230), héritée et non re-déclarée à ce niveau ; elle est
    /// mobilisée par <see cref="LoadAsync"/> qui encapsule son
    /// invocation par le filet hérité
    /// <see cref="VM_Generic.ExecuteSafeAsync"/>. L'injection de
    /// <see cref="IS_UseCaseInvoker"/> par le constructeur est nominale
    /// au titre du mode d'invocation depuis <c>D_Presentation</c> posé
    /// en §4.10.10 du 0230 : les ViewModels invoquent les contrats
    /// <c>IU_</c> et <c>IQ_</c> via <see cref="IS_UseCaseInvoker"/> qui
    /// matérialise un <c>IServiceScope</c> distinct à chaque
    /// invocation. EA-11 (« Composant créateur de scope DI par
    /// invocation », §4.10.10 et §4.15.10 du 0230, §17.4 du 0231) est
    /// portée exclusivement par <c>SR_UseCaseInvoker</c> ; elle est
    /// consommée nominalement par le présent ViewModel et non
    /// re-déclarée à son niveau.</para>
    ///
    /// <para>Statut de premier exemple canonique de second rang :</para>
    ///
    /// <para>Le présent ViewModel constitue le premier exemple
    /// canonique de second rang de la famille VM_Page, en
    /// complémentarité avec <c>VM_Page99</c> qui constitue le premier
    /// exemple canonique de premier rang. Là où <c>VM_Page99</c>
    /// illustre le cas minimal d'un ViewModel de page purement statique
    /// sans donnée métier à charger (override exclusif de
    /// <see cref="LoadLabels"/>, aucun override de
    /// <see cref="LoadAsync"/>), <see cref="VM_Page98"/> illustre le
    /// cas riche d'un ViewModel de page combinant le chargement
    /// synchrone des libellés multilingues au constructeur via
    /// <see cref="VM_Generic.InitializeLabels"/> et le chargement
    /// asynchrone d'une donnée métier au <c>Loaded</c> de la page via
    /// override de <see cref="LoadAsync"/>, consommant un UseCase au
    /// titre d'EA-11 par <see cref="IS_UseCaseInvoker"/>. Les deux
    /// composants forment ainsi un couple canonique complémentaire qui
    /// couvre la totalité du spectre d'usage du socle
    /// <see cref="VM_Page_Generic"/>, et constituent ensemble la
    /// matière première de la future §5.12 du 0230 et du futur
    /// 0232_VI_VM, à inscrire dans un fil de maintenance documentaire
    /// ultérieur.</para>
    ///
    /// <para>Structure des régions :</para>
    ///
    /// <para>La classe applique la structure normative à cinq régions
    /// standard (§4.4.2) complétée par deux extensions (§4.4.3) : au
    /// titre de §4.4.3 du 0230 l'extension Propriétés publiques pour
    /// les 55 propriétés <c>Label_P98_NN</c> et la propriété
    /// <see cref="VersionNumber"/>, et au titre de R-4.4.10 du 0231
    /// l'extension Méthodes protégées pour l'override
    /// <see cref="LoadLabels"/>. Soit sept régions au total :</para>
    ///
    /// <list type="number">
    ///   <item><description><c>=== Propriétés privées ===</c> : 55
    ///   champs <c>_label_p98_NN</c> et champ
    ///   <c>_versionNumber</c>.</description></item>
    ///   <item><description><c>=== Dépendances privées ===</c> : champ
    ///   <c>_useCaseInvoker</c>.</description></item>
    ///   <item><description><c>=== Propriétés publiques ===</c> : 55
    ///   propriétés <c>Label_P98_NN</c> et propriété
    ///   <see cref="VersionNumber"/>.</description></item>
    ///   <item><description><c>=== Constructeur ===</c> : constructeur
    ///   <c>public</c> à quatre paramètres, délégation à
    ///   <see cref="VM_Page_Generic"/> via <c>base(...)</c> et
    ///   invocation finale de
    ///   <see cref="VM_Generic.InitializeLabels"/>.</description></item>
    ///   <item><description><c>=== Méthodes publiques ===</c> :
    ///   override <see cref="LoadAsync"/>.</description></item>
    ///   <item><description><c>=== Méthodes protégées ===</c> :
    ///   override <see cref="LoadLabels"/>.</description></item>
    ///   <item><description><c>=== Méthodes privées ===</c> : présente
    ///   mais vide, marqueur <c>// A compléter</c>.</description></item>
    /// </list>
    ///
    /// <para>L'extension <c>=== Événements / Délégués / Indexeurs ===</c>
    /// n'est pas présente : <see cref="VM_Page98"/> n'expose aucun
    /// événement propre, l'événement <c>PropertyChanged</c> étant porté
    /// par <see cref="VM_Generic"/> au titre d'INPC et hérité par
    /// transitivité.</para>
    /// </remarks>
    public class VM_Page98 : VM_Page_Generic
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Champ support de la propriété observable <see cref="Label_P98_01"/>.
        /// Initialisé à <see cref="string.Empty"/> ; écrasé au constructeur
        /// par le premier appel à <see cref="LoadLabels"/> orchestré par
        /// <see cref="VM_Generic.InitializeLabels"/> avec la valeur localisée
        /// de la clé <c>P98_01</c>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : L'initialisation à <see cref="string.Empty"/>
        /// garantit que la propriété est dans un état défini avant le
        /// premier binding WPF, même dans l'hypothèse théorique où la
        /// résolution de la clé <c>P98_01</c> retournerait la valeur de
        /// repli <c>[P98_01] not found</c> de <c>SR_Dictionary</c>. Cette
        /// posture vaut pour les 55 champs <c>_label_p98_NN</c> du présent
        /// ViewModel ; elle n'est documentée nominativement que sur le
        /// premier d'entre eux pour fixer la convention.</para>
        /// </remarks>
        private string _label_p98_01 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_02"/> (clé <c>P98_02</c>).</summary>
        private string _label_p98_02 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_03"/> (clé <c>P98_03</c>).</summary>
        private string _label_p98_03 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_04"/> (clé <c>P98_04</c>).</summary>
        private string _label_p98_04 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_05"/> (clé <c>P98_05</c>).</summary>
        private string _label_p98_05 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_06"/> (clé <c>P98_06</c>).</summary>
        private string _label_p98_06 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_07"/> (clé <c>P98_07</c>).</summary>
        private string _label_p98_07 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_08"/> (clé <c>P98_08</c>).</summary>
        private string _label_p98_08 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_09"/> (clé <c>P98_09</c>).</summary>
        private string _label_p98_09 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_10"/> (clé <c>P98_10</c>).</summary>
        private string _label_p98_10 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_11"/> (clé <c>P98_11</c>).</summary>
        private string _label_p98_11 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_12"/> (clé <c>P98_12</c>).</summary>
        private string _label_p98_12 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_13"/> (clé <c>P98_13</c>).</summary>
        private string _label_p98_13 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_14"/> (clé <c>P98_14</c>).</summary>
        private string _label_p98_14 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_15"/> (clé <c>P98_15</c>).</summary>
        private string _label_p98_15 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_16"/> (clé <c>P98_16</c>).</summary>
        private string _label_p98_16 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_17"/> (clé <c>P98_17</c>).</summary>
        private string _label_p98_17 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_18"/> (clé <c>P98_18</c>).</summary>
        private string _label_p98_18 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_19"/> (clé <c>P98_19</c>).</summary>
        private string _label_p98_19 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_20"/> (clé <c>P98_20</c>).</summary>
        private string _label_p98_20 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_21"/> (clé <c>P98_21</c>).</summary>
        private string _label_p98_21 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_22"/> (clé <c>P98_22</c>).</summary>
        private string _label_p98_22 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_23"/> (clé <c>P98_23</c>).</summary>
        private string _label_p98_23 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_24"/> (clé <c>P98_24</c>).</summary>
        private string _label_p98_24 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_25"/> (clé <c>P98_25</c>).</summary>
        private string _label_p98_25 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_26"/> (clé <c>P98_26</c>).</summary>
        private string _label_p98_26 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_27"/> (clé <c>P98_27</c>).</summary>
        private string _label_p98_27 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_28"/> (clé <c>P98_28</c>).</summary>
        private string _label_p98_28 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_29"/> (clé <c>P98_29</c>).</summary>
        private string _label_p98_29 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_30"/> (clé <c>P98_30</c>).</summary>
        private string _label_p98_30 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_31"/> (clé <c>P98_31</c>).</summary>
        private string _label_p98_31 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_32"/> (clé <c>P98_32</c>).</summary>
        private string _label_p98_32 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_33"/> (clé <c>P98_33</c>).</summary>
        private string _label_p98_33 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_34"/> (clé <c>P98_34</c>).</summary>
        private string _label_p98_34 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_35"/> (clé <c>P98_35</c>).</summary>
        private string _label_p98_35 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_36"/> (clé <c>P98_36</c>).</summary>
        private string _label_p98_36 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_37"/> (clé <c>P98_37</c>).</summary>
        private string _label_p98_37 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_38"/> (clé <c>P98_38</c>).</summary>
        private string _label_p98_38 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_39"/> (clé <c>P98_39</c>).</summary>
        private string _label_p98_39 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_40"/> (clé <c>P98_40</c>).</summary>
        private string _label_p98_40 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_41"/> (clé <c>P98_41</c>).</summary>
        private string _label_p98_41 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_42"/> (clé <c>P98_42</c>).</summary>
        private string _label_p98_42 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_43"/> (clé <c>P98_43</c>).</summary>
        private string _label_p98_43 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_44"/> (clé <c>P98_44</c>).</summary>
        private string _label_p98_44 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_45"/> (clé <c>P98_45</c>).</summary>
        private string _label_p98_45 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_46"/> (clé <c>P98_46</c>).</summary>
        private string _label_p98_46 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_47"/> (clé <c>P98_47</c>).</summary>
        private string _label_p98_47 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_48"/> (clé <c>P98_48</c>).</summary>
        private string _label_p98_48 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_49"/> (clé <c>P98_49</c>).</summary>
        private string _label_p98_49 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_50"/> (clé <c>P98_50</c>).</summary>
        private string _label_p98_50 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_51"/> (clé <c>P98_51</c>).</summary>
        private string _label_p98_51 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_52"/> (clé <c>P98_52</c>).</summary>
        private string _label_p98_52 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_53"/> (clé <c>P98_53</c>).</summary>
        private string _label_p98_53 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_54"/> (clé <c>P98_54</c>).</summary>
        private string _label_p98_54 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P98_55"/> (clé <c>P98_55</c>).</summary>
        private string _label_p98_55 = string.Empty;

        /// <summary>
        /// Champ support de la propriété observable <see cref="VersionNumber"/>.
        /// Initialisé à <see cref="string.Empty"/> ; alimenté par
        /// <see cref="LoadAsync"/> à chaque entrée sur la page via
        /// l'invocation du UseCase <see cref="IU_GetApplicationVersion"/>
        /// par <see cref="IS_UseCaseInvoker"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : L'initialisation à <see cref="string.Empty"/>
        /// garantit que la propriété est dans un état défini avant le
        /// premier binding WPF. En cas d'exception applicative typée
        /// capturée par le pipeline du UseCase amont, ce dernier retourne
        /// <see cref="string.Empty"/> ; la propriété est alors alimentée
        /// par cette valeur de repli et conserve un état observable
        /// cohérent.</para>
        /// </remarks>
        private string _versionNumber = string.Empty;

        #endregion

        #region === Dépendances privées ===

        /// <summary>
        /// Composant Singleton porteur de l'exception architecturale EA-11
        /// (« Composant créateur de scope DI par invocation », §4.10.10 et
        /// §4.15.10 du 0230, §17.4 du 0231), unique voie d'invocation des
        /// UseCases (<c>IU_</c>) et Query Handlers (<c>IQ_</c>) depuis un
        /// composant de <c>D_Presentation</c>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Injecté en Singleton par le conteneur DI au
        /// constructeur, conformément au mode d'invocation depuis
        /// <c>D_Presentation</c> posé en §4.10.10 du 0230. À chaque
        /// invocation, <see cref="IS_UseCaseInvoker"/> matérialise un
        /// <c>IServiceScope</c> distinct, y résout le composant cible et
        /// l'exécute via le délégué fourni, puis dispose le scope. Le
        /// présent ViewModel est consommateur de
        /// <see cref="IS_UseCaseInvoker"/> et non porteur d'EA-11 : EA-11
        /// est portée exclusivement par <c>SR_UseCaseInvoker</c>.</para>
        /// <para>Mode d'invocation strict : Le passage par
        /// <see cref="IS_UseCaseInvoker"/> est imposé par la lecture
        /// stricte du §4.10.10 du 0230 qui pose l'interdiction
        /// structurelle de l'injection directe d'un contrat <c>IU_</c>
        /// dans un composant de <c>D_Presentation</c>, indépendamment de
        /// toute question de captive dependency. Conformité I-4.10.10 du
        /// 0231.</para>
        /// </remarks>
        private readonly IS_UseCaseInvoker _useCaseInvoker;

        #endregion

        #region === Propriétés publiques ===

        /// <summary>
        /// Obtient le libellé multilingue associé à la clé <c>P98_01</c>
        /// du dictionnaire de langue actif (« Version de l'application : »
        /// en français).
        /// </summary>
        /// <value>
        /// Chaîne localisée résolue à partir du dictionnaire de langue
        /// actif. En cas de clé absente, <c>SR_Dictionary</c> retourne la
        /// valeur de repli <c>[P98_01] not found</c> conformément à
        /// R-4.11.10 du 0231.
        /// </value>
        /// <remarks>
        /// <para>Contexte : Propriété bindable consommée par la vue
        /// <see cref="DG244Cutting.D_Presentation.Views.Pages.Page98"/>.
        /// L'accesseur en écriture est privé : la valeur ne peut être
        /// modifiée qu'à travers l'override <see cref="LoadLabels"/>,
        /// invoqué initialement par
        /// <see cref="VM_Generic.InitializeLabels"/> au constructeur,
        /// puis par le handler interne d'abonnement INPC de
        /// <see cref="VM_Generic"/> à chaque changement de langue
        /// dynamique notifié par
        /// <see cref="ISE_App.AppCultureCode"/>.</para>
        /// <para>Convention de documentation : Le présent <c>summary</c>
        /// fixe la convention de documentation pour les 55 propriétés
        /// <c>Label_P98_NN</c> du ViewModel. Les 54 autres propriétés
        /// portent un <c>summary</c> condensé d'une ligne, par
        /// homogénéité du patron et pour limiter la prolifération
        /// documentaire sans perdre la traçabilité de la clé
        /// associée.</para>
        /// </remarks>
        public string Label_P98_01
        {
            get => _label_p98_01;
            private set => SetProperty(ref _label_p98_01, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_02</c>.</summary>
        public string Label_P98_02
        {
            get => _label_p98_02;
            private set => SetProperty(ref _label_p98_02, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_03</c>.</summary>
        public string Label_P98_03
        {
            get => _label_p98_03;
            private set => SetProperty(ref _label_p98_03, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_04</c>.</summary>
        public string Label_P98_04
        {
            get => _label_p98_04;
            private set => SetProperty(ref _label_p98_04, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_05</c>.</summary>
        public string Label_P98_05
        {
            get => _label_p98_05;
            private set => SetProperty(ref _label_p98_05, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_06</c>.</summary>
        public string Label_P98_06
        {
            get => _label_p98_06;
            private set => SetProperty(ref _label_p98_06, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_07</c>.</summary>
        public string Label_P98_07
        {
            get => _label_p98_07;
            private set => SetProperty(ref _label_p98_07, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_08</c>.</summary>
        public string Label_P98_08
        {
            get => _label_p98_08;
            private set => SetProperty(ref _label_p98_08, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_09</c>.</summary>
        public string Label_P98_09
        {
            get => _label_p98_09;
            private set => SetProperty(ref _label_p98_09, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_10</c>.</summary>
        public string Label_P98_10
        {
            get => _label_p98_10;
            private set => SetProperty(ref _label_p98_10, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_11</c>.</summary>
        public string Label_P98_11
        {
            get => _label_p98_11;
            private set => SetProperty(ref _label_p98_11, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_12</c>.</summary>
        public string Label_P98_12
        {
            get => _label_p98_12;
            private set => SetProperty(ref _label_p98_12, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_13</c>.</summary>
        public string Label_P98_13
        {
            get => _label_p98_13;
            private set => SetProperty(ref _label_p98_13, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_14</c>.</summary>
        public string Label_P98_14
        {
            get => _label_p98_14;
            private set => SetProperty(ref _label_p98_14, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_15</c>.</summary>
        public string Label_P98_15
        {
            get => _label_p98_15;
            private set => SetProperty(ref _label_p98_15, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_16</c>.</summary>
        public string Label_P98_16
        {
            get => _label_p98_16;
            private set => SetProperty(ref _label_p98_16, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_17</c>.</summary>
        public string Label_P98_17
        {
            get => _label_p98_17;
            private set => SetProperty(ref _label_p98_17, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_18</c>.</summary>
        public string Label_P98_18
        {
            get => _label_p98_18;
            private set => SetProperty(ref _label_p98_18, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_19</c>.</summary>
        public string Label_P98_19
        {
            get => _label_p98_19;
            private set => SetProperty(ref _label_p98_19, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_20</c>.</summary>
        public string Label_P98_20
        {
            get => _label_p98_20;
            private set => SetProperty(ref _label_p98_20, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_21</c>.</summary>
        public string Label_P98_21
        {
            get => _label_p98_21;
            private set => SetProperty(ref _label_p98_21, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_22</c>.</summary>
        public string Label_P98_22
        {
            get => _label_p98_22;
            private set => SetProperty(ref _label_p98_22, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_23</c>.</summary>
        public string Label_P98_23
        {
            get => _label_p98_23;
            private set => SetProperty(ref _label_p98_23, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_24</c>.</summary>
        public string Label_P98_24
        {
            get => _label_p98_24;
            private set => SetProperty(ref _label_p98_24, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_25</c>.</summary>
        public string Label_P98_25
        {
            get => _label_p98_25;
            private set => SetProperty(ref _label_p98_25, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_26</c>.</summary>
        public string Label_P98_26
        {
            get => _label_p98_26;
            private set => SetProperty(ref _label_p98_26, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_27</c>.</summary>
        public string Label_P98_27
        {
            get => _label_p98_27;
            private set => SetProperty(ref _label_p98_27, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_28</c>.</summary>
        public string Label_P98_28
        {
            get => _label_p98_28;
            private set => SetProperty(ref _label_p98_28, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_29</c>.</summary>
        public string Label_P98_29
        {
            get => _label_p98_29;
            private set => SetProperty(ref _label_p98_29, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_30</c>.</summary>
        public string Label_P98_30
        {
            get => _label_p98_30;
            private set => SetProperty(ref _label_p98_30, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_31</c>.</summary>
        public string Label_P98_31
        {
            get => _label_p98_31;
            private set => SetProperty(ref _label_p98_31, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_32</c>.</summary>
        public string Label_P98_32
        {
            get => _label_p98_32;
            private set => SetProperty(ref _label_p98_32, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_33</c>.</summary>
        public string Label_P98_33
        {
            get => _label_p98_33;
            private set => SetProperty(ref _label_p98_33, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_34</c>.</summary>
        public string Label_P98_34
        {
            get => _label_p98_34;
            private set => SetProperty(ref _label_p98_34, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_35</c>.</summary>
        public string Label_P98_35
        {
            get => _label_p98_35;
            private set => SetProperty(ref _label_p98_35, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_36</c>.</summary>
        public string Label_P98_36
        {
            get => _label_p98_36;
            private set => SetProperty(ref _label_p98_36, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_37</c>.</summary>
        public string Label_P98_37
        {
            get => _label_p98_37;
            private set => SetProperty(ref _label_p98_37, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_38</c>.</summary>
        public string Label_P98_38
        {
            get => _label_p98_38;
            private set => SetProperty(ref _label_p98_38, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_39</c>.</summary>
        public string Label_P98_39
        {
            get => _label_p98_39;
            private set => SetProperty(ref _label_p98_39, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_40</c>.</summary>
        public string Label_P98_40
        {
            get => _label_p98_40;
            private set => SetProperty(ref _label_p98_40, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_41</c>.</summary>
        public string Label_P98_41
        {
            get => _label_p98_41;
            private set => SetProperty(ref _label_p98_41, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_42</c>.</summary>
        public string Label_P98_42
        {
            get => _label_p98_42;
            private set => SetProperty(ref _label_p98_42, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_43</c>.</summary>
        public string Label_P98_43
        {
            get => _label_p98_43;
            private set => SetProperty(ref _label_p98_43, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_44</c>.</summary>
        public string Label_P98_44
        {
            get => _label_p98_44;
            private set => SetProperty(ref _label_p98_44, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_45</c>.</summary>
        public string Label_P98_45
        {
            get => _label_p98_45;
            private set => SetProperty(ref _label_p98_45, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_46</c>.</summary>
        public string Label_P98_46
        {
            get => _label_p98_46;
            private set => SetProperty(ref _label_p98_46, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_47</c>.</summary>
        public string Label_P98_47
        {
            get => _label_p98_47;
            private set => SetProperty(ref _label_p98_47, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_48</c>.</summary>
        public string Label_P98_48
        {
            get => _label_p98_48;
            private set => SetProperty(ref _label_p98_48, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_49</c>.</summary>
        public string Label_P98_49
        {
            get => _label_p98_49;
            private set => SetProperty(ref _label_p98_49, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_50</c>.</summary>
        public string Label_P98_50
        {
            get => _label_p98_50;
            private set => SetProperty(ref _label_p98_50, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_51</c>.</summary>
        public string Label_P98_51
        {
            get => _label_p98_51;
            private set => SetProperty(ref _label_p98_51, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_52</c>.</summary>
        public string Label_P98_52
        {
            get => _label_p98_52;
            private set => SetProperty(ref _label_p98_52, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_53</c>.</summary>
        public string Label_P98_53
        {
            get => _label_p98_53;
            private set => SetProperty(ref _label_p98_53, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_54</c>.</summary>
        public string Label_P98_54
        {
            get => _label_p98_54;
            private set => SetProperty(ref _label_p98_54, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P98_55</c>.</summary>
        public string Label_P98_55
        {
            get => _label_p98_55;
            private set => SetProperty(ref _label_p98_55, value);
        }

        /// <summary>
        /// Obtient le numéro de version courant de l'application
        /// DG244Cutting, alimenté par invocation du UseCase
        /// <see cref="IU_GetApplicationVersion"/> via
        /// <see cref="IS_UseCaseInvoker"/> au moment de l'appel à
        /// <see cref="LoadAsync"/>.
        /// </summary>
        /// <value>
        /// Chaîne représentant la version courante telle que retournée
        /// par <c>Version.ToString()</c> sur l'assembly d'exécution
        /// (typiquement de la forme <c>major.minor.build.revision</c>),
        /// ou <see cref="string.Empty"/> en cas d'absence de propriété
        /// <c>Version</c> sur l'assembly ou d'exception applicative
        /// typée capturée par le pipeline d'erreur du UseCase consommé.
        /// </value>
        /// <remarks>
        /// <para>Contexte : Propriété bindable consommée par la vue
        /// <see cref="DG244Cutting.D_Presentation.Views.Pages.Page98"/>.
        /// L'accesseur en écriture est privé : la valeur ne peut être
        /// modifiée qu'à travers l'override <see cref="LoadAsync"/>,
        /// invoqué par le code-behind de <c>Page98</c> au point
        /// d'extension <c>OnLoadedAsync</c> exposé par
        /// <c>Page_Generic</c> (§4.15.7 du 0230).</para>
        /// <para>Origine de la valeur : Le UseCase
        /// <see cref="IU_GetApplicationVersion"/> lit la propriété
        /// <c>Version</c> de l'assembly d'exécution via
        /// <c>System.Reflection</c>. Cette propriété n'est pas affectée
        /// par <see cref="LoadLabels"/> (le numéro de version n'est pas
        /// un libellé multilingue) ; elle n'est pas non plus rechargée
        /// par le handler interne d'abonnement INPC de
        /// <see cref="VM_Generic"/> (le numéro de version ne dépend pas
        /// de la langue active).</para>
        /// </remarks>
        public string VersionNumber
        {
            get => _versionNumber;
            private set => SetProperty(ref _versionNumber, value);
        }

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="VM_Page98"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Constructeur invoqué par le conteneur DI
        /// lors de la résolution du Singleton <see cref="VM_Page98"/>
        /// par la vue <c>Page98</c> via
        /// <c>App.ServiceProvider.GetRequiredService</c> dans son propre
        /// constructeur (EA-02 Service Locator de
        /// <see cref="DG244Cutting.D_Presentation.Views.Generic.Page_Generic"/>,
        /// étendue aux dérivés directs pour la résolution de leur
        /// ViewModel — cf. §4.15.7 du 0230).</para>
        ///
        /// <para>Séquence d'initialisation :</para>
        /// <list type="number">
        ///   <item><description>Délégation à
        ///   <see cref="VM_Page_Generic"/> via
        ///   <c>base(dictionary, logAndNotify, app)</c> en première
        ///   instruction. La chaîne <c>base(...)</c> remonte à
        ///   <see cref="VM_Generic"/> qui applique les trois gardes
        ///   <see cref="ArgumentNullException"/> sur les trois
        ///   paramètres, stocke <paramref name="dictionary"/> et
        ///   <paramref name="logAndNotify"/> en champs
        ///   <c>protected</c> (<see cref="VM_Generic._dictionary"/>,
        ///   <see cref="VM_Generic._logAndNotify"/>) accessibles aux
        ///   dérivés, stocke <paramref name="app"/> en champ
        ///   <c>private</c> non hérité (encapsulation de la mécanique
        ///   multilingue, conformément à I-4.11.11 du 0231), et
        ///   initialise le champ <c>_callee</c> via
        ///   <c>GetType().Name</c>.</description></item>
        ///   <item><description>Garde
        ///   <see cref="ArgumentNullException"/> sur l'unique
        ///   dépendance propre au dérivé
        ///   (<paramref name="useCaseInvoker"/>) et affectation au
        ///   champ <c>_useCaseInvoker</c>.</description></item>
        ///   <item><description>Appel à
        ///   <see cref="VM_Generic.InitializeLabels"/> en dernière
        ///   instruction du corps. Ce hook explicite orchestre la
        ///   séquence normative en trois temps : construction d'une
        ///   CallChain initiale via
        ///   <see cref="VM_Generic.BuildFirstCallChain"/>, premier
        ///   appel synchrone à l'override <see cref="LoadLabels"/>
        ///   peuplant les 55 propriétés <c>Label_P98_NN</c> avant le
        ///   premier binding WPF de la vue, et branchement de
        ///   l'abonnement INPC interne à <see cref="ISE_App"/> pour la
        ///   prise en compte du changement de langue dynamique
        ///   (R-4.11.8 et R-4.11.9 du 0231).</description></item>
        /// </list>
        ///
        /// <para>Règle d'invocation d'<c>InitializeLabels</c>
        /// (R-4.11.8 du 0231) : L'appel à
        /// <see cref="VM_Generic.InitializeLabels"/> est exclusivement
        /// effectué dans le constructeur du ViewModel dérivé concret
        /// final, en dernière instruction, après l'affectation de
        /// toutes les dépendances propres. Cette règle prévient
        /// l'écueil classique de l'invocation virtuelle dans le
        /// constructeur d'une classe de base avec dépendances dérivées
        /// non encore initialisées.</para>
        ///
        /// <para>La donnée fonctionnelle <see cref="VersionNumber"/>
        /// n'est pas chargée par le constructeur ; son chargement est
        /// porté par l'override <see cref="LoadAsync"/>, invoqué par
        /// le code-behind de <c>Page98</c> au point d'extension
        /// <c>OnLoadedAsync</c> exposé par <c>Page_Generic</c>,
        /// conformément à la disjonction stricte chargement des
        /// libellés / chargement des données posée par le socle
        /// <see cref="VM_Page_Generic"/>.</para>
        ///
        /// <para>Filet de sécurité : Aucune invocation susceptible de
        /// lever une exception terminale n'est portée par le
        /// constructeur. Les gardes
        /// <see cref="ArgumentNullException"/> sur les trois premiers
        /// paramètres sont déléguées à <see cref="VM_Generic"/> via la
        /// chaîne <c>base(...)</c> ; l'appel à
        /// <see cref="VM_Generic.InitializeLabels"/> est protégé en
        /// interne par le filet existant de <c>SR_Dictionary</c>
        /// (R-4.11.6 et R-4.11.10 du 0231). Aucune intervention de
        /// <see cref="VM_Generic.ExecuteSafeAsync"/> hérité n'est
        /// requise au constructeur — la méthode est destinée à
        /// encapsuler les opérations asynchrones, absentes du
        /// constructeur. Le filet hérité est en revanche mobilisé par
        /// <see cref="LoadAsync"/>.</para>
        /// </remarks>
        /// <param name="dictionary">Service d'accès au dictionnaire
        /// multilingue, transmis à <see cref="VM_Page_Generic"/> via
        /// <c>base(...)</c>. Injecté en Singleton par le conteneur
        /// DI.</param>
        /// <param name="logAndNotify">Orchestrateur du traitement
        /// terminal des erreurs, transmis à
        /// <see cref="VM_Page_Generic"/> via <c>base(...)</c>.
        /// Mobilisé par le filet hérité
        /// <see cref="VM_Generic.ExecuteSafeAsync"/> dans
        /// <see cref="LoadAsync"/>. Injecté en Singleton par le
        /// conteneur DI au titre de l'EA-01.</param>
        /// <param name="app">Setting Singleton de l'état applicatif
        /// global, transmis à <see cref="VM_Page_Generic"/> via
        /// <c>base(...)</c> pour l'alimentation de la mécanique
        /// multilingue factorisée par l'ancêtre commun (abonnement
        /// INPC interne à <see cref="ISE_App.AppCultureCode"/>). Le
        /// présent dérivé ne stocke pas cette dépendance ni n'y
        /// accède directement, conformément à I-4.11.11 du 0231.
        /// Injecté en Singleton par le conteneur DI.</param>
        /// <param name="useCaseInvoker">Composant Singleton porteur
        /// d'EA-11, unique voie d'invocation du UseCase
        /// <see cref="IU_GetApplicationVersion"/> depuis le présent
        /// ViewModel. Injecté en Singleton par le conteneur DI.</param>
        /// <exception cref="ArgumentNullException">Levée si
        /// <paramref name="useCaseInvoker"/> est
        /// <see langword="null"/>. Les gardes sur
        /// <paramref name="dictionary"/>,
        /// <paramref name="logAndNotify"/> et <paramref name="app"/>
        /// sont portées par <see cref="VM_Generic"/> via la chaîne
        /// <c>base(...)</c>.</exception>
        public VM_Page98(
            IS_Dictionary dictionary,
            IU_LogAndNotify logAndNotify,
            ISE_App app,
            IS_UseCaseInvoker useCaseInvoker)
            : base(dictionary, logAndNotify, app)
        {
            _useCaseInvoker = useCaseInvoker
                ?? throw new ArgumentNullException(nameof(useCaseInvoker));

            InitializeLabels();
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Redéfinit le hook canonique
        /// <see cref="VM_Page_Generic.LoadAsync"/> pour charger la
        /// donnée fonctionnelle <see cref="VersionNumber"/> par
        /// invocation du UseCase
        /// <see cref="IU_GetApplicationVersion"/> via
        /// <see cref="IS_UseCaseInvoker"/> (EA-11).
        /// </summary>
        /// <param name="callChain">CallChain construite par
        /// l'orchestrateur appelant côté <c>Page_Generic</c> au
        /// format normatif
        /// <c>{_callee} &gt; OnLoadedHandler &gt; OnLoadedAsync</c>
        /// et propagée tel quel par le code-behind via
        /// <c>_viewModel.LoadAsync(callChain, ct)</c>. Le paramètre est
        /// reçu par contrat du hook au socle
        /// <see cref="VM_Page_Generic"/> mais n'est pas consommé par le
        /// corps du présent override : une CallChain interne distincte
        /// est construite via
        /// <see cref="VM_Generic.BuildFirstCallChain"/> et consommée
        /// par <see cref="VM_Generic.ExecuteSafeAsync"/> et par le
        /// délégué d'invocation du UseCase, conformément au patron de
        /// surcharge normatif §4.15.6 du 0230.</param>
        /// <param name="ct">Jeton d'annulation coopérative propagé par
        /// le code-behind appelant. Propagé à
        /// <see cref="VM_Generic.ExecuteSafeAsync"/>, à
        /// <see cref="IS_UseCaseInvoker.InvokeAsync{TUseCase, TResult}(System.Func{TUseCase, System.Threading.CancellationToken, System.Threading.Tasks.Task{TResult}}, System.Threading.CancellationToken)"/>
        /// et, par le délégué, au UseCase
        /// <see cref="IU_GetApplicationVersion"/>. Valeur par défaut :
        /// <see langword="default"/>.</param>
        /// <returns>Une tâche représentant l'exécution asynchrone du
        /// chargement de <see cref="VersionNumber"/>.</returns>
        /// <remarks>
        /// <para>Contexte : Override du hook canonique
        /// <see cref="VM_Page_Generic.LoadAsync"/> déclaré
        /// <c>public virtual</c> au socle conformément à §4.15.6
        /// du 0230. Invoquée depuis le code-behind de <c>Page98</c>
        /// au point d'extension <c>OnLoadedAsync</c> exposé par
        /// <c>Page_Generic</c> (§4.15.7 du 0230). Méthode strictement
        /// disjointe de <see cref="LoadLabels"/> : libellés synchrones
        /// au constructeur d'un côté, donnée fonctionnelle asynchrone
        /// au <c>Loaded</c> de la page de l'autre.</para>
        ///
        /// <para>Objectif : Alimenter la propriété observable
        /// <see cref="VersionNumber"/> avec le numéro de version
        /// courant de l'application, lu via le UseCase
        /// <see cref="IU_GetApplicationVersion"/>.</para>
        ///
        /// <para>Patron de surcharge normatif (§4.15.6 du 0230) :
        /// L'override construit une CallChain interne
        /// (<c>innerCallChain</c>) via
        /// <see cref="VM_Generic.BuildFirstCallChain"/> hérité, plutôt
        /// que de consommer la CallChain reçue en paramètre. Le
        /// paramètre <paramref name="callChain"/> reçu du hook est
        /// utile à des fins de traçabilité amont mais la CallChain
        /// consommée par le filet et par le délégué d'invocation est
        /// celle reconstruite localement, garantissant que le format
        /// normatif <c>{_callee} &gt; LoadAsync</c> est appliqué pour
        /// l'opération elle-même.</para>
        ///
        /// <para>Idempotence : La méthode est ré-appelée à chaque
        /// entrée sur la page sans flag de mémoire d'état. Chaque
        /// appel produit une nouvelle invocation complète du UseCase
        /// et une nouvelle affectation de
        /// <see cref="VersionNumber"/> — coût négligeable, stricte
        /// simplicité du contrat.</para>
        ///
        /// <para>Filet de sécurité : L'invocation est encapsulée par
        /// le filet hérité <see cref="VM_Generic.ExecuteSafeAsync"/>
        /// (§4.7.3 du 0230). Toute exception applicative
        /// typée non capturée par le pipeline du UseCase amont est
        /// traitée terminalement par <see cref="IU_LogAndNotify"/> via
        /// le filet hérité. En cas d'exception applicative typée
        /// capturée par le pipeline du UseCase lui-même, ce dernier
        /// retourne <see cref="string.Empty"/> ;
        /// <see cref="VersionNumber"/> est alors affectée à la chaîne
        /// vide. Aucun try/catch local n'est posé au sein de
        /// <see cref="LoadAsync"/>.</para>
        ///
        /// <para>Propagation du <see cref="System.Threading.CancellationToken"/> :
        /// Le jeton fourni par l'appelant est propagé à
        /// <see cref="VM_Generic.ExecuteSafeAsync"/> et, par fermeture,
        /// à
        /// <see cref="IS_UseCaseInvoker.InvokeAsync{TUseCase, TResult}(System.Func{TUseCase, System.Threading.CancellationToken, System.Threading.Tasks.Task{TResult}}, System.Threading.CancellationToken)"/>,
        /// puis au délégué d'invocation du UseCase consommé. Le jeton
        /// effectif passé au UseCase au sein du délégué est celui que
        /// <see cref="IS_UseCaseInvoker"/> transmet au délégué
        /// (paramètre <c>innerCt</c>), conformément à la signature
        /// canonique de §4.10.10 du 0230.</para>
        ///
        /// <para>Mode d'invocation : Conformément à §4.10.10 du 0230,
        /// l'invocation du UseCase
        /// <see cref="IU_GetApplicationVersion"/> est portée par
        /// <see cref="IS_UseCaseInvoker"/> qui matérialise un
        /// <c>IServiceScope</c> distinct pour l'invocation, y résout
        /// l'implémentation du contrat et l'exécute via le délégué
        /// fourni, puis dispose le scope. Le présent ViewModel
        /// n'injecte pas directement le contrat
        /// <see cref="IU_GetApplicationVersion"/>, conformément à
        /// I-4.10.10 du 0231.</para>
        /// </remarks>
        /// <exception cref="OperationCanceledException">
        /// Propagée silencieusement à l'appelant sur signal
        /// d'annulation coopérative par le filet hérité
        /// <see cref="VM_Generic.ExecuteSafeAsync"/>, conformément à
        /// §4.7.3 du 0230. Aucune journalisation ni notification.
        /// </exception>
        public override async Task LoadAsync(
            string callChain,
            CancellationToken ct = default)
        {
            string innerCallChain = BuildFirstCallChain();

            await ExecuteSafeAsync(innerCallChain, async () =>
            {
                VersionNumber = await _useCaseInvoker
                    .InvokeAsync<IU_GetApplicationVersion, string>(
                        (useCase, innerCt) => useCase.ExecuteAsync(innerCallChain, innerCt),
                        ct);
            }, ct);
        }

        #endregion

        #region === Méthodes protégées ===

        /// <summary>
        /// Redéfinit le point d'extension
        /// <see cref="VM_Generic.LoadLabels"/> pour charger les 55
        /// libellés multilingues affichés par la page <c>Page98</c> à
        /// partir des clés <c>P98_01</c> à <c>P98_55</c> du
        /// dictionnaire de langue actif et les affecter aux 55
        /// propriétés observables <see cref="Label_P98_01"/> à
        /// <see cref="Label_P98_55"/>.
        /// </summary>
        /// <param name="callChain">CallChain construite par
        /// <see cref="VM_Generic.InitializeLabels"/> au constructeur
        /// (premier chargement) ou par le handler interne d'abonnement
        /// INPC de <see cref="VM_Generic"/> au changement de langue
        /// dynamique (rechargement), et transmise au service de
        /// dictionnaire pour traçabilité.</param>
        /// <remarks>
        /// <para>Contexte : Méthode redéfinissant le point d'extension
        /// <see cref="VM_Generic.LoadLabels"/> conformément à R-4.11.8
        /// du 0231. Invoquée par
        /// <see cref="VM_Generic.InitializeLabels"/> au constructeur
        /// pour le premier chargement, puis par le handler interne
        /// d'abonnement INPC de <see cref="VM_Generic"/> à chaque
        /// changement de langue dynamique notifié par
        /// <see cref="ISE_App.AppCultureCode"/>, avec marshalling
        /// Dispatcher défensif vers le thread UI. Ne touche pas à la
        /// propriété <see cref="VersionNumber"/>, dont le chargement
        /// est porté par <see cref="LoadAsync"/>.</para>
        ///
        /// <para>Objectif : Garantir que les 55 propriétés
        /// <c>Label_P98_NN</c> sont synchronisées avec la langue
        /// active du dictionnaire, tant au moment de l'instanciation
        /// du ViewModel que lors de tout changement ultérieur de
        /// langue dynamique au cours de la session.</para>
        ///
        /// <para>Patron strict : Une affectation par ligne, dans
        /// l'ordre numérique croissant des clés (<c>P98_01</c> à
        /// <c>P98_55</c>), sans regroupement et sans condition. Aucun
        /// raccourci de type boucle dynamique : la résolution
        /// nominative permet une revue de code aisée et un repérage
        /// statique des clés consommées.</para>
        ///
        /// <para>Absence d'appel à <c>base.LoadLabels(callChain)</c> :
        /// L'implémentation par défaut de
        /// <see cref="VM_Generic.LoadLabels"/> ne porte aucun
        /// traitement. L'appel à <c>base.LoadLabels(callChain)</c>
        /// n'apporterait qu'un bruit inutile et est délibérément omis,
        /// conformément à la pratique standard d'override lorsque la
        /// base ne porte aucun traitement, alignée sur le patron de
        /// <c>VM_Page99.LoadLabels</c>.</para>
        ///
        /// <para>Filet de sécurité : Aucun try/catch local n'est posé.
        /// Le filet est porté exclusivement par <c>SR_Dictionary</c>
        /// conformément à R-4.11.6 et R-4.11.10 du 0231 — toute
        /// anomalie (clé absente, erreur inattendue) est journalisée
        /// en interne par <c>SR_Dictionary</c> et résolue par une
        /// valeur de repli <c>[P98_NN] not found</c>, sans
        /// interruption ni propagation d'exception au présent
        /// ViewModel. L'unique exception susceptible d'être propagée
        /// serait <see cref="OperationCanceledException"/>,
        /// structurellement impossible ici puisque la signature de
        /// <c>IS_Dictionary.GetText</c> est invoquée sans
        /// <see cref="System.Threading.CancellationToken"/> explicite
        /// (paramètre optionnel par défaut <c>default</c>, équivalent
        /// à <see cref="System.Threading.CancellationToken.None"/>).</para>
        /// </remarks>
        protected override void LoadLabels(string callChain)
        {
            Label_P98_01 = _dictionary.GetText(callChain, "P98_01");
            Label_P98_02 = _dictionary.GetText(callChain, "P98_02");
            Label_P98_03 = _dictionary.GetText(callChain, "P98_03");
            Label_P98_04 = _dictionary.GetText(callChain, "P98_04");
            Label_P98_05 = _dictionary.GetText(callChain, "P98_05");
            Label_P98_06 = _dictionary.GetText(callChain, "P98_06");
            Label_P98_07 = _dictionary.GetText(callChain, "P98_07");
            Label_P98_08 = _dictionary.GetText(callChain, "P98_08");
            Label_P98_09 = _dictionary.GetText(callChain, "P98_09");
            Label_P98_10 = _dictionary.GetText(callChain, "P98_10");
            Label_P98_11 = _dictionary.GetText(callChain, "P98_11");
            Label_P98_12 = _dictionary.GetText(callChain, "P98_12");
            Label_P98_13 = _dictionary.GetText(callChain, "P98_13");
            Label_P98_14 = _dictionary.GetText(callChain, "P98_14");
            Label_P98_15 = _dictionary.GetText(callChain, "P98_15");
            Label_P98_16 = _dictionary.GetText(callChain, "P98_16");
            Label_P98_17 = _dictionary.GetText(callChain, "P98_17");
            Label_P98_18 = _dictionary.GetText(callChain, "P98_18");
            Label_P98_19 = _dictionary.GetText(callChain, "P98_19");
            Label_P98_20 = _dictionary.GetText(callChain, "P98_20");
            Label_P98_21 = _dictionary.GetText(callChain, "P98_21");
            Label_P98_22 = _dictionary.GetText(callChain, "P98_22");
            Label_P98_23 = _dictionary.GetText(callChain, "P98_23");
            Label_P98_24 = _dictionary.GetText(callChain, "P98_24");
            Label_P98_25 = _dictionary.GetText(callChain, "P98_25");
            Label_P98_26 = _dictionary.GetText(callChain, "P98_26");
            Label_P98_27 = _dictionary.GetText(callChain, "P98_27");
            Label_P98_28 = _dictionary.GetText(callChain, "P98_28");
            Label_P98_29 = _dictionary.GetText(callChain, "P98_29");
            Label_P98_30 = _dictionary.GetText(callChain, "P98_30");
            Label_P98_31 = _dictionary.GetText(callChain, "P98_31");
            Label_P98_32 = _dictionary.GetText(callChain, "P98_32");
            Label_P98_33 = _dictionary.GetText(callChain, "P98_33");
            Label_P98_34 = _dictionary.GetText(callChain, "P98_34");
            Label_P98_35 = _dictionary.GetText(callChain, "P98_35");
            Label_P98_36 = _dictionary.GetText(callChain, "P98_36");
            Label_P98_37 = _dictionary.GetText(callChain, "P98_37");
            Label_P98_38 = _dictionary.GetText(callChain, "P98_38");
            Label_P98_39 = _dictionary.GetText(callChain, "P98_39");
            Label_P98_40 = _dictionary.GetText(callChain, "P98_40");
            Label_P98_41 = _dictionary.GetText(callChain, "P98_41");
            Label_P98_42 = _dictionary.GetText(callChain, "P98_42");
            Label_P98_43 = _dictionary.GetText(callChain, "P98_43");
            Label_P98_44 = _dictionary.GetText(callChain, "P98_44");
            Label_P98_45 = _dictionary.GetText(callChain, "P98_45");
            Label_P98_46 = _dictionary.GetText(callChain, "P98_46");
            Label_P98_47 = _dictionary.GetText(callChain, "P98_47");
            Label_P98_48 = _dictionary.GetText(callChain, "P98_48");
            Label_P98_49 = _dictionary.GetText(callChain, "P98_49");
            Label_P98_50 = _dictionary.GetText(callChain, "P98_50");
            Label_P98_51 = _dictionary.GetText(callChain, "P98_51");
            Label_P98_52 = _dictionary.GetText(callChain, "P98_52");
            Label_P98_53 = _dictionary.GetText(callChain, "P98_53");
            Label_P98_54 = _dictionary.GetText(callChain, "P98_54");
            Label_P98_55 = _dictionary.GetText(callChain, "P98_55");
        }

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}