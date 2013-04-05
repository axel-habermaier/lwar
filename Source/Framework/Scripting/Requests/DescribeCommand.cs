using System;

namespace Pegasus.Framework.Scripting.Requests
{
	using System.Text;

	/// <summary>
	///   A user request that instructs the system to show the affected command's help description.
	/// </summary>
	internal sealed class DescribeCommand : CommandRequest
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="command">The command that should be described.</param>
		public DescribeCommand(ICommand command)
			: base(command)
		{
		}

		/// <summary>
		///   Executes the user command.
		/// </summary>
		public override void Execute()
		{
			Log.Info(GetCommandDescription(Command));
		}

		/// <summary>
		/// Generates a description of the command.
		/// </summary>
		/// <param name="command">The command for which the description should be generated.</param>
		public static string GetCommandDescription(ICommand command)
		{
			var builder = new StringBuilder();

			builder.AppendFormat("\n'{0}': {1}\n", command.Name, command.Description);
			foreach (var parameter in command.Parameters)
			{
				var type = TypeDescription.GetDescription(parameter.Type);
				var defaultValue = String.Empty;
				if (parameter.HasDefaultValue)
					defaultValue = String.Format(" = {0}", TypeRepresentation.ToString(parameter.DefaultValue));

				builder.AppendFormat("    {0} : [{1}]{3}  {2}\n", parameter.Name, type, parameter.Description, defaultValue);
			}

			return builder.ToString();
		}
	}
}