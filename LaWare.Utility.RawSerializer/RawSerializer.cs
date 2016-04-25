using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using LaWare.Utility.AxSerializer;

// ReSharper disable InconsistentNaming

namespace LaWare.Utility.RawSerializer
{
	[Flags]
	public enum RawHeader : byte
	{
		False = 0,
		True = 1,
		Null = 2,
		OpenObject = 3,
		CloseObject = 4,
		OpenArray = 5,
		CloseArray = 6,
		Byte = 7,
		Char = 8,
		Int16 = 9,
		Int32 = 10,
		Int64 = 11,
		Single = 12,
		Double = 13,
		Decimal = 14,
		Time = 15,
		DateTime = 16,
		Guid = 17,
		String = 18,
		ByteArray = 19,
		UInt16 = 20,
		UInt32 = 21,
		UInt64 = 22,
		TypeMask = 0x1f,
		SizeMask = 0xe0,
		SizeFull = 0,
		Size1 = 0x20,
		Size2 = 0x40,
		Size3 = 0x60,
		Size4 = 0x80,
		Size5 = 0xa0,
		Size6 = 0xc0,
		Size7 = 0xe0
	}
	public class RawSerializer : IAxSerializer
	{
		private const int Int24_MaxValue = 8388352;
		private const int Int24_MinValue = -8388608;
		private const long Int40_MaxValue = 549755813632L;
		private const long Int40_MinValue = -549755813888L;
		private const long Int48_MaxValue = 140737488289792L;
		private const long Int48_MinValue = -140737488355328L;
		private const long Int56_MaxValue = 36028797002186752L;
		private const long Int56_MinValue = -36028797018963968L;
		private const uint UInt24_MaxValue = 16776960u;
		private const ulong UInt40_MaxValue = 1099511627520uL;
		private const ulong UInt48_MaxValue = 281474976645120uL;
		private const ulong UInt56_MaxValue = 72057594021150720uL;

		private readonly List<byte> data = new List<byte>();

		public byte[] GetBytes()
		{
			return data.ToArray();
		}

		private void Type(RawHeader type)
		{
			data.Add((byte)type);
		}

		public void WriteNull()
		{
			Type(RawHeader.Null);
		}

		public void WriteOpenObject()
		{
			Type(RawHeader.OpenObject);
		}

		public void WriteCloseObject()
		{
			Type(RawHeader.CloseObject);
		}

		public void WriteOpenArray()
		{
			Type(RawHeader.OpenArray);
		}

		public void WriteCloseArray()
		{
			Type(RawHeader.CloseArray);
		}

		private unsafe void WriteRawBytes(void* p, int count)
		{
			for (var i = 0; i < count; i++)
			{
				data.Add(((byte*)p)[i]);
			}
		}

		private unsafe void WriteShortI16(short source, RawHeader type)
		{
			if (source <= 127 && source >= -128)
			{
				Type(type | RawHeader.Size1);
				WriteRawBytes(&source, 1);
			}
			else
			{
				Type(type);
				WriteRawBytes(&source, 2);
			}
		}

		private unsafe void WriteShortU16(ushort source, RawHeader type)
		{
			if (source <= 255)
			{
				Type(type | RawHeader.Size1);
				WriteRawBytes(&source, 1);
			}
			else
			{
				Type(type);
				WriteRawBytes(&source, 2);
			}
		}

		private unsafe void WriteShortI32(int source, RawHeader type)
		{
			if (source <= 127 && source >= -128)
			{
				Type(type | RawHeader.Size1);
				WriteRawBytes(&source, 1);
			}
			else if (source <= 32767 && source >= -32768)
			{
				Type(type | RawHeader.Size2);
				WriteRawBytes(&source, 2);
			}
			else if (source <= 8388352 && source >= -8388608)
			{
				Type(type | RawHeader.Size3);
				WriteRawBytes(&source, 3);
			}
			else
			{
				Type(type);
				WriteRawBytes(&source, 4);
			}
		}

