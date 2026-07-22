using DG244Cutting.A_Domain.Interfaces.Services.App;
using System.Security.Cryptography;
using System.Text;

namespace DG244Cutting.B_UseCases.Services.App
{
    /// <summary>
    /// Description :
    /// <para>
    /// Service de hachage : primitive terminale de calcul d'empreinte d'une chaîne de
    /// caractères, restituée sous forme de chaîne hexadécimale en minuscules.
    /// </para>
    ///
    /// Contexte :
    /// <para>
    /// Ce service réside dans <c>B_UseCases</c> et honore le contrat <see cref="IS_Hashing"/>.
    /// Il est injecté dans les UseCases orchestrateurs (typiquement un UseCase
    /// d'authentification) qui rattachent l'empreinte produite à une validation métier de
    /// leur ressort. À l'image de <c>SR_ExClassifier</c>, il s'agit d'une primitive terminale
    /// sans dépendance : sa méthode publique est nue (une chaîne en entrée, une chaîne en
    /// sortie), sans paramètre de traçabilité, sans <c>try/catch</c> et sans injection.
    /// Le calcul étant purement en mémoire, il ne lève aucune exception d'infrastructure
    /// attendue.
    /// </para>
    ///
    /// Objectif :
    /// <para>
    /// Fournir une empreinte hexadécimale minuscule déterministe, en continuité algorithmique
    /// exacte avec le legacy (<c>Shared/Utilities/EncryptionHelper.Crypte_md5</c>), afin de
    /// préserver la comparabilité avec les empreintes déjà présentes en base.
    /// </para>
    ///
    /// Invariant d'implémentation :
    /// <para>
    /// L'algorithme concret est MD5, figé pour garantir la comparabilité avec les empreintes
    /// existantes (encodage ASCII de l'entrée, calcul MD5, reconstruction hexadécimale au
    /// format <c>{0:X2}</c>, puis passage en minuscules). Cet algorithme n'est pas exposé au
    /// contrat : il vit exclusivement dans le corps et la présente documentation. Toute
    /// divergence d'encodage, de casse ou de format de sortie invaliderait la comparaison
    /// avec les empreintes existantes.
    /// </para>
    ///
    /// Robustesse assumée par rapport au legacy :
    /// <para>
    /// Le legacy ne gère pas l'entrée <c>null</c> (il lèverait via <c>Encoding.ASCII.GetBytes</c>).
    /// Le présent service normalise <c>null</c> en chaîne vide en tête de méthode
    /// (<c>input ??= string.Empty</c>) avant d'appliquer l'algorithme legacy strictement
    /// inchangé. Cette normalisation étend le domaine de définition sans modifier l'empreinte
    /// des entrées non nulles : l'invariant de continuité algorithmique est préservé.
    /// </para>
    ///
    /// Utilisateurs cibles :
    /// <para>
    /// UseCases orchestrateurs (<c>UC_*</c>), via l'interface <see cref="IS_Hashing"/>.
    /// </para>
    /// </summary>
    /// <seealso cref="IS_Hashing"/>
    public class SR_Hashing : IS_Hashing
    {
        #region === Propriétés privées ===

        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        // Aucune. Primitive terminale sans dépendance (cf. étalon SR_ExClassifier).

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Description :
        /// <para>
        /// Initialise une nouvelle instance du service <see cref="SR_Hashing"/>.
        /// </para>
        ///
        /// Tâches / Actions :
        /// <list type="bullet">
        /// <item><description>Initialiser <c>_callee</c> pour la traçabilité.</description></item>
        /// </list>
        /// </summary>
        public SR_Hashing()
        {
            _callee = GetType().Name;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Description :
        /// <para>
        /// Calcule l'empreinte d'une chaîne de caractères et la restitue sous forme de
        /// chaîne hexadécimale en minuscules.
        /// </para>
        ///
        /// Contexte :
        /// <para>
        /// Point d'entrée unique du service. Le calcul est pur en mémoire, sans effet de bord,
        /// sans I/O ni mutation d'état. Le résultat est déterministe : deux appels sur une même
        /// entrée produisent une empreinte identique.
        /// </para>
        ///
        /// Objectif :
        /// <para>
        /// Restituer une empreinte hexadécimale minuscule identique bit-à-bit à celle produite
        /// par le legacy <c>EncryptionHelper.Crypte_md5</c> pour toute entrée non nulle.
        /// </para>
        ///
        /// Remarque de dénomination :
        /// <para>
        /// Le préfixe canonique <c>Execute</c> des méthodes publiques de Service (R-4.2.12) est
        /// ici remplacé par le verbe d'action <c>Hash</c>, dérogation admise en cas Concept
        /// (SR20 du 0232-SR) : la sémantique du concept porté — le hachage — justifie cette
        /// dénomination, plus expressive de la capacité exposée que le préfixe générique.
        /// </para>
        ///
        /// Tâches / Actions :
        /// <list type="bullet">
        /// <item><description>Normaliser une entrée <c>null</c> en chaîne vide (robustesse assumée).</description></item>
        /// <item><description>Encoder l'entrée en ASCII, calculer l'empreinte MD5.</description></item>
        /// <item><description>Reconstruire la chaîne hexadécimale et la restituer en minuscules.</description></item>
        /// </list>
        /// </summary>
        /// <param name="input">
        /// Chaîne dont l'empreinte est calculée. Une valeur <c>null</c> ou vide est admise et
        /// traitée comme la chaîne vide.
        /// </param>
        /// <returns>
        /// L'empreinte hexadécimale de <paramref name="input"/> en minuscules.
        /// </returns>
        public string Hash(string input)
        {
            input ??= string.Empty;

            byte[] bytes = Encoding.ASCII.GetBytes(input);

            using (MD5 md5 = MD5.Create())
            {
                bytes = md5.ComputeHash(bytes);
            }

            return BuildHexString(bytes);
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Description :
        /// <para>
        /// Reconstruit la représentation hexadécimale minuscule d'un tableau d'octets, au
        /// format legacy (<c>{0:X2}</c> par octet, puis passage global en minuscules).
        /// </para>
        /// </summary>
        /// <param name="bytes">Tableau d'octets de l'empreinte à formater.</param>
        /// <returns>La chaîne hexadécimale correspondante, en minuscules.</returns>
        private static string BuildHexString(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder(bytes.Length * 2);

            for (int i = 0; i < bytes.Length; i++)
            {
                sb.AppendFormat("{0:X2}", bytes[i]);
            }

            return sb.ToString().ToLower();
        }

        #endregion
    }
}