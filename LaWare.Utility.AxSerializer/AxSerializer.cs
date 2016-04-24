

using LaWare.Utility.Emitter;
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

namespace LaWare.Utility.AxSerializer
{
	public interface IAxSerializer
	{
		void WriteNull();

		void WriteOpenObject();

		void WriteCloseObject();

		void WriteOpenArray();

		void WriteCloseArray();

		void Write(ref bool source);

		void Write(ref byte source);

		void Write(ref char source);

		void Write(ref short source);

		void Write(ref int source);

		void Write(ref long source);

		void Write(ref ushort source);

		void Write(ref uint source);

		void Write(ref ulong source);

		void Write(ref float source);

		void Write(ref double source);

		void Write(ref decimal source);

		void Write(ref DateTime source);

		void Write(ref TimeSpan source);

		void Write(ref Guid source);

		void Write(ref string source);

		void Write(ref byte[] source);
	}
	public interface IAxDeserializer
	{
		bool ReadNull();

		void ReadOpenObject();

		void ReadCloseObject();

		void ReadOpenArray();

		void ReadCloseArray();

		void Read(out bool target);

		void Read(out byte target);

		void Read(out char target);

		void Read(out short target);

		void Read(out int target);

		void Read(out long target);

		void Read(out ushort target);

		void Read(out uint target);

		void Read(out ulong target);

		void Read(out float target);

		void Read(out double target);

		void Read(out decimal target);

		void Read(out DateTime target);

		void Read(out TimeSpan target);

		void Read(out Guid target);

		void Read(out string target);

		void Read(out byte[] target);
	}
	public static class AxSerializer
	{
		private delegate void ClassSerializer<T>(IAxSerializer s, T source);

		private delegate void StructSerializer<T>(IAxSerializer s, ref T source);

		private class Serializer : SerializerEmitterBase
		{
			private static readonly Dictionary<Type, MethodInfo> BasicSerializers = BasicMethods<IAxSerializer>("Write", "source");

			private static readonly MethodInfo WriteNull = typeof(IAxSerializer).GetMethod("WriteNull");

			private static readonly MethodInfo WriteOpenObject = typeof(IAxSerializer).GetMethod("WriteOpenObject");

			private static readonly MethodInfo WriteCloseObject = typeof(IAxSerializer).GetMethod("WriteCloseObject");

			private static readonly MethodInfo WriteOpenArray = typeof(IAxSerializer).GetMethod("WriteOpenArray");

			private static readonly MethodInfo WriteCloseArray = typeof(IAxSerializer).GetMethod("WriteCloseArray");

			private static readonly MethodInfo WriteInt32 = typeof(IAxSerializer).GetMethod("Write", new Type[]
			{
				typeof(int).MakeByRefType()
			});

			private static readonly Dictionary<Type, DynamicMethod> DynamicSerializers = new Dictionary<Type, DynamicMethod>();

			internal static DynamicMethod GetDynamicSerializer(Type t)
			{
				object dynamicCreateLock = DynamicCreateLock;
				DynamicMethod result;
				lock (dynamicCreateLock)
				{
					DynamicMethod dynamicMethod;
					bool flag2 = DynamicSerializers.TryGetValue(t, out dynamicMethod);
					if (flag2)
					{
						result = dynamicMethod;
					}
					else
					{
						result = CreateSerializer(t);
					}
				}
				return result;
			}

			private static SerializerInfo GetWriter(Type t)
			{
				return new SerializerInfo(t, BasicSerializers, new Func<Type, DynamicMethod>(GetDynamicSerializer));
			}

			private static DynamicMethod CreateSerializer(Type t)
			{
				return new Serializer(t).Method;
			}

			protected override DynamicMethod OnCreateMethod()
			{
				return DynamicSerializers[T] = (T.IsClass ? CreateDynamicMethod(typeof(void), "Write", new Type[]
				{
					typeof(IAxSerializer),
					T
				}) : CreateDynamicMethod(typeof(void), "Write", new Type[]
				{
					typeof(IAxSerializer),
					T.MakeByRefType()
				}));
			}

