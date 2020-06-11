using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisab.Common;
using Hisab.Common.BO;
using Hisab.Dapper;
using Hisab.Dapper.Identity;
using Hisab.Dapper.Repository;
using Microsoft.AspNetCore.Identity;

namespace Hisab.BL
{
    public interface IEventManager
    {
        Task<Guid> CreateEvent(NewEventBO newNewEvent);

        Task<List<UserEventBO>> GetEvents(Guid userId);

        Task<List<UserEventBO>> GetAllEvents();

        Task<EventBO> GetEventById(Guid eventId);

        Task<ManagerResponse> CreateEventFriend(Guid eventId, string userEmail);

        

        

        Task<bool> UpdateEvenSettings(EventSettingsBO eventSettingsBO);

        Task<bool> ArchieveEvent(Guid eventId);

        Task<bool> DisableFriend(int eventFriendId);

        Task<bool> UpdateFriend(EventFriendBO eventFriendBo);





        Task<bool> CheckEventAccess(EventBO eventBo, string userName);




        Task<EventDashboardStatBo> GetDashboardStats(int eventId, int eventFriendId);

        

        Task<List<SettlementData>> GetSettlementData(int eventId);
    }

    

    public class EventManager : IEventManager
    {
        private IDbConnectionProvider _connectionProvider;
        private UserManager<ApplicationUser> _userManager;

        private const int TotalAllowedEventsPerUser = 3;

        public EventManager(IDbConnectionProvider connectionProvider, UserManager<ApplicationUser> userManager)
        {
            _connectionProvider = connectionProvider;
            _userManager = userManager;
        }
        public async Task<Guid> CreateEvent(NewEventBO newNewEvent)
        {
            //Check total allowed 
            using (var context = await HisabContextFactory.InitializeUnitOfWorkAsync(_connectionProvider))
            {
                try
                {
                    var events = context.EventRepository.GetEventsForUser(newNewEvent.EventOwner.UserId);

                    //if (events.Count >= TotalAllowedEventsPerUser)
                    //    throw new HisabException("You have reached maximum number of allowed Events");


                    
                    newNewEvent.Id = Guid.NewGuid();
                    newNewEvent.EventOwner.EventId = newNewEvent.Id;
                    newNewEvent.CreateDateTime = DateTime.UtcNow;
                    newNewEvent.Status = EventStatus.Active;

                    if(context.EventRepository.CreateEvent(newNewEvent))
                    {
                        //add event and add owner as event friend
                        var friendResult = context.EventRepository.AddEventOwnerToEvent(newNewEvent.EventOwner);
                    }
                                    
                    

                    //create Accounts
                    var currentAccountId = context.EventRepository.CreateEventAccount(newNewEvent.Id,ApplicationAccountType.Cash);
                    //var expenseAccountId = context.EventRepository.CreateExpenseAccount(newNewEvent.Id);
                    //var owerAccount = context.EventRepository.CreateEventFriendAccount(newNewEvent.Id, friendId);

                    context.SaveChanges();

                    return newNewEvent.Id;
                }
                catch (Exception ex)
                {
                    if (context != null)
                    {
                        context.Dispose();
                    }

                    throw ex;
                }
             
            }

             
        }

        public async Task<List<UserEventBO>> GetEvents(Guid userId)
        {
            using (var context = await HisabContextFactory.InitializeAsync(_connectionProvider))
            {
                var events = context.EventRepository.GetEventsForUser(userId);

                
                return events;
                
            }
        }

        public async Task<List<UserEventBO>> GetAllEvents()
        {
            using (var context = await HisabContextFactory.InitializeAsync(_connectionProvider))
            {
                var events = context.EventRepository.GetAllEvents();


                return events;

            }
        }

        public async Task<EventBO> GetEventById(Guid eventId)
        {
            using (var context = await HisabContextFactory.InitializeAsync(_connectionProvider))
            {
                var events = context.EventRepository.GetEventById(eventId);


                return events;

            }
        }

       

        public async Task<ManagerResponse> CreateEventFriend(Guid eventId, string userEmail)
        {

            return new ManagerResponse();
            

            
        }

        public async Task<bool> UpdateEvenSettings(EventSettingsBO eventSettingsBO)
        {
            using (var context = await HisabContextFactory.InitializeUnitOfWorkAsync(_connectionProvider))
            {
                
                
                var rows = context.EventRepository.UpdateEvent(eventSettingsBO);
                context.SaveChanges();

                if (rows == 1)
                    return true;

                return false;

            }
        }

        public async Task<bool> DisableFriend(int eventFriendId)
        {
            using (var context = await HisabContextFactory.InitializeUnitOfWorkAsync(_connectionProvider))
            {
                var rows = context.EventRepository.DisableFriend(eventFriendId);
                context.SaveChanges();

                if (rows == 1)
                    return true;

                return false;

            }
        }

