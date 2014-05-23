﻿namespace Pegasus.Framework.UserInterface
{
	using System;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using Converters;
	using Platform.Logging;

	/// <summary>
	///     Binds a target dependency object/dependency property pair to a source object and a property path.
	/// </summary>
	/// <typeparam name="T">The type of the value that is bound.</typeparam>
	/// <remarks>With the current implementation of the data binding, a property path can only access up to three properties.</remarks>
	internal sealed class DataBinding<T> : Binding<T>
	{
		/// <summary>
		///     Indicates the direction of the data flow of the data binding.
		/// </summary>
		private readonly BindingMode _bindingMode;

		/// <summary>
		///     The converter that is used to convert the source value to the dependency property type.
		/// </summary>
		private readonly IValueConverter _converter;

		/// <summary>
		///     The number of member accesses in the source expression.
		/// </summary>
		private readonly byte _memberAccessCount;

		/// <summary>
		///     The source object that is passed to the source expression in order to get the value that is set on the target
		///     property.
		/// </summary>
		private readonly object _sourceObject;

		/// <summary>
		///     Indicates whether the currently bound value is null.
		/// </summary>
		private bool _isNull;

		/// <summary>
		///     Provides information about the first member access (such as 'a.b') in a property path 'a.b.c.d'.
		/// </summary>
		private MemberAccess _memberAccess1;

		/// <summary>
		///     Provides information about the second member access (such as 'b.c') in a property path 'a.b.c.d'.
		/// </summary>
		private MemberAccess _memberAccess2;

		/// <summary>
		///     Provides information about the third member access (such as 'c.d') in a property path 'a.b.c.d'.
		/// </summary>
		private MemberAccess _memberAccess3;

		/// <summary>
		///     The compiled expression that is used to get the value from the source.
		/// </summary>
		private Func<object, IValueConverter, T> _sourceFunc;

		/// <summary>
		///     The compiled expression that is used to set the value of the source.
		/// </summary>
		private Action<object, T, IValueConverter> _targetFunc;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="sourceObject">The source object that should provide the value that is bound.</param>
		/// <param name="bindingMode">Indicates the direction of the data flow of the data binding.</param>
		/// <param name="property1">The name of the first property in the property path.</param>
		/// <param name="property2">The name of the second property in the property path.</param>
		/// <param name="property3">The name of the third property in the property path.</param>
		/// <param name="converter">The converter that should be used to convert the source value to the dependency property type.</param>
		internal DataBinding(object sourceObject, BindingMode bindingMode,
							 string property1, string property2 = null, string property3 = null,
							 IValueConverter converter = null)
		{
			Assert.ArgumentNotNull(sourceObject);
			Assert.ArgumentInRange(bindingMode);
			Assert.ArgumentNotNullOrWhitespace(property1);

			_sourceObject = sourceObject;
			_converter = converter;
			_bindingMode = bindingMode;

			_memberAccessCount = 1;
			_memberAccess1 = new MemberAccess(property1) { Changed = OnMember1Changed };

			if (!String.IsNullOrWhiteSpace(property2))
			{
				_memberAccessCount = 2;
				_memberAccess2 = new MemberAccess(property2) { Changed = OnMember2Changed };
			}

			if (!String.IsNullOrWhiteSpace(property3))
			{
				_memberAccessCount = 3;
				_memberAccess3 = new MemberAccess(property3) { Changed = OnMember3Changed };
			}
		}

		/// <summary>
		///     Gets the property path that is bound to the dependency property.
		/// </summary>
		private string PropertyPath
		{
			get
			{
				if (_memberAccessCount == 3)
					return String.Format("{0}.{1}.{2}", _memberAccess1.MemberName, _memberAccess2.MemberName, _memberAccess3.MemberName);

				if (_memberAccessCount == 2)
					return String.Format("{0}.{1}", _memberAccess1.MemberName, _memberAccess2.MemberName);

				return _memberAccess1.MemberName;
			}
		}

		/// <summary>
		///     Invoked when the binding has been activated.
		/// </summary>
		protected override void OnActivated()
		{
			UpdateTargetProperty();
		}

		/// <summary>
		///     Initializes the binding.
		/// </summary>
		protected override void Initialize()
		{
			Assert.ArgumentSatisfies(!_targetProperty.IsDataBindingProhibited, "Data binding is not allowed on the target property.");

			// Set the source object of the first member access
			_memberAccess1.SourceObject = _sourceObject;

			if (_bindingMode != BindingMode.OneWay)
				_targetObject.AddChangedHandler(_targetProperty, OnTargetPropertyChanged);
		}

		/// <summary>
		///     Removes the binding. The binding can decide to ignore the removal if it would be overwritten by a local value. True is
		///     returned to indicate that the binding was removed.
		/// </summary>
		/// <param name="overwrittenByLocalValue">Indicates whether the binding is removed because it was overriden by a local value.</param>
		internal override bool Remove(bool overwrittenByLocalValue = false)
		{
			if (overwrittenByLocalValue && _bindingMode != BindingMode.OneWay)
				return false;

			_memberAccess1.Remove();
			_memberAccess2.Remove();
			_memberAccess3.Remove();

			if (_bindingMode != BindingMode.OneWay)
				_targetObject.RemoveChangedHandler(_targetProperty, OnTargetPropertyChanged);

			return true;
		}

		/// <summary>
		///     Compiles the function that is used to get the source value.
		/// </summary>
		private void CompileSourceFunction()
		{
			var expression = Expressions.GetMemberAccessExpression(this);

			if (_converter != null)
				expression = Expressions.InvokeConvertToTargetMethod(this, expression);

			_sourceFunc = Expression.Lambda<Func<object, IValueConverter, T>>(
				expression, Expressions.SourceObjectParameter, Expressions.ConverterParameter).Compile();
		}

		/// <summary>
		///     Compiles the function that is used to set the source value.
		/// </summary>
		private void CompileTargetFunction()
		{
			var expression = Expressions.GetMemberAccessExpression(this);

			Expression value = Expressions.ValueParameter;
			if (_converter != null)
				value = Expressions.InvokeConvertToSourceMethod(this);

			var convertExpression = (UnaryExpression)expression;
			var assignment = Expression.Assign(convertExpression.Operand, value);
			_targetFunc = Expression.Lambda<Action<object, T, IValueConverter>>(
				assignment, Expressions.SourceObjectParameter, Expressions.ValueParameter, Expressions.ConverterParameter).Compile();
		}

		/// <summary>
		///     Invoked when the first accessed member changed.
		/// </summary>
		private void OnMember1Changed()
		{
			OnMemberChanged(ref _memberAccess1, ref _memberAccess2, 1);
		}

		/// <summary>
		///     Invoked when the second accessed member changed.
		/// </summary>
		private void OnMember2Changed()
		{
			OnMemberChanged(ref _memberAccess2, ref _memberAccess3, 2);
		}

		/// <summary>
		///     Invoked when the third accessed member changed.
		/// </summary>
		private void OnMember3Changed()
		{
			OnMemberChanged(ref _memberAccess3, ref _memberAccess3, Int32.MaxValue);
		}

		/// <summary>
		///     Invoked when the target value changed. Updates the source property with the new target value.
		/// </summary>
		private void OnTargetPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs<T> args)
		{
			UpdateSourceProperty();
		}

		/// <summary>
		///     Handles a value change of an accessed member.
		/// </summary>
		/// <param name="memberAccess">The member that has been accessed.</param>
		/// <param name="nextMemberAccess">The next member access that must be updated.</param>
		/// <param name="memberAccessCount">
		///     The number of member accesses the source expression must contain for the next member
		///     access to be updated.
		/// </param>
		private void OnMemberChanged(ref MemberAccess memberAccess, ref MemberAccess nextMemberAccess, int memberAccessCount)
		{
			Log.DebugIf(Active && !_isNull && memberAccess.Value == null,
						"Data binding failure: Encountered a null value in property path '{0}' when accessing '{1}'.",
						PropertyPath, memberAccess.MemberName);

			if (memberAccessCount < _memberAccessCount)
			{
				var value = memberAccess.Value;
				_isNull = value == null;

				// If the value is not null and the type of a value somewhere in the middle of the path has changed,
				// we have to regenerate the source function
				if (!_isNull)
				{
					var type = value.GetType();

					if (type != memberAccess.ValueType)
					{
						_sourceFunc = null;
						_targetFunc = null;
						memberAccess.ValueType = type;
					}
				}

				nextMemberAccess.SourceObject = value;
			}
			else
			{
				var propertyType = memberAccess.PropertyType;
				if (propertyType != null && !propertyType.IsValueType)
					_isNull = memberAccess.Value == null;

				UpdateTargetProperty();
				UpdateSourceProperty();
			}
		}

		/// <summary>
		///     Updates the target property with the current source value.
		/// </summary>
		private void UpdateTargetProperty()
		{
			if (!Active || _bindingMode == BindingMode.OneWayToSource)
				return;

			if (_sourceFunc == null && !_isNull)
				CompileSourceFunction();

			if (!_isNull)
				_targetObject.SetBoundValue(_targetProperty, _sourceFunc(_sourceObject, _converter));
			else
				_targetObject.SetBoundValue(_targetProperty, _targetProperty.DefaultValue);
		}

		/// <summary>
		///     Updates the source property with the current target value.
		/// </summary>
		private void UpdateSourceProperty()
		{
			if (!Active || _bindingMode == BindingMode.OneWay)
				return;

			if (_targetFunc == null)
				CompileTargetFunction();

			_targetFunc(_sourceObject, _targetObject.GetValue(_targetProperty), _converter);
		}

		/// <summary>
		///     Helper for the compilation of the source and target functions.
		/// </summary>
		private static class Expressions
		{
			/// <summary>
			///     Represents the source object parameter of the functions.
			/// </summary>
			public static readonly ParameterExpression SourceObjectParameter = Expression.Parameter(typeof(object));

			/// <summary>
			///     Represents the converter object parameter of the functions.
			/// </summary>
			public static readonly ParameterExpression ConverterParameter = Expression.Parameter(typeof(IValueConverter));

			/// <summary>
			///     Represents the value parameter of the functions.
			/// </summary>
			public static readonly ParameterExpression ValueParameter = Expression.Parameter(typeof(T));

			/// <summary>
			///     Invokes the converter method with the given name.
			/// </summary>
			/// <param name="binding">The binding the expression is created for.</param>
			/// <param name="methodName">The name of the converter method that should be invoked.</param>
			/// <param name="methodParameter">The parameter for the converter method.</param>
			private static Expression InvokeConverterMethod(DataBinding<T> binding, string methodName, Expression methodParameter)
			{
				var converterType = binding._converter.GetType();
				var castConverter = Expression.Convert(ConverterParameter, converterType);
				return Expression.Call(castConverter, converterType.GetMethod(methodName), methodParameter);
			}

			/// <summary>
			///     Invokes the to source conversion function.
			/// </summary>
			/// <param name="binding">The binding the expression is created for.</param>
			public static Expression InvokeConvertToSourceMethod(DataBinding<T> binding)
			{
				return InvokeConverterMethod(binding, "ConvertToSource", ValueParameter);
			}

			/// <summary>
			///     Invokes the to target conversion function.
			/// </summary>
			/// <param name="binding">The binding the expression is created for.</param>
			/// <param name="methodParameter">The parameter for the converter method.</param>
			public static Expression InvokeConvertToTargetMethod(DataBinding<T> binding, Expression methodParameter)
			{
				return InvokeConverterMethod(binding, "ConvertToTarget", methodParameter);
			}

			/// <summary>
			///     Generates the expression that accesses the members of the binding's property path.
			/// </summary>
			/// <param name="binding">The binding the expression is created for.</param>
			public static Expression GetMemberAccessExpression(DataBinding<T> binding)
			{
				var expression = Expression.Convert(SourceObjectParameter, binding._sourceObject.GetType()) as Expression;
				expression = binding._memberAccess1.GetAccessExpression(expression);

				if (binding._memberAccessCount > 1)
					expression = binding._memberAccess2.GetAccessExpression(expression);

				if (binding._memberAccessCount > 2)
					expression = binding._memberAccess3.GetAccessExpression(expression);

				return expression;
			}
		}

		/// <summary>
		///     Provides information about a member access in the source expression.
		/// </summary>
		private struct MemberAccess
		{
			/// <summary>
			///     The name of the accessed property.
			/// </summary>
			private readonly string _propertyName;

			/// <summary>
			///     The strongly-typed changed handler that has been added for the dependency property.
			/// </summary>
			private Delegate _changeHandler;

			/// <summary>
			///     The dependency property that is accessed, if any.
			/// </summary>
			private DependencyProperty _dependencyProperty;

			/// <summary>
			///     The reflection info instance for the property that is accessed, if any.
			/// </summary>
			private PropertyInfo _propertyInfo;

			/// <summary>
			///     The source object that is accessed.
			/// </summary>
			private object _sourceObject;

			/// <summary>
			///     Initializes a new instance.
			/// </summary>
			/// <param name="propertyName">The name of the property that should be accessed.</param>
			public MemberAccess(string propertyName)
				: this()
			{
				Assert.ArgumentNotNullOrWhitespace(propertyName);
				_propertyName = propertyName;
			}

			/// <summary>
			///     Gets or sets the type of the value currently stored by the accessed property.
			/// </summary>
			public Type ValueType { get; set; }

			/// <summary>
			///     Gets the declared type of the accessed property.
			/// </summary>
			public Type PropertyType
			{
				get
				{
					if (_dependencyProperty != null)
						return _dependencyProperty.ValueType;

					if (_propertyInfo != null)
						return _propertyInfo.PropertyType;

					return null;
				}
			}

			/// <summary>
			///     Gets the name of the member that is accessed.
			/// </summary>
			public string MemberName
			{
				get { return _propertyName; }
			}

			/// <summary>
			///     Sets the change handler that is invoked when the value of the member has changed.
			/// </summary>
			public Action Changed { private get; set; }

			/// <summary>
			///     Sets the source object that is accessed.
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
			///     Gets the value of the accessed member.
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
			///     Gets the access expression for the accessed property or dependency property.
			/// </summary>
			/// <param name="expression">The expression to the left-hand side of the access expression.</param>
			public Expression GetAccessExpression(Expression expression)
			{
				return Expression.Convert(Expression.Property(expression, _propertyInfo), Value.GetType());
			}

			/// <summary>
			///     Attaches the instance to the source object's property changed event.
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
					{
						Log.Debug("Possibly unintended data binding: The source property is neither a dependency property, nor does " +
								  "the containing type implement INotifyPropertyChanged. Support for automatic value updates disabled.");
						return;
					}

					_changeHandler = (PropertyChangedHandler)PropertyChanged;
					ReflectionHelper.AttachPropertyChangedEventHandler(notifyPropertyChanged, (PropertyChangedHandler)_changeHandler);
				}
			}

			/// <summary>
			///     Detaches the instance from the source object's property changed event.
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

					ReflectionHelper.DetachPropertyChangedEventHandler(notifyPropertyChanged, (PropertyChangedHandler)_changeHandler);
				}
			}

			/// <summary>
			///     Gets the reflection info of the property or the instance of the dependency property that is accessed.
			/// </summary>
			private void GetReflectedProperty()
			{
				if (_sourceObject == null)
					return;

				if (_sourceObject is DependencyObject)
					_dependencyProperty = ReflectionHelper.GetDependencyProperty(_sourceObject.GetType(), _propertyName);

				var name = _propertyName;
				_propertyInfo = _sourceObject.GetType()
											 .GetRuntimeProperties()
											 .SingleOrDefault(p => p.GetMethod != null && p.GetMethod.IsPublic && !p.GetMethod.IsStatic && p.Name == name);

				if (_propertyInfo == null)
					Log.Die("Unable to find public, non-static property or dependency property '{0}' on '{1}'.",
							_propertyName, _sourceObject.GetType().FullName);
			}

			/// <summary>
			///     Handles a change notification for a property.
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
			///     Handles a change notification for a dependency property.
			/// </summary>
			/// <param name="obj">The dependency object the changed dependency property belongs to.</param>
			/// <param name="property">The dependency property that has changed.</param>
			private void DependencyPropertyChanged(DependencyObject obj, DependencyProperty property)
			{
				Assert.That(obj == _sourceObject, "Received an unexpected dependency property change notification.");
				Assert.NotNull(_dependencyProperty, "Received an unexpected dependency property change notification.");

				if (property == _dependencyProperty && Changed != null)
					Changed();
			}

			/// <summary>
			///     Deactivates the change handling of the member access.
			/// </summary>
			public void Remove()
			{
				DetachFromChangeEvent();
			}
		}
	}
}