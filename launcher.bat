cd /d "Serveur/ProjetBike/RoutingServer/bin/Debug"

powershell Start-Process -FilePath "ProxyCacheServeur.exe" -Verb RunAs
powershell Start-Process -FilePath "RoutingServer.exe" -Verb RunAs

cd /d "../../../../../HeavyClient"
cmd /c mvn exec:java -Dexec.mainClass="com.soap.ws.client.generated.Launcher"
cd /d ".."

echo "Programme fini. N'oubliez pas de fermer les fenetres du RoutingServer, et du Proxy."