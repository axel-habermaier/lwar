namespace Pegasus.UserInterface
{
	using System;
	using System.Reflection;
	using Converters;
	using Platform.Logging;
	using Utilities;

	/// <summary>
	///     Binds a target dependency object/dependency property pair to a source object and a property path.
	/// </summary>
	/// <typeparam name="T">The type of the value that is bound.</typeparam>
	/// <remarks>With the current implementation of the data binding, a property path can only access up to three properties.</remarks>
	internal sealed class DataBinding<T> : Binding<T>
	{
		/// <summary>
		///     Indicates whether the first property in the property path is the data context of an UI element.
		/// </summary>
		private readonly bool _boundToDataContext;

		/// <summary>
		///     A cached method info.
		/// </summary>
		private readonly MethodInfo _convertToSourceMethodInfo;

		/// <summary>
		///     A cached method info.
		/// </summary>
		private readonly MethodInfo _convertToTargetMethodInfo;

		/// <summary>
		///     The converter that is used to convert the source value to the dependency property type.
		/// </summary>
		private readonly IValueConverter _converter;

		/// <summary>
		///     The fallback value that is used when the binding fails or a null value is bound.
		/// </summary>
		private readonly T _fallbackValue;

		/// <summary>
		///     Indicates whether there is an explicit fallback value for the binding. Otherwise, the default value of the dependency
		///     property is used.
		/// </summary>
		private readonly bool _hasFallbackValue;

		/// <summary>
		///     The number of member accesses in the source expression.
		/// </summary>
		private readonly int _memberAccessCount;

		/// <summary>
		///     The source object that is passed to the source expression in order to get the value that is set on the target
		///     property.
		/// </summary>
		private readonly object _sourceObject;

		/// <summary>
		///     Indicates the direction of the data flow of the data binding.
		/// </summary>
		private BindingMode _bindingMode;

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
		///     Indicates whether the property path contains a null value, not including the last accessed property.
		/// </summary>
		private bool _pathHasNullValue;

		/// <summary>
		///     The function that is used to get the value from the source.
		/// </summary>
		private Func<T> _sourceFunc;

		/// <summary>
		///     Indicates whether the value of the source property is null.
		/// </summary>
		private bool _sourceValueIsNull;

		/// <summary>
		///     The function that is used to set the value of the source.
		/// </summary>
		private Action<T> _targetFunc;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="sourceObject">The source object that should provide the value that is bound.</param>
		/// <param name="fallbackValue">The fallback value that should be used when the binding fails or a null value is bound.</param>
		/// <param name="bindingMode">Indicates the direction of the data flow of the data binding.</param>
		/// <param name="property1">The name of the first property in the property path.</param>
		/// <param name="property2">The name of the second property in the property path.</param>
		/// <param name="property3">The name of the third property in the property path.</param>
		/// <param name="converter">The converter that should be used to convert the source value to the dependency property type.</param>
		internal DataBinding(object sourceObject, T fallbackValue, BindingMode bindingMode,
							 string property1, string property2 = null, string property3 = null, IValueConverter converter = null)
			: this(sourceObject, bindingMode, fallbackValue, true, property1, property2, property3, converter)
		{
		}

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
							 string property1, string property2 = null, string property3 = null, IValueConverter converter = null)
			: this(sourceObject, bindingMode, default(T), false, property1, property2, property3, converter)
		{
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="sourceObject">The source object that should provide the value that is bound.</param>
		/// <param name="bindingMode">Indicates the direction of the data flow of the data binding.</param>
		/// <param name="fallbackValue">The fallback value that should be used when the binding fails or a null value is bound.</param>
		/// <param name="hasFallbackValue">Indicates whether the binding has an explicit fallback value.</param>
		/// <param name="property1">The name of the first property in the property path.</param>
		/// <param name="property2">The name of the second property in the property path.</param>
		/// <param name="property3">The name of the third property in the property path.</param>
		/// <param name="converter">The converter that should be used to convert the source value to the dependency property type.</param>
		private DataBinding(object sourceObject, BindingMode bindingMode, T fallbackValue, bool hasFallbackValue,
							string property1, string property2, string property3, IValueConverter converter)
		{
			Assert.ArgumentNotNull(sourceObject);
			Assert.ArgumentInRange(bindingMode);
			Assert.ArgumentNotNullOrWhitespace(property1);
			Assert.ArgumentSatisfies(property3 == null || property2 != null, "Property 2 must be non-null when property 3 is non-null.");

			if (converter != null)
			{
				_convertToSourceMethodInfo = GetType().GetMethod("ConvertToSource", BindingFlags.Instance | BindingFlags.NonPublic);
				_convertToTargetMethodInfo = GetType().GetMethod("ConvertToTarget", BindingFlags.Instance | BindingFlags.NonPublic);
			}

			_sourceObject = sourceObject;
			_converter = converter;
			_bindingMode = bindingMode;
			_memberAccessCount = property3 == null ? (property2 == null ? 1 : 2) : 3;
			_boundToDataContext = sourceObject is UIElement && property1 == "DataContext";
			_fallbackValue = fallbackValue;
			_hasFallbackValue = hasFallbackValue;

			_memberAccess1 = new MemberAccess(property1) { Changed = OnMember1Changed };

			if (_memberAccessCount >= 2)
				_memberAccess2 = new MemberAccess(property2) { Changed = OnMember2Changed };

			if (_memberAccessCount == 3)
				_memberAccess3 = new MemberAccess(property3) { Changed = OnMember3Changed };
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
		///     Gets the source object of the data binding. If the first property of the property path is the data context of the source
		///     UI element, the UI element's parent element is considered as the source.
		/// </summary>
		private object SourceObject
		{
			get
			{
				if (_boundToDataContext && ReferenceEquals(_targetProperty, UIElement.DataContextProperty))
					return ((UIElement)_sourceObject).LogicalParent;

				return _sourceObject;
			}
		}

		/// <summary>
		///     Gets the actual type of the last property in the property path.
		/// </summary>
		private Type ActualSourcePropertyType
		{
			get
			{
				if (_memberAccessCount == 1)
					return _memberAccess1.ValueType;

				if (_memberAccessCount == 2)
					return _memberAccess2.ValueType;

				if (_memberAccessCount == 3)
					return _memberAccess3.ValueType;

				Assert.NotReached("Unexpected number of member accesses.");
				return null;
			}
		}

		/// <summary>
		///     Gets the declared type of the last property in the property path.
		/// </summary>
		private Type DeclaredSourcePropertyType
		{
			get
			{
				if (_memberAccessCount == 1)
					return _memberAccess1.PropertyType;

				if (_memberAccessCount == 2)
					return _memberAccess2.PropertyType;

				if (_memberAccessCount == 3)
					return _memberAccess3.PropertyType;

				Assert.NotReached("Unexpected number of member accesses.");
				return null;
			}
		}

		/// <summary>
		///     Gets a delegate that can be used to retrieve the source value.
		/// </summary>
		private Func<TSource> GetSourceValueGetterDelegate<TSource>()
		{
			if (_memberAccessCount == 1)
				return _memberAccess1.GetGetterDelegate<TSource>();

			if (_memberAccessCount == 2)
				return _memberAccess2.GetGetterDelegate<TSource>();

			if (_memberAccessCount == 3)
				return _memberAccess3.GetGetterDelegate<TSource>();

			Assert.NotReached("Unexpected number of member accesses.");
			return null;
		}

		/// <summary>
		///     Gets a delegate that can be used to set the source value.
		/// </summary>
		private Action<TSource> GetSourceValueSetterDelegate<TSource>()
		{
			if (_memberAccessCount == 1)
				return _memberAccess1.GetSetterDelegate<TSource>();

			if (_memberAccessCount == 2)
				return _memberAccess2.GetSetterDelegate<TSource>();

			if (_memberAccessCount == 3)
				return _memberAccess3.GetSetterDelegate<TSource>();

			Assert.NotReached("Unexpected number of member accesses.");
			return null;
		}

		/// <summary>
		///     Invoked when the binding has been activated.
		/// </summary>
		protected override void Activate()
		{
			Assert.ArgumentSatisfies(!_targetProperty.IsDataBindingProhibited, "Data binding is not allowed on the target property.");

			// Check if the default binding mode of the target dependency property should be used
			if (_bindingMode == BindingMode.Default)
				_bindingMode = _targetProperty.DefaultBindingMode;

			if (_bindingMode != BindingMode.OneWay)
				_targetObject.AddChangedHandler(_targetProperty, OnTargetPropertyChanged);

			// Set the access types of the members
			_memberAccess1.SetAccessTypes(_memberAccessCount == 1, _bindingMode);
			_memberAccess2.SetAccessTypes(_memberAccessCount == 2, _bindingMode);
			_memberAccess3.SetAccessTypes(_memberAccessCount == 3, _bindingMode);

			_memberAccess1.SourceObject = SourceObject;
		}

		/// <summary>
		///     Removes the binding. The binding can decide to ignore the removal if it would be overwritten by a local value. True is
		///     returned to indicate that the binding was removed.
		/// </summary>
		/// <param name="overwrittenByLocalValue">Indicates whether the binding is removed because it was overriden by a local value.</param>
		internal override bool Deactivate(bool overwrittenByLocalValue = false)
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
		///     Creates the function that is used to get the source value.
		/// </summary>
		private void CreateSourceFunction()
		{
			if (_converter == null)
				_sourceFunc = GetSourceValueGetterDelegate<T>();
			else
				_sourceFunc = (Func<T>)_convertToTargetMethodInfo.MakeGenericMethod(ActualSourcePropertyType).Invoke(this, null);
		}

		/// <summary>
		///     Creates the function that is used to set the source value.
		/// </summary>
		private void CreateTargetFunction()
		{
			if (_converter == null)
				_targetFunc = GetSourceValueSetterDelegate<T>();
			else
				_targetFunc = (Action<T>)_convertToSourceMethodInfo.MakeGenericMethod(DeclaredSourcePropertyType).Invoke(this, null);
		}

		/// <summary>
		///     Converts the source value to the target type.
		/// </summary>
		/// <typeparam name="TActual">The value type.</typeparam>
		[UsedImplicitly]
		private Func<T> ConvertToTarget<TActual>()
		{
			var getter = GetSourceValueGetterDelegate<TActual>();
			return () => ((IValueConverter<TActual, T>)_converter).ConvertToTarget(getter());
		}

		/// <summary>
		///     Converts the target value to the source type.
		/// </summary>
		/// <typeparam name="TActual">The value type.</typeparam>
		[UsedImplicitly]
		private Action<T> ConvertToSource<TActual>()
		{
			var setter = GetSourceValueSetterDelegate<TActual>();
			return value => setter(((IValueConverter<TActual, T>)_converter).ConvertToSource(value));
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
			Log.DebugIf(Active && !_pathHasNullValue && _bindingMode != BindingMode.OneWayToSource && memberAccess.Value == null,
				"Data binding failure: Encountered a null value in property path '{0}' when accessing '{1}'.",
				PropertyPath, memberAccess.MemberName);

			if (memberAccessCount < _memberAccessCount)
			{
				var value = memberAccess.Value;
				_pathHasNullValue = value == null;
				_sourceFunc = null;
				_targetFunc = null;

				// If the value is not null and the type of a value somewhere in the middle of the path has changed,
				// we have to regenerate the source function
				if (!_pathHasNullValue)
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
				if (_bindingMode != BindingMode.OneWayToSource)
				{
					// We try to avoid getting the value of the property here via reflection when the property is 
					// of value type - changes to these properties are likely to occur often and we want to avoid  
					// boxing the value every time the property is changed.
					var propertyType = memberAccess.PropertyType;
					_sourceValueIsNull = propertyType == null || (!propertyType.IsValueType && memberAccess.Value == null);

					if (!_sourceValueIsNull && propertyType != null && !propertyType.IsValueType)
					{
						var type = memberAccess.Value.GetType();

						if (type != memberAccess.ValueType)
						{
							_sourceFunc = null;
							_targetFunc = null;
							memberAccess.ValueType = type;
						}
					}
					else if (propertyType != null && propertyType.IsValueType)
						memberAccess.ValueType = propertyType;
				}

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

			if (_sourceFunc == null && !_pathHasNullValue && !_sourceValueIsNull)
				CreateSourceFunction();

			if (!_pathHasNullValue && !_sourceValueIsNull)
				_targetObject.SetBoundValue(_targetProperty, _sourceFunc());
			else
				_targetObject.SetBoundValue(_targetProperty, _hasFallbackValue ? _fallbackValue : _targetProperty.DefaultValue);
		}

		/// <summary>
		///     Updates the source property with the current target value.
		/// </summary>
		private void UpdateSourceProperty()
		{
			if (!Active || _bindingMode == BindingMode.OneWay || _pathHasNullValue)
				return;

			if (_targetFunc == null)
				CreateTargetFunction();

			// The target function might still be null if a property could not be found on the path
			if (_targetFunc != null)
				_targetFunc(_targetObject.GetValue(_targetProperty));
		}

		/// <summary>
		///     Gets the instance of the dependency property with the given name or null if it could not be found.
		/// </summary>
		/// <param name="type">The type of the dependency object that declares the dependency property.</param>
		/// <param name="propertyName">The name of the dependency property without the 'Property' suffix.</param>
		private static DependencyProperty GetDependencyProperty(Type type, string propertyName)
		{
			Assert.ArgumentNotNull(type);
			Assert.ArgumentNotNullOrWhitespace(propertyName);

			var fieldName = String.Format("{0}Property", propertyName);
			var propertyField = type.GetRuntimeField(fieldName);
			if (propertyField != null && (!propertyField.IsStatic || !propertyField.IsPublic))
				propertyField = null;

			// For some reason, inherited static fields are not returned by GetRuntimeFields(), so let's check the base
			// types explicitly if we didn't find a matching dependency property field on the current type
			var baseType = type.GetTypeInfo().BaseType;
			if (propertyField == null && baseType != typeof(object))
				return GetDependencyProperty(baseType, propertyName);

			Assert.NotNull(propertyField, "Unable to find dependency property '{0}' on '{1}'.", propertyName, type.FullName);
			return (DependencyProperty)propertyField.GetValue(null);
		}

		/// <summary>
		///     Provides information about a member access in the source expression.
		/// </summary>
		private struct MemberAccess
		{
			/// <summary>
			///     A cached method info instance.
			/// </summary>
			private static readonly MethodInfo ImplicitStringConversionMethodInfo =
				typeof(MemberAccess).GetMethod("ImplicitStringConversion", BindingFlags.NonPublic | BindingFlags.Instance);

			/// <summary>
			///     A cached method info instance.
			/// </summary>
			private static readonly MethodInfo DownCastGetterMethodInfo =
				typeof(MemberAccess).GetMethod("DownCastGetter", BindingFlags.NonPublic | BindingFlags.Instance);

			/// <summary>
			///     A cached method info instance.
			/// </summary>
			private static readonly MethodInfo UpCastGetterMethodInfo =
				typeof(MemberAccess).GetMethod("UpCastGetter", BindingFlags.NonPublic | BindingFlags.Instance);

			/// <summary>
			///     A cached method info instance.
			/// </summary>
			private static readonly MethodInfo UpCastSetterMethodInfo =
				typeof(MemberAccess).GetMethod("UpCastSetter", BindingFlags.NonPublic | BindingFlags.Instance);

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
			///     Indicates whether the accessed member is read.
			/// </summary>
			private bool _isRead;

			/// <summary>
			///     Indicates whether the accessed member is written.
			/// </summary>
			private bool _isWritten;

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
					if (_sourceObject == null || (_dependencyProperty == null && _propertyInfo == null))
						return null;

					if (_dependencyProperty == null)
						return _propertyInfo.GetValue(_sourceObject);

					var dependencyObject = _sourceObject as DependencyObject;
					Assert.NotNull(dependencyObject, "Trying to use a dependency property on a type not derived from DependencyObject.");

					return _dependencyProperty.GetValue(dependencyObject);
				}
			}

			/// <summary>
			///     Sets the access types of the member.
			/// </summary>
			/// <param name="isLast">Indicates whether the property is the last one of the property path.</param>
			/// <param name="bindingMode">The binding mode of the data binding.</param>
			public void SetAccessTypes(bool isLast, BindingMode bindingMode)
			{
				_isRead = !isLast || bindingMode != BindingMode.OneWayToSource;
				_isWritten = isLast && bindingMode != BindingMode.OneWay;
			}

			/// <summary>
			///     Gets a delegate that can be used to get the property's value.
			/// </summary>
			public Func<TProperty> GetGetterDelegate<TProperty>()
			{
				Assert.That(_isRead, "Property cannot be read.");
				Assert.That(_propertyInfo != null, "Unknown property.");
				Assert.NotNull(_sourceObject);

				if (typeof(TProperty) == PropertyType)
					return GetGetter<TProperty>();

				if (PropertyType != typeof(string) && typeof(TProperty) == typeof(string))
					return (Func<TProperty>)ImplicitStringConversionMethodInfo.MakeGenericMethod(PropertyType).Invoke(this, null);

				if (PropertyType.IsAssignableFrom(typeof(TProperty)))
					return (Func<TProperty>)DownCastGetterMethodInfo.MakeGenericMethod(PropertyType, typeof(TProperty)).Invoke(this, null);

				return (Func<TProperty>)UpCastGetterMethodInfo.MakeGenericMethod(PropertyType, typeof(TProperty)).Invoke(this, null);
			}

			/// <summary>
			///     Gets a delegate that can be used to set the property's value.
			/// </summary>
			public Action<TProperty> GetSetterDelegate<TProperty>()
			{
				Assert.That(_isWritten, "Property cannot be written.");

				if (_propertyInfo == null)
					return null;

				if (typeof(TProperty) != PropertyType && !PropertyType.IsAssignableFrom(typeof(TProperty)))
					return (Action<TProperty>)UpCastSetterMethodInfo.MakeGenericMethod(PropertyType, typeof(TProperty)).Invoke(this, null);

				return GetSetter<TProperty>();
			}

			/// <summary>
			///     Gets a delegate for the getter of the bound property.
			/// </summary>
			/// <typeparam name="TProperty">The declared type of the property.</typeparam>
			private Func<TProperty> GetGetter<TProperty>()
			{
				var getter = _sourceObject.GetType().GetProperty(_propertyName, BindingFlags.Public | BindingFlags.Instance).GetGetMethod();
				Assert.NotNull(getter, "Cannot bind to the property, as it has no public getter.");

				return (Func<TProperty>)Delegate.CreateDelegate(typeof(Func<TProperty>), _sourceObject, getter);
			}

			/// <summary>
			///     Gets a delegate for the setter of the bound property.
			/// </summary>
			/// <typeparam name="TProperty">The declared type of the property.</typeparam>
			private Action<TProperty> GetSetter<TProperty>()
			{
				var setter = _sourceObject.GetType().GetProperty(_propertyName, BindingFlags.Public | BindingFlags.Instance).GetSetMethod();
				return (Action<TProperty>)Delegate.CreateDelegate(typeof(Action<TProperty>), _sourceObject, setter);
			}

			/// <summary>
			///     Converts the value obtained from the property to a string.
			/// </summary>
			/// <typeparam name="TProperty">The declared type of the property.</typeparam>
			[UsedImplicitly]
			private Func<string> ImplicitStringConversion<TProperty>()
			{
				var getter = GetGetter<TProperty>();
				return () => getter().ToString();
			}

			/// <summary>
			///     Down casts the value obtained from the property to the given type.
			/// </summary>
			/// <typeparam name="TDeclared">The declared type of the property.</typeparam>
			/// <typeparam name="TActual">The actual type the property value should be cast to.</typeparam>
			[UsedImplicitly]
			private Func<TActual> DownCastGetter<TDeclared, TActual>() where TActual : TDeclared
			{
				var getter = GetGetter<TDeclared>();
				return () => (TActual)getter();
			}

			/// <summary>
			///     Up casts the value obtained from the property to the given type.
			/// </summary>
			/// <typeparam name="TDeclared">The declared type of the property.</typeparam>
			/// <typeparam name="TActual">The actual type the property value should be cast to.</typeparam>
			[UsedImplicitly]
			private Func<TActual> UpCastGetter<TDeclared, TActual>() where TDeclared : TActual
			{
				var getter = GetGetter<TDeclared>();
				return () => (TActual)getter();
			}

			/// <summary>
			///     Up casts the value obtained from the property to the given type.
			/// </summary>
			/// <typeparam name="TDeclared">The declared type of the property.</typeparam>
			/// <typeparam name="TActual">The actual type the property value should be cast to.</typeparam>
			[UsedImplicitly]
			private Action<TActual> UpCastSetter<TDeclared, TActual>() where TDeclared : TActual
			{
				var setter = GetSetter<TDeclared>();
				return value => setter((TDeclared)value);
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
				else if (_propertyInfo != null)
				{
					var notifyPropertyChanged = _sourceObject as INotifyPropertyChanged;
					if (notifyPropertyChanged == null)
						return;

					_changeHandler = (PropertyChangedHandler)PropertyChanged;
					notifyPropertyChanged.PropertyChanged += (PropertyChangedHandler)_changeHandler;
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
				else if (_propertyInfo != null)
				{
					var notifyPropertyChanged = _sourceObject as INotifyPropertyChanged;
					if (notifyPropertyChanged != null)
						notifyPropertyChanged.PropertyChanged -= (PropertyChangedHandler)_changeHandler;
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
					_dependencyProperty = GetDependencyProperty(_sourceObject.GetType(), _propertyName);

				var name = _propertyName;
				_propertyInfo = _sourceObject.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public);

				Log.DebugIf(_propertyInfo == null, "Unable to find public, non-static property '{0}' on '{1}'.",
					_propertyName, _sourceObject.GetType().FullName);

				if (_propertyInfo == null)
					return;

				var method = _propertyInfo.CanRead ? _propertyInfo.GetMethod : _propertyInfo.SetMethod;
				Assert.That(!method.IsStatic, "Cannot data bind to static property '{0}' on '{1};.",
					_propertyName, _sourceObject.GetType().FullName);

				Assert.That(!_isRead || _propertyInfo.CanRead, "Cannot read non-readable property.");
				Assert.That(!_isWritten || _propertyInfo.CanWrite, "Cannot write to non-writable property.");
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