using AgileStringEncryptionRemake.Core.Helpers;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                IEnumerable<IDnlibDef> members = InjectHelper.Inject(typeDef, mod.GlobalType, mod);
                MethodDef init = (MethodDef)members.Single(method => method.Name == "decryptString");

                foreach (IDnlibDef member in members)
                    if (member.Name == "decryptString")
                        member.Name = "QWdpbGVTdHJpbmdFbmNyeXB0aW9uTG1hbw==";
                    else if (member.Name == "byteArrayYouKnow")
                        member.Name = "QWdpbGVTdHJpbmdFbmNyeXB0aW9uQnl0ZXNBcnJheUxtYW8=";

                foreach (TypeDef type in mod.Types)
                {
                    foreach (MethodDef method in type.Methods.Where(x => x.HasBody))
                    {
                        for (int i = 0; i < method.Body.Instructions.Count; i++)
                            if (method.Body.Instructions[i].OpCode == OpCodes.Ldstr && !method.Body.Instructions[i].Operand.ToString().Contains(Convert.ToBase64String(Encoding.UTF8.GetBytes("ց֥֥֞֨ՙ֐֥֨֫֝ՙ՚"))) && !method.Body.Instructions[i].Operand.ToString().Contains("ZG5zcHk=") && !type.Name.Contains("AssemblyLoader")) //Assembly Loader for Costura
                            {
                                if (method.Body.Instructions[i].Operand == Settings.whatAbadKindOfWatermark) continue;
                                var shiftedString = Helpers.Strings.ProtectString(method.Body.Instructions[i].Operand.ToString());

                                method.Body.Instructions[i].OpCode = OpCodes.Ldstr;
                                method.Body.Instructions[i].Operand = shiftedString;

                                method.Body.Instructions.Insert(i + 1, OpCodes.Ldstr.ToInstruction(Settings.whatAbadKindOfWatermark));
                                method.Body.Instructions.Insert(i + 2, OpCodes.Call.ToInstruction(init));

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