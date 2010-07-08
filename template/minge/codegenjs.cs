
using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Cecil.Cil;


namespace Mango.Templates.Minge {

	public enum CompareOperator {
		Invalid,

		Is,
		Equal,
		NotEqual,
		GreaterThan,
		LessThan,
		GreaterThanOrEqual,
		LessThanOrEqual,
	}

	public class Target {

	}

	public class NamedTarget : Target {

		public NamedTarget (string name)
		{
			Name = name;
		}

		public string Name {
			get;
			private set;
		}
	}

	public abstract class Value {

		public abstract void Emit (Application app, Page page, TextWriter writer);
	}

	public class VariableValue : Value {

		public VariableValue (NamedTarget name)
		{
			Name = name;
		}

		public NamedTarget Name {
			get;
			private set;
		}

		public override void Emit (Application app, Page page, TextWriter writer)
		{
			//
			// It could be: A local variable, a supplied parameter,
			// a property of the class, or it could be a property
			// of the supplied object
			// 

			writer.Write (Name.Name);
		}
	}

	public abstract class ConstantValue : Value {

		public abstract object GetValue ();
	}

	public class ConstantStringValue : ConstantValue {

		public ConstantStringValue (string value)
		{
			Value = value;
			
		}

		public string Value {
			get;
			private set;
		}

		public override object GetValue ()
		{
			return Value;
		}

		public override void Emit (Application app, Page page, TextWriter writer)
		{
			writer.Write ("'");
			writer.Write (Value);
			writer.Write ("'");
		}
	}

	public class ConstantIntValue : ConstantValue {

		public ConstantIntValue (int value)
		{
			Value = value;
		}

		public int Value {
			get;
			private set;
		}

		public override object GetValue ()
		{
			return Value;
		}

		public override void Emit (Application app, Page page, TextWriter writer)
		{
			writer.Write (Value);
		}
	}

	public class ConstantDoubleValue : ConstantValue {

		public ConstantDoubleValue (double value)
		{
			Value = value;
		}

		public double Value {
			get;
			private set;
		}

		public override object GetValue ()
		{
			return Value;
		}

		public override void Emit (Application app, Page page, TextWriter writer)
		{
			writer.Write (Value);
		}
	}

	public class PropertyAccessValue : Value {

		public PropertyAccessValue (Value target, string property)
		{
			Target = target;
			Property = property;
		}

		public Value Target {
			get;
			private set;
		}

		public string Property {
			get;
			private set;
		}

		public override void Emit (Application app, Page page, TextWriter writer)
		{
			Target.Emit (app, page, worker);

			writer.Write ("['");
			writer.Write (Property);
			writer.Write ("]");
		}

	}

	public abstract class Callable : Value {

		public string Name {
			get;
			protected set;
		}

		public List<Expression> Arguments {
			get;
			protected set;
		}

		protected void EmitArguments (TextWriter writer)
		{
			writer.Write ("(");
			for (int i = 0; i < Arguments.Count; i++) {
				Arguments [i].Emit (app, page, writer);
				if (i + 1 < Arguments.Count)
					writer.Write (",");
			}
			writer.Write (")");
		}
	}

	public class InvokeValue : Callable {

		public InvokeValue (string name, List<Expression> args)
		{
			Name = name;
			Arguments = args;
		}

		public override void Emit (Application app, Page page, TextWriter writer)
		{
			writer.Write (Name);

			EmitArguments ();
		}
	}

	public class Filter : Callable {

		public Filter (string name, List<Expression> args)
		{
			Name = name;
			Arguments = args;
		}

		public override void Emit (Application app, Page page, TextWriter writer)
		{
			var filter = MingeFilterManager.GetFilter (Name);

			if (filter == null)
				return;

			writer.Write ("filter_");
			writer.Write (Name);

			EmitArguments (writer);
		}
	}

	public class Expression {

		private List<Filter> filters = new List<Filter> ();

		public Expression (Value value)
		{
			Value = value;
		}

		public Value Value {
			get;
			private set;
		}

		public virtual void Emit (Application app, Page page, TextWriter writer)
		{
			Value.Emit (app, page, writer);

			foreach (Filter filter in filters) {
				filter.Emit (app, page, writer);
			}
		}

		public void AddFilter (Filter filter)
		{
			filters.Add (filter);
		}
	}

	public class ArgumentDefinition {

		public ArgumentDefinition (string name, ConstantValue default_value)
		{
			Name = name;
			DefaultValue = default_value;
		}

		public string Name {
			get;
			private set;
		}

		public ConstantValue DefaultValue {
			get;
			private set;
		}
	}

