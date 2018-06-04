using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Projact.Service
{
    public class TransactionModel<T> where T:class
    {
        public string SQL;
        public T[] Pars;

        public TransactionModel(string sql, T[] pars)
        {
            SQL = sql;
            Pars = pars;
        }
        public override string ToString()
        {
            string text = SQL + "\n";
            foreach (var item in Pars)
            {
                text += ("\n" + item.ToString() + "\n");
            }
            return text;
        }
    }
}