using System.Reflection.Emit;

namespace LaWare.Utility.Emitter
{
	public static class OpCodeDump
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
				Value = op.Size == 1 ? op.Value.ToString("X2") + "h" : op.Value.ToString("X4") + "h";
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
					FlowControl = op.OpCodeType == System.Reflection.Emit.OpCodeType.Nternal ? "zMeta" : op.FlowControl.ToString().Replace("Cond_Branch", "Branch_Cond").Replace("Next", "").Replace("Meta", "xMeta");
				}
			}

			private static string fixpoppush(string v, string poppush)
			{
				return v.ToLower().Replace(poppush + "0", "").Replace(poppush + "1", poppush + " ").Replace(poppush + "ref", poppush + "r").Replace("var" + poppush, poppush + "...").Replace("i8", "l").Replace("r4", "f").Replace("r8", "d").Replace("_", " ");
			}
		}

		public static DumpInfo GetInfo(this OpCode op, bool simplify = false)
		{
			return new DumpInfo(op, simplify);
		}
	}
}
