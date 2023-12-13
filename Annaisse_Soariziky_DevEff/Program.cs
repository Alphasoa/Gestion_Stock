// See https://aka.ms/new-console-template for more information


using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
class Gestion_des_Stocks
{
    
   
    private List<Queue<string>> stock = new List<Queue<string>>();
    private List<Queue<string>> alertes = new List<Queue<string>>();
    private List<Stack<string>> paquet = new List<Stack<string>>();

    private List<string> alertesBasStock = new List<string>();
    private Stack<string> Colis_now;
    const int limiteAlertes = 4;


    #region gestion des stocks
    public void Gestion_des_stocks()
    {
       Concatenation_Prod();
       Menu();
    }
 #endregion


    #region menu
    private void Menu()
        {
            // Initialiser le stock
            
            while (true)
            {
                Console.WriteLine("MENU");
                Console.WriteLine("1. Saisir stocks");
                Console.WriteLine("2. Saisir colis");
                Console.WriteLine("3. Quitter");

                Console.Write("Choisissez une option : ");
                string choix = Console.ReadLine();

                switch (choix)
                {
                    case "1":
                        Affichage_stockage();
                        Saisie_Ajout_Produit();
                        Generer_Alerte_Bas_Stock(); // Générer les alertes bas stock
                        Print_alertesBasStock(); // Afficher la liste d'alertes bas stock
                        Gestion_Alerte_Pleine();
                    break;

                    case "2":
                    Affichage_stockage();
                    Generer_Alerte_Bas_Stock(); // Générer les alertes bas stock
                    Print_alertesBasStock(); // Afficher la liste d'alertes bas stock
                    Creer_Colis();
                    Console.Write("Choississez les produits que vous allez prendre");
                    Console.WriteLine();
                    ColisDisponibleEtPlein();
                    Ajouter_Produits_Au_Colis();
                    Retirer_Produits_Colis_Du_Stock();
                    Affichage_stockage();
                    break;

                    case "3":
                        Console.WriteLine("Au revoir !");
                        Environment.Exit(0);
                        break;

                    default:
                        Console.WriteLine("Option non valide. Veuillez réessayer.");
                        break;
                }
            }
        }
    #endregion


   

    #region initialisation du type du produit
    private List<string> Set_TypeProd()
    {
        // Fonction pour fournir les types de produits (A, B, C)
        return new List<string> { "A", "B", "C" };
    }
    #endregion


    #region initialisation du volume du produit
    private List<string> Set_VolumeProd()
    {
        // Fonction pour fournir les volumes de produits (1, 2, 3, 4)
        return new List<string> { "1", "2", "3", "4" };
    }
    #endregion

    #region création du produit
    private void Concatenation_Prod()
    {
        List<string> types = Set_TypeProd();
        List<string> volumes = Set_VolumeProd();

        foreach (var type in types)
        {
            foreach (var volume in volumes)
            {
                for (int i = 0; i < 3; i++)
                {
                    string concatenatedValue = type + volume;

                    // Ajouter la valeur concaténée à la file correspondante dans stock
                    Stock_initial(concatenatedValue);
                }
            }
        }
    }
    #endregion

    #region hébergement des produits
    private void Stock_initial(string value)
    {
        // Si la liste stock est vide, ajoute une nouvelle file
        if (stock.Count == 0)
        {
            stock.Add(new Queue<string>());
        }

        // Ajoute la valeur à la dernière file dans la liste stock
        stock[stock.Count - 1].Enqueue(value);
    }
    #endregion

    #region affichage stock
    private void Affichage_stockage()
    {
        int fileIndex = 1;

        foreach (var file in stock)
        {
            Console.Write($"Stock {fileIndex}: ");

            foreach (var item in file)
            {
                Console.Write($"{item} ");
            }

            Console.WriteLine(); // Nouvelle ligne après chaque file
            fileIndex++;
        }
    }
    #endregion

    #region saisie et ajout de produit
    public void Saisie_Ajout_Produit()
    {
        Console.WriteLine("Saisissez le produit à ajouter au stock (par exemple, A1, B3, C2) : ");
        string saisie = Console.ReadLine();

        if (Bonne_Saisie(saisie))
        {
            Stock_initial(saisie);
            Console.WriteLine($"Le produit {saisie} a été ajouté au stock avec succès.");
        }
        else
        {
            Console.WriteLine("Saisie invalide. Assurez-vous que la saisie est une lettre majuscule suivie d'un nombre.");
        }
    }

    private bool Bonne_Saisie(string saisie)  // fonction en plus
    {
        // Utiliser une expression régulière pour valider la saisie (lettre majuscule suivie d'un nombre)
        Regex regex = new Regex(@"^[A-Z]\d$");
        return regex.IsMatch(saisie);
    }
    #endregion

    #region générer des alertes
 
