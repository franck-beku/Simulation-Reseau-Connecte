// ConnexionInfo.cs
namespace TP_Session
{
    // Définition de la classe ConnexionInfo
    // Cette classe représente toutes les informations associées à une connexion réseau simulée.
    internal class ConnexionInfo
    {
        // Identifiant unique de la connexion (généré automatiquement)
        public int ConnexionID;

        // Adresse source de la station qui initie la connexion
        public int Src;

        // Adresse destination de la station cible de la connexion
        public int Dest;

        // Numéro de séquence pour l'envoi (PS - Prochain Segment à envoyer)
        public int PS = 0;

        // Numéro de séquence attendu en réception (PR - Prochain Segment attendu)
        public int PR = 0;

        // Indique si la connexion est actuellement établie (true) ou non (false)
        public bool ConnexionEtablie = false;

        // Message utilisateur à transmettre lors de l'échange de données
        public string Message;
    }
}