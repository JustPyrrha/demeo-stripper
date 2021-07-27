using System.IO;
using Mono.Cecil;

namespace DemeoStripper
{
    internal class ModuleProcessor
    {
        public static ModuleProcessor Load(string fileName, params string[] resolverDirs)
        {
            return new ModuleProcessor(fileName, resolverDirs);
        }

        private readonly FileInfo _file;
        private ModuleDefinition _module;

        internal ModuleProcessor(string fileName, params string[] resolverDirs)
        {
            this._file = new FileInfo(fileName);
            this.LoadModules(resolverDirs);
        }

        private void LoadModules(string[] directories)
        {
            var resolver = new CecilLibLoader();
            resolver.AddSearchDirectory(this._file.Directory?.FullName);

            foreach (string dir in directories)
            {
                if (Directory.Exists(dir))
                    resolver.AddSearchDirectory(dir);
            }

            ReaderParameters parameters = new ReaderParameters
            {
                AssemblyResolver = resolver,
                ReadWrite = false,
                ReadingMode = ReadingMode.Immediate,
                InMemory = true
            };

            this._module = ModuleDefinition.ReadModule(this._file.FullName, parameters);
        }

        public void Virtualize()
        {
            foreach (TypeDefinition type in this._module.Types)
            {
                this.VirtualizeType(type);
            }
        }

        private void VirtualizeType(TypeDefinition type)
        {
            if (type.IsSealed) type.IsSealed = false;

            if (type.IsInterface) return;
            if (type.IsAbstract) return;

            foreach (var subType in type.NestedTypes)
            {
                this.VirtualizeType(subType);
            }

            foreach (var m in type.Methods)
            {
                if (m.IsManaged
                    && m.IsIL
                    && !m.IsStatic
                    && !m.IsVirtual
                    && !m.IsAbstract
                    && !m.IsAddOn
                    && !m.IsConstructor
                    && !m.IsSpecialName
                    && !m.IsGenericInstance
                    && !m.HasOverrides)
                {
                    m.IsVirtual = true;
                    m.IsPublic = true;
                    m.IsPrivate = false;
                    m.IsNewSlot = true;
                    m.IsHideBySig = true;
                }
            }

            foreach (var field in type.Fields)
            {
                if (field.IsPrivate) field.IsFamily = true;
            }
        }

        public void Strip()
        {
            foreach (TypeDefinition type in this._module.Types)
            {
                this.StripType(type);
            }
        }

        private void StripType(TypeDefinition type)
        {
            foreach (var m in type.Methods)
            {
                if (m.Body != null)
                {
                    m.Body.Instructions.Clear();
                    m.Body.InitLocals = false;
                    m.Body.Variables.Clear();
                }
            }

            foreach (var subType in type.NestedTypes)
            {
                this.StripType(subType);
            }
        }

        public void Write(string outFile)
        {
            this._module.Write(outFile);
        }
    }
}
