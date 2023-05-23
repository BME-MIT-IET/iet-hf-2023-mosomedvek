# Manuális tesztek
Tesztek elvégzéséhez szükséges a program futtatásához megfelelő környezet(nekem VS Code),ami képes C# és .Net futtatására. Ezen kivül PosgreSQL adatbázis. Adatbázishoz való kapcsolódás beállítása az “appsettings.json”-ban fájlban található. Ezeken kívül fontos szerepet fog még játszani a futó program konzolja.

A teszteket Postman-ben is végre lehet hajtani, de a szoftverhez van Swagger UI, amin keresztül egyszerübb a tesztelés elvégzése. Ezeken kívül fontos szerepet fog még játszani a futó program konzolja, egyes tesztek esetén innét fontos információt kell kinyernünk.

Swagger URL: https://localhost:7258/swagger/index.html.Vagy más port esetén a konzolból lehet kivenni.

Az API teszteknek az alapját is ez a localhost cím fogja jelenteni.

## 1. Tests without Login

Jelenleg a kód átirányítás miatt 404 kódot dob 401 helyett. 

| Test Case ID  |  Test Steps |  Expected Results  |  Actual Results | Status |
|---|---|---|---|---|
| 1.  |  Try out GET /api/User |  404 |  404 |  Expected |
| 2.  |  Try out GET /api/Class |  404 |  404 |  Expected |
| 3.  |  Try out GET /api/User/1 |  404 |  404 |  Expected |


## Innéttől mindegyik teszt elött szükséges a bejelentkezés a következő módon.


POST /api/User/Login -t használva a következő request body-val:

    {
    "email": "admin@grip.sytes.net",
    "password": "Admin123!"
    }

Válasznak 200-as kódot várunk a felhasználó adataival.


## 2. Tests with Login

| Test Case ID  |  Test Steps |  Expected Results  |  Actual Results | Status |
|---|---|---|---|---|
| 4.  |  Try out GET /api/User |  200 <br> Users in the database in JSON format |  200 <br> Users in the database in JSON format |  Pass |
| 5.  |  Try out GET /api/Class |  200 <br> Classes in the database in JSON format |  200 <br> Classes in the database in JSON format |  Pass |
| 6.  |  Try out GET /api/User/1 |  200 <br> Classes in the database in JSON format |  200 <br> Classes in the database in JSON format |  Pass |

## 3. Registration and Delete User tests

Az első lépés estén 500 hiba ellenére az user elmetődik az adatbázisba. A probléma abból adódik, hogy a megadott email nem igazi és nem tudja rá az üzenetet elküldeni.

| Test Case ID  |  Test Steps |  Expected Results  |  Actual Results | Status |
|---|---|---|---|---|
| 7.  |  Use POST /api/USER with <br> {<br>"email":   "test@gmail.com",<br>"name": "testelek"<br>}|  500 |  500 |  Pass |
| 7.  | Get the activation token(ActivationToken) from Console:<br>„New user testelek (test@gmail.com) created by admin with activation token:” <br> Get the new user id(userID) from database or use GET /api/User 
 | 7.  |  Use GET /api/User/{userID}|  200<br>{<br>"id": {userID},<br>"email": "test@gmail.com",<br> "userName": "testelek",<br>**"emailConfirmed": false**<br>}|  200<br>{<br>"id": {userID},<br>"email": "test@gmail.com",<br> "userName": "testelek",<br>**"emailConfirmed": false**<br>}|  Pass |
 | 7.  |  Use POST /api/ConfirmEmail with <br> {<br>"email":   "test@gmail.com",<br>"token": {ActivationToken} <br> "password": "Test123!" <br>}|  500 |  500 |  Pass |
 | 7.  |  Use GET /api/User/{userID}|  200<br>{<br>"id": {userID},<br>"email": "test@gmail.com",<br> "userName": "testelek",<br>**"emailConfirmed": true**<br>}|  200<br>{<br>"id": {userID},<br>"email": "test@gmail.com",<br> "userName": "testelek",<br>**"emailConfirmed": true**<br>}|  Pass |
 | 8.  |  Use DELETE /api/User/{userId}|  200 |  200 |  Pass |
 | 8.  |  Use GET /api/User/{userId}|  404<br>„title” : „User not found.”|  404<br>„title” : „User not found.” |  Expected |

## Következő tesztekhez szükséges új userek felvétele

A "userId" az adatbázisban létrehozott felhasználók egyik id-ja. A következő legyen az egyik user akit létrehozunk.

    {
    "email":  "test@gmail.com",
    "name": "testelek"
    }



## 4. Add and Remove Role

Not yet implemented.

| Test Case ID  |  Test Steps |  Expected Results  |  Actual Results | Status |
|---|---|---|---|---|
| 9.  |  USe POST /api/User/AddRole/{userId}/{roleId} |  200| |   |

## 5. Get student and Search

| Test Case ID  |  Test Steps |  Expected Results  |  Actual Results | Status |
|---|---|---|---|---|
| 10.  |  Use GET /api/Student/{userId} |  200 <br> Student id, email,username, absences in JSON format| 200 <br> Student id, email,username, absences in JSON format| Pass  |
| 11.  |  Use GET /api/Student/Search <br>Parameters: name: test |  200 <br> Student id and username in JSON format| 200 <br> Student id and username in JSON format| Pass  |

## 6. Group tests

Create, Add user, Get , Update, Delete

| Test Case ID  |  Test Steps |  Expected Results  |  Actual Results | Status |
|---|---|---|---|---|
| 12.  |  Use POST /api/Group<br>{<br>"id": 100,<br>"name": "testGroup"<br>}|  201 <br> Class id and name in JSON format| 201 <br> Class id and name in JSON format| Pass  |
| 13.  |  Use PATCH /api/Group/100/AddUser/{userId}|  200 | 500 <br> „detail” : ”Object reference not set to an instance of an object.”| Pass  |
| 14.  |  Use GET /api/Group/100 |  200 <br> Class id and name in JSON format| 200 <br> Class id and name in JSON format| Pass  |
| 15.  |  Use PUT /api/Group/100<br>{<br>"id": 100,<br>"name": "groupTest"<br>}|  204| 204 | Pass   |
| 16.  |  Use DELETE /api/Group/100|  204| 204 | Pass  |

## 7. Create Passive Tag, Attend with it then delete

A teszthez szükséges készíteni egy Station. Ez az adatbáziskezelőben tudjuk legegyszerűbben megtenni. A Station Id-ja legyen 1.

| Test Case ID  |  Test Steps |  Expected Results  |  Actual Results | Status |
|---|---|---|---|---|
| 17.  |  Use POST /api/PassiveTag<br>{<br>"serialNumber": 1234,<br>"userId": 1<br>}|  201 <br> The tag id, serialNumber and user data in JSON format| 201 <br> The tag id, serialNumber and user data in JSON format| Pass  |
| 18.  |  Use POST /api/Attendance/passive <br> Parameter:<br> <{apiKey}><br>Body:<br>{<br>"stationId": 1,<br>"serialNumber": 1234<br>}|  200| 200| Pass  |
| 19.  |  Use DELETE /api/PassiveTag/1234|  204| 204| Pass  |