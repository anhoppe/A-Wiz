﻿using Gwiz.Core.Contract;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using Gwiz.Core.Serializer;
using System.Text;
using Awiz.Core.Git;
using Awiz.Core.Contract.Git;
using Awiz.Core.Contract.CodeInfo;
using Awiz.Core.SequenceDiagram;
using Awiz.Core.SequenceDiagram.Serializer;
using Awiz.Core.CSharpParsing;

namespace Awiz.Core.Storage
{
    internal class YamlStorageAccess : IStorageAccess
    {
        internal ISourceCode? SourceCode { private get; init; }

        public Stack<CallInfo> LoadSequenceCallstack(Stream stream, IDictionary<INode, ClassInfo> nodeToClassInfoMapping)
        {
            if (SourceCode == null)
            {
                throw new NullReferenceException("SourceCode is not set");
            }

            var callstack = new Stack<CallInfo>();

            using (var reader = new StreamReader(stream))
            {
                string yaml = reader.ReadToEnd();
                var serializer = new DeserializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();

                var callstackAsList = serializer.Deserialize<List<CallInfoDto>>(yaml) ?? new();

                foreach (var callInfoDto in callstackAsList)
                {
                    var sourceNode = nodeToClassInfoMapping.Keys.FirstOrDefault(n => n.Id == callInfoDto.SourceNodeId);
                    var targetNode = nodeToClassInfoMapping.Keys.FirstOrDefault(n => n.Id == callInfoDto.TargetNodeId);
                    if (sourceNode == null || targetNode == null)
                    {
                        continue;
                    }

                    var callInfo = new CallInfo
                    {
                        SourceNode = sourceNode,
                        TargetNode = targetNode,
                        CalledMethod = SourceCode.GetMethodInfoById(callInfoDto.CalledMethodId) ?? throw new InvalidOperationException($"Method with ID {callInfoDto.CalledMethodId} not found in source code"),
                    };
                    callstack.Push(callInfo);
                }
            }

            return callstack;
        }

        public IGraph LoadDiagramGraph(string name, string path)
        {
            using (var templateDefinitions = GetEmbeddedUmlYaml())
            {
                using (var nodeDefinitinos = File.Open(path, FileMode.Open))
                {
                    var templatesAsText = new StreamReader(templateDefinitions, Encoding.UTF8).ReadToEnd();
                    var graphAsText = new StreamReader(nodeDefinitinos, Encoding.UTF8).ReadToEnd();

                    var combined = templatesAsText.TrimEnd() + "\n" + graphAsText.TrimStart();

                    using (MemoryStream combinedStream = new MemoryStream(Encoding.UTF8.GetBytes(combined)))
                    {
                        using (var reader = new StreamReader(combinedStream))
                        {
                            var gwizDeserializer = new YamlSerializer();
                            return gwizDeserializer.Deserialize(combinedStream);
                        }
                    }
                }
            }
        }

        public Dictionary<string, IGitNodeInfo> LoadGitInfo(Stream stream)
        {
            Dictionary<string, IGitNodeInfo> gitInfo = new();

            using (var reader = new StreamReader(stream))
            {
                string yaml = reader.ReadToEnd();
                var serializer = new DeserializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();

                var gitInfoInternal = serializer.Deserialize<Dictionary<string, GitNodeInfo>>(yaml) ?? new ();

                gitInfo = gitInfoInternal.ToDictionary(kvp => kvp.Key, kvp => (IGitNodeInfo)kvp.Value);
            }

            return gitInfo;
        }

        public IDictionary<string, ClassInfo> LoadNodeIdToClassInfoMapping(Stream stream)
        {
            if (SourceCode == null)
            {
                throw new NullReferenceException("SourceCode is not set");
            }

            Dictionary<string, ClassInfo> nodeToClassMapping = new();

            using (var reader = new StreamReader(stream))
            {
                string yaml = reader.ReadToEnd();
                var serializer = new DeserializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();

                var deserializedNodeToClassMapping = serializer.Deserialize<Dictionary<string, ClassInfo>>(yaml) ?? new();

                foreach (var kvp in deserializedNodeToClassMapping)
                {
                    if (kvp.Value is null)
                    {
                        throw new InvalidOperationException($"ClassInfo for node ID {kvp.Key} is null");
                    }

                    var deserializedClassInfo = kvp.Value;
                    try
                    {
                        nodeToClassMapping[kvp.Key] = SourceCode.GetClassInfoById(deserializedClassInfo.Id());
                    }
                    catch (InvalidDataException)
                    {
                        // This happens for the ClassInfo that was generated for the user. For this case
                        // we can just use the deserialized object
                        nodeToClassMapping[kvp.Key] = deserializedClassInfo;
                    }
                }
            }

            return nodeToClassMapping;
        }

        public void SaveClassInfos(IList<ClassInfo> classInfos, Stream stream)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();

            var yaml = serializer.Serialize(classInfos);

            using (var writer = new StreamWriter(stream))
            {
                writer.Write(yaml);
            }
        }

        public void SaveSequenceCallstack(Stack<CallInfo> callstack, Stream stream)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();

            var yaml = serializer.Serialize(callstack.Select(p => new CallInfoDto(p)).ToList());

            using (var writer = new StreamWriter(stream))
            {
                writer.Write(yaml);
            }
        }

        public void SaveGitInfo(Dictionary<string, IGitNodeInfo> gitInfo, Stream stream)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();

            var yaml = serializer.Serialize(gitInfo);

            using (var writer = new StreamWriter(stream))
            {
                writer.Write(yaml);
            }
        }

        public void SaveNodeIdToClassInfoMapping(IDictionary<string, ClassInfo> nodeToClassMapping, Stream stream)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();

            var yaml = serializer.Serialize(nodeToClassMapping);

            using (var writer = new StreamWriter(stream))
            {
                writer.Write(yaml);
            }
        }

        private static Stream GetEmbeddedUmlYaml()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            string resourceName = "Awiz.Core.Assets.Uml.yaml";

            Stream? stream = assembly.GetManifestResourceStream(resourceName);

            if (stream == null)
            {
                throw new FileNotFoundException($"Resource {resourceName} not found in assembly {assembly.FullName}");
            }

            return stream;
        }
    }
}
