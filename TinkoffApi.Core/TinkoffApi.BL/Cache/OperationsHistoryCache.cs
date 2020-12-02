using System.Collections.Generic;
using System.Linq;
using TinkoffApi.Data.Entities;

namespace TinkoffApi.BL.Cache
{
    public static class OperationsHistoryCache
    {
        public static List<OperationHistory> operations = new List<OperationHistory>();

        public static bool AddOrUpdate(OperationHistory operation)
        {
            var elem = operations.Where(x => x.id == operation.id).FirstOrDefault();
            if (elem != null)
            {
                elem = operation;
                return false;
            }

            operations.Add(operation);

            return true;
        }


    }
}