		private unsafe void WriteShortU32(uint source, RawHeader type)
		{
			if (source <= 255u)
			{
				Type(type | RawHeader.Size1);
				WriteRawBytes(&source, 1);
			}
			else if (source <= 65535u)
			{
				Type(type | RawHeader.Size2);
				WriteRawBytes(&source, 2);
			}
			else if (source <= 16776960u)
			{
				Type(type | RawHeader.Size3);
				WriteRawBytes(&source, 3);
			}
			else
			{
				Type(type);
				WriteRawBytes(&source, 4);
			}
		}

		private unsafe void WriteShortI64(long source, RawHeader type)
		{
			if (source <= 127L && source >= -128L)
			{
				Type(type | RawHeader.Size1);
				WriteRawBytes(&source, 1);
			}
			else if (source <= 32767L && source >= -32768L)
			{
				Type(type | RawHeader.Size2);
				WriteRawBytes(&source, 2);
			}
			else if (source <= 8388352L && source >= -8388608L)
			{
				Type(type | RawHeader.Size3);
				WriteRawBytes(&source, 3);
			}
			else if (source <= 2147483647L && source >= -2147483648L)
			{
				Type(type | RawHeader.Size4);
				WriteRawBytes(&source, 4);
			}
			else if (source <= 549755813632L && source >= -549755813888L)
			{
				Type(type | RawHeader.Size5);
				WriteRawBytes(&source, 5);
			}
			else if (source <= 140737488289792L && source >= -140737488355328L)
			{
				Type(type | RawHeader.Size6);
				WriteRawBytes(&source, 6);
			}
			else if (source <= 36028797002186752L && source >= -36028797018963968L)
			{
				Type(type | RawHeader.SizeMask);
				WriteRawBytes(&source, 7);
			}
			else
			{
				Type(type);
				WriteRawBytes(&source, 8);
			}
		}

		private unsafe void WriteShortU64(ulong source, RawHeader type)
		{
			if (source <= 255uL)
			{
				Type(type | RawHeader.Size1);
				WriteRawBytes(&source, 1);
			}
			else if (source <= 65535uL)
			{
				Type(type | RawHeader.Size2);
				WriteRawBytes(&source, 2);
			}
			else if (source <= 16776960uL)
			{
				Type(type | RawHeader.Size3);
				WriteRawBytes(&source, 3);
			}
			else if (source <= 4294901760uL)
			{
				Type(type | RawHeader.Size4);
				WriteRawBytes(&source, 4);
			}
			else if (source <= 1099511627520uL)
			{
				Type(type | RawHeader.Size5);
				WriteRawBytes(&source, 5);
			}
			else if (source <= 281474976645120uL)
			{
				Type(type | RawHeader.Size6);
				WriteRawBytes(&source, 6);
			}
			else if (source <= 72057594021150720uL)
			{
				Type(type | RawHeader.SizeMask);
				WriteRawBytes(&source, 7);
			}
			else
			{
				Type(type);
				WriteRawBytes(&source, 8);
			}
		}

		public void Write(ref bool source)
		{
			Type(source ? RawHeader.True : RawHeader.False);
		}

		public void Write(ref byte source)
		{
			Type(RawHeader.Byte);
			data.Add(source);
		}

		public void Write(ref char source)
		{
			WriteShortU16(source, RawHeader.Char);
		}

		public void Write(ref short source)
		{
			WriteShortI16(source, RawHeader.Int16);
		}

		public void Write(ref ushort source)
		{
			WriteShortU16(source, RawHeader.UInt16);
		}

		public void Write(ref int source)
		{
			WriteShortI32(source, RawHeader.Int32);
		}

		public void Write(ref uint source)
		{
			WriteShortU32(source, RawHeader.UInt32);
		}

		public void Write(ref long source)
		{
			WriteShortI64(source, RawHeader.Int64);
		}