	public class Application {

		public Application (MingeCompiler compiler, string name, string path)
		{
			Compiler = compiler;
			Name = name;
			Path = path;
		}

		public string Name {
			get;
			private set;
		}

		public string Path {
			get;
			private set;
		}

		public MingeCompiler Compiler {
			get;
			private set;
		}

		public void Save ()
		{
		}

		public Page CreatePage (string path)
		{
			string ns;
			string name = Page.NameForPath (path, out ns);

			return new Page (this, name);
		}

		public Page LoadPage (string path)
		{
			string ns;
			string name = Page.NameForPath (path, out ns);

			Page page = Compiler.ParsePage (path);
			return page;
		}

	}

	public class Page {

		private Page base_type;

		private Stack<MethodDefinition> method_stack;
		private Stack<ForLoopContext> forloop_stack;
		
		public Page (string name)
		{
			this.application = application;
			this.assembly = assembly;

			Definition = definition;

			MethodDefinition ctor = new MethodDefinition (".ctor", MethodAttributes.Public, assembly.MainModule.Import (typeof (void)));
			Definition.Methods.Add (ctor);
			ctor.Body.CilWorker.Emit (OpCodes.Ret);

			ValueToStringMethod = AddValueToStringMethod ();

			render = AddRenderMethod ("RenderToStream");
			first_instruction = render.Body.CilWorker.Emit (OpCodes.Nop);

			method_stack = new Stack<MethodDefinition> ();
			method_stack.Push (render);

			forloop_stack = new Stack<ForLoopContext> ();
		}

		public TypeDefinition Definition {
			get;
			private set;
		}

		public MethodDefinition ValueToStringMethod {
			get;
			private set;
		}

		public MethodDefinition CurrentMethod {
			get {
				return method_stack.Peek ();
			}
		}

		public bool IsChildTemplate {
			get {
				return base_type != null && method_stack.Count == 1;
			}
		}

		public bool InForLoop {
			get {
				return forloop_stack.Count > 0;
			}
		}

		public void AddData (string data)
		{
			if (IsChildTemplate)
				return;

			CilWorker worker = CurrentMethod.Body.CilWorker;

			worker.Emit (OpCodes.Ldarg_1);
			worker.Emit (OpCodes.Ldstr, data);
			worker.Emit (OpCodes.Callvirt, application.CommonMethods.WriteStringMethod);
		}

		public void EmitExtends (string base_template)
		{
			if (base_type != null)
				throw new Exception (String.Format ("Multiple extends statements are not allowed. ({0})", base_type));

			base_type = application.LoadPage (base_template);

			if (base_type == null)
				throw new Exception ("Could not find base.");

			Definition.BaseType = base_type.Definition;
			EmitBaseRenderToStreamCall ();
		}

		private void EmitBaseRenderToStreamCall ()
		{
			CilWorker worker = method_stack.Last ().Body.CilWorker;

			MethodReference base_render = base_type.Definition.Methods.GetMethod ("RenderToStream") [0];

			worker.InsertAfter (first_instruction, worker.Create (OpCodes.Ret));
			worker.InsertAfter (first_instruction, worker.Create (OpCodes.Call, base_render));
			worker.InsertAfter (first_instruction, worker.Create (OpCodes.Ldarg_2));
			worker.InsertAfter (first_instruction, worker.Create (OpCodes.Ldarg_1));
			worker.InsertAfter (first_instruction, worker.Create (OpCodes.Ldarg_0));
		}

		public void BeginBlock (string name)
		{
			MethodDefinition meth = GetMethod (name);

			if (meth != null)
				throw new Exception (String.Format ("Invalid block name {0} the name is already in use.", name));

			meth = AddRenderMethod (name);

			CilWorker worker = CurrentMethod.Body.CilWorker;
			worker.Emit (OpCodes.Ldarg_0);
			worker.Emit (OpCodes.Ldarg_1);
			worker.Emit (OpCodes.Ldarg_2);
			Instruction block_call = worker.Emit (OpCodes.Callvirt, meth);

			method_stack.Push (meth);
		}

		public void EndBlock (string name)
		{
			if (name != null && CurrentMethod.Name != name)
				throw new Exception (String.Format ("Unmatched block names, expected {0} but got {1}", CurrentMethod.Name, name));

			CilWorker worker = CurrentMethod.Body.CilWorker;
			worker.Emit (OpCodes.Ret);

			method_stack.Pop ();
		}

