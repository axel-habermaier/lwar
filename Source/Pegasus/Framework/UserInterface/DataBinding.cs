﻿using System;

namespace Pegasus.Framework.UserInterface
{
	using System.Linq.Expressions;
	using System.Reflection;
	using Platform.Logging;

	/// <summary>
	///   Binds a target dependency object/dependency property pair to a source object and path selector.
	/// </summary>
	/// <typeparam name="T">The type of the value that is bound.</typeparam>
	internal sealed class DataBinding<T> : Binding<T>
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
		///   Indicates whether the currently bound value is null.
		/// </summary>
		private bool _isNull;

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
		///   Initializes a new instance.
		/// </summary>
		/// <param name="sourceExpression">The expression that should be used to get the source value.</param>
		public DataBinding(Expression<Func<object, T>> sourceExpression)
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
		public DataBinding(object sourceObject, Expression<Func<object, T>> sourceExpression)
		{
			Assert.ArgumentNotNull(sourceObject);
			Assert.ArgumentNotNull(sourceExpression);

			_sourceObject = sourceObject;
			_sourceExpression = sourceExpression;
		}
		/// <summary>
		///   Initializes the binding.
		/// </summary>
		protected override void Initialize()
		{
			Assert.ArgumentSatisfies(!_targetProperty.IsDataBindingProhibited, "Data binding is not allowed on the target property.");
			Assert.That(!_sourceIsViewModel || _targetObject is UIElement,
						"No source object has been set; this is OK as long as the target object is an UIElement, " +
						"in which case the UIElement's view model becomes the source object.");

			if (_sourceIsViewModel)
			{
				_sourceExpression = SourceExpressionRewriter.Instance.Rewrite(_sourceExpression);
				_sourceObject = _targetObject;
			}

			AnalyzeExpression();

			_sourceFunc = _sourceExpression.Compile();
			_memberAccess1.SourceObject = _sourceObject;

			// In release builds, we unset the source expression to free up the memory; in debug builds, we need
			// the expression in case of errors
#if !DEBUG
			_sourceExpression = null;
#endif
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
				Log.Debug("DataBinding failure: Expression '{0}' encountered a null value when accessing '{1}'.",
						  _sourceExpression, memberAccess.MemberName);
#endif

			var value = memberAccess.Value;
			_isNull = value == null;

			if (_memberAccessCount > memberAccessCount)
				nextMemberAccess.SourceObject = value;

			--_isChanging;

			if (_isChanging == 0 && !_isNull)
				_targetObject.SetValue(_targetProperty, _sourceFunc(_sourceObject));
			else if (_isNull)
				_targetObject.SetValue(_targetProperty, _targetProperty.DefaultValue);
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
			///   Gets the name of the member that is accessed.
			/// </summary>
			public string MemberName
			{
				get { return _propertyInfo.Name; }
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