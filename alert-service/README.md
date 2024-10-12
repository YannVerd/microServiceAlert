# alert-service

## Fonctionnement du Micro-Service Alert

    ## Front :
        Sur le Composant Dynamique Card, il y a une méthode toggleAlert() qui contient le code qui permet de récupérer:
            - Type : "annonce", "conversation", etc.. 
            - Name : titre, nom, ou autre champs qui décrit rapidement le type d'élément retourné
            - IdDMUser :  l'id de l'utilisateur (A l'heure actuelle l'id est vide). 
        Cette méthode créé une requete vers l'API Node.

    ## Back : 
        - La requete précédente emprunte la route "alert" et utilise le controleur alert-controller. 
        - Le controleur contient les méthode SubscribeAlert et UnsubscribeAlert.
        - Il n'y a, pour l'instant, pas la méthode "alertUpdated" car ce n'était pas clair quant au retour de cet évènement sur l'API node.
        - Chaque méthode instancie un producer pour envoyer au conteneur RabbitMQ le contenu de la requete du front pour qu'il l'intègre à la file d'attente adéquate.

    ## Micro_Service Alert: 
        - Les Consumers (initialisés dans le Program.cs) "ecoutent" la file d'attente de RabbitMQ qui leur est attribuée.
        - Lors de la réception d'un message, le Consumer fait appel au contrôleur "ManagmentEvent" et à la méthode qui correspond à la file d'attente
        - Dans le Controleur: 
            - La Methode CreateAndSubcribeAlert teste si une alert correspondant à l'id de l'élément existe déjà :
                - Si oui : il ajoute l'utilisateur à sa liste de diffusion (ajoute une Notify avec l'IdDMUser et l'IdAlert). 
                - Si non : il créé une EntityDetails avec l'id de l'élément (IdDMEntity), puis il créé l'élément Alert dans la bdd qui contiendra l'id de l'EntityDetails (IdENtityDetails). Et enfin il créé une Notify qui aura l'id de l'utilisateur (IdDMUser) et l'id de l'alert (IdAlert). La table Notify correspond à la liste de diffusion de l'alerte.
            - La Méthode UnsubscribeAlert supprime la Notify qui correspond a l'IdUSer et l'IdAlert.
            - La Méthode AlertUpdated retourne la liste des utilisateurs (dans Notify) qui correspond à l'id de l'élément (entity) modifié.
            - Tous les autres controleurs (NotifyController, EntityDetailsController, AlertController) ont étés conservés pour des utilisations futures (utilisation par un admin)

## Reste à faire

    - créer l’élément de notification sur le front
    - Renvoyer cet élément lors de l’évènement alertUpdated
    - Faire communiquer les consumers (receivers) avec les Contrôleur : 
        - J’ai essayé en faisant une redirection avec HttpClient sans succès 
        - J'ai essayé d’instancier directement le Contrôleur dans le consumer mais cela n’a pas marché non plus.
        - Le plus simple sera peut être de mettre en place une API de consumers pour l’ensemble des micro-services.

## Axes d'Améliorations / à revoir

    - Revoir les relations côté dotnet car j'ai changé les typages des modèles pour correspondre à ceux de l'API Node.
    - Utiliser les relations pour dans service EventService. Pour l’instant c’est du requêtage simple mais il y a moyen de faire plus optimisé

        

## 🚀 Installation du repository

```bash
# Installation du projet
git clone git@gitlab.com:dwwm_ern_sete_01/immo-ia/micro-services/alert-service.git
# ou
git clone https://gitlab.com/dwwm_ern_sete_01/immo-ia/micro-services/alert-service.git

```

## 📚 Installer les dépendances du projet

```bash
dotnet restore
```

Pour ajouter ou supprimer des dépendances, vous pouvez utiliser les commandes suivante:

```bash
# La version est facultative et sera par defaut la derniere
dotnet add package <nom-package> --version <version>

dotnet remove package <nom-package>
```

## 🏁 Lancer le service

Pour un _lancement rapide_ afin tester localement vos modification:

```bash
dotnet run
```

Depuis un fichier executable compilé à partir du code source:

```bash
# Créer l'éxecutable dans /bin
dotnet build

# Executer le service compilé
dotnet ./bin/Debug/net8.0/alert-service.dll
```

## 🧪 Lancer les tests unitaires

```bash
dotnet test
```