    private void Verif_Stock()
    {
        Console.WriteLine("Vérification du stock :");

        // Utiliser deux listes pour stocker les produits et leurs quantités
        List<string> produits = new List<string>();
        List<int> quantites = new List<int>();

        foreach (var file in stock)
        {
            foreach (var produit in file)
            {
                int i = produits.IndexOf(produit);

                if (i != -1)
                {
                    // Si le produit existe, incrémenter la quantité correspondante
                    quantites[i]++;
                }
                else
                {
                    // Sinon, initialiser la quantité à 1 (le produit est ajouté à la liste lors de la première occurrence)
                    produits.Add(produit);
                    quantites.Add(1);
                }
            }
        }

        for (int i = 0; i < produits.Count; i++)
        {
            Console.WriteLine($"Produit {produits[i]} - Quantité en stock : {quantites[i]}");
        }
    }




    private int QuantiteProduit(string produit) // fonction en plus
     {
         if (int.TryParse(produit.Substring(1), out int quantite))
         {
             return quantite;
         }
         return 0;
     }



    private void Generer_Alerte_Bas_Stock()
    {
        Verif_Stock();

        Console.WriteLine("Génération d'alertes pour bas stock :");

        foreach (var file in stock)
        {
            var groupedProducts = file.GroupBy(p => p);

            foreach (var group in groupedProducts)
            {
                if (group.Count() < 2)
                {
                    Ajouter_Alerte(group.Key);
                    Console.WriteLine($"Alerte : Stock bas pour le produit {group.Key} (Quantité : {group.Count()} unité(s) en stock).");
                }
            }
        }

        Gerer_Limite_AlertesBasStock(); // Gérer la limite après avoir généré les alertes
    }




    private void Ajouter_Alerte(string produit)
    {
        string alerte = $"Alerte : Bas stock détecté pour le produit {produit}.";
        Console.WriteLine($"Alerte ajoutée : {alerte}");

        // Ajouter le produit à la liste d'alertes de bas stock
        alertesBasStock.Add(produit);
    }

   private void Gerer_Limite_AlertesBasStock()
{
    // Vérifier si le nombre d'alertes bas stock atteint la limite (3)
    if (alertesBasStock.Count >= limiteAlertes)
    {
        // Réapprovisionner le stock avec le produit de l'alerte
        Reapprovisionnement_Stock();
    }
}

    private void Reapprovisionnement_Stock()
    {
        foreach (var alerte in alertesBasStock)
        {
            // Réapprovisionner le stock avec le produit de l'alerte
            Stock_initial(alerte);
            Console.WriteLine($"Réapprovisionnement du produit {alerte} effectué.");
        }

        // Effacer les alertes bas stock après réapprovisionnement
        alertesBasStock.Clear();
    }

    private void Print_alertesBasStock()
    {
        Console.WriteLine("Produits en bas de stock :");
        foreach (var produit in alertesBasStock)
        {
            Console.WriteLine($"Produit en bas de stock : {produit}");
        }

        // Ajout du message pour le retrait d'alerte
        if (alertesBasStock.Count >= limiteAlertes)
        {
            string alerteRetiree = alertesBasStock[0];
            Console.WriteLine($"Retrait de l'alerte bas stock la plus ancienne : {alerteRetiree}");
        }
    }

    private void Verif_Alertes()
{
        Console.WriteLine("Vérification des alertes :");

        foreach (var alerte in alertesBasStock)
        {
            Console.WriteLine($"Vérification de l'alerte : {alerte}");
        }
    }

    #endregion

    #region gestion des alertes pleines
    public void Gestion_Alerte_Pleine()
    {
        if (alertes.Count >= limiteAlertes)
        {
            Console.WriteLine("Le seuil d'alerte est sur le point d'être atteint.");

            // Réapprovisionner le stock avec les produits des alertes existantes
            Reapprovisionnement_Stock();

            // Effacer les alertes existantes
            alertes.Clear();
        }
    }

    
    #endregion

    #region affichage des alertes
    public void Print_alert()
    {
        Console.WriteLine("Produits en alerte :");

        foreach (var alerte in alertes)
        {
            foreach (var produit in alerte)
            {
                Console.WriteLine($"Produit en alerte : {produit}");
            }
        }
    }

    #endregion

    #region création d'un colis
    private void Creer_Colis()
    {
        // Créer une nouvelle pile (colis)
        Stack<string> colis = new Stack<string>();

        // Ajouter le colis à la liste des paquets
        paquet.Add(colis);

        // Afficher un message annonçant la création du colis avec un nom unique
        int numeroColis = paquet.Count;
        Console.WriteLine($"Colis {numeroColis} créé avec succès.");
    }
    #endregion

    #region disponibilité d'un colis
    private bool ColisDisponibleEtPlein()
    {
        // Vérifier si des colis sont disponibles
        if (paquet.Count == 0)
        {
            Console.WriteLine("Aucun colis disponible. Veuillez créer un colis.");
            return false;
        }

        // Sélectionner le dernier colis (le plus récent)
        Stack<string> colisActuel = paquet[paquet.Count - 1];

        // Vérifier si le colis est plein (a atteint la limite de 5 produits)
        if (colisActuel.Count >= 5)
        {
            Console.WriteLine("Le colis est plein. Veuillez créer un nouveau colis.");
            return false;
        }

        return true;
    }
    #endregion

