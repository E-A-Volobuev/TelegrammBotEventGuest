# TelegrammBotEventGuest

![основная страница](https://github.com/E-A-Volobuev/TelegrammBotEventGuest/blob/master/%D0%B3%D0%BB%D0%B0%D0%B2%D0%BD%D0%B0%D1%8F.png)

## Описание функционала 
Для взаимодействия с API Telegramm используется библиотека Telegram.Bot.
В приложении применена многоуровневая архитектура и механизм внедрения зависемостей LightInject, встраиваемая бд SQLite, взаимодействие с бд через Microsoft.EntityFrameworkCore.

## Принцип работы
В боте задействован мехнизм распределения ролей пользователей.
Вид главного меню администратора:

![админ главная](https://github.com/E-A-Volobuev/TelegrammBotEventGuest/blob/master/%D0%B0%D0%B4%D0%BC%D0%B8%D0%BD%20%D0%B3%D0%BB%D0%B0%D0%B2%D0%BD%D0%B0%D1%8F.png)

Вид главного меню обычного пользователя:

![юзер главная](https://github.com/E-A-Volobuev/TelegrammBotEventGuest/blob/master/%D0%BE%D0%B1%D1%8B%D1%87%D0%BD%D1%8B%D0%B9%20%D0%BF%D0%BE%D0%BB%D1%8C%D0%B7%D0%BE%D0%B2%D0%B0%D1%82%D0%B5%D0%BB%D1%8C.png)


## Просмотр событий и регистрация на события

Для просмотра и регистрации на события , кликаем на кнопку "Просмотр событий" и выбираем месяц:

![просмотр месяц](https://github.com/E-A-Volobuev/TelegrammBotEventGuest/blob/master/%D0%BF%D1%80%D0%BE%D1%81%D0%BC%D0%BE%D1%82%D1%80%20%D1%81%D0%BE%D0%B1%D1%8B%D1%82%D0%B8%D0%B9%20.png)

![просмотр события](https://github.com/E-A-Volobuev/TelegrammBotEventGuest/blob/master/%D0%BF%D1%80%D0%BE%D1%81%D0%BC%D0%BE%D1%82%D1%80%20%D1%81%D0%BE%D0%B1%D1%8B%D1%82%D0%B8%D0%B9%201%20.png)

Для регистрации на событие кликаем "Зарегистрироваться"

![регистрация](https://github.com/E-A-Volobuev/TelegrammBotEventGuest/blob/master/%D1%80%D0%B5%D0%B3%D0%B8%D1%81%D1%82%D1%80%D0%B0%D1%86%D0%B8%D1%8F.png)

Если в указанные месяц событий нет , то пользователь получит уведомление :

![событий нет](https://github.com/E-A-Volobuev/TelegrammBotEventGuest/blob/master/%D0%BF%D1%80%D0%BE%D1%81%D0%BC%D0%BE%D1%82%D1%80%20%D1%81%D0%BE%D0%B1%D1%8B%D1%82%D0%B8%D0%B9%20%D0%BD%D0%B5%D1%82.png)

## Просмотр мероприятий, на которые уже зарегистрирован пользователь и отмена регистрации

Для просмотра мероприятий , на которые зарегистрирован пользователь, необходимо кликнуть кнопку "Мои события":

![мои события](https://github.com/E-A-Volobuev/TelegrammBotEventGuest/blob/master/%D0%BC%D0%BE%D0%B8%20%D1%81%D0%BE%D0%B1%D1%8B%D1%82%D0%B8%D1%8F.png)

Для отмены регистрации необходимо кликнуть на соответствующую кнопку:

![отмена регистрации](https://github.com/E-A-Volobuev/TelegrammBotEventGuest/blob/master/%D0%BE%D1%82%D0%BC%D0%B5%D0%BD%D0%B0%20%D1%80%D0%B5%D0%B3%D0%B8%D1%81%D1%82%D1%80%D0%B0%D1%86%D0%B8%D0%B8.png)

Если пользователь не зарегистрирован ни на одно событие, то он получит уведомление:

![моих событий нет](https://github.com/E-A-Volobuev/TelegrammBotEventGuest/blob/master/%D0%BC%D0%BE%D0%B8%D1%85%20%D1%81%D0%BE%D0%B1%D1%8B%D1%82%D0%B8%D0%B9%20%D0%BD%D0%B5%D1%82.png)

## Создание событий

Кликаем "Создание события" и следуем инструкции

![создание событий](https://github.com/E-A-Volobuev/TelegrammBotEventGuest/blob/master/%D1%81%D0%BE%D0%B7%D0%B4%D0%B0%D0%BD%D0%B8%D0%B5%20%D1%81%D0%BE%D0%B1%D1%8B%D1%82%D0%B8%D1%8F.png)

![создание событий1](https://github.com/E-A-Volobuev/TelegrammBotEventGuest/blob/master/%D1%81%D0%BE%D0%B7%D0%B4%D0%B0%D0%BD%D0%B8%D0%B5%20%D1%81%D0%BE%D0%B1%D1%8B%D1%82%D0%B8%D1%8F%201.png)

![создание событий2](https://github.com/E-A-Volobuev/TelegrammBotEventGuest/blob/master/%D1%81%D0%BE%D0%B7%D0%B4%D0%B0%D0%BD%D0%B8%D0%B5%20%D1%81%D0%BE%D0%B1%D1%8B%D1%82%D0%B8%D1%8F%202.png)

Структура хранения картинок в БД выглядит вот так:

![создание событий3](https://github.com/E-A-Volobuev/TelegrammBotEventGuest/blob/master/%D1%81%D0%BE%D0%B7%D0%B4%D0%B0%D0%BD%D0%B8%D0%B5%20%D1%81%D0%BE%D0%B1%D1%8B%D1%82%D0%B8%D1%8F%203.png)

## Редактирование событий

Выбираем событие и следуем инструкции

![редактирование](https://github.com/E-A-Volobuev/TelegrammBotEventGuest/blob/master/%D1%80%D0%B5%D0%B4%D0%B0%D0%BA%D1%82%D0%B8%D1%80%D0%BE%D0%B2%D0%B0%D0%BD%D0%B8%D0%B5%20%D1%81%D0%BE%D0%B1%D1%8B%D1%82%D0%B8%D1%8F.png)

Редактирование названия мероприятия, описания и даты происходит однотипно:

![редактирование1](https://github.com/E-A-Volobuev/TelegrammBotEventGuest/blob/master/%D1%80%D0%B5%D0%B4%D0%B0%D0%BA%D1%82%D0%B8%D1%80%D0%BE%D0%B2%D0%B0%D0%BD%D0%B8%D0%B5%20%D1%81%D0%BE%D0%B1%D1%8B%D1%82%D0%B8%D1%8F%201.png)

![редактирование2](https://github.com/E-A-Volobuev/TelegrammBotEventGuest/blob/master/%D1%80%D0%B5%D0%B4%D0%B0%D0%BA%D1%82%D0%B8%D1%80%D0%BE%D0%B2%D0%B0%D0%BD%D0%B8%D0%B5%20%D1%81%D0%BE%D0%B1%D1%8B%D1%82%D0%B8%D1%8F%202.png)

Редактирование обложки мероприятия:

![редактирование3](https://github.com/E-A-Volobuev/TelegrammBotEventGuest/blob/master/%D1%80%D0%B5%D0%B4%D0%B0%D0%BA%D1%82%D0%B8%D1%80%D0%BE%D0%B2%D0%B0%D0%BD%D0%B8%D0%B5%20%D0%BE%D0%B1%D0%BB%D0%BE%D0%B6%D0%BA%D0%B8%20%D1%81%D0%BE%D0%B1%D1%8B%D1%82%D0%B8%D1%8F.png)

![редактирование4](https://github.com/E-A-Volobuev/TelegrammBotEventGuest/blob/master/%D1%80%D0%B5%D0%B4%D0%B0%D0%BA%D1%82%D0%B8%D1%80%D0%BE%D0%B2%D0%B0%D0%BD%D0%B8%D0%B5%20%D0%BE%D0%B1%D0%BB%D0%BE%D0%B6%D0%BA%D0%B8%20%D1%81%D0%BE%D0%B1%D1%8B%D1%82%D0%B8%D1%8F%201.png)

![редактирование5](https://github.com/E-A-Volobuev/TelegrammBotEventGuest/blob/master/%D1%80%D0%B5%D0%B4%D0%B0%D0%BA%D1%82%D0%B8%D1%80%D0%BE%D0%B2%D0%B0%D0%BD%D0%B8%D0%B5%20%D0%BE%D0%B1%D0%BB%D0%BE%D0%B6%D0%BA%D0%B8%20%D1%81%D0%BE%D0%B1%D1%8B%D1%82%D0%B8%D1%8F%202.png)

## Отмена события

![отмена события](https://github.com/E-A-Volobuev/TelegrammBotEventGuest/blob/master/%D0%BE%D1%82%D0%BC%D0%B5%D0%BD%D0%B0%20%D1%81%D0%BE%D0%B1%D1%8B%D1%82%D0%B8%D1%8F.png)

## Напоминание пользователю о событии

В приложении создано 3 вида напоминания о событии ,на которое зарегистрирован пользователь.
Первое напоминание происходит за 24 часа до мероприятия.
Второе напоминание за 12 часов до начала мероприятия.
Третье напоминание, когда мероприятие началось.

![напоминание](https://github.com/E-A-Volobuev/TelegrammBotEventGuest/blob/master/%D0%BD%D0%B0%D0%BF%D0%BE%D0%BC%D0%B8%D0%BD%D0%B0%D0%BD%D0%B8%D0%B5.png)

