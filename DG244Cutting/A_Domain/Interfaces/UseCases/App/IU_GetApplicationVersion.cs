namespace DG244Cutting.A_Domain.Interfaces.UseCases.App
{
    /// <summary>
    /// Contrat du UseCase responsable de la production du numéro de version courant
    /// de l'application DG244Cutting sous forme d'une chaîne de caractères.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : Le numéro de version applicatif est porté par l'attribut Version
    /// de l'assembly d'exécution courant. Ce UseCase encapsule l'accès à cette
    /// information transverse, sans rattachement à un domaine métier ni à une
    /// entité persistée. Il est consommé en chaîne (1) directe par
    /// <c>VM_Page98.LoadDataAsync</c> pour alimentation d'une propriété observable
    /// <c>VersionNumber</c> bindée à la vue <c>Page98</c>. Implémenté par
    /// <see cref="UC_GetApplicationVersion"/>.
    /// </para>
    /// <para>
    /// Objectif : Exposer un point d'entrée unique et stable pour la lecture du
    /// numéro de version applicatif, indépendant des évolutions de la chaîne
    /// d'assemblage et isolé du reste du code par un contrat IU_ standard.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Exposer la signature canonique de lecture du numéro de version applicatif.</description></item>
    ///   <item><description>Garantir l'uniformité du résultat retourné (chaîne issue de <see cref="System.Version.ToString()"/> ou <see cref="string.Empty"/>).</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Ne préjuge pas du format d'affichage du numéro de version dans la couche de présentation.</description></item>
    ///   <item><description>Ne porte aucune logique métier, aucune persistance, aucune mutation d'état applicatif.</description></item>
    ///   <item><description>N'expose aucun type technique de persistance (entité EF Core, IQueryable, DbContext).</description></item>
    /// </list>
    /// </remarks>
    public interface IU_GetApplicationVersion
    {
        /// <summary>
        /// Produit le numéro de version courant de l'application DG244Cutting sous
        /// forme d'une chaîne de caractères.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : Cette méthode est appelée par un ViewModel consommateur
        /// (<c>VM_Page98.LoadDataAsync</c> dans la chaîne d'appel actuellement
        /// référencée) après construction de la CallChain racine via
        /// <c>BuildFirstCallChain</c>.
        /// </para>
        /// <para>
        /// Objectif : Lire le numéro de version porté par
        /// <see cref="System.Reflection.Assembly.GetExecutingAssembly"/> et le
        /// restituer dans un format directement consommable par la couche de
        /// présentation.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        ///   <item><description>Lire l'assembly d'exécution courant et extraire la propriété <see cref="System.Reflection.AssemblyName.Version"/> de son <see cref="System.Reflection.AssemblyName"/>.</description></item>
        ///   <item><description>Retourner la chaîne issue de <see cref="System.Version.ToString()"/> lorsque la propriété est présente.</description></item>
        ///   <item><description>Retourner <see cref="string.Empty"/> en cas d'absence légitime de donnée ou d'exception applicative typée capturée par le pipeline d'erreur standard.</description></item>
        /// </list>
        /// <para>
        /// Particularité doctrinale : La signature <see cref="Task{TResult}"/> avec
        /// <c>TResult = string</c> est retenue non au titre du retour signalable de
        /// R-4.14.21 (chaîne UC → UC normalisée), puisque ce UseCase n'est pas
        /// consommé en sous-séquence par un orchestrateur amont, mais parce que la
        /// valeur retournée est la donnée fonctionnelle elle-même - le numéro de
        /// version - et non un signal d'issue de scénario. Cette particularité est
        /// tracée nominativement conformément à R-4.2.13 ; le préfixe normatif
        /// ExecuteAsync est par ailleurs préservé, aucune dérogation au préfixe
        /// n'étant portée par cette méthode.
        /// </para>
        /// </remarks>
        /// <param name="caller">
        /// CallChain amont construite par le ViewModel consommateur via
        /// <c>BuildFirstCallChain</c>. Obligatoire ; transmise telle quelle au
        /// pipeline d'erreur le cas échéant.
        /// </param>
        /// <param name="ct">
        /// Jeton d'annulation coopérative. Vérifié à l'entrée de la méthode et
        /// propagé à <c>IU_LogAndNotify</c> en cas de capture d'exception
        /// applicative typée. Valeur par défaut : <see langword="default"/>.
        /// </param>
        /// <returns>
        /// Le numéro de version courant de l'application, tel que retourné par
        /// <see cref="System.Version.ToString()"/>, ou <see cref="string.Empty"/>
        /// si l'assembly d'exécution n'expose pas de propriété <c>Version</c>, ou
        /// si une exception applicative typée (<c>Ex_Business</c>,
        /// <c>Ex_Infrastructure</c>, <c>Ex_Unclassified</c>) est capturée par le
        /// pipeline d'erreur standard du UseCase.
        /// </returns>
        /// <exception cref="OperationCanceledException">
        /// Propagée silencieusement à l'appelant sur signal d'annulation
        /// coopérative, conformément à §4.6.3 du 0230. Aucune journalisation ni
        /// notification n'est déclenchée pour ce type d'exception.
        /// </exception>
        Task<string> ExecuteAsync(string caller, CancellationToken ct = default);
    }
}