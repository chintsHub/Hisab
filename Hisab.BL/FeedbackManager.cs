using Hisab.Common.BO;
using Hisab.Dapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hisab.BL
{
    public interface IFeedbackManager
    {
        Task<bool> CreateNewFeedback(NewFeedbackBO newFeedback);

        Task<List<FeedBackBO>> GetTestimonyFeedBack();

    }

    public class FeedbackManager : IFeedbackManager
    {
        private IDbConnectionProvider _connectionProvider { get; set; }
        public FeedbackManager(IDbConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }
        public async Task<bool> CreateNewFeedback(NewFeedbackBO newFeedback)
        {
            bool result = false;

            using(var context = await HisabContextFactory.InitializeUnitOfWorkAsync(_connectionProvider))
            {
                newFeedback.Id = System.Guid.NewGuid();
                newFeedback.FeedbackDate = DateTime.UtcNow;

                result = await context.FeedbackRepository.CreateNewFeedback(newFeedback);

                context.SaveChanges();
            }

            return result;
        }

        public async Task<List<FeedBackBO>> GetTestimonyFeedBack()
        {
            using(var context = await HisabContextFactory.InitializeAsync(_connectionProvider))
            {
                var testimony = context.FeedbackRepository.GetTestimonyFeedBack();

                return testimony;
            }
        }
    }
}
