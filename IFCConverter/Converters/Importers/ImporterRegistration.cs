using System;
using System.Linq.Expressions;
using System.Reflection;
using IFCConverter.Interfaces;

namespace IFCConverter.Converters.Importers
{
    internal class ImporterRegistration
    {
        public readonly Type ImporterType;
        public readonly IImporterFilter ImporterFilter;
        public readonly int Priority;

        public ImporterRegistration(Type importerType, IImporterFilter importerFilter, int priority)
        {
            ImporterType = importerType;
            ImporterFilter = importerFilter;
            Priority = priority;
        }

        private static Func<IImporter> CreateFactory(Type type)
        {
            ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes)!;
            NewExpression newExpression = Expression.New(constructor);
            Expression<Func<IImporter>> lambda = Expression.Lambda<Func<IImporter>>(newExpression);
            return lambda.Compile();
        }
    }
}