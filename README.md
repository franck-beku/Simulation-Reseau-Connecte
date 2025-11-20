# 📡 Simulation d’un service réseau connecté – INF1009 (UQTR)

Projet académique visant à implémenter une simulation complète d’un **service de réseau en mode connecté**, en reproduisant les étapes d’un protocole simple : établissement, transfert segmenté et libération de connexions.

Développé en **C# (.NET, Visual Studio)** dans le cadre du cours **INF1009 – Réseaux I**.

---

## 🔌 1. Établissement de connexion

- Lecture automatique des demandes dans `S_lec.txt`
- Génération d’un identifiant unique (ID)
- Gestion des cas particuliers :
  - Refus fournisseur
  - Refus distant
  - Absence de réponse simulée
- Émission des primitives :
  - `N_CONNECT.req`
  - `N_CONNECT.ind`
  - `N_CONNECT.conf`

---

## 📤 2. Transfert de données segmenté

- Segmentation des messages en blocs de **128 caractères**
- Numérotation cyclique :
  - `PS` — prochain paquet envoyé  
  - `PR` — prochain paquet attendu
- Simulation réseau :
  - Acquittement positif
  - Acquittement négatif
  - Absence d’acquittement
  - Réémission automatique
- Écriture dans les fichiers :
  - `L_ecr.txt` (vers réseau)
  - `L_lec.txt` (réponses simulées)

---

## 🔚 3. Libération ou refus de connexion

- `N_DISCONNECT.req`
- `N_DISCONNECT.ind`
- Libération manuelle
- Nettoyage des connexions actives


