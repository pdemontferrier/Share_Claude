using System.Windows;
using System.Windows.Controls;

namespace BatchStockRelease.D_Presentation.Utilities
{
    /// <summary>
    /// Classe utilitaire <c>UT_PasswordBoxHelper</c> — Permet d’activer le <b>data binding</b> 
    /// sur un contrôle <see cref="PasswordBox"/> dans une interface WPF.
    /// </summary>
    /// <para>
    /// Par défaut, la propriété <see cref="PasswordBox.Password"/> n’est pas une 
    /// <see cref="DependencyProperty"/> et ne peut donc pas être liée directement
    /// à une propriété d’un ViewModel dans le modèle MVVM.
    /// </para>
    /// <para>
    /// Cette classe ajoute deux propriétés attachées :
    /// <list type="bullet">
    ///   <item><description><c>BindPassword</c> : active la synchronisation entre la vue et le ViewModel.</description></item>
    ///   <item><description><c>BoundPassword</c> : stocke la valeur du mot de passe et permet le binding bidirectionnel.</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// Exemple d’utilisation dans le XAML :
    /// <code>
    /// &lt;PasswordBox 
    ///     utils:UT_PasswordBoxHelper.BindPassword="True"
    ///     utils:UT_PasswordBoxHelper.BoundPassword="{Binding Password, Mode=TwoWay}" /&gt;
    /// </code>
    /// </para>
    public static class UT_PasswordBoxHelper
    {
        #region === Dependency Properties ===

        /// <summary>
        /// Propriété attachée permettant de lier la valeur du mot de passe à une propriété du ViewModel.
        /// </summary>
        public static readonly DependencyProperty BoundPasswordProperty =
            DependencyProperty.RegisterAttached(
                "BoundPassword",
                typeof(string),
                typeof(UT_PasswordBoxHelper),
                new PropertyMetadata(string.Empty, OnBoundPasswordChanged));

        /// <summary>
        /// Propriété attachée qui active ou désactive la synchronisation du mot de passe.
        /// </summary>
        public static readonly DependencyProperty BindPasswordProperty =
            DependencyProperty.RegisterAttached(
                "BindPassword",
                typeof(bool),
                typeof(UT_PasswordBoxHelper),
                new PropertyMetadata(false, OnBindPasswordChanged));

        /// <summary>
        /// Propriété interne utilisée pour éviter les boucles de mise à jour 
        /// (vue → ViewModel → vue).
        /// </summary>
        private static readonly DependencyProperty UpdatingPasswordProperty =
            DependencyProperty.RegisterAttached(
                "UpdatingPassword",
                typeof(bool),
                typeof(UT_PasswordBoxHelper),
                new PropertyMetadata(false));

        #endregion

        #region === Accesseurs publics ===

        /// <summary>
        /// Définit la valeur de la propriété <c>BindPassword</c>.
        /// </summary>
        public static void SetBindPassword(DependencyObject dp, bool value)
            => dp.SetValue(BindPasswordProperty, value);

        /// <summary>
        /// Obtient la valeur de la propriété <c>BindPassword</c>.
        /// </summary>
        public static bool GetBindPassword(DependencyObject dp)
            => (bool)dp.GetValue(BindPasswordProperty);

        /// <summary>
        /// Obtient la valeur de la propriété <c>BoundPassword</c>.
        /// </summary>
        public static string GetBoundPassword(DependencyObject dp)
            => (string)dp.GetValue(BoundPasswordProperty);

        /// <summary>
        /// Définit la valeur de la propriété <c>BoundPassword</c>.
        /// </summary>
        public static void SetBoundPassword(DependencyObject dp, string value)
            => dp.SetValue(BoundPasswordProperty, value);

        #endregion

        #region === Accesseurs privés ===

        /// <summary>
        /// Obtient la valeur de la propriété interne <c>UpdatingPassword</c>.
        /// </summary>
        private static bool GetUpdatingPassword(DependencyObject dp)
            => (bool)dp.GetValue(UpdatingPasswordProperty);

        /// <summary>
        /// Définit la valeur de la propriété interne <c>UpdatingPassword</c>.
        /// </summary>
        private static void SetUpdatingPassword(DependencyObject dp, bool value)
            => dp.SetValue(UpdatingPasswordProperty, value);

        #endregion

        #region === Gestion des événements de liaison ===

        /// <summary>
        /// Appelé lorsque la propriété <c>BindPassword</c> est modifiée sur un contrôle <see cref="PasswordBox"/>.
        /// </summary>
        /// <param name="dp">Objet de dépendance concerné (normalement un <see cref="PasswordBox"/>).</param>
        /// <param name="e">Informations relatives à la modification de la propriété.</param>
        private static void OnBindPasswordChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
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
        /// Appelé lorsque la valeur de la propriété <c>BoundPassword</c> est modifiée
        /// (par exemple, depuis le ViewModel).
        /// </summary>
        /// <param name="dp">Objet de dépendance concerné.</param>
        /// <param name="e">Informations sur la nouvelle valeur.</param>
        private static void OnBoundPasswordChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            if (dp is PasswordBox passwordBox)
            {
                passwordBox.PasswordChanged -= PasswordChanged;

                // Empêche la double mise à jour pendant la synchronisation
                if (!(bool)GetUpdatingPassword(passwordBox))
                    passwordBox.Password = e.NewValue?.ToString() ?? string.Empty;

                passwordBox.PasswordChanged += PasswordChanged;
            }
        }

        /// <summary>
        /// Gestionnaire d’événement appelé lors d’une modification du mot de passe
        /// dans le contrôle <see cref="PasswordBox"/>.
        /// </summary>
        /// <param name="sender">Instance du <see cref="PasswordBox"/> déclencheur.</param>
        /// <param name="e">Arguments de l’événement.</param>
        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                SetUpdatingPassword(passwordBox, true);
                SetBoundPassword(passwordBox, passwordBox.Password);
                SetUpdatingPassword(passwordBox, false);
            }
        }

        #endregion
    }
}