		public void Write(ref ulong source)
		{
			WriteShortU64(source, RawHeader.UInt64);
		}

		public unsafe void Write(ref float source)
		{
			Type(RawHeader.Single);
			fixed (float* ptr = &source)
			{
				WriteRawBytes(ptr, 4);
			}
		}

		public unsafe void Write(ref double source)
		{
			Type(RawHeader.Double);
			fixed (double* ptr = &source)
			{
				WriteRawBytes(ptr, 8);
			}
		}

		public void Write(ref DateTime source)
		{
			WriteShortI64((long)(source - RawBuffer.UnixEpoc).TotalMilliseconds, RawHeader.DateTime);
		}

		public void Write(ref TimeSpan source)
		{
			WriteShortI32((int)source.TotalMilliseconds, RawHeader.Time);
		}

		public unsafe void Write(ref decimal source)
		{
			Type(RawHeader.Decimal);
			fixed (decimal* ptr = &source)
			{
				WriteRawBytes(ptr, 16);
			}
		}

		public unsafe void Write(ref Guid source)
		{
			Type(RawHeader.Guid);
			fixed (Guid* ptr = &source)
			{
				WriteRawBytes(ptr, 16);
			}
		}

		public void Write(ref string source)
		{
			if (source == null)
			{
				Type(RawHeader.Null);
			}
			else
			{
				var bytes = Encoding.UTF8.GetBytes(source);
				WriteShortI16((short)bytes.Length, RawHeader.String);
				data.AddRange(bytes);
			}
		}

		public void Write(ref byte[] source)
		{
			if (source == null)
			{
				Type(RawHeader.Null);
			}
			else
			{
				WriteShortI32(source.Length, RawHeader.ByteArray);
				data.AddRange(source);
			}
		}

		public void WriteBytes(byte[] source)
		{
			data.AddRange(source);
		}
	}
	public class RawDeserializer : IAxDeserializer
	{
		private readonly RawBuffer buffer;

		private int fields_read;

		public RawDeserializer(byte[] data)
		{
			buffer = new RawBuffer(data);
		}

		public RawDeserializer(RawBuffer data)
		{
			buffer = new RawBuffer(data);
		}

		private SerializationException Fail(RawHeader found, params RawHeader[] expected)
		{
			return
				new SerializationException(
					$"Incorrect field type header. expected ({string.Join("|", expected)}) but found ({found}) at position {buffer.Pos}, field {fields_read}");
		}

		private void Success()
		{
			fields_read++;
		}

		private RawHeader Expect(RawHeader expected)
		{
			var found = buffer.NextType();
			var rawHeader = buffer.TrySimple(found, expected);
			if (rawHeader.HasValue) return rawHeader.Value;
			throw Fail(found, expected);
		}

		private RawHeader? ExpectNull(RawHeader expected)
		{
			var found = buffer.NextType();
			var rawHeader = buffer.TrySimple(found, expected);
			if (rawHeader.HasValue) return rawHeader;
			if (buffer.TrySimple(found, RawHeader.Null).HasValue) return null;
			throw Fail(found, expected, RawHeader.Null);
		}

		public bool ReadNull()
		{
			if (buffer.TrySimple(buffer.NextType(), RawHeader.Null).HasValue) return false;
			Success();
			return true;
		}

		public void ReadOpenObject()
		{
			Expect(RawHeader.OpenObject);
			Success();
		}

		public void ReadCloseObject()
		{
			Expect(RawHeader.CloseObject);
			Success();
		}

		public void ReadOpenArray()
		{
			Expect(RawHeader.OpenArray);
			Success();
		}

		public void ReadCloseArray()
		{
			Expect(RawHeader.CloseArray);
			Success();
		}

		public void Read(out bool target)
		{
			var found = buffer.NextType();
			if (buffer.TrySimple(found, RawHeader.False).HasValue) target = false;
			else if (buffer.TrySimple(found, RawHeader.True).HasValue) target = true;
			else
				throw Fail(found, RawHeader.False, RawHeader.True);
			Success();
		}

