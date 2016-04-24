using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;
using System.Security.Permissions;

namespace LaWare.Utility.Emitter
{
	public class Emitter : EmitterBase
	{
		public class IndexLoop : EmitterBase, IDisposable
		{
			public readonly Target Continue;

			public readonly Target Break;

			public readonly Local Index;

			internal IndexLoop(OperationsContainer ops, Local terminal) : base(ops)
			{
				Continue = DefineLabel();
				Break = DefineLabel();
				Index = DeclareLocal(typeof(int), false);
				Ldc(0);
				Stloc(Index);
				MarkLabel(Continue);
				Ldloc(Index);
				Ldloc(terminal);
				Bge(Break, true);
			}

			public void Dispose()
			{
				Ldloc(Index);
				Ldc(1);
				Add(false, true);
				Stloc(Index);
				Br(Continue);
				MarkLabel(Break);
			}
		}

		private static int EmittedMethodId = 1;

		public Emitter() : base(new OperationsContainer())
		{
		}

		public void Emit(ILGenerator il)
		{
			operations.Emit(il);
		}

		public MethodInfo EmitMethod(string name, Type returnType, params Type[] parameterTypes)
		{
			var num = EmittedMethodId++;
			var dynamicMethod = new DynamicMethod(name + "_" + num, returnType, parameterTypes, true);
			Emit(dynamicMethod.GetILGenerator());
			return dynamicMethod;
		}