			private Serializer(Type t) : base(t)
			{
				bool canSerializeNull = CanSerializeNull;
				if (canSerializeNull)
				{
					bool isClass = T.IsClass;
					if (isClass)
					{
						Ldarg(1);
					}
					else
					{
						Ldarg(1);
						Ldfld(T.GetFieldPrivate("hasValue"));
					}
					Target target = DefineLabel();
					Brtrue(target);
					Ldarg(0);
					Callvirt(WriteNull);
					Ret();
					MarkLabel(target);
				}
				bool isNullable = IsNullable;
				if (isNullable)
				{
					FieldInfo f = T.GetFieldPrivate("value");
					SerializerInfo writer = GetWriter(f.FieldType);
					EmitFieldSerializer(writer
						, LdTarget: () => Ldarg(1)
						, LdAddress: () => Ldflda(f)
						, LdValue: () => Ldfld(f)
						);
					Ret();
				}
				else
				{
					bool isArray = T.IsArray;
					if (isArray)
					{
						var elementType = T.GetElementType();
						SerializerInfo writer2 = GetWriter(elementType);
						SerializerCall(WriteOpenArray);
						Local local = DeclareLocal(typeof(int), false);
						Ldarg(1);
						Ldlen();
						Conv<int>(false, true);
						Stloc(local);
						Ldarg(0);
						Ldloca(local);
						Callvirt(WriteInt32);
						using (IndexLoop loop = Loop(local))
						{
							this.EmitFieldSerializer(writer2
								, LdTarget: () =>
								{
									Ldarg(1);
									Ldloc(loop.Index);
								}
								, LdAddress: () =>
								{
									Ldelema(elementType);
								}
								, LdValue: delegate
								{
									Ldelem(ObjectReference);
								});
						}
						SerializerCall(WriteCloseArray);
						Ret();
					}
					else
					{
						SerializerCall(WriteOpenObject);
						FieldInfo[] fields = T.GetFields();
						for (int i = 0; i < fields.Length; i++)
						{
							FieldInfo f2 = fields[i];
							FieldInfo f = f2;
							SerializerInfo writer3 = GetWriter(f.FieldType);
							EmitFieldSerializer(writer3, delegate
							{
								Ldarg(1);
							}, delegate
							{
								Ldflda(f);
							}, delegate
							{
								Ldfld(f);
							});
						}
						SerializerCall(WriteCloseObject);
						Ret();
					}
				}
			}

			private void EmitFieldSerializer(SerializerInfo writer, Action LdTarget, Action LdAddress, Action LdValue)
			{
				bool isBasic = writer.IsBasic;
				if (isBasic)
				{
					Ldarg(0);
					LdTarget();
					LdAddress();
					Callvirt(writer.Method);
				}
				else
				{
					bool isClass = writer.IsClass;
					if (isClass)
					{
						Ldarg(0);
						LdTarget();
						LdValue();
						Call(writer.Method);
					}
					else
					{
						Ldarg(0);
						LdTarget();
						LdAddress();
						Call(writer.Method);
					}
				}
			}
		}

		private delegate T ClassDeserializer<T>(IAxDeserializer s);

		private delegate void StructDeserializer<T>(IAxDeserializer s, out T target);

		private class Deserializer : SerializerEmitterBase
		{
			private static readonly Dictionary<Type, MethodInfo> BasicDeserializers = BasicMethods<IAxDeserializer>("Read", "target");

			private static readonly MethodInfo ReadNull = typeof(IAxDeserializer).GetMethod("ReadNull");

			private static readonly MethodInfo ReadOpenObject = typeof(IAxDeserializer).GetMethod("ReadOpenObject");

			private static readonly MethodInfo ReadCloseObject = typeof(IAxDeserializer).GetMethod("ReadCloseObject");

			private static readonly MethodInfo ReadOpenArray = typeof(IAxDeserializer).GetMethod("ReadOpenArray");

			private static readonly MethodInfo ReadCloseArray = typeof(IAxDeserializer).GetMethod("ReadCloseArray");

			private static readonly MethodInfo ReadInt32 = typeof(IAxDeserializer).GetMethod("Read", new Type[]
			{
				typeof(int).MakeByRefType()
			});

			private static readonly Dictionary<Type, DynamicMethod> DynamicDeserializers = new Dictionary<Type, DynamicMethod>();

			internal static DynamicMethod GetDynamicDeserializer(Type t)
			{
				object dynamicCreateLock = DynamicCreateLock;
				DynamicMethod result;
				lock (dynamicCreateLock)
				{
					DynamicMethod dynamicMethod;
					bool flag2 = DynamicDeserializers.TryGetValue(t, out dynamicMethod);
					if (flag2)
					{
						result = dynamicMethod;
					}
					else
					{
						result = CreateDeserializer(t);
					}
				}
				return result;
			}

			private static SerializerInfo GetReader(Type t)
			{
				return new SerializerInfo(t, BasicDeserializers, new Func<Type, DynamicMethod>(GetDynamicDeserializer));
			}

			private static DynamicMethod CreateDeserializer(Type t)
			{
				return new Deserializer(t).Method;
			}