        public async Task<bool> UpdateFriend(EventFriendBO eventFriendBo)
        {
            using (var context = await HisabContextFactory.InitializeUnitOfWorkAsync(_connectionProvider))
            {
                int rows =0;
                //if (eventFriendBo.Status == EventFriendStatus.EventOwner || eventFriendBo.Status == EventFriendStatus.EventJoined
                //                                                         || eventFriendBo.Status == EventFriendStatus.PendingAcceptance
                //                                                         || eventFriendBo.Status == EventFriendStatus.PendingRegistration)
                //{
                //    rows = context.EventRepository.UpdateFriend(eventFriendBo.KidsCount, eventFriendBo.AdultCount, eventFriendBo.EventFriendId);
                //    context.SaveChanges();
                //}

                //if (eventFriendBo.Status == EventFriendStatus.EventFriend)
                //{
                //    rows = context.EventRepository.UpdateFriend(eventFriendBo.KidsCount, eventFriendBo.AdultCount, eventFriendBo.Email, eventFriendBo.EventFriendId);
                //    context.SaveChanges();
                //}

                if (rows == 1)
                    return true;

               
                return false;

            }
        }

        //public async Task<bool> CanAccessEvent(Guid eventId, Guid userId)
        //{
        //    using (var context = await HisabContextFactory.InitializeAsync(_connectionProvider))
        //    {
        //        var eventBo = context.EventRepository.GetEventById(eventId);

        //        return CheckEventAccess(eventBo, userId);



        //    }

           
        //}

        public async Task<bool> CheckEventAccess(EventBO eventBo, string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if(await _userManager.IsInRoleAsync(user,"admin"))
            {
                return true;
            }

            var friend = eventBo.Friends.FirstOrDefault(x => x.Email.ToLower() == userName.ToLower());

            return friend != null;
        }

        public async Task<EventDashboardStatBo> GetDashboardStats(int eventId, int eventFriendId)
        {
            var retVal = new EventDashboardStatBo();
            using (var context = await HisabContextFactory.InitializeAsync(_connectionProvider))
            {
                //retVal.TotalEventExpense = context.EventTransactionRepository.GetEventExpense(eventId);

                retVal.TotalEventPoolBalance = context.EventTransactionRepository.GetEventPoolBalance(eventId);

                retVal.MyContributions =
                    context.EventTransactionRepository.GetEventFriendContributionAmount(eventId, eventFriendId);

                retVal.MyEventExpense =
                    context.EventTransactionRepository.GetEventFriendExpense(eventId, eventFriendId);

                retVal.MyNetAmount = retVal.MyContributions - retVal.MyEventExpense;

            }

            return retVal;
        }

        //public async Task<List<TransactionBo>> GetAllTransactions(int eventId)
        //{
        //    var retValue = new List<TransactionBo>();

        //    using (var context = await HisabContextFactory.InitializeAsync(_connectionProvider))
        //    {
        //        retValue = context.EventTransactionRepository.GetAllTransactions(eventId);
        //    }

        //    return retValue;
        //}

        public async Task<List<SettlementData>> GetSettlementData(int eventId)
        {
            var retValue = new List<SettlementData>();

            using (var context = await HisabContextFactory.InitializeAsync(_connectionProvider))
            {
                retValue = context.EventTransactionRepository.GetSettlementData(eventId);
            }

            return retValue;
        }

       

        private async Task<int> ProcessContributionTransaction(EventPoolTransactionBo transactionBo)
        {
            int transactionId = 0;

            //using (var context = await HisabContextFactory.InitializeUnitOfWorkAsync(_connectionProvider))
            //{
            //    var accounts = context.EventTransactionRepository.GetAccountsForEvent(transactionBo.EventId);
            //    //prepare Journals
            //    var eventCashJournal = new TransactionJournalBo
            //    {
            //        AccountId =
            //            accounts.First(x => x.EventFriendId == 0 && x.AccountTypeId == 1).AccountId,
            //        Particulars = transactionBo.Description,
            //        CreditAmount = 0,
            //        DebitAmount = transactionBo.TotalAmount

            //    };
            //    transactionBo.Journals.Add(eventCashJournal);

            //    var friendJournal = new TransactionJournalBo()
            //    {
            //        AccountId =
            //            accounts.First(x => x.EventFriendId == transactionBo.EventFriendId && x.AccountTypeId == 1).AccountId,
            //        Particulars = "Deposit paid: " + transactionBo.Description,
            //        CreditAmount = transactionBo.TotalAmount,
            //        DebitAmount = 0
            //    };
            //    transactionBo.Journals.Add(friendJournal);

            //    //transactionId = context.EventTransactionRepository.CreateTransaction(transactionBo.TotalAmount, transactionBo.EventId,
            //    //    transactionBo.Description, (int)transactionBo.SplitType, transactionBo.CreatedByUserId, transactionBo.CreatedDateTime);

            //    foreach (var journal in transactionBo.Journals)
            //    {
            //        context.EventTransactionRepository.CreateTransactionJournal(transactionId, journal.Particulars,
            //            journal.AccountId, journal.DebitAmount, journal.CreditAmount);
            //    }

            //    context.SaveChanges();
            //}

            return transactionId;
        }

        

        public async Task<bool> ArchieveEvent(Guid eventId)
        {
            using (var context = await HisabContextFactory.InitializeUnitOfWorkAsync(_connectionProvider))
            {
                var rows = context.EventRepository.ArchieveEvent(eventId);
                context.SaveChanges();

                if (rows == 1)
                    return true;

                return false;

            }
        }

       

        
    }

  
}
