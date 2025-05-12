namespace Awiz.Core.Contract.CodeInfo
{
    /// <summary>
    /// Updates the class info object from a class diagram in comparison to the class info
    /// objects in the repo.
    /// This operation adds information to the class info objets in the class diagram
    /// </summary>
    public interface IVersionUpdater
    {
        /// <summary>
        /// Event is raised when a class info object is updated
        /// </summary>
        event EventHandler<ClassInfo>? ClassUpdated;

        void CheckVersionUpdates(ClassInfo updatedClassInfo, IList<ClassInfo> repoClassInfos);

        /// <summary>
        /// Delete a suggestion for a method that was added to the class in the repo
        /// </summary>
        /// <param name="updatedClassInfo"></param>
        /// <param name="propertyInfo"></param>
        void DeleteAddedSuggestion(ClassInfo updatedClassInfo, MethodInfo methodInfo);

        /// <summary>
        /// Delete a suggestion for a property that was added to the class in the repo
        /// </summary>
        /// <param name="updatedClassInfo"></param>
        /// <param name="propertyInfo"></param>
        void DeleteAddedSuggestion(ClassInfo updatedClassInfo, PropertyInfo propertyInfo);

        /// <summary>
        /// Delete a suggestion for a method that was deleted from the class in the repo
        /// </summary>
        /// <param name="updatedClassInfo"></param>
        /// <param name="methodInfo"></param>
        void DeleteDeletedSuggestion(ClassInfo updatedClassInfo, MethodInfo methodInfo);

        /// <summary>
        /// Delete a suggestion for a property that was deleted from the class in the repo
        /// </summary>
        /// <param name="updatedClassInfo"></param>
        /// <param name="propertyInfo"></param>
        void DeleteDeletedSuggestion(ClassInfo updatedClassInfo, PropertyInfo propertyInfo);

        /// <summary>
        /// Update the class info object with a method that was added to the class in the repo
        /// </summary>
        /// <param name="updatedClassInfo"></param>
        /// <param name="propertyInfo"></param>
        void UpdateAdd(ClassInfo updatedClassInfo, MethodInfo methodInfo);

        /// <summary>
        /// Update the class info object with a property that was added to the class in the repo
        /// </summary>
        /// <param name="updatedClassInfo"></param>
        /// <param name="propertyInfo"></param>
        void UpdateAdd(ClassInfo updatedClassInfo, PropertyInfo propertyInfo);

        /// <summary>
        /// Deletes a method from a class based on a suggestion
        /// </summary>
        void UpdateDelete(ClassInfo updatedClassInfo, MethodInfo methodInfo);

        /// <summary>
        /// Deletes a property from a class based on a suggestion
        /// </summary>
        void UpdateDelete(ClassInfo updatedClassInfo, PropertyInfo propertyInfo);
    }
}
