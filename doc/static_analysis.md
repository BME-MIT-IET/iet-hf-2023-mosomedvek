# Api mappa

Nem használt függvények mezők törlése, ifek mergelése, elnevezeséi konvenciókhoz való iagzodás, mind a kód olvashatósága és karbantarthatósága érdekében. 

![img](static_analysis/screenshots/false_error_naming.png)

Ebben az esetben hibásan jelzi a PascalCase megsértését.

# Bll

Nem használt lokális változók törlése, csak a konstruktorban beállított tagok readonly-vá és priváttá tétele. Paraméterek átnevezése, hogy egyezzen az interfész által deklarálttal. Kikommentezett kódok 
törlése. 

![Alt text](static_analysis/screenshots/remove_role.png)

RemoveRole, AddRoleként viselkedett, cseréltem őket és implementáltam a RemoveRolet.