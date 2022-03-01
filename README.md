# Sale System

## Wstęp

Projekt rozwijany jest w ramach przedmiotu Inżynieria Oprogramowania 2 na 6. semestrze studiów informatycznych. Z drobnymi korektami stanowi implementację tworzonej w czasie poprzedniego semestru specyfikacji systemu promocji.

## Role w systemie

W systemie przewidziano funkcjonowanie kilku ról:

* **osoba fizyczna** - zwykły użykownik,
* **przedsiębiorca** - zweryfikowany użytkownik mający specjalne uprawnienia,
* **administrator** - nadzorujący system.

W dalszym etapie przewidziano również rolę **moderatora** - pomocnika administratora o zredukowanych możliwościach zarządania.

## Wybrane możliwości uczestników systemu

**Funkcjonalności osoby fizycznej**:

* wyświetlanie scrollowalnej tablicy z promocjami,
* dodanie posta o znalezionej przez siebie promocji,
* edycja i usuwanie dodanych przez siebie postów,
* filtrowanie postów po kategoriach,
* komentowanie postów oraz lajkowanie postów i komentarzy innych użytkowników,
* edycja i usuwanie swoich komentarzy,
* edytowanie i usunięcie swojego profilu,
* subskrybowanie i odsubskrybowanie interesujących kategorii (funkcjonalność newslettera).

**Funkcjonalności przedsiębiorcy**:

* promowanie wybranych przez siebie postów (promowane posty są wyświetlane jako pierwsze na tablicy),
* dostęp do narzędzi typu Business Intelligence - przewidziane podczas dalszego rozwoju aplikacji.

**Funkcjonalności administratora**:

* nadanie użytkownikom statusu "aktywny",
* nadanie przedsiębiorcom statusu "zweryfikowany",
* możliwość usunięcia wybranego użytkownika,
* możliwość usuwania postów.

**Funkcjonalności moderatora** (opcjonalnie - przewidziane na dalszym etapie rozwoju systemu):

* usuwanie postów niezgodnych z regulaminem,
* czasowe zablokowanie użykownikom nagminnie łamiącym regulamin możliwości dodawania ogłoszeń.

## Uruchomienie

Projekt jest budowany w technologii ASP.NET Core 5.0. Może być otwarty za pomocą środowiska Visual Studio 2019 bądź programu Visual Studio Code (część frontendowa).

## Budowanie i testy

Projekt jest budowany i testowany po każdym commit'cie. Może być otwarty i skompilowany w środowisku Visual Studio 2019.

Całość można uruchomić poleceniem `docker-compose up -d --build` i wyłączyć poleceniem `docker-compose down`

## Zasady projektowe

Wszystkie dodawane funkcjonalności muszą być przetestowane. Wprowadzone zmiany nie mogą w żadnym stopniu naruszać już działających funkcjonalności. Gałąź "main" może zawierać jedynie w pełni przetestowany i działający kod.

## Autorzy

* Piotr Kryczka (https://github.com/piotrek999)
* Jakub Michalak (https://github.com/jmichalak9)
* Damian Opoka (https://github.com/Maxization)
* Wojciech Podmokły (https://github.com/ozel981)
* Adam Ryl (https://github.com/adassimo25)