			protected override DynamicMethod OnCreateMethod()
			{
				return DynamicDeserializers[T] = (T.IsClass ? CreateDynamicMethod(T, "Read", new Type[]
				{
					typeof(IAxDeserializer)
				}) : CreateDynamicMethod(typeof(void), "Read", new Type[]
				{
					typeof(IAxDeserializer),
					T.MakeByRefType()
				}));
			}

			private Deserializer(Type t) : base(t)
			{
				bool canSerializeNull = CanSerializeNull;
				if (canSerializeNull)
				{
					Target target = DefineLabel();
					Ldarg(0);
					Callvirt(ReadNull);
					Brfalse(target);
					bool isClass = T.IsClass;
					if (isClass)
					{
						Ldnull();
						Ret();
					}
					else
					{
						Ldarg(1);
						Initobj(T);
						Ret();
					}
					MarkLabel(target);
				}
				bool isNullable = IsNullable;
				if (isNullable)
				{
					FieldInfo f = T.GetFieldPrivate("value");
					SerializerInfo reader = GetReader(f.FieldType);
					EmitFieldDeserializer(reader, delegate
					{
						Ldarg(1);
					}, delegate
					{
						Ldflda(f);
					}, delegate
					{
						Stfld(f);
					});
					Ldarg(1);
					Ldc(1);
					Stfld(T.GetFieldPrivate("hasValue"));
					Ret();
				}
				else
				{
					bool isArray = T.IsArray;
					if (isArray)
					{
						var elementType = T.GetElementType();
						SerializerInfo reader2 = GetReader(elementType);
						SerializerCall(ReadOpenArray);
						Local local = DeclareLocal<int>(false);
						Ldarg(0);
						Ldloca(local);
						Callvirt(ReadInt32);
						var target = DeclareLocal(T, false);
						Ldloc(local);
						Newarr(elementType);
						Stloc(target);
						using (IndexLoop loop = Loop(local))
						{
							EmitFieldDeserializer(reader2
								, LdTarget: () =>
								{
									Ldloc(target);
									Ldloc(loop.Index);
								}
								, LdAddress: () =>
								{
									Ldelema(elementType);
								}
								, StValue: () =>
								{
									Stelem(ObjectReference);
								});
						}
						SerializerCall(ReadCloseArray);
						Ldloc(target);
						Ret();
					}
					else
					{
						bool isClass2 = T.IsClass;
						if (isClass2)
						{
							var target = DeclareLocal(T, false);
							Newobj(T);
							Stloc(target);
							SerializerCall(ReadOpenObject);
							FieldInfo[] fields = T.GetFields();
							for (int i = 0; i < fields.Length; i++)
							{
								FieldInfo f3 = fields[i];
								FieldInfo f = f3;
								SerializerInfo reader3 = GetReader(f.FieldType);
								SerializerInfo arg_36B_1 = reader3;
								EmitFieldDeserializer(arg_36B_1
									, LdTarget: () => { Ldloc(target); }
									, LdAddress: ()=> { Ldflda(f); }
									, StValue: () => { Stfld(f); });
							}
							SerializerCall(ReadCloseObject);
							Ldloc(target);
							Ret();
						}
						else
						{
							SerializerCall(ReadOpenObject);
							FieldInfo[] fields2 = T.GetFields();
							for (int j = 0; j < fields2.Length; j++)
							{
								FieldInfo f2 = fields2[j];
								FieldInfo f = f2;
								SerializerInfo reader4 = GetReader(f.FieldType);
								EmitFieldDeserializer(reader4, delegate
								{
									Ldarg(1);
								}, delegate
								{
									Ldflda(f);
								}, delegate
								{
									Stfld(f);
								});
							}
							SerializerCall(ReadCloseObject);
							Ret();
						}
					}
				}
			}

			private void EmitFieldDeserializer(SerializerInfo reader, Action LdTarget, Action LdAddress, Action StValue)
			{
				bool isBasic = reader.IsBasic;
				if (isBasic)
				{
					Ldarg(0);
					LdTarget();
					LdAddress();
					Callvirt(reader.Method);
				}
				else
				{
					bool isClass = reader.IsClass;
					if (isClass)
					{
						LdTarget();
						Ldarg(0);
						Call(reader.Method);
						StValue();
					}
					else
					{
						Ldarg(0);
						LdTarget();
						LdAddress();
						Call(reader.Method);
					}
				}
			}
		}

		private abstract class SerializerEmitterBase : Emitter.Emitter
		{
			protected readonly Type T;

			protected readonly bool IsNullable;

			protected readonly bool CanSerializeNull;

			private DynamicMethod method = null;

			protected DynamicMethod Method
			{
				get
				{
					bool flag = method != null;
					DynamicMethod result;
					if (flag)
					{
						result = method;
					}
					else
					{
						method = OnCreateMethod();
						Emit(method.GetILGenerator());
						result = method;
					}
					return result;
				}
			}