    private List<string> Trier_Produits_Par_Volume(List<string> produits)
    {
        // Utiliser une fonction de tri personnalisée en spécifiant un comparateur
        produits.Sort((produit1, produit2) => Comparer_Volume(produit1, produit2));

        return produits;
    }

    private int Comparer_Volume(string produit1, string produit2)
    {
        // Extraire les chiffres du nom du produit
        int volume1 = int.Parse(Regex.Match(produit1, @"\d+").Value);
        int volume2 = int.Parse(Regex.Match(produit2, @"\d+").Value);

        // Comparer les volumes
        return volume1.CompareTo(volume2);
    }

    #region ajout des produits dans le colis
    private void Ajouter_Produits_Au_Colis()
    {
        if (!ColisDisponibleEtPlein())
        {
            return;
        }

        Stack<string> colisActuel = paquet.LastOrDefault(); // Utiliser Linq pour obtenir le dernier colis

        const int nombreSaisies = 5; // Nombre de produits à saisir
        List<string> saisies = new List<string>();

        for (int i = 0; i < nombreSaisies; i++)
        {
            Console.WriteLine($"Saisissez le produit {i + 1} à ajouter au colis : ");
            string saisie = Console.ReadLine();

            if (Bonne_Saisie(saisie) && StockContientProduit(saisie))
            {
                saisies.Add(saisie);
                Console.WriteLine($"Produit {saisie} ajouté à la liste des saisies.");
            }
            else
            {
                Console.WriteLine("Saisie invalide ou produit non trouvé dans le stock. Assurez-vous que la saisie est correcte.");
                i--; // Décrémenter i pour refaire la saisie du même indice
            }
        }

        // Trier les produits par volume et les ajouter au colis
        Trier_Produits_Par_Volume(saisies).ForEach(produit =>
        {
            colisActuel.Push(produit);
            Console.WriteLine($"Produit {produit} ajouté au colis.");
        });

        // Afficher le contenu actuel du colis
        Afficher_Colis_Actuel();
    }
    #endregion

    #region Présence produit dans stock
    private bool StockContientProduit(string produit)
    {
        // Vérifier si le produit est présent dans le stock
        foreach (var file in stock)
        {
            if (file.Contains(produit))
            {
                return true;
            }
        }
        return false;
    }

    private void Saisir_Produits_Apres_Ajout_Au_Colis()
    {
        Console.WriteLine("Saisissez de nouveaux produits à ajouter au stock (par exemple, A1, B3, C2) : ");
        string saisie = Console.ReadLine();


        if (Bonne_Saisie(saisie))
        {
            Ajouter_Produits_Au_Stock(saisie);
            Console.WriteLine($"Le produit {saisie} a été ajouté au stock avec succès.");
        }
        else
        {
            Console.WriteLine("Saisie invalide. Assurez-vous que la saisie est une lettre majuscule suivie d'un nombre.");
        }
    }
    #endregion

    #region ajout produit au stock
    private void Ajouter_Produits_Au_Stock(string produit)
    {
        // Ajouter le produit au stock
        Stock_initial(produit);
    }

    private void Retirer_Produits_Colis_Du_Stock()
    {
        if (Colis_now == null)
        {
            Console.WriteLine("Aucun colis disponible.");
            return;
        }

        foreach (var produitColis in Colis_now)
        {
            Retirer_Produit_Du_Stock(produitColis); // Utiliser la fonction pour retirer chaque produit du stock
        }
    }
    #endregion

    private void Retirer_Produit_Du_Stock(string produit)
    {
        foreach (var file in stock)
        {
            if (file.Contains(produit))
            {
                // Retirer le produit de la file
                file.Dequeue();

                Console.WriteLine($"Produit {produit} retiré du stock.");
                break; // Sortir de la boucle interne après avoir retiré le produit
            }
        }
    }

    #region affichage du colis
    private void Afficher_Colis_Actuel()
    {
        Console.WriteLine("Contenu actuel du colis :");

        if (paquet.Count > 0)
        {
            Stack<string> colisActuel = paquet[paquet.Count - 1];
            foreach (var produit in colisActuel)
            {
                Console.Write($"{produit} ");
            }


            Console.WriteLine(); // Nouvelle ligne après l'affichage du colis
        }
        else
        {
            Console.WriteLine("Aucun colis disponible.");
        }
    }
    #endregion

    #region main
    static void Main(string[] args)
    {
        Gestion_des_Stocks stockManager = new Gestion_des_Stocks();
        // Utiliser la nouvelle méthode pour gérer les stocks
        stockManager.Gestion_des_stocks();
    }
    #endregion
}
