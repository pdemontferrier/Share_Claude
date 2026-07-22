using System.Windows;
using System.Windows.Controls;

namespace DG244Cutting.D_Presentation.Utilities.Bindings
{
    /// <summary>
    /// Utilitaire de binding WPF réalisant la synchronisation bidirectionnelle entre
    /// le contenu d'un PasswordBox et une propriété string exposée par un ViewModel.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : La propriété PasswordBox.Password n'est pas une DependencyProperty -
    /// choix de conception de WPF motivé par la sécurité, le mot de passe n'étant pas
    /// exposé en clair dans le système de propriétés de dépendance. Elle ne peut donc
    /// pas être la cible d'un binding déclaratif direct dans le modèle MVVM. Le
    /// contournement idiomatique consiste à porter la liaison par des propriétés
    /// attachées, seul canal permettant d'exposer la saisie du contrôle au binding.
    /// </para>
    /// <para>
    /// Objectif : Établir un canal de liaison bidirectionnel entre la saisie
    /// utilisateur dans le PasswordBox et une propriété string du ViewModel. La saisie
    /// dans le contrôle est répercutée vers la propriété liée, et toute écriture de la
    /// propriété liée depuis le ViewModel est répercutée dans le contrôle, sans qu'aucune
    /// des deux répercussions n'en déclenche une autre en retour.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Exposer les propriétés attachées EnableBinding (activation de la synchronisation) et Password (valeur liée au ViewModel).</description></item>
    ///   <item><description>Prévenir les boucles de mise à jour vue vers ViewModel vers vue au moyen d'un drapeau interne et du désabonnement temporaire du gestionnaire de saisie.</description></item>
    ///   <item><description>Activer ou désactiver la synchronisation conditionnellement selon la valeur de la propriété attachée EnableBinding.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Ne porte aucune logique métier : la string transportée n'est ni interprétée, ni validée, ni hachée ; le composant ne connaît pas le concept de mot de passe.</description></item>
    ///   <item><description>Ne participe à aucune mécanique transverse du référentiel (CallChain, annulation coopérative, gestion d'erreurs, multilingue, navigation).</description></item>
    ///   <item><description>Ne dépend d'aucun service injecté et n'est pas enregistrée dans le conteneur DI (SR_ConteneurDI).</description></item>
    /// </list>
    /// <para>Usage type :</para>
    /// <code>
    /// &lt;PasswordBox
    ///     bindings:UT_BindingPasswordBox.EnableBinding="True"
    ///     bindings:UT_BindingPasswordBox.Password="{Binding Password, Mode=TwoWay}" /&gt;
    /// </code>
    /// <para>
    /// où le préfixe d'alias bindings est déclaré par
    /// xmlns:bindings="clr-namespace:DG244Cutting.D_Presentation.Utilities.Bindings".
    /// </para>
    /// <para>
    /// Statut hors familles canoniques : La classe UT_BindingPasswordBox relève de la
    /// famille UT_ (Utility WPF) au sens de 2.7 du 0230 (Famille 6 - Utilitaires). Cette
    /// famille n'est pas couverte par la règle de parité interface/implémentation posée
    /// en 2.7.4 du 0230 et indexée par R-2.7.6 du 0231 ; aucun contrat IS_ ou autre n'est
    /// attendu.
    /// </para>
    /// <para>
    /// Exceptions architecturales : La classe ne porte aucune exception architecturale
    /// propre. Aucune ligne EA-NN ne lui est associée à l'inventaire de 4.15.10 du 0230.
    /// </para>
    /// </remarks>
    public static class UT_BindingPasswordBox
    {
        #region === Propriétés privées ===

        // A compléter

        #endregion

        #region === Dépendances privées ===

        // A compléter

        #endregion

        #region === Constructeur ===