		public void EmitPrint (List<Expression> expressions)
		{
			if (IsChildTemplate)
				return;

			if (expressions.Count == 1) {
				EmitSinglePrint (expressions [0]);
				return;
			}

			CilWorker worker = CurrentMethod.Body.CilWorker;

			StringBuilder format_str = new StringBuilder ();
			for (int i = 0; i < expressions.Count; i++) {
				format_str.AppendFormat ("{{0}} ", i);
			}

			worker.Emit (OpCodes.Ldarg_1);
			worker.Emit (OpCodes.Ldstr, format_str.ToString ());

			worker.Emit (OpCodes.Ldc_I4, expressions.Count);
			worker.Emit (OpCodes.Newarr, assembly.MainModule.Import (typeof (object)));

			for (int i = 0; i < expressions.Count; i++) {
				worker.Emit (OpCodes.Dup);
				worker.Emit (OpCodes.Ldc_I4, i);
				expressions [i].Emit (application, this, worker);
				EmitToString (application, this, worker, expressions [i].ResolvedType);
				worker.Emit (OpCodes.Stelem_Ref);
			}
			worker.Emit (OpCodes.Call, application.CommonMethods.FormatStringMethod);
			worker.Emit (OpCodes.Callvirt, application.CommonMethods.WriteStringMethod);
		}

		public void EmitToString (Application app, Page page, CilWorker worker, TypeReference resolved)
		{
			if (resolved.FullName == "System.String")
				return;

			if (resolved.FullName == "System.Void") {
				worker.Emit (OpCodes.Ldsfld, application.CommonMethods.StringEmptyField);
				return;
			}

			if (resolved.IsValueType) {
				worker.Emit (OpCodes.Box, app.Assembly.MainModule.Import (resolved));
				worker.Emit (OpCodes.Call, app.Assembly.MainModule.Import (page.ValueToStringMethod));
				return;
			}

			TypeDefinition rtype = resolved.Resolve ();
			MethodReference method = rtype.Methods.GetMethod ("ToString", new TypeReference [0]);

			// Import it so we get a method reference
			method = application.Assembly.MainModule.Import (method);
			Instruction inst = worker.Emit (OpCodes.Callvirt, (MethodReference) method);
		}

		public void EmitSinglePrint (Expression expression)
		{
			if (IsChildTemplate)
				return;

			CilWorker worker = CurrentMethod.Body.CilWorker;

			worker.Emit (OpCodes.Ldarg_1);
			expression.Emit (application, this, worker);
			EmitToString (application, this, worker, expression.ResolvedType);
			worker.Emit (OpCodes.Callvirt, application.CommonMethods.WriteStringMethod);
		}

		public void EmitSet (NamedTarget target, Expression expression)
		{
			CilWorker worker = CurrentMethod.Body.CilWorker;

			//
			// For now lets make them all fields
			//

			FieldDefinition field = FindField (target.Name);
			if (field == null)
				field = AddField (target.Name);

			worker.Emit (OpCodes.Ldarg_0);
			expression.Emit (application, this, worker);
			worker.Emit (OpCodes.Stfld, field);
		}

		public void BeginMacro (string name, List<ArgumentDefinition> args)
		{
			MethodDefinition meth = AddRenderMethod (name, args);

			method_stack.Push (meth);
		}

		public void EndMacro (string name)
		{
			if (name != null && CurrentMethod.Name != name)
				throw new Exception (String.Format ("Unmatched macro names, expected {0} but got {1}", CurrentMethod.Name, name));

			CilWorker worker = CurrentMethod.Body.CilWorker;
			worker.Emit (OpCodes.Ret);

			method_stack.Pop ();
		}

		private class IfContext {

			public IfContext ()
			{
				NextConditionalBranches = new List<Instruction> ();
				EndIfBranches = new List<Instruction> ();
			}

			public void UpdateNextConditionalBranches (Instruction new_target)
			{
				foreach (Instruction inst in NextConditionalBranches) {
					inst.Operand = new_target;
				}
				NextConditionalBranches.Clear ();
			}

			public void UpdateEndIfBranches (Instruction new_target)
			{
				foreach (Instruction inst in EndIfBranches) {
					inst.Operand = new_target;
				}
				EndIfBranches.Clear ();
			}

			public List<Instruction> NextConditionalBranches {
				get;
				private set;
			}

			public List<Instruction> EndIfBranches {
				get;
				private set;
			}
		}

		private Stack<IfContext> ifstack = new Stack<IfContext> ();

