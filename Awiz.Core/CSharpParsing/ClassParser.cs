﻿using Awiz.Core.Contract.CodeInfo;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Awiz.Core.CSharpParsing
{
    public class ClassParser : ISourceCode
    {
        private IDictionary<MethodInfo, IEnumerable<(string, string)>> _methodInvocations = new Dictionary<MethodInfo, IEnumerable<(string, string)>>();

        public List<ClassInfo> Classes { get; private set; } = new();

        internal IProjectParser? ProjectParser { private get; set; }

        public IList<CallSite> GetCallSites(MethodInfo method)
        {
            var invocations = _methodInvocations[method];

            if (invocations == null)
            {
                return new List<CallSite>();
            }

            var callSites = new List<CallSite>();

            foreach (var invocation in invocations)
            {
                var calledClass = Classes.FirstOrDefault(p => p.Id() == invocation.Item1);
                if (calledClass == null)
                {
                    continue;
                }

                var calledMethod = calledClass.Methods.FirstOrDefault(p => p.Name == invocation.Item2);
                if (calledMethod == null)
                {
                    continue;
                }

                callSites.Add(new CallSite(calledClass, calledMethod));
            }

            return callSites;
        }

        public ClassInfo GetClassInfoById(string classId)
        {
            var classInfo = Classes.FirstOrDefault(p => p.Id() == classId);
            if (classInfo == null)
            {
                throw new InvalidDataException($"Class with ID {classId} not found.");
            }

            return classInfo;
        }

        public IList<ClassInfo> GetImplementations(ClassInfo interfaceInfo)
        {
            return Classes.Where(p => p.ImplementedInterfaces.Contains(interfaceInfo.Id())).ToList();
        }

        public MethodInfo GetMethodInfoById(string methodId)
        {
            var methodInfo = _methodInvocations.FirstOrDefault(p => p.Key.Id == methodId).Key;
            if (methodInfo == null)
            {
                throw new InvalidDataException($"Method with ID {methodId} not found.");
            }

            return methodInfo;
        }

        public void ParseClasses(string repoPath)
        {
            if (ProjectParser == null)
            {
                throw new NullReferenceException("ProjectParser not set");
            }

            ProjectParser.ParseProject(repoPath);

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
                .Select(property =>
                {
                    var typeSyntax = property.Type;

                    var propertyInfo = new PropertyInfo
                    {
                        Name = property.Identifier.ToString(),
                        TypeName = typeSyntax?.ToString() ?? "",
                        AccessModifier = GetAccessModifier(property)
                    };

                    if (typeSyntax != null)
                    {
                        var typeSymbol = model.GetTypeInfo(typeSyntax).Type; // Get the type symbol

                        if (typeSymbol is INamedTypeSymbol namedTypeSymbol) // Check if it's a named type (e.g., List<T>)
                        {
                            propertyInfo.TypeNamespace = typeSymbol.ContainingNamespace.ToDisplayString();

                            // Check if it implements IEnumerable<T>
                            var enumerableInterface = namedTypeSymbol.AllInterfaces
                                .FirstOrDefault(i =>
                                {
                                    return i.OriginalDefinition.ToString() == "System.Collections.Generic.IEnumerable<T>";
                                });

                            if (enumerableInterface != null)
                            {
                                var genericArgument = enumerableInterface.TypeArguments.First(); // Extract T

                                propertyInfo.IsEnumerable = true;
                                propertyInfo.GenericType.Name = genericArgument?.Name ?? string.Empty;
                                propertyInfo.GenericType.Namespace = genericArgument?.ContainingNamespace.ToString() ?? string.Empty;
                            }
                        }
                    }

                    return propertyInfo;
                }).ToList();
        }

        private void AddClassDefinitions(List<ClassInfo> classInfos, SemanticModel model, IEnumerable<ClassDeclarationSyntax> classDeclarations, string directory)
        {
            if (ProjectParser == null)
            {
                throw new NullReferenceException("ProjectParser not set");
            }

            foreach (var classDeclaration in classDeclarations)
            {
                string namespaceName = GetNamespace(classDeclaration, model);
                string className = classDeclaration.Identifier.ToString();

                var classInfo = new ClassInfo
                {
                    Assembly = ProjectParser.GetProject(directory),
                    Directory = directory,
                    Name = className,
                    Namespace = namespaceName,
                    Type = ClassType.Class,
                };

                // Extract members
                classInfo.Methods = GetMethods(classDeclaration, model, classInfo);
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

        private void AddInterfaceDefinitions(List<ClassInfo> classInfos, SemanticModel model, IEnumerable<InterfaceDeclarationSyntax> interfaceDeclarations, string directory)
        {
            if (ProjectParser == null)
            {
                throw new NullReferenceException("ProjectParser not set");
            }

            foreach (var interfaceDeclaration in interfaceDeclarations)
            {
                string namespaceName = GetNamespace(interfaceDeclaration, model);
                string className = interfaceDeclaration.Identifier.ToString();

                var classInfo = new ClassInfo
                {
                    Assembly = ProjectParser.GetProject(directory),
                    Directory = directory,
                    Name = className,
                    Namespace = namespaceName,
                    Type = ClassType.Interface,
                };

                // Extract members
                classInfo.Methods = GetMethods(interfaceDeclaration, model, classInfo);
                classInfo.Properties = GetProperties(interfaceDeclaration, model);

                var classSymbol = model.GetDeclaredSymbol(interfaceDeclaration);
                if (classSymbol != null)
                {
                    classInfo.ImplementedInterfaces.AddRange(classSymbol.Interfaces.Select(i => i.ToString()).ToList());
                }

                classInfos.Add(classInfo);
            }
        }

        private void GetCallSites(SemanticModel model, MethodDeclarationSyntax method, MethodInfo methodInfo)
        {
            IEnumerable<InvocationExpressionSyntax> invocations = method.DescendantNodes().OfType<InvocationExpressionSyntax>();

            var callSiteByName = new List<(string, string)>();
            foreach (var invocation in invocations)
            {
                var symbolInfo = model.GetSymbolInfo(invocation);
                string typeId = string.Empty;
                string methodName = string.Empty;
                if (symbolInfo.Symbol is IMethodSymbol methodSymbol)
                {
                    typeId = $"{methodSymbol.ContainingNamespace}.{methodSymbol.ContainingType.Name}";
                    methodName = methodSymbol.Name;
                }
                else if (symbolInfo.CandidateSymbols.Length == 1 && symbolInfo.CandidateSymbols[0] is IMethodSymbol candidate)
                {
                    typeId = $"{candidate.ContainingNamespace}.{candidate.ContainingType.Name}";
                    methodName = candidate.Name;
                }

                if (!callSiteByName.Any(p => p.Item1 == typeId && p.Item2 == methodName))
                {
                    callSiteByName.Add((typeId, methodName));
                }
            }

            _methodInvocations[methodInfo] = callSiteByName;
        }

        private List<MethodInfo> GetMethods(SyntaxNode classDeclaration, SemanticModel model, ClassInfo parentClass)
        {
            return classDeclaration.DescendantNodes().OfType<MethodDeclarationSyntax>()
                .Select(method =>
                {
                    var methodInfo = new MethodInfo()
                    {
                        ParentClass = parentClass,
                        Name = method.Identifier.ToString(),
                        ReturnType = method.ReturnType?.ToString() ?? "void", // Handle void return type
                        AccessModifier = GetAccessModifier(method),
                        Parameters = method.ParameterList.Parameters.Select(p => new ParameterInfo
                        {
                            Name = p.Identifier.ToString(),
                            Type = p.Type?.ToString() ?? ""
                        }).ToList()
                    };

                    GetCallSites(model, method, methodInfo);

                    return methodInfo;
                }).ToList();
        }
    }
}
