using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;

namespace EntityFrameworkCore.ProviderStarter.Infrastructure
{
    public class MyModelSource : IModelSource
    {
        public IModel GetModel(DbContext context, IConventionSetBuilder conventionSetBuilder, IModelValidator validator)
        {
            throw new NotImplementedException();
        }
    }
}
