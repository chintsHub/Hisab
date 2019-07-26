using System;
using System.Collections.Generic;
using System.Text;
using Hisab.Common.BO;

namespace Hisab.BL
{
    public static class TransactionFactory
    {
        public static TransactionBo CreateNewSplitTransaction(SplitType splitType)
        {
            TransactionBo _transactionBo = null;
            switch (splitType)
            {
                case SplitType.SplitPerFriend:
                {
                    _transactionBo = new SplitPerFriendTransactionBo();
                    break;
                }
                
            }

            return _transactionBo;
        }

        public static TransactionBo CreatePoolEntryTransaction()
        {
            return new EventPoolTransactionBo();
        }
    }
}
