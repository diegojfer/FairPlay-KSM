using System;

namespace FoolishTech.Support.Binary
{
    internal static class BinaryConverter
    {
        internal enum Endianess 
        {
            SystemEndian,
            LittleEndian,
            BigEndian
        }

		internal static SByte ReadInt8(byte[] buffer) => ReadInt8(new ReadOnlyMemory<byte>(buffer));
		internal static SByte ReadInt8(ReadOnlyMemory<byte> buffer) => ReadInt8(buffer.Span);
		internal static SByte ReadInt8(ReadOnlySpan<byte> buffer)	
		{
			if (buffer.Length != 1) throw new ArgumentException(nameof(buffer), "Invalid buffer length. The length must be the same of target integer.");
	
			return (SByte)buffer.ToArray()[0];
		}
		internal static byte[] WriteInt8(SByte integer) => new byte[] { (byte)integer };

		internal static Byte ReadUInt8(byte[] buffer) => ReadUInt8(new ReadOnlyMemory<byte>(buffer));
		internal static Byte ReadUInt8(ReadOnlyMemory<byte> buffer) => ReadUInt8(buffer.Span);
		internal static Byte ReadUInt8(ReadOnlySpan<byte> buffer)
		{
			if (buffer.Length != 1) throw new ArgumentException(nameof(buffer), "Invalid buffer length. The length must be the same of target integer.");
	
			return (Byte)buffer.ToArray()[0];
		}
		internal static byte[] WriteUInt8(Byte integer) => new byte[] { integer };

		internal static Int16 ReadInt16(byte[] buffer, Endianess endianess = Endianess.SystemEndian) => ReadInt16(new ReadOnlyMemory<byte>(buffer), endianess);
		internal static Int16 ReadInt16(ReadOnlyMemory<byte> buffer, Endianess endianess = Endianess.SystemEndian) => ReadInt16(buffer.Span, endianess);
		internal static Int16 ReadInt16(ReadOnlySpan<byte> buffer, Endianess endianess = Endianess.SystemEndian)
		{
			if (buffer.Length != 2) throw new ArgumentException(nameof(buffer), "Invalid buffer length. The length must be the same of target integer.");
			
			if (ShouldReverseBytes(endianess)) {
				var span = new Span<byte>(new byte[2]);
				buffer.CopyTo(span);
				span.Reverse();
				return BitConverter.ToInt16(span);
			} else {
				return BitConverter.ToInt16(buffer);
			}
		}
		internal static byte[] WriteInt16(Int16 integer, Endianess endianess = Endianess.SystemEndian) 
		{
			byte[] buffer = BitConverter.GetBytes(integer);

			if (ShouldReverseBytes(endianess)) {
				var span = new Span<byte>(buffer); 
				span.Reverse();
				buffer = span.ToArray();
			}
			
			return buffer;
		}

		internal static UInt16 ReadUInt16(byte[] buffer, Endianess endianess = Endianess.SystemEndian) => ReadUInt16(new ReadOnlyMemory<byte>(buffer), endianess);
		internal static UInt16 ReadUInt16(ReadOnlyMemory<byte> buffer, Endianess endianess = Endianess.SystemEndian) => ReadUInt16(buffer.Span, endianess);
		internal static UInt16 ReadUInt16(ReadOnlySpan<byte> buffer, Endianess endianess = Endianess.SystemEndian)
		{
			if (buffer.Length != 2) throw new ArgumentException(nameof(buffer), "Invalid buffer length. The length must be the same of target integer.");
			
			if (ShouldReverseBytes(endianess)) {
				var span = new Span<byte>(new byte[2]);
				buffer.CopyTo(span);
				span.Reverse();
				return BitConverter.ToUInt16(span);
			} else {
				return BitConverter.ToUInt16(buffer);
			}
		}
		internal static byte[] WriteUInt16(UInt16 integer, Endianess endianess = Endianess.SystemEndian) 
		{
			byte[] buffer = BitConverter.GetBytes(integer);

			if (ShouldReverseBytes(endianess)) {
				var span = new Span<byte>(buffer); 
				span.Reverse();
				buffer = span.ToArray();
			}
			
			return buffer;
		}

		internal static Int32 ReadInt32(byte[] buffer, Endianess endianess = Endianess.SystemEndian) => ReadInt32(new ReadOnlyMemory<byte>(buffer), endianess);
		internal static Int32 ReadInt32(ReadOnlyMemory<byte> buffer, Endianess endianess = Endianess.SystemEndian) => ReadInt32(buffer.Span, endianess);
		internal static Int32 ReadInt32(ReadOnlySpan<byte> buffer, Endianess endianess = Endianess.SystemEndian)
		{
			if (buffer.Length != 4) throw new ArgumentException(nameof(buffer), "Invalid buffer length. The length must be the same of target integer.");
			
			if (ShouldReverseBytes(endianess)) {
				var span = new Span<byte>(new byte[4]);
				buffer.CopyTo(span);
				span.Reverse();
				return BitConverter.ToInt32(span);
			} else {
				return BitConverter.ToInt32(buffer);
			}
		}
		internal static byte[] WriteInt32(Int32 integer, Endianess endianess = Endianess.SystemEndian) 
		{
			byte[] buffer = BitConverter.GetBytes(integer);

			if (ShouldReverseBytes(endianess)) {
				var span = new Span<byte>(buffer); 
				span.Reverse();
				buffer = span.ToArray();
			}
			
			return buffer;
		}

