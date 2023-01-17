using LightInject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using TelegrammBotEventGuest.BLL.Repositories;
using TelegrammBotEventGuest.Core.Interfaces;
using TelegrammBotEventGuest.DataAccessLayer.Entities;

namespace TelegrammBotEventGuest.BLL.Services
{
    public class RequestService : IRequestService
    {
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, string idEvent)
        {
            if (update.Type == UpdateType.Message)
            {
                var message = update.Message;
                if (message.Text != null)
                    await MessageTextHelperAsync(botClient,update, cancellationToken, idEvent, message);

                ///добавление события
                if (message.Caption!=null)
                  await MessageCaptionHelperAsync(botClient,update,idEvent,message);

            }
            if (update.Type == UpdateType.CallbackQuery)
                await CallbackQueryHelperAsync(botClient,update,cancellationToken,idEvent);

        }

        private async Task CallbackQueryHelperAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, string idEvent)
        {
            await ViewEventsHelperAsync(botClient, update, cancellationToken, idEvent);
            await CreateEventsHelperAsync(botClient,update,cancellationToken,idEvent);
            await CancelEventsHelperAsync(botClient, update, cancellationToken, idEvent);
            await EditEventHelperAsync(botClient,update);

            if (IsMonthByCancelView(update))
            {
                var eventsBySelectMonth = await GetEntityByMonthAsync(update);
                await CreateCardEventsAsync(botClient, update.CallbackQuery.Message, eventsBySelectMonth, update.CallbackQuery.Data, cancellationToken, TypeActionEvent.CANCEL);

                return;
            }

            await RegisterAsync(botClient,update,idEvent);
            await CancelRegisterHelperAsync(botClient,update,idEvent);
        }

        private async Task CancelRegisterHelperAsync(ITelegramBotClient botClient, Update update, string idEvent)
        {
            if (update.CallbackQuery.Data.Contains("cancellRegister_"))
            {
                var chat = update.CallbackQuery.Message.Chat;
                var entity = await CancellationRegistrationByUseAsync(update, idEvent);
                string texto = @"Регистрация на событие:" + "\n" + @$"<b>{entity.Name}</b>" + "\n" + "отменена!";

                await botClient.SendTextMessageAsync(chat, texto, ParseMode.Html);
                return;
            }
        }
        private async Task RegisterAsync(ITelegramBotClient botClient, Update update,string idEvent)
        {
            if (update.CallbackQuery.Data.Contains("registerForEvent_"))
            {
                var chat = update.CallbackQuery.Message.Chat;
                bool flag = await IsRegisterAsync(update, idEvent);
                await RegisterHelperAsync(flag, botClient, update, idEvent, chat);

                return;
            }
        }
        private async Task RegisterHelperAsync(bool flag, ITelegramBotClient botClient, Update update, string idEvent, Chat chat)
        {
            if (flag)
            {
                string texto = "Вы уже зарегистрировались на это событие";
                await botClient.SendTextMessageAsync(chat, texto, ParseMode.Html);
            }
            else
            {
                var entity = await RegisterForEventAsync(update, idEvent);
                string texto = @"Вы зарегистрировались на событие:" + "\n" + @$"<b>{entity.Name}</b>";
                await botClient.SendTextMessageAsync(chat, texto, ParseMode.Html);
            }
        }
        private async Task EditEventHelperAsync(ITelegramBotClient botClient, Update update)
        {
            if (update.CallbackQuery.Data.Contains("editNameCardEvents_"))
            {
                string texto = @"<b>Введите новое название меромприятия в формате:</b>" + "\n" +
                               @"<code>editName-новое название Вашего события</code>" + "\n" +
                               @"<b>Пример:</b>" + "\n" +
                               @"<code>editName-День рождения Андрея</code>";

                await EditEventCardInfoAsync(botClient, update,  texto);
            }
            if (update.CallbackQuery.Data.Contains("editDescriptionCardEvents_"))
            {
                string texto = @"<b>Введите новое описание меромприятия в формате:</b>" + "\n" +
                               @"<code>editDescription-новое описание Вашего меромприятия</code>" + "\n" +
                               @"<b>Пример:</b>" + "\n" +
                               @"<code>editDescription-Будем играть в боулинг</code>";

                await EditEventCardInfoAsync(botClient, update, texto);
            }
            if (update.CallbackQuery.Data.Contains("editDateCardEvents_"))
            {
                string texto = @"<b>Введите новую дату меромприятия в формате:</b>" + "\n" +
                               @"<code>editDate-новая дата Вашего меромприятия</code>" + "\n" +
                               @"<b>Пример:</b>" + "\n" +
                               @"<code>editDate-05.02.2021 19:00</code>";

                await EditEventCardInfoAsync(botClient, update, texto);
            }
            if (update.CallbackQuery.Data.Contains("editPhotoCardEvents_"))
            {
                string texto = @"<b>Прикрепите новую картинку для обложки мероприятия.</b>" + "\n" +
                               @"<b>В описание к прикрепляемой картинке  добавьте:</b>" + "\n" +
                               @"<code>editPhoto</code>";

                await EditEventCardInfoAsync(botClient, update, texto);
            }
        }
        private async Task EditEventCardInfoAsync(ITelegramBotClient botClient, Update update, string texto)
        {
            var chat = update.CallbackQuery.Message.Chat;
            await botClient.SendTextMessageAsync(chat, texto, ParseMode.Html);

            return;
        }
        private async Task CancelEventsHelperAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, string idEvent)
        {
            if (update.CallbackQuery.Data == "editEvents")
            {
                await SelectMonthAsync(botClient, update, cancellationToken, TypeActionEvent.EDIT);
                return;
            }
            if (update.CallbackQuery.Data == "cancelEvents")
            {
                await SelectMonthAsync(botClient, update, cancellationToken, TypeActionEvent.CANCEL);
                return;
            }

            await CancelCardHelperAsync(botClient,update,cancellationToken,idEvent);
        }
        private async Task CancelCardHelperAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, string idEvent)
        {
            if (IsMonthByEditView(update))
            {
                var eventsBySelectMonth = await GetEntityByMonthAsync(update);
                await CreateCardEventsAsync(botClient, update.CallbackQuery.Message, eventsBySelectMonth, update.CallbackQuery.Data, cancellationToken, TypeActionEvent.EDIT);

                return;
            }
            if (update.CallbackQuery.Data.Contains("cancelCardEvents_"))
            {
                var chat = update.CallbackQuery.Message.Chat;

                var entity = await CancelEventAsync(update, idEvent);
                string texto = @$"<b>Событие: {entity.Name}</b>" + "\n" +
                               @"<code>УДАЛЕНО</code>";

                await botClient.SendTextMessageAsync(chat, texto, ParseMode.Html);
                return;
            }
        }
        private async Task CreateEventsHelperAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, string idEvent)
        {
            if (update.CallbackQuery.Data == "createEvents")
            {
                await ClickCreateEventAsync(update, botClient);
                return;
            }
            if (IsMonth(update))
            {
                var eventsBySelectMonth = await GetEventsByMonthAsync(update.CallbackQuery.Data);
                await CreateCardEventsAsync(botClient, update.CallbackQuery.Message, eventsBySelectMonth, update.CallbackQuery.Data, cancellationToken, TypeActionEvent.VIEW);
                return;
            }
        }
        private async Task ViewEventsHelperAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, string idEvent)
        {
            if (update.CallbackQuery.Data == "viewEvents")
            {
                await SelectMonthAsync(botClient, update, cancellationToken, TypeActionEvent.VIEW);
                return;
            }
            if (update.CallbackQuery.Data == "myEvents")
                await MyEventsHelperAsync(botClient,update,cancellationToken);
        }
        private async Task MyEventsHelperAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var myEvents = await GetEventsByUserAsync(update);
            if (myEvents.Count>0)
                await CreateCardEventsAsync(botClient, update.CallbackQuery.Message, myEvents, update.CallbackQuery.Data, cancellationToken, TypeActionEvent.CANCEL_REGISTER);
            else
            {
                string description = "Мероприятий 🎊🎈 не обнаружено ❌";
                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat, description);
            }
            return;
        }
        private async Task MessageCaptionHelperAsync(ITelegramBotClient botClient, Update update,string idEvent, Message message)
        {
            ///добавление события
            if (message.Caption.Contains("nameEvent-"))
            {
                await AddedEventAsync(update, botClient);
                return;
            }

            if (message.Caption.Contains("editPhoto"))
            {
                var updateEntity = await UpdatePhotoEventAsync(update, botClient, idEvent);
                await UpdatedCardEventViewAsync(botClient, message, updateEntity);

                return;
            }
        }
        private async Task MessageTextHelperAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, string idEvent, Message message)
        {
            if (message.Text.ToLower() == "/start")
            {
                await StartHelperAsync(botClient, update, cancellationToken);
                // await CreateAdmin(update);  //создание администратора
                return;
            }

            if (message.Text.Contains("editName"))
            {
                var updateEntity = await UpdateEventAsync(update, idEvent, TypeEdit.NAME);
                await UpdatedCardEventViewAsync(botClient, message, updateEntity);

                return;
            }
            if (message.Text.Contains("editDescription"))
            {
                var updateEntity = await UpdateEventAsync(update, idEvent, TypeEdit.DESCRIPTION);
                await UpdatedCardEventViewAsync(botClient, message, updateEntity);

                return;
            }
            if (message.Text.Contains("editDate"))
            {
                var updateEntity = await UpdateEventAsync(update, idEvent, TypeEdit.DATE);
                await UpdatedCardEventViewAsync(botClient, message, updateEntity);

                return;
            }
        }

        private async Task<List<EventEntity>> GetEntityByMonthAsync(Update update)
        {
            string month = GetNameMonth(update.CallbackQuery.Data);
            var eventsBySelectMonth = await GetEventsByMonthAsync(month);

            return eventsBySelectMonth;
        }


        private async Task StartHelperAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var ikm = await CreateStartButtonAsync(update);

            await botClient.SendTextMessageAsync(update.Message.Chat.Id, "выберите действие:", replyMarkup: ikm, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// создание кнопок при старте бота с проверкой на права админа
        /// </summary>
        /// <param name="update"></param>
        /// <returns></returns>
        private async Task<InlineKeyboardMarkup> CreateStartButtonAsync(Update update)
        {
            var container = new ServiceContainer();
            container.Register<IGuestEntityRepository, GuestEntityRepository>();
            var method = container.GetInstance<IGuestEntityRepository>();

            var user = await method.GetGuestByChatIdAsync(update.Message.Chat.Id);

            var arrayButton = CreateStartButtonHelperAsync(user);

            return arrayButton;
        }

        private InlineKeyboardMarkup CreateStartButtonHelperAsync(GuestEntity user)
        {
            if (user != null)
            {
                var arrayButton = StartButtonHelper(user);

                return arrayButton;
            }
            else
            {
                var arrayButton = new InlineKeyboardMarkup(new[] {new[] { InlineKeyboardButton.WithCallbackData("Просмотр всех событий", "viewEvents")},
                                                                  new[] { InlineKeyboardButton.WithCallbackData("Мои события", "myEvents")}});
                return arrayButton;
            }
        }

        private InlineKeyboardMarkup StartButtonHelper(GuestEntity user)
        {
            if (user.UserType == UserType.ADMIN) //если пользователь админ
            {
                var arrayButton = new InlineKeyboardMarkup(new[] {new[] { InlineKeyboardButton.WithCallbackData("Просмотр всех событий", "viewEvents")},
                                                                      new[] { InlineKeyboardButton.WithCallbackData("Мои события", "myEvents")},
                                                                      new[] { InlineKeyboardButton.WithCallbackData("Создание события", "createEvents")},
                                                                      new[] { InlineKeyboardButton.WithCallbackData("Редактирование события", "editEvents")},
                                                                      new[] { InlineKeyboardButton.WithCallbackData("Отмена события", "cancelEvents")}});
                return arrayButton;
            }
            else //если пользователь есть в бд, то он либо админ , либо зарегался на событие
            {
                var arrayButton = new InlineKeyboardMarkup(new[] {new[] { InlineKeyboardButton.WithCallbackData("Просмотр всех событий", "viewEvents")},
                                                                  new[] { InlineKeyboardButton.WithCallbackData("Мои события", "myEvents")}});
                return arrayButton;
            }
        }


        /// <summary>
        /// метод создания админа, использовался при первом запуске бота
        /// </summary>
        /// <param name="update"></param>
        /// <returns></returns>
        private async Task CreateAdminAsync(Update update)
        {
            var container = new ServiceContainer();
            container.Register<IGuestEntityRepository, GuestEntityRepository>();
            var method = container.GetInstance<IGuestEntityRepository>();

            GuestEntity admin = new GuestEntity();
            admin.FirstName = update.Message.Chat.FirstName;
            admin.LastName = update.Message.Chat.LastName;
            admin.ChatId = update.Message.Chat.Id;
            admin.UserType = UserType.ADMIN;

            await method.CreatAsync(admin);
        }

        #region cancellation of registration

        /// <summary>
        ///  отмена регистрации на событие
        /// </summary>
        /// <param name="update"></param>
        /// <param name="idEvent"></param>
        /// <returns></returns>
        private async Task<EventEntity> CancellationRegistrationByUseAsync(Update update,string idEvent)
        {
            var container = new ServiceContainer();
            container.Register<IGuestEntityRepository, GuestEntityRepository>();
            var method = container.GetInstance<IGuestEntityRepository>();

            Guid id = GetIdHelper(idEvent);

            await method.CancelRegistrationAsync(update.CallbackQuery.Message.Chat.Id, id);

            var containerEvent = new ServiceContainer();
            container.Register<IEventRepository, EventRepository>();
            var methodEvent = container.GetInstance<IEventRepository>();

            var currentEvent = await methodEvent.GetByIdAsync(id);

            return currentEvent;
        }


        /// <summary>
        /// получаем из бд события, на которые зарегистрирован пользователь
        /// </summary>
        /// <param name="update"></param>
        /// <returns></returns>
        private async Task<List<EventEntity>> GetEventsByUserAsync(Update update)
        {
            var container = new ServiceContainer();
            container.Register<IGuestEntityRepository, GuestEntityRepository>();
            var method = container.GetInstance<IGuestEntityRepository>();

            var guests = await method.GetAllGuestsByChatIdAsync(update.CallbackQuery.Message.Chat.Id);

            var listEvents =await GetEventsByUserHelperAsync(guests);

            return listEvents;
        }

        private async Task<List<EventEntity>> GetEventsByUserHelperAsync(List<GuestEntity> guests)
        {
            var container = new ServiceContainer();
            container.Register<IEventRepository, EventRepository>();
            var method = container.GetInstance<IEventRepository>();

            List<EventEntity> listEvents = await ListEntityHelper(method,guests);

          return listEvents;
        }

        private async Task<List<EventEntity>> ListEntityHelper(IEventRepository method,List<GuestEntity> guests)
        {
            List<EventEntity> listEvents = new List<EventEntity>();

            foreach (var guest in guests)
            {
                if (guest.EventEntityId != null)
                {
                    Guid idEvent = new Guid(guest.EventEntityId.ToString());
                    EventEntity currentEvent = await method.GetByIdAsync(idEvent);

                    if (currentEvent != null)
                        listEvents.Add(currentEvent);
                }
            }
            return listEvents;
        }

        /// <summary>
        /// карточка события пользователя
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="message"></param>
        /// <param name="pathPhoto"></param>
        /// <param name="description"></param>
        /// <param name="eventId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task UserCardEventAsync(ITelegramBotClient botClient, Message message, string pathPhoto,
                                         string description, Guid eventId, CancellationToken cancellationToken)
        {
            string command = "cancellRegister_" + eventId.ToString();

            var arrayButton = new InlineKeyboardMarkup(new[] { new[] { InlineKeyboardButton.WithCallbackData("Отменить регистрацию", command) } });

            await SendRecievCardEventAsync(botClient, message, pathPhoto, description, eventId, cancellationToken, arrayButton);
        }

        #endregion

        #region register for the event

        /// <summary>
        /// карточка события
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="message"></param>
        /// <param name="pathPhoto"></param>
        /// <param name="description"></param>
        /// <param name="eventId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task CurrentCardEventAsync(ITelegramBotClient botClient, Message message, string pathPhoto,
                                         string description, Guid eventId, CancellationToken cancellationToken)
        {
            string commandRegister = "registerForEvent_" + eventId.ToString();

            var arrayButton = new InlineKeyboardMarkup(new[] { new[] { InlineKeyboardButton.WithCallbackData("Зарегистрироваться", commandRegister) }});

            await SendRecievCardEventAsync(botClient, message, pathPhoto, description, eventId, cancellationToken, arrayButton);
        }

        /// <summary>
        /// проверка, зарегистрирован ли пользователь
        /// </summary>
        /// <param name="update"></param>
        /// <param name="idEvent"></param>
        /// <returns></returns>
        private async Task<bool> IsRegisterAsync(Update update, string idEvent)
        {
            var container = new ServiceContainer();
            container.Register<IGuestEntityRepository, GuestEntityRepository>();
            var method = container.GetInstance<IGuestEntityRepository>();

            Guid id = GetIdHelper(idEvent);

            GuestEntity guest = await method.GetGuestByEventAsync(update.CallbackQuery.Message.Chat.Id, id);

            if (guest != null)
                return true;
            else
                return false;
        }

        private async Task<EventEntity> RegisterForEventAsync(Update update, string idEvent)
        {
            var container = new ServiceContainer();
            container.Register<IEventRepository, EventRepository>();
            var method = container.GetInstance<IEventRepository>();

            Guid id = GetIdHelper(idEvent);

            EventEntity entity = await method.GetByIdAsync(id);

            if (entity != null)
                await RegisterHelperAsync(update,entity);

            return entity;
        }

        private async Task RegisterHelperAsync(Update update,EventEntity entity)
        {
            var container = new ServiceContainer();
            container.Register<IGuestEntityRepository, GuestEntityRepository>();
            var method = container.GetInstance<IGuestEntityRepository>();

            GuestEntity guest = await method.GetGuestByEventAsync(update.CallbackQuery.Message.Chat.Id, entity.Id);

            if (guest == null)
                await CreateGuestAsync(update,entity, guest,method);
        }

        private async Task CreateGuestAsync(Update update, EventEntity entity, GuestEntity guest, IGuestEntityRepository method)
        {
            guest = new GuestEntity();
            guest.FirstName = update.CallbackQuery.Message.Chat.FirstName;
            guest.LastName = update.CallbackQuery.Message.Chat.LastName;
            guest.ChatId = update.CallbackQuery.Message.Chat.Id;
            guest.EventEntityId = entity.Id;

            var admin = await method.GetAdminAsync(update.CallbackQuery.Message.Chat.Id);
            if (admin != null)
                guest.UserType = UserType.ADMIN;
            else
                guest.UserType = UserType.GUEST;

            await method.CreatAsync(guest);
        }

        #endregion

        #region canel events
        private bool IsMonthByCancelView(Update update)
        {
            bool flag = false;

            if (update.CallbackQuery.Data == "januaryCancel")
                flag = true;
            if (update.CallbackQuery.Data == "februaryCancel")
                flag = true;
            if (update.CallbackQuery.Data == "marchCancel")
                flag = true;
            if (update.CallbackQuery.Data == "aprilCancel")
                flag = true;
            if (update.CallbackQuery.Data == "mayCancel")
                flag = true;
            if (update.CallbackQuery.Data == "juneCancel")
                flag = true;
            if (update.CallbackQuery.Data == "julyCancel")
                flag = true;
            if (update.CallbackQuery.Data == "augustCancel")
                flag = true;
            if (update.CallbackQuery.Data == "septemberCancel")
                flag = true;
            if (update.CallbackQuery.Data == "octoberCancel")
                flag = true;
            if (update.CallbackQuery.Data == "novemberCancel")
                flag = true;
            if (update.CallbackQuery.Data == "decemberCancel")
                flag = true;

            return flag;
        }

        /// <summary>
        /// карточка отмены события
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="message"></param>
        /// <param name="pathPhoto"></param>
        /// <param name="description"></param>
        /// <param name="eventId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task CancelCardEventAsync(ITelegramBotClient botClient, Message message, string pathPhoto,
                                            string description, Guid eventId, CancellationToken cancellationToken)
        {
            string commandParametr = "cancelCardEvents_" + eventId.ToString();

            var arrayButton = new InlineKeyboardMarkup(new[] { new[] { InlineKeyboardButton.WithCallbackData("Отменить событие", commandParametr) } });

            await SendRecievCardEventAsync(botClient, message, pathPhoto, description, eventId, cancellationToken, arrayButton);
        }

        private async Task SendRecievCardEventAsync(ITelegramBotClient botClient, Message message, string pathPhoto,
                                               string description, Guid eventId, CancellationToken cancellationToken,
                                               InlineKeyboardMarkup arrayButton)
        {
            using (var fileStream = new FileStream(pathPhoto, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                await botClient.SendPhotoAsync(
                    chatId: message.Chat.Id,
                    photo: new InputOnlineFile(fileStream),
                    caption: description,
                    replyMarkup: arrayButton,
                    cancellationToken: cancellationToken
                );
            }
        }

        private async Task<EventEntity> CancelEventAsync(Update update, string idEvent)
        {
            var container = new ServiceContainer();
            container.Register<IEventRepository, EventRepository>();
            var method = container.GetInstance<IEventRepository>();

            Guid id = GetIdHelper(idEvent);

            EventEntity entity = await method.GetByIdAsync(id);

            if (entity != null)
                await CancelEventHelperAsync(entity, method);

            return entity;
        }

        private async Task CancelEventHelperAsync(EventEntity entity, IEventRepository method)
        {
            FileInfo fileInf = new FileInfo(entity.PathPhoto);
            if (fileInf.Exists)
                fileInf.Delete();

            await method.DeleteAsync(entity);
        }
        #endregion

        #region edit events



        private async Task<EventEntity> UpdateEventAsync(Update update, string idEvent, TypeEdit typeEdit)
        {
            var container = new ServiceContainer();
            container.Register<IEventRepository, EventRepository>();
            var method = container.GetInstance<IEventRepository>();

            Guid id = GetIdHelper(idEvent);

            EventEntity entity = await method.GetByIdAsync(id);

            if (entity != null)
                await UpdateHelperAsync(typeEdit, update, entity, method);

            return entity;
        }

        private async Task UpdateHelperAsync(TypeEdit typeEdit, Update update, EventEntity entity, IEventRepository method)
        {
            switch (typeEdit)
            {
                case TypeEdit.NAME:
                    string name = GetNameHelper(update.Message, TypeMessage.TEXT);
                    entity.Name = name;
                    break;
                case TypeEdit.DESCRIPTION:
                    string description = GetDescription(update.Message, TypeMessage.TEXT);
                    entity.Description = description;
                    break;
                case TypeEdit.DATE:
                    DateTime date = GetDate(update.Message, TypeMessage.TEXT);
                    entity.Date = date;
                    break;
                default:
                    break;
            }

            await method.UpdateAsync(entity);
        }

        private async Task UpdatedCardEventViewAsync(ITelegramBotClient botClient, Message message, EventEntity entity)
        {
            string texto = @"<b>Событие отредактировано:</b>";

            await botClient.SendTextMessageAsync(message.Chat, texto, ParseMode.Html);

            string pathPhoto = entity.PathPhoto;
            string description = "🎉 " + entity.Name + "\n" +
                                 "📝 " + entity.Description + "\n" +
                                 "📆 " + entity.Date.ToString("f");

            await ViewCardUpdatedEventAsync(botClient, message, pathPhoto, description);
        }

        private async Task<EventEntity> UpdatePhotoEventAsync(Update update, ITelegramBotClient botClient, string idEvent)
        {
            var container = new ServiceContainer();
            container.Register<IEventRepository, EventRepository>();
            var method = container.GetInstance<IEventRepository>();

            Guid id = GetIdHelper(idEvent);

            EventEntity entity = await method.GetByIdAsync(id);

            await UpdatePhotoHelperAsync(entity, update, botClient);

            await method.UpdateAsync(entity);

            return entity;
        }

        private async Task UpdatePhotoHelperAsync(EventEntity entity, Update update, ITelegramBotClient botClient)
        {
            if (entity != null)
            {
                FileInfo fileInf = new FileInfo(entity.PathPhoto);
                if (fileInf.Exists)
                    fileInf.Delete();

                string pictureCatalog = CreateCatalogByPhoto();
                string photosFileId = GetPhotosByMessage(update);

                entity.PathPhoto = await SavePictureAsync(botClient, photosFileId, pictureCatalog);
            }
        }

        private Guid GetIdHelper(string idText)
        {
            Guid guidId = Guid.Empty;
            try
            {
                guidId = new Guid(idText);
                return guidId;
            }
            catch
            {
                return guidId;
            }
        }
        private bool IsMonthByEditView(Update update)
        {
            bool flag = false;

            if (update.CallbackQuery.Data == "januaryEdit")
                flag = true;
            if (update.CallbackQuery.Data == "februaryEdit")
                flag = true;
            if (update.CallbackQuery.Data == "marchEdit")
                flag = true;
            if (update.CallbackQuery.Data == "aprilEdit")
                flag = true;
            if (update.CallbackQuery.Data == "mayEdit")
                flag = true;
            if (update.CallbackQuery.Data == "juneEdit")
                flag = true;
            if (update.CallbackQuery.Data == "julyEdit")
                flag = true;
            if (update.CallbackQuery.Data == "augustEdit")
                flag = true;
            if (update.CallbackQuery.Data == "septemberEdit")
                flag = true;
            if (update.CallbackQuery.Data == "octoberEdit")
                flag = true;
            if (update.CallbackQuery.Data == "novemberEdit")
                flag = true;
            if (update.CallbackQuery.Data == "decemberEdit")
                flag = true;

            return flag;
        }

        /// <summary>
        /// редактирование карточки события
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="message"></param>
        /// <param name="pathPhoto"></param>
        /// <param name="description"></param>
        /// <param name="eventId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task EditCardEventAsync(ITelegramBotClient botClient, Message message, string pathPhoto,
                                         string description, Guid eventId, CancellationToken cancellationToken)
        {
            string commandParametrByEditName = "editNameCardEvents_" + eventId.ToString();
            string commandParametrByEditDescription = "editDescriptionCardEvents_" + eventId.ToString();
            string commandParametrByEditDate = "editDateCardEvents_" + eventId.ToString();
            string commandParametrByEditPhoto = "editPhotoCardEvents_" + eventId.ToString();

            var arrayButton = new InlineKeyboardMarkup(new[] {new[] { InlineKeyboardButton.WithCallbackData("Редактировать название события", commandParametrByEditName)},
                                                              new[] { InlineKeyboardButton.WithCallbackData("Редактировать описание события", commandParametrByEditDescription)},
                                                              new[] { InlineKeyboardButton.WithCallbackData("Редактировать дату события", commandParametrByEditDate)},
                                                              new[] { InlineKeyboardButton.WithCallbackData("Заменить обложку события", commandParametrByEditPhoto)}});

            await SendRecievCardEventAsync(botClient, message, pathPhoto, description, eventId, cancellationToken, arrayButton);
        }

        private string GetNameMonth(string data)
        {
            string result = string.Empty;

            if (data.Contains("Edit"))
                result = data.Replace("Edit", "");

            if (data.Contains("Cancel"))
                result = data.Replace("Cancel", "");

            return result;
        }

        #endregion

        #region view events

        private bool IsMonth(Update update)
        {
            bool flag = false;

            if (update.CallbackQuery.Data == "january")
                flag = true;
            if (update.CallbackQuery.Data == "february")
                flag = true;
            if (update.CallbackQuery.Data == "march")
                flag = true;
            if (update.CallbackQuery.Data == "april")
                flag = true;
            if (update.CallbackQuery.Data == "may")
                flag = true;
            if (update.CallbackQuery.Data == "june")
                flag = true;
            if (update.CallbackQuery.Data == "july")
                flag = true;
            if (update.CallbackQuery.Data == "august")
                flag = true;
            if (update.CallbackQuery.Data == "september")
                flag = true;
            if (update.CallbackQuery.Data == "october")
                flag = true;
            if (update.CallbackQuery.Data == "november")
                flag = true;
            if (update.CallbackQuery.Data == "december")
                flag = true;

            return flag;
        }

        /// <summary>
        ///  формируем карточку события
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="message"></param>
        /// <param name="listEvents"></param>
        /// <param name="month"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private async Task CreateCardEventsAsync(ITelegramBotClient botClient, Message message, List<EventEntity> listEvents,
                                            string month, CancellationToken cancellationToken, TypeActionEvent type)
        {
            if (listEvents.Count > 0)
                await CreateCardHelperAsync(botClient, message, listEvents, type, cancellationToken);
            else
                await ResponceNoExistEventsAsync(botClient, message, month);

        }
        private async Task ResponceNoExistEventsAsync(ITelegramBotClient botClient, Message message, string month)
        {
            string translateMonth = GetTranslateMonth(month);
            string description = "📆 На " + translateMonth + " мероприятий 🎊🎈 не обнаружено ❌";
            await botClient.SendTextMessageAsync(message.Chat, description);
        }
        private async Task CreateCardHelperAsync(ITelegramBotClient botClient, Message message, List<EventEntity> listEvents,
                                            TypeActionEvent typeAction, CancellationToken cancellationToken)
        {
            foreach (var ev in listEvents)
            {
                string pathPhoto = ev.PathPhoto;
                string description = "🎉 " + ev.Name + "\n" +
                                     "📝 " + ev.Description + "\n" +
                                     "📆 " + ev.Date.ToString("f");
                await CreateCardByTypeActionAsync(botClient, message, listEvents, description, typeAction, cancellationToken, pathPhoto,ev.Id);
            }
        }

        private async Task CreateCardByTypeActionAsync(ITelegramBotClient botClient, Message message, List<EventEntity> listEvents, string description,
                                                  TypeActionEvent typeAction, CancellationToken cancellationToken, string pathPhoto,Guid eventId)
        {
            switch (typeAction)
            {
                case TypeActionEvent.VIEW:
                    await CurrentCardEventAsync(botClient, message, pathPhoto, description, eventId, cancellationToken);
                    break;
                case TypeActionEvent.EDIT:
                    await EditCardEventAsync(botClient, message, pathPhoto, description, eventId, cancellationToken);
                    break;
                case TypeActionEvent.CANCEL:
                    await CancelCardEventAsync(botClient,message,pathPhoto,description,eventId,cancellationToken);
                    break;
                case TypeActionEvent.CANCEL_REGISTER:
                    await UserCardEventAsync(botClient, message, pathPhoto, description, eventId, cancellationToken);
                    break;
            }
        }
        /// <summary>
        /// просмотр обновленной карточки события
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="message"></param>
        /// <param name="pathPhoto"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        private async Task ViewCardUpdatedEventAsync(ITelegramBotClient botClient, Message message, string pathPhoto, string description)
        {
            using (var fileStream = new FileStream(pathPhoto, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                await botClient.SendPhotoAsync(
                    chatId: message.Chat.Id,
                    photo: new InputOnlineFile(fileStream),
                    caption: description
                );
            }
        }
        /// <summary>
        /// выбор месяца
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="update"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="isEdit"></param>
        /// <returns></returns>
        private async Task SelectMonthAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, TypeActionEvent typeAction)
        {
            string editPrefix = GetTypeActionEvent(typeAction);

            var monthButtons = new InlineKeyboardMarkup(new[]
                                                                   {
                                                                      new[] { InlineKeyboardButton.WithCallbackData("Январь ☃️", "january" + editPrefix) },
                                                                      new[] { InlineKeyboardButton.WithCallbackData("Февраль ❄️", "february" + editPrefix) },
                                                                      new[] { InlineKeyboardButton.WithCallbackData("Март 🌨", "march" + editPrefix) },
                                                                      new[] { InlineKeyboardButton.WithCallbackData("Апрель 🍃", "april" + editPrefix) },
                                                                      new[] { InlineKeyboardButton.WithCallbackData("Май 💐", "may" + editPrefix) },
                                                                      new[] { InlineKeyboardButton.WithCallbackData("Июнь 🌤", "june" + editPrefix) },
                                                                      new[] { InlineKeyboardButton.WithCallbackData("Июль 🌞", "july" + editPrefix) },
                                                                      new[] { InlineKeyboardButton.WithCallbackData("Август ☀️", "august" + editPrefix) },
                                                                      new[] { InlineKeyboardButton.WithCallbackData("Сентябрь 🍁", "september" + editPrefix) },
                                                                      new[] { InlineKeyboardButton.WithCallbackData("Октябрь 🍂", "october" + editPrefix) },
                                                                      new[] { InlineKeyboardButton.WithCallbackData("Ноябрь ☔️", "november" + editPrefix) },
                                                                      new[] { InlineKeyboardButton.WithCallbackData("Декабрь 🌨", "december" + editPrefix) }
                                                                    });

            await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "выберите месяц:", replyMarkup: monthButtons, cancellationToken: cancellationToken);
        }

        private string GetTypeActionEvent(TypeActionEvent type)
        {
            string prefix = string.Empty;

            switch (type)
            {
                case TypeActionEvent.VIEW:
                    break;
                case TypeActionEvent.EDIT:
                    prefix = "Edit";
                    break;
                case TypeActionEvent.CANCEL:
                    prefix = "Cancel";
                    break;
                default:
                    break;
            }

            return prefix;
        }

        /// <summary>
        /// получаем из бд события указанного месяца
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        private async Task<List<EventEntity>> GetEventsByMonthAsync(string month)
        {
            var container = new ServiceContainer();
            container.Register<IEventRepository, EventRepository>();
            var method = container.GetInstance<IEventRepository>();

            int numMonth = GetNumberMonth(month);
            var eventsBySelectMonth = await method.GetEventsByMonthAsync(numMonth);

            return eventsBySelectMonth;
        }
        /// <summary>
        /// преобразуем письменное название месяца в номер для поиска даты по  месяцу в бд
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        private int GetNumberMonth(string month)
        {
            int number = 0;

            switch (month)
            {
                case "january":
                    number = 1;
                    break;
                case "february":
                    number = 2;
                    break;
                case "march":
                    number = 3;
                    break;
                case "april":
                    number = 4;
                    break;
                case "may":
                    number = 5;
                    break;
                case "june":
                    number = 6;
                    break;
                case "july":
                    number = 7;
                    break;
                case "august":
                    number = 8;
                    break;
                case "september":
                    number = 9;
                    break;
                case "october":
                    number = 10;
                    break;
                case "november":
                    number = 11;
                    break;
                case "december":
                    number = 12;
                    break;
                default:
                    break;
            }

            return number;
        }

        private string GetTranslateMonth(string month)
        {
            string result = string.Empty;

            switch (month)
            {
                case "january":
                    result = "январь";
                    break;
                case "february":
                    result = "ферваль";
                    break;
                case "march":
                    result = "март";
                    break;
                case "april":
                    result = "апрель";
                    break;
                case "may":
                    result = "май";
                    break;
                case "june":
                    result = "июнь";
                    break;
                case "july":
                    result = "июль";
                    break;
                case "august":
                    result = "август";
                    break;
                case "september":
                    result = "сентябрь";
                    break;
                case "october":
                    result = "октябрь";
                    break;
                case "november":
                    result = "ноябрь";
                    break;
                case "december":
                    result = "декабрь";
                    break;
                default:
                    break;
            }

            return result;
        }

        #endregion

        #region create event

        private async Task<EventEntity> CreateEventAsync(Update update, ITelegramBotClient botClient)
        {
            var container = new ServiceContainer();
            container.Register<IEventRepository, EventRepository>();
            var method = container.GetInstance<IEventRepository>();

            EventEntity entity = await GetEntityEventAsync(update, botClient, TypeMessage.CAPTION);

            if (entity != null)
                await method.CreatAsync(entity);

            return entity;
        }

        private async Task<EventEntity> GetEntityEventAsync(Update update, ITelegramBotClient botClient, TypeMessage typeMessage)
        {
            var message = update.Message;

            EventEntity entity = new EventEntity();
            entity.Name = GetNameHelper(message, typeMessage);
            entity.Description = GetDescription(message, typeMessage);
            entity.Date = GetDate(message, typeMessage);

            string pictureCatalog = CreateCatalogByPhoto();
            string photosFileId = GetPhotosByMessage(update);

            entity.PathPhoto = await SavePictureAsync(botClient, photosFileId, pictureCatalog);

            return entity;
        }

        private string GetNameHelper(Message message, TypeMessage typeMessage)
        {
            int indexNameStart = 0;
            int indexNameEnd = 0;
            string name = string.Empty;

            switch (typeMessage)
            {
                case TypeMessage.CAPTION:
                    indexNameStart = message.Caption.IndexOf("-") + 1;
                    indexNameEnd = message.Caption.IndexOf("descriptionEvent");
                    name = message.Caption.Substring(indexNameStart, indexNameEnd - indexNameStart);
                    break;
                case TypeMessage.TEXT:
                    indexNameStart = message.Text.IndexOf("-") + 1;
                    name = message.Text.Substring(indexNameStart, message.Text.Length - indexNameStart);
                    break;
                default:
                    break;
            }

            return name;
        }

        private string GetDescription(Message message, TypeMessage typeMessage)
        {
            int indexStart = 0;
            int indexDescriptEnd = 0;
            string description = string.Empty;

            switch (typeMessage)
            {
                case TypeMessage.CAPTION:
                    indexStart = message.Caption.IndexOf("descriptionEvent");
                    indexDescriptEnd = message.Caption.IndexOf("dateEvent");
                    description = message.Caption.Substring(indexStart+17, indexDescriptEnd - (indexStart+18));
                    break;
                case TypeMessage.TEXT:
                    indexStart = message.Text.IndexOf("-") + 1;
                    description = message.Text.Substring(indexStart, message.Text.Length - indexStart);
                    break;
                default:
                    break;
            }

            return description;
        }

        private DateTime GetDate(Message message, TypeMessage typeMessage)
        {
            int indexDescriptEnd = 0;
            DateTime date = new DateTime();

            switch (typeMessage)
            {
                case TypeMessage.CAPTION:
                    indexDescriptEnd = message.Caption.IndexOf("dateEvent");
                    date = CreateDateHelper(message, indexDescriptEnd, typeMessage);
                    break;
                case TypeMessage.TEXT:
                    indexDescriptEnd = message.Text.IndexOf("editDate");
                    date = CreateDateHelper(message, indexDescriptEnd, typeMessage);
                    break;
                default:
                    break;
            }


            return date;
        }

        private DateTime CreateDateHelper(Message message, int indexDescriptEnd, TypeMessage typeMessage)
        {
            try
            {
                string dateString = string.Empty;
                switch (typeMessage)
                {
                    case TypeMessage.CAPTION:
                        dateString = message.Caption.Substring(indexDescriptEnd + 10, message.Caption.Length - (indexDescriptEnd + 10));
                        break;
                    case TypeMessage.TEXT:
                        dateString = message.Text.Substring(indexDescriptEnd + 9, message.Text.Length - (indexDescriptEnd + 9));
                        break;
                    default:
                        break;
                }
                var date = Convert.ToDateTime(dateString);

                return date;
            }
            catch
            {
                DateTime date = new DateTime();
                return date;
            }
        }

        /// <summary>
        /// сохраняем картинку в во временную папку пользователя
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="photosFileId"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private async Task<string> SavePictureAsync(ITelegramBotClient botClient, string photosFileId, string path)
        {
            try
            {
                return await SavePictureHelperAsync(botClient, photosFileId, path);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error downloading: " + ex.Message);

                return string.Empty;
            }
        }
        private async Task<string> SavePictureHelperAsync(ITelegramBotClient botClient, string photo, string path)
        {
            var file = await botClient.GetFileAsync(photo);
            string pictureName = photo + ".jpg";
            string picturePath = Path.Combine(path, pictureName);
            FileStream fs = new FileStream(picturePath, FileMode.Create);
            await botClient.DownloadFileAsync(file.FilePath, fs);
            fs.Close();
            fs.Dispose();

            return picturePath;
        }
        private string CreateCatalogByPhoto()
        {
            string newPath = Path.GetTempPath();
            string path = Path.Combine(newPath, "pictureByBot");

            DirectoryInfo dirInfo = new DirectoryInfo(path);
            if (!dirInfo.Exists)
                dirInfo.Create();

            return path;
        }

        /// <summary>
        /// добавление события
        /// </summary>
        /// <param name="update"></param>
        /// <param name="botClient"></param>
        /// <returns></returns>
        private async Task AddedEventAsync(Update update, ITelegramBotClient botClient)
        {
            var message = update.Message;
            var currentEvent = await CreateEventAsync(update, botClient);

            await botClient.SendTextMessageAsync(message.Chat, "Событие добавлено:\n " +
                                                    "🎉 " + currentEvent.Name + "\n" +
                                                    "📝 " + currentEvent.Description + "\n" +
                                                    "📆 " + currentEvent.Date.ToString("f"));

        }
        /// <summary>
        /// обработка клика на кнопку создать событие
        /// </summary>
        /// <param name="update"></param>
        /// <param name="botClient"></param>
        /// <returns></returns>
        private async Task ClickCreateEventAsync(Update update, ITelegramBotClient botClient)
        {
            var chat = update.CallbackQuery.Message.Chat;
            await botClient.SendTextMessageAsync(chat, "Прикрепите фотографию события, а описание укажите в формате:\n " +
                                                        "🎉 nameEvent-название Вашего события\n" +
                                                        "📝 descriptionEvent-описание Вашего события\n" +
                                                        "📆 dateEvent-dd.mm.yyyy hh:mm\n" +
                                                        "Пример:\n" +
                                                        "nameEvent-День рождения\n" +
                                                        "descriptionEvent-Празднуем день рождения в ресторане\n" +
                                                        "dateEvent-05.02.2021 19:00");
        }
        /// <summary>
        /// получаем уникальные фотографии из сообщения с созданием события
        /// </summary>
        /// <param name="update"></param>
        /// <returns></returns>
        private string GetPhotosByMessage(Update update)
        {
            string photo = string.Empty;
            if (update.Message.Photo.Length > 0)
                photo = update.Message.Photo[update.Message.Photo.Length-1].FileId;

            return photo;
        }
        #endregion


    }
    /// <summary>
    /// что именно редактируем
    /// </summary>
    public enum TypeEdit
    {
        NAME,
        DESCRIPTION,
        DATE
    }

    public enum TypeMessage
    {
        TEXT,
        CAPTION
    }
    /// <summary>
    /// тип действия с событием
    /// </summary>
    public enum TypeActionEvent
    {
        EDIT,
        VIEW,
        CANCEL,
        CANCEL_REGISTER
    }
}
