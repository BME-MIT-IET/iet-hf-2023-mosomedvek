# Api mappa

Nem használt függvények mezők törlése, ifek mergelése, elnevezeséi konvenciókhoz való iagzodás, mind a kód olvashatósága és karbantarthatósága érdekében. 

![img](static_analysis/screenshots/false_error_naming.png)

Ebben az esetben hibásan jelzi a PascalCase megsértését.

# Bll

Nem használt lokális változók törlése, csak a konstruktorban beállított tagok readonly-vá és priváttá tétele.