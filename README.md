# P2SoftwareProjekt
#### Lavet af
Daniel Vilslev <br />
Ida Thoft Christiansen <br />
Jakob Østenkjær Hansen <br />
Lena Said <br />
Leon Christensen <br />
Liv  Holm <br />
Nina Lyngsie Burmester  <br />

## Om programmet
Programmet er en web-applikation, lavet som et universitets 2.semester projekt, for at undersøge sandsynlighedsfordelingen under et spil Sænke slagskibe. Dette er gjort ved hjælp af et bayesiansk netværk med et API fra Hugin. 

#### dll-filer
For at programmet kan køre er det nødvendigt at have referencer til to forskellige dll-filer. Disse er navngivet HAPI.dll og netcore2.dll. For at benytte disse filer er det nødvendigt at kontakte Hugin Expert A/S.

## Vejledning til det kørende program
Hvis der ikke er andre som er i gang med at spille vil brugeren kunne påbegynde spillet. Brugeren skal først angive sit navn på startskærmen. Når dette er angivet er det muligt at placere skibe. Dette gøres ved at trykke på knappen med det skibsnavn der ønskes at placere. Herefter trykkes der på brættet for at placere skibet. Det er efterfølgende muligt at fjerne et eller alle skibe fra brættet med knapper. Når alle skibe er sat trykkes på “Done” for at begynde spillet. Når spillet er begyndt trykkes der på det blå bræt til venstre for at skyde på programmets bræt, hvorefter brugeren vil blive oplyst om der er ramt et skib eller ej. Til højre kan brugeren se hvor det Bayesianske netværk skyder. Ved at trykke på knappen “show AI-probabilities” vil det være muligt at se procentfordelingen for alle felter på brættet. Når spillet er vundet vil dette blive oplyst med en modal. Hvis brugeren er inaktiv i to minutter vil brugeren blive tvunget til at stoppe spillet og blive vist startskærmen igen. 

## Eksperimenter med det Bayesianske netværk
For at undersøge hvilke svagheder og styrker det Bayesianske netværk har, er netværket blevet testet mod forskellige skipsplaceringer. Herunder er programmet blevet kørt 5000 gange for at få et mere generelt resultat. Efter eksperimenter er udført fandt gruppen ud af at netværket klarer sig dårligst hvis alle skibene er sat i samme hjørne, da der er flest mulige skibsplaceringer i midten og ikke ved siden af de andre skibe. Det eksperiment hvor programmet klarede sig bedst var hvis skibene var placeret i midten af brættet dog uden af de berørte hinanden. Disse resultater stemmer overens med teori om det Bayesianske netværk og var derfor som forventet. 


