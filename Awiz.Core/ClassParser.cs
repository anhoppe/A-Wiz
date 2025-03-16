using Awiz.Core.CodeInfo;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Awiz.Core
{
    public class ClassParser : IClassProvider
    {
        public List<ClassInfo> Classes { get; private set; } = new();

        public void ParseClasses(string repoPath)
        {
            var classInfos = new List<ClassInfo>();

            var syntaxTrees = GenerateSyntaxTrees(repoPath);

            var references = new List<MetadataReference>
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location), // Core runtime
                MetadataReference.CreateFromFile(typeof(Console).Assembly.Location) // System.Console
                // Add other assemblies if needed
            };

            var compilation = CSharpCompilation.Create(
                "MyAnalysis",
                syntaxTrees.Select(p => p.Item2),
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            );

            foreach (var syntaxTree in syntaxTrees)
            {
                var model = compilation.GetSemanticModel(syntaxTree.Item2);
                var root = syntaxTree.Item2.GetRoot();

                var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
                AddClassDefinitions(classInfos, model, classDeclarations, syntaxTree.Item1);

                var interfaceDeclarations = root.DescendantNodes().OfType<InterfaceDeclarationSyntax>();
                AddInterfaceDefinitions(classInfos, model, interfaceDeclarations, syntaxTree.Item1);
            }

            Classes = classInfos;
        }

        private static void AddClassDefinitions(List<ClassInfo> classInfos, SemanticModel model, IEnumerable<ClassDeclarationSyntax> classDeclarations, string directory)
        {
            foreach (var classDeclaration in classDeclarations)
            {
                string namespaceName = GetNamespace(classDeclaration, model);
                string className = classDeclaration.Identifier.ToString();

                var classInfo = new ClassInfo
                {
                    Directory = directory,
                    Name = className,
                    Namespace = namespaceName,
                    Type = ClassType.Class,
                };

                // Extract members
                classInfo.Methods = GetMethods(classDeclaration, model);
                classInfo.Properties = GetProperties(classDeclaration, model);
                classInfo.Fields = GetFields(classDeclaration, model);

                var classSymbol = model.GetDeclaredSymbol(classDeclaration);
                if (classSymbol != null)
                {
                    var baseType = classSymbol.BaseType;

                    if (baseType != null && baseType.TypeKind != TypeKind.Interface && baseType.Name != null)
                    {
                        classInfo.BaseClass = baseType.Name != "Object" ? baseType.ToString() : string.Empty;
                    }

                    classInfo.ImplementedInterfaces.AddRange(classSymbol.Interfaces.Select(i => i.ToString()).ToList());
                }

                classInfos.Add(classInfo);
            }
        }

        private static void AddInterfaceDefinitions(List<ClassInfo> classInfos, SemanticModel model, IEnumerable<InterfaceDeclarationSyntax> interfaceDeclarations, string directory)
        {
            foreach (var interfaceDeclaration in interfaceDeclarations)
            {
                string namespaceName = GetNamespace(interfaceDeclaration, model);
                string className = interfaceDeclaration.Identifier.ToString();

                var classInfo = new ClassInfo
                {
                    Directory = directory,
                    Name = className,
                    Namespace = namespaceName,
                    Type = ClassType.Interface,
                };

                // Extract members
                classInfo.Methods = GetMethods(interfaceDeclaration, model);
                classInfo.Properties = GetProperties(interfaceDeclaration, model);

                var classSymbol = model.GetDeclaredSymbol(interfaceDeclaration);
                if (classSymbol != null)
                {
                    classInfo.ImplementedInterfaces.AddRange(classSymbol.Interfaces.Select(i => i.ToString()).ToList());
                }

                classInfos.Add(classInfo);
            }
        }

        private static List<(string, SyntaxTree)> GenerateSyntaxTrees(string repoPath)
        {
            var files = Directory.GetFiles(repoPath, "*.cs", SearchOption.AllDirectories);

            var syntaxTrees = new List<(string, SyntaxTree)>();
            var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp10);

            foreach (var file in files)
            {
                try
                {
                    SyntaxTree tree = CSharpSyntaxTree.ParseText(File.ReadAllText(file));
                    syntaxTrees.Add((file, tree));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing file {file}: {ex.Message}");
                }
            }

            return syntaxTrees;
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

        private static List<FieldInfo> GetFields(SyntaxNode classDeclaration, SemanticModel model)
        {
            return classDeclaration.DescendantNodes().OfType<FieldDeclarationSyntax>()
                .Select(field => new FieldInfo
                {
                    Name = field.Declaration.Variables.First().Identifier.ToString(), // Get field name
                    Type = field.Declaration.Type?.ToString() ?? "",
                    AccessModifier = GetAccessModifier(field)
                }).ToList();
        }

        private static List<MethodInfo> GetMethods(SyntaxNode classDeclaration, SemanticModel model)
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

        private static string GetNamespace(SyntaxNode classDeclaration, SemanticModel model)
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

        private static List<PropertyInfo> GetProperties(SyntaxNode classDeclaration, SemanticModel model)
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
