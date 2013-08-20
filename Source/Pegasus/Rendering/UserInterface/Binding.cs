using System;

namespace Pegasus.Framework.Rendering.UserInterface
{
	using System.Linq.Expressions;
	using System.Reflection;

	/// <summary>
	///   Binds a target dependency object/dependency property pair to a source object and path selector.
	/// </summary>
	/// <typeparam name="T">The type of the value that is bound.</typeparam>
	public class Binding<T>
	{
		/// <summary>
		///   Indicates whether the view model of the bound UI element is the source object.
		/// </summary>
		private readonly bool _sourceIsViewModel;

		/// <summary>
		///   If greater than 0, the properties accessed by the source expression are currently changing.
		/// </summary>
		private byte _isChanging;

		/// <summary>
		/// Gets a value indicating whether the binding has already been bound to a dependency property.
		/// </summary>
		public bool IsBound { get; private set; }

		/// <summary>
		///   Provides information about the first member access (such as 'a.b') in a source expression 'a.b.c.d'.
		/// </summary>
		/// <remarks>
		///   We do not use an array to store the member access, but rather use a hard-coded limit to save possibly numerous
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
		///   The expression that is used to get the value from the source object.
		/// </summary>
		private Expression<Func<object, T>> _sourceExpression;

		/// <summary>
		///   The compiled expression that is used to get the value from the source object.
		/// </summary>
		private Func<object, T> _sourceFunc;

		/// <summary>
		///   The source object that is passed to the source expression in order to get the value that is set on the target
		///   property.
		/// </summary>
		private object _sourceObject;

		/// <summary>
		///   The target dependency object that defines the target dependency property.
		/// </summary>
		private DependencyObject _targetObject;