		public IndexLoop Loop(Local terminal)
		{
			return new IndexLoop(operations, terminal);
		}
	}
	public abstract class EmitterBase
	{
		private class OperationOptions
		{
			public void Clear()
			{
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct I
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct U
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct O
		{
		}

		private static class Op
		{
			public static readonly OpCode Ldnull = OpCodes.Ldnull;

			public static readonly OpCode Ldstr = OpCodes.Ldstr;

			public static readonly OpCode Ldc_I4_M1 = OpCodes.Ldc_I4_M1;

			public static readonly OpCode Ldc_I4_0 = OpCodes.Ldc_I4_0;

			public static readonly OpCode Ldc_I4_1 = OpCodes.Ldc_I4_1;

			public static readonly OpCode Ldc_I4_2 = OpCodes.Ldc_I4_2;

			public static readonly OpCode Ldc_I4_3 = OpCodes.Ldc_I4_3;

			public static readonly OpCode Ldc_I4_4 = OpCodes.Ldc_I4_4;

			public static readonly OpCode Ldc_I4_5 = OpCodes.Ldc_I4_5;

			public static readonly OpCode Ldc_I4_6 = OpCodes.Ldc_I4_6;

			public static readonly OpCode Ldc_I4_7 = OpCodes.Ldc_I4_7;

			public static readonly OpCode Ldc_I4_8 = OpCodes.Ldc_I4_8;

			public static readonly OpCode Ldc_I4_S = OpCodes.Ldc_I4_S;

			public static readonly OpCode Ldc_I4 = OpCodes.Ldc_I4;

			public static readonly OpCode Ldc_I8 = OpCodes.Ldc_I8;

			public static readonly OpCode Ldc_R4 = OpCodes.Ldc_R4;

			public static readonly OpCode Ldc_R8 = OpCodes.Ldc_R8;

			public static readonly OpCode Ldarg = OpCodes.Ldarg;

			public static readonly OpCode Ldarg_0 = OpCodes.Ldarg_0;

			public static readonly OpCode Ldarg_1 = OpCodes.Ldarg_1;

			public static readonly OpCode Ldarg_2 = OpCodes.Ldarg_2;

			public static readonly OpCode Ldarg_3 = OpCodes.Ldarg_3;

			public static readonly OpCode Ldarg_S = OpCodes.Ldarg_S;

			public static readonly OpCode Ldarga = OpCodes.Ldarga;

			public static readonly OpCode Ldarga_S = OpCodes.Ldarga_S;

			public static readonly OpCode Starg = OpCodes.Starg;

			public static readonly OpCode Starg_S = OpCodes.Starg_S;

			public static readonly OpCode Ldloc = OpCodes.Ldloc;

			public static readonly OpCode Ldloc_0 = OpCodes.Ldloc_0;

			public static readonly OpCode Ldloc_1 = OpCodes.Ldloc_1;

			public static readonly OpCode Ldloc_2 = OpCodes.Ldloc_2;

			public static readonly OpCode Ldloc_3 = OpCodes.Ldloc_3;

			public static readonly OpCode Ldloc_S = OpCodes.Ldloc_S;

			public static readonly OpCode Ldloca = OpCodes.Ldloca;

			public static readonly OpCode Ldloca_S = OpCodes.Ldloca_S;

			public static readonly OpCode Stloc = OpCodes.Stloc;

			public static readonly OpCode Stloc_0 = OpCodes.Stloc_0;

			public static readonly OpCode Stloc_1 = OpCodes.Stloc_1;

			public static readonly OpCode Stloc_2 = OpCodes.Stloc_2;

			public static readonly OpCode Stloc_3 = OpCodes.Stloc_3;

			public static readonly OpCode Stloc_S = OpCodes.Stloc_S;

			public static readonly OpCode Ldfld = OpCodes.Ldfld;

			public static readonly OpCode Ldflda = OpCodes.Ldflda;

			public static readonly OpCode Stfld = OpCodes.Stfld;

			public static readonly OpCode Ldsfld = OpCodes.Ldsfld;

			public static readonly OpCode Ldsflda = OpCodes.Ldsflda;

			public static readonly OpCode Stsfld = OpCodes.Stsfld;

			public static readonly OpCode Newarr = OpCodes.Newarr;

			public static readonly OpCode Ldlen = OpCodes.Ldlen;

			public static readonly OpCode Ldelem = OpCodes.Ldelem;

			public static readonly OpCode Ldelem_I1 = OpCodes.Ldelem_I1;

			public static readonly OpCode Ldelem_U1 = OpCodes.Ldelem_U1;

			public static readonly OpCode Ldelem_I2 = OpCodes.Ldelem_I2;

			public static readonly OpCode Ldelem_U2 = OpCodes.Ldelem_U2;

			public static readonly OpCode Ldelem_I4 = OpCodes.Ldelem_I4;

			public static readonly OpCode Ldelem_U4 = OpCodes.Ldelem_U4;

			public static readonly OpCode Ldelem_I8 = OpCodes.Ldelem_I8;

			public static readonly OpCode Ldelem_I = OpCodes.Ldelem_I;

			public static readonly OpCode Ldelem_R4 = OpCodes.Ldelem_R4;

			public static readonly OpCode Ldelem_R8 = OpCodes.Ldelem_R8;

			public static readonly OpCode Ldelem_Ref = OpCodes.Ldelem_Ref;

			public static readonly OpCode Ldelema = OpCodes.Ldelema;

			public static readonly OpCode Stelem = OpCodes.Stelem;

			public static readonly OpCode Stelem_I = OpCodes.Stelem_I;

			public static readonly OpCode Stelem_I1 = OpCodes.Stelem_I1;

			public static readonly OpCode Stelem_I2 = OpCodes.Stelem_I2;

			public static readonly OpCode Stelem_I4 = OpCodes.Stelem_I4;

			public static readonly OpCode Stelem_I8 = OpCodes.Stelem_I8;

			public static readonly OpCode Stelem_R4 = OpCodes.Stelem_R4;

			public static readonly OpCode Stelem_R8 = OpCodes.Stelem_R8;

			public static readonly OpCode Stelem_Ref = OpCodes.Stelem_Ref;

			public static readonly OpCode Ldind_I = OpCodes.Ldind_I;

			public static readonly OpCode Ldind_I1 = OpCodes.Ldind_I1;

			public static readonly OpCode Ldind_I2 = OpCodes.Ldind_I2;

			public static readonly OpCode Ldind_I4 = OpCodes.Ldind_I4;

			public static readonly OpCode Ldind_U1 = OpCodes.Ldind_U1;

			public static readonly OpCode Ldind_U2 = OpCodes.Ldind_U2;

			public static readonly OpCode Ldind_U4 = OpCodes.Ldind_U4;

			public static readonly OpCode Ldind_I8 = OpCodes.Ldind_I8;

			public static readonly OpCode Ldind_R4 = OpCodes.Ldind_R4;

			public static readonly OpCode Ldind_R8 = OpCodes.Ldind_R8;

			public static readonly OpCode Ldind_Ref = OpCodes.Ldind_Ref;

			public static readonly OpCode Stind_I = OpCodes.Stind_I;

			public static readonly OpCode Stind_I1 = OpCodes.Stind_I1;

			public static readonly OpCode Stind_I2 = OpCodes.Stind_I2;

			public static readonly OpCode Stind_I4 = OpCodes.Stind_I4;

			public static readonly OpCode Stind_I8 = OpCodes.Stind_I8;

			public static readonly OpCode Stind_R4 = OpCodes.Stind_R4;

			public static readonly OpCode Stind_R8 = OpCodes.Stind_R8;

			public static readonly OpCode Stind_Ref = OpCodes.Stind_Ref;

			public static readonly OpCode Ceq = OpCodes.Ceq;

			public static readonly OpCode Cgt = OpCodes.Cgt;

			public static readonly OpCode Cgt_Un = OpCodes.Cgt_Un;

			public static readonly OpCode Clt = OpCodes.Clt;

			public static readonly OpCode Clt_Un = OpCodes.Clt_Un;

			public static readonly OpCode Br = OpCodes.Br;

			public static readonly OpCode Br_S = OpCodes.Br_S;

			public static readonly OpCode Brfalse = OpCodes.Brfalse;

			public static readonly OpCode Brfalse_S = OpCodes.Brfalse_S;

			public static readonly OpCode Brtrue = OpCodes.Brtrue;

			public static readonly OpCode Brtrue_S = OpCodes.Brtrue_S;

			public static readonly OpCode Beq = OpCodes.Beq;

			public static readonly OpCode Beq_S = OpCodes.Beq_S;

			public static readonly OpCode Bne_Un = OpCodes.Bne_Un;

			public static readonly OpCode Bne_Un_S = OpCodes.Bne_Un_S;

			public static readonly OpCode Bge = OpCodes.Bge;

			public static readonly OpCode Bge_S = OpCodes.Bge_S;

			public static readonly OpCode Bge_Un = OpCodes.Bge_Un;

			public static readonly OpCode Bge_Un_S = OpCodes.Bge_Un_S;

			public static readonly OpCode Bgt = OpCodes.Bgt;

			public static readonly OpCode Bgt_S = OpCodes.Bgt_S;

			public static readonly OpCode Bgt_Un = OpCodes.Bgt_Un;

			public static readonly OpCode Bgt_Un_S = OpCodes.Bgt_Un_S;

			public static readonly OpCode Ble = OpCodes.Ble;

			public static readonly OpCode Ble_S = OpCodes.Ble_S;

			public static readonly OpCode Ble_Un = OpCodes.Ble_Un;

			public static readonly OpCode Ble_Un_S = OpCodes.Ble_Un_S;

			public static readonly OpCode Blt = OpCodes.Blt;

			public static readonly OpCode Blt_S = OpCodes.Blt_S;

			public static readonly OpCode Blt_Un = OpCodes.Blt_Un;

			public static readonly OpCode Blt_Un_S = OpCodes.Blt_Un_S;

			public static readonly OpCode Conv_I = OpCodes.Conv_I;

			public static readonly OpCode Conv_I1 = OpCodes.Conv_I1;

			public static readonly OpCode Conv_I2 = OpCodes.Conv_I2;

			public static readonly OpCode Conv_I4 = OpCodes.Conv_I4;

			public static readonly OpCode Conv_I8 = OpCodes.Conv_I8;

			public static readonly OpCode Conv_U = OpCodes.Conv_U;

			public static readonly OpCode Conv_U1 = OpCodes.Conv_U1;

			public static readonly OpCode Conv_U2 = OpCodes.Conv_U2;

			public static readonly OpCode Conv_U4 = OpCodes.Conv_U4;

			public static readonly OpCode Conv_U8 = OpCodes.Conv_U8;

			public static readonly OpCode Conv_R4 = OpCodes.Conv_R4;

			public static readonly OpCode Conv_R8 = OpCodes.Conv_R8;

			public static readonly OpCode Conv_R_Un = OpCodes.Conv_R_Un;

			public static readonly OpCode Conv_Ovf_I = OpCodes.Conv_Ovf_I;

			public static readonly OpCode Conv_Ovf_I1 = OpCodes.Conv_Ovf_I1;

			public static readonly OpCode Conv_Ovf_I2 = OpCodes.Conv_Ovf_I2;

			public static readonly OpCode Conv_Ovf_I4 = OpCodes.Conv_Ovf_I4;

			public static readonly OpCode Conv_Ovf_I8 = OpCodes.Conv_Ovf_I8;

			public static readonly OpCode Conv_Ovf_U = OpCodes.Conv_Ovf_U;

			public static readonly OpCode Conv_Ovf_U1 = OpCodes.Conv_Ovf_U1;

			public static readonly OpCode Conv_Ovf_U2 = OpCodes.Conv_Ovf_U2;

			public static readonly OpCode Conv_Ovf_U4 = OpCodes.Conv_Ovf_U4;

			public static readonly OpCode Conv_Ovf_U8 = OpCodes.Conv_Ovf_U8;

			public static readonly OpCode Conv_Ovf_I_Un = OpCodes.Conv_Ovf_I_Un;

			public static readonly OpCode Conv_Ovf_I1_Un = OpCodes.Conv_Ovf_I1_Un;

			public static readonly OpCode Conv_Ovf_I2_Un = OpCodes.Conv_Ovf_I2_Un;

			public static readonly OpCode Conv_Ovf_I4_Un = OpCodes.Conv_Ovf_I4_Un;

			public static readonly OpCode Conv_Ovf_I8_Un = OpCodes.Conv_Ovf_I8_Un;

			public static readonly OpCode Conv_Ovf_U_Un = OpCodes.Conv_Ovf_U_Un;

			public static readonly OpCode Conv_Ovf_U1_Un = OpCodes.Conv_Ovf_U1_Un;

			public static readonly OpCode Conv_Ovf_U2_Un = OpCodes.Conv_Ovf_U2_Un;

			public static readonly OpCode Conv_Ovf_U4_Un = OpCodes.Conv_Ovf_U4_Un;

			public static readonly OpCode Conv_Ovf_U8_Un = OpCodes.Conv_Ovf_U8_Un;

			public static readonly OpCode Add = OpCodes.Add;

			public static readonly OpCode Sub = OpCodes.Sub;

			public static readonly OpCode Neg = OpCodes.Neg;

			public static readonly OpCode Mul = OpCodes.Mul;

			public static readonly OpCode Div = OpCodes.Div;

			public static readonly OpCode Div_Un = OpCodes.Div_Un;

			public static readonly OpCode Rem = OpCodes.Rem;

			public static readonly OpCode Rem_Un = OpCodes.Rem_Un;

			public static readonly OpCode Add_Ovf = OpCodes.Add_Ovf;

			public static readonly OpCode Add_Ovf_Un = OpCodes.Add_Ovf_Un;

			public static readonly OpCode Mul_Ovf = OpCodes.Mul_Ovf;

			public static readonly OpCode Mul_Ovf_Un = OpCodes.Mul_Ovf_Un;

			public static readonly OpCode Sub_Ovf = OpCodes.Sub_Ovf;

			public static readonly OpCode Sub_Ovf_Un = OpCodes.Sub_Ovf_Un;

			public static readonly OpCode And = OpCodes.And;

			public static readonly OpCode Or = OpCodes.Or;

			public static readonly OpCode Not = OpCodes.Not;

			public static readonly OpCode Xor = OpCodes.Xor;

			public static readonly OpCode Shl = OpCodes.Shl;

			public static readonly OpCode Shr = OpCodes.Shr;

			public static readonly OpCode Shr_Un = OpCodes.Shr_Un;

			public static readonly OpCode Ret = OpCodes.Ret;

			public static readonly OpCode Call = OpCodes.Call;

			public static readonly OpCode Callvirt = OpCodes.Callvirt;

			public static readonly OpCode Ldftn = OpCodes.Ldftn;

			public static readonly OpCode Ldvirtftn = OpCodes.Ldvirtftn;

			public static readonly OpCode Calli = OpCodes.Calli;

			public static readonly OpCode Jmp = OpCodes.Jmp;

			public static readonly OpCode Tailcall = OpCodes.Tailcall;

			public static readonly OpCode Switch = OpCodes.Switch;

			public static readonly OpCode Leave = OpCodes.Leave;

			public static readonly OpCode Leave_S = OpCodes.Leave_S;

			public static readonly OpCode Constrained = OpCodes.Constrained;

			public static readonly OpCode Throw = OpCodes.Throw;

			public static readonly OpCode Rethrow = OpCodes.Rethrow;

			public static readonly OpCode Ckfinite = OpCodes.Ckfinite;

			public static readonly OpCode Endfilter = OpCodes.Endfilter;

			public static readonly OpCode Endfinally = OpCodes.Endfinally;

			public static readonly OpCode Initobj = OpCodes.Initobj;

			public static readonly OpCode Newobj = OpCodes.Newobj;

			public static readonly OpCode Stobj = OpCodes.Stobj;

			public static readonly OpCode Ldobj = OpCodes.Ldobj;

			public static readonly OpCode Cpobj = OpCodes.Cpobj;

			public static readonly OpCode Castclass = OpCodes.Castclass;

			public static readonly OpCode Isinst = OpCodes.Isinst;

			public static readonly OpCode Box = OpCodes.Box;

			public static readonly OpCode Unbox = OpCodes.Unbox;

			public static readonly OpCode Unbox_Any = OpCodes.Unbox_Any;

			public static readonly OpCode Dup = OpCodes.Dup;

			public static readonly OpCode Pop = OpCodes.Pop;

			public static readonly OpCode Nop = OpCodes.Nop;

			public static readonly OpCode Break = OpCodes.Break;

			public static readonly OpCode Refanyval = OpCodes.Refanyval;

			public static readonly OpCode Mkrefany = OpCodes.Mkrefany;

			public static readonly OpCode Refanytype = OpCodes.Refanytype;

			public static readonly OpCode Ldtoken = OpCodes.Ldtoken;

			public static readonly OpCode Arglist = OpCodes.Arglist;

			public static readonly OpCode Unaligned = OpCodes.Unaligned;

			public static readonly OpCode Volatile = OpCodes.Volatile;

			public static readonly OpCode Localloc = OpCodes.Localloc;

			public static readonly OpCode Cpblk = OpCodes.Cpblk;

			public static readonly OpCode Initblk = OpCodes.Initblk;

			public static readonly OpCode Sizeof = OpCodes.Sizeof;

			public static readonly OpCode Readonly = OpCodes.Readonly;

			public static readonly OpCode Prefix7 = OpCodes.Prefix7;

			public static readonly OpCode Prefix6 = OpCodes.Prefix6;

			public static readonly OpCode Prefix5 = OpCodes.Prefix5;

			public static readonly OpCode Prefix4 = OpCodes.Prefix4;

			public static readonly OpCode Prefix3 = OpCodes.Prefix3;

			public static readonly OpCode Prefix2 = OpCodes.Prefix2;

			public static readonly OpCode Prefix1 = OpCodes.Prefix1;

			public static readonly OpCode Prefixref = OpCodes.Prefixref;
		}

		protected readonly OperationsContainer operations;

		private readonly OperationOptions Option = new OperationOptions();

		public static readonly Type NativeInt = typeof(I);

		public static readonly Type NativeIntUnsigned = typeof(U);

		public static readonly Type ObjectReference = typeof(O);

		protected EmitterBase(OperationsContainer operations)
		{
			this.operations = operations;
		}

		protected EmitterBase(EmitterBase emitterBase) : this(emitterBase.operations)
		{
		}

		private void Emit(OpCode opcode)
		{
			operations.Add(opcode, delegate(ILGenerator il)
			{
				il.Emit(opcode);
			});
			Option.Clear();
		}

		private void Emit(OpCode opcode, byte u1)
		{
			operations.Add(opcode, delegate(ILGenerator il)
			{
				il.Emit(opcode, u1);
			});
			Option.Clear();
		}

		private void Emit(OpCode opcode, sbyte i1)
		{
			operations.Add(opcode, delegate(ILGenerator il)
			{
				il.Emit(opcode, i1);
			});
			Option.Clear();
		}

		private void Emit(OpCode opcode, short i2)
		{
			operations.Add(opcode, delegate(ILGenerator il)
			{
				il.Emit(opcode, i2);
			});
			Option.Clear();
		}

		private void Emit(OpCode opcode, int i4)
		{
			operations.Add(opcode, delegate(ILGenerator il)
			{
				il.Emit(opcode, i4);
			});
			Option.Clear();
		}

		private void Emit(OpCode opcode, long i8)
		{
			operations.Add(opcode, delegate(ILGenerator il)
			{
				il.Emit(opcode, i8);
			});
			Option.Clear();
		}

		private void Emit(OpCode opcode, float r4)
		{
			operations.Add(opcode, delegate(ILGenerator il)
			{
				il.Emit(opcode, r4);
			});
			Option.Clear();
		}

		private void Emit(OpCode opcode, double r8)
		{
			operations.Add(opcode, delegate(ILGenerator il)
			{
				il.Emit(opcode, r8);
			});
			Option.Clear();
		}

		private void Emit(OpCode opcode, string str)
		{
			operations.Add(opcode, delegate(ILGenerator il)
			{
				il.Emit(opcode, str);
			});
			Option.Clear();
		}

		private void Emit(OpCode opcode, Local local)
		{
			operations.Add(opcode, delegate(ILGenerator il)
			{
				il.Emit(opcode, local.localBuiler);
			});
			Option.Clear();
		}

		private void Emit(OpCode opcode, MethodInfo method)
		{
			operations.Add(opcode, delegate(ILGenerator il)
			{
				il.Emit(opcode, method);
			});
			Option.Clear();
		}

		private void Emit(OpCode opcode, ConstructorInfo constructor)
		{
			operations.Add(opcode, delegate(ILGenerator il)
			{
				il.Emit(opcode, constructor);
			});
			Option.Clear();
		}

		private void Emit(OpCode opcode, FieldInfo field)
		{
			operations.Add(opcode, delegate(ILGenerator il)
			{
				il.Emit(opcode, field);
			});
			Option.Clear();
		}

		private void Emit(OpCode opcode, Type type)
		{
			operations.Add(opcode, delegate(ILGenerator il)
			{
				il.Emit(opcode, type);
			});
			Option.Clear();
		}

		public void Ldnull()
		{
			Emit(Op.Ldnull);
		}

		public void Ldstr(string s)
		{
			Emit(Op.Ldstr, s);
		}

		public void Ldc(int c)
		{
			var flag = c == -1;
			if (flag)
			{
				Emit(Op.Ldc_I4_M1);
			}
			else
			{
				var flag2 = c == 0;
				if (flag2)
				{
					Emit(Op.Ldc_I4_0);
				}
				else
				{
					var flag3 = c == 1;
					if (flag3)
					{
						Emit(Op.Ldc_I4_1);
					}
					else
					{
						var flag4 = c == 2;
						if (flag4)
						{
							Emit(Op.Ldc_I4_2);
						}
						else
						{
							var flag5 = c == 3;
							if (flag5)
							{
								Emit(Op.Ldc_I4_3);
							}
							else
							{
								var flag6 = c == 4;
								if (flag6)
								{
									Emit(Op.Ldc_I4_4);
								}
								else
								{
									var flag7 = c == 5;
									if (flag7)
									{
										Emit(Op.Ldc_I4_5);
									}
									else
									{
										var flag8 = c == 6;
										if (flag8)
										{
											Emit(Op.Ldc_I4_6);
										}
										else
										{
											var flag9 = c == 7;
											if (flag9)
											{
												Emit(Op.Ldc_I4_7);
											}
											else
											{
												var flag10 = c == 8;
												if (flag10)
												{
													Emit(Op.Ldc_I4_8);
												}
												else
												{
													var flag11 = c >= -128 && c < 128;
													if (flag11)
													{
														Emit(Op.Ldc_I4_S, (sbyte)c);
													}
													else
													{
														Emit(Op.Ldc_I4, c);
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public void Ldc(long c)
		{
			Emit(Op.Ldc_I8, c);
		}

		public void Ldc(float c)
		{
			Emit(Op.Ldc_R4, c);
		}

		public void Ldc(double c)
		{
			Emit(Op.Ldc_R8, c);
		}

		public void Ldarg(short i)
		{
			var flag = i == 0;
			if (flag)
			{
				Emit(Op.Ldarg_0);
			}
			else
			{
				var flag2 = i == 1;
				if (flag2)
				{
					Emit(Op.Ldarg_1);
				}
				else
				{
					var flag3 = i == 2;
					if (flag3)
					{
						Emit(Op.Ldarg_2);
					}
					else
					{
						var flag4 = i == 3;
						if (flag4)
						{
							Emit(Op.Ldarg_3);
						}
						else
						{
							var flag5 = i < 256;
							if (flag5)
							{
								Emit(Op.Ldarg_S, (byte)i);
							}
							else
							{
								Emit(Op.Ldarg, i);
							}
						}
					}
				}
			}
		}

		public void Ldarga(short i)
		{
			var flag = i < 256;
			if (flag)
			{
				Emit(Op.Ldarga_S, (byte)i);
			}
			else
			{
				Emit(Op.Ldarga, i);
			}
		}

		public void Starg(short i)
		{
			var flag = i < 256;
			if (flag)
			{
				Emit(Op.Starg_S, (byte)i);
			}
			else
			{
				Emit(Op.Starg, i);
			}
		}

		public Local DeclareLocal(Type localType, bool pinned = false)
		{
			return operations.DeclareLocal(localType, pinned);
		}

		public Local DeclareLocal<T>(bool pinned = false)
		{
			return operations.DeclareLocal(typeof(T), pinned);
		}

		public void Ldloc(Local local)
		{
			var localIndex = local.localBuiler.LocalIndex;
			var flag = localIndex == 0;
			if (flag)
			{
				Emit(Op.Ldloc_0);
			}
			else
			{
				var flag2 = localIndex == 1;
				if (flag2)
				{
					Emit(Op.Ldloc_1);
				}
				else
				{
					var flag3 = localIndex == 2;
					if (flag3)
					{
						Emit(Op.Ldloc_2);
					}
					else
					{
						var flag4 = localIndex == 3;
						if (flag4)
						{
							Emit(Op.Ldloc_3);
						}
						else
						{
							var flag5 = localIndex < 256;
							if (flag5)
							{
								Emit(Op.Ldloc_S, local);
							}
							else
							{
								Emit(Op.Ldloc, local);
							}
						}
					}
				}
			}
		}

		public void Ldloca(Local local)
		{
			var localIndex = local.localBuiler.LocalIndex;
			var flag = localIndex < 256;
			if (flag)
			{
				Emit(Op.Ldloca_S, local);
			}
			else
			{
				Emit(Op.Ldloca, local);
			}
		}

		public void Stloc(Local local)
		{
			var localIndex = local.localBuiler.LocalIndex;
			var flag = localIndex == 0;
			if (flag)
			{
				Emit(Op.Stloc_0);
			}
			else
			{
				var flag2 = localIndex == 1;
				if (flag2)
				{
					Emit(Op.Stloc_1);
				}
				else
				{
					var flag3 = localIndex == 2;
					if (flag3)
					{
						Emit(Op.Stloc_2);
					}
					else
					{
						var flag4 = localIndex == 3;
						if (flag4)
						{
							Emit(Op.Stloc_3);
						}
						else
						{
							var flag5 = localIndex < 256;
							if (flag5)
							{
								Emit(Op.Stloc_S, local);
							}
							else
							{
								Emit(Op.Stloc, local);
							}
						}
					}
				}
			}
		}

		public void Ldfld(FieldInfo field)
		{
			Emit(field.IsStatic ? Op.Ldsfld : Op.Ldfld, field);
		}

		public void Ldflda(FieldInfo field)
		{
			Emit(field.IsStatic ? Op.Ldsflda : Op.Ldflda, field);
		}

		public void Stfld(FieldInfo field)
		{
			Emit(field.IsStatic ? Op.Stsfld : Op.Stfld, field);
		}

		public void Newarr(Type t)
		{
			Emit(Op.Newarr, t);
		}

		public void Ldlen()
		{
			Emit(Op.Ldlen);
		}

		public void Ldelem(Type t)
		{
			var flag = t == typeof(sbyte);
			if (flag)
			{
				Emit(Op.Ldelem_I1);
			}
			else
			{
				var flag2 = t == typeof(byte);
				if (flag2)
				{
					Emit(Op.Ldelem_U1);
				}
				else
				{
					var flag3 = t == typeof(short);
					if (flag3)
					{
						Emit(Op.Ldelem_I2);
					}
					else
					{
						var flag4 = t == typeof(ushort);
						if (flag4)
						{
							Emit(Op.Ldelem_U2);
						}
						else
						{
							var flag5 = t == typeof(int);
							if (flag5)
							{
								Emit(Op.Ldelem_I4);
							}
							else
							{
								var flag6 = t == typeof(uint);
								if (flag6)
								{
									Emit(Op.Ldelem_U4);
								}
								else
								{
									var flag7 = t == typeof(long);
									if (flag7)
									{
										Emit(Op.Ldelem_I8);
									}
									else
									{
										var flag8 = t == typeof(float);
										if (flag8)
										{
											Emit(Op.Ldelem_R4);
										}
										else
										{
											var flag9 = t == typeof(double);
											if (flag9)
											{
												Emit(Op.Ldelem_R8);
											}
											else
											{
												var flag10 = t == typeof(I);
												if (flag10)
												{
													Emit(Op.Ldelem_I);
												}
												else
												{
													var flag11 = t == typeof(O);
													if (flag11)
													{
														Emit(Op.Ldelem_Ref);
													}
													else
													{
														Emit(Op.Ldelem, t);
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public void Stelem(Type t)
		{
			var flag = t == typeof(sbyte);
			if (flag)
			{
				Emit(Op.Stelem_I1);
			}
			else
			{
				var flag2 = t == typeof(short);
				if (flag2)
				{
					Emit(Op.Stelem_I2);
				}
				else
				{
					var flag3 = t == typeof(int);
					if (flag3)
					{
						Emit(Op.Stelem_I4);
					}
					else
					{
						var flag4 = t == typeof(long);
						if (flag4)
						{
							Emit(Op.Stelem_I8);
						}
						else
						{
							var flag5 = t == typeof(float);
							if (flag5)
							{
								Emit(Op.Stelem_R4);
							}
							else
							{
								var flag6 = t == typeof(double);
								if (flag6)
								{
									Emit(Op.Stelem_R8);
								}
								else
								{
									var flag7 = t == typeof(I);
									if (flag7)
									{
										Emit(Op.Stelem_I);
									}
									else
									{
										var flag8 = t == typeof(O);
										if (flag8)
										{
											Emit(Op.Stelem_Ref);
										}
										else
										{
											Emit(Op.Stelem, t);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public void Ldelema(Type t)
		{
			Emit(Op.Ldelema, t);
		}

		public void Ldelem<T>()
		{
			Ldelem(typeof(T));
		}

		public void Stelem<T>()
		{
			Stelem(typeof(T));
		}

		public void Ldelema<T>()
		{
			Ldelema(typeof(T));
		}

		public void Ldind(Type t)
		{
			var flag = t == typeof(sbyte);
			if (flag)
			{
				Emit(Op.Ldind_I1);
			}
			else
			{
				var flag2 = t == typeof(byte);
				if (flag2)
				{
					Emit(Op.Ldind_U1);
				}
				else
				{
					var flag3 = t == typeof(short);
					if (flag3)
					{
						Emit(Op.Ldind_I2);
					}
					else
					{
						var flag4 = t == typeof(ushort);
						if (flag4)
						{
							Emit(Op.Ldind_U2);
						}
						else
						{
							var flag5 = t == typeof(int);
							if (flag5)
							{
								Emit(Op.Ldind_I4);
							}
							else
							{
								var flag6 = t == typeof(uint);
								if (flag6)
								{
									Emit(Op.Ldind_U4);
								}
								else
								{
									var flag7 = t == typeof(long);
									if (flag7)
									{
										Emit(Op.Ldind_I8);
									}
									else
									{
										var flag8 = t == typeof(float);
										if (flag8)
										{
											Emit(Op.Ldind_R4);
										}
										else
										{
											var flag9 = t == typeof(double);
											if (flag9)
											{
												Emit(Op.Ldind_R8);
											}
											else
											{
												var flag10 = t == typeof(I);
												if (flag10)
												{
													Emit(Op.Ldind_I);
												}
												else
												{
													var flag11 = t == typeof(O);
													if (!flag11)
													{
														throw new ArgumentException("Invalid Type specified.");
													}
													Emit(Op.Ldind_Ref);
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public void Stind(Type t)
		{
			var flag = t == typeof(sbyte);
			if (flag)
			{
				Emit(Op.Stind_I1);
			}
			else
			{
				var flag2 = t == typeof(short);
				if (flag2)
				{
					Emit(Op.Stind_I2);
				}
				else
				{
					var flag3 = t == typeof(int);
					if (flag3)
					{
						Emit(Op.Stind_I4);
					}
					else
					{
						var flag4 = t == typeof(long);
						if (flag4)
						{
							Emit(Op.Stind_I8);
						}
						else
						{
							var flag5 = t == typeof(float);
							if (flag5)
							{
								Emit(Op.Stind_R4);
							}
							else
							{
								var flag6 = t == typeof(double);
								if (flag6)
								{
									Emit(Op.Stind_R8);
								}
								else
								{
									var flag7 = t == typeof(I);
									if (flag7)
									{
										Emit(Op.Stind_I);
									}
									else
									{
										var flag8 = t == typeof(O);
										if (!flag8)
										{
											throw new ArgumentException("Invalid Type specified.");
										}
										Emit(Op.Stind_Ref);
									}
								}
							}
						}
					}
				}
			}
		}

		public void Ldind<T>()
		{
			Ldind(typeof(T));
		}

		public void Stind<T>()
		{
			Stind(typeof(T));
		}

		public void Ceq()
		{
			Emit(Op.Ceq);
		}

		public void Cgt(bool CompareSigned = true)
		{
			Emit(CompareSigned ? Op.Cgt : Op.Cgt_Un);
		}

		public void Clt(bool CompareSigned = true)
		{
			Emit(CompareSigned ? Op.Clt : Op.Clt_Un);
		}

		public Target DefineLabel()
		{
			return operations.DefineLabel();
		}

		public void MarkLabel(Target target)
		{
			operations.MarkLabel(target);
		}

		private void EmitBr(OpCode opcode, OpCode opcode_s, Target target)
		{
			operations.Add(opcode, opcode_s, target);
		}

		public void Br(Target l)
		{
			EmitBr(Op.Br, Op.Br_S, l);
		}

		public void Brtrue(Target l)
		{
			EmitBr(Op.Brtrue, Op.Brtrue_S, l);
		}

		public void Brfalse(Target l)
		{
			EmitBr(Op.Brfalse, Op.Brfalse_S, l);
		}

		public void Beq(Target l)
		{
			EmitBr(Op.Beq, Op.Beq_S, l);
		}

		public void Bne_Un(Target l)
		{
			EmitBr(Op.Bne_Un, Op.Bne_Un_S, l);
		}

		public void Bge(Target l, bool CompareSigned = true)
		{
			if (CompareSigned)
			{
				EmitBr(Op.Bge, Op.Bge_S, l);
			}
			else
			{
				EmitBr(Op.Bge_Un, Op.Bge_Un_S, l);
			}
		}

		public void Bgt(Target l, bool CompareSigned = true)
		{
			if (CompareSigned)
			{
				EmitBr(Op.Bgt, Op.Bgt_S, l);
			}
			else
			{
				EmitBr(Op.Bgt_Un, Op.Bgt_Un_S, l);
			}
		}

		public void Ble(Target l, bool CompareSigned = true)
		{
			if (CompareSigned)
			{
				EmitBr(Op.Ble, Op.Ble_S, l);
			}
			else
			{
				EmitBr(Op.Ble_Un, Op.Ble_Un_S, l);
			}
		}

		public void Blt(Target l, bool CompareSigned = true)
		{
			if (CompareSigned)
			{
				EmitBr(Op.Blt, Op.Blt_S, l);
			}
			else
			{
				EmitBr(Op.Blt_Un, Op.Blt_Un_S, l);
			}
		}

		public void Conv<T>(bool OverFlowException = false, bool OpperandSigned = true)
		{
			Conv(typeof(T), OverFlowException, OpperandSigned);
		}

		public void Conv(Type t, bool OverflowException = false, bool OpperandSigned = true)
		{
			var flag = !OverflowException;
			if (flag)
			{
				if (OpperandSigned)
				{
					var flag2 = t == typeof(sbyte);
					if (flag2)
					{
						Emit(Op.Conv_I1);
					}
					else
					{
						var flag3 = t == typeof(byte);
						if (flag3)
						{
							Emit(Op.Conv_U1);
						}
						else
						{
							var flag4 = t == typeof(short);
							if (flag4)
							{
								Emit(Op.Conv_I2);
							}
							else
							{
								var flag5 = t == typeof(ushort);
								if (flag5)
								{
									Emit(Op.Conv_U2);
								}
								else
								{
									var flag6 = t == typeof(int);
									if (flag6)
									{
										Emit(Op.Conv_I4);
									}
									else
									{
										var flag7 = t == typeof(uint);
										if (flag7)
										{
											Emit(Op.Conv_U4);
										}
										else
										{
											var flag8 = t == typeof(long);
											if (flag8)
											{
												Emit(Op.Conv_I8);
											}
											else
											{
												var flag9 = t == typeof(ulong);
												if (flag9)
												{
													Emit(Op.Conv_U8);
												}
												else
												{
													var flag10 = t == typeof(float);
													if (flag10)
													{
														Emit(Op.Conv_R4);
													}
													else
													{
														var flag11 = t == typeof(double);
														if (flag11)
														{
															Emit(Op.Conv_R8);
														}
														else
														{
															var flag12 = t == typeof(I);
															if (flag12)
															{
																Emit(Op.Conv_I);
															}
															else
															{
																var flag13 = t == typeof(U);
																if (!flag13)
																{
																	throw new ArgumentException("Invalid Type specified.");
																}
																Emit(Op.Conv_U);
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
				else
				{
					var flag14 = t == typeof(float);
					if (!flag14)
					{
						throw new ArgumentException("Invalid Type specified.");
					}
					Emit(Op.Conv_R_Un);
				}
			}
			else if (OpperandSigned)
			{
				var flag15 = t == typeof(sbyte);
				if (flag15)
				{
					Emit(Op.Conv_Ovf_I1);
				}
				else
				{
					var flag16 = t == typeof(byte);
					if (flag16)
					{
						Emit(Op.Conv_Ovf_U1);
					}
					else
					{
						var flag17 = t == typeof(short);
						if (flag17)
						{
							Emit(Op.Conv_Ovf_I2);
						}
						else
						{
							var flag18 = t == typeof(ushort);
							if (flag18)
							{
								Emit(Op.Conv_Ovf_U2);
							}
							else
							{
								var flag19 = t == typeof(int);
								if (flag19)
								{
									Emit(Op.Conv_Ovf_I4);
								}
								else
								{
									var flag20 = t == typeof(uint);
									if (flag20)
									{
										Emit(Op.Conv_Ovf_U4);
									}
									else
									{
										var flag21 = t == typeof(long);
										if (flag21)
										{
											Emit(Op.Conv_Ovf_I8);
										}
										else
										{
											var flag22 = t == typeof(ulong);
											if (flag22)
											{
												Emit(Op.Conv_Ovf_U8);
											}
											else
											{
												var flag23 = t == typeof(I);
												if (flag23)
												{
													Emit(Op.Conv_Ovf_I);
												}
												else
												{
													var flag24 = t == typeof(U);
													if (!flag24)
													{
														throw new ArgumentException("Invalid Type specified.");
													}
													Emit(Op.Conv_Ovf_U);
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			else
			{
				var flag25 = t == typeof(sbyte);
				if (flag25)
				{
					Emit(Op.Conv_Ovf_I1_Un);
				}
				else
				{
					var flag26 = t == typeof(byte);
					if (flag26)
					{
						Emit(Op.Conv_Ovf_U1_Un);
					}
					else
					{
						var flag27 = t == typeof(short);
						if (flag27)
						{
							Emit(Op.Conv_Ovf_I2_Un);
						}
						else
						{
							var flag28 = t == typeof(ushort);
							if (flag28)
							{
								Emit(Op.Conv_Ovf_U2_Un);
							}
							else
							{
								var flag29 = t == typeof(int);
								if (flag29)
								{
									Emit(Op.Conv_Ovf_I4_Un);
								}
								else
								{
									var flag30 = t == typeof(uint);
									if (flag30)
									{
										Emit(Op.Conv_Ovf_U4_Un);
									}
									else
									{
										var flag31 = t == typeof(long);
										if (flag31)
										{
											Emit(Op.Conv_Ovf_I8_Un);
										}
										else
										{
											var flag32 = t == typeof(ulong);
											if (flag32)
											{
												Emit(Op.Conv_Ovf_U8_Un);
											}
											else
											{
												var flag33 = t == typeof(I);
												if (flag33)
												{
													Emit(Op.Conv_Ovf_I_Un);
												}
												else
												{
													var flag34 = t == typeof(U);
													if (!flag34)
													{
														throw new ArgumentException("Invalid Type specified.");
													}
													Emit(Op.Conv_Ovf_U_Un);
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public void Neg()
		{
			Emit(Op.Neg);
		}

		public void Add(bool OverFlowException = false, bool OpperandSigned = true)
		{
			var flag = !OverFlowException;
			if (flag)
			{
				if (!OpperandSigned)
				{
					throw new InvalidOperationException("Op.Add_Un is invalid.");
				}
				Emit(Op.Add);
			}
			else if (OpperandSigned)
			{
				Emit(Op.Add_Ovf);
			}
			else
			{
				Emit(Op.Add_Ovf_Un);
			}
		}

		public void Sub(bool OverFlowException = false, bool OpperandSigned = true)
		{
			var flag = !OverFlowException;
			if (flag)
			{
				if (!OpperandSigned)
				{
					throw new InvalidOperationException("Op.Sub_Un is invalid.");
				}
				Emit(Op.Sub);
			}
			else if (OpperandSigned)
			{
				Emit(Op.Sub_Ovf);
			}
			else
			{
				Emit(Op.Sub_Ovf_Un);
			}
		}

		public void Mul(bool OverFlowException = false, bool OpperandSigned = true)
		{
			var flag = !OverFlowException;
			if (flag)
			{
				if (!OpperandSigned)
				{
					throw new InvalidOperationException("Op.Mul_Un is invalid.");
				}
				Emit(Op.Mul);
			}
			else if (OpperandSigned)
			{
				Emit(Op.Mul_Ovf);
			}
			else
			{
				Emit(Op.Mul_Ovf_Un);
			}
		}

		public void Div(bool OpperandSigned = true)
		{
			if (OpperandSigned)
			{
				Emit(Op.Div);
			}
			else
			{
				Emit(Op.Div_Un);
			}
		}

		public void Rem(bool OpperandSigned = true)
		{
			if (OpperandSigned)
			{
				Emit(Op.Rem);
			}
			else
			{
				Emit(Op.Rem_Un);
			}
		}

		public void And()
		{
			Emit(Op.And);
		}

		public void Or()
		{
			Emit(Op.Or);
		}

		public void Not()
		{
			Emit(Op.Not);
		}

		public void Xor()
		{
			Emit(Op.Xor);
		}

		public void Shl()
		{
			Emit(Op.Shl);
		}

		public void Shr(bool SignExtend = true)
		{
			if (SignExtend)
			{
				Emit(Op.Shr);
			}
			else
			{
				Emit(Op.Shr_Un);
			}
		}

		public void Call(MethodInfo method)
		{
			Emit(Op.Call, method);
		}

		public void Callvirt(MethodInfo method)
		{
			Emit(Op.Callvirt, method);
		}

		public void Ret()
		{
			Emit(Op.Ret);
		}

		public void Initobj<T>()
		{
			Initobj(typeof(T));
		}

		public void Stobj<T>()
		{
			Stobj(typeof(T));
		}

		public void Newobj<T>(params Type[] ctorArgs)
		{
			Newobj(typeof(T), ctorArgs);
		}

		public void Initobj(Type t)
		{
			Emit(Op.Initobj, t);
		}

		public void Stobj(Type t)
		{
			Emit(Op.Stobj, t);
		}

		public void Newobj(Type t, params Type[] ctorArgs)
		{
			Newobj(t.GetConstructor(ctorArgs));
		}

		public void Newobj(ConstructorInfo ctor)
		{
			Emit(Op.Newobj, ctor);
		}

		public void Pop()
		{
			Emit(Op.Pop);
		}

		public void Dup()
		{
			Emit(Op.Dup);
		}
	}
	public static class OpCode_Extension
	{
		public class DumpInfo
		{
			public readonly string Value;

			public readonly string Name;

			public readonly string Size;

			public readonly string OperandType;

			public readonly string StackPop;

			public readonly string StackPush;

			public readonly string FlowControl;

			public readonly string OpCodeType;

			internal DumpInfo(OpCode op, bool simplify)
			{
				Value = ((op.Size == 1) ? (op.Value.ToString("X2") + "h") : (op.Value.ToString("X4") + "h"));
				Name = op.Name;
				Size = (op.Size + "+" + op.GetOperandSize()).Replace("+0", "");
				OperandType = op.OperandType.ToString().Replace("Inline", "").Replace("None", "").Replace("Short", "");
				StackPop = fixpoppush(op.StackBehaviourPop.ToString(), "pop");
				StackPush = fixpoppush(op.StackBehaviourPush.ToString(), "push");
				FlowControl = op.FlowControl.ToString();
				OpCodeType = op.OpCodeType.ToString();
				if (simplify)
				{
					OperandType = OperandType.Replace("Inline", "").Replace("None", "").Replace("Short", "");
					StackPop = fixpoppush(StackPop, "pop");
					StackPush = fixpoppush(StackPush, "push");
					FlowControl = ((op.OpCodeType == System.Reflection.Emit.OpCodeType.Nternal) ? "zMeta" : op.FlowControl.ToString().Replace("Cond_Branch", "Branch_Cond").Replace("Next", "").Replace("Meta", "xMeta"));
				}
			}

			private static string fixpoppush(string v, string poppush)
			{
				return v.ToLower().Replace(poppush + "0", "").Replace(poppush + "1", poppush + " ").Replace(poppush + "ref", poppush + "r").Replace("var" + poppush, poppush + "...").Replace("i8", "l").Replace("r4", "f").Replace("r8", "d").Replace("_", " ");
			}
		}

		public static int GetOperandSize(this OpCode op)
		{
			switch (op.OperandType)
			{
			case OperandType.InlineBrTarget:
			case OperandType.InlineField:
			case OperandType.InlineI:
			case OperandType.InlineMethod:
			case OperandType.InlineSig:
			case OperandType.InlineString:
			case OperandType.InlineSwitch:
			case OperandType.InlineTok:
			case OperandType.InlineType:
			case OperandType.ShortInlineR:
			{
				var result = 4;
				return result;
			}
			case OperandType.InlineI8:
			case OperandType.InlineR:
			{
				var result = 8;
				return result;
			}
			case OperandType.InlineNone:
			{
				var result = 0;
				return result;
			}
			case OperandType.InlineVar:
			{
				var result = 2;
				return result;
			}
			case OperandType.ShortInlineBrTarget:
			case OperandType.ShortInlineI:
			case OperandType.ShortInlineVar:
			{
				var result = 1;
				return result;
			}
			}
			throw new ArgumentOutOfRangeException();
		}

		public static int GetInstructionSize(this OpCode op)
		{
			return op.Size + op.GetOperandSize();
		}

		public static DumpInfo GetInfo(this OpCode op, bool simplify = false)
		{
			return new DumpInfo(op, simplify);
		}
	}
	public sealed class Local
	{
		internal LocalBuilder localBuiler;
	}
	public sealed class Target
	{
		internal Label Label;

		internal Operation Placeholder;
	}
	internal sealed class Operation
	{
		public Operation Prev = null;

		public Func<OpCode> OpCode;

		public Action<ILGenerator> Emit;

		public bool Placeholder = false;

		public int Offset
		{
			get
			{
				return (Prev == null) ? 0 : Prev.OffsetAfter;
			}
		}

		public int OffsetAfter
		{
			get
			{
				return Offset + (Placeholder ? 0 : OpCode().GetInstructionSize());
			}
		}
	}
	public sealed class OperationsContainer
	{
		private readonly List<Operation> Operations = new List<Operation>();

		public void Emit(ILGenerator il)
		{
			foreach (var current in Operations)
			{
				current.Emit(il);
			}
		}

		private Operation addOperation(Operation operation)
		{
			operation.Prev = Operations.LastOrDefault<Operation>();
			Operations.Add(operation);
			return operation;
		}

		private Operation newOperation(bool placeholder = false)
		{
			return addOperation(new Operation
			{
				Placeholder = placeholder
			});
		}

		public void Add(OpCode opcode, Action<ILGenerator> emit)
		{
			addOperation(new Operation
			{
				OpCode = (() => opcode),
				Emit = emit
			});
		}

		public void Add(OpCode opcode, OpCode opcode_s, Target target)
		{
			var operation = newOperation(false);
			operation.OpCode = delegate
			{
				var num = target.Placeholder.Offset - operation.OffsetAfter;
				return (-128 <= num && num < 128) ? opcode_s : opcode;
			};
			operation.Emit = delegate(ILGenerator il)
			{
				il.Emit(operation.OpCode(), target.Label);
			};
		}

		public Local DeclareLocal(Type localType, bool pinned = false)
		{
			var local = new Local();
			var operation = newOperation(true);
			operation.Emit = delegate(ILGenerator il)
			{
				local.localBuiler = il.DeclareLocal(localType, pinned);
			};
			return local;
		}

		public Target DefineLabel()
		{
			var target = new Target();
			var operation = newOperation(true);
			operation.Emit = delegate(ILGenerator il)
			{
				target.Label = il.DefineLabel();
			};
			return target;
		}

		public void MarkLabel(Target target)
		{
			var operation = newOperation(true);
			operation.Emit = delegate(ILGenerator il)
			{
				il.MarkLabel(target.Label);
			};
			target.Placeholder = operation;
		}
	}
}
