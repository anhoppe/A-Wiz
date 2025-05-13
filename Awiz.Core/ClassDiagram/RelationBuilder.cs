using Awiz.Core.Contract.CodeInfo;
using Gwiz.Core.Contract;
using System.Globalization;

namespace Awiz.Core.ClassDiagram
{
    internal class RelationBuilder : IRelationBuilder
    {
        internal IClassNodeGenerator? ClassNodeGenerator { private get; set; }

        public void Build(IGraph graph, ClassInfo addedClassInfo, IList<ClassInfo> classesInDiagram)
        {

            foreach (var classInfo in classesInDiagram)
            {
                AddExtensions(graph, addedClassInfo, classInfo);

                AddImplementations(graph, addedClassInfo, classInfo);

                AddAssociationFromOwnerToTarget(graph, classInfo, addedClassInfo);
                AddAssociationFromOwnerToTarget(graph, addedClassInfo, classInfo);
            }
        }

        private void AddAssociationFromOwnerToTarget(IGraph graph, ClassInfo associationOwner, ClassInfo associationTarget)
        {
            if (ClassNodeGenerator == null)
            {
                throw new NullReferenceException("ClassNodeGenerator is not set");
            }

            if (associationOwner.Properties.Any(p => p.IsEnumerable && p.GenericType.Id() == associationTarget.Id()))
            {
                ClassNodeGenerator.CreateAssociation(graph, associationOwner, associationTarget, "1", "*");
            }
            else
            {
                var count = associationOwner.Properties.Count(p => p.TypeId() == associationTarget.Id());
                if (count == 1)
                {
                    ClassNodeGenerator.CreateAssociation(graph, associationOwner, associationTarget);
                }
                else if (count > 1)
                {
                    ClassNodeGenerator.CreateAssociation(graph, associationOwner, associationTarget, "1", count.ToString(CultureInfo.InvariantCulture));
                }
            }
        }

        private void AddExtensions(IGraph graph, ClassInfo addedClassInfo, ClassInfo classInfo)
        {
            if (ClassNodeGenerator == null)
            {
                throw new NullReferenceException("ClassNodeGenerator is not set");
            }

            // Check if the class is base to the added class, in that case an extension relation is added
            if (classInfo.Id() == addedClassInfo.BaseClass)
            {
                ClassNodeGenerator.CreateExtension(graph, classInfo, addedClassInfo);
            }

            // Check if the class is derived freom the added class, in that case an extension relation is added
            if (classInfo.BaseClass == addedClassInfo.Id())
            {
                ClassNodeGenerator.CreateExtension(graph, addedClassInfo, classInfo);
            }
        }

        private void AddImplementations(IGraph graph, ClassInfo addedClassInfo, ClassInfo classInfo)
        {
            if (ClassNodeGenerator == null)
            {
                throw new NullReferenceException("ClassNodeGenerator is not set");
            }

            // Check if the class is in the list of implemented interfaces of the added class, in that case an implements relation is added
            if (addedClassInfo.ImplementedInterfaces.Contains(classInfo.Id()))
            {
                ClassNodeGenerator.CreateImplementation(graph, classInfo, addedClassInfo);
            }

            // Check if the class implements the added interface
            if (classInfo.ImplementedInterfaces.Contains(addedClassInfo.Id()))
            {
                ClassNodeGenerator.CreateImplementation(graph, addedClassInfo, classInfo);
            }
        }
    }
}