		/// <summary>
		///   The target dependency property whose value is bound.
		/// </summary>
		private DependencyProperty<T> _targetProperty;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="sourceExpression">The expression that should be used to get the source value.</param>
		public Binding(Expression<Func<object, T>> sourceExpression)
		{
			Assert.ArgumentNotNull(sourceExpression);

			_sourceExpression = sourceExpression;
			_sourceIsViewModel = true;
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="sourceObject">The source object that should provide the value that is bound.</param>
		/// <param name="sourceExpression">The expression that should be used to get the source value.</param>
		public Binding(object sourceObject, Expression<Func<object, T>> sourceExpression)
		{
			Assert.ArgumentNotNull(sourceObject);
			Assert.ArgumentNotNull(sourceExpression);

			_sourceObject = sourceObject;
			_sourceExpression = sourceExpression;
		}

		/// <summary>
		///   Initializes the binding.
		/// </summary>
		/// <param name="targetObject">The target dependency object that defines the target dependency property.</param>
		/// <param name="targetProperty">The target dependency property whose value should be bound.</param>
		internal void Initialize(DependencyObject targetObject, DependencyProperty<T> targetProperty)
		{
			Assert.ArgumentNotNull(targetObject);
			Assert.ArgumentNotNull(targetProperty);
			Assert.That(!IsBound, "The binding has already been bound to a dependency property.");
			Assert.That(!_sourceIsViewModel || targetObject is UIElement,
						"No source object has been set; this is OK as long as the target object is an UIElement, " +
						"in which case the UIElement's view model becomes the source object.");

			_targetObject = targetObject;
			_targetProperty = targetProperty;

			if (_sourceIsViewModel)
			{
				_sourceExpression = SourceExpressionRewriter.Instance.Rewrite(_sourceExpression);
				_sourceObject = _targetObject;
			}

			AnalyzeExpression();

			_sourceFunc = _sourceExpression.Compile();
			_memberAccess1.SourceObject = _sourceObject;

			IsBound = true;
		}

		/// <summary>
		///   Analyzes the source expression and generates the member access infos.
		/// </summary>
		private void AnalyzeExpression()
		{
			// Analyze the source expression
			var analyzer = SourceExpressionAnalyzer.Instance;
			var memberAccesses = analyzer.Analyze(_sourceExpression);

			// Store a copy of the member access infos
			_memberAccessCount = (byte)memberAccesses.Count;
			switch (memberAccesses.Count)
			{
				case 3:
					_memberAccess3 = memberAccesses.Array[0];
					_memberAccess2 = memberAccesses.Array[1];
					_memberAccess1 = memberAccesses.Array[2];

					_memberAccess1.Changed = OnMember1Changed;
					_memberAccess2.Changed = OnMember2Changed;
					_memberAccess3.Changed = OnMember3Changed;
					break;
				case 2:
					_memberAccess2 = memberAccesses.Array[0];
					_memberAccess1 = memberAccesses.Array[1];

					_memberAccess1.Changed = OnMember1Changed;
					_memberAccess2.Changed = OnMember2Changed;
					break;
				case 1:
					_memberAccess1 = memberAccesses.Array[0];
					_memberAccess1.Changed = OnMember1Changed;
					break;
				case 0:
					Assert.That(false, "The source expression does not access any members.");
					break;
				default:
					throw new InvalidOperationException("Unsupported number of member accesses.");
			}
		}

		/// <summary>
		///   Invoked when the first accessed member changed.
		/// </summary>
		private void OnMember1Changed()
		{
			++_isChanging;

			if (_memberAccessCount > 1)
				_memberAccess2.SourceObject = _memberAccess1.Value;

			--_isChanging;

			UpdateTargetProperty();
		}

		/// <summary>
		///   Invoked when the second accessed member changed.
		/// </summary>
		private void OnMember2Changed()
		{
			++_isChanging;

			if (_memberAccessCount > 2)
				_memberAccess3.SourceObject = _memberAccess2.Value;

			--_isChanging;

			UpdateTargetProperty();
		}

		/// <summary>
		///   Invoked when the third accessed member changed.
		/// </summary>
		private void OnMember3Changed()
		{
			UpdateTargetProperty();
		}

		/// <summary>
		///   Updates the target property, setting it to the current source value.
		/// </summary>
		private void UpdateTargetProperty()
		{
			if (_isChanging == 0)
				_targetObject.SetValue(_targetProperty, _sourceFunc(_sourceObject));
		}

		/// <summary>
		///   Provides information about a member access in the source expression.
		/// </summary>
		private struct MemberAccess
		{
			/// <summary>
			///   The reflection info instance for the property that is accessed, if any.
			/// </summary>
			private readonly PropertyInfo _propertyInfo;

			/// <summary>
			///   The strongly-typed changed handler that has been added for the dependency property.
			/// </summary>
			private Delegate _changeHandler;

			/// <summary>
			///   The dependency property that is accessed, if any.
			/// </summary>
			private DependencyProperty _dependencyProperty;

			/// <summary>
			///   The source object that is accessed.
			/// </summary>
			private object _sourceObject;

			/// <summary>
			///   Initializes a new instance.
			/// </summary>
			/// <param name="propertyInfo">The reflection info instance for the property that is accessed.</param>
			public MemberAccess(PropertyInfo propertyInfo)
				: this()
			{
				Assert.ArgumentNotNull(propertyInfo);
				_propertyInfo = propertyInfo;
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
					Assert.NotNull(value, "A binding expression returned null.");

					if (_sourceObject == value)
						return;

					DetachFromChangeEvent();

					_sourceObject = value;
					_dependencyProperty = null;

					GetDependencyProperty();
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
					if (_dependencyProperty == null)
						return _propertyInfo.GetValue(_sourceObject);

					var dependencyObject = _sourceObject as DependencyObject;
					Assert.NotNull(dependencyObject, "Trying to use a dependency property on a type not derived from DependencyObject.");

					return _dependencyProperty.GetValue(dependencyObject);
				}
			}

			/// <summary>
			///   Attaches the instance to the source object's property changed event.
			/// </summary>
			private void AttachToChangeEvent()
			{
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
			///   Gets the dependency property instance if the accessed member is a dependency property.
			/// </summary>
			private void GetDependencyProperty()
			{
				if (!(_sourceObject is DependencyObject))
					return;

				_dependencyProperty = ReflectionHelper.GetDependencyProperty(_sourceObject.GetType(), _propertyInfo.Name);
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

		/// <summary>
		///   Analyzes a source expression of a binding.
		/// </summary>
		private class SourceExpressionAnalyzer : MemberAccessExpressionVisitor
		{
			/// <summary>
			///   The singleton instance of the source expression analyzer.
			/// </summary>
			public static readonly SourceExpressionAnalyzer Instance = new SourceExpressionAnalyzer();

			/// <summary>
			///   Provides information about the member accesses (such as 'a.b') in a source expression 'a.b.c.d'. The member access at
			///   index 0 corresponds to 'c.d', the one at index 1 corresponds to 'b.c', etc.
			/// </summary>
			private readonly MemberAccess[] _memberAccesses = new MemberAccess[3];

			/// <summary>
			///   The number of member accesses in the source expression.
			/// </summary>
			private int _memberAccessCount;

			/// <summary>
			///   Indicates whether the source object has been found in the expression.
			/// </summary>
			private bool _sourceObjectFound;

			/// <summary>
			///   Initializes a new instance.
			/// </summary>
			private SourceExpressionAnalyzer()
			{
			}

			/// <summary>
			///   Analyzes the source expression and returns the member accesses.
			/// </summary>
			/// <param name="expression">The expression that should be analyzed.</param>
			public ArraySegment<MemberAccess> Analyze(Expression<Func<object, T>> expression)
			{
				Assert.ArgumentNotNull(expression);

				_sourceObjectFound = false;
				_memberAccessCount = 0;

				Visit(expression.Body);
				return new ArraySegment<MemberAccess>(_memberAccesses, 0, _memberAccessCount);
			}

			/// <summary>
			///   Visits an unary expression. Only conversion expressions (casts) are supported.
			/// </summary>
			protected override Expression VisitUnary(UnaryExpression expression)
			{
				Assert.ArgumentSatisfies(expression.NodeType == ExpressionType.Convert, "Unsupported unary expression.");
				return base.VisitUnary(expression);
			}

			/// <summary>
			///   Visits a parameter access expression. Only lambda function parameter accesses are supported.
			/// </summary>
			protected override Expression VisitParameter(ParameterExpression expression)
			{
				_sourceObjectFound = true;
				return base.VisitParameter(expression);
			}

			/// <summary>
			///   Visits a member access expression. Only property accesses are supported.
			/// </summary>
			protected override Expression VisitMember(MemberExpression expression)
			{
				Assert.ArgumentSatisfies(expression.Member.MemberType == MemberTypes.Property, "Unsupported non-property member access.");
				Assert.That(!_sourceObjectFound, "Found a member access that is not transitively connected to the source object.");
				Assert.That(_memberAccessCount + 1 <= _memberAccesses.Length, "Too many member accesses in source expression.");

				_memberAccesses[_memberAccessCount] = new MemberAccess((PropertyInfo)expression.Member);
				++_memberAccessCount;

				return base.VisitMember(expression);
			}
		}

		/// <summary>
		///   Rewrites the source expression of a binding such that the first member access is UIElement's ViewModel property.
		/// </summary>
		private class SourceExpressionRewriter : MemberAccessExpressionVisitor
		{
			/// <summary>
			///   The singleton instance of the source expression analyzer.
			/// </summary>
			public static readonly SourceExpressionRewriter Instance = new SourceExpressionRewriter();

			/// <summary>
			///   The expression that is used to access the view model.
			/// </summary>
			private MemberExpression _viewModelAccess;

			/// <summary>
			///   Analyzes the source expression and returns the member accesses.
			/// </summary>
			/// <param name="expression">The expression that should be rewritten.</param>
			public Expression<Func<object, T>> Rewrite(Expression<Func<object, T>> expression)
			{
				Assert.ArgumentNotNull(expression);

				var convertedParameter = Expression.Convert(expression.Parameters[0], typeof(UIElement));
				_viewModelAccess = Expression.MakeMemberAccess(convertedParameter, ReflectionHelper.ViewModelPropertyInfo);

				return Expression.Lambda<Func<object, T>>(Visit(expression.Body), expression.Parameters);
			}

			/// <summary>
			///   Visits a parameter access expression. Only lambda function parameter accesses are supported.
			/// </summary>
			protected override Expression VisitParameter(ParameterExpression expression)
			{
				return _viewModelAccess;
			}
		}
	}
}