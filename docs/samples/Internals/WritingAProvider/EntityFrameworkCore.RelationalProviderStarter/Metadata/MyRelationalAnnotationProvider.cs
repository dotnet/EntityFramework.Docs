using System;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EntityFrameworkCore.RelationalProviderStarter.Metadata
{
    public class MyRelationalAnnotationProvider : IRelationalAnnotationProvider
    {
        public IRelationalIndexAnnotations For(IIndex index)
        {
            throw new NotImplementedException();
        }

        public IRelationalModelAnnotations For(IModel model)
        {
            throw new NotImplementedException();
        }

        public IRelationalPropertyAnnotations For(IProperty property)
        {
            throw new NotImplementedException();
        }

        public IRelationalKeyAnnotations For(IKey key)
        {
            throw new NotImplementedException();
        }

        public IRelationalForeignKeyAnnotations For(IForeignKey foreignKey)
        {
            throw new NotImplementedException();
        }

        public IRelationalEntityTypeAnnotations For(IEntityType entityType)
        {
            throw new NotImplementedException();
        }
    }
}