        // A compléter

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Propriété attachée publique activant ou désactivant la synchronisation
        /// bidirectionnelle du mot de passe sur le PasswordBox cible.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : DependencyProperty attachée enregistrée sous le nom logique
        /// "EnableBinding", de type bool, de type propriétaire UT_BindingPasswordBox et
        /// de valeur par défaut false. Sa métadonnée déclare le callback
        /// OnEnableBindingChanged, déclenché à chaque changement de valeur sur l'élément
        /// porteur.
        /// </para>
        /// <para>
        /// Objectif : Servir d'interrupteur déclaratif de la liaison en XAML. Le passage
        /// à true abonne le gestionnaire de saisie PasswordChanged sur le PasswordBox
        /// cible ; le passage à false l'en désabonne. Tant que la valeur reste false,
        /// aucune synchronisation n'est établie.
        /// </para>
        /// </remarks>
        public static readonly DependencyProperty EnableBindingProperty =
            DependencyProperty.RegisterAttached(
                "EnableBinding",
                typeof(bool),
                typeof(UT_BindingPasswordBox),
                new PropertyMetadata(false, OnEnableBindingChanged));

        /// <summary>
        /// Lit la valeur courante de la propriété attachée EnableBinding sur l'élément cible.
        /// </summary>
        /// <param name="dp">Objet de dépendance porteur de la propriété attachée.</param>
        /// <returns><see langword="true"/> si la synchronisation est active sur l'élément ; <see langword="false"/> sinon.</returns>
        public static bool GetEnableBinding(DependencyObject dp) =>
            (bool)dp.GetValue(EnableBindingProperty);

        /// <summary>
        /// Affecte la valeur de la propriété attachée EnableBinding sur l'élément cible.
        /// </summary>
        /// <param name="dp">Objet de dépendance porteur de la propriété attachée.</param>
        /// <param name="value">Nouvel état d'activation de la synchronisation à appliquer.</param>
        public static void SetEnableBinding(DependencyObject dp, bool value) =>
            dp.SetValue(EnableBindingProperty, value);

        /// <summary>
        /// Propriété attachée publique portant la valeur du mot de passe liée à une
        /// propriété string du ViewModel, cible du binding bidirectionnel.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : DependencyProperty attachée enregistrée sous le nom logique
        /// "Password", de type string, de type propriétaire UT_BindingPasswordBox et de
        /// valeur par défaut string.Empty. Sa métadonnée déclare le callback
        /// OnPasswordChanged, déclenché à chaque changement de valeur, notamment lors
        /// d'une écriture d'origine ViewModel.
        /// </para>
        /// <para>
        /// Objectif : Exposer un point de binding string sur le PasswordBox, absent
        /// nativement du contrôle. Une écriture d'origine ViewModel est répercutée dans
        /// PasswordBox.Password ; une saisie utilisateur est répercutée dans cette
        /// propriété par le gestionnaire PasswordChanged. Le drapeau anti-réentrance
        /// interne garantit qu'aucune des deux répercussions n'en déclenche une autre en
        /// retour.
        /// </para>
        /// </remarks>
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached(
                "Password",
                typeof(string),
                typeof(UT_BindingPasswordBox),
                new PropertyMetadata(string.Empty, OnPasswordChanged));

        /// <summary>
        /// Lit la valeur courante de la propriété attachée Password sur l'élément cible.
        /// </summary>
        /// <param name="dp">Objet de dépendance porteur de la propriété attachée.</param>
        /// <returns>Valeur du mot de passe actuellement liée sur l'élément.</returns>
        public static string GetPassword(DependencyObject dp) =>
            (string)dp.GetValue(PasswordProperty);

        /// <summary>
        /// Affecte la valeur de la propriété attachée Password sur l'élément cible.
        /// </summary>
        /// <param name="dp">Objet de dépendance porteur de la propriété attachée.</param>
        /// <param name="value">Nouvelle valeur du mot de passe à lier sur l'élément.</param>
        public static void SetPassword(DependencyObject dp, string value) =>
            dp.SetValue(PasswordProperty, value);

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Propriété attachée privée servant de drapeau anti-réentrance pendant la
        /// synchronisation, afin de prévenir les boucles de mise à jour vue vers
        /// ViewModel vers vue.
        /// </summary>
        /// <remarks>
        /// DependencyProperty attachée enregistrée sous le nom logique "UpdatingPassword",
        /// de type bool, de valeur par défaut false et sans callback. Positionnée à true
        /// le temps de la recopie de la saisie vers la propriété liée, elle inhibe la
        /// répercussion inverse déclenchée par OnPasswordChanged.
        /// </remarks>
        private static readonly DependencyProperty UpdatingPasswordProperty =
            DependencyProperty.RegisterAttached(
                "UpdatingPassword",
                typeof(bool),
                typeof(UT_BindingPasswordBox),
                new PropertyMetadata(false));

