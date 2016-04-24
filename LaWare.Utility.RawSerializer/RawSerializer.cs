using LaWare.Utility.AxSerializer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Versioning;
using System.Security;
using System.Security.Permissions;
using System.Text;

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
		TypeMask = 31,
		SizeMask = 224,
		SizeFull = 0,
		Size1 = 32,
		Size2 = 64,
		Size3 = 96,
		Size4 = 128,
		Size5 = 160,
		Size6 = 192,
		Size7 = 224
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
			return this.data.ToArray();
		}

		private void Type(RawHeader type)
		{
			this.data.Add((byte)type);
		}

		public void WriteNull()
		{
			this.Type(RawHeader.Null);
		}

		public void WriteOpenObject()
		{
			this.Type(RawHeader.OpenObject);
		}

		public void WriteCloseObject()
		{
			this.Type(RawHeader.CloseObject);
		}

		public void WriteOpenArray()
		{
			this.Type(RawHeader.OpenArray);
		}

		public void WriteCloseArray()
		{
			this.Type(RawHeader.CloseArray);
		}

		private unsafe void WriteRawBytes(void* p, int count)
		{
			for (int i = 0; i < count; i++)
			{
				this.data.Add(((byte*)p)[i]);
			}
		}

		private unsafe void WriteShortI16(short source, RawHeader type)
		{
			bool flag = source <= 127 && source >= -128;
			if (flag)
			{
				this.Type(type | RawHeader.Size1);
				this.WriteRawBytes((void*)(&source), 1);
			}
			else
			{
				this.Type(type);
				this.WriteRawBytes((void*)(&source), 2);
			}
		}

		private unsafe void WriteShortU16(ushort source, RawHeader type)
		{
			bool flag = source <= 255;
			if (flag)
			{
				this.Type(type | RawHeader.Size1);
				this.WriteRawBytes((void*)(&source), 1);
			}
			else
			{
				this.Type(type);
				this.WriteRawBytes((void*)(&source), 2);
			}
		}

		private unsafe void WriteShortI32(int source, RawHeader type)
		{
			bool flag = source <= 127 && source >= -128;
			if (flag)
			{
				this.Type(type | RawHeader.Size1);
				this.WriteRawBytes((void*)(&source), 1);
			}
			else
			{
				bool flag2 = source <= 32767 && source >= -32768;
				if (flag2)
				{
					this.Type(type | RawHeader.Size2);
					this.WriteRawBytes((void*)(&source), 2);
				}
				else
				{
					bool flag3 = source <= 8388352 && source >= -8388608;
					if (flag3)
					{
						this.Type(type | RawHeader.Size3);
						this.WriteRawBytes((void*)(&source), 3);
					}
					else
					{
						this.Type(type);
						this.WriteRawBytes((void*)(&source), 4);
					}
				}
			}
		}

		private unsafe void WriteShortU32(uint source, RawHeader type)
		{
			bool flag = source <= 255u;
			if (flag)
			{
				this.Type(type | RawHeader.Size1);
				this.WriteRawBytes((void*)(&source), 1);
			}
			else
			{
				bool flag2 = source <= 65535u;
				if (flag2)
				{
					this.Type(type | RawHeader.Size2);
					this.WriteRawBytes((void*)(&source), 2);
				}
				else
				{
					bool flag3 = source <= 16776960u;
					if (flag3)
					{
						this.Type(type | RawHeader.Size3);
						this.WriteRawBytes((void*)(&source), 3);
					}
					else
					{
						this.Type(type);
						this.WriteRawBytes((void*)(&source), 4);
					}
				}
			}
		}

		private unsafe void WriteShortI64(long source, RawHeader type)
		{
			bool flag = source <= 127L && source >= -128L;
			if (flag)
			{
				this.Type(type | RawHeader.Size1);
				this.WriteRawBytes((void*)(&source), 1);
			}
			else
			{
				bool flag2 = source <= 32767L && source >= -32768L;
				if (flag2)
				{
					this.Type(type | RawHeader.Size2);
					this.WriteRawBytes((void*)(&source), 2);
				}
				else
				{
					bool flag3 = source <= 8388352L && source >= -8388608L;
					if (flag3)
					{
						this.Type(type | RawHeader.Size3);
						this.WriteRawBytes((void*)(&source), 3);
					}
					else
					{
						bool flag4 = source <= 2147483647L && source >= -2147483648L;
						if (flag4)
						{
							this.Type(type | RawHeader.Size4);
							this.WriteRawBytes((void*)(&source), 4);
						}
						else
						{
							bool flag5 = source <= 549755813632L && source >= -549755813888L;
							if (flag5)
							{
								this.Type(type | RawHeader.Size5);
								this.WriteRawBytes((void*)(&source), 5);
							}
							else
							{
								bool flag6 = source <= 140737488289792L && source >= -140737488355328L;
								if (flag6)
								{
									this.Type(type | RawHeader.Size6);
									this.WriteRawBytes((void*)(&source), 6);
								}
								else
								{
									bool flag7 = source <= 36028797002186752L && source >= -36028797018963968L;
									if (flag7)
									{
										this.Type(type | RawHeader.SizeMask);
										this.WriteRawBytes((void*)(&source), 7);
									}
									else
									{
										this.Type(type);
										this.WriteRawBytes((void*)(&source), 8);
									}
								}
							}
						}
					}
				}
			}
		}

		private unsafe void WriteShortU64(ulong source, RawHeader type)
		{
			bool flag = source <= 255uL;
			if (flag)
			{
				this.Type(type | RawHeader.Size1);
				this.WriteRawBytes((void*)(&source), 1);
			}
			else
			{
				bool flag2 = source <= 65535uL;
				if (flag2)
				{
					this.Type(type | RawHeader.Size2);
					this.WriteRawBytes((void*)(&source), 2);
				}
				else
				{
					bool flag3 = source <= 16776960uL;
					if (flag3)
					{
						this.Type(type | RawHeader.Size3);
						this.WriteRawBytes((void*)(&source), 3);
					}
					else
					{
						bool flag4 = source <= 4294901760uL;
						if (flag4)
						{
							this.Type(type | RawHeader.Size4);
							this.WriteRawBytes((void*)(&source), 4);
						}
						else
						{
							bool flag5 = source <= 1099511627520uL;
							if (flag5)
							{
								this.Type(type | RawHeader.Size5);
								this.WriteRawBytes((void*)(&source), 5);
							}
							else
							{
								bool flag6 = source <= 281474976645120uL;
								if (flag6)
								{
									this.Type(type | RawHeader.Size6);
									this.WriteRawBytes((void*)(&source), 6);
								}
								else
								{
									bool flag7 = source <= 72057594021150720uL;
									if (flag7)
									{
										this.Type(type | RawHeader.SizeMask);
										this.WriteRawBytes((void*)(&source), 7);
									}
									else
									{
										this.Type(type);
										this.WriteRawBytes((void*)(&source), 8);
									}
								}
							}
						}
					}
				}
			}
		}

		public void Write(ref bool source)
		{
			this.Type(source ? RawHeader.True : RawHeader.False);
		}

		public void Write(ref byte source)
		{
			this.Type(RawHeader.Byte);
			this.data.Add(source);
		}

		public void Write(ref char source)
		{
			this.WriteShortU16((ushort)source, RawHeader.Char);
		}

		public void Write(ref short source)
		{
			this.WriteShortI16(source, RawHeader.Int16);
		}

		public void Write(ref ushort source)
		{
			this.WriteShortU16(source, RawHeader.UInt16);
		}

		public void Write(ref int source)
		{
			this.WriteShortI32(source, RawHeader.Int32);
		}

		public void Write(ref uint source)
		{
			this.WriteShortU32(source, RawHeader.UInt32);
		}

		public void Write(ref long source)
		{
			this.WriteShortI64(source, RawHeader.Int64);
		}

		public void Write(ref ulong source)
		{
			this.WriteShortU64(source, RawHeader.UInt64);
		}

		public unsafe void Write(ref float source)
		{
			this.Type(RawHeader.Single);
			fixed (float* ptr = &source)
			{
				this.WriteRawBytes(ptr, 4);
			}
		}

		public unsafe void Write(ref double source)
		{
			this.Type(RawHeader.Double);
			fixed (double* ptr = &source)
			{
				this.WriteRawBytes(ptr, 8);
			}
		}

		public void Write(ref DateTime source)
		{
			this.WriteShortI64((long)(source - RawBuffer.UnixEpoc).TotalMilliseconds, RawHeader.DateTime);
		}

		public void Write(ref TimeSpan source)
		{
			this.WriteShortI32((int)source.TotalMilliseconds, RawHeader.Time);
		}

		public unsafe void Write(ref decimal source)
		{
			this.Type(RawHeader.Decimal);
			fixed (decimal* ptr = &source)
			{
				this.WriteRawBytes(ptr, 16);
			}
		}

		public unsafe void Write(ref Guid source)
		{
			this.Type(RawHeader.Guid);
			fixed (Guid* ptr = &source)
			{
				this.WriteRawBytes(ptr, 16);
			}
		}

		public void Write(ref string source)
		{
			bool flag = source == null;
			if (flag)
			{
				this.Type(RawHeader.Null);
			}
			else
			{
				byte[] bytes = Encoding.UTF8.GetBytes(source);
				this.WriteShortI16((short)bytes.Length, RawHeader.String);
				this.data.AddRange(bytes);
			}
		}

		public void Write(ref byte[] source)
		{
			bool flag = source == null;
			if (flag)
			{
				this.Type(RawHeader.Null);
			}
			else
			{
				this.WriteShortI32(source.Length, RawHeader.ByteArray);
				this.data.AddRange(source);
			}
		}

		public void WriteBytes(byte[] source)
		{
			this.data.AddRange(source);
		}
	}
	public class RawDeserializer : IAxDeserializer
	{
		private readonly RawBuffer buffer;

		private int fields_read = 0;

		public RawDeserializer(byte[] data)
		{
			this.buffer = new RawBuffer(data);
		}

		public RawDeserializer(RawBuffer data)
		{
			this.buffer = new RawBuffer(data);
		}

		private SerializationException Fail(RawHeader found, params RawHeader[] expected)
		{
			return new SerializationException(string.Concat(new object[]
			{
				"Incorrect field type header. expected (",
				string.Join<RawHeader>("|", expected),
				") but found (",
				found,
				") at position ",
				this.buffer.Pos,
				", field ",
				this.fields_read
			}));
		}

		private void Success()
		{
			this.fields_read++;
		}

		private RawHeader Expect(RawHeader expected)
		{
			RawHeader found = this.buffer.NextType();
			RawHeader? rawHeader = this.buffer.TrySimple(found, expected);
			bool hasValue = rawHeader.HasValue;
			if (hasValue)
			{
				return rawHeader.Value;
			}
			throw this.Fail(found, new RawHeader[]
			{
				expected
			});
		}

		private RawHeader? ExpectNull(RawHeader expected)
		{
			RawHeader found = this.buffer.NextType();
			RawHeader? rawHeader = this.buffer.TrySimple(found, expected);
			bool hasValue = rawHeader.HasValue;
			RawHeader? result;
			if (hasValue)
			{
				result = rawHeader;
			}
			else
			{
				bool hasValue2 = this.buffer.TrySimple(found, RawHeader.Null).HasValue;
				if (!hasValue2)
				{
					throw this.Fail(found, new RawHeader[]
					{
						expected,
						RawHeader.Null
					});
				}
				result = null;
			}
			return result;
		}

		public bool ReadNull()
		{
			bool flag = !this.buffer.TrySimple(this.buffer.NextType(), RawHeader.Null).HasValue;
			bool result;
			if (flag)
			{
				this.Success();
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public void ReadOpenObject()
		{
			this.Expect(RawHeader.OpenObject);
			this.Success();
		}

		public void ReadCloseObject()
		{
			this.Expect(RawHeader.CloseObject);
			this.Success();
		}

		public void ReadOpenArray()
		{
			this.Expect(RawHeader.OpenArray);
			this.Success();
		}

		public void ReadCloseArray()
		{
			this.Expect(RawHeader.CloseArray);
			this.Success();
		}

		public void Read(out bool target)
		{
			RawHeader found = this.buffer.NextType();
			bool hasValue = this.buffer.TrySimple(found, RawHeader.False).HasValue;
			if (hasValue)
			{
				target = false;
			}
			else
			{
				bool hasValue2 = this.buffer.TrySimple(found, RawHeader.True).HasValue;
				if (!hasValue2)
				{
					throw this.Fail(found, new RawHeader[]
					{
						RawHeader.False,
						RawHeader.True
					});
				}
				target = true;
			}
			this.Success();
		}

		public void Read(out byte target)
		{
			this.Expect(RawHeader.Byte);
			target = this.buffer.Byte();
			this.Success();
		}

		public void Read(out char target)
		{
			RawHeader size = this.Expect(RawHeader.Char);
			target = this.buffer.Char(size);
			this.Success();
		}

		public void Read(out short target)
		{
			RawHeader size = this.Expect(RawHeader.Int16);
			target = this.buffer.Int16(size);
			this.Success();
		}

		public void Read(out ushort target)
		{
			RawHeader size = this.Expect(RawHeader.UInt16);
			target = this.buffer.UInt16(size);
			this.Success();
		}

		public void Read(out int target)
		{
			RawHeader size = this.Expect(RawHeader.Int32);
			target = this.buffer.Int32(size);
			this.Success();
		}

		public void Read(out uint target)
		{
			RawHeader size = this.Expect(RawHeader.UInt32);
			target = this.buffer.UInt32(size);
			this.Success();
		}

		public void Read(out long target)
		{
			RawHeader size = this.Expect(RawHeader.Int64);
			target = this.buffer.Int64(size);
			this.Success();
		}

		public void Read(out ulong target)
		{
			RawHeader size = this.Expect(RawHeader.UInt64);
			target = this.buffer.UInt64(size);
			this.Success();
		}

		public void Read(out float target)
		{
			this.Expect(RawHeader.Single);
			target = this.buffer.Single();
			this.Success();
		}

		public void Read(out double target)
		{
			this.Expect(RawHeader.Double);
			target = this.buffer.Double();
			this.Success();
		}

		public void Read(out decimal target)
		{
			this.Expect(RawHeader.Decimal);
			target = this.buffer.Decimal();
			this.Success();
		}

		public void Read(out DateTime target)
		{
			this.Expect(RawHeader.DateTime);
			target = RawBuffer.UnixEpoc.AddMilliseconds((double)this.buffer.Int64(RawHeader.False));
			this.Success();
		}

		public void Read(out TimeSpan target)
		{
			RawHeader size = this.Expect(RawHeader.Time);
			target = new TimeSpan(0, 0, 0, 0, this.buffer.Int32(size));
			this.Success();
		}

		public void Read(out Guid target)
		{
			this.Expect(RawHeader.Guid);
			target = this.buffer.Guid();
			this.Success();
		}

		public void Read(out string target)
		{
			RawHeader? rawHeader = this.ExpectNull(RawHeader.String);
			target = (rawHeader.HasValue ? this.buffer.String(rawHeader.Value) : null);
			this.Success();
		}

		public void Read(out byte[] target)
		{
			RawHeader? rawHeader = this.ExpectNull(RawHeader.ByteArray);
			target = (rawHeader.HasValue ? this.buffer.Bytes(rawHeader.Value) : null);
			this.Success();
		}

		public void Read(out byte[] target, int count)
		{
			target = this.buffer.Bytes(count);
			this.Success();
		}
	}
	public class RawBuffer
	{
		private readonly byte[] Data;

		private int pos = 0;

		internal static DateTime UnixEpoc = new DateTime(1970, 1, 1);

		public int Pos
		{
			get
			{
				return this.pos;
			}
		}

		public RawBuffer(byte[] data)
		{
			this.Data = data;
		}

		public RawBuffer(byte[] data, int offset, int count)
		{
			this.Data = new byte[count];
			Array.Copy(data, offset, this.Data, 0, count);
		}

		public RawBuffer(RawBuffer data)
		{
			this.Data = data.Data;
		}

		public RawHeader NextType()
		{
			return (RawHeader)this.Data[this.pos];
		}

		public bool EndOfBuffer()
		{
			return this.pos >= this.Data.Length;
		}

		public void MoveNext()
		{
			this.pos++;
		}

		public RawHeader? TrySimple(RawHeader found, RawHeader expected)
		{
			bool flag = (found & RawHeader.TypeMask) == expected;
			RawHeader? result;
			if (flag)
			{
				this.MoveNext();
				result = new RawHeader?(found & RawHeader.SizeMask);
			}
			else
			{
				result = null;
			}
			return result;
		}

		public byte Byte()
		{
			byte result = this.Data[this.pos];
			this.pos++;
			return result;
		}

		public unsafe char Char(RawHeader size)
		{
			bool flag = size == RawHeader.Size1;
			char result;
			fixed (byte* buf = Data)
			{
				if (flag)
				{
					char c = (char)*(byte*)(buf + pos);
					this.pos++;
					result = c;
				}
				else
				{
					char c2 = (char)*(ushort*)(buf + pos);
					this.pos += 2;
					result = c2;
				}
			}
			return result;
		}

		public unsafe ushort UInt16(RawHeader size)
		{
			bool flag = size == RawHeader.Size1;
			ushort result;
			fixed (byte* buf = Data)
			{
				if (flag)
				{
					ushort num = (ushort)*(byte*)(buf + pos);
					this.pos++;
					result = num;
				}
				else
				{
					ushort num2 = *(ushort*)(buf + pos);
					this.pos += 2;
					result = num2;
				}
			}
			return result;
		}

		public unsafe uint UInt32(RawHeader size)
		{
			bool flag = size == RawHeader.Size1;
			uint result;
			fixed (byte* buf = Data)
			{
				if (flag)
				{
					uint num = (uint) (*(buf + pos));
					this.pos++;
					result = num;
				}
				else
				{
					bool flag2 = size == RawHeader.Size2;
					if (flag2)
					{
						uint num2 = (uint) (*(ushort*) (buf + pos));
						this.pos += 2;
						result = num2;
					}
					else
					{
						bool flag3 = size == RawHeader.Size3;
						if (flag3)
						{
							uint num3 = (uint) ((int) (*(ushort*) (buf + pos)) | (int) (buf + pos)[2] << 16);
							this.pos += 3;
							result = num3;
						}
						else
						{
							uint num4 = *(uint*) (buf + pos);
							this.pos += 4;
							result = num4;
						}
					}
				}
			}
			return result;
		}

		public unsafe ulong UInt64(RawHeader size)
		{
			bool flag = size == RawHeader.Size1;
			ulong result;
			fixed (byte* buf = Data)
			{
				if (flag)
				{
					ulong num = (ulong) (*(buf + pos));
					this.pos++;
					result = num;
				}
				else
				{
					bool flag2 = size == RawHeader.Size2;
					if (flag2)
					{
						ulong num2 = (ulong) (*(ushort*) (buf + pos));
						this.pos += 2;
						result = num2;
					}
					else
					{
						bool flag3 = size == RawHeader.Size3;
						if (flag3)
						{
							ulong num3 = (ulong) (*(ushort*) (buf + pos)) | (ulong) (buf + pos)[2] << 16;
							this.pos += 3;
							result = num3;
						}
						else
						{
							bool flag4 = size == RawHeader.Size4;
							if (flag4)
							{
								ulong num4 = (ulong) (*(uint*) (buf + pos));
								this.pos += 4;
								result = num4;
							}
							else
							{
								bool flag5 = size == RawHeader.Size5;
								if (flag5)
								{
									ulong num5 = (ulong) (*(uint*) (buf + pos)) | (ulong) (buf + pos)[4] << 32;
									this.pos += 5;
									result = num5;
								}
								else
								{
									bool flag6 = size == RawHeader.Size6;
									if (flag6)
									{
										ulong num6 = (ulong) (*(uint*) (buf + pos)) |
										             (ulong) ((ushort*) (buf + pos))[2] << 32;
										this.pos += 6;
										result = num6;
									}
									else
									{
										bool flag7 = size == RawHeader.SizeMask;
										if (flag7)
										{
											ulong num7 = (ulong) (*(uint*) (buf + pos)) |
											             (ulong) ((ushort*) (buf + pos))[2] << 32 |
											             (ulong) (buf + pos)[6] << 48;
											this.pos += 7;
											result = num7;
										}
										else
										{
											ulong num8 = (ulong) (*(long*) (buf + pos));
											this.pos += 8;
											result = num8;
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		public unsafe short Int16(RawHeader size)
		{
			bool flag = size == RawHeader.Size1;
			short result;
			fixed (byte* buf = Data)
			{
				if (flag)
				{
					short num = (short) (*(sbyte*) (buf + pos));
					this.pos++;
					result = num;
				}
				else
				{
					short num2 = *(short*) (buf + pos);
					this.pos += 2;
					result = num2;
				}
			}
			return result;
		}

		public unsafe int Int32(RawHeader size)
		{
			bool flag = size == RawHeader.Size1;
			int result;
			fixed (byte* buf = Data)
			{
				if (flag)
				{
					int num = (int) (*(sbyte*) (buf + pos));
					this.pos++;
					result = num;
				}
				else
				{
					bool flag2 = size == RawHeader.Size2;
					if (flag2)
					{
						int num2 = (int) (*(short*) (buf + pos));
						this.pos += 2;
						result = num2;
					}
					else
					{
						bool flag3 = size == RawHeader.Size3;
						if (flag3)
						{
							int num3 = (int) (*(ushort*) (buf + pos)) | (int) ((sbyte*) (buf + pos))[2] << 16;
							this.pos += 3;
							result = num3;
						}
						else
						{
							int num4 = *(int*) (buf + pos);
							this.pos += 4;
							result = num4;
						}
					}
				}
			}
			return result;
		}

		public unsafe long Int64(RawHeader size)
		{
			bool flag = size == RawHeader.Size1;
			long result;
			fixed (byte* buf = Data)
			{
				if (flag)
				{
					long num = (long) (*(sbyte*) (buf + pos));
					this.pos++;
					result = num;
				}
				else
				{
					bool flag2 = size == RawHeader.Size2;
					if (flag2)
					{
						long num2 = (long) (*(short*) (buf + pos));
						this.pos += 2;
						result = num2;
					}
					else
					{
						bool flag3 = size == RawHeader.Size3;
						if (flag3)
						{
							long num3 =
								(long)
									((ulong) (*(ushort*) (buf + pos)) |
									 (ulong) ((ulong) ((long) ((sbyte*) (buf + pos))[2]) << 16));
							this.pos += 3;
							result = num3;
						}
						else
						{
							bool flag4 = size == RawHeader.Size4;
							if (flag4)
							{
								long num4 = (long) (*(int*) (buf + pos));
								this.pos += 4;
								result = num4;
							}
							else
							{
								bool flag5 = size == RawHeader.Size5;
								if (flag5)
								{
									long num5 =
										(long)
											((ulong) (*(uint*) (buf + pos)) |
											 (ulong) ((ulong) ((long) ((sbyte*) (buf + pos))[4]) << 32));
									this.pos += 5;
									result = num5;
								}
								else
								{
									bool flag6 = size == RawHeader.Size6;
									if (flag6)
									{
										long num6 =
											(long)
												((ulong) (*(uint*) (buf + pos)) |
												 (ulong) ((ulong) ((long) ((short*) (buf + pos))[2]) << 32));
										this.pos += 6;
										result = num6;
									}
									else
									{
										bool flag7 = size == RawHeader.SizeMask;
										if (flag7)
										{
											long num7 =
												(long)
													((ulong) (*(uint*) (buf + pos)) | (ulong) ((ushort*) (buf + pos))[2] << 32 |
													 (ulong) ((ulong) ((long) ((sbyte*) (buf + pos))[6]) << 48));
											this.pos += 7;
											result = num7;
										}
										else
										{
											long num8 = *(long*) (buf + pos);
											this.pos += 8;
											result = num8;
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		public unsafe float Single()
		{
			float result;
			fixed (byte* buf = Data)
			{
				result = *(float*) (buf + pos);
				this.pos += 4;
			}
			return result;
		}

		public unsafe double Double()
		{
			double result;
			fixed (byte* buf = Data)
			{
				result = *(double*) (buf + pos);
				this.pos += 8;
			}
			return result;
		}

		public unsafe decimal Decimal()
		{
			int[] array = new int[4];
			fixed (byte* buf = Data)
			{
				for (int i = 0; i < 4; i++)
				{
					array[i] = *(int*)(buf + i*4);
				}
			}
			this.pos += 16;
			return new decimal(array);
		}

		public DateTime DateTime()
		{
			return RawBuffer.UnixEpoc.AddMilliseconds((double)this.Int64(RawHeader.False));
		}

		public TimeSpan Time(RawHeader size)
		{
			return new TimeSpan(0, 0, 0, 0, this.Int32(size));
		}

		public Guid Guid()
		{
			return new Guid(this.Bytes(16));
		}

		public byte[] Bytes(int count)
		{
			byte[] array = new byte[count];
			Array.Copy(this.Data, this.pos, array, 0, count);
			this.pos += count;
			return array;
		}

		public string String(RawHeader size)
		{
			return Encoding.UTF8.GetString(this.Bytes((int)this.Int16(size)));
		}

		public byte[] Bytes(RawHeader size)
		{
			return this.Bytes(this.Int32(size));
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
			this.buffer = new RawBuffer(data, offset, count);
			this.Items = new List<string>();
			this.print();
		}

		public RawBufferInspector(byte[] data)
		{
			this.buffer = new RawBuffer(data);
			this.Items = new List<string>();
			this.print();
		}

		public RawBufferInspector(RawBuffer data)
		{
			this.buffer = new RawBuffer(data);
			this.Items = new List<string>();
			this.print();
		}

		public override string ToString()
		{
			return string.Join(Environment.NewLine, this.Items);
		}

		private void print()
		{
			while (this.print(0) && !this.buffer.EndOfBuffer())
			{
			}
		}

		private bool print(int depth)
		{
			RawHeader FieldSize = RawHeader.False;
			string FieldSizeStr = "";
			Func<RawHeader, bool> func = delegate(RawHeader expected)
			{
				RawHeader? rawHeader = this.buffer.TrySimple(this.buffer.NextType(), expected);
				bool hasValue = rawHeader.HasValue;
				if (hasValue)
				{
					FieldSize = rawHeader.Value;
					FieldSizeStr = ((FieldSize == RawHeader.Size1) ? ":1" : ((FieldSize == RawHeader.Size2) ? ":2" : ((FieldSize == RawHeader.Size3) ? ":3" : ((FieldSize == RawHeader.Size4) ? ":4" : ((FieldSize == RawHeader.Size5) ? ":5" : ((FieldSize == RawHeader.Size6) ? ":6" : ((FieldSize == RawHeader.SizeMask) ? ":7" : "  ")))))));
				}
				return rawHeader.HasValue;
			};
			bool flag = this.buffer.EndOfBuffer();
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool flag2 = func(RawHeader.CloseObject);
				if (flag2)
				{
					this.print(depth, "}");
					this.print(depth, "ERROR: UNEXPECTED CLOSE OF OBJECT");
					result = false;
				}
				else
				{
					bool flag3 = func(RawHeader.CloseArray);
					if (flag3)
					{
						this.print(depth, "]");
						this.print(depth, "ERROR: UNEXPECTED CLOSE OF ARRAY");
						result = false;
					}
					else
					{
						bool flag4 = func(RawHeader.OpenObject);
						if (flag4)
						{
							this.print(depth, "OBJECT");
							this.print(depth, "{");
							while (true)
							{
								bool flag5 = this.buffer.EndOfBuffer();
								if (flag5)
								{
									break;
								}
								bool flag6 = func(RawHeader.CloseObject);
								if (flag6)
								{
									goto Block_6;
								}
								bool flag7 = !this.print(depth + 1);
								if (flag7)
								{
									goto Block_7;
								}
							}
							this.print(depth + 1, "ERROR: UNEXPECTED END OF BUFFER");
							result = false;
							return result;
							Block_6:
							this.print(depth, "}");
							result = true;
							return result;
							Block_7:
							result = false;
						}
						else
						{
							bool flag8 = func(RawHeader.OpenArray);
							if (flag8)
							{
								bool flag9 = this.buffer.EndOfBuffer();
								if (flag9)
								{
									this.print(depth, "ARRAY[???]");
									this.print(depth, "[");
									this.print(depth + 1, "ERROR: UNEXPECTED END OF BUFFER");
									result = false;
								}
								else
								{
									bool flag10 = func(RawHeader.Int32);
									if (flag10)
									{
										this.print(depth, string.Concat(new object[]
										{
											"ARRAY[",
											this.buffer.Int32(FieldSize),
											FieldSizeStr,
											"]"
										}));
										this.print(depth, "[");
										while (true)
										{
											bool flag11 = this.buffer.EndOfBuffer();
											if (flag11)
											{
												break;
											}
											bool flag12 = func(RawHeader.CloseArray);
											if (flag12)
											{
												goto Block_12;
											}
											bool flag13 = !this.print(depth + 1);
											if (flag13)
											{
												goto Block_13;
											}
										}
										this.print(depth + 1, "ERROR: UNEXPECTED END OF BUFFER");
										result = false;
										return result;
										Block_12:
										this.print(depth, "]");
										result = true;
										return result;
										Block_13:
										result = false;
									}
									else
									{
										this.print(depth, "ARRAY[???]");
										this.print(depth, "[");
										this.print(depth + 1, "ERROR: MISSING INT32 FOR ARRAY LENGHT");
										result = false;
									}
								}
							}
							else
							{
								bool flag14 = func(RawHeader.Null);
								if (flag14)
								{
									this.print(depth, "NULL     NULL");
								}
								else
								{
									bool flag15 = func(RawHeader.True);
									if (flag15)
									{
										this.print(depth, "BOOL     TRUE");
									}
									else
									{
										bool flag16 = func(RawHeader.False);
										if (flag16)
										{
											this.print(depth, "BOOL     FALSE");
										}
										else
										{
											bool flag17 = func(RawHeader.Byte);
											if (flag17)
											{
												this.print(depth, "BYTE     " + this.buffer.Byte());
											}
											else
											{
												bool flag18 = func(RawHeader.Char);
												if (flag18)
												{
													this.print(depth, string.Concat(new string[]
													{
														"CHAR",
														FieldSizeStr,
														"   '",
														this.buffer.Char(FieldSize).ToString(),
														"'"
													}));
												}
												else
												{
													bool flag19 = func(RawHeader.Int16);
													if (flag19)
													{
														this.print(depth, string.Concat(new object[]
														{
															"INT16",
															FieldSizeStr,
															"  ",
															this.buffer.Int16(FieldSize)
														}));
													}
													else
													{
														bool flag20 = func(RawHeader.UInt16);
														if (flag20)
														{
															this.print(depth, string.Concat(new object[]
															{
																"UINT16",
																FieldSizeStr,
																" ",
																this.buffer.UInt16(FieldSize)
															}));
														}
														else
														{
															bool flag21 = func(RawHeader.Int32);
															if (flag21)
															{
																this.print(depth, string.Concat(new object[]
																{
																	"INT32",
																	FieldSizeStr,
																	"  ",
																	this.buffer.Int32(FieldSize)
																}));
															}
															else
															{
																bool flag22 = func(RawHeader.UInt32);
																if (flag22)
																{
																	this.print(depth, string.Concat(new object[]
																	{
																		"UINT32",
																		FieldSizeStr,
																		" ",
																		this.buffer.UInt32(FieldSize)
																	}));
																}
																else
																{
																	bool flag23 = func(RawHeader.Int64);
																	if (flag23)
																	{
																		this.print(depth, string.Concat(new object[]
																		{
																			"INT64",
																			FieldSizeStr,
																			"  ",
																			this.buffer.Int64(FieldSize)
																		}));
																	}
																	else
																	{
																		bool flag24 = func(RawHeader.UInt64);
																		if (flag24)
																		{
																			this.print(depth, string.Concat(new object[]
																			{
																				"UINT64",
																				FieldSizeStr,
																				" ",
																				this.buffer.UInt64(FieldSize)
																			}));
																		}
																		else
																		{
																			bool flag25 = func(RawHeader.Single);
																			if (flag25)
																			{
																				this.print(depth, "SINGLE   " + this.buffer.Single());
																			}
																			else
																			{
																				bool flag26 = func(RawHeader.Double);
																				if (flag26)
																				{
																					this.print(depth, "DOUBLE   " + this.buffer.Double());
																				}
																				else
																				{
																					bool flag27 = func(RawHeader.Decimal);
																					if (flag27)
																					{
																						this.print(depth, "DECIMAL  " + this.buffer.Decimal());
																					}
																					else
																					{
																						bool flag28 = func(RawHeader.DateTime);
																						if (flag28)
																						{
																							this.print(depth, "DATETIME " + this.buffer.DateTime());
																						}
																						else
																						{
																							bool flag29 = func(RawHeader.Time);
																							if (flag29)
																							{
																								this.print(depth, string.Concat(new object[]
																								{
																									"TIME",
																									FieldSizeStr,
																									"   ",
																									this.buffer.Time(FieldSize)
																								}));
																							}
																							else
																							{
																								bool flag30 = func(RawHeader.Guid);
																								if (flag30)
																								{
																									this.print(depth, "GUID     " + this.buffer.Guid());
																								}
																								else
																								{
																									bool flag31 = func(RawHeader.String);
																									if (flag31)
																									{
																										this.print(depth, string.Concat(new string[]
																										{
																											"STRING",
																											FieldSizeStr,
																											" \"",
																											this.buffer.String(FieldSize),
																											"\""
																										}));
																									}
																									else
																									{
																										bool flag32 = func(RawHeader.ByteArray);
																										if (!flag32)
																										{
																											this.print(depth, "ERROR: UNEXPECTED TYPE HEADER " + this.buffer.NextType());
																											result = false;
																											return result;
																										}
																										byte[] array = this.buffer.Bytes(FieldSize);
																										object[] expr_782 = new object[5];
																										expr_782[0] = "BYTE[";
																										expr_782[1] = array.Length;
																										expr_782[2] = FieldSizeStr;
																										expr_782[3] = "] ";
																										int arg_7D9_1 = 4;
																										string arg_7D4_0 = " ";
																										expr_782[arg_7D9_1] = string.Join(arg_7D4_0, array.Select(b => b.ToString("X02")));
																										this.print(depth, string.Concat(expr_782));
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
			this.Items.Add(new string(' ', 4 * depth) + str);
		}
	}
}