		public void EmitIf (Expression expression)
		{
			if (IsChildTemplate)
				return;

			TypeReference string_type = application.Assembly.MainModule.Import (typeof (string));

			CilWorker worker = CurrentMethod.Body.CilWorker;
			expression.Emit (application, this, worker);
			Instruction null_branch = worker.Emit (OpCodes.Brfalse, worker.Create (OpCodes.Nop));

			expression.Emit (application, this, worker);
			worker.Emit (OpCodes.Isinst, string_type);
			Instruction isstr_branch = worker.Emit (OpCodes.Brfalse, worker.Create (OpCodes.Nop));

			expression.Emit (application, this, worker);
			worker.Emit (OpCodes.Castclass, string_type);
			worker.Emit (OpCodes.Call, application.CommonMethods.IsNullOrEmptyMethod);
			Instruction empty_branch = worker.Emit (OpCodes.Brtrue, worker.Create (OpCodes.Nop));

			isstr_branch.Operand = worker.Emit (OpCodes.Nop);

			IfContext ifcontext = new IfContext ();
			ifcontext.NextConditionalBranches.Add (null_branch);
			ifcontext.NextConditionalBranches.Add (empty_branch);

			ifstack.Push (ifcontext);
		}

		public void EmitElseIf (Expression expression)
		{
			if (IsChildTemplate)
				return;
		}

		public void EmitElse ()
		{
			if (IsChildTemplate)
				return;

			IfContext ifcontext = ifstack.Peek ();

			CilWorker worker = CurrentMethod.Body.CilWorker;

			Instruction branch_to_end = worker.Emit (OpCodes.Br, worker.Create (OpCodes.Nop));
			Instruction begin_else = worker.Emit (OpCodes.Nop);

			ifcontext.UpdateNextConditionalBranches (begin_else);
			ifcontext.EndIfBranches.Add (branch_to_end);
		}

		public void EmitEndIf ()
		{
			if (IsChildTemplate)
				return;

			IfContext ifcontext = ifstack.Pop ();

			CilWorker worker = CurrentMethod.Body.CilWorker;

			Instruction end_branch = worker.Emit (OpCodes.Nop);
			ifcontext.UpdateNextConditionalBranches (end_branch);
			ifcontext.UpdateEndIfBranches (end_branch);
		}

		private class ForLoopContext {

			public ForLoopContext (string varname, Instruction begin_loop, VariableDefinition enumvar)
			{
				VariableName = varname;
				BeginLoopInstruction = begin_loop;
				EnumeratorVariable = enumvar;
			}

			public string VariableName {
				get;
				private set;
			}

			public Instruction BeginLoopInstruction {
				get;
				private set;
			}

			public VariableDefinition EnumeratorVariable {
				get;
				private set;
			}
		}

		public void BeginForLoop (string name, Expression expression)
		{
			if (IsChildTemplate)
				return;

			TypeReference enum_type = assembly.MainModule.Import (typeof (System.Collections.IEnumerable));

			string local_enum_name = String.Format ("__enum_{0}", forloop_stack.Count);
			VariableDefinition enumeratorvar = FindLocalVariable (local_enum_name);
			if (enumeratorvar == null) {
				VariableDefinitionCollection vars = CurrentMethod.Body.Variables;
				enumeratorvar = new VariableDefinition (local_enum_name, vars.Count, CurrentMethod, enum_type);
				vars.Add (enumeratorvar);
			}

			CilWorker worker = CurrentMethod.Body.CilWorker;
			expression.Emit (application, this, worker);
			worker.Emit (OpCodes.Castclass, assembly.MainModule.Import (typeof (System.Collections.IEnumerable)));
			worker.Emit (OpCodes.Callvirt, application.CommonMethods.GetEnumeratorMethod);
			
			worker.Emit (OpCodes.Stloc, enumeratorvar);

			Instruction begin_loop = worker.Emit (OpCodes.Br, worker.Create (OpCodes.Nop));
			forloop_stack.Push (new ForLoopContext (name, begin_loop, enumeratorvar));

			worker.Emit (OpCodes.Nop);
		}

		public void EndForLoop ()
		{
			if (IsChildTemplate)
				return;

			ForLoopContext forloop = forloop_stack.Pop ();

			CilWorker worker = CurrentMethod.Body.CilWorker;

			Instruction enter_loop = worker.Emit (OpCodes.Ldloc, forloop.EnumeratorVariable);
			worker.Emit (OpCodes.Callvirt, application.CommonMethods.EnumeratorMoveNextMethod);
			worker.Emit (OpCodes.Brtrue, forloop.BeginLoopInstruction.Next);

			forloop.BeginLoopInstruction.Operand = enter_loop;
		}

		public void Save ()
		{
		       CilWorker worker = CurrentMethod.Body.CilWorker;

		       worker.Emit (OpCodes.Ret);
		}

