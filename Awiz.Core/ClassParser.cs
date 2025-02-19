using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Awiz.Core
{
    public class ClassParser
    {
        public List<ClassInfo> ParseClasses(string repoPath)
        {
            List<ClassInfo> classInfos = new List<ClassInfo>();

            var files = Directory.GetFiles(repoPath, "*.cs", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                try
                {
                    SyntaxTree tree = CSharpSyntaxTree.ParseText(File.ReadAllText(file));
                    var compilation = CSharpCompilation.Create("MyCompilation", syntaxTrees: new[] { tree });
                    var model = compilation.GetSemanticModel(tree);

                    var classDeclarations = tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>();

                    foreach (var classDeclaration in classDeclarations)
                    {
                        string namespaceName = GetNamespace(classDeclaration, model);
                        string className = classDeclaration.Identifier.ToString();

                        var classInfo = new ClassInfo { Namespace = namespaceName, ClassName = className };

                        // Extract members
                        classInfo.Methods = GetMethods(classDeclaration, model);
                        classInfo.Properties = GetProperties(classDeclaration, model);
                        classInfo.Fields = GetFields(classDeclaration, model);

                        classInfos.Add(classInfo);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing file {file}: {ex.Message}");
                }
            }

            return classInfos;
        }

        private static string GetAccessModifier(SyntaxNode node)
        {
            SyntaxTokenList modifiers = default; // Initialize to default

            switch (node)
            {
                case MethodDeclarationSyntax method:
                    modifiers = method.Modifiers;
                    break;
                case PropertyDeclarationSyntax property:
                    modifiers = property.Modifiers;
                    break;
                case FieldDeclarationSyntax field:
                    modifiers = field.Modifiers;
                    break;
                case ClassDeclarationSyntax @class:
                    modifiers = @class.Modifiers;
                    break;
                // Add other types if needed (e.g., interface, struct)
                // ...
                default:
                    return ""; // Or handle the case where modifiers are not applicable
            }

            if (modifiers.Any(SyntaxKind.PublicKeyword)) return "public";
            if (modifiers.Any(SyntaxKind.PrivateKeyword)) return "private";
            if (modifiers.Any(SyntaxKind.ProtectedKeyword)) return "protected";
            if (modifiers.Any(SyntaxKind.InternalKeyword)) return "internal";
            return ""; // Default (no explicit modifier)
        }

        private static List<FieldInfo> GetFields(ClassDeclarationSyntax classDeclaration, SemanticModel model)
        {
            return classDeclaration.DescendantNodes().OfType<FieldDeclarationSyntax>()
                .Select(field => new FieldInfo
                {
                    Name = field.Declaration.Variables.First().Identifier.ToString(), // Get field name
                    Type = field.Declaration.Type?.ToString() ?? "",
                    AccessModifier = GetAccessModifier(field)
                }).ToList();
        }

        private static List<MethodInfo> GetMethods(ClassDeclarationSyntax classDeclaration, SemanticModel model)
        {
            return classDeclaration.DescendantNodes().OfType<MethodDeclarationSyntax>()
                .Select(method => new MethodInfo
                {
                    Name = method.Identifier.ToString(),
                    ReturnType = method.ReturnType?.ToString() ?? "void", // Handle void return type
                    AccessModifier = GetAccessModifier(method),
                    Parameters = method.ParameterList.Parameters.Select(p => new ParameterInfo
                    {
                        Name = p.Identifier.ToString(),
                        Type = p.Type?.ToString() ?? ""
                    }).ToList()
                }).ToList();
        }

        private static string GetNamespace(ClassDeclarationSyntax classDeclaration, SemanticModel model)
        {
            // Handle nested classes and namespaces
            var parent = classDeclaration.Parent;
            while (parent != null)
            {
                if (parent is NamespaceDeclarationSyntax namespaceDeclaration)
                {
                    return namespaceDeclaration.Name.ToString();
                }
                else if (parent is ClassDeclarationSyntax parentClass)
                {
                    // For nested classes, get the namespace of the parent class
                    return GetNamespace(parentClass, model);
                }
                parent = parent.Parent;
            }
            return ""; // Or handle the case where no namespace is found
        }

        private static List<PropertyInfo> GetProperties(ClassDeclarationSyntax classDeclaration, SemanticModel model)
        {
            return classDeclaration.DescendantNodes().OfType<PropertyDeclarationSyntax>()
                .Select(property => new PropertyInfo
                {
                    Name = property.Identifier.ToString(),
                    Type = property.Type?.ToString() ?? "",
                    AccessModifier = GetAccessModifier(property)
                }).ToList();
        }
    }
}