		public void Read(out byte target)
		{
			Expect(RawHeader.Byte);
			target = buffer.Byte();
			Success();
		}

		public void Read(out char target)
		{
			var size = Expect(RawHeader.Char);
			target = buffer.Char(size);
			Success();
		}

		public void Read(out short target)
		{
			var size = Expect(RawHeader.Int16);
			target = buffer.Int16(size);
			Success();
		}

		public void Read(out ushort target)
		{
			var size = Expect(RawHeader.UInt16);
			target = buffer.UInt16(size);
			Success();
		}

		public void Read(out int target)
		{
			var size = Expect(RawHeader.Int32);
			target = buffer.Int32(size);
			Success();
		}

		public void Read(out uint target)
		{
			var size = Expect(RawHeader.UInt32);
			target = buffer.UInt32(size);
			Success();
		}

		public void Read(out long target)
		{
			var size = Expect(RawHeader.Int64);
			target = buffer.Int64(size);
			Success();
		}

		public void Read(out ulong target)
		{
			var size = Expect(RawHeader.UInt64);
			target = buffer.UInt64(size);
			Success();
		}

		public void Read(out float target)
		{
			Expect(RawHeader.Single);
			target = buffer.Single();
			Success();
		}

		public void Read(out double target)
		{
			Expect(RawHeader.Double);
			target = buffer.Double();
			Success();
		}

		public void Read(out decimal target)
		{
			Expect(RawHeader.Decimal);
			target = buffer.Decimal();
			Success();
		}

		public void Read(out DateTime target)
		{
			Expect(RawHeader.DateTime);
			target = RawBuffer.UnixEpoc.AddMilliseconds(buffer.Int64(RawHeader.SizeFull));
			Success();
		}

		public void Read(out TimeSpan target)
		{
			var size = Expect(RawHeader.Time);
			target = new TimeSpan(0, 0, 0, 0, buffer.Int32(size));
			Success();
		}

		public void Read(out Guid target)
		{
			Expect(RawHeader.Guid);
			target = buffer.Guid();
			Success();
		}

		public void Read(out string target)
		{
			var rawHeader = ExpectNull(RawHeader.String);
			target = (rawHeader.HasValue ? buffer.String(rawHeader.Value) : null);
			Success();
		}

		public void Read(out byte[] target)
		{
			var rawHeader = ExpectNull(RawHeader.ByteArray);
			target = (rawHeader.HasValue ? buffer.Bytes(rawHeader.Value) : null);
			Success();
		}

		public void Read(out byte[] target, int count)
		{
			target = buffer.Bytes(count);
			Success();
		}
	}
	public class RawBuffer
	{
		private readonly byte[] Data;

		internal static DateTime UnixEpoc = new DateTime(1970, 1, 1);

		public int Pos { get; private set; }

		public RawBuffer(byte[] data)
		{
			Data = data;
		}

		public RawBuffer(byte[] data, int offset, int count)
		{
			Data = new byte[count];
			Array.Copy(data, offset, Data, 0, count);
		}

		public RawBuffer(RawBuffer data)
		{
			Data = data.Data;
		}

		public RawHeader NextType()
		{
			return (RawHeader)Data[Pos];
		}

		public bool EndOfBuffer()
		{
			return Pos >= Data.Length;
		}

		public void MoveNext()
		{
			Pos++;
		}

		public RawHeader? TrySimple(RawHeader found, RawHeader expected)
		{
			if ((found & RawHeader.TypeMask) == expected)
			{
				MoveNext();
				return found & RawHeader.SizeMask;
			}
			return null;
		}

		public byte Byte()
		{
			var result = Data[Pos];
			Pos++;
			return result;
		}

