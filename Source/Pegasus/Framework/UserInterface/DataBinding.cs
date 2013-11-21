namespace Pegasus.Framework.UserInterface
{
	using System;
	using System.Linq.Expressions;
	using System.Reflection;
	using Platform.Logging;

	/// <summary>
	///   Binds a target dependency object/dependency property pair to a source object and a path selector.
	/// </summary>
	/// <typeparam name="T">The type of the value that is bound.</typeparam>
	internal sealed class DataBinding<T> : Binding<T>
	{
		/// <summary>
		///   Cached array instance used to separate the properties of a path.
		/// </summary>
		private static readonly string[] PathSeparator = { "." };

		/// <summary>
		///   The property path that is evaluated on the source object to get the source value.
		/// </summary>
		private readonly string _path;

		/// <summary>
		///   The source object that is passed to the source expression in order to get the value that is set on the target
		///   property.
		/// </summary>
		private readonly object _sourceObject;

		/// <summary>
		///   If greater than 0, the properties accessed by the source expression are currently changing.
		/// </summary>
		private byte _isChanging;

		/// <summary>
		///   Indicates whether the currently bound value is null.
		/// </summary>
		private bool _isNull;

		/// <summary>
		///   Provides information about the first member access (such as 'a.b') in a source expression 'a.b.c.d'.
		/// </summary>
		/// <remarks>
		///   We do not use an array to store the member accesses, but rather use a hard-coded limit to avoid possibly numerous
		///   array allocations.
		/// </remarks>
		private MemberAccess _memberAccess1;

		/// <summary>
		///   Provides information about the second member access (such as 'b.c') in a source expression 'a.b.c.d'.
		/// </summary>
		private MemberAccess _memberAccess2;

		/// <summary>
		///   Provides information about the third member access (such as 'c.d') in a source expression 'a.b.c.d'.
		/// </summary>
		private MemberAccess _memberAccess3;

		/// <summary>
		///   The number of member accesses in the source expression.
		/// </summary>
		private byte _memberAccessCount;

		/// <summary>
		///   The compiled expression that is used to get the value from the source object.
		/// </summary>
		private Func<object, T> _sourceFunc;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="sourceObject">The source object that should provide the value that is bound.</param>
		/// <param name="path">The property path that should be evaluated on the source object to get the source value.</param>
		internal DataBinding(object sourceObject, string path)
		{
			Assert.ArgumentNotNullOrWhitespace(path);

			_sourceObject = sourceObject;
			_path = path;
		}

		/// <summary>
		///   Initializes the binding.
		/// </summary>
		protected override void Initialize()
		{
			Assert.ArgumentSatisfies(!_targetProperty.IsDataBindingProhibited, "Data binding is not allowed on the target property.");

			// Initialize the member access information
			var properties = _path.Split(PathSeparator, StringSplitOptions.RemoveEmptyEntries);

			_memberAccessCount = (byte)properties.Length;
			Assert.That(_memberAccessCount > 0, "The source expression does not access any members.");
			Assert.That(_memberAccessCount <= 3, "Unsupported number of member accesses.");

			_memberAccess1 = new MemberAccess(properties[0]) { Changed = OnMember1Changed };

			if (_memberAccessCount > 1)
				_memberAccess2 = new MemberAccess(properties[1]) { Changed = OnMember2Changed };

			if (_memberAccessCount > 2)
				_memberAccess3 = new MemberAccess(properties[2]) { Changed = OnMember3Changed };

			// Set the source object of the first member access
			_memberAccess1.SourceObject = _sourceObject;
		}

		/// <summary>
		///   Compiles the function to access the source value.
		/// </summary>
		private void CompileFunction()
		{
			var parameter = Expression.Parameter(typeof(object));
			var expression = Expression.Convert(parameter, _sourceObject.GetType()) as Expression;
			expression = _memberAccess1.GetAccessExpression(expression);

			if (_memberAccessCount > 1)
				expression = _memberAccess2.GetAccessExpression(expression);

			if (_memberAccessCount > 2)
				expression = _memberAccess3.GetAccessExpression(expression);

			expression = Expression.Convert(expression, typeof(T));
			_sourceFunc = Expression.Lambda<Func<object, T>>(expression, parameter).Compile();
			UpdateTargetProperty();
		}

		/// <summary>
		///   Invoked when the first accessed member changed.
		/// </summary>
		private void OnMember1Changed()
		{
			OnMemberChanged(ref _memberAccess1, ref _memberAccess2, 1);
		}

		/// <summary>
		///   Invoked when the second accessed member changed.
		/// </summary>
		private void OnMember2Changed()
		{
			OnMemberChanged(ref _memberAccess2, ref _memberAccess3, 2);
		}

		/// <summary>
		///   Invoked when the third accessed member changed.
		/// </summary>
		private void OnMember3Changed()
		{
			OnMemberChanged(ref _memberAccess3, ref _memberAccess3, Int32.MaxValue);
		}

		/// <summary>
		///   Handles a value change of an accessed member.
		/// </summary>
		/// <param name="memberAccess">The member that has been accessed.</param>
		/// <param name="nextMemberAccess">The next member access that must be updated.</param>
		/// <param name="memberAccessCount">
		///   The number of member accesses the source expression must contain for the next member
		///   access to be updated.
		/// </param>
		private void OnMemberChanged(ref MemberAccess memberAccess, ref MemberAccess nextMemberAccess, int memberAccessCount)
		{
			++_isChanging;

#if DEBUG
			if (!_isNull && memberAccess.Value == null)
				Log.Debug("Data binding failure: Encountered a null value in property path '{0}' when accessing '{1}'.",
						  _path, memberAccess.MemberName);
#endif

			var value = memberAccess.Value;
			_isNull = value == null;

			// If the value is not null and the type of a value somewhere in the middle of the path has changed,
			// we have to regenerate the source function
			if (memberAccessCount < _memberAccessCount && !_isNull)
			{
				var type = value.GetType();

				if (type != memberAccess.ValueType)
				{
					_sourceFunc = null;
					memberAccess.ValueType = type;
				}
			}

			if (_memberAccessCount > memberAccessCount)
				nextMemberAccess.SourceObject = value;

			--_isChanging;

			if (_isChanging == 0)
				UpdateTargetProperty();
		}

		/// <summary>
		///   Updates the target property with the current source value.
		/// </summary>
		private void UpdateTargetProperty()
		{
			if (_sourceFunc == null && !_isNull)
				CompileFunction();

			if (!_isNull)
				_targetObject.SetValue(_targetProperty, _sourceFunc(_sourceObject));
			else
				_targetObject.SetValue(_targetProperty, _targetProperty.DefaultValue);
		}

		/// <summary>
		///   Provides information about a member access in the source expression.
		/// </summary>
		private struct MemberAccess
		{
			/// <summary>
			///   The name of the accessed property.
			/// </summary>
			private readonly string _propertyName;

			/// <summary>
			///   The strongly-typed changed handler that has been added for the dependency property.
			/// </summary>
			private Delegate _changeHandler;

			/// <summary>
			///   The dependency property that is accessed, if any.
			/// </summary>
			private DependencyProperty _dependencyProperty;

			/// <summary>
			///   The reflection info instance for the property that is accessed, if any.
			/// </summary>
			private PropertyInfo _propertyInfo;

			/// <summary>
			///   The source object that is accessed.
			/// </summary>
			private object _sourceObject;

			/// <summary>
			///   Initializes a new instance.
			/// </summary>
			/// <param name="propertyName">The name of the property that should be accessed.</param>
			public MemberAccess(string propertyName)
				: this()
			{
				Assert.ArgumentNotNullOrWhitespace(propertyName);
				_propertyName = propertyName;
			}

			/// <summary>
			///   Gets or sets the type of the value currently stored by the accessed property.
			/// </summary>
			public Type ValueType { get; set; }

			/// <summary>
			///   Gets the name of the member that is accessed.
			/// </summary>
			public string MemberName
			{
				get { return _propertyName; }
			}

			/// <summary>
			///   Sets the change handler that is invoked when the value of the member has changed.
			/// </summary>
			public Action Changed { private get; set; }

			/// <summary>
			///   Sets the source object that is accessed.
			/// </summary>
			public object SourceObject
			{
				set
				{
					// Don't do unnecessary work; a value of null, however, must always be propagated
					if (_sourceObject == value && value != null)
						return;

					DetachFromChangeEvent();

					_sourceObject = value;
					_dependencyProperty = null;
					_propertyInfo = null;

					GetReflectedProperty();
					AttachToChangeEvent();

					if (Changed != null)
						Changed();
				}
			}

			/// <summary>
			///   Uses reflection to get the value of the accessed member.
			/// </summary>
			public object Value
			{
				get
				{
					if (_sourceObject == null)
						return null;

					if (_dependencyProperty == null)
						return _propertyInfo.GetValue(_sourceObject);

					var dependencyObject = _sourceObject as DependencyObject;
					Assert.NotNull(dependencyObject, "Trying to use a dependency property on a type not derived from DependencyObject.");

					return _dependencyProperty.GetValue(dependencyObject);
				}
			}

			/// <summary>
			///   Gets the access expression for the accessed property or dependency property.
			/// </summary>
			/// <param name="expression">The expression that defines the property that should be accessed.</param>
			public Expression GetAccessExpression(Expression expression)
			{
				return Expression.Convert(Expression.Property(expression, _propertyInfo), Value.GetType());
			}

			/// <summary>
			///   Attaches the instance to the source object's property changed event.
			/// </summary>
			private void AttachToChangeEvent()
			{
				if (_sourceObject == null)
					return;

				if (_dependencyProperty != null)
				{
					var dependencyObject = _sourceObject as DependencyObject;
					Assert.NotNull(dependencyObject, "Trying to use a dependency property on a type not derived from DependencyObject.");

					_changeHandler = _dependencyProperty.AddUntypedChangeHandler(dependencyObject, DependencyPropertyChanged);
				}
				else
				{
					var notifyPropertyChanged = _sourceObject as INotifyPropertyChanged;
					if (notifyPropertyChanged == null)
						return;

					ReflectionHelper.AttachPropertyChangedEventHandler(notifyPropertyChanged, PropertyChanged);
				}
			}

			/// <summary>
			///   Detaches the instance from the source object's property changed event.
			/// </summary>
			private void DetachFromChangeEvent()
			{
				if (_sourceObject == null)
					return;

				if (_dependencyProperty != null)
				{
					var dependencyObject = _sourceObject as DependencyObject;
					Assert.NotNull(dependencyObject, "Trying to use a dependency property on a type not derived from DependencyObject.");

					_dependencyProperty.RemoveUntypedChangeHandler(dependencyObject, _changeHandler);
				}
				else
				{
					var notifyPropertyChanged = _sourceObject as INotifyPropertyChanged;
					if (notifyPropertyChanged == null)
						return;

					ReflectionHelper.DetachPropertyChangedEventHandler(notifyPropertyChanged, PropertyChanged);
				}
			}

			/// <summary>
			///   Gets the reflection info of the property or the instance of the dependency property that is accessed.
			/// </summary>
			private void GetReflectedProperty()
			{
				if (_sourceObject == null)
					return;

				if (_sourceObject is DependencyObject)
					_dependencyProperty = ReflectionHelper.GetDependencyProperty(_sourceObject.GetType(), _propertyName);

				_propertyInfo = _sourceObject.GetType().GetProperty(_propertyName, BindingFlags.Public | BindingFlags.Instance);
				if (_propertyInfo == null)
					Log.Die("Unable to find public, non-static property or dependency property '{0}' on '{1}'.",
							_propertyName, _sourceObject.GetType().FullName);
			}

			/// <summary>
			///   Handles a change notification for a property.
			/// </summary>
			/// <param name="obj">The object the changed property belongs to.</param>
			/// <param name="property">The name of the property that has changed.</param>
			private void PropertyChanged(object obj, string property)
			{
				Assert.That(obj == _sourceObject, "Received an unexpected property change notification.");
				Assert.That(_dependencyProperty == null, "Received an unexpected property change notification.");

				if (property == _propertyInfo.Name && Changed != null)
					Changed();
			}

			/// <summary>
			///   Handles a change notification for a dependency property.
			/// </summary>
			/// <param name="obj">The dependency object the changed dependency property belongs to.</param>
			/// <param name="property">The dependency property that has changed.</param>
			private void DependencyPropertyChanged(DependencyObject obj, DependencyProperty property)
			{
				Assert.That(obj == _sourceObject, "Received an unexpected dependency property change notification.");
				Assert.NotNull(_dependencyProperty, "Received un unexpected dependency property change notification.");

				if (property == _dependencyProperty && Changed != null)
					Changed();
			}
		}
	}
}