// E:\GitHub\LaWare.Utility_\TSqlParseInfo\bin\Debug\TSqlParseInfo.exe
// TSqlParseInfo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

// Global type: <Module>
// Entry point: TSqlParseInfo.Program.Main
// Architecture: AnyCPU (32-bit preferred)
// Runtime: .NET 4.0

using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace TSqlParseInfo
{
	public class Form1 : Form
	{

		private IContainer components = null;

		private RichTextBox rtb1;

		private Button bParse;

		private RichTextBox rtb2;

		private CheckBox cbHide;

		private TableLayoutPanel tableLayoutPanel1;

		private Panel panel1;

		public Form1()
		{
			this.InitializeComponent();
			this.rtb1.KeyDown += new KeyEventHandler(this.Rtb1_KeyDown);
		}

		private void cbHide_CheckedChanged(object sender, EventArgs e)
		{
			this.bParse_Click(null, null);
		}

		private void Rtb1_KeyDown(object sender, KeyEventArgs e)
		{
			var flag = e.KeyCode == Keys.F5;
			if (flag)
			{
				this.bParse_Click(null, null);
			}
		}

		private void bParse_Click(object sender, EventArgs e)
		{
			this.rtb2.Clear();
			var tSql120Parser = new TSql120Parser(true);
			IList<ParseError> list;
			var fragment = tSql120Parser.Parse(new StringReader(this.rtb1.Text), out list);
			var flag = list.Any<ParseError>();
			if (flag)
			{
				this.rtb2.SelectionColor = Color.Red;
				foreach (var current in list)
				{
					this.rtb2.AppendText(string.Format("error {0}: ({1},{2}): {3}{4}", new object[]
					{
						current.Number,
						current.Line,
						current.Column,
						current.Message,
						Environment.NewLine
					}));
				}
			}
			else
			{
				try
				{
					this.EmitFragment(0, fragment);
				}
				catch (Exception ex)
				{
					this.rtb2.SelectionColor = Color.Red;
					this.rtb2.AppendText(ex.ToString());
				}
			}
		}

		private void EmitIndex(int indent, int index, Type type)
		{
			this.rtb2.SelectionColor = Color.Black;
			this.rtb2.AppendText(new string(' ', indent * 4));
			this.rtb2.AppendText(string.Concat(new object[]
			{
				"[",
				index,
				"] (",
				type.Name,
				")"
			}));
			this.rtb2.AppendText(Environment.NewLine);
		}

		private void EmitLine(int indent, PropertyInfo p, object v, bool isDefaultValue = false)
		{
			isDefaultValue = (isDefaultValue || v == null);
			var flag = isDefaultValue && this.cbHide.Checked;
			if (!flag)
			{
				this.rtb2.SelectionColor = (isDefaultValue ? Color.LightGray : Color.Black);
				this.rtb2.AppendText(new string(' ', indent * 4));
				var flag2 = p != null;
				if (flag2)
				{
					this.rtb2.SelectionColor = (isDefaultValue ? Color.LightGray : Color.DarkGray);
					this.rtb2.AppendText(p.DeclaringType.Name + ".");
					var flag3 = !isDefaultValue;
					if (flag3)
					{
						this.rtb2.SelectionFont = new Font(this.rtb2.Font, FontStyle.Bold);
					}
					this.rtb2.SelectionColor = (isDefaultValue ? Color.LightGray : Color.Black);
					this.rtb2.AppendText(p.Name + " ");
					this.rtb2.SelectionFont = this.rtb2.Font;
					this.rtb2.AppendText(" = " + v);
				}
				this.rtb2.AppendText(Environment.NewLine);
			}
		}

		private void EmitFragmentList(int indent, PropertyInfo p, IEnumerable<TSqlFragment> list)
		{
			var num = 0;
			foreach (var current in list)
			{
				this.EmitIndex(indent + 1, num++, current.GetType());
				this.EmitFragment(indent + 1, current);
			}
		}

		private void EmitFragment(int indent, TSqlFragment fragment)
		{
			var t = fragment.GetType();
			Func<PropertyInfo, bool> __9__0 = null;
			while (t != null && t != typeof(TSqlFragment))
			{
				var properties = TypeDescriptor.GetProperties(t);
				IEnumerable<PropertyInfo> arg_50_0 = t.GetProperties();
				Func<PropertyInfo, bool> arg_50_1;
				if ((arg_50_1 = __9__0) == null)
				{
					arg_50_1 = (__9__0 = ((PropertyInfo e) => e.DeclaringType == t && e.GetIndexParameters().Length == 0));
				}
				foreach (var current in arg_50_0.Where(arg_50_1))
				{
					var propertyDescriptor = properties[current.Name];
					var value = current.GetValue(fragment);
					var enumerable = value as IEnumerable<TSqlFragment>;
					var flag = value == null;
					if (flag)
					{
						this.EmitLine(indent + 1, current, null, false);
					}
					else
					{
						var flag2 = enumerable != null;
						if (flag2)
						{
							var array = enumerable.ToArray<TSqlFragment>();
							this.EmitLine(indent + 1, current, (array.Length == 0) ? null : string.Format(" ({0}[])", value.GetType().GetGenericArguments()[0].Name), false);
							this.EmitFragmentList(indent + 1, current, array);
						}
						else
						{
							var flag3 = value is Literal;
							if (flag3)
							{
								var literal = (Literal)value;
								this.EmitLine(indent + 1, current, string.Format(" ({0})  '{1}'", value.GetType().Name, literal.Value), false);
							}
							else
							{
								var flag4 = value is Identifier;
								if (flag4)
								{
									var identifier = (Identifier)value;
									this.EmitLine(indent + 1, current, string.Format(" ({0})  '{1}'", value.GetType().Name, identifier.Value), false);
								}
								else
								{
									var flag5 = value is IdentifierOrValueExpression;
									if (flag5)
									{
										var identifierOrValueExpression = (IdentifierOrValueExpression)value;
										this.EmitLine(indent + 1, current, string.Format(" ({0})  '{1}'", value.GetType().Name, identifierOrValueExpression.Value), false);
									}
									else
									{
										var flag6 = value is ColumnReferenceExpression;
										if (flag6)
										{
											var columnReferenceExpression = (ColumnReferenceExpression) value;
											var arg_255_1 = indent + 1;
											var arg_255_2 = current;
											var arg_24F_0 = " ({0})  {1}";
											object arg_24F_1 = value.GetType().Name;
											var arg_24A_0 = ".";
											IEnumerable<Identifier> arg_245_0 = columnReferenceExpression.MultiPartIdentifier.Identifiers;
											var arg_245_1 = (new Func<Identifier, string>(id => "[" + id.Value + "]"));
											this.EmitLine(arg_255_1, arg_255_2,
												string.Format(arg_24F_0, arg_24F_1, string.Join(arg_24A_0, arg_245_0.Select(arg_245_1))), false);
										}
										else
										{
											var flag7 = value is SchemaObjectName;
											if (flag7)
											{
												var schemaObjectName = (SchemaObjectName)value;
												var arg_2CD_1 = indent + 1;
												var arg_2CD_2 = current;
												var arg_2C7_0 = " ({0})  {1}";
												object arg_2C7_1 = value.GetType().Name;
												var arg_2C2_0 = ".";
												IEnumerable<Identifier> arg_2BD_0 = schemaObjectName.Identifiers;
												var arg_2BD_1 = ( new Func<Identifier, string>(id => "[" + id.Value + "]"));
												this.EmitLine(arg_2CD_1, arg_2CD_2, string.Format(arg_2C7_0, arg_2C7_1, string.Join(arg_2C2_0, arg_2BD_0.Select(arg_2BD_1))), false);
											}
											else
											{
												var flag8 = typeof(TSqlFragment).IsAssignableFrom(current.PropertyType);
												if (flag8)
												{
													this.EmitLine(indent + 1, current, " (" + value.GetType().Name + ")", false);
													this.EmitFragment(indent + 1, (TSqlFragment)value);
												}
												else
												{
													try
													{
														this.EmitLine(indent + 1, current, value, Convert.ToInt64(value).Equals(0L));
													}
													catch
													{
														this.EmitLine(indent + 1, current, value, value.Equals(0));
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
				t = t.BaseType;
			}
		}

		protected override void Dispose(bool disposing)
		{
			var flag = disposing && this.components != null;
			if (flag)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.rtb1 = new RichTextBox();
			this.bParse = new Button();
			this.rtb2 = new RichTextBox();
			this.cbHide = new CheckBox();
			this.tableLayoutPanel1 = new TableLayoutPanel();
			this.panel1 = new Panel();
			this.tableLayoutPanel1.SuspendLayout();
			this.panel1.SuspendLayout();
			base.SuspendLayout();
			this.rtb1.Dock = DockStyle.Fill;
			this.rtb1.Location = new Point(3, 3);
			this.rtb1.Name = "rtb1";
			this.rtb1.Size = new Size(950, 167);
			this.rtb1.TabIndex = 0;
			this.rtb1.Text = "";
			this.bParse.Location = new Point(5, 4);
			this.bParse.Name = "bParse";
			this.bParse.Size = new Size(106, 23);
			this.bParse.TabIndex = 1;
			this.bParse.Text = "Parse (F5)";
			this.bParse.UseVisualStyleBackColor = true;
			this.bParse.Click += new EventHandler(this.bParse_Click);
			this.rtb2.BorderStyle = BorderStyle.None;
			this.rtb2.Dock = DockStyle.Fill;
			this.rtb2.Location = new Point(3, 213);
			this.rtb2.Name = "rtb2";
			this.rtb2.ReadOnly = true;
			this.rtb2.Size = new Size(950, 514);
			this.rtb2.TabIndex = 2;
			this.rtb2.Text = "";
			this.cbHide.AutoSize = true;
			this.cbHide.Checked = true;
			this.cbHide.CheckState = CheckState.Checked;
			this.cbHide.Location = new Point(117, 8);
			this.cbHide.Name = "cbHide";
			this.cbHide.Size = new Size(104, 17);
			this.cbHide.TabIndex = 3;
			this.cbHide.Text = "Hide Defaults";
			this.cbHide.UseVisualStyleBackColor = true;
			this.cbHide.CheckedChanged += new EventHandler(this.cbHide_CheckedChanged);
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
			this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.rtb2, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.rtb1, 0, 0);
			this.tableLayoutPanel1.Dock = DockStyle.Fill;
			this.tableLayoutPanel1.Location = new Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25f));
			this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 75f));
			this.tableLayoutPanel1.Size = new Size(956, 730);
			this.tableLayoutPanel1.TabIndex = 4;
			this.panel1.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.panel1.Controls.Add(this.bParse);
			this.panel1.Controls.Add(this.cbHide);
			this.panel1.Location = new Point(3, 176);
			this.panel1.Name = "panel1";
			this.panel1.Size = new Size(950, 31);
			this.panel1.TabIndex = 0;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(956, 730);
			base.Controls.Add(this.tableLayoutPanel1);
			this.Font = new Font("Consolas", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			base.Name = "Form1";
			this.Text = "Form1";
			this.tableLayoutPanel1.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			base.ResumeLayout(false);
		}
	}
	internal static class Program
	{
		[STAThread]
		private static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form1());
		}
	}
}
namespace TSqlParseInfo.Properties
{
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0"), DebuggerNonUserCode, CompilerGenerated]
	internal class Resources
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				var flag = Resources.resourceMan == null;
				if (flag)
				{
					var resourceManager = new ResourceManager("TSqlParseInfo.Properties.Resources", typeof(Resources).Assembly);
					Resources.resourceMan = resourceManager;
				}
				return Resources.resourceMan;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return Resources.resourceCulture;
			}
			set
			{
				Resources.resourceCulture = value;
			}
		}

		internal Resources()
		{
		}
	}
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "11.0.0.0"), CompilerGenerated]
	internal sealed class Settings : ApplicationSettingsBase
	{
		private static Settings defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());

		public static Settings Default
		{
			get
			{
				return Settings.defaultInstance;
			}
		}
	}
}