		public unsafe char Char(RawHeader size)
		{
			char result;
			fixed (byte* buf = Data)
			{
				if (size == RawHeader.Size1)
				{
					result = (char)*(buf + Pos);
					Pos++;
				}
				else
				{
					result = (char)*(ushort*)(buf + Pos);
					Pos += 2;
				}
			}
			return result;
		}

		public unsafe ushort UInt16(RawHeader size)
		{
			ushort result;
			fixed (byte* buf = Data)
			{
				if (size == RawHeader.Size1)
				{
					result = (ushort)*(buf + Pos);
					Pos++;
				}
				else
				{
					result = (*(ushort*)(buf + Pos));
					Pos += 2;
				}
			}
			return result;
		}

		public unsafe uint UInt32(RawHeader size)
		{
			uint result;
			fixed (byte* buf = Data)
			{
				if (size == RawHeader.Size1)
				{
					result = (uint) (*(buf + Pos));
					Pos++;
				}
				else if (size == RawHeader.Size2)
				{
					result = (uint) (*(ushort*) (buf + Pos));
					Pos += 2;
				}
				else if (size == RawHeader.Size3)
				{
					result = (uint) (*(ushort*) (buf + Pos) | (buf + Pos)[2] << 16);
					Pos += 3;
				}
				else
				{
					result = (*(uint*) (buf + Pos));
					Pos += 4;
				}
			}
			return result;
		}

		public unsafe ulong UInt64(RawHeader size)
		{
			ulong result;
			fixed (byte* buf = Data)
			{
				if (size == RawHeader.Size1)
				{
					result = (ulong) (*(buf + Pos));
					Pos++;
				}
				else if (size == RawHeader.Size2)
				{
					result = (ulong) (*(ushort*) (buf + Pos));
					Pos += 2;
				}
				else if (size == RawHeader.Size3)
				{
					result = *(ushort*) (buf + Pos) | (ulong) (buf + Pos)[2] << 16;
					Pos += 3;
				}
				else if (size == RawHeader.Size4)
				{
					result = (ulong) (*(uint*) (buf + Pos));
					Pos += 4;
				}
				else if (size == RawHeader.Size5)
				{
					result = *(uint*) (buf + Pos) | (ulong) (buf + Pos)[4] << 32;
					Pos += 5;
				}
				else if (size == RawHeader.Size6)
				{
					result = *(uint*) (buf + Pos) |
					         (ulong) ((ushort*) (buf + Pos))[2] << 32;
					Pos += 6;
				}
				else if (size == RawHeader.SizeMask)
				{
					result = *(uint*) (buf + Pos) |
					         (ulong) ((ushort*) (buf + Pos))[2] << 32 |
					         (ulong) (buf + Pos)[6] << 48;
					Pos += 7;
				}
				else
				{
					result = (ulong) (*(long*) (buf + Pos));
					Pos += 8;
				}
			}
			return result;
		}

		public unsafe short Int16(RawHeader size)
		{
			short result;
			fixed (byte* buf = Data)
			{
				if (size == RawHeader.Size1)
				{
					result = (short)(*(sbyte*)(buf + Pos));
					Pos++;
				}
				else
				{
					result = (*(short*)(buf + Pos));
					Pos += 2;
				}
			}
			return result;
		}

		public unsafe int Int32(RawHeader size)
		{
			int result;
			fixed (byte* buf = Data)
			{
				if (size == RawHeader.Size1)
				{
					result = (int)(*(sbyte*)(buf + Pos));
					Pos++;
				}
				else if (size == RawHeader.Size2)
				{
					result = (int) (*(short*) (buf + Pos));
					Pos += 2;
				}
				else if (size == RawHeader.Size3)
				{
					result = *(ushort*) (buf + Pos) | ((sbyte*) (buf + Pos))[2] << 16;
					Pos += 3;
				}
				else
				{
					result = (*(int*) (buf + Pos));
					Pos += 4;
				}
			}
			return result;
		}

