// SimulationReseau.cs

using System;
using System.Collections.Generic;
using System.IO;
using TP_Session; // Importation de l'espace de noms contenant la structure ConnexionInfo

namespace TP_Session
{
    // Classe principale qui simule le comportement du service de réseau connecté
    internal class SimulationReseau
    {
        // Identifiant de la prochaine connexion à établir
        private int prochainIDConnexion = 1;

        // Dictionnaire pour stocker toutes les connexions en cours (clé = "Src-Dest-ID")
        private Dictionary<string, ConnexionInfo> connexions = new();

        // Générateur aléatoire pour simuler les acquittements positifs/négatifs
        private Random rand = new();

        // Fichiers utilisés pour simuler la communication entre couches
        StreamWriter s_ecr = new("S_ecr.txt", false); // Écriture vers Transport
        StreamWriter l_ecr = new("L_ecr.txt", false); // Écriture vers Liaison
        StreamWriter l_lec = new("L_lec.txt", false); // Lecture fictive des réponses
        StreamReader s_lec = new("S_lec.txt");        // Lecture des demandes de connexion

        // Méthode pour établir une connexion à partir d'une demande lue dans S_lec.txt
        public void EtablirConnexion()
        {
            string ligne = s_lec.ReadLine();
            if (ligne == null)
            {
                Console.WriteLine("Aucune demande disponible.");
                return;
            }

            // Extraction des informations src, dest et message
            var parts = ligne.Split(';');
            int src = int.Parse(parts[0].Split('=')[1]);
            int dest = int.Parse(parts[1].Split('=')[1]);
            string message = parts[2].Split('=')[1];
            string identifiant = $"{src}-{dest}-{prochainIDConnexion}";

            // Création et enregistrement de la nouvelle connexion
            var info = new ConnexionInfo
            {
                ConnexionID = prochainIDConnexion++,
                Src = src,
                Dest = dest,
                Message = message
            };
            connexions[identifiant] = info;

            // Application des règles de refus spéciales selon src
            if (src % 27 == 0)
            {
                l_lec.WriteLine($"{info.ConnexionID} [RefusFournisseur]");
                EcrireVersTransport($"{identifiant} N_DISCONNECT.ind (Fournisseur)");
            }
            else if (src % 13 == 0)
            {
                l_lec.WriteLine($"{info.ConnexionID} [RefusDistant]");
                EcrireVersTransport($"{identifiant} N_DISCONNECT.ind (Distant)");
            }
            else if (src % 19 == 0)
            {
                return; // Pas de réponse simulée (silence)
            }
            else
            {
                // Établissement normal : envoi d'un paquet d'appel
                l_lec.WriteLine($"{info.ConnexionID} [PaquetAppel]");
                EcrireVersReseau($"[PaquetAppel] ID={info.ConnexionID} Src={src} Dest={dest}");
            }
        }

        // Méthode pour accepter une connexion en attente
        public void AccepterConnexion()
        {
            foreach (var info in connexions.Values)
            {
                if (!info.ConnexionEtablie)
                {
                    EcrireVersTransport($"{info.Src}-{info.Dest}-{info.ConnexionID} N_CONNECT.conf");
                    info.ConnexionEtablie = true;
                    return;
                }
            }
            Console.WriteLine("Aucune connexion en attente.");
        }

        // Méthode pour lancer l'échange de données après établissement
        public void LancerEchangeDonnees()
        {
            foreach (var info in connexions.Values)
            {
                if (info.ConnexionEtablie)
                {
                    EnvoyerDonnees(info);
                    return;
                }
            }
            Console.WriteLine("Aucune connexion établie.");
        }

        // Méthode privée pour envoyer toutes les données d'une connexion segmentée
        private void EnvoyerDonnees(ConnexionInfo info)
        {
            // Calcul du nombre de segments nécessaires
            int segmentCount = (int)Math.Ceiling(info.Message.Length / 128.0);
            for (int i = 0; i < segmentCount; i++)
            {
                // Extraction d'un segment de 128 caractères max
                string segment = info.Message.Substring(i * 128, Math.Min(128, info.Message.Length - i * 128));
                int m = (i < segmentCount - 1) ? 1 : 0; // Indicateur de message continu (m=1) ou fin de message (m=0)
                int typeOctet = (info.PR << 5) | (m << 4) | (info.PS << 1);

                bool paquetEnvoye = false;
                int tentative = 0;

                do
                {
                    // Envoi du segment courant
                    EcrireVersReseau($"[PaquetDonnées] ID={info.ConnexionID} PS={info.PS} Données={segment}");

                    if (info.Src % 15 == 0)
                    {
                        // Cas où aucun acquittement n'est simulé
                        l_lec.WriteLine($"{info.ConnexionID} [AucunAcquittement]");
                        break;
                    }
                    else
                    {
                        // Simulation aléatoire d'acquittement négatif
                        bool acquittementNegatif = rand.Next(0, 8) == info.PS;
                        if (acquittementNegatif)
                        {
                            l_lec.WriteLine($"{info.ConnexionID} [AcquittementNegatif] PR={info.PR}");
                            tentative++;
                            if (tentative > 1)
                            {
                                Console.WriteLine($"[WARNING] Échec de l'envoi après une tentative de réémission (PS={info.PS})");
                                break;
                            }
                            Console.WriteLine($"[INFO] Réémission du paquet PS={info.PS}");
                        }
                        else
                        {
                            // Acquittement positif simulé
                            l_lec.WriteLine($"{info.ConnexionID} [AcquittementPositif] PR={info.PR}");
                            info.PS = (info.PS + 1) % 8;
                            paquetEnvoye = true;
                        }
                    }
                } while (!paquetEnvoye);
            }
        }

        // Méthode pour libérer manuellement une connexion active
        public void LibererConnexionManuelle()
        {
            foreach (var info in connexions.Values)
            {
                if (info.ConnexionEtablie)
                {
                    EcrireVersReseau($"[PaquetLibération] ID={info.ConnexionID}");
                    EcrireVersTransport($"{info.Src}-{info.Dest}-{info.ConnexionID} N_DISCONNECT.req");
                    connexions.Remove($"{info.Src}-{info.Dest}-{info.ConnexionID}");
                    return;
                }
            }
            Console.WriteLine("Aucune connexion à libérer.");
        }

        // Méthode pour refuser une connexion non encore établie
        public void RefuserConnexion()
        {
            foreach (var info in connexions.Values)
            {
                if (!info.ConnexionEtablie)
                {
                    EcrireVersTransport($"{info.Src}-{info.Dest}-{info.ConnexionID} N_DISCONNECT.ind");
                    connexions.Remove($"{info.Src}-{info.Dest}-{info.ConnexionID}");
                    return;
                }
            }
            Console.WriteLine("Aucune connexion à refuser.");
        }

        // Méthode privée pour écrire un message vers la couche transport (fichier et console)
        private void EcrireVersTransport(string msg)
        {
            s_ecr.WriteLine(msg);
            Console.WriteLine("[TRANSPORT] " + msg);
        }

        // Méthode privée pour écrire un message vers la couche réseau (fichier et console)
        private void EcrireVersReseau(string msg)
        {
            l_ecr.WriteLine(msg);
            Console.WriteLine("[RÉSEAU] " + msg);
        }

        // Méthode pour fermer tous les fichiers ouverts proprement à la fin de la simulation
        public void TerminerSimulation()
        {
            s_ecr.Close();
            l_ecr.Close();
            l_lec.Close();
            s_lec.Close();
        }
    }
}