        /// <summary>
        /// Lit la valeur du drapeau anti-réentrance UpdatingPassword sur l'élément cible.
        /// </summary>
        /// <param name="dp">Objet de dépendance porteur de la propriété attachée.</param>
        /// <returns><see langword="true"/> si une mise à jour est en cours sur l'élément ; <see langword="false"/> sinon.</returns>
        private static bool GetUpdatingPassword(DependencyObject dp) =>
            (bool)dp.GetValue(UpdatingPasswordProperty);

        /// <summary>
        /// Affecte la valeur du drapeau anti-réentrance UpdatingPassword sur l'élément cible.
        /// </summary>
        /// <param name="dp">Objet de dépendance porteur de la propriété attachée.</param>
        /// <param name="value">Nouvel état du drapeau à appliquer.</param>
        private static void SetUpdatingPassword(DependencyObject dp, bool value) =>
            dp.SetValue(UpdatingPasswordProperty, value);

        /// <summary>
        /// Callback déclenché à chaque changement de la propriété attachée EnableBinding,
        /// abonnant ou désabonnant le gestionnaire de saisie PasswordChanged sur le
        /// PasswordBox cible selon la nouvelle valeur.
        /// </summary>
        /// <param name="dp">Élément WPF dont la propriété a changé, attendu être un PasswordBox.</param>
        /// <param name="e">Données de l'événement contenant l'ancienne et la nouvelle valeur.</param>
        private static void OnEnableBindingChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            if (dp is PasswordBox passwordBox)
            {
                if ((bool)e.NewValue)
                    passwordBox.PasswordChanged += PasswordChanged;
                else
                    passwordBox.PasswordChanged -= PasswordChanged;
            }
        }

        /// <summary>
        /// Callback déclenché à chaque changement de la propriété attachée Password,
        /// répercutant une écriture d'origine ViewModel dans PasswordBox.Password.
        /// </summary>
        /// <remarks>
        /// Le gestionnaire PasswordChanged est désabonné le temps de la répercussion puis
        /// réabonné, afin qu'une écriture d'origine ViewModel ne re-déclenche pas une
        /// propagation vers le ViewModel. La recopie n'a lieu qu'en l'absence de mise à
        /// jour en cours (drapeau UpdatingPassword à false), ce qui neutralise la
        /// répercussion consécutive à une saisie utilisateur.
        /// </remarks>
        /// <param name="dp">Élément WPF dont la propriété a changé, attendu être un PasswordBox.</param>
        /// <param name="e">Données de l'événement contenant la nouvelle valeur d'origine ViewModel.</param>
        private static void OnPasswordChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            if (dp is PasswordBox passwordBox)
            {
                passwordBox.PasswordChanged -= PasswordChanged;

                if (!GetUpdatingPassword(passwordBox))
                    passwordBox.Password = e.NewValue?.ToString() ?? string.Empty;

                passwordBox.PasswordChanged += PasswordChanged;
            }
        }

        /// <summary>
        /// Gestionnaire de l'événement PasswordChanged du PasswordBox, répercutant la
        /// saisie utilisateur vers la propriété attachée Password.
        /// </summary>
        /// <remarks>
        /// Positionne le drapeau UpdatingPassword à true le temps de la recopie de
        /// PasswordBox.Password vers la propriété attachée Password, puis le repositionne
        /// à false. Cette séquence prévient la boucle de mise à jour vue vers ViewModel
        /// vers vue en neutralisant la répercussion inverse portée par OnPasswordChanged.
        /// </remarks>
        /// <param name="sender">Instance du PasswordBox déclencheur de l'événement.</param>
        /// <param name="e">Arguments de l'événement de routage.</param>
        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                SetUpdatingPassword(passwordBox, true);
                SetPassword(passwordBox, passwordBox.Password);
                SetUpdatingPassword(passwordBox, false);
            }
        }

        #endregion
    }
}