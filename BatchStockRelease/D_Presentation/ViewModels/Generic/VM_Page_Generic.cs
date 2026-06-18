using System.ComponentModel;
using System.Runtime.CompilerServices;
using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;

namespace BatchStockRelease.D_Presentation.ViewModels.Generic
{
    /// <summary>
    /// VM_Page_Generic — Classe de base pour tous les ViewModels de page.
    ///
    /// <para><b>Contexte :</b> Cette classe fournit un socle commun à tous les
    /// ViewModels de l’application BatchStockRelease. Elle implémente
    /// <see cref="INotifyPropertyChanged"/> pour permettre la notification
    /// automatique des modifications de propriétés vers la vue WPF.</para>
    ///
    /// <para><b>Objectif :</b> Centraliser les services transverses et les méthodes
    /// utilitaires communes aux pages, notamment :</para>
    /// <list type="bullet">
    ///   <item><description>Accès aux paramètres métier (<see cref="IS_Settings_UseCase"/>).</description></item>
    ///   <item><description>Navigation entre les pages (<see cref="IS_Navigation"/>).</description></item>
    ///   <item><description>Accès au dictionnaire multilingue (<see cref="IS_Dictionary"/>).</description></item>
    ///   <item><description>Journalisation et notification des erreurs (<see cref="IS_LogAndNotify"/>).</description></item>
    /// </list>
    ///
    /// <para><b>Spécificités techniques :</b> Cette classe ne contient aucune logique métier,
    /// uniquement des outils d’infrastructure et de notification de propriétés.</para>
    /// </summary>
    public abstract class VM_Page_Generic : INotifyPropertyChanged
    {
        #region === Propriétés privés ===
        /// <summary>
        /// Nom du ViewModel appelé, utilisé pour la traçabilité et les logs.
        /// </summary>
        protected readonly string _callee;

        /// <summary>
        /// Fournit l’accès aux paramètres métier.
        /// </summary>
        protected readonly IS_Settings_UseCase _settingsUseCase;

        /// <summary>
        /// Gère la navigation entre les pages.
        /// </summary>
        protected readonly IS_Navigation _navigation;

        /// <summary>
        /// Permet d’accéder au dictionnaire de traduction multilingue.
        /// </summary>
        protected readonly IS_Dictionary _dictionary;

        /// <summary>
        /// Service centralisé de journalisation et de notification d’erreurs.
        /// </summary>
        protected readonly IS_LogAndNotify _logAndNotify;
        #endregion

        #region === Propriétés publiques ===
        // Ajouter ici les propriétés ou commandes spécifiques.
        #endregion

        #region === Constructeur ===
        /// <summary>
        /// Initialise le ViewModel générique avec les services partagés de l’application.
        /// </summary>
        /// <param name="settingsUseCase">Service de gestion des paramètres.</param>
        /// <param name="navigation">Service de navigation entre les pages.</param>
        /// <param name="dictionary">Service de gestion multilingue.</param>
        /// <param name="logAndNotify">Service de journalisation et de notification d’erreurs.</param>
        protected VM_Page_Generic(
            IS_Settings_UseCase settingsUseCase,
            IS_Navigation navigation,
            IS_Dictionary dictionary,
            IS_LogAndNotify logAndNotify)
        {
            _callee = GetType().Name;

            _settingsUseCase = settingsUseCase;
            _navigation = navigation;
            _dictionary = dictionary;
            _logAndNotify = logAndNotify;
        }
        #endregion

        #region === Méthodes génériques ===
        /// <summary>
        /// Construit la première partie d'une callChain pour la traçabilité et les logs.
        /// </summary>
        protected string BuildFirstCallChain([CallerMemberName] string caller = "")
            => $"{_callee} > {caller}";

        /// <summary>
        /// Exécute une action asynchrone en capturant automatiquement les exceptions
        /// selon leur type (Business, Infrastructure, Exception générique) et en les
        /// journalisant via le service <see cref="IS_LogAndNotify"/>.
        /// </summary>
        /// <param name="action">Action asynchrone à exécuter.</param>
        /// <exception cref="Ex_Business">Lancée lorsqu’une erreur métier se produit.</exception>
        /// <exception cref="Ex_Infrastructure">Lancée lorsqu’une erreur d’infrastructure se produit.</exception>
        protected async Task ExecuteSafeAsync(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (Ex_Business ex)
            {
                await _logAndNotify.ExecuteAsync("No_EC_18", ex);
            }
            catch (Ex_Infrastructure ex)
            {
                await _logAndNotify.ExecuteAsync("No_EC_19", ex);
            }
            catch (Exception ex)
            {
                await _logAndNotify.ExecuteAsync("No_EC_20", ex);
            }
        }
        #endregion

        #region === Implémentation de INotifyPropertyChanged ===
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Déclenche la notification de modification d’une propriété.
        /// </summary>
        /// <param name="propertyName">Nom de la propriété modifiée.</param>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Met à jour la valeur d’un champ et déclenche la notification
        /// de modification si la valeur a changé.
        /// </summary>
        /// <typeparam name="T">Type de la propriété.</typeparam>
        /// <param name="field">Référence du champ à mettre à jour.</param>
        /// <param name="value">Nouvelle valeur à affecter.</param>
        /// <param name="propertyName">Nom de la propriété (automatiquement détecté).</param>
        /// <returns><c>true</c> si la valeur a changé, sinon <c>false</c>.</returns>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
        #endregion
    }
}