		public bool IsForLoopVariable (string name)
		{
			if (forloop_stack.Count == 0)
				return false;

			ForLoopContext forloop = forloop_stack.Peek ();

			return name == forloop.VariableName;
		}

		public void EmitForLoopVariableAccess ()
		{
			CilWorker worker = CurrentMethod.Body.CilWorker;

			ForLoopContext forloop = forloop_stack.Peek ();
			worker.Emit (OpCodes.Ldloc, forloop.EnumeratorVariable);
			worker.Emit (OpCodes.Callvirt, application.CommonMethods.EnumeratorGetCurrentMethod);
		}

		public ParameterDefinition FindParameter (string name)
		{
			if (!CurrentMethod.HasParameters)
				return null;

			foreach (ParameterDefinition p in CurrentMethod.Parameters) {
				if (p.Name == name)
					return p;
			}

			return null;
		}

		public VariableDefinition FindLocalVariable (string name)
		{
			if (!CurrentMethod.Body.HasVariables)
				return null;
			return FindVariable (name, CurrentMethod.Body.Variables);
		}

		private VariableDefinition FindVariable (string name, VariableDefinitionCollection variables)
		{
			foreach (VariableDefinition variable in variables) {
				if (variable.Name == name)
					return variable;
			}

			return null;
		}

		public FieldDefinition FindField (string name)
		{
			if (!Definition.HasFields)
				return null;
			return Definition.Fields.GetField (name);
		}

		public FieldDefinition AddField (string name)
		{
			FieldDefinition field = new FieldDefinition (name, assembly.MainModule.Import (typeof (object)), FieldAttributes.Public);
			Definition.Fields.Add (field);

			return field;
		}

		public MethodDefinition GetMethod (string name)
		{
			MethodDefinition [] methods = Definition.Methods.GetMethod (name);

			if (methods.Length < 1)
				return null;

			return methods [0];
		}

		public MethodDefinition AddRenderMethod (string name, List<ArgumentDefinition> extra_args=null)
		{
			MethodAttributes atts = MethodAttributes.Public | MethodAttributes.Virtual;

			MethodDefinition render = new MethodDefinition (name, atts, assembly.MainModule.Import (typeof (void)));

			render.Parameters.Add (new ParameterDefinition ("stream", -1, (ParameterAttributes) 0,
					assembly.MainModule.Import (typeof (TextWriter))));
			render.Parameters.Add (new ParameterDefinition ("args", -1, (ParameterAttributes) 0,
					assembly.MainModule.Import (typeof (object))));

			if (extra_args != null) {
				TypeReference object_type = assembly.MainModule.Import (typeof (object));
				foreach (ArgumentDefinition arg in extra_args) {
					ParameterDefinition pdef = new ParameterDefinition (arg.Name, -1, (ParameterAttributes) 0, object_type);
					if (arg.DefaultValue != null) {
						pdef.Constant = arg.DefaultValue.GetValue ();
						pdef.HasDefault = true;
					}
					render.Parameters.Add (pdef);
				}
			}

			Definition.Methods.Add (render);

			return render;
		}

		public MethodDefinition AddValueToStringMethod ()
		{
			MethodAttributes atts = MethodAttributes.Public | MethodAttributes.Static;

			MethodDefinition to_string = new MethodDefinition ("ValueToString", atts, assembly.MainModule.Import (typeof (string)));
			to_string.Parameters.Add (new ParameterDefinition ("the_value", -1, (ParameterAttributes) 0, assembly.MainModule.Import (typeof (System.ValueType))));

			Definition.Methods.Add (to_string);

			CilWorker worker = to_string.Body.CilWorker;
			worker.Emit (OpCodes.Ldarg_0);
			worker.Emit (OpCodes.Callvirt, assembly.MainModule.Import (typeof (System.ValueType).GetMethod ("ToString")));
			worker.Emit (OpCodes.Ret);
					
			return to_string;
		}

		public static string NameForPath (string path, out string name_space)
		{
			string [] pieces = path.Split (System.IO.Path.DirectorySeparatorChar);
			StringBuilder ns = new StringBuilder ("templates");

			string name = String.Concat ("page_", System.IO.Path.GetFileNameWithoutExtension (pieces [pieces.Length - 1]));
			for (int i = 0; i < pieces.Length - 1; i++) {
				ns.Append (".");
				ns.Append (pieces [0]);
			}

			name_space = ns.ToString ();
			return name;
		}

		public static string FullNameForPath (string path)
		{
			string ns;
			string name = NameForPath (path, out ns);

			return String.Concat (ns, ".", name);
		}
	}
}

