using Awiz.Core.Contract.CodeInfo;

namespace Awiz.Core.CSharpParsing
{
    internal class VersionUpdater : IVersionUpdater
    {
        public event EventHandler<ClassInfo>? ClassUpdated;

        public void CheckVersionUpdates(ClassInfo updatedClassInfo, IList<ClassInfo> repoClassInfo)
        {
            var repoClass = repoClassInfo.FirstOrDefault(c => c.Id() == updatedClassInfo.Id());
            if (repoClass == null)
            {
                return;
            }

            CheckForAddedProperties(updatedClassInfo, repoClass);

            CheckForDeletedProperties(updatedClassInfo, repoClass);

            CheckForAddedMethods(updatedClassInfo, repoClass);

            CheckForDeletedMethods(updatedClassInfo, repoClass);
        }

        public void DeleteAddedSuggestion(ClassInfo updatedClassInfo, MethodInfo propertyInfo)
        {
            updatedClassInfo.AddedMethods.Remove(propertyInfo);
            ClassUpdated?.Invoke(this, updatedClassInfo);
        }

        public void DeleteAddedSuggestion(ClassInfo updatedClassInfo, PropertyInfo propertyInfo)
        {
            updatedClassInfo.AddedProperties.Remove(propertyInfo);
            ClassUpdated?.Invoke(this, updatedClassInfo);
        }

        public void DeleteDeletedSuggestion(ClassInfo updatedClassInfo, MethodInfo methodInfo)
        {
            updatedClassInfo.DeletedMethods.Remove(methodInfo);
            ClassUpdated?.Invoke(this, updatedClassInfo);
        }

        public void DeleteDeletedSuggestion(ClassInfo updatedClassInfo, PropertyInfo propertyInfo)
        {
            updatedClassInfo.DeletedProperties.Remove(propertyInfo);
            ClassUpdated?.Invoke(this, updatedClassInfo);
        }

        public void UpdateAdd(ClassInfo updatedClassInfo, MethodInfo methodInfo)
        {
            updatedClassInfo.AddedMethods.Remove(methodInfo);
            updatedClassInfo.Methods.Add(methodInfo);
            ClassUpdated?.Invoke(this, updatedClassInfo);
        }

        public void UpdateAdd(ClassInfo updatedClassInfo, PropertyInfo propertyInfo)
        {
            updatedClassInfo.AddedProperties.Remove(propertyInfo);
            updatedClassInfo.Properties.Add(propertyInfo);
            ClassUpdated?.Invoke(this, updatedClassInfo);
        }

        public void UpdateDelete(ClassInfo updatedClassInfo, MethodInfo methodInfo)
        {
            updatedClassInfo.DeletedMethods.Remove(methodInfo);
            updatedClassInfo.Methods.Remove(methodInfo);
            ClassUpdated?.Invoke(this, updatedClassInfo);
        }

        public void UpdateDelete(ClassInfo updatedClassInfo, PropertyInfo propertyInfo)
        {
            updatedClassInfo.DeletedProperties.Remove(propertyInfo);
            updatedClassInfo.Properties.Remove(propertyInfo);
            ClassUpdated?.Invoke(this, updatedClassInfo);
        }

        private static void CheckForAddedMethods(ClassInfo updatedClass, ClassInfo repoClass)
        {
            foreach (var method in repoClass.Methods)
            {
                if (!updatedClass.Methods.Any(m => m.Name == method.Name) &&
                    !updatedClass.AddedMethods.Any(m => m.Name == method.Name))
                {
                    updatedClass.AddedMethods.Add(method);
                }
            }
        }

        private static void CheckForAddedProperties(ClassInfo updatedClass, ClassInfo repoClass)
        {
            foreach (var property in repoClass.Properties)
            {
                if (!updatedClass.Properties.Any(p => p.Name == property.Name) &&
                    !updatedClass.AddedProperties.Any(m => m.Name == property.Name))
                {
                    updatedClass.AddedProperties.Add(property);
                }
            }
        }

        private static void CheckForDeletedMethods(ClassInfo updatedClass, ClassInfo repoClass)
        {
            foreach (var method in updatedClass.Methods)
            {
                if (!repoClass.Methods.Any(m => m.Name == method.Name) &&
                    !updatedClass.DeletedMethods.Any(m => m.Name == method.Name))
                {
                    updatedClass.DeletedMethods.Add(method);
                }
            }
        }

        private static void CheckForDeletedProperties(ClassInfo updatedClass, ClassInfo repoClass)
        {
            foreach (var property in updatedClass.Properties)
            {
                if (!repoClass.Properties.Any(p => p.Name == property.Name) &&
                    !updatedClass.DeletedProperties.Any(m => m.Name == property.Name))
                {
                    updatedClass.DeletedProperties.Add(property);
                }
            }
        }
    }
}
