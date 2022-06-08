using AgileStringEncryptionRemake.Core.Helpers;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AgileStringEncryptionRemake.Core.Protections
{
    public static class Strings
    {
        public static void Execute(ModuleDef mod)
        {
            try
            {
                ModuleDefMD typeModule = ModuleDefMD.Load(typeof(Strings).Module);
                TypeDef typeDef = typeModule.ResolveTypeDef(MDToken.ToRID(typeof(Runtime.Strings).MetadataToken));
                TypeDef newTypeDef = new TypeDefUser(typeModule.GlobalType.Namespace, Settings.typeDefName, typeDef.BaseType);
                mod.Types.Add(newTypeDef);
                IEnumerable<IDnlibDef> members = InjectHelper.Inject(typeDef, newTypeDef, mod);
                MethodDef init = (MethodDef)members.Single(method => method.Name == "decryptString");

                foreach (IDnlibDef member in members)
                    if (member.Name == "decryptString")
                        member.Name = Settings.decryptStringMethodName;
                    else if (member.Name == "byteArrayYouKnow")
                        member.Name = Settings.byteArrayName;

                foreach (TypeDef type in mod.Types)
                {
                    foreach (MethodDef method in type.Methods.Where(x => x.HasBody))
                    {
                        for (int i = 0; i < method.Body.Instructions.Count; i++)
                            if (method.Body.Instructions[i].OpCode == OpCodes.Ldstr)
                            {
                                var protectedString = Helpers.Strings.ProtectString(method.Body.Instructions[i].Operand.ToString());

                                method.Body.Instructions[i].OpCode = OpCodes.Ldstr;
                                method.Body.Instructions[i].Operand = protectedString;

                                method.Body.Instructions.Insert(i + 1, OpCodes.Call.ToInstruction(init));

                                method.Body.SimplifyBranches();
                                method.Body.OptimizeBranches();

                                i += 2;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
            }
        }
    }
}