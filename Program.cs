// Program.cs

using System;
using System.IO;
using TP_Session; // Importation de l'espace de noms pour accéder à SimulationReseau et ConnexionInfo

namespace TP_Session
{
    // Classe principale contenant le point d'entrée de l'application
    internal class Program
    {
        // Point d'entrée de l'application
        static void Main(string[] args)
        {
            // Génère le fichier S_lec.txt contenant les demandes de connexion
            GenererSlec(5);

            // Instancie l'objet SimulationReseau pour gérer la simulation
            var sim = new SimulationReseau();
            bool quitter = false;

            // Boucle principale affichant le menu et traitant les choix utilisateur
            while (!quitter)
            {
                Console.WriteLine("\n================ MENU ================");
                Console.WriteLine("1 - Établir une connexion");
                Console.WriteLine("2 - Accepter une connexion");
                Console.WriteLine("3 - Lancer l'échange de données");
                Console.WriteLine("4 - Libérer la connexion");
                Console.WriteLine("5 - Refuser la connexion");
                Console.WriteLine("0 - Quitter");
                Console.Write("Votre choix : ");

                // Lecture du choix utilisateur et exécution de l'action correspondante
                switch (Console.ReadLine())
                {
                    case "1": sim.EtablirConnexion(); break;               // Établir une connexion
                    case "2": sim.AccepterConnexion(); break;              // Accepter une connexion
                    case "3": sim.LancerEchangeDonnees(); break;           // Lancer l'échange de données
                    case "4": sim.LibererConnexionManuelle(); break;       // Libérer une connexion
                    case "5": sim.RefuserConnexion(); break;               // Refuser une connexion
                    case "0": quitter = true; break;                       // Quitter la simulation
                    default: Console.WriteLine("\n[Erreur] Choix invalide.\n"); break; // Gestion d'un mauvais choix
                }
            }

            // À la sortie de la boucle : fermeture des fichiers et terminaison propre
            sim.TerminerSimulation();
            Console.WriteLine("\nSimulation terminée.");
        }

        // Méthode statique pour générer aléatoirement des demandes de connexion
        static void GenererSlec(int nombreDemandes)
        {
            Random rand = new Random();
            using (StreamWriter writer = new StreamWriter("S_lec.txt", false))
            {
                for (int i = 0; i < nombreDemandes; i++)
                {
                    int src, dest;
                    // Tire des adresses source et destination différentes
                    do
                    {
                        src = rand.Next(0, 255);
                        dest = rand.Next(0, 255);
                    } while (src == dest);

                    // Message de 150 caractères pour forcer la segmentation (limite > 128)
                    string longMessage = new string('A', 150);

                    // Écrit la demande dans S_lec.txt au format attendu
                    writer.WriteLine($"src={src};dest={dest};message={longMessage}");
                }
            }
        }
    }
}
