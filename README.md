# Clément REMY

## Présentation et remerciements

Voici le README du projet "Let's go biking", réalisé par moi, Clément REMY, dans le cadre d'un projet scolaire. 

## Todo-List

### Fonctionnalités implémentées :

- **Self Hosted Routing Server**, un projet C# qui a une méthode getItinerary qui, à partir de deux strings (nom de ville/village, nom de lieu ou adresse précise), va calculer les itinéraires à pied et en vélo et retourner la liste d'itinéraires la plus courte en matière de temps. Le RS communique en REST avec les API OpenRouteService et Nominatim. Fonctionne avec des exécutables.
- **Proxy/Cache**, un projet C# qui communique en SOAP avec le RS et en REST avec l'API JCDeceaux pour retrouver les informations sur les contrats et stations de vélo
- **HeavyClient java** : un client java qui récupère les itinéraires trouvés par le RS et qui les affichent, soit via ActiveMQ soit via le retour de la méthode getItinerary si ActiveMQ est inaccessible.
- **Communication via ActiveMQ V1** : le serveur crée une queue que le client consomme. Cependant, l'affichage est moins beau car il ne supporte pas les caractères spéciaux, j'ai essayé de régler le souci, sans succès.

## Adresses intéressantes à rentrer

**IMPORTANT** : ne pas mettre de caractères spéciaux dans les noms d'adresses, depuis que j'ai implémenté ActiveMQ les caractères spéciaux font planter le programme.

- Campus SophiaTech - Gare Routiere Sophia : parfait pour tester un trajet relativement court (~ 45 minutes à pied) + noms précis de lieux
- Lyon - Mulhouse : deux villes relativement proches l'une de l'autre à l'échelle de la France, avec des stations JCDeceaux
- 71 Rue Robert Desnos - 19 Rue au Bois, 57000, Metz : intéressant pour voir qu'on doit beaucoup marcher avant de trouver une station JC Deceaux + noms précis d'adresses
- Gare du Nord - Auxerre : gros trajet sans contrat, énormément de marche

## Comment lancer et se servir du projet ?

0. **(OPTIONNEL)** dans le dossier bin de votre installation d'activemq, ouvrez un terminal, et lancez la commande "activemq start". Si vous n'avez pas activemq, pas de panique, le projet est fait pour fonctionner avec ou sans activemq.
1. A la racine du projet, double-cliquer sur le fichier "launcher.bat", qui lancera à tour de rôle :
	- le fichier ```ProxyCacheServeur.exe```, correspondant à l'exécutable du projet C# qui fait office de Proxy et de Cache. 
	- le fichier ```RoutingServer.exe```, correspondant à l'exécutable du projet C# qui fait office de Self Hosted Routing Service. A noter que le .bat est configuré de sorte à ce que les deux exécutables se lancent en mode administrateur, afin de permettre l'ouverture des ports sans soucis.
	- le main "Launcher" du heavy client java, via la commande mvn exec:java. 

	A noter que l'étape 1 est réalisable également de cette manière : ouvrez un terminal à la racine du projet, et écrivez ceci dans un terminal : ".\launcher.bat". De cette manière, le terminal crée par le .bat ne se fermera pas de suite une fois le programme fini, ce qui est pratique pour voir le log qui précise que c'est à l'utilisateur de fermer les terminaux lancés par les deux fichiers .exe.

2. Entrez deux adresses / ville / nom de lieu. Après cela, le trajet, étape par étape, sera affiché dans la console. A savoir que l'affichage est bien plus clair si l'appel est fait sans activeMQ (les caractères spéciaux sont supportés et le temps du trajet apparaît à la fin.)
Si cela ne marche pas ainsi, vous pouvez essayer de :
- Lancer la partie C# en double cliquant directement sur les exécutables, trouvables ici : ```Serveur/ProjetBike/RoutingServer/bin/Debug``` , et le HeavyClient via un IDE. Au sein du HeavyClient, il faut lancer le main de la classe Launcher.java, trouvable dans le package ```com.soap.ws.client.generated```
- Lancer la partie C# via Visual Studio.
