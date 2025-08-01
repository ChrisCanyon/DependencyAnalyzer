﻿using Microsoft.CodeAnalysis;
using System.Text.Json.Serialization;

namespace DependencyAnalyzer
{
    //there are ordered by shortest to longest lived
    //a simple > or < comparison tells you if you are longer or shorter lived
    public enum LifetimeTypes
    {
        Controller,
        Transient,
        PerWebRequest,
        Singleton,
        Unknown //maybe should be unregistered
    }

    public class RegistrationInfo
    {
        public INamedTypeSymbol? Implementation { get; set; } 
        public INamedTypeSymbol? Interface { get; set; }
        public required string ProjectName { get; set; }
        public LifetimeTypes Lifetime { get; set; }
        public bool IsFactoryResolved { get; set; } = false;
        //if this is true we assume ANY implementation of the interface is valid
        public bool UnresolvableImplementation { get; set; } = false;
        public string Print()
        {
            var interfaceName = Interface?.ToDisplayString() ?? "(none)";
            return
                $"- Project: {ProjectName}\n" +
                $"  Interface: {interfaceName}\n" +
                $"  Lifetime: {Lifetime}";
        }
    }

    public class DependencyNode
    {
        [JsonIgnore]
        public required INamedTypeSymbol Class { get; set; }
        public required string ClassName { get; set; }
        public Dictionary<string, RegistrationInfo> RegistrationInfo { get; set; } = []; //<projectName, RegistrationInfo>
        public List<DependencyNode> DependsOn { get; set; } = [];
        public List<DependencyNode> DependedOnBy { get; set; } = [];
        public List<DependencyNode> Implements { get; set; } = [];
        public List<DependencyNode> ImplementedBy { get; set; } = [];
        public bool IsInterface => Class.TypeKind == TypeKind.Interface;
    }
}
