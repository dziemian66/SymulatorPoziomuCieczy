# SymulatorZbiornika (aplikacja WPF)
Jest to pierwsza większa aplikacja, którą wykonałem w ramach pracy magisterskiej. Celem pracy magisterskiej było wykonanie sprzętowo-programowego symulatora procesu
regulacji poziomu wody w zbiorniku. Symulator ten składa się z aplikacji komputerowej, która symuluje proces regulacji poziomu wody w zbiorniku oraz mikrokontroler, który mapuje dane procesu symulacji i na tej podstawie ustawia swoje wyjścia. Do wyjść tych podłączony jest np. sterownik PLC, który za pośrednictwem mikrokontrolera nadzoruje symulowany proces regulacji poziomu wody w zbiorniku.

Solucja wykonana została w całości w języku polskim. 

## Obsługa aplikacji symulatora
Symulator zbiornika z wodą został opracowany, aby umożliwić testowanie i uruchamianie procesów regulacji poziomu wody w zbiorniku. Graficznie przedstawiona aplikacja pozwala na podgląd działania zbiornika w zależności od sygnałów sterujących oraz zdefiniowanych parametrów. Wygląd interfejsu użytkownika przedstawiono na rysunku 4.27. Okno symulatora 
podzielone zostało na pięć głównych części:
• zbiornik;
• panel sterujący;
• ustawienia;
• wykresy;
• podgląd sygnałów.

![1](https://user-images.githubusercontent.com/99491279/223162654-430e22d5-ee82-45ee-9597-dd9c5e0e2edc.png)

Rysunek 4.28 zawiera symulowany zbiornik oraz jego urządzenia wykonawcze. Na zbiorniku znajdują się oznaczenia wskazujące wysokości słupa wody: maksimum, połowę oraz minimum. Podniesiony pływak (4) sygnalizuje przekroczenie połowy maksymalnej wartości poziomu wody oraz załączenie czwartego wyjścia cyfrowego. Natomiast do dokładniejszego pomiaru wykorzystywany jest czujnik poziomu cieczy (3). Zbiornik posiada również wyświetlacz ukazujący aktualną ilość cieczy w zbiorniku w litrach (5). Przepływ nalewający ukazuje wyświetlacz (1), jednakże, aby nalewanie było możliwe, wymagane jest wysterowanie zaworu oraz załączenie pompy (2). Pompa (6) wylewa wodę ze zbiornika zgodnie z ustawieniami użytkownika. Praca obydwu pomp symbolizowane jest przez delikatne drgania. Znajdująca się w zbiorniku rura przelewowa uniemożliwia przekroczenie maksymalnego poziomu wody.

![2](https://user-images.githubusercontent.com/99491279/223163233-c7a87fbd-7ff1-402d-9fbb-42a59955ce12.png)

Prosty panel sterujący znajdujący się w aplikacji ukazuje rysunek 4.29. Zawiera on dwa przyciski bistabilne NO (ang. Normally open) oraz lampki sygnalizacyjne o kolorze zielonym i czerwonym. Elementy te są dowolnie programowalne zgodnie z programem PLC. Użytkownik może również edytować nazwy znajdujące się nad nimi. Panel zawiera też przycisk bezpieczeństwa, którego naciśnięcie uniemożliwia pracę zbiornika (nalewanie oraz wylewanie wody) oraz działanie panelu sterującego. Przycisk bezpieczeństwa wirtualnie odłącza zasilanie procesu. Dopóty uniemożliwia jego sterowanie, dopóki przycisk ten nie powróci do pozycji wyjściowej. 

![3](https://user-images.githubusercontent.com/99491279/223163842-5672bead-7330-4b6f-94b7-d5c3d0f01ab1.png)

Wykresy wyświetlane wewnątrz aplikacji przedstawia rysunek 4.30. Ich podgląd odświeżany jest co jedną sekundę i umożliwia ukazanie zmian wartości z ostatnich stu sekund. Oś o kolorze niebieskim dotyczy poziomu cieczy w zbiorniku w centymetrach. Natomiast oś koloru czerwonego ukazuje nalewaną ciecz w mililitrach na sekundę. Dodatkowo wskazanie kursorem myszki na dany punkt wykresu umożliwia dokładny odczyt wartości w danym czasie.

![4](https://user-images.githubusercontent.com/99491279/223164198-94548d5f-1e4b-4061-a395-ce1a665b772e.png)

Ustawienia aplikacji (rysunek 4.32) umożliwiają zmianę parametrów komunikacji oraz symulacji. W komunikacji możliwe jest wybranie portu COM podłączonego do mikrokontrolera oraz prędkość w bitach na sekundę. W przypadku, gdy lista zawierająca porty jest pusta, a mikrokontroler został podłączony, należy nacisnąć przycisk Odśwież, który wczyta aktualną listę portów. Przyciski Połącz oraz Rozłącz służą do nawiązania komunikacji z mikrokontrolerem oraz jej zakończenia. Na podgląd aktualnego stanu komunikacji pozwala lampka Stan komunikacji, której jasnozielony kolor symbolizuje trwającą wymianę danych.

![5](https://user-images.githubusercontent.com/99491279/223164709-da93e4f5-fbde-4d7a-b193-0cd20bda15ad.png)

W ustawieniach możliwe jest również rozpoczęcie symulacji (przycisk Start) oraz jej zakończenie (przycisk Stop). Aby włączyć symulację wymagane jest wcześniejsze nawiązanie połączenia z mikrokontrolerem. W innym wypadku kontrolki dotyczące symulacji zostaną zablokowane. Lampka kontrolna (Stan symulacji) symbolizuje trwającą symulację, gdzie ciemnozielony kolor oznacza jej wyłączenie. Przycisk Reset służy do przywrócenie warunków początkowych zbiornika oraz usunięcia danych z wykresu. Natomiast przycisk Model zbiornika otwiera okno przedstawione na rysunku 4.33, pozwalające wpływać na parametry symulowanego zbiornika.

![6](https://user-images.githubusercontent.com/99491279/223165101-47bc047d-0be8-4979-83be-45a6320ed36c.png)