			protected SerializerEmitterBase(Type t)
			{
				T = t;
				IsNullable = (Nullable.GetUnderlyingType(T) != null);
				CanSerializeNull = (T.IsClass || IsNullable);
			}

			protected abstract DynamicMethod OnCreateMethod();

			protected void SerializerCall(MethodInfo m)
			{
				Ldarg(0);
				Callvirt(m);
			}
		}

		private class SerializerInfo
		{
			public readonly MethodInfo Method;

			public readonly bool IsBasic;

			public readonly bool IsClass;

			public SerializerInfo(Type t, Dictionary<Type, MethodInfo> basic, Func<Type, DynamicMethod> getDynamic)
			{
				basic.TryGetValue((t.IsEnum ? Enum.GetUnderlyingType(t) : t).MakeByRefType(), out Method);
				bool flag = !(IsBasic = (Method != null));
				if (flag)
				{
					Method = getDynamic(t);
				}
				IsClass = t.IsClass;
			}
		}

		private static readonly Dictionary<Type, Delegate> Delegates = new Dictionary<Type, Delegate>();

		private static readonly object DynamicCreateLock = new object();

		private static int DynamicMethodNumber = 1;

		public static IAxSerializer WriteClass<T>(this IAxSerializer s, T source) where T : class
		{
			Delegate @delegate;
			bool flag = !Delegates.TryGetValue(typeof(T), out @delegate);
			if (flag)
			{
				@delegate = Serializer.GetDynamicSerializer(typeof(T)).CreateDelegate(typeof(ClassSerializer<T>));
			}
			ClassSerializer<T> classSerializer = (ClassSerializer<T>)@delegate;
			classSerializer(s, source);
			return s;
		}

		public static IAxSerializer WriteStruct<T>(this IAxSerializer s, ref T source) where T : struct
		{
			Delegate @delegate;
			bool flag = !Delegates.TryGetValue(typeof(T), out @delegate);
			if (flag)
			{
				@delegate = Serializer.GetDynamicSerializer(typeof(T)).CreateDelegate(typeof(StructSerializer<T>));
			}
			StructSerializer<T> structSerializer = (StructSerializer<T>)@delegate;
			structSerializer(s, ref source);
			return s;
		}

		public static T ReadClass<T>(this IAxDeserializer s) where T : class
		{
			Delegate @delegate;
			bool flag = !Delegates.TryGetValue(typeof(T), out @delegate);
			if (flag)
			{
				@delegate = Deserializer.GetDynamicDeserializer(typeof(T)).CreateDelegate(typeof(ClassDeserializer<T>));
			}
			ClassDeserializer<T> classDeserializer = (ClassDeserializer<T>)@delegate;
			return classDeserializer(s);
		}

		public static void ReadStruct<T>(this IAxDeserializer s, out T target) where T : struct
		{
			Delegate @delegate;
			bool flag = !Delegates.TryGetValue(typeof(T), out @delegate);
			if (flag)
			{
				@delegate = Deserializer.GetDynamicDeserializer(typeof(T)).CreateDelegate(typeof(StructDeserializer<T>));
			}
			StructDeserializer<T> structDeserializer = (StructDeserializer<T>)@delegate;
			structDeserializer(s, out target);
		}

		private static DynamicMethod CreateDynamicMethod(Type returnType, string name, params Type[] paramTypes)
		{
			int num = DynamicMethodNumber++;
			return new DynamicMethod(string.Concat(new object[]
			{
				"IAxSerializer_",
				num,
				"_",
				name
			}), returnType, paramTypes, true);
		}

		private static Dictionary<Type, MethodInfo> BasicMethods<T>(string method_name, string arg0_name)
		{
			IEnumerable<MethodInfo> arg_56_0 = typeof(T).GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy).Where(delegate(MethodInfo mi)
			{
				bool flag = mi.Name != method_name;
				bool result;
				if (flag)
				{
					result = false;
				}
				else
				{
					ParameterInfo[] parameters = mi.GetParameters();
					bool flag2 = parameters.Length != 1;
					if (flag2)
					{
						result = false;
					}
					else
					{
						ParameterInfo parameterInfo = parameters[0];
						result = (parameterInfo.Name == arg0_name && parameterInfo.ParameterType.IsByRef);
					}
				}
				return result;
			});
			Func<MethodInfo, Type> arg_56_1 = new Func<MethodInfo, Type>(mi => mi.GetParameters()[0].ParameterType);
			return arg_56_0.ToDictionary(arg_56_1);
		}

		private static FieldInfo GetFieldPrivate(this Type t, string name)
		{
			return t.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
		}
	}
}
