using AgileStringEncryptionRemake.Core.Protections;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Writer;
using System;
using System.IO;
using System.Linq;

namespace AgileStringEncryptionRemake
{
    internal class Program
    {
        //https://lifeinhex.com/how-to-inject-byte-array-using-dnlib
        static FieldDef InjectArrayAndWrite(ModuleDefMD mod, byte[] injectedData, string byteArrayName, string path)
        {
            Importer importer = new Importer(mod);

            ITypeDefOrRef valueTypeRef = importer.Import(typeof(System.ValueType));
            TypeDef classWithLayout = new TypeDefUser("kaoClass", valueTypeRef);
            classWithLayout.Attributes |= TypeAttributes.Sealed | TypeAttributes.ExplicitLayout;
            classWithLayout.ClassLayout = new ClassLayoutUser(1, (uint)injectedData.Length);
            mod.Types.Add(classWithLayout);

            FieldDef fieldWithRVA = new FieldDefUser("kaoField", new FieldSig(classWithLayout.ToTypeSig()), FieldAttributes.Static | FieldAttributes.Assembly | FieldAttributes.HasFieldRVA);
            fieldWithRVA.InitialValue = injectedData;
            classWithLayout.Fields.Add(fieldWithRVA);

            var fieldInjectedArray = mod.GlobalType.Fields.Where(x => x.Name == byteArrayName).First();
            Console.WriteLine(fieldInjectedArray.Name);
            fieldInjectedArray = new FieldDefUser(byteArrayName, new FieldSig(mod.CorLibTypes.Byte.ToTypeDefOrRef().ToTypeSig()), FieldAttributes.Static | FieldAttributes.Private);

            ITypeDefOrRef systemByte = importer.Import(typeof(System.Byte));
            ITypeDefOrRef runtimeHelpers = importer.Import(typeof(System.Runtime.CompilerServices.RuntimeHelpers));
            IMethod initArray = importer.Import(typeof(System.Runtime.CompilerServices.RuntimeHelpers).GetMethod("InitializeArray", new Type[] { typeof(System.Array), typeof(System.RuntimeFieldHandle) }));

            MethodDef cctor = mod.GlobalType.FindOrCreateStaticConstructor();
            for (int i = 0; i < cctor.Body.Instructions.Count(); i++)
            {
                if (cctor.Body.Instructions[i].ToString().Contains(byteArrayName) && cctor.Body.Instructions[i].OpCode == OpCodes.Stsfld)
                {
                    cctor.Body.Instructions[i - 1].OpCode = OpCodes.Call;
                    cctor.Body.Instructions[i - 1].Operand = initArray;

                    cctor.Body.Instructions.Insert(i - 1, new Instruction(OpCodes.Ldtoken, fieldWithRVA));
                    cctor.Body.Instructions.Insert(i - 1, new Instruction(OpCodes.Dup));
                    cctor.Body.Instructions.Insert(i - 1, new Instruction(OpCodes.Newarr, systemByte));
                    cctor.Body.Instructions[i - 2].Operand = (int)injectedData.Length;

                    try
                    {
                        ModuleWriterOptions options = new ModuleWriterOptions(mod);
                        options.MetadataOptions.Flags = MetadataFlags.KeepOldMaxStack;
                        mod.Write(Path.ChangeExtension(path, "agiled" + Path.GetExtension(path)), options);
                        Console.WriteLine("Finished !");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                    Environment.Exit(0);
                }
            }

            return fieldInjectedArray;
        }

        static void Main(string[] args)
        {
            Core.Protections.Runtime.Strings.byteArrayYouKnow = Utils.RandomBytesArray().Take(new Random().Next(100, 255)).ToArray();
            ModuleDefMD Module = ModuleDefMD.Load(args[0]);
            Strings.Execute(Module);

            InjectArrayAndWrite(Module, Core.Protections.Runtime.Strings.byteArrayYouKnow, "QWdpbGVTdHJpbmdFbmNyeXB0aW9uQnl0ZXNBcnJheUxtYW8=", args[0]);
            /*try
            {

                ModuleWriterOptions options = new ModuleWriterOptions(Module);
                options.MetadataOptions.Flags = MetadataFlags.KeepOldMaxStack;
                Module.Write(Path.ChangeExtension(args[0], "agiled" + Path.GetExtension(args[0])), options);
                Console.WriteLine("Finished !");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }*/
            Environment.Exit(0);
            Console.ReadLine();
        }
    }
}
