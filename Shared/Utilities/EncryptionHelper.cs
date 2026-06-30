using System.Security.Cryptography;
using System.Text;

namespace Shared.Utilities
{
    public static class EncryptionHelper
    {
        public static string Crypte_md5(string chaine)
        {
            // Convertir la chaîne en tableau de bytes
            byte[] tab_byte = Encoding.ASCII.GetBytes(chaine);

            // Initialiser le fournisseur de services MD5
            using (MD5 md5 = MD5.Create())
            {
                // Calculer le hash MD5
                tab_byte = md5.ComputeHash(tab_byte);
            }

            // Construire la chaîne hexadécimale à partir du tableau de bytes
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < tab_byte.Length; i++)
            {
                sb.AppendFormat("{0:X2}", tab_byte[i]);
            }

            // Retourner la chaîne en minuscules
            return sb.ToString().ToLower();
        }
    }
}
