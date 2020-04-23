﻿using Dapper;
using Hisab.Common.BO;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Hisab.Dapper.Repository
{
    public interface IFeedbackRepository
    {
        Task<bool> CreateNewFeedback(NewFeedbackBO newFeedbackBO);

        List<FeedBackBO> GetTestimonyFeedBack();
    }

    internal class FeedbackRepository : RepositoryBase, IFeedbackRepository
    {
        
        public FeedbackRepository(IDbConnection connection, IDbTransaction transaction) : base(connection, transaction)
        {

        }

        public async Task<bool> CreateNewFeedback(NewFeedbackBO newFeedbackBO)
        {
            string command = $@"

            INSERT INTO [UserFeedback] ([FeedbackId], [UserId], [Message], [FeedbackType], [FeedbackDate], [ShowAsTestimony])
                    VALUES (@{nameof(newFeedbackBO.Id)},@{nameof(newFeedbackBO.UserId)},
                            @{nameof(newFeedbackBO.Message)}, @{nameof(newFeedbackBO.FeedbackType)}, 
                            @{nameof(newFeedbackBO.FeedbackDate)},
                            @{nameof(newFeedbackBO.ShowAsTestimony)});  ";

            try
            {
                await Connection.ExecuteAsync(command, newFeedbackBO, transaction: Transaction);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            

            return true;
        }

        public List<FeedBackBO> GetTestimonyFeedBack()
        {
            var result = Connection.Query<FeedBackBO>($@"
                   select 
	                f.FeedbackId,
	                f.Message,
	                f.FeedbackDate,
	                f.FeedbackType,
	                u.NickName
                from
	                [dbo].[UserFeedback] f
	                inner join [dbo].[ApplicationUser] u on f.UserId = u.Id
                where
	                f.ShowAsTestimony = 1"

               , transaction: Transaction);

            return result.ToList();
        }
    }
}
