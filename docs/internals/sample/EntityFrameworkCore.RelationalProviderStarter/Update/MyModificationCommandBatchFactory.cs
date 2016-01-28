using System;
using Microsoft.EntityFrameworkCore.Update;

namespace EntityFrameworkCore.RelationalProviderStarter.Update
{
    public class MyModificationCommandBatchFactory : IModificationCommandBatchFactory
    {
        public ModificationCommandBatch Create()
        {
            throw new NotImplementedException();
        }
    }
}