        internal static UInt32 ReadUInt32(byte[] buffer, Endianess endianess = Endianess.SystemEndian) => ReadUInt32(new ReadOnlyMemory<byte>(buffer), endianess);
		internal static UInt32 ReadUInt32(ReadOnlyMemory<byte> buffer, Endianess endianess = Endianess.SystemEndian) => ReadUInt32(buffer.Span, endianess);
		internal static UInt32 ReadUInt32(ReadOnlySpan<byte> buffer, Endianess endianess = Endianess.SystemEndian)
		{
			if (buffer.Length != 4) throw new ArgumentException(nameof(buffer), "Invalid buffer length. The length must be the same of target integer.");
			
			if (ShouldReverseBytes(endianess)) {
				var span = new Span<byte>(new byte[4]);
				buffer.CopyTo(span);
				span.Reverse();
				return BitConverter.ToUInt32(span);
			} else {
				return BitConverter.ToUInt32(buffer);
			}
		}
		internal static byte[] WriteUInt32(UInt32 integer, Endianess endianess = Endianess.SystemEndian) 
		{
			byte[] buffer = BitConverter.GetBytes(integer);

			if (ShouldReverseBytes(endianess)) {
				var span = new Span<byte>(buffer); 
				span.Reverse();
				buffer = span.ToArray();
			}
			
			return buffer;
		}

        internal static Int64 ReadInt64(byte[] buffer, Endianess endianess = Endianess.SystemEndian) => ReadInt64(new ReadOnlyMemory<byte>(buffer), endianess);
		internal static Int64 ReadInt64(ReadOnlyMemory<byte> buffer, Endianess endianess = Endianess.SystemEndian) => ReadInt64(buffer.Span, endianess);
		internal static Int64 ReadInt64(ReadOnlySpan<byte> buffer, Endianess endianess = Endianess.SystemEndian)
		{
			if (buffer.Length != 8) throw new ArgumentException(nameof(buffer), "Invalid buffer length. The length must be the same of target integer.");
			
			if (ShouldReverseBytes(endianess)) {
				var span = new Span<byte>(new byte[8]);
				buffer.CopyTo(span);
				span.Reverse();
				return BitConverter.ToInt64(span);
			} else {
				return BitConverter.ToInt64(buffer);
			}
		}
		internal static byte[] WriteInt64(Int64 integer, Endianess endianess = Endianess.SystemEndian) 
		{
			byte[] buffer = BitConverter.GetBytes(integer);

			if (ShouldReverseBytes(endianess)) {
				var span = new Span<byte>(buffer); 
				span.Reverse();
				buffer = span.ToArray();
			}
			
			return buffer;
		}

        internal static UInt64 ReadUInt64(byte[] buffer, Endianess endianess = Endianess.SystemEndian) => ReadUInt64(new ReadOnlyMemory<byte>(buffer), endianess);
        internal static UInt64 ReadUInt64(ReadOnlyMemory<byte> buffer, Endianess endianess = Endianess.SystemEndian) => ReadUInt64(buffer.Span, endianess);
        internal static UInt64 ReadUInt64(ReadOnlySpan<byte> buffer, Endianess endianess = Endianess.SystemEndian)
        {
            if (buffer.Length != 8) throw new ArgumentException(nameof(buffer), "Invalid buffer length. The length must be the same of target integer.");

            if (ShouldReverseBytes(endianess)) {
                var span = new Span<byte>(new byte[8]);
                buffer.CopyTo(span);
                span.Reverse();
                return BitConverter.ToUInt64(span);
            } else {
                return BitConverter.ToUInt64(buffer);
            }
        }				
		internal static byte[] WriteUInt64(UInt64 integer, Endianess endianess = Endianess.SystemEndian) 
		{
			byte[] buffer = BitConverter.GetBytes(integer);

			if (ShouldReverseBytes(endianess)) {
				var span = new Span<byte>(buffer); 
				span.Reverse();
				buffer = span.ToArray();
			}
			
			return buffer;
		}

        internal static bool ShouldReverseBytes(Endianess endianess) 
        {
            if (endianess == Endianess.SystemEndian) return false;
            if (endianess == Endianess.LittleEndian && BitConverter.IsLittleEndian == false) return true;
            if (endianess == Endianess.LittleEndian && BitConverter.IsLittleEndian == true) return false;
            if (endianess == Endianess.BigEndian && BitConverter.IsLittleEndian == true) return true;
            if (endianess == Endianess.BigEndian && BitConverter.IsLittleEndian == false) return false;
             
            throw new ArgumentException(nameof(endianess), $"Endianness should be [{nameof(Endianess.SystemEndian)}, {nameof(Endianess.LittleEndian)} or {nameof(Endianess.BigEndian)}].");
        }
    }   
}