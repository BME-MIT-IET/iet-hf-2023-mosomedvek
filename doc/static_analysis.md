# Technológia - Sonarlint 

A munka során használt bővítmény, a SonarLint for Visual Studio 2022
egyszerűen telepíthető és fájdalommentesen végezi a dolgát. Fejlesztőként a működése egyezik a fordító által generált warningok-kal, valamint integráltan kapunk egy részletezőt és rossz/jó példakódokat a warninggal kapcsolatban.


# Api mappa

Nem használt függvények mezők törlése, ifek mergelése, elnevezeséi konvenciókhoz való iagzodás, mind a kód olvashatósága és karbantarthatósága érdekében. 

![img](static_analysis/screenshots/false_error_naming.png)

Ebben az esetben hibásan jelzi a PascalCase megsértését.


# Bll

Nem használt lokális változók törlése, csak a konstruktorban beállított tagok readonly-vá és priváttá tétele. Paraméterek átnevezése, hogy egyezzen az interfész által deklarálttal. Kikommentezett kódok 
törlése. 

![Alt text](static_analysis/screenshots/remove_role.png)

RemoveRole, AddRoleként viselkedett, cseréltem őket és implementáltam a RemoveRolet.


# Dal

Csak a konstruktorban beállított tagok readonly-vá és priváttá tétele. Más hibát manuális átvizsgálással és solarlint segítségével ne mtaláltam.


# Összegzés

A projekt jól szervezett, jól struktúrált, igényesen készített munka. A sonarlint által jelzett értelmes hibákat javítottam, a felmerülő problémák nagyrésze, a kód olvashatóságával voltak kapcsolatosak(nem használt tagok, függvények). Ezek javítása hozzájárul a későbbi fejlesztés, kódkarbantartás, megértés, megkönnyítéséhez. A jelzett de nem javított hibák, hibásan próbálták a PascalCase-t kikényszeríteni, valamint a megmaradt todo commentek.

# Jövőbeni elvégzendő feladatok: 

A projekt jelen állása szerint van pár TODO, vigyázni kell ,hogy ezek valóban megvalósulásra kerüljenek/törlődjenek és ne csak felhalmozódjanak. Valamint lehetnének egy kicsit bőbeszédűebbek ezek a kommentek.

