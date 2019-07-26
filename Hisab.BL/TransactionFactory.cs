using System;
using System.Collections.Generic;
using System.Text;
using Hisab.Common.BO;

namespace Hisab.BL
{
    public static class TransactionFactory
    {
        public static TransactionBo CreateNewTransaction(TransactionType transactionType)
        {
            TransactionBo _transactionBo = null;
            switch (transactionType)
            {
                case TransactionType.SplitPerFriend:
                {
                    _transactionBo = new SplitPerFriendTransactionBo();
                    break;
                }
                case TransactionType.EventPoolEntry:
                {
                    _transactionBo = new EventPoolTransactionBo();
                    break;
                }
            }

            return _transactionBo;
        }
    }
}
