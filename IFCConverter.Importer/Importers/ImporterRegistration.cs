using System;
using System.Linq.Expressions;
using System.Reflection;
using IFCConverter.Importer.Interfaces;

namespace IFCConverter.Importer.Importers
{
    internal class ImporterRegistration
    {
        public readonly IImporterFilter ImporterFilter;
        public readonly Type ImporterType;
        public readonly int Priority;

        public ImporterRegistration(Type importerType, IImporterFilter importerFilter, int priority)
        {
            ImporterType = importerType;
            ImporterFilter = importerFilter;
            Priority = priority;
        }

        private static Func<IImporter> CreateFactory(Type type)
        {
            ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
            NewExpression newExpression = Expression.New(constructor);
            Expression<Func<IImporter>> lambda = Expression.Lambda<Func<IImporter>>(newExpression);
            return lambda.Compile();
        }
    }
}