		public unsafe long Int64(RawHeader size)
		{
			long result;
			fixed (byte* buf = Data)
			{
				if (size == RawHeader.Size1)
				{
					result = (long)(*(sbyte*)(buf + Pos));
					Pos++;
				}
				else if (size == RawHeader.Size2)
				{
					var num2 = (long) (*(short*) (buf + Pos));
					result = num2;
					Pos += 2;
				}
				else if (size == RawHeader.Size3)
				{
					result = (long)
							(*(ushort*)(buf + Pos) |
							 (ulong)((sbyte*)(buf + Pos))[2] << 16);
					Pos += 3;
				}
				else if (size == RawHeader.Size4)
				{
					result = (long)(*(int*)(buf + Pos));
					Pos += 4;
				}
				else if (size == RawHeader.Size5)
				{
					result = (long)
							(*(uint*)(buf + Pos) |
							 (ulong)((sbyte*)(buf + Pos))[4] << 32);
					Pos += 5;
				}
				else if (size == RawHeader.Size6)
				{
					result = (long)
							(*(uint*)(buf + Pos) |
							 (ulong)((short*)(buf + Pos))[2] << 32);
					Pos += 6;
				}
				else if (size == RawHeader.Size7)
				{
					result = (long)
							(*(uint*)(buf + Pos) | (ulong)((ushort*)(buf + Pos))[2] << 32 |
							 (ulong)((sbyte*)(buf + Pos))[6] << 48);
					Pos += 7;
				}
				else
				{
					result = (*(long*)(buf + Pos));
					Pos += 8;
				}
			}
			return result;
		}

		public unsafe float Single()
		{
			float result;
			fixed (byte* buf = Data)
				result = *(float*) (buf + Pos);
			Pos += 4;
			return result;
		}

		public unsafe double Double()
		{
			double result;
			fixed (byte* buf = Data)
				result = *(double*) (buf + Pos);
			Pos += 8;
			return result;
		}

		public unsafe decimal Decimal()
		{
			decimal result;
			fixed (byte* buf = Data)
				result = *(decimal*) buf;
			Pos += 16;
			return result;
		}

		public DateTime DateTime()
		{
			return UnixEpoc.AddMilliseconds(Int64(RawHeader.SizeFull));
		}

		public TimeSpan Time(RawHeader size)
		{
			return new TimeSpan(0, 0, 0, 0, Int32(size));
		}

		public unsafe Guid Guid()
		{
			Guid result;
			fixed (byte* buf = Data)
				result = *(Guid*)buf;
			Pos += 16;
			return result;
		}

		public byte[] Bytes(int count)
		{
			var array = new byte[count];
			Array.Copy(Data, Pos, array, 0, count);
			Pos += count;
			return array;
		}

		public string String(RawHeader size)
		{
			return Encoding.UTF8.GetString(Bytes(Int16(size)));
		}

		public byte[] Bytes(RawHeader size)
		{
			return Bytes(Int32(size));
		}
	}
	public class RawBufferInspector
	{

		private readonly RawBuffer buffer;

		public List<string> Items
		{
			get;
			private set;
		}

		public RawBufferInspector(byte[] data, int offset, int count)
		{
			buffer = new RawBuffer(data, offset, count);
			Items = new List<string>();
			print();
		}

		public RawBufferInspector(byte[] data)
		{
			buffer = new RawBuffer(data);
			Items = new List<string>();
			print();
		}

		public RawBufferInspector(RawBuffer data)
		{
			buffer = new RawBuffer(data);
			Items = new List<string>();
			print();
		}

		public override string ToString()
		{
			return string.Join(Environment.NewLine, Items);
		}

		private void print()
		{
			while (print(0) && !buffer.EndOfBuffer())
			{
			}
		}

		private bool print(int depth)
		{
			var FieldSize = RawHeader.SizeFull;
			var FieldSizeStr = "";
			Func<RawHeader, bool> func = delegate(RawHeader expected)
			{
				var rawHeader = buffer.TrySimple(buffer.NextType(), expected);
				var hasValue = rawHeader.HasValue;
				if (hasValue)
				{
					FieldSize = rawHeader.Value;
					FieldSizeStr = ((FieldSize == RawHeader.Size1) ? ":1" : ((FieldSize == RawHeader.Size2) ? ":2" : ((FieldSize == RawHeader.Size3) ? ":3" : ((FieldSize == RawHeader.Size4) ? ":4" : ((FieldSize == RawHeader.Size5) ? ":5" : ((FieldSize == RawHeader.Size6) ? ":6" : ((FieldSize == RawHeader.SizeMask) ? ":7" : "  ")))))));
				}
				return rawHeader.HasValue;
			};
			var flag = buffer.EndOfBuffer();
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				var flag2 = func(RawHeader.CloseObject);
				if (flag2)
				{
					print(depth, "}");
					print(depth, "ERROR: UNEXPECTED CLOSE OF OBJECT");
					result = false;
				}
				else
				{
					var flag3 = func(RawHeader.CloseArray);
					if (flag3)
					{
						print(depth, "]");
						print(depth, "ERROR: UNEXPECTED CLOSE OF ARRAY");
						result = false;
					}
					else
					{
						var flag4 = func(RawHeader.OpenObject);
						if (flag4)
						{
							print(depth, "OBJECT");
							print(depth, "{");
							while (true)
							{
								var flag5 = buffer.EndOfBuffer();
								if (flag5)
								{
									break;
								}
								var flag6 = func(RawHeader.CloseObject);
								if (flag6)
								{
									goto Block_6;
								}
								var flag7 = !print(depth + 1);
								if (flag7)
								{
									goto Block_7;
								}
							}
							print(depth + 1, "ERROR: UNEXPECTED END OF BUFFER");
							result = false;
							return result;
							Block_6:
							print(depth, "}");
							result = true;
							return result;
							Block_7:
							result = false;
						}
						else
						{
							var flag8 = func(RawHeader.OpenArray);
							if (flag8)
							{
								var flag9 = buffer.EndOfBuffer();
								if (flag9)
								{
									print(depth, "ARRAY[???]");
									print(depth, "[");
									print(depth + 1, "ERROR: UNEXPECTED END OF BUFFER");
									result = false;
								}
								else
								{
									var flag10 = func(RawHeader.Int32);
									if (flag10)
									{
										print(depth, string.Concat("ARRAY[", buffer.Int32(FieldSize), FieldSizeStr, "]"));
										print(depth, "[");
										while (true)
										{
											var flag11 = buffer.EndOfBuffer();
											if (flag11)
											{
												break;
											}
											var flag12 = func(RawHeader.CloseArray);
											if (flag12)
											{
												goto Block_12;
											}
											var flag13 = !print(depth + 1);
											if (flag13)
											{
												goto Block_13;
											}
										}
										print(depth + 1, "ERROR: UNEXPECTED END OF BUFFER");
										result = false;
										return result;
										Block_12:
										print(depth, "]");
										result = true;
										return result;
										Block_13:
										result = false;
									}
									else
									{
										print(depth, "ARRAY[???]");
										print(depth, "[");
										print(depth + 1, "ERROR: MISSING INT32 FOR ARRAY LENGHT");
										result = false;
									}
								}
							}
							else
							{
								var flag14 = func(RawHeader.Null);
								if (flag14)
								{
									print(depth, "NULL     NULL");
								}
								else
								{
									var flag15 = func(RawHeader.True);
									if (flag15)
									{
										print(depth, "BOOL     TRUE");
									}
									else
									{
										var flag16 = func(RawHeader.False);
										if (flag16)
										{
											print(depth, "BOOL     FALSE");
										}
										else
										{
											var flag17 = func(RawHeader.Byte);
											if (flag17)
											{
												print(depth, "BYTE     " + buffer.Byte());
											}
											else
											{
												var flag18 = func(RawHeader.Char);
												if (flag18)
												{
													print(depth, string.Concat("CHAR", FieldSizeStr, "   '", buffer.Char(FieldSize).ToString(), "'"));
												}
												else
												{
													var flag19 = func(RawHeader.Int16);
													if (flag19)
													{
														print(depth, string.Concat("INT16", FieldSizeStr, "  ", buffer.Int16(FieldSize)));
													}
													else
													{
														var flag20 = func(RawHeader.UInt16);
														if (flag20)
														{
															print(depth, string.Concat("UINT16", FieldSizeStr, " ", buffer.UInt16(FieldSize)));
														}
														else
														{
															var flag21 = func(RawHeader.Int32);
															if (flag21)
															{
																print(depth, string.Concat("INT32", FieldSizeStr, "  ", buffer.Int32(FieldSize)));
															}
															else
															{
																var flag22 = func(RawHeader.UInt32);
																if (flag22)
																{
																	print(depth, string.Concat("UINT32", FieldSizeStr, " ", buffer.UInt32(FieldSize)));
																}
																else
																{
																	var flag23 = func(RawHeader.Int64);
																	if (flag23)
																	{
																		print(depth, string.Concat("INT64", FieldSizeStr, "  ", buffer.Int64(FieldSize)));
																	}
																	else
																	{
																		var flag24 = func(RawHeader.UInt64);
																		if (flag24)
																		{
																			print(depth, string.Concat("UINT64", FieldSizeStr, " ", buffer.UInt64(FieldSize)));
																		}
																		else
																		{
																			var flag25 = func(RawHeader.Single);
																			if (flag25)
																			{
																				print(depth, "SINGLE   " + buffer.Single());
																			}
																			else
																			{
																				var flag26 = func(RawHeader.Double);
																				if (flag26)
																				{
																					print(depth, "DOUBLE   " + buffer.Double());
																				}
																				else
																				{
																					var flag27 = func(RawHeader.Decimal);
																					if (flag27)
																					{
																						print(depth, "DECIMAL  " + buffer.Decimal());
																					}
																					else
																					{
																						var flag28 = func(RawHeader.DateTime);
																						if (flag28)
																						{
																							print(depth, "DATETIME " + buffer.DateTime());
																						}
																						else
																						{
																							var flag29 = func(RawHeader.Time);
																							if (flag29)
																							{
																								print(depth, string.Concat("TIME", FieldSizeStr, "   ", buffer.Time(FieldSize)));
																							}
																							else
																							{
																								var flag30 = func(RawHeader.Guid);
																								if (flag30)
																								{
																									print(depth, "GUID     " + buffer.Guid());
																								}
																								else
																								{
																									var flag31 = func(RawHeader.String);
																									if (flag31)
																									{
																										print(depth, string.Concat("STRING", FieldSizeStr, " \"", buffer.String(FieldSize), "\""));
																									}
																									else
																									{
																										var flag32 = func(RawHeader.ByteArray);
																										if (!flag32)
																										{
																											print(depth, "ERROR: UNEXPECTED TYPE HEADER " + buffer.NextType());
																											result = false;
																											return result;
																										}
																										var array = buffer.Bytes(FieldSize);
																										var expr_782 = new object[5];
																										expr_782[0] = "BYTE[";
																										expr_782[1] = array.Length;
																										expr_782[2] = FieldSizeStr;
																										expr_782[3] = "] ";
																										var arg_7D9_1 = 4;
																										var arg_7D4_0 = " ";
																										expr_782[arg_7D9_1] = string.Join(arg_7D4_0, array.Select(b => b.ToString("X02")));
																										print(depth, string.Concat(expr_782));
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
													}
												}
											}
										}
									}
								}
								result = true;
							}
						}
					}
				}
			}
			return result;
		}

		private void print(int depth, string str)
		{
			Items.Add(new string(' ', 4 * depth) + str);
		}
	}
}
